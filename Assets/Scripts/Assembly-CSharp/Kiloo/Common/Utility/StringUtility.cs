using System;
using System.Collections.Generic;
using System.Text;

namespace Kiloo.Common.Utility
{
	public static class StringUtility
	{
		public enum MatchType
		{
			Is = 0,
			EndsWith = 1,
			BeginsWith = 2,
			Contains = 3
		}

		public static bool Match(string str, string pattern, MatchType matchType, bool ignoreCase)
		{
			switch (matchType)
			{
			case MatchType.Is:
				return str.Equals(pattern, (!ignoreCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
			case MatchType.EndsWith:
				return str.EndsWith(pattern, (!ignoreCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
			case MatchType.BeginsWith:
				return str.StartsWith(pattern, (!ignoreCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
			case MatchType.Contains:
				if (ignoreCase)
				{
					str = str.ToLower();
					pattern = pattern.ToLower();
				}
				return str.Contains(pattern);
			default:
				return false;
			}
		}

		public static string ArrayToString<MyType>(MyType[] array, string seperator = ", ")
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MyType val in array)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(seperator);
				}
				stringBuilder.Append(val);
			}
			return stringBuilder.ToString();
		}

		public static void ValidateEnumValueString(Type enumType, string enumValueString, bool mayBeNullOrEmpty, string varName)
		{
			if (string.IsNullOrEmpty(enumValueString))
			{
				if (mayBeNullOrEmpty)
				{
					return;
				}
			}
			else
			{
				try
				{
					Enum.Parse(enumType, enumValueString, true);
					return;
				}
				catch (Exception)
				{
				}
			}
			SDebug.LogError(varName + " must be one of: " + ArrayToString(Enum.GetNames(enumType)));
		}

		public static int NonEscapedIndexOf(string text, int startIndex, char ch)
		{
			int num = text.IndexOf(ch, startIndex);
			if (num == 0)
			{
				return num;
			}
			if (num > 0 && text[num - 1] != '\\')
			{
				return num;
			}
			return -1;
		}

		public static int GetNextKeyValuePair(string text, int startIndex, out string key, out string value)
		{
			int num = NonEscapedIndexOf(text, startIndex, '[');
			if (num >= 0)
			{
				int num2 = NonEscapedIndexOf(text, num + 1, ']');
				if (num2 >= 0)
				{
					key = text.Substring(num + 1, num2 - num - 1).Trim();
					int num3 = NonEscapedIndexOf(text, num2 + 1, '[');
					if (num3 < 0)
					{
						num3 = text.Length;
					}
					value = text.Substring(num2 + 1, num3 - num2 - 1).Trim();
					return num3;
				}
			}
			key = null;
			value = null;
			return -1;
		}

		public static Dictionary<string, string> ParseProperties(string text)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 0;
			string key;
			string value;
			while ((num = GetNextKeyValuePair(text, num, out key, out value)) >= 0)
			{
				dictionary[key] = value;
				if (num == text.Length)
				{
					break;
				}
			}
			return dictionary;
		}

		public static string FormatProperties(Dictionary<string, string> props)
		{
			StringBuilder stringBuilder = new StringBuilder();
			char[] anyOf = new char[2] { '[', ']' };
			foreach (KeyValuePair<string, string> prop in props)
			{
				if (!string.IsNullOrEmpty(prop.Key) && !string.IsNullOrEmpty(prop.Value))
				{
					if (prop.Key.IndexOfAny(anyOf) >= 0)
					{
						SDebug.LogError("Property data should not contain '[' and ']' characters");
					}
					stringBuilder.Append('[').Append(prop.Key).Append(']')
						.AppendLine()
						.Append(prop.Value)
						.AppendLine()
						.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		public static string[][] ParseSets(string setsString)
		{
			string[] separator = new string[1] { ");(" };
			string[] array = setsString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				if (array[0][0] == '(')
				{
					array[0] = array[0].Substring(1);
				}
				int num = array.Length - 1;
				int num2 = array[num].Length - 1;
				if (array[num][num2] == ')')
				{
					array[num] = array[num].Remove(num2);
				}
				string[][] array2 = new string[array.Length][];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = array[i].Split(';');
				}
				return array2;
			}
			return new string[0][];
		}

		public static string ToHexString(byte[] bytes, bool lowerCase = false)
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

		public static byte[] FromHexString(string hex)
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
