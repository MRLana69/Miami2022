using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class JsonSerializerExtensions
	{
		public static string ToJson([NotNull] this IJsonSerializer serializer, [CanBeNull] object value)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			using (StringWriter stringWriter = new StringWriter())
			{
				serializer.WriteJsonValue(value, stringWriter);
				return stringWriter.ToString();
			}
		}

		[Pure]
		public static object FromJson([NotNull] this IJsonSerializer serializer, [NotNull] Type type, [NotNull] string json)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			using (StringReader reader = new StringReader(json))
			{
				return serializer.ReadJsonValue(type, reader);
			}
		}

		[Pure]
		public static T FromJson<T>([NotNull] this IJsonSerializer serializer, [NotNull] string json)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			using (StringReader reader = new StringReader(json))
			{
				return (T)serializer.ReadJsonValue(typeof(T), reader);
			}
		}

		[Pure]
		public static T ReadJsonValue<T>([NotNull] this IJsonSerializer serializer, [NotNull] TextReader reader)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return (T)serializer.ReadJsonValue(typeof(T), reader);
		}

		[Pure]
		public static T ConvertValue<T>([NotNull] this IJsonSerializer serializer, [CanBeNull] object value)
		{
			return (T)serializer.ConvertValue(typeof(T), value);
		}

		public static void WriteJsonValue([NotNull] this TextWriter writer, [CanBeNull] object value, [NotNull] IJsonSerializer serializer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			serializer.WriteJsonValue(value, writer);
		}

		public static T ReadJsonValue<T>([NotNull] this TextReader reader, [NotNull] IJsonSerializer serializer)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return (T)serializer.ReadJsonValue(typeof(T), reader);
		}

		public static object ReadJsonValue([NotNull] this TextReader reader, [NotNull] Type type, [NotNull] IJsonSerializer serializer)
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
			return serializer.ReadJsonValue(type, reader);
		}
	}
}
