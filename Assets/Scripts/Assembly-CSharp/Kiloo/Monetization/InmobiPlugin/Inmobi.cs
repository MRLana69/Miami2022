using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.InmobiPlugin.Internal;
using UnityEngine;

namespace Kiloo.Monetization.InmobiPlugin
{
	public static class Inmobi
	{
		private static IInmobiPlatformBridge _platformBridge;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InmobiCallback onCacheAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InmobiCallback onFailToLoadAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InmobiCallback onDismissAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InmobiCallback willShowAd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InmobiCallback onCompleteIncentive__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InmobiCallback onInitSuccessfully__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static InmobiCallback onFailToDisplayAd__BackingField;

		public static event InmobiCallback onCacheAd
		{
			add
			{
				InmobiCallback inmobiCallback = onCacheAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onCacheAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = onCacheAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onCacheAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public static event InmobiCallback onFailToLoadAd
		{
			add
			{
				InmobiCallback inmobiCallback = onFailToLoadAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onFailToLoadAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = onFailToLoadAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onFailToLoadAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public static event InmobiCallback onDismissAd
		{
			add
			{
				InmobiCallback inmobiCallback = onDismissAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onDismissAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = onDismissAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onDismissAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public static event InmobiCallback willShowAd
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

		public static event InmobiCallback onCompleteIncentive
		{
			add
			{
				InmobiCallback inmobiCallback = onCompleteIncentive__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onCompleteIncentive__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = onCompleteIncentive__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onCompleteIncentive__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public static event InmobiCallback onInitSuccessfully
		{
			add
			{
				InmobiCallback inmobiCallback = onInitSuccessfully__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onInitSuccessfully__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = onInitSuccessfully__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onInitSuccessfully__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		public static event InmobiCallback onFailToDisplayAd
		{
			add
			{
				InmobiCallback inmobiCallback = onFailToDisplayAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onFailToDisplayAd__BackingField, (InmobiCallback)Delegate.Combine(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
			remove
			{
				InmobiCallback inmobiCallback = onFailToDisplayAd__BackingField;
				InmobiCallback inmobiCallback2;
				do
				{
					inmobiCallback2 = inmobiCallback;
					inmobiCallback = Interlocked.CompareExchange(ref onFailToDisplayAd__BackingField, (InmobiCallback)Delegate.Remove(inmobiCallback2, value), inmobiCallback);
				}
				while ((object)inmobiCallback != inmobiCallback2);
			}
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("[Inmobi] " + msg, context);
		}

		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("INMOBI_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void TriggerEvent(InmobiCallback evt, string data)
		{
			if (evt != null)
			{
				evt(data);
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
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
				return !DeviceUtility.isKindleDevice;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return true;
			default:
				return false;
			}
		}

		public static void Initialize(string accountId)
		{
			if (IsInitialized())
			{
				return;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				_platformBridge = InmobiIOS.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				_platformBridge = InmobiAndroid.CreatePlugin();
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
				_platformBridge.didCacheAd += (string obj) =>
				{
					TriggerEvent(onCacheAd__BackingField, obj);
				};
				_platformBridge.didDismissAd += (string obj) =>
				{
					TriggerEvent(onDismissAd__BackingField, obj);
				};
				_platformBridge.didFailToLoadAd += (string obj) =>
				{
					TriggerEvent(onFailToLoadAd__BackingField, obj);
				};
				_platformBridge.willShowAd += (string obj) =>
				{
					TriggerEvent(willShowAd__BackingField, obj);
				};
				_platformBridge.didCompleteIncentive += (string obj) =>
				{
					TriggerEvent(onCompleteIncentive__BackingField, obj);
				};
				_platformBridge.didInitSuccessfully += (string obj) =>
				{
					TriggerEvent(onInitSuccessfully__BackingField, obj);
				};
				_platformBridge.didFailToDisplayAd += (string obj) =>
				{
					TriggerEvent(onFailToDisplayAd__BackingField, obj);
				};
				_platformBridge.Init(accountId);
			}
		}

		public static void CacheAd(string placementId)
		{
			if (IsInitialized())
			{
				_platformBridge.CacheAd(placementId);
			}
		}

		public static void ShowAd(string placementId)
		{
			if (IsInitialized())
			{
				_platformBridge.ShowAd(placementId);
			}
		}

		public static bool HasCachedAd(string placementId)
		{
			if (!IsInitialized())
			{
				return false;
			}
			return _platformBridge.HasCachedAd(placementId);
		}
	}
}
