using System.IO;
using System.Security.Cryptography;
using System.Text;
using Ionic.Crc;

namespace Kiloo.Common.Utility
{
	public static class Checksum
	{
		public static byte[] ComputeSHA1(byte[] data)
		{
			using (SHA1 sHA = SHA1.Create())
			{
				return sHA.ComputeHash(data);
			}
		}

		public static byte[] ComputeMD5(byte[] data)
		{
			using (MD5 mD = MD5.Create())
			{
				return mD.ComputeHash(data);
			}
		}

		public static byte[] ComputeSHA256(byte[] data)
		{
			using (SHA256 sHA = SHA256.Create())
			{
				return sHA.ComputeHash(data);
			}
		}

		public static byte[] ComputeSHA1(string data, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return ComputeSHA1(encoding.GetBytes(data));
		}

		public static byte[] ComputeMD5(string data, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return ComputeMD5(encoding.GetBytes(data));
		}

		public static byte[] ComputeSHA256(string data, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return ComputeSHA256(encoding.GetBytes(data));
		}

		public static int ComputeCrc32(byte[] data)
		{
			return ComputeCrc32(data, 0, data.Length);
		}

		public static int ComputeCrc32(byte[] data, int offset, int length)
		{
			CRC32 cRC = new CRC32();
			cRC.SlurpBlock(data, offset, length);
			return cRC.Crc32Result;
		}

		public static int ComputeCrc32(Stream stream)
		{
			return new CRC32().GetCrc32(stream);
		}
	}
}
