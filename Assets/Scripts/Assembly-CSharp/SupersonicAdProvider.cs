using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.SupersonicPlugin;
using UnityEngine;

public class SupersonicAdProvider : AbstractAdProvider
{
	public const string ID = "supersonic";

	private bool _addedListeners;

	private bool _videoWatched;

	private bool _isVideoCaching;

	public override string id
	{
		get
		{
			return "supersonic";
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
		bool flag = !string.IsNullOrEmpty(configuration.appID);
		_supportedAdTypes[AdType.Interstitial] = Supersonic.IsSupportedOnPlatform(AdType.Interstitial) && flag;
		_supportedAdTypes[AdType.Video] = Supersonic.IsSupportedOnPlatform(AdType.Video) && flag;
	}

	protected override void OnInitialize(AdProviderConfiguration configuration)
	{
		if (!_addedListeners)
		{
			Supersonic.onInitFinished += base.FinishInitialization;
			Supersonic.onRewardedVideoAdOpened += () =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
			};
			Supersonic.onRewardedVideoAdRewarded += (string key) =>
			{
				_videoWatched = true;
			};
			Supersonic.onRewardedVideoAdClosed += () =>
			{
				if (_videoWatched)
				{
					_videoWatched = false;
					AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
				}
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
			};
			Supersonic.onRewardedVideoAvailabilityChanged += (bool available) =>
			{
				if (!_cacheIDToSlot.Values.All((AdCacheSlotInfo adCacheSlotInfo) => adCacheSlotInfo.adType != AdType.Video))
				{
					if (_isVideoCaching && !available)
					{
						foreach (AdCacheSlotInfo item in _cacheIDToSlot.Values.Where((AdCacheSlotInfo adCacheSlotInfo) => adCacheSlotInfo.adType == AdType.Video))
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, GetCurrentTier(item), item);
						}
					}
					else if (available)
					{
						foreach (AdCacheSlotInfo item2 in _cacheIDToSlot.Values.Where((AdCacheSlotInfo adCacheSlotInfo) => adCacheSlotInfo.adType == AdType.Video))
						{
							AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, GetCurrentTier(item2), item2, false);
						}
					}
					_isVideoCaching = false;
					_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Video);
				}
			};
			Supersonic.onRewardedVideoAdShowFailed += (string key) =>
			{
				AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
			};
			Supersonic.onInterstitialAdClicked += () =>
			{
				AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
			};
			Supersonic.onInterstitialAdReady += () =>
			{
				foreach (AdCacheSlotInfo item3 in _cacheIDToSlot.Values.Where((AdCacheSlotInfo adCacheSlotInfo) => adCacheSlotInfo.adType == AdType.Interstitial))
				{
					AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, id, GetCurrentTier(item3), item3, false);
				}
				_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
			};
			Supersonic.onInterstitialAdLoadFailed += (string error) =>
			{
				foreach (AdCacheSlotInfo item4 in _cacheIDToSlot.Values.Where((AdCacheSlotInfo adCacheSlotInfo) => adCacheSlotInfo.adType == AdType.Interstitial))
				{
					AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, id, GetCurrentTier(item4), item4);
				}
				_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
			};
			Supersonic.onInterstitialAdOpened += () =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
			};
			Supersonic.onInterstitialAdShowFailed += (string key) =>
			{
				AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
			};
			Supersonic.onInterstitialAdClosed += () =>
			{
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, id, GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
			};
			_addedListeners = true;
		}
		try
		{
			string userId = DeviceUtility.GetDeviceIdentifier();
			Action<string> performInit = (string uid) =>
			{
				Supersonic.Init(configuration.appID, uid);
			};
			if (Application.platform == RuntimePlatform.Android)
			{
				Application.AdvertisingIdentifierCallback delegateMethod = (string advertisingId, bool trackingEnabled, string error) =>
				{
					if (trackingEnabled && !string.IsNullOrEmpty(advertisingId))
					{
						userId = advertisingId;
					}
					performInit(userId);
				};
				if (!Application.RequestAdvertisingIdentifierAsync(delegateMethod))
				{
					performInit(userId);
				}
			}
			else
			{
				performInit(userId);
			}
		}
		catch (Exception)
		{
			FinishInitialization(false);
		}
	}

	protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
	{
		_isVideoCaching = true;
		_cacheIDToSlot[cacheID] = adCacheSlotInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
	}

	protected override bool HasCachedVideo(string cacheID)
	{
		return Supersonic.IsRewardedVideoAvailable();
	}

	protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
	{
		_videoWatched = false;
		_showVideoCacheSlot = adCacheSlotInfo;
		_showVideoAdLocation = adLocationInfo;
		Supersonic.ShowRewardedVideo(cacheID);
	}

	protected override bool HasCachedInterstitial(string cacheID)
	{
		return Supersonic.IsInterstitialReady();
	}

	protected override void CacheInterstitial(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
	{
		_cacheIDToSlot[cacheID] = adCacheSlotInfo;
		AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
		Supersonic.LoadInterstitial();
	}

	protected override void ShowInterstitial(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
	{
		_showInterstitialCacheSlot = adCacheSlotInfo;
		_showInterstitialAdLocation = adLocationInfo;
		Supersonic.ShowInterstitial(cacheID);
	}

	protected override void OnCacheTimeout(AdType adType, AdCacheSlotInfo adCacheSlotInfo)
	{
		base.OnCacheTimeout(adType, adCacheSlotInfo);
		if (adType == AdType.Video)
		{
			_isVideoCaching = false;
		}
	}

	[Conditional("ENABLE_SUPERSONIC_LOGS")]
	private void Log(string message)
	{
	}
}
