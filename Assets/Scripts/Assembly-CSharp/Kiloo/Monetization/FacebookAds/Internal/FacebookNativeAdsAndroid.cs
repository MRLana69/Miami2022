using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Monetization.FacebookAds.Internal
{
	public class FacebookNativeAdsAndroid : MonoBehaviour, IFacebookNativeAdsBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "FacebookAdAndroidReceiver";

		private AndroidJavaObject _androidBridge;

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

		private void Call(string method, params object[] args)
		{
			_androidBridge.Call(method, args);
		}

		private T Call<T>(string method, params object[] args)
		{
			return _androidBridge.Call<T>(method, args);
		}

		public void Initialize()
		{
			if (_androidBridge == null)
			{
				_androidBridge = new AndroidJavaObject("com.kiloo.facebookadsbridge.FacebookAdsNativeBridge");
			}
			SetDebugLogs(false);
			Call("init");
		}

		public bool IsNativeAdLoaded(string cacheID)
		{
			return Call<bool>("isNativeAdLoaded", new object[1] { cacheID });
		}

		public void CacheNativeAd(string cacheID)
		{
			if (!IsNativeAdLoaded(cacheID))
			{
				Call("loadNativeAd", cacheID);
			}
		}

		public void ShowInterstitial(string cacheID)
		{
			Call("ShowNativeAd", cacheID, (int)((double)Screen.width * 0.8), (int)((double)Screen.height * 0.8));
		}

		public bool IsVideoAdLoaded(string cacheID)
		{
			return Call<bool>("IsVideoLoaded", new object[1] { cacheID });
		}

		public void LoadVideoAd(string cacheID)
		{
			Call("LoadVideoAd", cacheID);
		}

		public void ShowVideo(string cacheID)
		{
			Call("ShowVideoAd", cacheID);
		}

		public void AddTestDevice(string deviceID)
		{
			Call("addTestDevice", deviceID);
		}

		public void SetDebugLogs(bool enableLogs)
		{
			Call("setDebugLogs", enableLogs);
		}

		private void _callback_onNativeAdClosed(string cacheID)
		{
			onNativeAdClosed__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onNativeAdInitialize(string succeeded)
		{
			onInitialize__BackingField.SafeInvoke(succeeded == "true");
		}

		private void _callback_onNativeAdShown(string cacheId)
		{
			onNativeAdShown__BackingField.SafeInvoke(cacheId);
		}

		private void _callback_onNativeAdLoaded(string cacheID)
		{
			onNativeAdLoaded__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onNativeAdError(string error)
		{
			onNativeAdError__BackingField.SafeInvoke(error);
		}

		private void _callback_onNativeAdCacheError(string cacheID)
		{
			onNativeAdError__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onNativeAdClicked(string cacheID)
		{
			onNativeClicked__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onNativeLoggedImpression(string cacheID)
		{
			onNativeLoggedImpression__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onRewardedVideoDidLoad(string placementID)
		{
			onRewardedVideoDidLoad__BackingField.SafeInvoke(placementID);
		}

		private void _callback_onVideoAdError(string message)
		{
			onRewardedVideoError__BackingField.SafeInvoke(message);
		}

		private void _callback_onVideoCacheAdError(string cacheID)
		{
			onRewardedVideoError__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onRewardedVideoCompleted(string placementID)
		{
			onRewardedVideoCompleted__BackingField.SafeInvoke(placementID);
		}

		private void _callback_onRewardedVideoClosed(string placementID)
		{
			onRewardedVideoClosed__BackingField.SafeInvoke(placementID);
		}

		private void _callback_onRewardedVideoStarted(string placementID)
		{
			onRewardedVideoStarted__BackingField.SafeInvoke(placementID);
		}

		public static FacebookNativeAdsAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("FacebookAdAndroidReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<FacebookNativeAdsAndroid>();
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("FacebookNativeAdsAndroid: " + msg, context);
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
