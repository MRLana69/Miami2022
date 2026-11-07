using System;
using System.Collections.Generic;

namespace Kiloo.Common.Ads
{
	public interface IAdSettingsProvider
	{
		IList<AdProviderInfo> GetPriorityList(string waterfallID, AdType adType, AdCacheSlotInfo adCacheSlotInfo);

		TimeSpan GetCacheTimeoutLimit(string providerID, AdType adType);

		TimeSpan? GetTimeLimitForAdProvider(string providerID, AdType adType);

		TimeSpan GetInterstitialShowInterval();

		bool OverrideInterstitialShowIntervalForLocation(string locationID, ref TimeSpan globalInterval);

		uint GetGracePeriodValue();

		DateTime GetScheduledFirstInterstitialShowTime();

		bool GetAvailabilityForLocation(string adLocationID);

		bool GetAvailabilityForCacheSlot(string adCacheSlotID);

		Dictionary<AdCacheSlotInfo, string> GetCacheIDsForProviderTier(string providerTier, AdType adType);

		Dictionary<AdCacheSlotInfo, List<string>> GetAllCacheIDsForProvider(string provider, AdType adType);

		string GetCacheID(string adProviderTier, AdCacheSlotInfo adCacheSlotInfo);

		string GetWaterfallID(AdType adType);

		string GetWaterfallID(string adLocationID);

		string GetLatestVersionForWaterfall(AdCacheSlot adCacheSlot);

		string[] GetAdProvidersToInitialize();

		AdProviderConfiguration GetConfigurationForProvider(string providerID);

		int GetFailCountTolerance(string adProvider, AdType adType);

		void SetDebugPriorityListVersion(AdType adType, string version);

		int GetMaxInterstitialImpressionsPerHour();

		int GetMaxInterstitialImpressionsPerDay();

		bool AreInterstitialsAllowed();

		int GetProviderWaitWhileCachingTimeLimit(string adLocationID, string providerID);

		bool TryGetProviderOverrideSupportedOnPlatform(AdType adType, string providerID, out string supportedDeviceVersion);

		Dictionary<string, bool> GetDebugProviderListState(AdType adType);
	}
}
