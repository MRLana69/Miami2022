using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common.Ads;
using UnityEngine;

namespace Kiloo.Monetization.MobVista
{
	public class MobVistaAdProvider : AbstractAdProvider
	{
		public const string ID = "mobvista";

		public override string id
		{
			get
			{
				return "mobvista";
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
				return list;
			}
		}

		protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
		{
			bool isSupportedOnPlatform = MobVista.isSupportedOnPlatform;
			bool flag = !string.IsNullOrEmpty(configuration.appID) && !string.IsNullOrEmpty(configuration.appSDKKey);
			_supportedAdTypes[AdType.Video] = isSupportedOnPlatform && flag;
			_supportedAdTypes[AdType.Interstitial] = MobVista.isInterstitialSupportedOnPlatform && flag;
		}

		protected override void OnInitialize(AdProviderConfiguration adProviderConfiguration)
		{
			MobVista.Initialize(adProviderConfiguration.appID, adProviderConfiguration.appSDKKey);
			SetupHooks();
			FinishInitialization(true);
		}

		private void SetupHooks()
		{
			MobVista.onVideoLoaded += OnVideoLoaded;
			MobVista.onVideoAdClose += OnVideoAdClose;
			MobVista.onVideoLoadFailed += OnVideoLoadFailed;
			MobVista.onVideoShow += OnVideoShow;
			MobVista.onVideoAdWatched += OnVideoAdWatched;
			MobVista.onVideoAdShowFailed += OnVideoAdShowFailed;
			MobVista.onVideoAdClicked += OnVideoAdClicked;
			MobVista.onInterstitialLoaded += OnInterstitialLoaded;
			MobVista.onInterstitialAdClose += OnInterstitialAdClose;
			MobVista.onInterstitialLoadFailed += OnInterstitialLoadFailed;
			MobVista.onInterstitialShow += OnInterstitialShow;
			MobVista.onInterstitialAdWatched += OnInterstitialAdWatched;
			MobVista.onInterstitialAdShowFailed += OnInterstitialAdShowFailed;
			MobVista.onInterstitialAdClicked += OnInterstitialAdClicked;
		}

		private bool IsFullScreenAdLoaded(string cacheId)
		{
			return MobVista.IsFullScreenAdLoaded(cacheId);
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			MobVista.LoadFullScreenAd(cacheID);
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			return IsFullScreenAdLoaded(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showCacheSlot = adCacheSlotInfo;
			_showAdLocation = adLocationInfo;
			MobVista.ShowFullScreenAd(cacheID);
		}

		protected override void CacheInterstitial(AdCacheSlotInfo adCacheSlot, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlot;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Interstitial, id, GetCurrentTier(adCacheSlot), adCacheSlot);
			MobVista.LoadInterstitialAd(cacheID);
		}

		protected override bool HasCachedInterstitial(string cacheID)
		{
			return MobVista.IsInterstitialAdLoaded(cacheID);
		}

		protected override void ShowInterstitial(AdCacheSlotInfo adCacheSlot, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showInterstitialCacheSlot = adCacheSlot;
			_showCacheSlot = adCacheSlot;
			_showInterstitialAdLocation = adLocationInfo;
			_showAdLocation = adLocationInfo;
			MobVista.ShowInterstitialAd(cacheID);
			AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, "mobvista", GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
		}

		private void OnVideoLoaded(string cacheID)
		{
			if (_cacheIDToSlot.ContainsKey(cacheID))
			{
				AbstractAdProvider.eventInvoker.CachedAd(_cacheIDToSlot[cacheID].adType, "mobvista", GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID], false);
				_cacheIDToSlot.Remove(cacheID);
			}
		}

		private void OnVideoShow(string cacheID)
		{
			AbstractAdProvider.eventInvoker.ShowedAd(_showCacheSlot.adType, "mobvista", GetTierForCacheId(cacheID), _showCacheSlot, _showAdLocation);
		}

		private void OnVideoAdClose(string cacheID)
		{
			AbstractAdProvider.eventInvoker.DismissedAd(_showCacheSlot.adType, "mobvista", GetTierForCacheId(cacheID), _showCacheSlot, _showAdLocation);
		}

		private void OnVideoAdWatched(string cacheID)
		{
			AbstractAdProvider.eventInvoker.FinishedWatchingVideo("mobvista", GetTierForCacheId(cacheID), _showCacheSlot, _showAdLocation);
		}

		private void OnVideoLoadFailed(string cacheID)
		{
			if (_cacheIDToSlot.ContainsKey(cacheID))
			{
				AbstractAdProvider.eventInvoker.FailedToCacheAd(_cacheIDToSlot[cacheID].adType, "mobvista", GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID]);
				_cacheIDToSlot.Remove(cacheID);
			}
		}

		private void OnVideoAdClicked(string cacheID)
		{
			AbstractAdProvider.eventInvoker.ClickedAd(_showCacheSlot.adType, "mobvista", GetTierForCacheId(cacheID), _showCacheSlot, _showAdLocation);
		}

		private void OnVideoAdShowFailed(string error)
		{
			AbstractAdProvider.eventInvoker.FailedToShowAd(_showCacheSlot.adType, "mobvista", GetCurrentTier(_showCacheSlot), _showCacheSlot, _showAdLocation);
		}

		private void OnInterstitialLoaded(string cacheID)
		{
			if (_cacheIDToSlot.ContainsKey(cacheID))
			{
				AbstractAdProvider.eventInvoker.CachedAd(AdType.Interstitial, "mobvista", GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID], false);
				_cacheIDToSlot.Remove(cacheID);
			}
		}

		private void OnInterstitialShow(string cacheID)
		{
			AbstractAdProvider.eventInvoker.ShowedAd(AdType.Interstitial, "mobvista", GetTierForCacheId(cacheID), _showInterstitialCacheSlot, _showInterstitialAdLocation);
		}

		private void OnInterstitialAdClose(string cacheID)
		{
			AbstractAdProvider.eventInvoker.DismissedAd(AdType.Interstitial, "mobvista", GetTierForCacheId(cacheID), _showInterstitialCacheSlot, _showInterstitialAdLocation);
		}

		private void OnInterstitialAdWatched(string cacheID)
		{
			AbstractAdProvider.eventInvoker.FinishedWatchingVideo("mobvista", GetTierForCacheId(cacheID), _showInterstitialCacheSlot, _showInterstitialAdLocation);
		}

		private void OnInterstitialLoadFailed(string cacheID)
		{
			AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Interstitial, "mobvista", GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID]);
			_cacheIDToSlot.Remove(cacheID);
		}

		private void OnInterstitialAdClicked(string unitId)
		{
			AbstractAdProvider.eventInvoker.ClickedAd(AdType.Interstitial, "mobvista", GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
		}

		private void OnInterstitialAdShowFailed(string error)
		{
			AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Interstitial, "mobvista", GetCurrentTier(_showInterstitialCacheSlot), _showInterstitialCacheSlot, _showInterstitialAdLocation);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, Object context = null)
		{
			SDebug.LogError("[MobvistaVideoAdProvider] " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, Object context = null)
		{
		}

		[Conditional("ENABLE_MOBVISTA_LOGS")]
		private static void Log(string msg, Object context = null)
		{
		}
	}
}
