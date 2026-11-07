using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Kiloo.Monetization.InmobiPlugin.Internal
{
	public class InmobiIOS : MonoBehaviour, IInmobiPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "InmobiBridgeReceiver";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback didCacheAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback didFailToLoadAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback didDismissAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback willShowAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback didClickAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback didCompleteIncentive__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback didInitSuccessfully__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private InmobiCallback didFailToDisplayAd__BackingField;

		public event InmobiCallback didCacheAd
		{
			add
			{
				InmobiCallback inmobiCallback = didCacheAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didCacheAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = didCacheAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didCacheAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public event InmobiCallback didFailToLoadAd
		{
			add
			{
				InmobiCallback inmobiCallback = didFailToLoadAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didFailToLoadAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = didFailToLoadAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didFailToLoadAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public event InmobiCallback didDismissAd
		{
			add
			{
				InmobiCallback inmobiCallback = didDismissAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didDismissAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = didDismissAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didDismissAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public event InmobiCallback willShowAd
		{
			add
			{
				InmobiCallback inmobiCallback = willShowAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref willShowAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = willShowAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref willShowAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public event InmobiCallback didClickAd
		{
			add
			{
				InmobiCallback inmobiCallback = didClickAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didClickAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = didClickAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didClickAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public event InmobiCallback didCompleteIncentive
		{
			add
			{
				InmobiCallback inmobiCallback = didCompleteIncentive__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didCompleteIncentive__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = didCompleteIncentive__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didCompleteIncentive__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public event InmobiCallback didInitSuccessfully
		{
			add
			{
				InmobiCallback inmobiCallback = didInitSuccessfully__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didInitSuccessfully__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = didInitSuccessfully__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didInitSuccessfully__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public event InmobiCallback didFailToDisplayAd
		{
			add
			{
				InmobiCallback inmobiCallback = didFailToDisplayAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didFailToDisplayAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = didFailToDisplayAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref didFailToDisplayAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("[InmobiIOS] " + msg, context);
		}

		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("INMOBI_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void _inmobiplugin_init(string accountId)
		{
		}

		private static void _inmobiplugin_cacheAd(string placementId)
		{
		}

		private static void _inmobiplugin_showAd(string placementId)
		{
		}

		private static bool _inmobiplugin_hasCachedAd(string placementId)
		{
			return false;
		}

		private static void _inmobiplugin_setDebugLogEnabled(bool val)
		{
		}

		public void Init(string accountId)
		{
			_inmobiplugin_init(accountId);
		}

		public void CacheAd(string placementId)
		{
			_inmobiplugin_cacheAd(placementId);
		}

		public void ShowAd(string placementId)
		{
			_inmobiplugin_showAd(placementId);
		}

		public bool HasCachedAd(string placementId)
		{
			return _inmobiplugin_hasCachedAd(placementId);
		}

		private void callback_didInitSuccessfully(string msg)
		{
			TriggerEvent(didInitSuccessfully__BackingField, msg);
		}

		private void callback_didCacheAd(string msg)
		{
			TriggerEvent(didCacheAd__BackingField, msg);
		}

		private void callback_didFailToLoadAd(string msg)
		{
			TriggerEvent(didFailToLoadAd__BackingField, msg);
		}

		private void callback_didFailToPresentAd(string msg)
		{
			TriggerEvent(didFailToDisplayAd__BackingField, msg);
		}

		private void callback_didDismissAd(string msg)
		{
			TriggerEvent(didDismissAd__BackingField, msg);
		}

		private void callback_willShowAd(string msg)
		{
			TriggerEvent(willShowAd__BackingField, msg);
		}

		private void callback_didCompleteIncentive(string msg)
		{
			TriggerEvent(didCompleteIncentive__BackingField, msg);
		}

		private void callback_didInteractWithParams(string msg)
		{
			TriggerEvent(didClickAd__BackingField, msg);
		}

		public static InmobiIOS CreatePlugin()
		{
			_inmobiplugin_setDebugLogEnabled(false);
			GameObject gameObject = new GameObject("InmobiBridgeReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<InmobiIOS>();
		}

		private void TriggerEvent(InmobiCallback evt, string data)
		{
			if (evt != null)
			{
				evt(data);
			}
		}
	}
}
