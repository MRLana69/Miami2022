using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class FileSystemExtensions
	{
		public static void WriteAllBytes([NotNull] this IFile file, [NotNull] byte[] bytes)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			using (Stream stream = file.OpenWrite(true))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}

		[NotNull]
		public static byte[] ReadAllBytes([NotNull] this IFile file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			using (Stream s = file.OpenRead())
			{
				byte[] array = new byte[file.GetSize()];
				s.ReadFully(array);
				return array;
			}
		}

		public static void SaveRawData([NotNull] this IFile file, [NotNull] byte[] data)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			file.Save((Stream stream) =>
			{
				stream.Write(data, 0, data.Length);
			});
		}

		public static void SaveRawData(this IFile file, byte[] data, int offset, int count)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset + count > data.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			file.Save((Stream stream) =>
			{
				stream.Write(data, offset, count);
			});
		}

		public static byte[] LoadRawData([NotNull] this IFile file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			return (byte[])file.Load(StreamExtensions.ReadAllBytes);
		}

		public static void SaveObject([NotNull] this IFile file, [NotNull] object data, [NotNull] ISerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			file.Save((Stream stream) =>
			{
				serializer.WriteObject(data, stream);
			});
		}

		public static object LoadObject([NotNull] this IFile file, [NotNull] Type type, [NotNull] ISerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return file.Load((Stream stream) => serializer.ReadObject(type, stream, true));
		}

		public static T LoadObject<T>([NotNull] this IFile file, [NotNull] ISerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return (T)file.Load((Stream stream) => serializer.ReadObject(typeof(T), stream, true));
		}

		public static void SaveJsonValue([NotNull] this IFile file, [CanBeNull] object data, [NotNull] IJsonSerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			file.Save((Stream stream) =>
			{
				StreamWriter streamWriter = new StreamWriter(stream);
				serializer.WriteJsonValue(data, streamWriter);
				streamWriter.Flush();
			});
		}

		public static object LoadJsonValue([NotNull] this IFile file, [NotNull] Type type, [NotNull] IJsonSerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return file.Load((Stream stream) => serializer.ReadJsonValue(type, new StreamReader(stream)));
		}

		public static T LoadJsonValue<T>([NotNull] this IFile file, [NotNull] IJsonSerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return (T)file.Load((Stream stream) => serializer.ReadJsonValue(typeof(T), new StreamReader(stream)));
		}

		public static void SaveBsonObject([NotNull] this IFile file, [NotNull] object data, [NotNull] IBsonSerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			file.Save((Stream stream) =>
			{
				BinaryWriter binaryWriter = new BinaryWriter(stream);
				serializer.WriteBsonObject(data, binaryWriter);
				binaryWriter.Flush();
			});
		}

		public static object LoadBsonObject([NotNull] this IFile file, [NotNull] Type type, [NotNull] IBsonSerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return file.Load((Stream stream) => serializer.ReadBsonObject(type, new BinaryReader(stream)));
		}

		public static T LoadBsonObject<T>([NotNull] this IFile file, [NotNull] IBsonSerializer serializer)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			return (T)file.Load((Stream stream) => serializer.ReadBsonObject(typeof(T), new BinaryReader(stream)));
		}
	}
}
