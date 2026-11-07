using Kiloo.Common.Ads;
using Kiloo.Monetization.AdBridgeCommon.MethodInvokers;
using UnityCommon.Kiloo.Monetization.AdBridgeCommon.MethodInvokers;
using UnityEngine;

namespace Kiloo.Monetization.AdBridgeCommon
{
	public class AdBridgeMethodInvoker
	{
		private const string INITIALIZE_METHOD_NAME = "initialize";

		private const string SET_CALLBACK_RECEIVER_NAME_METHOD_NAME = "setUnityCallbackReceiverName";

		private const string ENABLE_DEBUG_LOGS_METHOD_NAME = "enableDebugLogs";

		private const string DISABLE_DEBUG_LOGS_METHOD_NAME = "disableDebugLogs";

		private const string LOAD_VIDEO_AD_METHOD_NAME = "redirectAndLogLoadVideoAdCall";

		private const string SHOW_VIDEO_AD_METHOD_NAME = "redirectAndLogShowVideoAdCall";

		private const string IS_VIDEO_AD_LOADED_METHOD_NAME = "redirectAndLogIsVideoAdLoadedCall";

		private const string LOAD_INTERSTITIAL_METHOD_NAME = "redirectAndLogLoadInterstitialCall";

		private const string SHOW_INTERSTITIAL_METHOD_NAME = "redirectAndLogShowInterstitialCall";

		private const string IS_INTERSTITIAL_LOADED_METHOD_NAME = "redirectAndLogIsInterstitialLoadedCall";

		protected IMethodInvoker _methodInvoker;

		protected AbstractAdProvider _adProvider;

		public AdBridgeLogger adBridgeLogger { get; set; }

		public void Initialize(string classPath, AbstractAdProvider adProvider)
		{
			_methodInvoker = CreatePlatformMethodInvoker(classPath);
			_adProvider = adProvider;
		}

		protected bool IsInitialized()
		{
			return _adProvider.isInitialized;
		}

		protected IMethodInvoker CreatePlatformMethodInvoker(string classPath)
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				return new IosMethodInvoker(classPath);
			case RuntimePlatform.Android:
				return new AndroidMethodInvoker(classPath);
			default:
				adBridgeLogger.LogError("Couldn't find a platform method invoker for the current platform: " + Application.platform);
				return null;
			}
		}

		public void OnInitialize(object[] initializeParams)
		{
			if (!IsInitialized())
			{
				Log("OnInitialize");
				_methodInvoker.Call("initialize", initializeParams);
			}
		}

		public void SetCallbackReceiverName(string callbackReceiverName)
		{
			_methodInvoker.Call("setUnityCallbackReceiverName", callbackReceiverName);
		}

		public void EnableDebugLogs()
		{
			Log("EnableDebugLogs");
			_methodInvoker.Call("enableDebugLogs");
		}

		public void DisableDebugLogs()
		{
			Log("DisableDebugLogs");
			_methodInvoker.Call("disableDebugLogs");
		}

		public void LoadVideoAd(string placementId)
		{
			Log("LoadVideoAd" + placementId);
			_methodInvoker.Call("redirectAndLogLoadVideoAdCall", placementId);
		}

		public void ShowVideoAd(string placementId)
		{
			Log("ShowVideoAd" + placementId);
			_methodInvoker.Call("redirectAndLogShowVideoAdCall", placementId);
		}

		public bool IsVideoAdLoaded(string placementId)
		{
			bool flag = _methodInvoker.Call<bool>("redirectAndLogIsVideoAdLoadedCall", new object[1] { placementId });
			Log(string.Format("IsVideoAdLoaded: {0}, placementID: {1}", flag, placementId));
			return flag;
		}

		public bool IsInterstitialLoaded(string placementId)
		{
			bool flag = _methodInvoker.Call<bool>("redirectAndLogIsInterstitialLoadedCall", new object[1] { placementId });
			Log(string.Format("IsInterstitialLoaded: {0}, placementID: {1}", flag, placementId));
			return flag;
		}

		public void LoadInterstitial(string placementId)
		{
			Log("LoadInterstitial" + placementId);
			_methodInvoker.Call("redirectAndLogLoadInterstitialCall", placementId);
		}

		public void ShowInterstitial(string placementId)
		{
			Log("ShowInterstitial" + placementId);
			_methodInvoker.Call("redirectAndLogShowInterstitialCall", placementId);
		}

		protected void Log(string message)
		{
			adBridgeLogger.Log(message);
		}
	}
}
