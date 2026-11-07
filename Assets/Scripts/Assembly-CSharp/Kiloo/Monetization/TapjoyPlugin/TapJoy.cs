using System;
using System.Diagnostics;
using System.Threading;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Monetization.TapjoyPlugin.Internal;
using UnityEngine;

namespace Kiloo.Monetization.TapjoyPlugin
{
	public static class TapJoy
	{
		private enum TapJoyConnectionState
		{
			Connecting = 0,
			Connected = 1,
			Disconnected = 2
		}

		public const string INITIALIZE_SUCCESS_RETURN_VALUE = "success";

		private static TapJoyConnectionState _connectionState = TapJoyConnectionState.Disconnected;

		private static ITapjoyBridge _platformBridge;

		private static bool _listenersAdded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyConnectEvent onInitializeFinished__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyGetFullScreenAdSuccess onFullScreenAdSuccess__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyGetFullScreenAdFail onFullScreenAdFail__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyEarnedPointsEvent onEarnPoints__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyGetPointsEvent onGetPoints__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyGetPointsErrorEvent onGetPointsError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoySpendPointsEvent onSpendPoints__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoySpendPointsErrorEvent onSpendPointsError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyViewWillAppearEvent onViewWillAppear__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyViewDidAppearEvent onViewDidAppear__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyViewWillDisappearEvent onViewWillDisappear__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyViewDidDisappearEvent onViewDidDisappear__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyVideoStartEvent onVideoStart__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyVideoErrorEvent onVideoError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyVideoCompleteEvent onVideoComplete__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static TapjoyGetOfferWallResponseEvent onOfferWallResponse__BackingField;

		public static bool isInitialized
		{
			get
			{
				return _platformBridge != null && _connectionState == TapJoyConnectionState.Connected;
			}
		}

		public static event TapjoyConnectEvent onInitializeFinished
		{
			add
			{
				TapjoyConnectEvent tapjoyConnectEvent = onInitializeFinished__BackingField;
				TapjoyConnectEvent tapjoyConnectEvent2;
				do
				{
					tapjoyConnectEvent2 = tapjoyConnectEvent;
					tapjoyConnectEvent = Interlocked.CompareExchange(ref onInitializeFinished__BackingField, (TapjoyConnectEvent)Delegate.Combine(tapjoyConnectEvent2, value), tapjoyConnectEvent);
				}
				while ((object)tapjoyConnectEvent != tapjoyConnectEvent2);
			}
			remove
			{
				TapjoyConnectEvent tapjoyConnectEvent = onInitializeFinished__BackingField;
				TapjoyConnectEvent tapjoyConnectEvent2;
				do
				{
					tapjoyConnectEvent2 = tapjoyConnectEvent;
					tapjoyConnectEvent = Interlocked.CompareExchange(ref onInitializeFinished__BackingField, (TapjoyConnectEvent)Delegate.Remove(tapjoyConnectEvent2, value), tapjoyConnectEvent);
				}
				while ((object)tapjoyConnectEvent != tapjoyConnectEvent2);
			}
		}

		public static event TapjoyGetFullScreenAdSuccess onFullScreenAdSuccess
		{
			add
			{
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess = onFullScreenAdSuccess__BackingField;
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess2;
				do
				{
					tapjoyGetFullScreenAdSuccess2 = tapjoyGetFullScreenAdSuccess;
					tapjoyGetFullScreenAdSuccess = Interlocked.CompareExchange(ref onFullScreenAdSuccess__BackingField, (TapjoyGetFullScreenAdSuccess)Delegate.Combine(tapjoyGetFullScreenAdSuccess2, value), tapjoyGetFullScreenAdSuccess);
				}
				while ((object)tapjoyGetFullScreenAdSuccess != tapjoyGetFullScreenAdSuccess2);
			}
			remove
			{
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess = onFullScreenAdSuccess__BackingField;
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess2;
				do
				{
					tapjoyGetFullScreenAdSuccess2 = tapjoyGetFullScreenAdSuccess;
					tapjoyGetFullScreenAdSuccess = Interlocked.CompareExchange(ref onFullScreenAdSuccess__BackingField, (TapjoyGetFullScreenAdSuccess)Delegate.Remove(tapjoyGetFullScreenAdSuccess2, value), tapjoyGetFullScreenAdSuccess);
				}
				while ((object)tapjoyGetFullScreenAdSuccess != tapjoyGetFullScreenAdSuccess2);
			}
		}

