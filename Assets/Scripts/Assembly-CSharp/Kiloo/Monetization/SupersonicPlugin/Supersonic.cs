#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.SupersonicPlugin.Internal;
using UnityEngine;

namespace Kiloo.Monetization.SupersonicPlugin
{
	public static class Supersonic
	{
		private static ISupersonicPlatformBridge _platformBridge;

		private static bool _rewardedVideoIsAvailable;

		private const int MINIMUM_SUPPORTED_ANDROID_API = 17;

		private const int DISALLOWED_INTERSTITIAL_ANDROID_API = 26;

		private const int MINIMUM_SUPPORTED_IOS_VERSION = 8;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> onInitFinished__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onRewardedVideoAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onRewardedVideoAdOpened__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onRewardedVideoAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onRewardedVideoAdRewarded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> onRewardedVideoAvailabilityChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onRewardedVideoAdStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onRewardedVideoAdEnded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onInterstitialAdReady__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onInterstitialAdLoadFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onInterstitialAdOpened__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onInterstitialAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onInterstitialAdShowSucceeded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onInterstitialAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onInterstitialAdClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onOfferwallAvailable__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onOfferwallOpened__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onOfferwallShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onOfferwallAdCredited__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string> onGetOfferwallCreditsFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onOfferwallClosed__BackingField;

