using System.Collections.Generic;
using SYBO.Components.Analytics.Integration;

namespace Data.Save
{
	public class SyboAnalyticsSave : DataManager<SyboAnalyticsSave>
	{
		public static string syboAnalyticsUserId
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsUserId;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsUserId = value;
			}
		}

		public static string localSyboAnalyticsUserId
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsUserId;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsUserId = value;
			}
		}

		public static int syboAnalyticsCurrentSession
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsCurrentSession;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsCurrentSession = value;
			}
		}

		public static int persistedSessionEventNumber
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.cloud.persistedSessionEventNumber;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.cloud.persistedSessionEventNumber = value;
			}
		}

		public static int syboAnalyticsGamesSinceFlush
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsGamesSinceFlush;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsGamesSinceFlush = value;
			}
		}

		public static string syboAnalyticsPreviousClientVersion
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsPreviousClientVersion;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsPreviousClientVersion = value;
			}
		}

		public static string syboAnalyticsOperatingSystem
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsOperatingSystem;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsOperatingSystem = value;
			}
		}

		public static string syboAnalyticsDeviceID
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsDeviceID;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsDeviceID = value;
			}
		}

		public static string syboAnalyticsOSLanguage
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsOSLanguage;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsOSLanguage = value;
			}
		}

		public static DeviceInfo.PerformanceLevel syboAnalyticsPerformanceLevel
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsPerformanceLevel;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsPerformanceLevel = value;
			}
		}

		public static int syboAnalyticsGpuScore
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsGpuScore;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsGpuScore = value;
			}
		}

		public static string syboAnalyticsOnlineSettingsLocale
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsOnlineSettingsLocale;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsOnlineSettingsLocale = value;
			}
		}

		public static Dictionary<string, string> syboAnalyticsPendingOfferRewardToOfferIdMap
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.local.syboAnalyticsPendingOfferRewardToOfferIdMap;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.local.syboAnalyticsPendingOfferRewardToOfferIdMap = value;
			}
		}

		public static Dictionary<string, string> syboAnalyticsUnhandledAbTestData
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsUnhandledAbTestData;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsUnhandledAbTestData = value;
			}
		}

		public static SampleState syboAnalyticsSampleState
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsSampleState;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsSampleState = value;
			}
		}

		public static string syboAggregatedDataString
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.cloud.syboAggregatedDataString;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.cloud.syboAggregatedDataString = value;
			}
		}

		public static bool kilooAnalyticsHasLoggedSyboUserId
		{
			get
			{
				return DataManager<SyboAnalyticsSave>.cloud.kilooAnalyticsHasLoggedSyboUserId;
			}
			set
			{
				DataManager<SyboAnalyticsSave>.cloud.kilooAnalyticsHasLoggedSyboUserId = value;
			}
		}

		public static bool isSyboAnalyticsUserIdAssigned
		{
			get
			{
				return !string.IsNullOrEmpty(DataManager<SyboAnalyticsSave>.cloud.syboAnalyticsUserId);
			}
		}

		public static void SyboAnalyticsAddUnhandledAbTestData(string testTag, int segmentIndex)
		{
			if (!syboAnalyticsUnhandledAbTestData.ContainsKey(testTag))
			{
				syboAnalyticsUnhandledAbTestData.Add(testTag, segmentIndex.ToString());
			}
		}

		public static void SyboAnalyticsClearUnhandledAbTestData()
		{
			if (0 < syboAnalyticsUnhandledAbTestData.Count)
			{
				syboAnalyticsUnhandledAbTestData.Clear();
			}
		}

		public static void SyboAnalyticsAddPendingOfferReward(string rewardAmount, long offerId)
		{
			if (!syboAnalyticsPendingOfferRewardToOfferIdMap.ContainsKey(rewardAmount))
			{
				syboAnalyticsPendingOfferRewardToOfferIdMap.Add(rewardAmount, offerId.ToString());
			}
		}

		public static void SyboAnalyticsRemovePendingOffer(string rewardAmount)
		{
			if (syboAnalyticsPendingOfferRewardToOfferIdMap.ContainsKey(rewardAmount))
			{
				syboAnalyticsPendingOfferRewardToOfferIdMap.Remove(rewardAmount);
			}
		}

		public static void SyboAnalyticsClearPendingOffers()
		{
			syboAnalyticsPendingOfferRewardToOfferIdMap.Clear();
		}

		public static Dictionary<string, string> SyboAnalyticsGetPendingOfferRewards()
		{
			return new Dictionary<string, string>(syboAnalyticsPendingOfferRewardToOfferIdMap);
		}
	}
}
