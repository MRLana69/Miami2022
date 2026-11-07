using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class MiscExtensions
	{
		public static void Destroy(this UnityEngine.Object obj)
		{
			if (!(obj == null))
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(obj);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(obj);
				}
			}
		}

		public static void KillChildren(this Transform trans)
		{
			for (int num = trans.childCount - 1; num >= 0; num--)
			{
				trans.GetChild(num).gameObject.Kill();
			}
		}

		public static void Kill(this GameObject go)
		{
			go.SetActive(false);
			go.Destroy();
		}

		public static string UnifyNewlines(this string content)
		{
			return content.UnifyNewlines(Environment.NewLine);
		}

		public static string UnifyNewlines(this string content, string newline)
		{
			return content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", newline);
		}

		public static bool TryParseHexToColor(this string hexString, out Color color)
		{
			try
			{
				hexString = hexString.Trim('#');
				byte r = byte.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
				byte g = byte.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
				byte b = byte.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
				color = new Color32(r, g, b, byte.MaxValue);
				return true;
			}
			catch (Exception)
			{
				color = Color.white;
				return false;
			}
		}

		public static Color ToColor(this string s)
		{
			s = s.Trim('#');
			byte r = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(s.Substring(4, 2), NumberStyles.HexNumber);
			byte a = ((s.Length <= 6) ? byte.MaxValue : byte.Parse(s.Substring(6, 2), NumberStyles.HexNumber));
			return new Color32(r, g, b, a);
		}

		public static string ToHex(this Color color)
		{
			return ((Color32)color).ToHex();
		}

		public static string ToUIHex(this Color color)
		{
			return ((Color32)color).ToUIHex();
		}

		public static string ToHex(this Color32 color)
		{
			return new StringBuilder("#").Append(string.Format("{0:X2}", color.r)).Append(string.Format("{0:X2}", color.g)).Append(string.Format("{0:X2}", color.b))
				.ToString();
		}

		public static string ToUIHex(this Color32 color)
		{
			return new StringBuilder("[").Append(string.Format("{0:X2}", color.r)).Append(string.Format("{0:X2}", color.g)).Append(string.Format("{0:X2}", color.b))
				.Append("]")
				.ToString();
		}

		public static Color WithAlpha(this Color c, float a)
		{
			return new Color(c.r, c.g, c.b, a);
		}

		public static Color NormalizeRGB(this Color color)
		{
			float a = color.a;
			color /= color.maxColorComponent;
			color.a = a;
			return color;
		}

		public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
		{
			if (strEnumValue == null || !Enum.IsDefined(typeof(TEnum), strEnumValue))
			{
				return defaultValue;
			}
			return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
		}

		public static T SafeInvoke<T>(this Func<T> f)
		{
			return (f == null) ? default(T) : f();
		}

		public static void SafeInvoke(this Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		public static void SafeInvoke<T>(this Action<T> action, T argument)
		{
			if (action != null)
			{
				action(argument);
			}
		}

		public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 argument1, T2 argument2)
		{
			if (action != null)
			{
				action(argument1, argument2);
			}
		}

		public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 argument1, T2 argument2, T3 argument3)
		{
			if (action != null)
			{
				action(argument1, argument2, argument3);
			}
		}

		public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 argument1, T2 argument2, T3 argument3, T4 argument4)
		{
			if (action != null)
			{
				action(argument1, argument2, argument3, argument4);
			}
		}

		public static string NicifyTimeString(this TimeSpan ts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ts.Days > 0)
			{
				stringBuilder.AppendFormat("{0}{2} {1}{3}", ts.Days, ts.Hours, Strings.Get("TIME_SHORTFORM_DAY"), Strings.Get("TIME_SHORTFORM_HOURS"));
			}
			else if (ts.Hours > 0)
			{
				stringBuilder.AppendFormat("{0}{2} {1}{3}", ts.Hours, ts.Minutes, Strings.Get("TIME_SHORTFORM_HOURS"), Strings.Get("TIME_SHORTFORM_MINUTES"));
			}
			else if (ts.Minutes > 0)
			{
				stringBuilder.AppendFormat("{0}{2} {1}{3}", ts.Minutes, ts.Seconds, Strings.Get("TIME_SHORTFORM_MINUTES"), Strings.Get("TIME_SHORTFORM_SECONDS"));
			}
			else if (ts.TotalSeconds > 0.0)
			{
				stringBuilder.AppendFormat("{0}{1}", ts.Seconds, Strings.Get("TIME_SHORTFORM_SECONDS"));
			}
			else
			{
				stringBuilder.AppendFormat("0{0}", Strings.Get("TIME_SHORTFORM_SECONDS"));
			}
			return stringBuilder.ToString();
		}

		public static string NicifyTimeStringSubwayStyle(this TimeSpan ts)
		{
			return "{0:00}:{1:00}:{2:00}".Populate((int)ts.TotalHours, ts.Minutes, ts.Seconds);
		}

		public static Texture2D Circle(this Texture2D tex, int x, int y, int r, Color color)
		{
			float num = r * r;
			for (int i = 0; i < tex.width; i++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					if ((float)((x - i) * (x - i) + (y - j) * (y - j)) < num)
					{
						tex.SetPixel(i, j, color);
					}
				}
			}
			return tex;
		}

		public static string FormatSI(this int num)
		{
			if (num >= 100000000)
			{
				return ((double)num / 1000000.0).ToString("0.#M");
			}
			if (num >= 1000000)
			{
				return ((double)num / 1000000.0).ToString("0.#M");
			}
			if (num >= 100000)
			{
				return ((double)num / 1000.0).ToString("0.#k");
			}
			if (num >= 10000)
			{
				return ((double)num / 1000.0).ToString("0.#k");
			}
			return num.ToString("#0");
		}
	}
}
