using System;
using System.IO;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class StreamExtensions
	{
		public static int CopyTo([NotNull] this Stream stream, [NotNull] Stream destination)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			byte[] buffer = new byte[1024];
			int num = 0;
			int num2;
			while ((num2 = stream.Read(buffer, 0, 1024)) > 0)
			{
				destination.Write(buffer, 0, num2);
				num += num2;
			}
			return num;
		}

		public static void ReadFully([NotNull] this Stream s, [NotNull] byte[] buffer)
		{
			s.ReadFully(buffer, 0, buffer.Length);
		}

		public static void ReadFully([NotNull] this Stream s, [NotNull] byte[] buffer, int offset, int count)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			int num = count;
			int num2 = 0;
			while (num > 0)
			{
				int num3 = s.Read(buffer, num2, num);
				if (num3 == 0)
				{
					throw new EndOfStreamException();
				}
				num2 += num3;
				num -= num3;
			}
		}

		[NotNull]
		public static byte[] ReadAllBytes([NotNull] this Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (stream.CanSeek)
			{
				byte[] array = new byte[stream.Length];
				if (array.Length > 0)
				{
					stream.ReadFully(array);
				}
				return array;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] buffer = new byte[1024];
				int count;
				while ((count = stream.Read(buffer, 0, 1024)) > 0)
				{
					memoryStream.Write(buffer, 0, count);
				}
				return memoryStream.ToArray();
			}
		}

		public static int ReadInt32(this Stream inputStream)
		{
			byte[] array = new byte[4];
			inputStream.ReadFully(array);
			return array[0] | (array[1] << 8) | (array[2] << 16) | (array[3] << 24);
		}

		public static void WriteInt32(this Stream outputStream, int value)
		{
			byte[] buffer = new byte[4]
			{
				(byte)value,
				(byte)(value >> 8),
				(byte)(value >> 16),
				(byte)(value >> 24)
			};
			outputStream.Write(buffer, 0, 4);
		}

		public static object ReadTamperProofData([NotNull] this Stream inputStream, [NotNull] ReadDataFunc readDataFunc, byte[] saltBytes = null)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (readDataFunc == null)
			{
				throw new ArgumentNullException("readDataFunc");
			}
			int num = inputStream.ReadInt32();
			if (num != 20)
			{
				throw new InvalidTamperProofDataException("Invalid tamper proof data signature.");
			}
			byte[] array = new byte[20];
			inputStream.ReadFully(array);
			int num2 = inputStream.ReadInt32();
			BoundedInputStream boundedInputStream = new BoundedInputStream(inputStream, num2);
			using (SHA1Stream sHA1Stream = new SHA1Stream(boundedInputStream, SHA1Stream.Mode.Read, saltBytes))
			{
				object result = readDataFunc(new KeepOpenStream(sHA1Stream));
				if (boundedInputStream.NumberOfBytesRead < num2)
				{
					byte[] buffer = new byte[128];
					while (sHA1Stream.Read(buffer, 0, 128) > 0)
					{
					}
					if (boundedInputStream.NumberOfBytesRead != num2)
					{
						throw new InvalidTamperProofDataException("Incorrect tamper proof data size.");
					}
				}
				sHA1Stream.FinalizeHash();
				if (!ArrayUtil.ArraysAreEqual(sHA1Stream.Hash, array))
				{
					throw new InvalidTamperProofDataException("Incorrect tamper proof checksum.");
				}
				return result;
			}
		}

		public static void WriteTamperProofData([NotNull] this Stream outputStream, [NotNull] WriteDataFunc writeDataFunc, byte[] saltBytes = null)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (writeDataFunc == null)
			{
				throw new ArgumentNullException("writeDataFunc");
			}
			if (outputStream.CanSeek)
			{
				outputStream.WriteInt32(20);
				outputStream.Write(new byte[24], 0, 24);
				long position = outputStream.Position;
				byte[] hash;
				using (SHA1Stream sHA1Stream = new SHA1Stream(outputStream, SHA1Stream.Mode.Write, saltBytes))
				{
					writeDataFunc(new KeepOpenStream(sHA1Stream));
					sHA1Stream.FinalizeHash();
					hash = sHA1Stream.Hash;
				}
				long position2 = outputStream.Position;
				outputStream.Seek(position - 24, SeekOrigin.Begin);
				outputStream.Write(hash, 0, 20);
				outputStream.WriteInt32((int)(position2 - position));
				outputStream.Seek(position2, SeekOrigin.Begin);
			}
			else
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] hash2;
				using (SHA1Stream sHA1Stream2 = new SHA1Stream(memoryStream, SHA1Stream.Mode.Write, saltBytes))
				{
					writeDataFunc(new KeepOpenStream(sHA1Stream2));
					sHA1Stream2.FinalizeHash();
					hash2 = sHA1Stream2.Hash;
				}
				outputStream.WriteInt32(20);
				outputStream.Write(hash2, 0, 20);
				outputStream.WriteInt32((int)memoryStream.Length);
				outputStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
		}
	}
}