		public static event TapjoyGetFullScreenAdFail onFullScreenAdFail
		{
			add
			{
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail = onFullScreenAdFail__BackingField;
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail2;
				do
				{
					tapjoyGetFullScreenAdFail2 = tapjoyGetFullScreenAdFail;
					tapjoyGetFullScreenAdFail = Interlocked.CompareExchange(ref onFullScreenAdFail__BackingField, (TapjoyGetFullScreenAdFail)Delegate.Combine(tapjoyGetFullScreenAdFail2, value), tapjoyGetFullScreenAdFail);
				}
				while ((object)tapjoyGetFullScreenAdFail != tapjoyGetFullScreenAdFail2);
			}
			remove
			{
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail = onFullScreenAdFail__BackingField;
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail2;
				do
				{
					tapjoyGetFullScreenAdFail2 = tapjoyGetFullScreenAdFail;
					tapjoyGetFullScreenAdFail = Interlocked.CompareExchange(ref onFullScreenAdFail__BackingField, (TapjoyGetFullScreenAdFail)Delegate.Remove(tapjoyGetFullScreenAdFail2, value), tapjoyGetFullScreenAdFail);
				}
				while ((object)tapjoyGetFullScreenAdFail != tapjoyGetFullScreenAdFail2);
			}
		}

		public static event TapjoyEarnedPointsEvent onEarnPoints
		{
			add
			{
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent = onEarnPoints__BackingField;
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent2;
				do
				{
					tapjoyEarnedPointsEvent2 = tapjoyEarnedPointsEvent;
					tapjoyEarnedPointsEvent = Interlocked.CompareExchange(ref onEarnPoints__BackingField, (TapjoyEarnedPointsEvent)Delegate.Combine(tapjoyEarnedPointsEvent2, value), tapjoyEarnedPointsEvent);
				}
				while ((object)tapjoyEarnedPointsEvent != tapjoyEarnedPointsEvent2);
			}
			remove
			{
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent = onEarnPoints__BackingField;
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent2;
				do
				{
					tapjoyEarnedPointsEvent2 = tapjoyEarnedPointsEvent;
					tapjoyEarnedPointsEvent = Interlocked.CompareExchange(ref onEarnPoints__BackingField, (TapjoyEarnedPointsEvent)Delegate.Remove(tapjoyEarnedPointsEvent2, value), tapjoyEarnedPointsEvent);
				}
				while ((object)tapjoyEarnedPointsEvent != tapjoyEarnedPointsEvent2);
			}
		}

		public static event TapjoyGetPointsEvent onGetPoints
		{
			add
			{
				TapjoyGetPointsEvent tapjoyGetPointsEvent = onGetPoints__BackingField;
				TapjoyGetPointsEvent tapjoyGetPointsEvent2;
				do
				{
					tapjoyGetPointsEvent2 = tapjoyGetPointsEvent;
					tapjoyGetPointsEvent = Interlocked.CompareExchange(ref onGetPoints__BackingField, (TapjoyGetPointsEvent)Delegate.Combine(tapjoyGetPointsEvent2, value), tapjoyGetPointsEvent);
				}
				while ((object)tapjoyGetPointsEvent != tapjoyGetPointsEvent2);
			}
			remove
			{
				TapjoyGetPointsEvent tapjoyGetPointsEvent = onGetPoints__BackingField;
				TapjoyGetPointsEvent tapjoyGetPointsEvent2;
				do
				{
					tapjoyGetPointsEvent2 = tapjoyGetPointsEvent;
					tapjoyGetPointsEvent = Interlocked.CompareExchange(ref onGetPoints__BackingField, (TapjoyGetPointsEvent)Delegate.Remove(tapjoyGetPointsEvent2, value), tapjoyGetPointsEvent);
				}
				while ((object)tapjoyGetPointsEvent != tapjoyGetPointsEvent2);
			}
		}

