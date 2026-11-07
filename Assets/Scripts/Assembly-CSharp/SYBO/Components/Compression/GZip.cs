using System.IO;
using Ionic.Zlib;

namespace SYBO.Components.Compression
{
	public static class GZip
	{
		public static byte[] CompressBytes(byte[] inputBytes)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gZipStream.FlushMode = FlushType.Finish;
					gZipStream.Write(inputBytes, 0, inputBytes.Length);
				}
				return memoryStream.ToArray();
			}
		}

		public static byte[] DecompressBytes(byte[] inputBytes)
		{
			using (MemoryStream stream = new MemoryStream(inputBytes))
			{
				using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress, true))
				{
					MemoryStream memoryStream = new MemoryStream();
					byte[] array = new byte[gZipStream.BufferSize];
					int num = array.Length;
					while (num == array.Length)
					{
						num = gZipStream.Read(array, 0, num);
						memoryStream.Write(array, 0, num);
					}
					return memoryStream.ToArray();
				}
			}
		}
	}
}
