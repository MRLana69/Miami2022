using System;

namespace SYBO.Components.Utilities.DateTimeUtilities
{
	public static class DateTimeUtils
	{
		private enum Months
		{
			Jan = 1,
			Feb = 2,
			Mar = 3,
			Apr = 4,
			May = 5,
			Jun = 6,
			Jul = 7,
			Aug = 8,
			Sep = 9,
			Oct = 10,
			Nov = 11,
			Dec = 12
		}

		private static readonly char[] RFC3339SplitSymbols = new char[6] { '-', 'T', ':', '.', 'Z', '+' };

		private static readonly char[] RFC822SplitSymbols = new char[3] { ' ', ',', ':' };

		public const int TicksPerSecond = 10000000;

		public const int TicksPerMillisecond = 10000;

		public const int NanosecondsPerTicks = 100;

		public static readonly DateTime FallbackDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static double TotalSecondsSinceEpoch()
		{
			return DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		}

		public static bool TryParseRFC3339(string timestamp, out DateTime parsedDateTime)
		{
			timestamp = timestamp.ToUpper();
			bool flag = timestamp.Contains(".");
			bool flag2 = !timestamp.Contains("Z");
			int num = (flag2 ? ((!timestamp.Contains("+")) ? 1 : (-1)) : 0);
			string[] array = timestamp.Split(RFC3339SplitSymbols, StringSplitOptions.RemoveEmptyEntries);
			try
			{
				bool flag3 = false;
				int num2 = int.Parse(array[5]);
				if (num2 == 60)
				{
					num2 = 59;
					flag3 = true;
				}
				DateTime dateTime = new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]), int.Parse(array[4]), num2, DateTimeKind.Utc);
				if (flag)
				{
					int length = array[6].Length;
					double num3 = Math.Pow(10.0, length);
					double num4 = 10000000.0 / num3;
					int num5 = (int)Math.Round((double)int.Parse(array[6]) * num4);
					dateTime = dateTime.AddTicks(num5);
				}
				if (flag2)
				{
					int num6 = (flag ? 1 : 0);
					dateTime = dateTime.AddHours(int.Parse(array[6 + num6]) * num).AddMinutes(int.Parse(array[7 + num6]) * num);
				}
				if (flag3)
				{
					dateTime = dateTime.AddSeconds(1.0);
				}
				parsedDateTime = dateTime;
				return true;
			}
			catch (Exception)
			{
				parsedDateTime = FallbackDateTime;
				return false;
			}
		}

		public static string ToStringRFC3339(DateTime time)
		{
			time = time.ToUniversalTime();
			DateTime dateTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, DateTimeKind.Utc);
			return string.Format("{0}:{1:D2}:{2:D2}T{3:D2}:{4:D2}:{5:D2}.{6:D9}Z", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, (time.Ticks - dateTime.Ticks) * 100);
		}

		public static bool TryParseRFC822(string timestamp, out DateTime parsedDateTime)
		{
			string[] array = timestamp.Split(RFC822SplitSymbols, StringSplitOptions.RemoveEmptyEntries);
			try
			{
				int month = (int)Enum.Parse(typeof(Months), array[2]);
				DateTime dateTime = new DateTime(int.Parse(array[3]), month, int.Parse(array[1]), int.Parse(array[4]), int.Parse(array[5]), int.Parse(array[6]), DateTimeKind.Utc);
				parsedDateTime = dateTime;
				return true;
			}
			catch (Exception)
			{
				parsedDateTime = FallbackDateTime;
				return false;
			}
		}
	}
}
