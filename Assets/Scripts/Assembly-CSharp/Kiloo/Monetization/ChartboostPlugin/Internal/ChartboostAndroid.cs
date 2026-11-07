using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Monetization.ChartboostPlugin.Internal
{
	public class ChartboostAndroid : MonoBehaviour, IChartboostPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "ChartboostBridgeReceiver";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didCacheInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didFailToLoadInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didDismissInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didCloseInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didClickInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didDisplayInterstitial__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didCacheRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didFailToLoadRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didDismissRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didCloseRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didClickRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didCompleteRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback didDisplayRewardedVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ChartboostLocationCallback willDisplayVideo__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<bool> onInitFinished__BackingField;

		private AndroidJavaObject _androidBridge;

		public event ChartboostLocationCallback didCacheInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didCacheInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCacheInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didCacheInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCacheInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didFailToLoadInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didFailToLoadInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didFailToLoadInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didFailToLoadInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didFailToLoadInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didDismissInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didDismissInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDismissInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didDismissInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDismissInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didCloseInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didCloseInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCloseInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didCloseInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCloseInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didClickInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didClickInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didClickInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didClickInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didClickInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didDisplayInterstitial
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didDisplayInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDisplayInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didDisplayInterstitial__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDisplayInterstitial__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didCacheRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didCacheRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCacheRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didCacheRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCacheRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didFailToLoadRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didFailToLoadRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didFailToLoadRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didFailToLoadRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didFailToLoadRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didDismissRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didDismissRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDismissRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didDismissRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDismissRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didCloseRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didCloseRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCloseRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didCloseRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCloseRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didClickRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didClickRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didClickRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didClickRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didClickRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didCompleteRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didCompleteRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCompleteRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didCompleteRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didCompleteRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback didDisplayRewardedVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = didDisplayRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDisplayRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = didDisplayRewardedVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref didDisplayRewardedVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event ChartboostLocationCallback willDisplayVideo
		{
			add
			{
				ChartboostLocationCallback chartboostLocationCallback = willDisplayVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref willDisplayVideo__BackingField, (ChartboostLocationCallback)Delegate.Combine(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
			remove
			{
				ChartboostLocationCallback chartboostLocationCallback = willDisplayVideo__BackingField;
				ChartboostLocationCallback chartboostLocationCallback2;
				do
				{
					chartboostLocationCallback2 = chartboostLocationCallback;
					chartboostLocationCallback = Interlocked.CompareExchange(ref willDisplayVideo__BackingField, (ChartboostLocationCallback)Delegate.Remove(chartboostLocationCallback2, value), chartboostLocationCallback);
				}
				while ((object)chartboostLocationCallback != chartboostLocationCallback2);
			}
		}

		public event Action<bool> onInitFinished
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
			SDebug.LogError("ChartboostAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_CHARTBOOST_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private void callback_didCacheInterstitial(string location)
		{
			TriggerEvent(didCacheInterstitial__BackingField, location);
		}

		private void callback_didFailToLoadInterstitial(string location)
		{
			TriggerEvent(didFailToLoadInterstitial__BackingField, location);
		}

		private void callback_didDismissInterstitial(string location)
		{
			TriggerEvent(didDismissInterstitial__BackingField, location);
		}

		private void callback_didCloseInterstitial(string location)
		{
			TriggerEvent(didCloseInterstitial__BackingField, location);
		}

		private void callback_didClickInterstitial(string location)
		{
			TriggerEvent(didClickInterstitial__BackingField, location);
		}

		private void callback_didDisplayInterstitial(string location)
		{
			TriggerEvent(didDisplayInterstitial__BackingField, location);
		}

		private void callback_didDisplayRewardedVideo(string location)
		{
			TriggerEvent(didDisplayRewardedVideo__BackingField, location);
		}

		private void callback_didCacheRewardedVideo(string location)
		{
			TriggerEvent(didCacheRewardedVideo__BackingField, location);
		}

		private void callback_didFailToLoadRewardedVideo(string location)
		{
			TriggerEvent(didFailToLoadRewardedVideo__BackingField, location);
		}

		private void callback_didDismissRewardedVideo(string location)
		{
			TriggerEvent(didDismissRewardedVideo__BackingField, location);
		}

		private void callback_didCloseRewardedVideo(string location)
		{
			TriggerEvent(didCloseRewardedVideo__BackingField, location);
		}

		private void callback_didClickRewardedVideo(string location)
		{
			TriggerEvent(didClickRewardedVideo__BackingField, location);
		}

		private void callback_didCompleteRewardedVideo(string location)
		{
			TriggerEvent(didCompleteRewardedVideo__BackingField, location);
		}

		private void callback_willDisplayVideo(string location)
		{
			TriggerEvent(willDisplayVideo__BackingField, location);
		}

		private void callback_onInitFinished(string success)
		{
			onInitFinished__BackingField.SafeInvoke(success == "true");
		}

		public void StartSession(string appId, string appSignature, bool consentGiven)
		{
			if (_androidBridge == null)
			{
				_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.chartboost.ChartboostBridgeAndroid");
			}
			_androidBridge.Set("debugLogsEnabled", false);
			_androidBridge.Call("startSession", appId, appSignature, Application.unityVersion, consentGiven);
		}

		public void CacheInterstitial(string location)
		{
			_androidBridge.Call("cacheInterstitial", location);
		}

		public void ShowInterstitial(string location)
		{
			_androidBridge.Call("showInterstitial", location);
		}

		public bool HasCachedInterstitial(string location)
		{
			return _androidBridge.Call<bool>("hasCachedInterstitial", new object[1] { location });
		}

		public void SetShouldRequestInterstitial(bool val)
		{
			_androidBridge.Set("delegate_shouldRequestInterstitial", val);
		}

		public void SetShouldDisplayInterstitial(bool val)
		{
			_androidBridge.Set("delegate_shouldDisplayInterstitial", val);
		}

		public void SetShouldRequestInterstitialsInFirstSession(bool val)
		{
			_androidBridge.Set("delegate_shouldRequestInterstitialsInFirstSession", val);
		}

		public void CacheRewardedVideo(string location)
		{
			_androidBridge.Call("cacheRewardedVideo", location);
		}

		public void ShowRewardedVideo(string location)
		{
			_androidBridge.Call("showRewardedVideo", location);
		}

		public bool HasCachedRewardedVideo(string location)
		{
			return _androidBridge.Call<bool>("hasCachedRewardedVideo", new object[1] { location });
		}

		private void TriggerEvent(ChartboostLocationCallback evt, string location)
		{
			if (evt != null)
			{
				evt(location);
			}
		}

		public static ChartboostAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("ChartboostBridgeReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<ChartboostAndroid>();
		}
	}
}
