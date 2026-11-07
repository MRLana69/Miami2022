using System;
using System.Collections.Generic;
using Kiloo;
using WeeklyHunt;

namespace Data.Save
{
	public class EventsSave : DataManager<EventsSave>
	{
		private static bool _willShowSMBAfterDailyChallenge;

		public static List<bool> hasLoggedWeeklyHuntPeriod
		{
			get
			{
				return DataManager<EventsSave>.cloud.hasLoggedWeeklyHuntPeriod;
			}
			set
			{
				DataManager<EventsSave>.cloud.hasLoggedWeeklyHuntPeriod = value;
			}
		}

		public static string weeklyHuntProgressVersion
		{
			get
			{
				return DataManager<EventsSave>.cloud.weeklyHuntProgressVersion;
			}
			set
			{
				DataManager<EventsSave>.cloud.weeklyHuntProgressVersion = value;
			}
		}

		public static WeeklyHuntProgressData weeklyHuntProgressData
		{
			get
			{
				return DataManager<EventsSave>.cloud.weeklyHuntProgressData;
			}
			set
			{
				DataManager<EventsSave>.cloud.weeklyHuntProgressData = value;
			}
		}

		public static List<bool> requiredAlternativeHuntPrize
		{
			get
			{
				return DataManager<EventsSave>.cloud.requiredAlternativeHuntPrize;
			}
			set
			{
				DataManager<EventsSave>.cloud.requiredAlternativeHuntPrize = value;
			}
		}

		public static string wordHuntDailyWord
		{
			get
			{
				return DataManager<EventsSave>.cloud.wordHuntDailyWord;
			}
			set
			{
				DataManager<EventsSave>.cloud.wordHuntDailyWord = value;
			}
		}

		public static bool wordHuntPayedOut
		{
			get
			{
				return DataManager<EventsSave>.cloud.wordHuntPayedOut;
			}
			set
			{
				DataManager<EventsSave>.cloud.wordHuntPayedOut = value;
			}
		}

		public static IntMask wordHuntUnlockedMask
		{
			get
			{
				return DataManager<EventsSave>.cloud.wordHuntUnlockedMask;
			}
			set
			{
				DataManager<EventsSave>.cloud.wordHuntUnlockedMask = value;
			}
		}

		public static DateTime wordHuntExpireTime
		{
			get
			{
				return DataManager<EventsSave>.cloud.wordHuntExpireTime;
			}
			set
			{
				DataManager<EventsSave>.cloud.wordHuntExpireTime = value;
			}
		}

		public static int wordHuntWordsInRow
		{
			get
			{
				return DataManager<EventsSave>.cloud.wordHuntWordsInRow;
			}
			set
			{
				DataManager<EventsSave>.cloud.wordHuntWordsInRow = value;
			}
		}

		public static int wordHuntLastPayoutDayOfYear
		{
			get
			{
				return DataManager<EventsSave>.cloud.wordHuntLastPayoutDayOfYear;
			}
			set
			{
				DataManager<EventsSave>.cloud.wordHuntLastPayoutDayOfYear = value;
			}
		}

		public static TimeSpan wordHuntTimeLeft
		{
			get
			{
				return wordHuntExpireTime - CDateTime.UtcNow;
			}
		}

		public static bool willShowSMBAfterDailyChallenge
		{
			get
			{
				return _willShowSMBAfterDailyChallenge;
			}
			set
			{
				_willShowSMBAfterDailyChallenge = value;
			}
		}

		public static DateTime wordHuntExpireDeviceTime { get; set; }

		public static TimeSpan dailyWordDeviceTimeLeft
		{
			get
			{
				return wordHuntExpireDeviceTime - CDateTime.UtcNow;
			}
		}

		public static CharacterNameEventData characterNameEventData
		{
			get
			{
				return DataManager<EventsSave>.cloud.characterNameEventData;
			}
			set
			{
				DataManager<EventsSave>.cloud.characterNameEventData = value;
			}
		}

		public static long lastEventID
		{
			get
			{
				return DataManager<EventsSave>.cloud.lastEventID;
			}
			set
			{
				DataManager<EventsSave>.cloud.lastEventID = value;
			}
		}

		public static long lastTimeAnEventHasBeenTried
		{
			get
			{
				return DataManager<EventsSave>.cloud.lastTimeAnEventHasBeenTried;
			}
			set
			{
				DataManager<EventsSave>.cloud.lastTimeAnEventHasBeenTried = value;
			}
		}

		public static long lastTimeAnEventWasStarted
		{
			get
			{
				return DataManager<EventsSave>.cloud.lastTimeAnEventWasStarted;
			}
			set
			{
				DataManager<EventsSave>.cloud.lastTimeAnEventWasStarted = value;
			}
		}

		public static string lastEventItemID
		{
			get
			{
				return DataManager<EventsSave>.cloud.lastEventItemID;
			}
			set
			{
				DataManager<EventsSave>.cloud.lastEventItemID = value;
			}
		}

		public static long lastTimeEventPopupWasForceShown
		{
			get
			{
				return DataManager<EventsSave>.cloud.lastTimeEventPopupWasForceShown;
			}
			set
			{
				DataManager<EventsSave>.cloud.lastTimeEventPopupWasForceShown = value;
			}
		}

		public static bool shouldRestoreItemAfterEvent
		{
			get
			{
				return DataManager<EventsSave>.cloud.shouldRestoreItemAfterEvent;
			}
			set
			{
				DataManager<EventsSave>.cloud.shouldRestoreItemAfterEvent = value;
			}
		}
	}
}
