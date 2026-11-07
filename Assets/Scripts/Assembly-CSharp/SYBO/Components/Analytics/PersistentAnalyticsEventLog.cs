using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using SYBO.Components.Compression;
using SYBO.Components.Crypt;
using SYBO.Components.Logging;
using UnityEngine;

namespace SYBO.Components.Analytics
{
	public class PersistentAnalyticsEventLog
	{
		public const string LogFilePrefix = "Sybo_Analytics";

		private const int DefaultQueueSize = 16;

		private const byte LogBegin = 91;

		private const byte LogSeperator = 44;

		private const byte LogEnd = 93;

		private const string WriteFilename = "Sybo_Analytics_Buffer.log";

		private const string ChunkFilenameFormat = "Sybo_Analytics_Chunk_{0}.log";

		private string _writeFilePath;

		private string _chunkPathFormat;

		private int _chunkSizeThresholdBytes;

		private int _maxChunkCount;

		private long _storageLimitBytes;

		private MD5 _md5;

		private Queue<byte[]> _events;

		private EncryptedCTRStream _writeStream;

		private AutoResetEvent _workerThreadEvent;

		private volatile int _chunkCount;

		private volatile bool _full;

		private volatile Thread _workerThread;

		private volatile bool _workerThreadRunning;

		private volatile bool _consumeChunk;

		private volatile bool _isWorking;

		private volatile AsyncRes _getChunkAsyncRes;

		private long _chunkSizes;

		public string LogWriteFilePath
		{
			get
			{
				return _writeFilePath;
			}
		}

		public bool HasData { get; private set; }

		public bool IsWorking
		{
			get
			{
				return _isWorking;
			}
		}

		private bool HasWorkerThread
		{
			get
			{
				return _workerThread != null;
			}
		}

		public PersistentAnalyticsEventLog(string storagePath, int chunkSizeThresholdKB, int storageLimitKB)
		{
			if (!Directory.Exists(storagePath))
			{
				Directory.CreateDirectory(storagePath);
			}
			_writeFilePath = Path.Combine(storagePath, "Sybo_Analytics_Buffer.log");
			_chunkPathFormat = Path.Combine(storagePath, "Sybo_Analytics_Chunk_{0}.log");
			_chunkSizeThresholdBytes = chunkSizeThresholdKB * 1024;
			_storageLimitBytes = storageLimitKB * 1024;
			_maxChunkCount = (int)(_storageLimitBytes / _chunkSizeThresholdBytes) + 1;
			_chunkCount = 0;
			for (int i = 0; i < _maxChunkCount; i++)
			{
				string chunkFilename = GetChunkFilename(i);
				if (File.Exists(chunkFilename))
				{
					if (i != _chunkCount)
					{
						File.Move(chunkFilename, GetChunkFilename(i));
					}
					_chunkCount++;
				}
			}
			UpdateChunkSizes();
			HasData = _chunkCount > 0 || File.Exists(LogWriteFilePath);
			_md5 = MD5.Create();
		}

		public void Wipe()
		{
			for (int i = 0; i < _maxChunkCount * 2; i++)
			{
				if (File.Exists(GetChunkFilename(i)))
				{
					File.Delete(GetChunkFilename(i));
				}
			}
			_chunkCount = 0;
			CloseWriteStream();
			if (File.Exists(LogWriteFilePath))
			{
				File.Delete(LogWriteFilePath);
			}
			HasData = false;
		}

		public void Append(string eventData)
		{
			if (eventData.Length > _chunkSizeThresholdBytes)
			{
				Log.Analytics.LogErrorFormat("Trying to add supersized analytics event to log! Event data is {0} bytes, max chunk size {2}, content:\n{1}", eventData.Length, eventData, _chunkSizeThresholdBytes);
			}
			else if (!_full)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(eventData);
				StartWorkerThreadIfNeeded();
				lock (_events)
				{
					HasData = true;
					_events.Enqueue(bytes);
				}
				WakeWorkerThread();
			}
		}

		public void Shutdown()
		{
			if (_workerThread != null && _workerThreadRunning)
			{
				_workerThreadRunning = false;
				_workerThreadEvent.Set();
				while (_workerThread != null)
				{
				}
				CloseWriteStream();
			}
		}

		public void Replace(string current, string replacement)
		{
			if (HasData)
			{
				throw new NotImplementedException();
			}
		}

		public void ConsumeChunk()
		{
			_consumeChunk = true;
			WakeWorkerThread();
		}

		public AsyncRes GetNextChunkForServerAsync()
		{
			if (_getChunkAsyncRes != null)
			{
				throw new Exception("Sybo Analytics: Only one GetNextChunkForServerAsync request allowed at a time.");
			}
			AsyncRes result = (_getChunkAsyncRes = new AsyncRes());
			WakeWorkerThread();
			return result;
		}

		public string GetMD5Hash(byte[] data)
		{
			return Convert.ToBase64String(_md5.ComputeHash(data));
		}

