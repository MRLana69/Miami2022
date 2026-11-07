using System;
using Data.Save;

namespace Data.Managers
{
	public static class FacebookWeeklyGiftManager
	{
		public static void GiftWasClaimed()
		{
			PlayerSave.weeklyGiftUnlockTime = GetNextUnlockTime();
		}

		private static DateTime GetNextUnlockTime()
		{
			DateTime nextWeekday = GetNextWeekday(UserSave.lastKnownServerTime, DayOfWeek.Saturday);
			return new DateTime(nextWeekday.Year, nextWeekday.Month, nextWeekday.Day, 11, 59, 59, DateTimeKind.Utc);
		}

		private static DateTime GetNextWeekday(DateTime startDate, DayOfWeek weekday)
		{
			int num = (weekday - startDate.DayOfWeek + 7) % 7;
			if (num == 0 && DateTime.Compare(t2: new DateTime(startDate.Year, startDate.Month, startDate.Day, 12, 0, 0), t1: startDate) >= 0)
			{
				num = 7;
			}
			return startDate.AddDays(num);
		}

		public static bool IsGiftAvailable()
		{
			return PlayerSave.weeklyGiftUnlockTime < UserSave.lastKnownServerTime && SocialManager.instance.facebookIsLoggedIn;
		}
	}
}
