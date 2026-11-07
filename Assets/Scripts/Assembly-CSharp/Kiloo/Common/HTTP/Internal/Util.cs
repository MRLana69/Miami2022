using System;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace Kiloo.Common.HTTP.Internal
{
	internal class Util
	{
		public const string EOL = "\r\n";

		public static int ConvertTimeoutToMilliseconds(float seconds)
		{
			int num = 0;
			if (seconds > 0f)
			{
				num = (int)(seconds * 1000f);
				if (num == 0)
				{
					num = 1;
				}
			}
			return num;
		}

		public static bool IsTextContentType(Headers headers)
		{
			if (headers.contentType.type == "text")
			{
				return true;
			}
			if (headers.contentType.type == "application" && (headers.contentType.subType.Contains("json") || headers.contentType.subType.Contains("javascript")))
			{
				return true;
			}
			return false;
		}

		public static bool IsPrintable(byte[] bytes, int maxScan = 256)
		{
			if (bytes == null)
			{
				return false;
			}
			if (bytes.Length < maxScan)
			{
				maxScan = bytes.Length;
			}
			for (int i = 0; i < maxScan; i++)
			{
				byte b = bytes[i];
				if (b < 32 && b != 9 && b != 10 && b != 13)
				{
					return false;
				}
			}
			return true;
		}

		public static string ContentAsString(byte[] bytes, Headers headers, int maxChars = int.MaxValue)
		{
			int totalCharCount;
			return ContentAsString(bytes, headers, maxChars, out totalCharCount);
		}

		public static string ContentAsString(byte[] bytes, Headers headers, int maxChars, out int totalCharCount)
		{
			if (bytes == null)
			{
				totalCharCount = 0;
				return null;
			}
			Encoding encoding = null;
			string parameter = headers.contentType.GetParameter("charset");
			if (!string.IsNullOrEmpty(parameter))
			{
				try
				{
					encoding = Encoding.GetEncoding(parameter);
				}
				catch (ArgumentException)
				{
				}
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			if (maxChars < int.MaxValue)
			{
				Decoder decoder = encoding.GetDecoder();
				totalCharCount = decoder.GetCharCount(bytes, 0, bytes.Length);
				if (totalCharCount > maxChars)
				{
					int num = encoding.GetMaxByteCount(maxChars);
					if (num > bytes.Length)
					{
						num = bytes.Length;
					}
					int charCount = decoder.GetCharCount(bytes, 0, num);
					char[] array = new char[charCount];
					decoder.GetChars(bytes, 0, num, array, 0);
					return new string(array, 0, maxChars);
				}
			}
			string text = encoding.GetString(bytes, 0, bytes.Length);
			totalCharCount = text.Length;
			return text;
		}

		public static string ContentAsHex(byte[] bytes, Headers headers, int bytesPerLine, int maxLines = 0)
		{
			int num = (bytes.Length + bytesPerLine - 1) / bytesPerLine;
			if (maxLines == 0 || maxLines > num)
			{
				maxLines = num;
			}
			int num2 = 10 + bytesPerLine * 4;
			StringBuilder stringBuilder = new StringBuilder(num2 * maxLines);
			int num3 = 0;
			for (int i = 0; i < maxLines; i++)
			{
				stringBuilder.AppendFormat("{0:X8} ", num3);
				int num4 = num3 + bytesPerLine;
				if (num4 > bytes.Length)
				{
					num4 = bytes.Length;
				}
				for (int j = num3; j < num4; j++)
				{
					stringBuilder.AppendFormat("{0:X2} ", bytes[j]);
				}
				for (int k = num3; k < num4; k++)
				{
					stringBuilder.Append((char)((bytes[k] >= 32) ? bytes[k] : 46));
				}
				stringBuilder.Append('\n');
				num3 = num4;
			}
			if (num3 < bytes.Length)
			{
				stringBuilder.Append("\n--truncated " + (bytes.Length - num3) + " more bytes--");
			}
			return stringBuilder.ToString();
		}

		public static string PrintableContent(byte[] bytes, Headers headers, int maxChars, int maxHexLines)
		{
			if (IsTextContentType(headers) || IsPrintable(bytes))
			{
				int totalCharCount;
				string text = ContentAsString(bytes, headers, maxChars, out totalCharCount);
				if (text.Length > totalCharCount)
				{
					string text2 = text;
					text = text2 + "\n--truncated " + (totalCharCount - text.Length) + " more chars--";
				}
				return text;
			}
			return ContentAsHex(bytes, headers, 32, maxHexLines);
		}

		public static byte[] UnZip(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (GZipStream gZipStream = new GZipStream(new MemoryStream(data, false), CompressionMode.Decompress))
			{
				byte[] array = new byte[1024];
				int count;
				while ((count = gZipStream.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
			}
			return memoryStream.ToArray();
		}

		public static ResponseCategory GetResponseCategory(int status)
		{
			switch (status / 100)
			{
			case 1:
				return ResponseCategory.Informational;
			case 2:
				return ResponseCategory.Successful;
			case 3:
				return ResponseCategory.Redirection;
			case 4:
				return ResponseCategory.ClientError;
			case 5:
				return ResponseCategory.ServerError;
			default:
				return ResponseCategory.Unknown;
			}
		}
	}
}
