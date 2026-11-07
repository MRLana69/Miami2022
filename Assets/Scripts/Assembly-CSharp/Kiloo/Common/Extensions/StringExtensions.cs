using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Kiloo.Common.Extensions
{
	public static class StringExtensions
	{
		public static string FilterWhiteSpaces(this string input)
		{
			if (input == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length);
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (i == 0 || c != ' ' || (c == ' ' && input[i - 1] != ' '))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Trim();
		}

		public static string Populate(this string str, params object[] args)
		{
			return str.Format(args);
		}

		public static string Format(this string str, params object[] args)
		{
			return string.Format(str, args);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static bool IsNullOrWhiteSpace(this string str)
		{
			if (str != null)
			{
				for (int i = 0; i < str.Length; i++)
				{
					if (!char.IsWhiteSpace(str[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static string ToTitleCase(this string str)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
		}

		public static bool Contains(this string haystack, string needle, StringComparison comp = StringComparison.Ordinal)
		{
			return haystack.IndexOf(needle, comp) >= 0;
		}

		public static bool IsOneOf(this string needle, IEnumerable<string> haystack)
		{
			return haystack.Any((string s) => needle == s);
		}

		public static bool IsOneOf(this string needle, params string[] haystack)
		{
			return haystack.Contains(needle);
		}

		public static string ToLowerString(this object obj)
		{
			return obj.ToString().ToLower();
		}

		public static string ToYesNoString(this bool value)
		{
			return (!value) ? "No" : "Yes";
		}
	}
}
