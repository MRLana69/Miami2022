namespace Kiloo.Common.Ads
{
	public interface IAdEventListener
	{
		void WillShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo);

		void ShowedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo);

		void CachedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached);

		void DismissedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo);

		void StartedCachingAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo);

		void FinishedWatchingVideo(string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo);

		void FailedToCacheAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo);

		void FailedToShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo);

		void FailedToInitAdProvider(string providerID);

		void CachingTimedOut(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo);

		void ClickedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo);

		void ReachedDailyInterstitialLimit(int limit);

		void ReachedHourlyInterstitialLimit(int limit);

		void ReachedNewUserInterstitialLimit(int limit);

		void ReachedTimeBetweenAdsInterstitialLimit(int limit);

		void ChangedProvider(AdType adType, AdProviderInfo newProviderInfo, string changedWaterfallID);

		void PriorityListWrapped(string waterfallID);
	}
}
