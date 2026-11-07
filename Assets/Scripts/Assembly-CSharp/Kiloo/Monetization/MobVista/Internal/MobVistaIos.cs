using UnityEngine;

namespace Kiloo.Monetization.MobVista.Internal
{
	public class MobVistaIos : MobVistaPlatformBridgeBase
	{
		private const string CALLBACK_RECEIVER_NAME = "MobVistaIOSVideoAdsReceiver";

		protected override string platform
		{
			get
			{
				return "IOS";
			}
		}

		private static void mobVistaSdkPluginInitialize(string appId, string sdkKey, bool consent)
		{
		}

		private static void mobVistaVideoAdsPluginInitialize()
		{
		}

		private static bool mobVistaVideoAdsPluginIsFullScreenAdLoaded(string placementId)
		{
			return false;
		}

		private static void mobVistaVideoAdsPluginLoadFullScreenAd(string placementId)
		{
		}

		private static void mobVistaVideoAdsPluginShowFullScreenAd(string placementId)
		{
		}

		private static void mobVistaVideoAdsPluginEnableDebugLogs()
		{
		}

		private static void mobVistaVideoAdsPluginDisableDebugLogs()
		{
		}

		public override void Initialize(string appId, string sdkKey, bool consent)
		{
			mobVistaSdkPluginInitialize(appId, sdkKey, consent);
			mobVistaVideoAdsPluginInitialize();
		}

		public override bool IsFullScreenAdLoaded(string placementId)
		{
			return mobVistaVideoAdsPluginIsFullScreenAdLoaded(placementId);
		}

		public override void LoadFullScreenAd(string placementId)
		{
			mobVistaVideoAdsPluginLoadFullScreenAd(placementId);
		}

		public override void ShowFullScreenAd(string placementId)
		{
			mobVistaVideoAdsPluginShowFullScreenAd(placementId);
		}

		public override bool IsInterstitialAdLoaded(string placementId)
		{
			return mobVistaVideoAdsPluginIsFullScreenAdLoaded(placementId);
		}

		public override void LoadInterstitialAd(string placementId)
		{
			mobVistaVideoAdsPluginLoadFullScreenAd(placementId);
		}

		public override void ShowInterstitialAd(string placementId)
		{
			mobVistaVideoAdsPluginShowFullScreenAd(placementId);
		}

		public override void EnableDebugLogs()
		{
			mobVistaVideoAdsPluginEnableDebugLogs();
		}

		public override void DisableDebugLogs()
		{
			mobVistaVideoAdsPluginDisableDebugLogs();
		}

		public static MobVistaIos CreatePlugin()
		{
			GameObject gameObject = new GameObject("MobVistaIOSVideoAdsReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<MobVistaIos>();
		}
	}
}
