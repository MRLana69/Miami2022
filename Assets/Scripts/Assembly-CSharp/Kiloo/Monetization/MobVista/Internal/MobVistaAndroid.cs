using UnityEngine;

namespace Kiloo.Monetization.MobVista.Internal
{
	public class MobVistaAndroid : MobVistaPlatformBridgeBase
	{
		private const string CALLBACK_RECEIVER_NAME = "MobVistaAndroidVideoAdsReceiver";

		private AndroidJavaObject _androidBridge;

		protected override string platform
		{
			get
			{
				return "Android";
			}
		}

		public override void Initialize(string appId, string sdkKey, bool consent)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.mobvista.MobVistaBridge.MobVistaSdkInit");
			androidJavaObject.Call("initialize", appId, sdkKey, consent);
			_androidBridge = new AndroidJavaObject("com.mobvista.MobVistaBridge.MobVistaVideoAdsBridge");
			_androidBridge.Call("initialize");
		}

		public override bool IsFullScreenAdLoaded(string placementId)
		{
			return _androidBridge.Call<bool>("isFullScreenAdLoaded", new object[1] { placementId });
		}

		public override void LoadFullScreenAd(string placementId)
		{
			_androidBridge.Call("loadFullScreenAd", placementId);
		}

		public override void ShowFullScreenAd(string placementId)
		{
			_androidBridge.Call("showFullScreenAd", placementId);
		}

		public override bool IsInterstitialAdLoaded(string placementId)
		{
			return false;
		}

		public override void LoadInterstitialAd(string placementId)
		{
		}

		public override void ShowInterstitialAd(string placementId)
		{
		}

		public override void EnableDebugLogs()
		{
			_androidBridge.Call("enableDebugLogs");
		}

		public override void DisableDebugLogs()
		{
			_androidBridge.Call("disableDebugLogs");
		}

		public void CallBackOnLoaded(string placementId)
		{
		}

		public static MobVistaAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("MobVistaAndroidVideoAdsReceiver");
			Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<MobVistaAndroid>();
		}
	}
}
