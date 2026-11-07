using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Monetization.AdColonyPlugin.Internal
{
	public class AdColonyIOS : MonoBehaviour, IAdColonyPlatformBridge
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onInitFinished__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onCachingFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onCachingSuccess__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<bool> onReward__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onAdStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onAdClicked__BackingField;

		private const string CALLBACK_RECEIVER_NAME = "AdColonyBridgeReceiver";

		public event Action onInitFinished
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

		public event Action<string> onCachingFailed
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

		public event Action<string> onCachingSuccess
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

		public event Action<bool> onReward
		{
			add
			{
				Action<bool> action = onReward__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onReward__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onReward__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onReward__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onAdClosed
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

		public event Action onAdStarted
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

		public event Action onAdClicked
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

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("AdColonyIOS: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void _adcolonyplugin_configure(string appID, int zoneIDcount, string[] zoneIDs, bool debug)
		{
		}

		private static void _adcolonyplugin_playVideoAd(string zoneID)
		{
		}

		private static bool _adcolonyplugin_isVideoExpired(string zoneID)
		{
			return false;
		}

		private static void _adcolonyplugin_loadVideoAd(string zoneID)
		{
		}

		public void Configure(string appID, string[] zoneIDs)
		{
			bool debug = false;
			_adcolonyplugin_configure(appID, zoneIDs.Length, zoneIDs, debug);
		}

		public void PlayVideoAd(string zoneID)
		{
			_adcolonyplugin_playVideoAd(zoneID);
		}

		public bool IsVideoExpired(string zoneID)
		{
			return _adcolonyplugin_isVideoExpired(zoneID);
		}

		public void LoadAd(string zoneID)
		{
			_adcolonyplugin_loadVideoAd(zoneID);
		}

		private void _callback_onInitFinished(string empty)
		{
			onInitFinished__BackingField.SafeInvoke();
		}

		private void _callback_onAdLoadedFailed(string cacheId)
		{
			onCachingFailed__BackingField.SafeInvoke(cacheId);
		}

		private void _callback_onAdLoadedSuccess(string cacheId)
		{
			onCachingSuccess__BackingField.SafeInvoke(cacheId);
		}

		private void _callback_onAdStarted(string empty)
		{
			onAdStarted__BackingField.SafeInvoke();
		}

		private void _callback_onAdClosed(string empty)
		{
			onReward__BackingField.SafeInvoke(true);
			onAdClosed__BackingField.SafeInvoke();
		}

		private void _callback_onAdClicked(string empty)
		{
			onAdClicked__BackingField.SafeInvoke();
		}

		public static AdColonyIOS CreatePlugin()
		{
			GameObject gameObject = new GameObject("AdColonyBridgeReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<AdColonyIOS>();
		}
	}
}