		public static event TapjoyGetPointsErrorEvent onGetPointsError
		{
			add
			{
				TapjoyGetPointsErrorEvent tapjoyGetPointsErrorEvent = onGetPointsError__BackingField;
				TapjoyGetPointsErrorEvent tapjoyGetPointsErrorEvent2;
				do
				{
					tapjoyGetPointsErrorEvent2 = tapjoyGetPointsErrorEvent;
					tapjoyGetPointsErrorEvent = Interlocked.CompareExchange(ref onGetPointsError__BackingField, (TapjoyGetPointsErrorEvent)Delegate.Combine(tapjoyGetPointsErrorEvent2, value), tapjoyGetPointsErrorEvent);
				}
				while ((object)tapjoyGetPointsErrorEvent != tapjoyGetPointsErrorEvent2);
			}
			remove
			{
				TapjoyGetPointsErrorEvent tapjoyGetPointsErrorEvent = onGetPointsError__BackingField;
				TapjoyGetPointsErrorEvent tapjoyGetPointsErrorEvent2;
				do
				{
					tapjoyGetPointsErrorEvent2 = tapjoyGetPointsErrorEvent;
					tapjoyGetPointsErrorEvent = Interlocked.CompareExchange(ref onGetPointsError__BackingField, (TapjoyGetPointsErrorEvent)Delegate.Remove(tapjoyGetPointsErrorEvent2, value), tapjoyGetPointsErrorEvent);
				}
				while ((object)tapjoyGetPointsErrorEvent != tapjoyGetPointsErrorEvent2);
			}
		}

		public static event TapjoySpendPointsEvent onSpendPoints
		{
			add
			{
				TapjoySpendPointsEvent tapjoySpendPointsEvent = onSpendPoints__BackingField;
				TapjoySpendPointsEvent tapjoySpendPointsEvent2;
				do
				{
					tapjoySpendPointsEvent2 = tapjoySpendPointsEvent;
					tapjoySpendPointsEvent = Interlocked.CompareExchange(ref onSpendPoints__BackingField, (TapjoySpendPointsEvent)Delegate.Combine(tapjoySpendPointsEvent2, value), tapjoySpendPointsEvent);
				}
				while ((object)tapjoySpendPointsEvent != tapjoySpendPointsEvent2);
			}
			remove
			{
				TapjoySpendPointsEvent tapjoySpendPointsEvent = onSpendPoints__BackingField;
				TapjoySpendPointsEvent tapjoySpendPointsEvent2;
				do
				{
					tapjoySpendPointsEvent2 = tapjoySpendPointsEvent;
					tapjoySpendPointsEvent = Interlocked.CompareExchange(ref onSpendPoints__BackingField, (TapjoySpendPointsEvent)Delegate.Remove(tapjoySpendPointsEvent2, value), tapjoySpendPointsEvent);
				}
				while ((object)tapjoySpendPointsEvent != tapjoySpendPointsEvent2);
			}
		}

		public static event TapjoySpendPointsErrorEvent onSpendPointsError
		{
			add
			{
				TapjoySpendPointsErrorEvent tapjoySpendPointsErrorEvent = onSpendPointsError__BackingField;
				TapjoySpendPointsErrorEvent tapjoySpendPointsErrorEvent2;
				do
				{
					tapjoySpendPointsErrorEvent2 = tapjoySpendPointsErrorEvent;
					tapjoySpendPointsErrorEvent = Interlocked.CompareExchange(ref onSpendPointsError__BackingField, (TapjoySpendPointsErrorEvent)Delegate.Combine(tapjoySpendPointsErrorEvent2, value), tapjoySpendPointsErrorEvent);
				}
				while ((object)tapjoySpendPointsErrorEvent != tapjoySpendPointsErrorEvent2);
			}
			remove
			{
				TapjoySpendPointsErrorEvent tapjoySpendPointsErrorEvent = onSpendPointsError__BackingField;
				TapjoySpendPointsErrorEvent tapjoySpendPointsErrorEvent2;
				do
				{
					tapjoySpendPointsErrorEvent2 = tapjoySpendPointsErrorEvent;
					tapjoySpendPointsErrorEvent = Interlocked.CompareExchange(ref onSpendPointsError__BackingField, (TapjoySpendPointsErrorEvent)Delegate.Remove(tapjoySpendPointsErrorEvent2, value), tapjoySpendPointsErrorEvent);
				}
				while ((object)tapjoySpendPointsErrorEvent != tapjoySpendPointsErrorEvent2);
			}
		}

