using System;
using System.Collections.Generic;
using Info.VideoRewardCooldowns;
using Kiloo.Common.Ads;

namespace Data.Save
{
	public class AdsSave : DataManager<AdsSave>
	{
		public static bool behavioralAdsAllowed
		{
			get
			{
				return DataManager<AdsSave>.cloud.behavioralAdsAllowed;
			}
			set
			{
				DataManager<AdsSave>.cloud.behavioralAdsAllowed = value;
			}
		}

		public static DateTime scheduledFirstInterstitialShowTime
		{
			get
			{
				return DataManager<AdsSave>.local.scheduledFirstInterstitialShowTime;
			}
			set
			{
				DataManager<AdsSave>.local.scheduledFirstInterstitialShowTime = value;
			}
		}

		public static Dictionary<string, string> lastShowedProviders
		{
			get
			{
				return DataManager<AdsSave>.local.lastShowedProviders;
			}
			set
			{
				DataManager<AdsSave>.local.lastShowedProviders = value;
			}
		}

		public static Dictionary<AdType, string> lastShowedCacheSlots
		{
			get
			{
				return DataManager<AdsSave>.local.lastShowedCacheSlotIDs;
			}
			set
			{
				DataManager<AdsSave>.local.lastShowedCacheSlotIDs = value;
			}
		}

		public static InterstitialStats interstitialStats
		{
			get
			{
				return DataManager<AdsSave>.cloud.interstitialStats;
			}
			set
			{
				DataManager<AdsSave>.cloud.interstitialStats = value;
			}
		}

		public static int currentIntroVideoAdPrizeIndex
		{
			get
			{
				return DataManager<AdsSave>.cloud.currentIntroVideoAdPrizeIndex;
			}
			set
			{
				DataManager<AdsSave>.cloud.currentIntroVideoAdPrizeIndex = value;
			}
		}

		public static int currentRandomVideoAdPrizeIndex
		{
			get
			{
				return DataManager<AdsSave>.cloud.currentRandomVideoAdPrizeIndex;
			}
			set
			{
				DataManager<AdsSave>.cloud.currentRandomVideoAdPrizeIndex = value;
			}
		}

		public static int currentVideoAdPrizeSeed
		{
			get
			{
				return DataManager<AdsSave>.cloud.currentVideoAdPrizeSeed;
			}
			set
			{
				DataManager<AdsSave>.cloud.currentVideoAdPrizeSeed = value;
			}
		}

		public static int videosWatchedSinceDailyKeys
		{
			get
			{
				return DataManager<AdsSave>.cloud.videosWatchedSinceDailyKeys;
			}
			set
			{
				DataManager<AdsSave>.cloud.videosWatchedSinceDailyKeys = value;
			}
		}

		public static Cooldowns cooldowns
		{
			get
			{
				return DataManager<AdsSave>.cloud.cooldowns;
			}
			set
			{
				DataManager<AdsSave>.cloud.cooldowns = value;
			}
		}
	}
}
