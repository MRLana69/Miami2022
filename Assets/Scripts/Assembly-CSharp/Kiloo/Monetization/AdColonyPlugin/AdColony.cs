#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.AdColonyPlugin.Internal;
using UnityEngine;

namespace Kiloo.Monetization.AdColonyPlugin
{
	public static class AdColony
	{
		private static IAdColonyPlatformBridge _platformBridge;

		private const int MINIMUM_SUPPORTED_ANDROID_API = 21;

		private const int MINIMUM_SUPPORTED_IOS_VERSION = 10;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> onAdReward__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onAdClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onAdStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onCachingFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onCachingSuccess__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onInitFinished__BackingField;

		public static event Action<bool> onAdReward
		{
			add
			{
				Action<bool> action = onAdReward__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdReward__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onAdReward__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdReward__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onAdClosed
		{
			add
			{
				Action action = onAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onAdClicked
		{
			add
			{
				Action action = onAdClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdClicked__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onAdClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdClicked__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onAdStarted
		{
			add
			{
				Action action = onAdStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onAdStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAdStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onCachingFailed
		{
			add
			{
				Action<string> action = onCachingFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCachingFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onCachingFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCachingFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onCachingSuccess
		{
			add
			{
				Action<string> action = onCachingSuccess__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCachingSuccess__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onCachingSuccess__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCachingSuccess__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onInitFinished
		{
			add
			{
				Action action = onInitFinished__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitFinished__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInitFinished__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitFinished__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("AdColony: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void OnInitializationFinished()
		{
			onInitFinished__BackingField.SafeInvoke();
		}

		private static void OnVideoRewardCallback(bool success)
		{
			onAdReward__BackingField.SafeInvoke(success);
		}

		private static void OnAdClosedCallback()
		{
			onAdClosed__BackingField.SafeInvoke();
		}

		private static void OnAdStartedCallback()
		{
			onAdStarted__BackingField.SafeInvoke();
		}

		private static void OnAdCachingFailedCallback(string cacheID)
		{
			onCachingFailed__BackingField.SafeInvoke(cacheID);
		}

		private static void OnAdCachingSuccessCallback(string cacheID)
		{
			onCachingSuccess__BackingField.SafeInvoke(cacheID);
		}

		private static bool CheckConfigured()
		{
			if (_platformBridge == null)
			{
				LogError("AdColony must be configured before this call.");
				return false;
			}
			return true;
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
				if (int.TryParse(oSVersion.Split('.')[0], out result2) && result2 < 10)
				{
					result = false;
				}
				return result;
			}
			case RuntimePlatform.Android:
				return DeviceUtility.GetAndroidApiLevel() >= 21;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return true;
			default:
				return false;
			}
		}

		public static void Configure(string appID, string[] zoneIDs)
		{
			if (_platformBridge != null)
			{
				LogError("AdColony has already been configured. Ignoring new parameters.");
				return;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = AdColonyIOS.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = AdColonyAndroid.CreatePlugin();
				break;
			default:
				LogError("There's no platform bridge for the current platform.");
				return;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				break;
			}
			_platformBridge.onInitFinished += OnInitializationFinished;
			_platformBridge.onReward += OnVideoRewardCallback;
			_platformBridge.onAdStarted += OnAdStartedCallback;
			_platformBridge.onCachingFailed += OnAdCachingFailedCallback;
			_platformBridge.onCachingSuccess += OnAdCachingSuccessCallback;
			_platformBridge.onAdClosed += OnAdClosedCallback;
			_platformBridge.onAdClicked += onAdClicked__BackingField;
			_platformBridge.Configure(appID, zoneIDs);
		}

		public static void LoadVideoAd(string zoneID)
		{
			_platformBridge.LoadAd(zoneID);
		}

		public static void PlayVideoAd(string zoneID)
		{
			if (CheckConfigured())
			{
				_platformBridge.PlayVideoAd(zoneID);
			}
		}

		public static bool IsVideoExpired(string zoneID)
		{
			if (!CheckConfigured())
			{
				return false;
			}
			return _platformBridge.IsVideoExpired(zoneID);
		}
	}
}