		public static event TapjoyViewWillAppearEvent onViewWillAppear
		{
			add
			{
				TapjoyViewWillAppearEvent tapjoyViewWillAppearEvent = onViewWillAppear__BackingField;
				TapjoyViewWillAppearEvent tapjoyViewWillAppearEvent2;
				do
				{
					tapjoyViewWillAppearEvent2 = tapjoyViewWillAppearEvent;
					tapjoyViewWillAppearEvent = Interlocked.CompareExchange(ref onViewWillAppear__BackingField, (TapjoyViewWillAppearEvent)Delegate.Combine(tapjoyViewWillAppearEvent2, value), tapjoyViewWillAppearEvent);
				}
				while ((object)tapjoyViewWillAppearEvent != tapjoyViewWillAppearEvent2);
			}
			remove
			{
				TapjoyViewWillAppearEvent tapjoyViewWillAppearEvent = onViewWillAppear__BackingField;
				TapjoyViewWillAppearEvent tapjoyViewWillAppearEvent2;
				do
				{
					tapjoyViewWillAppearEvent2 = tapjoyViewWillAppearEvent;
					tapjoyViewWillAppearEvent = Interlocked.CompareExchange(ref onViewWillAppear__BackingField, (TapjoyViewWillAppearEvent)Delegate.Remove(tapjoyViewWillAppearEvent2, value), tapjoyViewWillAppearEvent);
				}
				while ((object)tapjoyViewWillAppearEvent != tapjoyViewWillAppearEvent2);
			}
		}

		public static event TapjoyViewDidAppearEvent onViewDidAppear
		{
			add
			{
				TapjoyViewDidAppearEvent tapjoyViewDidAppearEvent = onViewDidAppear__BackingField;
				TapjoyViewDidAppearEvent tapjoyViewDidAppearEvent2;
				do
				{
					tapjoyViewDidAppearEvent2 = tapjoyViewDidAppearEvent;
					tapjoyViewDidAppearEvent = Interlocked.CompareExchange(ref onViewDidAppear__BackingField, (TapjoyViewDidAppearEvent)Delegate.Combine(tapjoyViewDidAppearEvent2, value), tapjoyViewDidAppearEvent);
				}
				while ((object)tapjoyViewDidAppearEvent != tapjoyViewDidAppearEvent2);
			}
			remove
			{
				TapjoyViewDidAppearEvent tapjoyViewDidAppearEvent = onViewDidAppear__BackingField;
				TapjoyViewDidAppearEvent tapjoyViewDidAppearEvent2;
				do
				{
					tapjoyViewDidAppearEvent2 = tapjoyViewDidAppearEvent;
					tapjoyViewDidAppearEvent = Interlocked.CompareExchange(ref onViewDidAppear__BackingField, (TapjoyViewDidAppearEvent)Delegate.Remove(tapjoyViewDidAppearEvent2, value), tapjoyViewDidAppearEvent);
				}
				while ((object)tapjoyViewDidAppearEvent != tapjoyViewDidAppearEvent2);
			}
		}

		public static event TapjoyViewWillDisappearEvent onViewWillDisappear
		{
			add
			{
				TapjoyViewWillDisappearEvent tapjoyViewWillDisappearEvent = onViewWillDisappear__BackingField;
				TapjoyViewWillDisappearEvent tapjoyViewWillDisappearEvent2;
				do
				{
					tapjoyViewWillDisappearEvent2 = tapjoyViewWillDisappearEvent;
					tapjoyViewWillDisappearEvent = Interlocked.CompareExchange(ref onViewWillDisappear__BackingField, (TapjoyViewWillDisappearEvent)Delegate.Combine(tapjoyViewWillDisappearEvent2, value), tapjoyViewWillDisappearEvent);
				}
				while ((object)tapjoyViewWillDisappearEvent != tapjoyViewWillDisappearEvent2);
			}
			remove
			{
				TapjoyViewWillDisappearEvent tapjoyViewWillDisappearEvent = onViewWillDisappear__BackingField;
				TapjoyViewWillDisappearEvent tapjoyViewWillDisappearEvent2;
				do
				{
					tapjoyViewWillDisappearEvent2 = tapjoyViewWillDisappearEvent;
					tapjoyViewWillDisappearEvent = Interlocked.CompareExchange(ref onViewWillDisappear__BackingField, (TapjoyViewWillDisappearEvent)Delegate.Remove(tapjoyViewWillDisappearEvent2, value), tapjoyViewWillDisappearEvent);
				}
				while ((object)tapjoyViewWillDisappearEvent != tapjoyViewWillDisappearEvent2);
			}
		}

