using System.Collections.Generic;
using Info;
using Kiloo.Common.Ads;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.AdBridgeCommon;
using Kiloo.Monetization.SuperAwesome.Internal;
using Kiloo.Monetization.Vungle.Internal;
using UnityEngine;

namespace Kiloo.Monetization.Vungle
{
	public class VungleAdProvider : AbstractAdProvider
	{
		public const string ID = "vungle";

		private const int MIN_SUPPORTED_IOS_VERSION = 8;

		private const int MIN_SUPPORTED_KINDLE_API = 22;

		private bool _addedListeners;

		private VungleAdBridge _vungleAdBridge;

		private AdBridgeMethodInvoker _vungleMethodInvoker;

		public override string id
		{
			get
			{
				return "vungle";
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
			bool flag = !string.IsNullOrEmpty(configuration.appID);
			_supportedAdTypes[AdType.Video] = flag && IsSupportedOnPlatform();
		}

		private bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
			{
				bool result = true;
				string oSVersion = DeviceUtility.GetOSVersion();
				int result2;
				if (int.TryParse(oSVersion.Split('.')[0], out result2) && result2 < 8)
				{
					result = false;
				}
				return result;
			}
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice && DeviceUtility.GetAndroidApiLevel() < 22)
				{
					return false;
				}
				return true;
			case RuntimePlatform.WindowsEditor:
				return false;
			case RuntimePlatform.OSXEditor:
				return false;
			default:
				return false;
			}
		}

		protected override void OnInitialize(AdProviderConfiguration configuration)
		{
			_vungleAdBridge = new VungleAdBridge(this);
			_vungleMethodInvoker = _vungleAdBridge.adBridgeMethodInvoker;
			if (!_addedListeners)
			{
				AddListeners();
			}
			_vungleAdBridge.Initialize(configuration.appID, !AgeRestriction.isRestricted);
		}

		private void AddListeners()
		{
			VungleCallbackReceiver adBridgeCallbackReceiver = _vungleAdBridge.adBridgeCallbackReceiver;
			adBridgeCallbackReceiver.onInitialized += base.FinishInitialization;
			adBridgeCallbackReceiver.onAdLoaded += OnAdLoaded;
			adBridgeCallbackReceiver.onAdLoadFailed += OnAdLoadFailed;
			adBridgeCallbackReceiver.onAdShowed += (string cacheID, AdType adType) =>
			{
				AbstractAdProvider.eventInvoker.ShowedAd(adType, id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
			};
			adBridgeCallbackReceiver.onAdClicked += (string cacheID, AdType adType) =>
			{
				AbstractAdProvider.eventInvoker.ClickedAd(adType, "vungle", GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
			};
			adBridgeCallbackReceiver.onAdWatched += (string cacheID, AdType adType) =>
			{
				AbstractAdProvider.eventInvoker.FinishedWatchingVideo("vungle", GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
			};
			adBridgeCallbackReceiver.onAdClosed += (string cacheID, AdType adType) =>
			{
				AbstractAdProvider.eventInvoker.DismissedAd(adType, id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
			};
			_addedListeners = true;
		}

		private void OnAdLoadFailed(string cacheID, AdType adType)
		{
			if (_cacheIDToSlot.ContainsKey(cacheID))
			{
				AbstractAdProvider.eventInvoker.FailedToCacheAd(adType, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID]);
				_cacheIDToSlot.Remove(cacheID);
				return;
			}
			AdSystemLog.LogError("Received cache fail for cacheId not in dictionary: " + cacheID);
			foreach (AdCacheSlotInfo value in _cacheIDToSlot.Values)
			{
				AbstractAdProvider.eventInvoker.FailedToCacheAd(adType, id, GetTierForCacheId(cacheID), value);
			}
			_cacheIDToSlot.Clear();
		}

		private void OnAdLoaded(string cacheID, AdType adType)
		{
			if (_cacheIDToSlot.ContainsKey(cacheID))
			{
				AbstractAdProvider.eventInvoker.CachedAd(adType, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID], false);
				_cacheIDToSlot.Remove(cacheID);
			}
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			_vungleMethodInvoker.LoadVideoAd(cacheID);
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			return _vungleMethodInvoker.IsVideoAdLoaded(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showVideoCacheSlot = adCacheSlotInfo;
			_showVideoAdLocation = adLocationInfo;
			_vungleMethodInvoker.ShowVideoAd(cacheID);
		}
	}
}
