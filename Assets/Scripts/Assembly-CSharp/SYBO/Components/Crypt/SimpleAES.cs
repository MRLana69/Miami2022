using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SYBO.Components.Crypt
{
	public class SimpleAES : ICrypt
	{
		private const int KIVLength = 16;

		private Aes _aes;

		public SimpleAES(byte[] key)
		{
			_aes = new AesManaged();
			_aes.Key = key;
			_aes.Mode = CipherMode.ECB;
		}

		public static byte[] GenerateKey()
		{
			using (AesManaged aesManaged = new AesManaged())
			{
				return aesManaged.Key;
			}
		}

		public string Encrypt(string unencrypted)
		{
			return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(unencrypted)));
		}

		public string Decrypt(string encrypted)
		{
			try
			{
				return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(encrypted)));
			}
			catch (Exception innerException)
			{
				throw new DecryptionException("Failed to decrypt message", innerException);
			}
		}

		public byte[] Encrypt(byte[] buffer)
		{
			_aes.GenerateIV();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(_aes.IV, 0, 16);
				using (ICryptoTransform transform = _aes.CreateEncryptor())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(buffer, 0, buffer.Length);
					}
				}
				return memoryStream.ToArray();
			}
		}

		public byte[] Decrypt(byte[] buffer)
		{
			byte[] array = new byte[16];
			Array.Copy(buffer, array, 16);
			_aes.IV = array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ICryptoTransform transform = _aes.CreateDecryptor())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(buffer, 16, buffer.Length - 16);
					}
				}
				return memoryStream.ToArray();
			}
		}

		private static byte[] Transform(byte[] buffer, ICryptoTransform transform)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
			{
				cryptoStream.Write(buffer, 0, buffer.Length);
			}
			return memoryStream.ToArray();
		}
	}
}
