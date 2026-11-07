using System;

namespace Kiloo.Monetization.ChartboostPlugin.Internal
{
	public interface IChartboostPlatformBridge
	{
		event ChartboostLocationCallback didCacheInterstitial;

		event ChartboostLocationCallback didFailToLoadInterstitial;

		event ChartboostLocationCallback didDismissInterstitial;

		event ChartboostLocationCallback didCloseInterstitial;

		event ChartboostLocationCallback didClickInterstitial;

		event ChartboostLocationCallback didDisplayInterstitial;

		event ChartboostLocationCallback didCacheRewardedVideo;

		event ChartboostLocationCallback didFailToLoadRewardedVideo;

		event ChartboostLocationCallback didDismissRewardedVideo;

		event ChartboostLocationCallback didCloseRewardedVideo;

		event ChartboostLocationCallback didClickRewardedVideo;

		event ChartboostLocationCallback didCompleteRewardedVideo;

		event ChartboostLocationCallback didDisplayRewardedVideo;

		event ChartboostLocationCallback willDisplayVideo;

		event Action<bool> onInitFinished;

		void StartSession(string appId, string appSignature, bool consentGiven);

		void CacheInterstitial(string location);

		void ShowInterstitial(string location);

		bool HasCachedInterstitial(string location);

		void SetShouldRequestInterstitial(bool val);

		void SetShouldDisplayInterstitial(bool val);

		void SetShouldRequestInterstitialsInFirstSession(bool val);

		void CacheRewardedVideo(string location);

		void ShowRewardedVideo(string location);

		bool HasCachedRewardedVideo(string location);
	}
}