		public static event Action<bool> onInitFinished
		{
			add
			{
				Action<bool> action = onInitFinished__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitFinished__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onInitFinished__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitFinished__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onRewardedVideoAdShowFailed
		{
			add
			{
				Action<string> action = onRewardedVideoAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onRewardedVideoAdOpened
		{
			add
			{
				Action action = onRewardedVideoAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onRewardedVideoAdClosed
		{
			add
			{
				Action action = onRewardedVideoAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onRewardedVideoAdRewarded
		{
			add
			{
				Action<string> action = onRewardedVideoAdRewarded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdRewarded__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoAdRewarded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdRewarded__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<bool> onRewardedVideoAvailabilityChanged
		{
			add
			{
				Action<bool> action = onRewardedVideoAvailabilityChanged__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAvailabilityChanged__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onRewardedVideoAvailabilityChanged__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAvailabilityChanged__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onRewardedVideoAdStarted
		{
			add
			{
				Action action = onRewardedVideoAdStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onRewardedVideoAdEnded
		{
			add
			{
				Action action = onRewardedVideoAdEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdEnded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdEnded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onInterstitialAdReady
		{
			add
			{
				Action action = onInterstitialAdReady__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdReady__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdReady__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdReady__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onInterstitialAdLoadFailed
		{
			add
			{
				Action<string> action = onInterstitialAdLoadFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdLoadFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onInterstitialAdLoadFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdLoadFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onInterstitialAdOpened
		{
			add
			{
				Action action = onInterstitialAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onInterstitialAdClosed
		{
			add
			{
				Action action = onInterstitialAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onInterstitialAdShowSucceeded
		{
			add
			{
				Action action = onInterstitialAdShowSucceeded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowSucceeded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdShowSucceeded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowSucceeded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onInterstitialAdShowFailed
		{
			add
			{
				Action<string> action = onInterstitialAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onInterstitialAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onInterstitialAdClicked
		{
			add
			{
				Action action = onInterstitialAdClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onOfferwallAvailable
		{
			add
			{
				Action<string> action = onOfferwallAvailable__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallAvailable__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onOfferwallAvailable__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallAvailable__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onOfferwallOpened
		{
			add
			{
				Action action = onOfferwallOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onOfferwallOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onOfferwallShowFailed
		{
			add
			{
				Action<string> action = onOfferwallShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onOfferwallShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onOfferwallAdCredited
		{
			add
			{
				Action<string> action = onOfferwallAdCredited__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallAdCredited__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onOfferwallAdCredited__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallAdCredited__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<string> onGetOfferwallCreditsFailed
		{
			add
			{
				Action<string> action = onGetOfferwallCreditsFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGetOfferwallCreditsFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onGetOfferwallCreditsFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGetOfferwallCreditsFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onOfferwallClosed
		{
			add
			{
				Action action = onOfferwallClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onOfferwallClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferwallClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("Supersonic: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_SUPERSONIC_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		public static bool IsInitialized()
		{
			if (_platformBridge == null)
			{
				return false;
			}
			return true;
		}

		public static bool IsSupportedOnPlatform(AdType adType)
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
			{
				int androidApiLevel = DeviceUtility.GetAndroidApiLevel();
				return !DeviceUtility.isKindleDevice && androidApiLevel >= 17 && (androidApiLevel != 26 || adType != AdType.Interstitial);
			}
			case RuntimePlatform.IPhonePlayer:
			{
				bool result = true;
				string oSVersion = DeviceUtility.GetOSVersion();
				int result2;
				if (int.TryParse(oSVersion.Split('.')[0], out result2) && result2 < 8)
				{
					result = false;
				}
				return result;
			}
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return true;
			default:
				return false;
			}
		}

		private static void SetupEvents()
		{
			_platformBridge.onInterstitialAdClicked += () =>
			{
				onInterstitialAdClicked__BackingField.SafeInvoke();
			};
			_platformBridge.onInterstitialAdClosed += () =>
			{
				onInterstitialAdClosed__BackingField.SafeInvoke();
			};
			_platformBridge.onInterstitialAdLoadFailed += (string obj) =>
			{
				onInterstitialAdLoadFailed__BackingField.SafeInvoke(obj);
			};
			_platformBridge.onInterstitialAdOpened += () =>
			{
				onInterstitialAdOpened__BackingField.SafeInvoke();
			};
			_platformBridge.onInterstitialAdReady += () =>
			{
				onInterstitialAdReady__BackingField.SafeInvoke();
			};
			_platformBridge.onInterstitialAdShowFailed += (string obj) =>
			{
				onInterstitialAdShowFailed__BackingField.SafeInvoke(obj);
			};
			_platformBridge.onInterstitialAdShowSucceeded += () =>
			{
				onInterstitialAdShowSucceeded__BackingField.SafeInvoke();
			};
			_platformBridge.onRewardedVideoAdClosed += () =>
			{
				onRewardedVideoAdClosed__BackingField.SafeInvoke();
			};
			_platformBridge.onRewardedVideoAdOpened += () =>
			{
				onRewardedVideoAdOpened__BackingField.SafeInvoke();
			};
			_platformBridge.onRewardedVideoAdRewarded += (string obj) =>
			{
				onRewardedVideoAdRewarded__BackingField.SafeInvoke(obj);
			};
			_platformBridge.onRewardedVideoAdShowFailed += (string obj) =>
			{
				onRewardedVideoAdShowFailed__BackingField.SafeInvoke(obj);
			};
			_platformBridge.onRewardedVideoAvailabilityChanged += (bool obj) =>
			{
				_rewardedVideoIsAvailable = obj;
				onRewardedVideoAvailabilityChanged__BackingField.SafeInvoke(obj);
			};
			_platformBridge.onRewardedVideoAdStarted += () =>
			{
				onRewardedVideoAdStarted__BackingField.SafeInvoke();
			};
			_platformBridge.onRewardedVideoAdEnded += () =>
			{
				onRewardedVideoAdEnded__BackingField.SafeInvoke();
			};
			_platformBridge.onInitFinished += (bool succeeded) =>
			{
				onInitFinished__BackingField.SafeInvoke(succeeded);
			};
			_platformBridge.onOfferWallAvailable += (string isAvailable) =>
			{
				onOfferwallAvailable__BackingField.SafeInvoke(isAvailable);
			};
			_platformBridge.onOfferWallOpened += () =>
			{
				onOfferwallOpened__BackingField.SafeInvoke();
			};
			_platformBridge.onOfferWallShowFailed += (string error) =>
			{
				onOfferwallShowFailed__BackingField.SafeInvoke(error);
			};
			_platformBridge.onOfferWallAdCredited += (string credits) =>
			{
				onOfferwallAdCredited__BackingField.SafeInvoke(credits);
			};
			_platformBridge.onGetOfferWallCreditsFailed += (string error) =>
			{
				onGetOfferwallCreditsFailed__BackingField.SafeInvoke(error);
			};
			_platformBridge.onOfferWallClosed += () =>
			{
				onOfferwallClosed__BackingField.SafeInvoke();
			};
		}

		public static void Init(string appKey, string userId)
		{
			if (!IsInitialized())
			{
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
					_platformBridge = SupersonicIOS.CreatePlugin();
					break;
				case RuntimePlatform.Android:
					_platformBridge = SupersonicAndroid.CreatePlugin();
					break;
				default:
					LogError("There's no platform bridge for the current platform.");
					return;
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.WindowsEditor:
					break;
				}
				if (_platformBridge != null)
				{
					SetupEvents();
					_platformBridge.Init(appKey, userId);
					_platformBridge.SetDebugLogsEnabled(false);
					_platformBridge.ShouldTrackNetworkState(true);
				}
			}
		}

		public static void ShowInterstitial()
		{
			if (IsInitialized())
			{
				_platformBridge.ShowInterstitial();
			}
		}

		public static void ShowInterstitial(string placementName)
		{
			if (IsInitialized())
			{
				_platformBridge.ShowInterstitial(placementName);
			}
		}

		public static void LoadInterstitial()
		{
			if (IsInitialized())
			{
				_platformBridge.LoadInterstitial();
			}
		}

		public static bool IsInterstitialReady()
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.IsInterstitialReady();
		}

		private static bool IsInterstitialPlacementCapped(string placementName)
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.IsInterstitialPlacementCapped(placementName);
		}

		public static void ShowRewardedVideo()
		{
			if (IsInitialized())
			{
				_platformBridge.ShowRewardedVideo();
			}
		}

		public static void ShowRewardedVideo(string placementName)
		{
			if (IsInitialized())
			{
				_platformBridge.ShowRewardedVideo(placementName);
			}
		}

		public static bool IsRewardedVideoAvailable()
		{
			if (!IsInitialized())
			{
				return false;
			}
			if (_rewardedVideoIsAvailable)
			{
				return _platformBridge.IsRewardedVideoAvailable();
			}
			return false;
		}

		public static void ShowOfferwall()
		{
			if (IsInitialized())
			{
				_platformBridge.ShowOfferWall();
			}
		}

		public static bool IsOfferwallAvailable()
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.IsOfferWallAvailable();
		}

		public static void GetOfferwallCredits()
		{
			if (IsInitialized())
			{
				_platformBridge.GetOfferWallCredits();
			}
		}

		public static void ValidateIntegration()
		{
			if (IsInitialized())
			{
				_platformBridge.ValidateIntegration();
			}
		}

		public static void ShouldTrackNetworkState(bool shouldTrack)
		{
			if (IsInitialized())
			{
				_platformBridge.ShouldTrackNetworkState(shouldTrack);
			}
		}
	}
}
