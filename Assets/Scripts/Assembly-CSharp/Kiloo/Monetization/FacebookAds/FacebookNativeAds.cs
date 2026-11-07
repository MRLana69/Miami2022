using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.FacebookAds.Internal;
using UnityEngine;

namespace Kiloo.Monetization.FacebookAds
{
	public static class FacebookNativeAds
	{
		private static IFacebookNativeAdsBridge _platformBridge;

		private const int MINIMUM_SUPPORTED_IOS_VERSION = 9;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> onNativeInitialize__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onNativeDidLoad__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onNativeAdShown__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onNativeAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onNativeAdClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onNativeAdError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onVideoDidLoad__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onVideoError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onVideoViewStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onVideoViewFinished__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onVideoViewClosed__BackingField;

		public static event Action<bool> onNativeInitialize
		{
			add
			{
				Action<bool> action = onNativeInitialize__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeInitialize__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onNativeInitialize__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeInitialize__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onNativeDidLoad
		{
			add
			{
				Action<string> action = onNativeDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeDidLoad__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeDidLoad__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onNativeAdShown
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

		public static event Action<string> onNativeAdClosed
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

		public static event Action<string> onNativeAdClicked
		{
			add
			{
				Action<string> action = onNativeAdClicked__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdClicked__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdClicked__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdClicked__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onNativeAdError
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

		public static event Action<string> onVideoDidLoad
		{
			add
			{
				Action<string> action = onVideoDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoDidLoad__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onVideoDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoDidLoad__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onVideoError
		{
			add
			{
				Action<string> action = onVideoError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoError__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onVideoError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoError__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onVideoViewStarted
		{
			add
			{
				Action<string> action = onVideoViewStarted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoViewStarted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onVideoViewStarted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoViewStarted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onVideoViewFinished
		{
			add
			{
				Action<string> action = onVideoViewFinished__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoViewFinished__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onVideoViewFinished__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoViewFinished__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onVideoViewClosed
		{
			add
			{
				Action<string> action = onVideoViewClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoViewClosed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onVideoViewClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onVideoViewClosed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private static bool IsInitialized()
		{
			return _platformBridge != null;
		}

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
			{
				bool result = true;
				string oSVersion = DeviceUtility.GetOSVersion();
				int result2;
				if (int.TryParse(oSVersion.Split('.')[0], out result2) && result2 < 9)
				{
					result = false;
				}
				return result;
			}
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice)
				{
					return false;
				}
				return true;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return false;
			default:
				return false;
			}
		}

		private static void InstantiateNativeBridge()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = FacebookNativeAdsIos.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = FacebookNativeAdsAndroid.CreatePlugin();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				_platformBridge = FacebookNativeAdsSimulated.CreatePlugin();
				break;
			default:
				LogError("Facebook ads are not supported for: " + Application.platform);
				break;
			}
		}

		public static void AddTestDevices(List<string> deviceIDs)
		{
			foreach (string deviceID in deviceIDs)
			{
				AddTestDevice(deviceID);
			}
		}

		public static void AddTestDevice(string deviceID)
		{
			if (_platformBridge != null)
			{
				_platformBridge.AddTestDevice(deviceID);
			}
		}

		public static void Initialize()
		{
			if (!IsInitialized())
			{
				InstantiateNativeBridge();
				if (_platformBridge != null)
				{
					_platformBridge.onInitialize += OnInitialize;
					_platformBridge.onNativeAdLoaded += OnNativeDidLoad;
					_platformBridge.onNativeClicked += onNativeAdClicked__BackingField;
					_platformBridge.onNativeAdShown += OnNativeAdShown;
					_platformBridge.onNativeAdClosed += OnNativeAdClosed;
					_platformBridge.onNativeAdError += OnNativeAdError;
					_platformBridge.Initialize();
					_platformBridge.onRewardedVideoDidLoad += OnVideoDidLoad;
					_platformBridge.onRewardedVideoError += OnVideoError;
					_platformBridge.onRewardedVideoStarted += OnVideoViewStarted;
					_platformBridge.onRewardedVideoCompleted += OnVideoViewFinished;
					_platformBridge.onRewardedVideoClosed += OnVideoViewClosed;
				}
			}
		}

		public static bool IsCachedVideoAvailable(string placementId)
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.IsVideoAdLoaded(placementId);
		}

		public static bool IsNativeAdLoaded(string cacheID)
		{
			return IsInitialized() && _platformBridge.IsNativeAdLoaded(cacheID);
		}

		public static void CacheNativeAd(string cacheID)
		{
			if (IsInitialized())
			{
				_platformBridge.CacheNativeAd(cacheID);
			}
		}

		public static void LoadVideoAd(string cacheID)
		{
			if (IsInitialized())
			{
				_platformBridge.LoadVideoAd(cacheID);
			}
		}

		public static void ShowVideo(string cacheID)
		{
			_platformBridge.ShowVideo(cacheID);
		}

		public static void ShowInterstitial(string cacheID)
		{
			_platformBridge.ShowInterstitial(cacheID);
		}

		private static void OnInitialize(bool succeeded)
		{
			onNativeInitialize__BackingField.SafeInvoke(succeeded);
		}

		private static void OnNativeDidLoad(string cacheID)
		{
			onNativeDidLoad__BackingField.SafeInvoke(cacheID);
			LogWarning("Downloaded Ad, but asset is null");
		}

		private static void OnNativeAdShown(string cacheID)
		{
			onNativeAdShown__BackingField.SafeInvoke(cacheID);
		}

		private static void OnNativeAdClosed(string cacheID)
		{
			onNativeAdClosed__BackingField.SafeInvoke(cacheID);
		}

		private static void OnNativeAdError(string message)
		{
			LogError("OnNativeAdError: " + message);
			onNativeAdError__BackingField.SafeInvoke(message);
		}

		private static void OnVideoDidLoad(string cacheID)
		{
			onVideoDidLoad__BackingField.SafeInvoke(cacheID);
		}

		private static void OnVideoError(string placement)
		{
			onVideoError__BackingField.SafeInvoke(placement);
		}

		private static void OnVideoViewStarted(string message)
		{
			onVideoViewStarted__BackingField.SafeInvoke(message);
		}

		private static void OnVideoViewFinished(string message)
		{
			onVideoViewFinished__BackingField.SafeInvoke(message);
		}

		private static void OnVideoViewClosed(string message)
		{
			onVideoViewClosed__BackingField.SafeInvoke(message);
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("FacebookNativeAds: " + msg, context);
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
