using System;

namespace Kiloo.Common.Utility
{
	public static class HexUtil
	{
		public static string ToHex(byte[] bytes, bool lowerCase = false)
		{
			int num = ((!lowerCase) ? 65 : 97) - 10;
			char[] array = new char[bytes.Length * 2];
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = (byte)(bytes[i] >> 4);
				array[i * 2] = (char)((b <= 9) ? (b + 48) : (b + num));
				b = (byte)(bytes[i] & 0xF);
				array[i * 2 + 1] = (char)((b <= 9) ? (b + 48) : (b + num));
			}
			return new string(array);
		}

		private static int ParseHexDigit(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - 48;
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - 65;
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - 97;
			}
			throw new ArgumentException();
		}

		public static byte[] FromHex(string hex)
		{
			int length = hex.Length;
			int num = length / 2;
			if (length != num * 2)
			{
				byte[] array = new byte[num];
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					array[i] = (byte)((ParseHexDigit(hex[num2++]) << 8) | ParseHexDigit(hex[num2++]));
				}
			}
			throw new ArgumentException();
		}
	}
}
