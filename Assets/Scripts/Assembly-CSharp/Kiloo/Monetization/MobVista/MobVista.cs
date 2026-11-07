using System;
using System.Diagnostics;
using System.Threading;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.MobVista.Internal;
using UnityEngine;

namespace Kiloo.Monetization.MobVista
{
	public class MobVista
	{
		private static IMobVistaPlatformBridge _platformBridge;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoShowEvent onVideoShow__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdShowFailedEvent onVideoAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdCloseEvent onVideoAdClose__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdCloseEvent onVideoAdWatched__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoLoadFailedEvent onVideoLoadFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoLoaded onVideoLoaded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdClicked onVideoAdClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoShowEvent onInterstitialShow__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdShowFailedEvent onInterstitialAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdCloseEvent onInterstitialAdClose__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdCloseEvent onInterstitialAdWatched__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoLoadFailedEvent onInterstitialLoadFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoLoaded onInterstitialLoaded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MobVistaVideoAdClicked onInterstitialAdClicked__BackingField;

		private const int UNSUPPORTED_ANDROID_API = 21;

		public static bool isSupportedOnPlatform
		{
			get
			{
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
					return true;
				case RuntimePlatform.Android:
					if (DeviceUtility.isKindleDevice)
					{
						return false;
					}
					return DeviceUtility.GetAndroidApiLevel() != 21;
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.WindowsEditor:
					return false;
				default:
					return false;
				}
			}
		}

		public static bool isInterstitialSupportedOnPlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform == RuntimePlatform.IPhonePlayer)
				{
					return true;
				}
				return false;
			}
		}

		public static event MobVistaVideoShowEvent onVideoShow
		{
			add
			{
				MobVistaVideoShowEvent mobVistaVideoShowEvent = onVideoShow__BackingField;
				MobVistaVideoShowEvent mobVistaVideoShowEvent2;
				do
				{
					mobVistaVideoShowEvent2 = mobVistaVideoShowEvent;
					mobVistaVideoShowEvent = Interlocked.CompareExchange(ref onVideoShow__BackingField, (MobVistaVideoShowEvent)Delegate.Combine(mobVistaVideoShowEvent2, value), mobVistaVideoShowEvent);
				}
				while ((object)mobVistaVideoShowEvent != mobVistaVideoShowEvent2);
			}
			remove
			{
				MobVistaVideoShowEvent mobVistaVideoShowEvent = onVideoShow__BackingField;
				MobVistaVideoShowEvent mobVistaVideoShowEvent2;
				do
				{
					mobVistaVideoShowEvent2 = mobVistaVideoShowEvent;
					mobVistaVideoShowEvent = Interlocked.CompareExchange(ref onVideoShow__BackingField, (MobVistaVideoShowEvent)Delegate.Remove(mobVistaVideoShowEvent2, value), mobVistaVideoShowEvent);
				}
				while ((object)mobVistaVideoShowEvent != mobVistaVideoShowEvent2);
			}
		}

		public static event MobVistaVideoAdShowFailedEvent onVideoAdShowFailed
		{
			add
			{
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent = onVideoAdShowFailed__BackingField;
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent2;
				do
				{
					mobVistaVideoAdShowFailedEvent2 = mobVistaVideoAdShowFailedEvent;
					mobVistaVideoAdShowFailedEvent = Interlocked.CompareExchange(ref onVideoAdShowFailed__BackingField, (MobVistaVideoAdShowFailedEvent)Delegate.Combine(mobVistaVideoAdShowFailedEvent2, value), mobVistaVideoAdShowFailedEvent);
				}
				while ((object)mobVistaVideoAdShowFailedEvent != mobVistaVideoAdShowFailedEvent2);
			}
			remove
			{
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent = onVideoAdShowFailed__BackingField;
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent2;
				do
				{
					mobVistaVideoAdShowFailedEvent2 = mobVistaVideoAdShowFailedEvent;
					mobVistaVideoAdShowFailedEvent = Interlocked.CompareExchange(ref onVideoAdShowFailed__BackingField, (MobVistaVideoAdShowFailedEvent)Delegate.Remove(mobVistaVideoAdShowFailedEvent2, value), mobVistaVideoAdShowFailedEvent);
				}
				while ((object)mobVistaVideoAdShowFailedEvent != mobVistaVideoAdShowFailedEvent2);
			}
		}

		public static event MobVistaVideoAdCloseEvent onVideoAdClose
		{
			add
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onVideoAdClose__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onVideoAdClose__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Combine(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
			remove
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onVideoAdClose__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onVideoAdClose__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Remove(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
		}

		public static event MobVistaVideoAdCloseEvent onVideoAdWatched
		{
			add
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onVideoAdWatched__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onVideoAdWatched__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Combine(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
			remove
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onVideoAdWatched__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onVideoAdWatched__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Remove(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
		}

		public static event MobVistaVideoLoadFailedEvent onVideoLoadFailed
		{
			add
			{
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent = onVideoLoadFailed__BackingField;
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent2;
				do
				{
					mobVistaVideoLoadFailedEvent2 = mobVistaVideoLoadFailedEvent;
					mobVistaVideoLoadFailedEvent = Interlocked.CompareExchange(ref onVideoLoadFailed__BackingField, (MobVistaVideoLoadFailedEvent)Delegate.Combine(mobVistaVideoLoadFailedEvent2, value), mobVistaVideoLoadFailedEvent);
				}
				while ((object)mobVistaVideoLoadFailedEvent != mobVistaVideoLoadFailedEvent2);
			}
			remove
			{
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent = onVideoLoadFailed__BackingField;
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent2;
				do
				{
					mobVistaVideoLoadFailedEvent2 = mobVistaVideoLoadFailedEvent;
					mobVistaVideoLoadFailedEvent = Interlocked.CompareExchange(ref onVideoLoadFailed__BackingField, (MobVistaVideoLoadFailedEvent)Delegate.Remove(mobVistaVideoLoadFailedEvent2, value), mobVistaVideoLoadFailedEvent);
				}
				while ((object)mobVistaVideoLoadFailedEvent != mobVistaVideoLoadFailedEvent2);
			}
		}

		public static event MobVistaVideoLoaded onVideoLoaded
		{
			add
			{
				MobVistaVideoLoaded mobVistaVideoLoaded = onVideoLoaded__BackingField;
				MobVistaVideoLoaded mobVistaVideoLoaded2;
				do
				{
					mobVistaVideoLoaded2 = mobVistaVideoLoaded;
					mobVistaVideoLoaded = Interlocked.CompareExchange(ref onVideoLoaded__BackingField, (MobVistaVideoLoaded)Delegate.Combine(mobVistaVideoLoaded2, value), mobVistaVideoLoaded);
				}
				while ((object)mobVistaVideoLoaded != mobVistaVideoLoaded2);
			}
			remove
			{
				MobVistaVideoLoaded mobVistaVideoLoaded = onVideoLoaded__BackingField;
				MobVistaVideoLoaded mobVistaVideoLoaded2;
				do
				{
					mobVistaVideoLoaded2 = mobVistaVideoLoaded;
					mobVistaVideoLoaded = Interlocked.CompareExchange(ref onVideoLoaded__BackingField, (MobVistaVideoLoaded)Delegate.Remove(mobVistaVideoLoaded2, value), mobVistaVideoLoaded);
				}
				while ((object)mobVistaVideoLoaded != mobVistaVideoLoaded2);
			}
		}

		public static event MobVistaVideoAdClicked onVideoAdClicked
		{
			add
			{
				MobVistaVideoAdClicked mobVistaVideoAdClicked = onVideoAdClicked__BackingField;
				MobVistaVideoAdClicked mobVistaVideoAdClicked2;
				do
				{
					mobVistaVideoAdClicked2 = mobVistaVideoAdClicked;
					mobVistaVideoAdClicked = Interlocked.CompareExchange(ref onVideoAdClicked__BackingField, (MobVistaVideoAdClicked)Delegate.Combine(mobVistaVideoAdClicked2, value), mobVistaVideoAdClicked);
				}
				while ((object)mobVistaVideoAdClicked != mobVistaVideoAdClicked2);
			}
			remove
			{
				MobVistaVideoAdClicked mobVistaVideoAdClicked = onVideoAdClicked__BackingField;
				MobVistaVideoAdClicked mobVistaVideoAdClicked2;
				do
				{
					mobVistaVideoAdClicked2 = mobVistaVideoAdClicked;
					mobVistaVideoAdClicked = Interlocked.CompareExchange(ref onVideoAdClicked__BackingField, (MobVistaVideoAdClicked)Delegate.Remove(mobVistaVideoAdClicked2, value), mobVistaVideoAdClicked);
				}
				while ((object)mobVistaVideoAdClicked != mobVistaVideoAdClicked2);
			}
		}

		public static event MobVistaVideoShowEvent onInterstitialShow
		{
			add
			{
				MobVistaVideoShowEvent mobVistaVideoShowEvent = onInterstitialShow__BackingField;
				MobVistaVideoShowEvent mobVistaVideoShowEvent2;
				do
				{
					mobVistaVideoShowEvent2 = mobVistaVideoShowEvent;
					mobVistaVideoShowEvent = Interlocked.CompareExchange(ref onInterstitialShow__BackingField, (MobVistaVideoShowEvent)Delegate.Combine(mobVistaVideoShowEvent2, value), mobVistaVideoShowEvent);
				}
				while ((object)mobVistaVideoShowEvent != mobVistaVideoShowEvent2);
			}
			remove
			{
				MobVistaVideoShowEvent mobVistaVideoShowEvent = onInterstitialShow__BackingField;
				MobVistaVideoShowEvent mobVistaVideoShowEvent2;
				do
				{
					mobVistaVideoShowEvent2 = mobVistaVideoShowEvent;
					mobVistaVideoShowEvent = Interlocked.CompareExchange(ref onInterstitialShow__BackingField, (MobVistaVideoShowEvent)Delegate.Remove(mobVistaVideoShowEvent2, value), mobVistaVideoShowEvent);
				}
				while ((object)mobVistaVideoShowEvent != mobVistaVideoShowEvent2);
			}
		}

		public static event MobVistaVideoAdShowFailedEvent onInterstitialAdShowFailed
		{
			add
			{
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent = onInterstitialAdShowFailed__BackingField;
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent2;
				do
				{
					mobVistaVideoAdShowFailedEvent2 = mobVistaVideoAdShowFailedEvent;
					mobVistaVideoAdShowFailedEvent = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (MobVistaVideoAdShowFailedEvent)Delegate.Combine(mobVistaVideoAdShowFailedEvent2, value), mobVistaVideoAdShowFailedEvent);
				}
				while ((object)mobVistaVideoAdShowFailedEvent != mobVistaVideoAdShowFailedEvent2);
			}
			remove
			{
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent = onInterstitialAdShowFailed__BackingField;
				MobVistaVideoAdShowFailedEvent mobVistaVideoAdShowFailedEvent2;
				do
				{
					mobVistaVideoAdShowFailedEvent2 = mobVistaVideoAdShowFailedEvent;
					mobVistaVideoAdShowFailedEvent = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (MobVistaVideoAdShowFailedEvent)Delegate.Remove(mobVistaVideoAdShowFailedEvent2, value), mobVistaVideoAdShowFailedEvent);
				}
				while ((object)mobVistaVideoAdShowFailedEvent != mobVistaVideoAdShowFailedEvent2);
			}
		}

		public static event MobVistaVideoAdCloseEvent onInterstitialAdClose
		{
			add
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onInterstitialAdClose__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onInterstitialAdClose__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Combine(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
			remove
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onInterstitialAdClose__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onInterstitialAdClose__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Remove(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
		}

		public static event MobVistaVideoAdCloseEvent onInterstitialAdWatched
		{
			add
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onInterstitialAdWatched__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onInterstitialAdWatched__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Combine(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
			remove
			{
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent = onInterstitialAdWatched__BackingField;
				MobVistaVideoAdCloseEvent mobVistaVideoAdCloseEvent2;
				do
				{
					mobVistaVideoAdCloseEvent2 = mobVistaVideoAdCloseEvent;
					mobVistaVideoAdCloseEvent = Interlocked.CompareExchange(ref onInterstitialAdWatched__BackingField, (MobVistaVideoAdCloseEvent)Delegate.Remove(mobVistaVideoAdCloseEvent2, value), mobVistaVideoAdCloseEvent);
				}
				while ((object)mobVistaVideoAdCloseEvent != mobVistaVideoAdCloseEvent2);
			}
		}

		public static event MobVistaVideoLoadFailedEvent onInterstitialLoadFailed
		{
			add
			{
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent = onInterstitialLoadFailed__BackingField;
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent2;
				do
				{
					mobVistaVideoLoadFailedEvent2 = mobVistaVideoLoadFailedEvent;
					mobVistaVideoLoadFailedEvent = Interlocked.CompareExchange(ref onInterstitialLoadFailed__BackingField, (MobVistaVideoLoadFailedEvent)Delegate.Combine(mobVistaVideoLoadFailedEvent2, value), mobVistaVideoLoadFailedEvent);
				}
				while ((object)mobVistaVideoLoadFailedEvent != mobVistaVideoLoadFailedEvent2);
			}
			remove
			{
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent = onInterstitialLoadFailed__BackingField;
				MobVistaVideoLoadFailedEvent mobVistaVideoLoadFailedEvent2;
				do
				{
					mobVistaVideoLoadFailedEvent2 = mobVistaVideoLoadFailedEvent;
					mobVistaVideoLoadFailedEvent = Interlocked.CompareExchange(ref onInterstitialLoadFailed__BackingField, (MobVistaVideoLoadFailedEvent)Delegate.Remove(mobVistaVideoLoadFailedEvent2, value), mobVistaVideoLoadFailedEvent);
				}
				while ((object)mobVistaVideoLoadFailedEvent != mobVistaVideoLoadFailedEvent2);
			}
		}

		public static event MobVistaVideoLoaded onInterstitialLoaded
		{
			add
			{
				MobVistaVideoLoaded mobVistaVideoLoaded = onInterstitialLoaded__BackingField;
				MobVistaVideoLoaded mobVistaVideoLoaded2;
				do
				{
					mobVistaVideoLoaded2 = mobVistaVideoLoaded;
					mobVistaVideoLoaded = Interlocked.CompareExchange(ref onInterstitialLoaded__BackingField, (MobVistaVideoLoaded)Delegate.Combine(mobVistaVideoLoaded2, value), mobVistaVideoLoaded);
				}
				while ((object)mobVistaVideoLoaded != mobVistaVideoLoaded2);
			}
			remove
			{
				MobVistaVideoLoaded mobVistaVideoLoaded = onInterstitialLoaded__BackingField;
				MobVistaVideoLoaded mobVistaVideoLoaded2;
				do
				{
					mobVistaVideoLoaded2 = mobVistaVideoLoaded;
					mobVistaVideoLoaded = Interlocked.CompareExchange(ref onInterstitialLoaded__BackingField, (MobVistaVideoLoaded)Delegate.Remove(mobVistaVideoLoaded2, value), mobVistaVideoLoaded);
				}
				while ((object)mobVistaVideoLoaded != mobVistaVideoLoaded2);
			}
		}

		public static event MobVistaVideoAdClicked onInterstitialAdClicked
		{
			add
			{
				MobVistaVideoAdClicked mobVistaVideoAdClicked = onInterstitialAdClicked__BackingField;
				MobVistaVideoAdClicked mobVistaVideoAdClicked2;
				do
				{
					mobVistaVideoAdClicked2 = mobVistaVideoAdClicked;
					mobVistaVideoAdClicked = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (MobVistaVideoAdClicked)Delegate.Combine(mobVistaVideoAdClicked2, value), mobVistaVideoAdClicked);
				}
				while ((object)mobVistaVideoAdClicked != mobVistaVideoAdClicked2);
			}
			remove
			{
				MobVistaVideoAdClicked mobVistaVideoAdClicked = onInterstitialAdClicked__BackingField;
				MobVistaVideoAdClicked mobVistaVideoAdClicked2;
				do
				{
					mobVistaVideoAdClicked2 = mobVistaVideoAdClicked;
					mobVistaVideoAdClicked = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (MobVistaVideoAdClicked)Delegate.Remove(mobVistaVideoAdClicked2, value), mobVistaVideoAdClicked);
				}
				while ((object)mobVistaVideoAdClicked != mobVistaVideoAdClicked2);
			}
		}

		private static IMobVistaPlatformBridge CreateInitializedPlatformBridge(string appId, string sdkKey)
		{
			IMobVistaPlatformBridge mobVistaPlatformBridge = null;
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				mobVistaPlatformBridge = MobVistaIos.CreatePlugin();
				break;
			case RuntimePlatform.Android:
				mobVistaPlatformBridge = MobVistaAndroid.CreatePlugin();
				break;
			default:
				return null;
			}
			mobVistaPlatformBridge.Initialize(appId, sdkKey, !AgeRestriction.isRestricted);
			return mobVistaPlatformBridge;
		}

		public static void Initialize(string appId, string sdkKey)
		{
			_platformBridge = CreateInitializedPlatformBridge(appId, sdkKey);
			UpdateDebugLogsState();
			SetupEventListeners();
		}

		private static void SetupEventListeners()
		{
			_platformBridge.onVideoShow += OnVideoShow;
			_platformBridge.onVideoAdClose += OnVideoAdClose;
			_platformBridge.onVideoAdWatched += OnVideoAdWatched;
			_platformBridge.onVideoLoadFailed += OnVideoLoadFailed;
			_platformBridge.onVideoAdShowFailed += OnVideoAdShowFailed;
			_platformBridge.onVideoAdClicked += OnVideoAdClicked;
			_platformBridge.onVideoLoaded += OnVideoLoaded;
			_platformBridge.onInterstitialShow += OnInterstitialShow;
			_platformBridge.onInterstitialAdClose += OnInterstitialAdClose;
			_platformBridge.onInterstitialAdWatched += OnInterstitialAdWatched;
			_platformBridge.onInterstitialLoadFailed += OnInterstitialLoadFailed;
			_platformBridge.onInterstitialAdShowFailed += OnInterstitialAdShowFailed;
			_platformBridge.onInterstitialAdClicked += OnInterstitialAdClicked;
			_platformBridge.onInterstitialLoaded += OnInterstitialLoaded;
		}

		private static void UpdateDebugLogsState()
		{
			_platformBridge.DisableDebugLogs();
		}

		public static void LoadFullScreenAd(string placementId)
		{
			_platformBridge.LoadFullScreenAd(placementId);
		}

		public static void ShowFullScreenAd(string placementId)
		{
			_platformBridge.ShowFullScreenAd(placementId);
		}

		public static bool IsFullScreenAdLoaded(string placementId)
		{
			return _platformBridge.IsFullScreenAdLoaded(placementId);
		}

		public static void LoadInterstitialAd(string placementId)
		{
			_platformBridge.LoadInterstitialAd(placementId);
		}

		public static void ShowInterstitialAd(string placementId)
		{
			_platformBridge.ShowInterstitialAd(placementId);
		}

		public static bool IsInterstitialAdLoaded(string placementId)
		{
			return _platformBridge.IsInterstitialAdLoaded(placementId);
		}

		private static void OnVideoLoaded(string cacheID)
		{
			if (onVideoLoaded__BackingField != null)
			{
				onVideoLoaded__BackingField(cacheID);
			}
		}

		private static void OnVideoShow(string cacheID)
		{
			if (onVideoShow__BackingField != null)
			{
				onVideoShow__BackingField(cacheID);
			}
		}

		private static void OnVideoAdClose(string cacheID)
		{
			if (onVideoAdClose__BackingField != null)
			{
				onVideoAdClose__BackingField(cacheID);
			}
		}

		private static void OnVideoAdWatched(string cacheID)
		{
			if (onVideoAdWatched__BackingField != null)
			{
				onVideoAdWatched__BackingField(cacheID);
			}
		}

		private static void OnVideoLoadFailed(string cacheID)
		{
			if (onVideoLoadFailed__BackingField != null)
			{
				onVideoLoadFailed__BackingField(cacheID);
			}
		}

		private static void OnVideoAdShowFailed(string cacheID)
		{
			if (onVideoAdShowFailed__BackingField != null)
			{
				onVideoAdShowFailed__BackingField(cacheID);
			}
		}

		private static void OnVideoAdClicked(string unitId)
		{
			if (onVideoAdClicked__BackingField != null)
			{
				onVideoAdClicked__BackingField(unitId);
			}
		}

		private static void OnInterstitialLoaded(string cacheID)
		{
			if (onInterstitialLoaded__BackingField != null)
			{
				onInterstitialLoaded__BackingField(cacheID);
			}
		}

		private static void OnInterstitialShow(string cacheID)
		{
			if (onInterstitialShow__BackingField != null)
			{
				onInterstitialShow__BackingField(cacheID);
			}
		}

		private static void OnInterstitialAdClose(string cacheID)
		{
			if (onInterstitialAdClose__BackingField != null)
			{
				onInterstitialAdClose__BackingField(cacheID);
			}
		}

		private static void OnInterstitialAdWatched(string cacheID)
		{
			if (onInterstitialAdWatched__BackingField != null)
			{
				onInterstitialAdWatched__BackingField(cacheID);
			}
		}

		private static void OnInterstitialLoadFailed(string cacheID)
		{
			if (onInterstitialLoadFailed__BackingField != null)
			{
				onInterstitialLoadFailed__BackingField(cacheID);
			}
		}

		private static void OnInterstitialAdShowFailed(string cacheID)
		{
			if (onInterstitialAdShowFailed__BackingField != null)
			{
				onInterstitialAdShowFailed__BackingField(cacheID);
			}
		}

		private static void OnInterstitialAdClicked(string unitId)
		{
			if (onInterstitialAdClicked__BackingField != null)
			{
				onInterstitialAdClicked__BackingField(unitId);
			}
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("Mobvista " + msg, context);
		}

		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_MOBVISTA_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
