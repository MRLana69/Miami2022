using Kiloo.Common.Ads;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.SuperAwesome.Internal;
using UnityEngine;

namespace Kiloo.Monetization.SuperAwesome
{
	public class SuperAwesomeAdProvider : AbstractAdProvider
	{
		public const string ID = "superawesome";

		private const bool ENABLE_TEST_ADS = false;

		private const int MIN_SUPPORTED_IOS_VERSION = 8;

		private const int MIN_SUPPORTED_ANDROID_API_VERSION = 16;

		private SuperAwesomeAdBridge _superAwesomeAdBridge;

		private SuperAwesomeMethodInvoker _superAwesomeMethodInvoker;

		private bool _addedListeners;

		public override string id
		{
			get
			{
				return "superawesome";
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
			_supportedAdTypes[AdType.Video] = IsSupportedOnPlatform();
			_supportedAdTypes[AdType.Interstitial] = IsSupportedOnPlatform();
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
				return DeviceUtility.GetAndroidApiLevel() >= 16;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return false;
			default:
				return false;
			}
		}

		protected override void OnInitialize(AdProviderConfiguration adProviderConfiguration)
		{
			_superAwesomeAdBridge = new SuperAwesomeAdBridge(this);
			_superAwesomeAdBridge.Initialize((int)((double)Screen.width * 0.8), (int)((double)Screen.height * 0.8));
			_superAwesomeMethodInvoker = _superAwesomeAdBridge.adBridgeMethodInvoker;
			AddListenersIfNotAdded();
			FinishInitialization(true);
			UpdateTestAdsState();
		}

		private void AddListenersIfNotAdded()
		{
			if (!_addedListeners)
			{
				_superAwesomeAdBridge.SubscribeAdListenerToAdBridgeEvents(AbstractAdProvider.eventInvoker, GetCurrentTier(_showCacheSlot), () => _showCacheSlot, () => _showAdLocation);
				_addedListeners = true;
			}
		}

		private void UpdateTestAdsState()
		{
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_superAwesomeMethodInvoker.LoadVideoAd(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_superAwesomeMethodInvoker.ShowVideoAd(cacheID);
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			return _superAwesomeMethodInvoker.IsVideoAdLoaded(cacheID);
		}

		protected override void CacheInterstitial(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_superAwesomeMethodInvoker.LoadInterstitial(cacheID);
		}

		protected override void ShowInterstitial(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocactionInfo, string cacheID)
		{
			_superAwesomeMethodInvoker.ShowInterstitial(cacheID);
		}

		protected override bool HasCachedInterstitial(string cacheID)
		{
			return _superAwesomeMethodInvoker.IsInterstitialLoaded(cacheID);
		}
	}
}
