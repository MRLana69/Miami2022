using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Monetization.AdColonyPlugin.Internal
{
	public class AdColonyAndroid : MonoBehaviour, IAdColonyPlatformBridge
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

		private AndroidJavaObject _androidBridge;

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
			SDebug.LogError("AdColonyAndroid: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		public void Configure(string appID, string[] zoneIDs)
		{
			bool val = false;
			_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.adcolony.AdColonyBridgeAndroid");
			_androidBridge.Call("configure", appID, zoneIDs);
			_androidBridge.Set("debugLogEnabled", val);
		}

		public void PlayVideoAd(string zoneID)
		{
			MakeNativeCall("PlayAd", zoneID);
		}

		public bool IsVideoExpired(string zoneID)
		{
			return MakeNativeCall<bool>("isVideoAdExpired", new object[1] { zoneID });
		}

		public void LoadAd(string zoneID)
		{
			MakeNativeCall("LoadAd", zoneID);
		}

		private T MakeNativeCall<T>(string call, params object[] args)
		{
			return _androidBridge.Call<T>(call, args);
		}

		private void MakeNativeCall(string call, params object[] args)
		{
			_androidBridge.Call(call, args);
		}

		private void _callback_onInitFinished(string s)
		{
			onInitFinished__BackingField.SafeInvoke();
		}

		private void _callback_onRequestFilled(string cacheID)
		{
			onCachingSuccess__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onRequestNotFilled(string cacheID)
		{
			onCachingFailed__BackingField.SafeInvoke(cacheID);
		}

		private void _callback_onReward(string success)
		{
			onReward__BackingField.SafeInvoke(bool.Parse(success));
		}

		private void _callback_onOpened(string s)
		{
			onAdStarted__BackingField.SafeInvoke();
		}

		private void _callback_onClosed(string s)
		{
			onAdClosed__BackingField.SafeInvoke();
		}

		private void _callback_onClicked(string s)
		{
			onAdClicked__BackingField.SafeInvoke();
		}

		public static AdColonyAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("AdColonyBridgeReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<AdColonyAndroid>();
		}
	}
}
