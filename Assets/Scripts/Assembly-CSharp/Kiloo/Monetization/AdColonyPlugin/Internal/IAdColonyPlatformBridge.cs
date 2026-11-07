using System;

namespace Kiloo.Monetization.AdColonyPlugin.Internal
{
	public interface IAdColonyPlatformBridge
	{
		event Action onInitFinished;

		event Action<string> onCachingFailed;

		event Action<string> onCachingSuccess;

		event Action<bool> onReward;

		event Action onAdClosed;

		event Action onAdStarted;

		event Action onAdClicked;

		void Configure(string appID, string[] zoneIDs);

		void PlayVideoAd(string zoneID);

		bool IsVideoExpired(string zoneID);

		void LoadAd(string zoneID);
	}
}
