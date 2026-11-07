using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;

namespace Kiloo.Monetization.FacebookAds
{
	public class FacebookAdProvider : AbstractAdProvider
	{
		public const string ID = "facebook";

		private bool _addedListeners;

		private bool _videoWatched;

		private readonly Dictionary<AdCacheSlotInfo, string> _adCacheSlotToCacheId = new Dictionary<AdCacheSlotInfo, string>();

		public override string id
		{
			get
			{
				return "facebook";
			}
		}

		public override bool supportsMulticaching
		{
			get
			{
				return false;
			}
		}

		public override List<AdType> adTypesWithClickCallbackSupport
		{
			get
			{
				List<AdType> list = new List<AdType>();
				list.Add(AdType.Interstitial);
				return list;
			}
		}

		protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
		{
			bool value = FacebookNativeAds.IsSupportedOnPlatform();
			_supportedAdTypes[AdType.Interstitial] = value;
			_supportedAdTypes[AdType.Video] = value;
		}

		protected override void OnInitialize(AdProviderConfiguration configuration)
		{
			if (!_addedListeners)
			{
				FacebookNativeAds.onNativeInitialize += base.FinishInitialization;
				FacebookNativeAds.onNativeDidLoad += (string cacheID) =>
				{
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID], false);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						LogError("Received cache success for cacheId not in dictionary: " + cacheID);
					}
				};
				FacebookNativeAds.onNativeAdError += (string cacheID) =>
				{
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID]);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						LogError("Received cache failed for cacheId not in dictionary: " + cacheID);
						foreach (AdCacheSlotInfo item in _cacheIDToSlot.Values.Where((AdCacheSlotInfo adSlot) => adSlot.adType == AdType.Interstitial))
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(item.adType, id, GetTierForCacheId(cacheID), item);
						}
						_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
					}
				};
				FacebookNativeAds.onNativeAdShown += (string placement) =>
				{
					AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
				};
				FacebookNativeAds.onNativeAdClosed += (string placement) =>
				{
					AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
				};
				FacebookNativeAds.onNativeAdClicked += (string placement) =>
				{
					AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, "facebook", GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
				};
				FacebookNativeAds.onVideoDidLoad += (string cacheID) =>
				{
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID], false);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						LogError("Received cache success for cacheId not in dictionary: " + cacheID);
					}
				};
				FacebookNativeAds.onVideoError += (string cacheID) =>
				{
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID]);
						_cacheIDToSlot.Remove(cacheID);
					}
					else if (_showVideoCacheSlot != null && !_showVideoCacheSlot.id.Equals(AbstractAdProvider.DEFAULT_SHOW_SLOT.id))
					{
						if (_videoWatched)
						{
							_videoWatched = false;
							AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
						}
						AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Video, id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
						_showVideoCacheSlot = AbstractAdProvider.DEFAULT_SHOW_SLOT;
					}
					else
					{
						LogError("Received video error but no show slot set and cache ID not found as waiting. CacheID: " + cacheID);
						foreach (AdCacheSlotInfo item2 in _cacheIDToSlot.Values.Where((AdCacheSlotInfo adSlot) => adSlot.adType == AdType.Video))
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, GetTierForCacheId(cacheID), item2);
						}
						_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Video);
					}
				};
				FacebookNativeAds.onVideoViewStarted += (string cacheID) =>
				{
					AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
				};
				FacebookNativeAds.onVideoViewFinished += (string cacheID) =>
				{
					_videoWatched = true;
				};
				FacebookNativeAds.onVideoViewClosed += (string cacheID) =>
				{
					if (_videoWatched)
					{
						_videoWatched = false;
						AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
					}
					AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
					_showVideoCacheSlot = AbstractAdProvider.DEFAULT_SHOW_SLOT;
				};
				_addedListeners = true;
			}
			try
			{
				FacebookNativeAds.Initialize();
			}
			catch (Exception)
			{
				FinishInitialization(false);
			}
		}

		protected override void CacheInterstitial(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_adCacheSlotToCacheId[adCacheSlotInfo] = cacheID;
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			FacebookNativeAds.CacheNativeAd(cacheID);
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
		}

		protected override void ShowInterstitial(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			FacebookNativeAds.ShowInterstitial(cacheID);
			_showInterstitialCacheSlot = adCacheSlotInfo;
			_showInterstitialAdLocation = adLocationInfo;
		}

		protected override bool HasCachedInterstitial(string cacheID)
		{
			return FacebookNativeAds.IsNativeAdLoaded(cacheID);
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_adCacheSlotToCacheId[adCacheSlotInfo] = cacheID;
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			FacebookNativeAds.LoadVideoAd(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheId)
		{
			_showVideoCacheSlot = adCacheSlotInfo;
			_showVideoAdLocation = adLocationInfo;
			FacebookNativeAds.ShowVideo(cacheId);
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			return FacebookNativeAds.IsCachedVideoAvailable(cacheID);
		}

		[Conditional("ENABLE_FACEBOOKADS_LOGS")]
		private void Log(string message)
		{
		}

		private void LogWarning(string message)
		{
		}

		private void LogError(string message)
		{
			SDebug.LogError("[FacebookAdProvider] " + message);
		}
	}
}
