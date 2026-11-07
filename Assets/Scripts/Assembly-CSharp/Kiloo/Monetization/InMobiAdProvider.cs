using System;
using Kiloo.Common.Ads;
using Kiloo.Monetization.InmobiPlugin;

namespace Kiloo.Monetization
{
	public class InMobiAdProvider : AbstractAdProvider
	{
		public const string ID = "inmobi";

		private bool _completedVideo;

		private bool _addedListeners;

		public override string id
		{
			get
			{
				return "inmobi";
			}
		}

		public override bool supportsMulticaching
		{
			get
			{
				return false;
			}
		}

		protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
		{
			bool flag = Inmobi.IsSupportedOnPlatform();
			bool flag2 = !string.IsNullOrEmpty(configuration.appID);
			_supportedAdTypes[AdType.Interstitial] = flag && flag2;
			_supportedAdTypes[AdType.Video] = flag && flag2;
		}

		protected override void OnInitialize(AdProviderConfiguration configuration)
		{
			if (!_addedListeners)
			{
				Inmobi.willShowAd += (string cacheId) =>
				{
					_completedVideo = false;
					AbstractAdProvider.eventInvoker.ShowedAd(_showCacheSlot.adType, id, GetTierForCacheId(cacheId), _showCacheSlot, _showAdLocation);
				};
				Inmobi.onCacheAd += (string cacheId) =>
				{
					if (_cacheIDToSlot.ContainsKey(cacheId))
					{
						AbstractAdProvider.eventInvoker.CachedAd(_cacheIDToSlot[cacheId].adType, id, GetTierForCacheId(cacheId), _cacheIDToSlot[cacheId], false);
						_cacheIDToSlot.Remove(cacheId);
					}
					else
					{
						LogError("Received Cached Ad callback for unrecognised cache ID: " + cacheId);
					}
				};
				Inmobi.onFailToLoadAd += (string cacheId) =>
				{
					if (_cacheIDToSlot.ContainsKey(cacheId))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(_cacheIDToSlot[cacheId].adType, id, GetTierForCacheId(cacheId), _cacheIDToSlot[cacheId]);
					}
					else
					{
						LogError("Received Fail To Load Ad callback for unrecognised cache ID: " + cacheId);
						foreach (AdCacheSlotInfo value in _cacheIDToSlot.Values)
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(value.adType, id, GetTierForCacheId(cacheId), value);
						}
						_cacheIDToSlot.Clear();
					}
				};
				Inmobi.onCompleteIncentive += (string cacheId) =>
				{
					_completedVideo = true;
				};
				Inmobi.onDismissAd += (string cacheId) =>
				{
					if (_completedVideo)
					{
						_completedVideo = false;
						AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, GetTierForCacheId(cacheId), _showCacheSlot, _showAdLocation);
					}
					AbstractAdProvider.eventInvoker.DismissedAd(_showCacheSlot.adType, id, GetTierForCacheId(cacheId), _showCacheSlot, _showAdLocation);
				};
				Inmobi.onInitSuccessfully += (string key) =>
				{
					FinishInitialization(true);
				};
				Inmobi.onFailToDisplayAd += (string key) =>
				{
					AbstractAdProvider.eventInvoker.FailedToShowAd(_showCacheSlot.adType, id, GetCurrentTier(_showCacheSlot), _showCacheSlot, _showAdLocation);
				};
				_addedListeners = true;
			}
			try
			{
				Inmobi.Initialize(configuration.appID);
			}
			catch (Exception)
			{
				FinishInitialization(false);
			}
		}

		protected override bool HasCachedInterstitial(string cacheID)
		{
			return Inmobi.HasCachedAd(cacheID);
		}

		protected override void CacheInterstitial(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			Inmobi.CacheAd(cacheID);
		}

		protected override void ShowInterstitial(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showCacheSlot = adCacheSlotInfo;
			_showAdLocation = adLocationInfo;
			Inmobi.ShowAd(cacheID);
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			Inmobi.CacheAd(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showCacheSlot = adCacheSlotInfo;
			_showAdLocation = adLocationInfo;
			Inmobi.ShowAd(cacheID);
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			return Inmobi.HasCachedAd(cacheID);
		}

		private void LogError(string message)
		{
			AdSystemLog.LogError("[InMobi] " + message);
		}
	}
}
