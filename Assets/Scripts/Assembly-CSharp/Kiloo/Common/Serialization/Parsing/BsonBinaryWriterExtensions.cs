using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Kiloo.Common.Serialization.Parsing
{
	public static class BsonBinaryWriterExtensions
	{
		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private static bool CanBeBsonObject(object obj)
		{
			if (obj == null || obj is string)
			{
				return false;
			}
			if (obj is IEnumerable && !(obj is IDictionary))
			{
				return false;
			}
			Type type = obj.GetType();
			return !type.IsPrimitive && !type.IsEnum;
		}

		public static void WriteBsonObject([NotNull] this BinaryWriter writer, [NotNull] object obj, KilooSerializerMapping mapping)
		{
			if (writer.BaseStream.CanSeek)
			{
				if (!CanBeBsonObject(obj))
				{
					throw new NotSupportedException("Unsupported type.");
				}
				IEnumerable<KeyValuePair<string, object>> enumerable = obj as IEnumerable<KeyValuePair<string, object>>;
				if (enumerable == null)
				{
					enumerable = ConversionHelper.ValueAsKeyValuePairs(obj, mapping);
				}
				long position = writer.BaseStream.Position;
				writer.Write(0);
				foreach (KeyValuePair<string, object> item in enumerable)
				{
					writer.WriteBsonProperty(item.Key, item.Value, mapping);
				}
				writer.Write((byte)0);
				long position2 = writer.BaseStream.Position;
				writer.BaseStream.Seek(position, SeekOrigin.Begin);
				writer.Write((int)(position2 - position));
				writer.BaseStream.Seek(position2, SeekOrigin.Begin);
				return;
			}
			using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream()))
			{
				binaryWriter.WriteBsonObject(obj, mapping);
				writer.Write(((MemoryStream)binaryWriter.BaseStream).GetBuffer(), 0, (int)binaryWriter.BaseStream.Length);
			}
		}

		private static void WriteBsonProperty([NotNull] this BinaryWriter writer, [NotNull] string name, [CanBeNull] object value, KilooSerializerMapping mapping)
		{
			if (value == null)
			{
				writer.Write((byte)10);
				writer.WriteBsonCString(name);
				return;
			}
			byte[] array = value as byte[];
			if (array != null)
			{
				writer.Write((byte)5);
				writer.WriteBsonCString(name);
				writer.WriteBsonBinaryData(0, array);
				return;
			}
			string text = value as string;
			if (text != null)
			{
				writer.Write((byte)2);
				writer.WriteBsonCString(name);
				writer.WriteBsonString(text);
				return;
			}
			if (value is float)
			{
				value = Convert.ChangeType(value, typeof(double));
			}
			if (value is double)
			{
				writer.Write((byte)1);
				writer.WriteBsonCString(name);
				writer.Write((double)value);
				return;
			}
			if (value is bool)
			{
				writer.Write((byte)8);
				writer.WriteBsonCString(name);
				writer.Write((byte)(((bool)value) ? 1u : 0u));
				return;
			}
			if (value is char || value is sbyte || value is byte || value is short || value is ushort)
			{
				value = Convert.ChangeType(value, typeof(int));
			}
			if (value is int)
			{
				writer.Write((byte)16);
				writer.WriteBsonCString(name);
				writer.Write((int)value);
				return;
			}
			if (value is uint)
			{
				value = Convert.ChangeType(value, typeof(long));
			}
			if (value is ulong)
			{
				writer.Write((byte)17);
				writer.WriteBsonCString(name);
				writer.Write((ulong)value);
				return;
			}
			if (value is long)
			{
				writer.Write((byte)18);
				writer.WriteBsonCString(name);
				writer.Write((long)value);
				return;
			}
			if (value is DateTime)
			{
				writer.Write((byte)9);
				writer.WriteBsonCString(name);
				writer.WriteBsonDateTime((DateTime)value);
				return;
			}
			if (value is decimal)
			{
				writer.Write((byte)19);
				writer.WriteBsonCString(name);
				writer.Write((decimal)value);
				return;
			}
			if (value is Guid)
			{
				writer.Write((byte)5);
				writer.WriteBsonCString(name);
				writer.WriteBsonBinaryData(4, ((Guid)value).ToByteArray());
				return;
			}
			text = ConversionHelper.ValueAsString(value, CultureInfo.InvariantCulture);
			if (text != null)
			{
				writer.Write((byte)2);
				writer.WriteBsonCString(name);
				writer.WriteBsonString(text);
				return;
			}
			IEnumerable enumerable = value as IEnumerable;
			if (enumerable != null && !(value is IDictionary))
			{
				writer.Write((byte)4);
				writer.WriteBsonCString(name);
				writer.WriteBsonArray(enumerable, mapping);
			}
			else
			{
				writer.Write((byte)3);
				writer.WriteBsonCString(name);
				writer.WriteBsonObject(value, mapping);
			}
		}

		private static void WriteBsonArray(this BinaryWriter writer, IEnumerable array, KilooSerializerMapping mapping)
		{
			if (writer.BaseStream.CanSeek)
			{
				long position = writer.BaseStream.Position;
				writer.Write(0);
				int num = 0;
				foreach (object item in array)
				{
					writer.WriteBsonProperty(num.ToString(), item, mapping);
					num++;
				}
				writer.Write((byte)0);
				long position2 = writer.BaseStream.Position;
				writer.BaseStream.Seek(position, SeekOrigin.Begin);
				writer.Write((int)(position2 - position));
				writer.BaseStream.Seek(position2, SeekOrigin.Begin);
				return;
			}
			using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream()))
			{
				binaryWriter.WriteBsonArray(array, mapping);
				writer.Write(((MemoryStream)binaryWriter.BaseStream).GetBuffer(), 0, (int)binaryWriter.BaseStream.Length);
			}
		}

		private static void WriteBsonBinaryData(this BinaryWriter writer, byte subType, byte[] buf)
		{
			byte[] bytes = BitConverter.GetBytes(buf.Length);
			writer.Write(bytes, 0, bytes.Length);
			writer.Write(subType);
			writer.Write(buf, 0, buf.Length);
		}

		private static void WriteBsonCString(this BinaryWriter writer, string value)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(value);
			writer.Write(bytes, 0, bytes.Length);
			writer.Write((byte)0);
		}

		private static void WriteBsonString(this BinaryWriter writer, string value)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(value);
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length + 1);
			writer.Write(bytes2, 0, bytes2.Length);
			writer.Write(bytes, 0, bytes.Length);
			writer.Write((byte)0);
		}

		private static void WriteBsonDateTime(this BinaryWriter writer, DateTime value)
		{
			byte[] bytes = BitConverter.GetBytes((long)(value.ToUniversalTime() - _epoch).TotalMilliseconds);
			writer.Write(bytes, 0, bytes.Length);
		}
	}
}
