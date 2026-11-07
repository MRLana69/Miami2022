using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class SerializerExtensions
	{
		public static byte[] Serialize([NotNull] this ISerializer serializer, [NotNull] object value)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				serializer.WriteObject(value, memoryStream);
				return memoryStream.ToArray();
			}
		}

		[Pure]
		public static T ReadObject<T>([NotNull] this ISerializer serializer, [NotNull] Stream inputStream, bool allowBuffering)
		{
			return (T)serializer.ReadObject(typeof(T), inputStream, allowBuffering);
		}

		[Pure]
		public static T Deserialize<T>([NotNull] this ISerializer serializer, [NotNull] byte[] data)
		{
			using (MemoryStream inputStream = new MemoryStream(data))
			{
				return (T)serializer.ReadObject(typeof(T), inputStream, true);
			}
		}

		[Pure]
		public static T ConvertValue<T>([NotNull] this ISerializer serializer, [CanBeNull] object value)
		{
			return (T)serializer.ConvertValue(typeof(T), value);
		}

		public static void WriteObject([NotNull] this Stream outputStream, [NotNull] object value, [NotNull] ISerializer serializer)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			serializer.WriteObject(value, outputStream);
		}

		public static object ReadObject([NotNull] this Stream inputStream, [NotNull] Type type, [NotNull] ISerializer serializer, bool allowBuffering)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return serializer.ReadObject(type, inputStream, allowBuffering);
		}

		public static T ReadObject<T>([NotNull] this Stream inputStream, [NotNull] ISerializer serializer, bool allowBuffering)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return (T)serializer.ReadObject(typeof(T), inputStream, allowBuffering);
		}
	}
}