		private void WorkerThreadLoop(object ignored)
		{
			bool flag = false;
			while (_workerThreadRunning)
			{
				bool flag2 = true;
				byte[] array = null;
				lock (_events)
				{
					if (_events.Count > 0)
					{
						array = _events.Dequeue();
					}
				}
				try
				{
					if (array == null)
					{
						goto IL_00e7;
					}
					flag2 = false;
					if (!GetWriteStream(true))
					{
						if (!flag)
						{
							flag = true;
						}
						continue;
					}
					_writeStream.Write(array, 0, array.Length);
					if (_writeStream.Length > _chunkSizeThresholdBytes || _writeStream.Length + _chunkSizes >= _storageLimitBytes)
					{
						_writeStream.WriteByte(93);
						CloseWriteStream();
						File.Move(LogWriteFilePath, GetChunkFilename(_chunkCount++));
						UpdateChunkSizes();
					}
					goto IL_00e7;
					IL_00e7:
					if (_consumeChunk || _getChunkAsyncRes != null)
					{
						if (_consumeChunk)
						{
							flag2 = false;
							_consumeChunk = false;
							string text = GetChunkFilename(0);
							File.Delete(text);
							for (int i = 1; i < _chunkCount; i++)
							{
								string chunkFilename = GetChunkFilename(i);
								File.Move(chunkFilename, text);
								text = chunkFilename;
							}
							_chunkCount--;
							UpdateChunkSizes();
							if (_chunkCount == 0)
							{
								lock (_events)
								{
									HasData = _writeStream != null || _events.Count > 0;
								}
							}
							if (_full)
							{
								_full = false;
								flag = false;
							}
						}
						else if (_getChunkAsyncRes != null)
						{
							flag2 = false;
							if (_chunkCount == 0 && HasData)
							{
								lock (_events)
								{
									if (_events.Count > 0)
									{
										continue;
									}
								}
								if (GetWriteStream(false))
								{
									_writeStream.WriteByte(93);
									CloseWriteStream();
									File.Move(LogWriteFilePath, GetChunkFilename(_chunkCount++));
								}
							}
							if (_chunkCount > 0)
							{
								byte[] array2 = ReadChunk();
								_getChunkAsyncRes.ChunkLength = array2.Length;
								if (Log.Analytics.Level >= 3)
								{
								}
								_getChunkAsyncRes.UncompressedMD5Hash = GetMD5Hash(array2);
								_getChunkAsyncRes.Result = GZip.CompressBytes(array2);
							}
							_getChunkAsyncRes.IsReady = true;
							_getChunkAsyncRes = null;
						}
					}
					if (flag2 && !_workerThreadEvent.WaitOne(0))
					{
						_isWorking = false;
						_workerThreadEvent.WaitOne();
						_isWorking = true;
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			_workerThread = null;
			_isWorking = false;
		}

		private void UpdateChunkSizes()
		{
			_chunkSizes = 0L;
			for (int i = 0; i < _chunkCount; i++)
			{
				string chunkFilename = GetChunkFilename(i);
				if (File.Exists(chunkFilename))
				{
					FileInfo fileInfo = new FileInfo(chunkFilename);
					_chunkSizes += fileInfo.Length;
				}
			}
			if (_chunkSizes >= _storageLimitBytes)
			{
				_full = true;
			}
		}

		private string GetChunkFilename(int chunkIdx)
		{
			return string.Format(_chunkPathFormat, chunkIdx);
		}

		private bool GetWriteStream(bool openForAppend)
		{
			if (_writeStream == null)
			{
				if (_chunkCount >= _maxChunkCount)
				{
					_full = true;
					return false;
				}
				if (File.Exists(LogWriteFilePath))
				{
					FileStream stream = new FileStream(LogWriteFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
					_writeStream = EncryptedCTRStream.WriteStream(stream);
					if (openForAppend)
					{
						_writeStream.WriteByte(44);
					}
				}
				else
				{
					FileStream stream2 = new FileStream(LogWriteFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
					_writeStream = EncryptedCTRStream.WriteStream(stream2);
					_writeStream.WriteByte(91);
				}
			}
			else if (openForAppend)
			{
				_writeStream.WriteByte(44);
			}
			return true;
		}

		private Stream GetChunkReader()
		{
			FileStream stream = new FileStream(GetChunkFilename(0), FileMode.Open, FileAccess.Read);
			return EncryptedCTRStream.ReadStream(stream);
		}

		private void CloseWriteStream()
		{
			if (_writeStream != null)
			{
				_writeStream.Flush();
				_writeStream.Dispose();
				_writeStream = null;
			}
		}

		private void WakeWorkerThread()
		{
			StartWorkerThreadIfNeeded();
			_isWorking = true;
			_workerThreadEvent.Set();
		}

		private void StartWorkerThreadIfNeeded()
		{
			if (!HasWorkerThread)
			{
				_workerThreadRunning = true;
				_isWorking = true;
				_events = new Queue<byte[]>(16);
				_workerThreadEvent = new AutoResetEvent(false);
				_workerThread = new Thread(WorkerThreadLoop);
				_workerThread.Start();
			}
		}

		private byte[] ReadChunk()
		{
			using (Stream stream = GetChunkReader())
			{
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, (int)stream.Length);
				return array;
			}
		}
	}
}
