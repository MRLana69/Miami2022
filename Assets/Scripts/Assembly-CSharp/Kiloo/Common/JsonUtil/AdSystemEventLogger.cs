using Kiloo.Common.Ads;

namespace Kiloo.Common.JsonUtil
{
	public class AdSystemEventLogger : AdEventListenerAdapter
	{
		public bool clicked { get; private set; }

		public CacheFailSource cacheFailSource { get; private set; }

		public AdState adState { get; private set; }

		public AdSystemEventLogger()
		{
			AdTrackingSystemLogger.SetAdSystemEventLogger(this);
		}

		public override void ShowedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			AdTrackingSystemLogger.LogFlow(adType, adLocationInfo.id, providerTier, "Attempt to show ad", "ShowedAd");
			adState = AdState.ShownButNotFinished;
		}

		public override void FailedToShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			AdTrackingSystemLogger.LogFlow(adType, adLocationInfo.id, providerTier, "Attempt to show ad", "FailedShowingAd");
		}

		public override void CachedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
		{
			string text = ((!wasAlreadyCached) ? "Success" : "AlreadyCached");
			if (GetCurrentProviderTierForCacheSlot(adCacheSlotInfo) != providerTier && !wasAlreadyCached)
			{
				text += "NotCurrent";
			}
			AdTrackingSystemLogger.LogAttemptToCacheResult(adType, providerID, text);
			cacheFailSource = CacheFailSource.NoFail;
		}

		public override void DismissedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			AdTrackingSystemLogger.LogFlow(adType, adLocationInfo.id, providerTier, "Result of ad shown", GetResultOfVideoShown(adType, providerID));
			clicked = false;
			adState = AdState.NotShown;
		}

		public override void FinishedWatchingVideo(string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			adState = AdState.ShownAndFinished;
		}

		public override void FailedToCacheAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
		{
			string text = "Failed";
			string currentProviderTierForCacheSlot = GetCurrentProviderTierForCacheSlot(adCacheSlotInfo);
			if (currentProviderTierForCacheSlot != providerTier)
			{
				text += "NotCurrent";
			}
			AdTrackingSystemLogger.LogAttemptToCacheResult(adType, providerTier, text);
			cacheFailSource = CacheFailSource.CacheFail;
		}

		public override void CachingTimedOut(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
		{
			AdTrackingSystemLogger.LogAttemptToCacheResult(adType, providerTier, "Timedout");
			cacheFailSource = CacheFailSource.Timeout;
		}

		public override void PriorityListWrapped(string waterfallID)
		{
			if (cacheFailSource == CacheFailSource.Timeout || cacheFailSource == CacheFailSource.CacheFail)
			{
				cacheFailSource = CacheFailSource.Waterfall;
			}
		}

		public override void ClickedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			clicked = true;
		}

		private string GetResultOfVideoShown(AdType adType, string providerId)
		{
			AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(providerId);
			if (clicked)
			{
				return string.Format("{0}{1}", GetAdShownPrefix(adType), "Clicked");
			}
			if (adProvider.adTypesWithClickCallbackSupport.Contains(adType))
			{
				return string.Format("{0}{1}", GetAdShownPrefix(adType), "Closed");
			}
			return string.Format("{0}{1}", GetAdShownPrefix(adType), "NoClickInfo");
		}

		private string GetAdShownPrefix(AdType adType)
		{
			if (adType == AdType.Video)
			{
				return GetVideoShownPrefix();
			}
			return adType.ToString();
		}

		private string GetVideoShownPrefix()
		{
			switch (adState)
			{
			case AdState.ShownButNotFinished:
				return string.Format("{0}{1}", AdType.Video, "Early");
			case AdState.ShownAndFinished:
				return string.Format("{0}{1}", AdType.Video, "Finished");
			default:
				return string.Empty;
			}
		}

		private string GetIdOfAdlocationBeingShown()
		{
			return AdSystemManager.GetIdOfAdlocationBeingShown();
		}

		private string GetCurrentProviderIDForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
		{
			return AdSystemManager.GetCurrentProviderIDForCacheSlot(adCacheSlotInfo);
		}

		private string GetCurrentProviderTierForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
		{
			return AdSystemManager.GetCurrentProviderTierForCacheSlot(adCacheSlotInfo);
		}
	}
}