		public static event TapjoyViewDidDisappearEvent onViewDidDisappear
		{
			add
			{
				TapjoyViewDidDisappearEvent tapjoyViewDidDisappearEvent = onViewDidDisappear__BackingField;
				TapjoyViewDidDisappearEvent tapjoyViewDidDisappearEvent2;
				do
				{
					tapjoyViewDidDisappearEvent2 = tapjoyViewDidDisappearEvent;
					tapjoyViewDidDisappearEvent = Interlocked.CompareExchange(ref onViewDidDisappear__BackingField, (TapjoyViewDidDisappearEvent)Delegate.Combine(tapjoyViewDidDisappearEvent2, value), tapjoyViewDidDisappearEvent);
				}
				while ((object)tapjoyViewDidDisappearEvent != tapjoyViewDidDisappearEvent2);
			}
			remove
			{
				TapjoyViewDidDisappearEvent tapjoyViewDidDisappearEvent = onViewDidDisappear__BackingField;
				TapjoyViewDidDisappearEvent tapjoyViewDidDisappearEvent2;
				do
				{
					tapjoyViewDidDisappearEvent2 = tapjoyViewDidDisappearEvent;
					tapjoyViewDidDisappearEvent = Interlocked.CompareExchange(ref onViewDidDisappear__BackingField, (TapjoyViewDidDisappearEvent)Delegate.Remove(tapjoyViewDidDisappearEvent2, value), tapjoyViewDidDisappearEvent);
				}
				while ((object)tapjoyViewDidDisappearEvent != tapjoyViewDidDisappearEvent2);
			}
		}

		public static event TapjoyVideoStartEvent onVideoStart
		{
			add
			{
				TapjoyVideoStartEvent tapjoyVideoStartEvent = onVideoStart__BackingField;
				TapjoyVideoStartEvent tapjoyVideoStartEvent2;
				do
				{
					tapjoyVideoStartEvent2 = tapjoyVideoStartEvent;
					tapjoyVideoStartEvent = Interlocked.CompareExchange(ref onVideoStart__BackingField, (TapjoyVideoStartEvent)Delegate.Combine(tapjoyVideoStartEvent2, value), tapjoyVideoStartEvent);
				}
				while ((object)tapjoyVideoStartEvent != tapjoyVideoStartEvent2);
			}
			remove
			{
				TapjoyVideoStartEvent tapjoyVideoStartEvent = onVideoStart__BackingField;
				TapjoyVideoStartEvent tapjoyVideoStartEvent2;
				do
				{
					tapjoyVideoStartEvent2 = tapjoyVideoStartEvent;
					tapjoyVideoStartEvent = Interlocked.CompareExchange(ref onVideoStart__BackingField, (TapjoyVideoStartEvent)Delegate.Remove(tapjoyVideoStartEvent2, value), tapjoyVideoStartEvent);
				}
				while ((object)tapjoyVideoStartEvent != tapjoyVideoStartEvent2);
			}
		}

		public static event TapjoyVideoErrorEvent onVideoError
		{
			add
			{
				TapjoyVideoErrorEvent tapjoyVideoErrorEvent = onVideoError__BackingField;
				TapjoyVideoErrorEvent tapjoyVideoErrorEvent2;
				do
				{
					tapjoyVideoErrorEvent2 = tapjoyVideoErrorEvent;
					tapjoyVideoErrorEvent = Interlocked.CompareExchange(ref onVideoError__BackingField, (TapjoyVideoErrorEvent)Delegate.Combine(tapjoyVideoErrorEvent2, value), tapjoyVideoErrorEvent);
				}
				while ((object)tapjoyVideoErrorEvent != tapjoyVideoErrorEvent2);
			}
			remove
			{
				TapjoyVideoErrorEvent tapjoyVideoErrorEvent = onVideoError__BackingField;
				TapjoyVideoErrorEvent tapjoyVideoErrorEvent2;
				do
				{
					tapjoyVideoErrorEvent2 = tapjoyVideoErrorEvent;
					tapjoyVideoErrorEvent = Interlocked.CompareExchange(ref onVideoError__BackingField, (TapjoyVideoErrorEvent)Delegate.Remove(tapjoyVideoErrorEvent2, value), tapjoyVideoErrorEvent);
				}
				while ((object)tapjoyVideoErrorEvent != tapjoyVideoErrorEvent2);
			}
		}

