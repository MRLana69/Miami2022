using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Monetization.FacebookAds.Internal
{
	public class FacebookNativeAdsIos : MonoBehaviour, IFacebookNativeAdsBridge
	{
		private const string CALL_SOURCE = "FacebookNativeAdBridgeIOS";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<bool> onInitialize__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdLoaded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdShown__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeLoggedImpression__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoDidLoad__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoCompleted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoStarted__BackingField;

		private static FacebookNativeAdsIos _instance;

		private const string CALLBACK_RECEIVER_NAME = "FacebookAdsBridgeReceiver";

		public event Action<bool> onInitialize
		{
			add
			{
				Action<bool> action = onInitialize__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitialize__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onInitialize__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitialize__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdLoaded
		{
			add
			{
				Action<string> action = onNativeAdLoaded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdLoaded__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdLoaded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdLoaded__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdShown
		{
			add
			{
				Action<string> action = onNativeAdShown__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdShown__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdShown__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdShown__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdClosed
		{
			add
			{
				Action<string> action = onNativeAdClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdClosed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdClosed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdError
		{
			add
			{
				Action<string> action = onNativeAdError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdError__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdError__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeClicked
		{
			add
			{
				Action<string> action = onNativeClicked__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeClicked__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeClicked__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeClicked__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeLoggedImpression
		{
			add
			{
				Action<string> action = onNativeLoggedImpression__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeLoggedImpression__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeLoggedImpression__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeLoggedImpression__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoDidLoad
		{
			add
			{
				Action<string> action = onRewardedVideoDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoDidLoad__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoDidLoad__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoError
		{
			add
			{
				Action<string> action = onRewardedVideoError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoError__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoError__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoCompleted
		{
			add
			{
				Action<string> action = onRewardedVideoCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoCompleted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoCompleted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoClosed
		{
			add
			{
				Action<string> action = onRewardedVideoClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoClosed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoClosed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoStarted
		{
			add
			{
				Action<string> action = onRewardedVideoStarted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoStarted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoStarted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoStarted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void Initialize()
		{
			_facebookAdsBridge_InitializePlugin();
		}

		public bool IsNativeAdLoaded(string cacheID)
		{
			bool flag = true;
			bool flag2 = _facebookAdsBridge_IsNativeAdValid(cacheID);
			return flag && flag2;
		}

		public bool IsVideoAdLoaded(string cacheID)
		{
			return _facebookAdsBridge_IsVideoAdValid(cacheID);
		}

		public void CacheNativeAd(string cacheID)
		{
			_facebookAdsBridge_LoadNativeAd(cacheID);
		}

		public void LoadVideoAd(string cacheID)
		{
			_facebookAdsBridge_LoadVideoAd(cacheID);
		}

		public void ShowVideo(string cacheID)
		{
			_facebookAdsBridge_ShowVideoAd(cacheID);
		}

		public void ShowInterstitial(string cacheID)
		{
			_facebookAdsBridge_ShowNativeAd(cacheID, (double)Screen.width * 0.8, (double)Screen.height * 0.8);
		}

		public void AddTestDevice(string deviceID)
		{
			_facebookAdsBridge_AddTestDevice(deviceID);
		}

		public void SetDebugLogs(bool enableLogs)
		{
			LogWarning("SetDebugLogs is not supported for FB native bridge on IOS");
		}

		private void _callback_onInitialize()
		{
			onInitialize__BackingField.SafeInvoke(true);
		}

		private void _callback_onNativeAdDidLoad(string cacheID)
		{
			onNativeAdLoaded__BackingField.SafeInvoke(string.Empty);
		}

		private void _callback_onNativeAdShown(string cacheID)
		{
			onNativeAdShown__BackingField.SafeInvoke(string.Empty);
		}

		private void _callback_onNativeAdClosed(string cacheID)
		{
			onNativeAdClosed__BackingField.SafeInvoke(string.Empty);
		}

		private void _callback_onNativeAdError(string error)
		{
			onNativeAdError__BackingField.SafeInvoke(error);
		}

		private void _callback_onNativeAdClick(string cacheID)
		{
			onNativeClicked__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onNativeAdFinishHandlingClick(string cacheID)
		{
		}

		private void _callback_onNativeAdWillLogImpression(string cacheID)
		{
			onNativeLoggedImpression__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onVideoAdDidLoad(string cacheID)
		{
			onRewardedVideoDidLoad__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onVideoAdDidClick(string cacheID)
		{
		}

		private void _callback_onVideoAdComplete(string cacheID)
		{
			onRewardedVideoCompleted__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onVideoAdDidClose(string cacheID)
		{
			onRewardedVideoClosed__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onVideoAdWillClose(string cacheID)
		{
		}

		private void _callback_onVideoAdWillLogImpression(string cacheID)
		{
			onRewardedVideoStarted__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onVideoAdError(string error)
		{
			LogError("_callback_onVideoAdErrorVideoAdError " + error);
			onRewardedVideoError__BackingField.SafeInvoke(string.Empty);
		}

		private void _facebookAdsBridge_InitializePlugin()
		{
		}

		private void _facebookAdsBridge_AddTestDevice(string deviceID)
		{
		}

		private void _facebookAdsBridge_LoadNativeAd(string placementId)
		{
		}

		private void _facebookAdsBridge_ShowNativeAd(string placementId, double width, double height)
		{
		}

		private bool _facebookAdsBridge_IsNativeAdValid(string placementId)
		{
			return false;
		}

		private void _facebookAdsBridge_ShowVideoAd(string placementId)
		{
		}

		private void _facebookAdsBridge_LoadVideoAd(string placementId)
		{
		}

		private bool _facebookAdsBridge_IsVideoAdValid(string placementId)
		{
			return false;
		}

		public static FacebookNativeAdsIos CreatePlugin()
		{
			if (_instance != null)
			{
				return _instance;
			}
			GameObject gameObject = new GameObject("FacebookAdsBridgeReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			_instance = gameObject.AddComponent<FacebookNativeAdsIos>();
			return _instance;
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("FacebookIOS: " + msg, context);
		}

		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_FACEBOOKADS_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
