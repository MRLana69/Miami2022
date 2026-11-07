namespace Kiloo.Monetization.MobVista.Internal
{
	public interface IMobVistaPlatformBridge
	{
		event MobVistaVideoShowEvent onVideoShow;

		event MobVistaVideoAdShowFailedEvent onVideoAdShowFailed;

		event MobVistaVideoAdCloseEvent onVideoAdClose;

		event MobVistaVideoAdWatchedEvent onVideoAdWatched;

		event MobVistaVideoLoadFailedEvent onVideoLoadFailed;

		event MobVistaVideoLoaded onVideoLoaded;

		event MobVistaVideoAdClicked onVideoAdClicked;

		event MobVistaVideoShowEvent onInterstitialShow;

		event MobVistaVideoAdShowFailedEvent onInterstitialAdShowFailed;

		event MobVistaVideoAdCloseEvent onInterstitialAdClose;

		event MobVistaVideoAdWatchedEvent onInterstitialAdWatched;

		event MobVistaVideoLoadFailedEvent onInterstitialLoadFailed;

		event MobVistaVideoLoaded onInterstitialLoaded;

		event MobVistaVideoAdClicked onInterstitialAdClicked;

		void Initialize(string appId, string sdkKey, bool consent);

		bool IsFullScreenAdLoaded(string placementId);

		void LoadFullScreenAd(string placementId);

		void ShowFullScreenAd(string placementId);

		bool IsInterstitialAdLoaded(string placementId);

		void LoadInterstitialAd(string placementId);

		void ShowInterstitialAd(string placementId);

		void EnableDebugLogs();

		void DisableDebugLogs();
	}
}
