#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using System.Threading;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.ChartboostPlugin.Internal;
using UnityEngine;

namespace Kiloo.Monetization.ChartboostPlugin
{
	public static class Chartboost
	{
		private const int MINIMUM_SUPPORTED_IOS_VERSION = 8;

		private static IChartboostPlatformBridge _platformBridge;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onCacheInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onFailToLoadInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onDismissInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onCloseInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onClickInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onDisplayInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onCacheRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onFailToLoadRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onDismissRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onCloseRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onClickRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onCompleteRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onDisplayRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ChartboostLocationCallback onWillDisplayVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> onInitFinished__BackingField;

		public static event ChartboostLocationCallback onCacheInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onCacheInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCacheInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onCacheInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCacheInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onFailToLoadInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onFailToLoadInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onFailToLoadInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onFailToLoadInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onFailToLoadInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onDismissInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onDismissInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDismissInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onDismissInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDismissInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onCloseInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onCloseInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCloseInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onCloseInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCloseInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onClickInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onClickInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onClickInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onClickInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onClickInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onDisplayInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onDisplayInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDisplayInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onDisplayInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDisplayInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onCacheRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onCacheRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCacheRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onCacheRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCacheRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onFailToLoadRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onFailToLoadRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onFailToLoadRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onFailToLoadRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onFailToLoadRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onDismissRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onDismissRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDismissRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onDismissRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDismissRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onCloseRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onCloseRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCloseRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onCloseRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCloseRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onClickRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onClickRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onClickRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onClickRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onClickRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onCompleteRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onCompleteRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCompleteRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onCompleteRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onCompleteRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onDisplayRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onDisplayRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDisplayRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onDisplayRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onDisplayRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public static event ChartboostLocationCallback onWillDisplayVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = onWillDisplayVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onWillDisplayVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = onWillDisplayVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref onWillDisplayVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

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

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("Chartboost: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_CHARTBOOST_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void TriggerEvent(ChartboostLocationCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}

		private static bool IsInitialized()
		{
			if (_platformBridge == null)
			{
				return false;
			}
			return true;
		}

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.Android:
				return true;
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
			default:
				return false;
			}
		}

		public static void StartSession(string appId, string appSignature)
		{
			if (IsInitialized())
			{
				return;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = ChartboostIOS.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = ChartboostAndroid.CreatePlugin();
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
				_platformBridge.didCacheInterstitial += (string obj) =>
				{
					TriggerEvent(onCacheInterstitial__BackingField, obj);
				};
				_platformBridge.didClickInterstitial += (string obj) =>
				{
					TriggerEvent(onClickInterstitial__BackingField, obj);
				};
				_platformBridge.didCloseInterstitial += (string obj) =>
				{
					TriggerEvent(onCloseInterstitial__BackingField, obj);
				};
				_platformBridge.didDismissInterstitial += (string obj) =>
				{
					TriggerEvent(onDismissInterstitial__BackingField, obj);
				};
				_platformBridge.didFailToLoadInterstitial += (string obj) =>
				{
					TriggerEvent(onFailToLoadInterstitial__BackingField, obj);
				};
				_platformBridge.didDisplayInterstitial += (string obj) =>
				{
					TriggerEvent(onDisplayInterstitial__BackingField, obj);
				};
				_platformBridge.didCacheRewardedVideo += (string obj) =>
				{
					TriggerEvent(onCacheRewardedVideo__BackingField, obj);
				};
				_platformBridge.didFailToLoadRewardedVideo += (string obj) =>
				{
					TriggerEvent(onFailToLoadRewardedVideo__BackingField, obj);
				};
				_platformBridge.didDismissRewardedVideo += (string obj) =>
				{
					TriggerEvent(onDismissRewardedVideo__BackingField, obj);
				};
				_platformBridge.didCloseRewardedVideo += (string obj) =>
				{
					TriggerEvent(onCloseRewardedVideo__BackingField, obj);
				};
				_platformBridge.didClickRewardedVideo += (string obj) =>
				{
					TriggerEvent(onClickRewardedVideo__BackingField, obj);
				};
				_platformBridge.didCompleteRewardedVideo += (string obj) =>
				{
					TriggerEvent(onCompleteRewardedVideo__BackingField, obj);
				};
				_platformBridge.didDisplayRewardedVideo += (string obj) =>
				{
					TriggerEvent(onDisplayRewardedVideo__BackingField, obj);
				};
				_platformBridge.willDisplayVideo += (string obj) =>
				{
					TriggerEvent(onWillDisplayVideo__BackingField, obj);
				};
				_platformBridge.onInitFinished += (bool succeeded) =>
				{
					onInitFinished__BackingField.SafeInvoke(succeeded);
				};
				_platformBridge.StartSession(appId, appSignature, !AgeRestriction.isRestricted);
			}
		}

		public static void CacheInterstitial(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.CacheInterstitial(location);
			}
		}

		public static void ShowInterstitial(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.ShowInterstitial(location);
			}
		}

		public static bool HasCachedInterstitial(string location)
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.HasCachedInterstitial(location);
		}

		public static void SetShouldRequestInterstitial(bool val)
		{
			if (IsInitialized())
			{
				_platformBridge.SetShouldRequestInterstitial(val);
			}
		}

		public static void SetShouldDisplayInterstitial(bool val)
		{
			if (IsInitialized())
			{
				_platformBridge.SetShouldDisplayInterstitial(val);
			}
		}

		public static void SetShouldRequestInterstitialsInFirstSession(bool val)
		{
			if (IsInitialized())
			{
				_platformBridge.SetShouldRequestInterstitialsInFirstSession(val);
			}
		}

		public static void CacheRewardedVideo(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.CacheRewardedVideo(location);
			}
		}

		public static void ShowRewardedVideo(string location)
		{
			if (IsInitialized())
			{
				_platformBridge.ShowRewardedVideo(location);
			}
		}

		public static bool HasCachedRewardedVideo(string location)
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.HasCachedRewardedVideo(location);
		}
	}
}
