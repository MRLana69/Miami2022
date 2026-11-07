using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.ChartboostPlugin;

namespace Kiloo.Monetization
{
	public class ChartboostAdProvider : AbstractAdProvider
	{
		public const string ID = "chartboost";

		private bool _addedListeners;

		private bool _completedRewardedVideo;

		public override string id
		{
			get
			{
				return "chartboost";
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
				list.Add(AdType.Video);
				list.Add(AdType.Interstitial);
				return list;
			}
		}

		protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
		{
			bool flag = Chartboost.IsSupportedOnPlatform();
			bool flag2 = !string.IsNullOrEmpty(configuration.appID) && !string.IsNullOrEmpty(configuration.appSignature);
			bool flag3 = !DeviceUtility.isKindleDevice;
			_supportedAdTypes[AdType.Interstitial] = flag && flag2;
			_supportedAdTypes[AdType.Video] = flag && flag2 && flag3;
		}

		protected override void OnInitialize(AdProviderConfiguration configuration)
		{
			if (!_addedListeners)
			{
				Chartboost.onInitFinished += base.FinishInitialization;
				Chartboost.onClickInterstitial += (string key) =>
				{
					AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
				};
				Chartboost.onFailToLoadInterstitial += (string cacheID) =>
				{
					string tierForCacheId = GetTierForCacheId(cacheID);
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, tierForCacheId, _cacheIDToSlot[cacheID]);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						AdSystemLog.LogError("Received cache failed for cacheID not in dictionary: " + cacheID);
						foreach (AdCacheSlotInfo item in _cacheIDToSlot.Values.Where((AdCacheSlotInfo location) => location.adType == AdType.Interstitial))
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, tierForCacheId, item);
						}
						_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
					}
				};
				Chartboost.onCacheInterstitial += (string cacheID) =>
				{
					string tierForCacheId = GetTierForCacheId(cacheID);
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, id, tierForCacheId, _cacheIDToSlot[cacheID], false);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						AdSystemLog.LogError("Received cached success for cacheID not in dictionary: " + cacheID);
					}
				};
				Chartboost.onDismissInterstitial += (string key) =>
				{
					AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
				};
				Chartboost.onDisplayInterstitial += (string key) =>
				{
					AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
				};
				Chartboost.onCacheRewardedVideo += (string cacheID) =>
				{
					string tierForCacheId = GetTierForCacheId(cacheID);
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, tierForCacheId, _cacheIDToSlot[cacheID], false);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						AdSystemLog.LogError("Received cached success for cacheID not in dictionary: " + cacheID);
					}
				};
				Chartboost.onFailToLoadRewardedVideo += (string cacheID) =>
				{
					string tierForCacheId = GetTierForCacheId(cacheID);
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, tierForCacheId, _cacheIDToSlot[cacheID]);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						AdSystemLog.LogError("Received cache failed for cacheID not in dictionary: " + cacheID);
						foreach (AdCacheSlotInfo item2 in _cacheIDToSlot.Values.Where((AdCacheSlotInfo location) => location.adType == AdType.Video))
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, tierForCacheId, item2);
						}
						_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Video);
					}
				};
				Chartboost.onClickRewardedVideo += (string key) =>
				{
					AbstractAdProvider.eventInvoker.ClickedAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
				};
				Chartboost.onDisplayRewardedVideo += (string key) =>
				{
					_completedRewardedVideo = false;
					AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
				};
				Chartboost.onCompleteRewardedVideo += (string key) =>
				{
					_completedRewardedVideo = true;
				};
				Chartboost.onDismissRewardedVideo += (string key) =>
				{
					if (_completedRewardedVideo)
					{
						_completedRewardedVideo = false;
						AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
					}
					AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
				};
				_addedListeners = true;
			}
			try
			{
				Chartboost.StartSession(configuration.appID, configuration.appSignature);
			}
			catch (Exception)
			{
				FinishInitialization(false);
			}
		}

		protected override bool HasCachedInterstitial(string cacheID)
		{
			return Chartboost.HasCachedInterstitial(cacheID);
		}

		protected override void CacheInterstitial(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			Chartboost.CacheInterstitial(cacheID);
		}

		protected override void ShowInterstitial(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showInterstitialCacheSlot = adCacheSlotInfo;
			_showInterstitialAdLocation = adLocationInfo;
			Chartboost.ShowInterstitial(cacheID);
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			Chartboost.CacheRewardedVideo(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showVideoCacheSlot = adCacheSlotInfo;
			_showVideoAdLocation = adLocationInfo;
			Chartboost.ShowRewardedVideo(cacheID);
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			return Chartboost.HasCachedRewardedVideo(cacheID);
		}
	}
}
