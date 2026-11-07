using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Data.Save.Internal
{
	public class SaveData<T> where T : DiskData, new()
	{
		private const string SECRET = "";

		private string _path;

		private T _diskData;

		private IBsonSerializer _serializer;

		private byte[] _lastSaved;

		public T diskData
		{
			get
			{
				return _diskData;
			}
		}

		public string debug_dataSerialized
		{
			get
			{
				_diskData.Deobfuscate();
				IJsonSerializer serializer = new KilooSerializer();
				string result = serializer.ToJson(_diskData);
				_diskData.Obfuscate();
				return result;
			}
			set
			{
				IJsonSerializer serializer = new KilooSerializer();
				T val = serializer.FromJson<T>(value);
				val.Obfuscate();
				_diskData = val;
			}
		}

		public SaveData(string path)
		{
			_path = path;
			_diskData = new T();
			_serializer = new KilooSerializer();
		}

		public SaveData(string path, T diskData)
		{
			_path = path;
			_diskData = diskData;
			_serializer = new KilooSerializer();
		}

		public void Save()
		{
			if (!string.IsNullOrEmpty(_path))
			{
				SaveIfDirty();
			}
		}

		private void SaveIfDirty()
		{
			_diskData.Deobfuscate();
			try
			{
				byte[] array = _serializer.ToBson(_diskData);
				if (_lastSaved == null || !_lastSaved.SequenceEqual(array))
				{
					FileUtil.SafeSave(_path, array, string.Empty);
				}
				_lastSaved = array;
			}
			catch (Exception ex)
			{
				SDebug.LogError("Error while saving data: " + ex);
				Flurry.LogEventWithAParameter("SaveData save error", "Exception Type", ex.ToString());
			}
			_diskData.Obfuscate();
		}

		public void Load()
		{
			if (string.IsNullOrEmpty(_path))
			{
				return;
			}
			try
			{
				byte[] array = FileUtil.Load(_path, string.Empty);
				if (array != null)
				{
					T val = _serializer.FromBson<T>(array);
					_diskData = val;
					_diskData.Obfuscate();
				}
			}
			catch (Exception ex)
			{
				if (ex is FileNotFoundException)
				{
					string key = "savedata_filenotfound_once-" + typeof(T);
					if (!PlayerPrefs.HasKey(key))
					{
						PlayerPrefs.SetInt(key, 1);
						return;
					}
				}
				Flurry.LogEventWithAParameter("SaveData load error", "Exception Type", ex.ToString());
				throw;
			}
		}

		public void ChangeSavePath(string newSavePath)
		{
			_path = newSavePath;
			_lastSaved = null;
		}

		public void Delete()
		{
			if (string.IsNullOrEmpty(_path))
			{
				return;
			}
			try
			{
				if (File.Exists(_path))
				{
					File.Delete(_path);
				}
			}
			catch (Exception ex)
			{
				LogError("Exception while trying to delete SaveFile at | " + _path + "| Exception - " + ex.Message);
			}
		}

		[Conditional("SAVE_GAME_LOGS")]
		private static void Log(string msg, UnityEngine.Object obj = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object obj = null)
		{
			SDebug.LogError("[SaveGame] " + msg, obj);
		}
	}
}