		public static event TapjoyVideoCompleteEvent onVideoComplete
		{
			add
			{
				TapjoyVideoCompleteEvent tapjoyVideoCompleteEvent = onVideoComplete__BackingField;
				TapjoyVideoCompleteEvent tapjoyVideoCompleteEvent2;
				do
				{
					tapjoyVideoCompleteEvent2 = tapjoyVideoCompleteEvent;
					tapjoyVideoCompleteEvent = Interlocked.CompareExchange(ref onVideoComplete__BackingField, (TapjoyVideoCompleteEvent)Delegate.Combine(tapjoyVideoCompleteEvent2, value), tapjoyVideoCompleteEvent);
				}
				while ((object)tapjoyVideoCompleteEvent != tapjoyVideoCompleteEvent2);
			}
			remove
			{
				TapjoyVideoCompleteEvent tapjoyVideoCompleteEvent = onVideoComplete__BackingField;
				TapjoyVideoCompleteEvent tapjoyVideoCompleteEvent2;
				do
				{
					tapjoyVideoCompleteEvent2 = tapjoyVideoCompleteEvent;
					tapjoyVideoCompleteEvent = Interlocked.CompareExchange(ref onVideoComplete__BackingField, (TapjoyVideoCompleteEvent)Delegate.Remove(tapjoyVideoCompleteEvent2, value), tapjoyVideoCompleteEvent);
				}
				while ((object)tapjoyVideoCompleteEvent != tapjoyVideoCompleteEvent2);
			}
		}

