using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP.Internal
{
	[Obsolete("Not used in Unity versions 5.4 or newer.")]
	internal class SocketsResponse : Response
	{
		private readonly object _chunksLock = new object();

		private List<byte[]> _chunks;

		public SocketsResponse()
			: base(new Headers(), null)
		{
		}

		private string ReadHeaderLine(Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			int num;
			while ((num = stream.ReadByte()) >= 0 && num != "\r\n"[1])
			{
				if (num != "\r\n"[0])
				{
					memoryStream.WriteByte((byte)num);
				}
			}
			return ASCIIEncoding.ASCII.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length).Trim();
		}

		private bool ReadHeader(Stream stream, out string key, out string value)
		{
			string text = ReadHeaderLine(stream);
			if (text == string.Empty)
			{
				key = null;
				value = null;
				return false;
			}
			int num = text.IndexOf(':');
			if (num == -1)
			{
				throw new HTTPException("Invalid header in response");
			}
			key = text.Substring(0, num).Trim();
			value = text.Substring(num + 1).Trim();
			return true;
		}

		public byte[] TakeChunk()
		{
			if (base.headers["Content-Encoding"].Contains("gzip"))
			{
				return null;
			}
			byte[] result = null;
			lock (_chunksLock)
			{
				if (_chunks != null && _chunks.Count > 0)
				{
					result = _chunks[0];
					_chunks.RemoveAt(0);
				}
			}
			return result;
		}

		public void ReadFromStream(Stream inputStream, bool ignoreContentMd5 = false)
		{
			string[] array = ReadHeaderLine(inputStream).Split(' ');
			int result;
			if (!int.TryParse(array[1], out result))
			{
				throw new HTTPException("Bad Status Code in response");
			}
			base.status = result;
			base.message = string.Join(" ", array, 2, array.Length - 2);
			base.headers.Clear();
			string key;
			string value;
			while (ReadHeader(inputStream, out key, out value))
			{
				base.headers.Add(key, value);
			}
			if (base.headers["Transfer-Encoding"] == "chunked")
			{
				lock (_chunksLock)
				{
					_chunks = new List<byte[]>();
				}
				int num = 0;
				int num2;
				do
				{
					string s = ReadHeaderLine(inputStream);
					num2 = int.Parse(s, NumberStyles.AllowHexSpecifier);
					if (num2 > 0)
					{
						byte[] array2 = new byte[num2];
						inputStream.ReadFully(array2);
						lock (_chunksLock)
						{
							_chunks.Add(array2);
						}
						num += num2;
						inputStream.ReadByte();
						inputStream.ReadByte();
					}
				}
				while (num2 > 0);
				string key2;
				string value2;
				while (ReadHeader(inputStream, out key2, out value2))
				{
					base.headers.Add(key2, value2);
				}
				byte[] array3 = new byte[num];
				int num3 = 0;
				lock (_chunksLock)
				{
					foreach (byte[] chunk in _chunks)
					{
						chunk.CopyTo(array3, num3);
						num3 += chunk.Length;
					}
					_chunks = null;
				}
				if (base.headers["Content-Encoding"].Contains("gzip"))
				{
					base.bytes = Util.UnZip(array3);
				}
				else
				{
					base.bytes = array3;
				}
				return;
			}
			long num4 = base.headers.Get("Content-Length", 0L);
			if (num4 > 0)
			{
				byte[] array4 = new byte[num4];
				inputStream.ReadFully(array4);
				string text = base.headers.Get("Content-MD5");
				if (!string.IsNullOrEmpty(text))
				{
					byte[] inArray = Checksum.ComputeMD5(array4);
					string text2 = Convert.ToBase64String(inArray);
					if (text2 != text)
					{
						if (!ignoreContentMd5)
						{
							throw new HTTPException("Content-MD5 digest mismatch. Data is corrupted.");
						}
						LogWarning("Content-MD5 digest mismatch. Data is corrupted.");
					}
				}
				if (base.headers["Content-Encoding"].Contains("gzip"))
				{
					base.bytes = Util.UnZip(array4);
				}
				else
				{
					base.bytes = array4;
				}
			}
			else
			{
				base.bytes = new byte[0];
			}
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[SocketsResponse] " + msg);
		}

		[Conditional("HTTP_VERBOSE_DEBUG_LOGS")]
		private static void LogVerbose(string msg)
		{
			Debug.Log("[SocketsResponse] " + msg);
		}
	}
}
