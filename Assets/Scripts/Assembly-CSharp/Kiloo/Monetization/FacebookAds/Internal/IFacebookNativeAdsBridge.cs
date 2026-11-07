using System;

namespace Kiloo.Monetization.FacebookAds.Internal
{
	public interface IFacebookNativeAdsBridge
	{
		event Action<bool> onInitialize;

		event Action<string> onNativeAdLoaded;

		event Action<string> onNativeAdShown;

		event Action<string> onNativeAdClosed;

		event Action<string> onNativeAdError;

		event Action<string> onNativeClicked;

		event Action<string> onNativeLoggedImpression;

		event Action<string> onRewardedVideoDidLoad;

		event Action<string> onRewardedVideoError;

		event Action<string> onRewardedVideoCompleted;

		event Action<string> onRewardedVideoClosed;

		event Action<string> onRewardedVideoStarted;

		void Initialize();

		bool IsNativeAdLoaded(string cacheID);

		bool IsVideoAdLoaded(string cacheID);

		void CacheNativeAd(string cacheID);

		void LoadVideoAd(string cacheID);

		void ShowVideo(string cacheID);

		void ShowInterstitial(string cacheID);

		void AddTestDevice(string deviceID);

		void SetDebugLogs(bool enableLogs);
	}
}
