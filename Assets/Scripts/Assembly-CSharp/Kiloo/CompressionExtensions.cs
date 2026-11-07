using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace Kiloo
{
	public static class CompressionExtensions
	{
		private static string DEBUG_TAG = "[CompressionExtension]";

		public static byte[] Compress(this byte[] source, string signature)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(signature);
					binaryWriter.Write(source.Length);
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress, true))
						{
							deflateStream.Write(source, 0, source.Length);
						}
						binaryWriter.Write(memoryStream2.ToArray());
						return memoryStream.ToArray();
					}
				}
			}
		}

		public static byte[] Decompress(this byte[] source, string expectedSignature)
		{
			using (MemoryStream input = new MemoryStream(source))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					string text = binaryReader.ReadString();
					if (!string.IsNullOrEmpty(expectedSignature) && expectedSignature != text)
					{
						throw new IOException("Invalid file signature: " + text);
					}
					int num = binaryReader.ReadInt32();
					byte[] buffer = binaryReader.ReadBytes(source.Length);
					using (MemoryStream stream = new MemoryStream(buffer))
					{
						using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
						{
							byte[] array = new byte[num];
							// O .NET novo pode devolver menos bytes por leitura: lê até completar
							int gjRead = 0;
							while (gjRead < num)
							{
								int gjCount = deflateStream.Read(array, gjRead, num - gjRead);
								if (gjCount <= 0)
								{
									break;
								}
								gjRead += gjCount;
							}
							if (array.Length != num)
							{
							}
							return array;
						}
					}
				}
			}
		}

		public static string ToContentString<T>(this IEnumerable<T> collection, string separator = ", ")
		{
			if (collection == null)
			{
				return "<null>";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (T item in collection)
			{
				stringBuilder.Append(item.ToString() + separator);
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - separator.Length, separator.Length);
			}
			return stringBuilder.ToString();
		}

		[Conditional("COMPRESSION_EXTENSIONS")]
		private static void Log(string msg)
		{
		}

		[Conditional("COMPRESSION_EXTENSIONS")]
		private static void LogWarning(string msg)
		{
		}

		private static void LogError(string msg)
		{
			SDebug.LogError(DEBUG_TAG + msg);
		}
	}
}
