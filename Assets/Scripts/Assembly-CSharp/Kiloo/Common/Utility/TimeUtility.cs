using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class TimeUtility
	{
		public static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		[Pure]
		public static long ToUnixTime(this DateTime time)
		{
			double totalSeconds = (time.ToUniversalTime() - epoch).TotalSeconds;
			return (long)totalSeconds;
		}

		[Pure]
		public static DateTime FromUnixTime(long unixtime)
		{
			return epoch.AddSeconds(unixtime);
		}

		[Pure]
		public static string ToJSON(this DateTime value)
		{
			return value.ToUniversalTime().ToString("O", DateTimeFormatInfo.InvariantInfo);
		}

		[Pure]
		public static string ToSortableNumber(this DateTime value)
		{
			return value.ToUniversalTime().ToString("yyyyMMddHHmmssfffffff", DateTimeFormatInfo.InvariantInfo);
		}

		[Pure]
		public static bool TryParseSortableNumber(string str, out DateTime result)
		{
			return DateTime.TryParseExact(str, "yyyyMMddHHmmssfffffff", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result);
		}

		[Pure]
		public static string ToRFC1123(this DateTime value)
		{
			return value.ToUniversalTime().ToString("R", DateTimeFormatInfo.InvariantInfo);
		}

		[Pure]
		public static bool TryParseRFC1123(string str, out DateTime result)
		{
			return DateTime.TryParseExact(str, DateTimeFormatInfo.InvariantInfo.RFC1123Pattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result);
		}

		[Pure]
		public static long RFC1123StringToUnixTime(string str)
		{
			DateTime result;
			if (str != null && TryParseRFC1123(str, out result))
			{
				return result.ToUnixTime();
			}
			return 0L;
		}

		[Pure]
		public static TimeSpan GetCurrentTimeZoneUTCOffset(this DateTime time)
		{
			return TimeZone.CurrentTimeZone.GetUtcOffset(time);
		}

		[Pure]
		public static DateTimeOffset Trim(this DateTimeOffset time, long ticks)
		{
			return time.AddTicks(-(time.Ticks % ticks));
		}

		[Pure]
		public static DateTimeOffset TrimToWholeSeconds(this DateTimeOffset time)
		{
			return time.Trim(10000000L);
		}

		[Pure]
		public static DateTimeOffset TrimToWholeMilliSeconds(this DateTimeOffset time)
		{
			return time.Trim(10000L);
		}

		[Pure]
		public static DateTime Trim(this DateTime time, long ticks)
		{
			return time.AddTicks(-(time.Ticks % ticks));
		}

		[Pure]
		public static DateTime TrimToWholeSeconds(this DateTime time)
		{
			return time.Trim(10000000L);
		}

		[Pure]
		public static DateTime TrimToWholeMilliSeconds(this DateTime time)
		{
			return time.Trim(10000L);
		}
	}
}
