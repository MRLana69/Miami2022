using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kiloo.Common.Serialization.Parsing
{
	internal static class BsonBinaryReaderExtensions
	{
		public static IDictionary ReadBsonObject(this BinaryReader reader, bool ignoreCase)
		{
			int bytesRead = 0;
			return reader.ReadBsonObject(ignoreCase, ref bytesRead);
		}

		private static IDictionary ReadBsonObject(this BinaryReader reader, bool ignoreCase, ref int bytesRead)
		{
			int num = reader.ReadInt32() - 4;
			bytesRead += 4;
			Dictionary<string, object> dictionary = ((!ignoreCase) ? new Dictionary<string, object>() : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
			int num2 = bytesRead;
			while (bytesRead < num2 + num - 1)
			{
				KeyValuePair<string, object> keyValuePair = reader.ReadBsonProperty(ignoreCase, ref bytesRead);
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			reader.ReadByte();
			bytesRead++;
			return dictionary;
		}

		private static KeyValuePair<string, object> ReadBsonProperty(this BinaryReader reader, bool ignoreCase, ref int bytesRead)
		{
			byte b = reader.ReadByte();
			bytesRead++;
			string text = reader.ReadBsonCString(ref bytesRead);
			object value;
			switch (b)
			{
			case 1:
				value = reader.ReadDouble();
				bytesRead += 8;
				break;
			case 2:
			case 13:
			case 14:
				value = reader.ReadBsonString(ref bytesRead);
				break;
			case 3:
				value = reader.ReadBsonObject(ignoreCase, ref bytesRead);
				break;
			case 4:
				value = reader.ReadBsonArray(ignoreCase, ref bytesRead);
				break;
			case 5:
			{
				int num2 = reader.ReadInt32();
				byte b2 = reader.ReadByte();
				bytesRead += 5;
				byte[] array = reader.ReadBytes(num2);
				value = ((b2 != 4) ? array : ((object)new Guid(array)));
				bytesRead += num2;
				break;
			}
			case 7:
				value = reader.ReadBytes(12);
				bytesRead += 12;
				break;
			case 8:
				value = reader.ReadByte() != 0;
				bytesRead++;
				break;
			case 9:
			{
				long num = reader.ReadInt64();
				bytesRead += 8;
				value = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(num * 10000);
				break;
			}
			case 6:
			case 10:
				value = null;
				break;
			case 16:
				value = reader.ReadInt32();
				bytesRead += 4;
				break;
			case 17:
				value = reader.ReadUInt64();
				bytesRead += 8;
				break;
			case 18:
				value = reader.ReadInt64();
				bytesRead += 8;
				break;
			case 19:
				value = reader.ReadDecimal();
				bytesRead += 16;
				break;
			case 11:
			{
				string text2 = reader.ReadBsonCString(ref bytesRead);
				string text3 = reader.ReadBsonCString(ref bytesRead);
				value = '/' + text2 + '/' + text3;
				break;
			}
			default:
				throw new Exception(string.Format("Unknown BSON type for {0}: {1}", text, b));
			}
			return new KeyValuePair<string, object>(text, value);
		}

		private static IList ReadBsonArray(this BinaryReader reader, bool ignoreCase, ref int bytesRead)
		{
			int num = reader.ReadInt32() - 4;
			bytesRead += 4;
			List<object> list = new List<object>();
			int num2 = bytesRead;
			while (bytesRead < num2 + num - 1)
			{
				list.Add(reader.ReadBsonProperty(ignoreCase, ref bytesRead).Value);
			}
			reader.ReadByte();
			bytesRead++;
			return list;
		}

		private static string ReadBsonString(this BinaryReader reader, ref int bytesRead)
		{
			int num = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(num - 1);
			reader.ReadByte();
			bytesRead += num + 4;
			return Encoding.UTF8.GetString(bytes);
		}

		private static string ReadBsonCString(this BinaryReader reader, ref int bytesRead)
		{
			MemoryStream memoryStream = new MemoryStream();
			while (true)
			{
				byte b = reader.ReadByte();
				bytesRead++;
				if (b == 0)
				{
					break;
				}
				memoryStream.WriteByte(b);
			}
			return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
		}
	}
}
