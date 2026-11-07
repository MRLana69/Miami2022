using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class BsonSerializerExtensions
	{
		[Pure]
		public static byte[] ToBson([NotNull] this IBsonSerializer serializer, [NotNull] object value)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter writer = new BinaryWriter(memoryStream))
			{
				serializer.WriteBsonObject(value, writer);
				return memoryStream.ToArray();
			}
		}

		[Pure]
		public static object FromBson([NotNull] this IBsonSerializer serializer, [NotNull] Type type, [NotNull] byte[] data)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
			{
				return serializer.ReadBsonObject(type, reader);
			}
		}

		[Pure]
		public static T FromBson<T>([NotNull] this IBsonSerializer serializer, [NotNull] byte[] data)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
			{
				return (T)serializer.ReadBsonObject(typeof(T), reader);
			}
		}

		[Pure]
		public static T ConvertValue<T>([NotNull] this IBsonSerializer serializer, [CanBeNull] object value)
		{
			return (T)serializer.ConvertValue(typeof(T), value);
		}

		public static T ReadBsonObject<T>([NotNull] this IBsonSerializer serializer, [NotNull] BinaryReader reader)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return (T)serializer.ReadBsonObject(typeof(T), reader);
		}

		public static void WriteBsonObject([NotNull] this BinaryWriter writer, [NotNull] object value, [NotNull] IBsonSerializer serializer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			serializer.WriteBsonObject(value, writer);
		}

		public static object ReadBsonObject([NotNull] this BinaryReader reader, [NotNull] Type type, [NotNull] IBsonSerializer serializer)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return serializer.ReadBsonObject(type, reader);
		}

		public static T ReadBsonObject<T>([NotNull] this BinaryReader reader, [NotNull] IBsonSerializer serializer)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return (T)serializer.ReadBsonObject(typeof(T), reader);
		}
	}
}