		public static event TapjoyGetOfferWallResponseEvent onOfferWallResponse
		{
			add
			{
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent = onOfferWallResponse__BackingField;
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent2;
				do
				{
					tapjoyGetOfferWallResponseEvent2 = tapjoyGetOfferWallResponseEvent;
					tapjoyGetOfferWallResponseEvent = Interlocked.CompareExchange(ref onOfferWallResponse__BackingField, (TapjoyGetOfferWallResponseEvent)Delegate.Combine(tapjoyGetOfferWallResponseEvent2, value), tapjoyGetOfferWallResponseEvent);
				}
				while ((object)tapjoyGetOfferWallResponseEvent != tapjoyGetOfferWallResponseEvent2);
			}
			remove
			{
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent = onOfferWallResponse__BackingField;
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent2;
				do
				{
					tapjoyGetOfferWallResponseEvent2 = tapjoyGetOfferWallResponseEvent;
					tapjoyGetOfferWallResponseEvent = Interlocked.CompareExchange(ref onOfferWallResponse__BackingField, (TapjoyGetOfferWallResponseEvent)Delegate.Remove(tapjoyGetOfferWallResponseEvent2, value), tapjoyGetOfferWallResponseEvent);
				}
				while ((object)tapjoyGetOfferWallResponseEvent != tapjoyGetOfferWallResponseEvent2);
			}
		}

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				return true;
			case RuntimePlatform.Android:
				return !DeviceUtility.isKindleDevice;
			case RuntimePlatform.WindowsEditor:
				return false;
			case RuntimePlatform.OSXEditor:
				return false;
			default:
				return false;
			}
		}

		public static void Initialize(string appId, string appKey)
		{
			if (_connectionState == TapJoyConnectionState.Connecting || isInitialized)
			{
				LogError("TapJoy has already been configured. Ignoring new parameters.");
				return;
			}
			_connectionState = TapJoyConnectionState.Connecting;
			if (_platformBridge == null)
			{
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
					_platformBridge = TapjoyIOS.CreatePlugin();
					break;
				case RuntimePlatform.Android:
					_platformBridge = TapjoyAndroid.CreatePlugin();
					break;
				default:
					LogError("There's no platform bridge for the current platform: " + Application.platform);
					return;
				}
			}
			if (!_listenersAdded)
			{
				_platformBridge.onConnect += OnInitializeFinished;
				_platformBridge.onGetOfferWallResponse += OnOfferWallResponse;
				_platformBridge.onEarnedPoints += OnEarnPoints;
				_platformBridge.onGetFullScreenAdSuccess += OnFullScreenAdSuccess;
				_platformBridge.onGetFullScreenAdFail += OnFullScreenAdFail;
				_platformBridge.onGetPoints += OnGetPoints;
				_platformBridge.onGetPointsError += OnGetPointsError;
				_platformBridge.onSpendPoints += OnSpendPoints;
				_platformBridge.onSpendPointsError += OnSpendPointsError;
				_platformBridge.onViewWillDisappear += OnViewWillDisappear;
				_platformBridge.onViewDidDisappear += OnViewDidDisappear;
				_platformBridge.onViewWillAppear += OnViewWillAppear;
				_platformBridge.onViewDidAppear += OnViewDidAppear;
				_platformBridge.onVideoStart += OnVideoStart;
				_platformBridge.onVideoError += OnVideoError;
				_platformBridge.onVideoComplete += OnVideoComplete;
				_listenersAdded = true;
			}
			_platformBridge.Initialize(appId, appKey);
			_platformBridge.SetGdprConsent(!AgeRestriction.isRestricted);
		}

		public static void GetFullScreenAd(string placement)
		{
			if (isInitialized)
			{
				_platformBridge.GetFullScreenAd(placement);
			}
		}

		public static void ShowFullScreenAd(string placement)
		{
			if (isInitialized)
			{
				_platformBridge.ShowFullScreenAd(placement);
			}
		}

		public static void GetOfferWall(string eventname)
		{
			if (isInitialized)
			{
				_platformBridge.GetOfferWall(eventname);
			}
		}

		public static void ShowOfferWall()
		{
			if (isInitialized)
			{
				_platformBridge.ShowOfferWall();
			}
		}

		public static bool IsOfferWallReady()
		{
			if (!isInitialized)
			{
				return false;
			}
			return _platformBridge.IsOfferWallReady();
		}

		public static void GetPoints()
		{
			if (isInitialized)
			{
				_platformBridge.GetPoints();
			}
		}

		public static void SpendPoints(int amount)
		{
			if (isInitialized)
			{
				_platformBridge.SpendPoints(amount);
			}
		}

		private static void OnInitializeFinished(string result)
		{
			if (result == "success")
			{
				_connectionState = TapJoyConnectionState.Connected;
			}
			else
			{
				_connectionState = TapJoyConnectionState.Disconnected;
			}
			if (onInitializeFinished__BackingField != null)
			{
				onInitializeFinished__BackingField(result);
			}
		}

		private static void OnFullScreenAdSuccess(string placement)
		{
			if (onFullScreenAdSuccess__BackingField != null)
			{
				onFullScreenAdSuccess__BackingField(placement);
			}
		}

		private static void OnFullScreenAdFail(string placement)
		{
			if (onFullScreenAdFail__BackingField != null)
			{
				onFullScreenAdFail__BackingField(placement);
			}
		}

		private static void OnOfferWallResponse(string result)
		{
			if (onOfferWallResponse__BackingField != null)
			{
				onOfferWallResponse__BackingField(result);
			}
		}

		private static void OnEarnPoints(string amount)
		{
			if (onEarnPoints__BackingField != null)
			{
				onEarnPoints__BackingField(amount);
			}
		}

		private static void OnGetPoints(string amount)
		{
			if (onGetPoints__BackingField != null)
			{
				onGetPoints__BackingField(amount);
			}
		}

		private static void OnGetPointsError()
		{
			if (onGetPointsError__BackingField != null)
			{
				onGetPointsError__BackingField();
			}
		}

		private static void OnSpendPoints(string amount)
		{
			if (onSpendPoints__BackingField != null)
			{
				onSpendPoints__BackingField(amount);
			}
		}

		private static void OnSpendPointsError()
		{
			if (onSpendPointsError__BackingField != null)
			{
				onSpendPointsError__BackingField();
			}
		}

		private static void OnViewWillAppear(string viewType)
		{
			if (onViewWillAppear__BackingField != null)
			{
				onViewWillAppear__BackingField(viewType);
			}
		}

		private static void OnViewDidAppear(string viewType)
		{
			if (onViewDidAppear__BackingField != null)
			{
				onViewDidAppear__BackingField(viewType);
			}
		}

		private static void OnViewWillDisappear(string viewType)
		{
			if (onViewWillDisappear__BackingField != null)
			{
				onViewWillDisappear__BackingField(viewType);
			}
		}

		private static void OnViewDidDisappear(string viewType)
		{
			if (onViewDidDisappear__BackingField != null)
			{
				onViewDidDisappear__BackingField(viewType);
			}
		}

		private static void OnVideoStart()
		{
			if (onVideoStart__BackingField != null)
			{
				onVideoStart__BackingField();
			}
		}

		private static void OnVideoError(string error)
		{
			if (onVideoError__BackingField != null)
			{
				onVideoError__BackingField(error);
			}
		}

		private static void OnVideoComplete()
		{
			if (onVideoComplete__BackingField != null)
			{
				onVideoComplete__BackingField();
			}
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("TapJoy: " + msg, context);
		}

		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_TAPJOY_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
