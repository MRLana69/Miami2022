using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Kiloo.Monetization.TapjoyPlugin.Internal
{
	public class TapjoyAndroid : MonoBehaviour, ITapjoyBridge
	{
		private const string SET_GDPR_CONSENT_METHOD_NAME = "setGdprConsent";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyConnectEvent onConnect__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyGetFullScreenAdSuccess onGetFullScreenAdSuccess__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyGetFullScreenAdFail onGetFullScreenAdFail__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyViewDidAppearEvent onViewDidAppear__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyViewDidDisappearEvent onViewDidDisappear__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyVideoStartEvent onVideoStart__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyVideoErrorEvent onVideoError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyVideoCompleteEvent onVideoComplete__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyEarnedPointsEvent onEarnedPoints__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyGetPointsEvent onGetPoints__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyGetPointsErrorEvent onGetPointsError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoySpendPointsEvent onSpendPoints__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoySpendPointsErrorEvent onSpendPointsError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyGetOfferWallResponseEvent onGetOfferWallResponse__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyViewWillAppearEvent onViewWillAppear__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TapjoyViewWillDisappearEvent onViewWillDisappear__BackingField;

		private AndroidJavaObject _androidBridge;

		private const string CALLBACK_RECEIVER_NAME = "TapjoyAndroidReceiver";

		public event TapjoyConnectEvent onConnect
		{
			add
			{
				TapjoyConnectEvent tapjoyConnectEvent = onConnect__BackingField;
				TapjoyConnectEvent tapjoyConnectEvent2;
				do
				{
					tapjoyConnectEvent2 = tapjoyConnectEvent;
					tapjoyConnectEvent = Interlocked.CompareExchange(ref onConnect__BackingField, (TapjoyConnectEvent)Delegate.Combine(tapjoyConnectEvent2, value), tapjoyConnectEvent);
				}
				while ((object)tapjoyConnectEvent != tapjoyConnectEvent2);
			}
			remove
			{
				TapjoyConnectEvent tapjoyConnectEvent = onConnect__BackingField;
				TapjoyConnectEvent tapjoyConnectEvent2;
				do
				{
					tapjoyConnectEvent2 = tapjoyConnectEvent;
					tapjoyConnectEvent = Interlocked.CompareExchange(ref onConnect__BackingField, (TapjoyConnectEvent)Delegate.Remove(tapjoyConnectEvent2, value), tapjoyConnectEvent);
				}
				while ((object)tapjoyConnectEvent != tapjoyConnectEvent2);
			}
		}

		public event TapjoyGetFullScreenAdSuccess onGetFullScreenAdSuccess
		{
			add
			{
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess = onGetFullScreenAdSuccess__BackingField;
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess2;
				do
				{
					tapjoyGetFullScreenAdSuccess2 = tapjoyGetFullScreenAdSuccess;
					tapjoyGetFullScreenAdSuccess = Interlocked.CompareExchange(ref onGetFullScreenAdSuccess__BackingField, (TapjoyGetFullScreenAdSuccess)Delegate.Combine(tapjoyGetFullScreenAdSuccess2, value), tapjoyGetFullScreenAdSuccess);
				}
				while ((object)tapjoyGetFullScreenAdSuccess != tapjoyGetFullScreenAdSuccess2);
			}
			remove
			{
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess = onGetFullScreenAdSuccess__BackingField;
				TapjoyGetFullScreenAdSuccess tapjoyGetFullScreenAdSuccess2;
				do
				{
					tapjoyGetFullScreenAdSuccess2 = tapjoyGetFullScreenAdSuccess;
					tapjoyGetFullScreenAdSuccess = Interlocked.CompareExchange(ref onGetFullScreenAdSuccess__BackingField, (TapjoyGetFullScreenAdSuccess)Delegate.Remove(tapjoyGetFullScreenAdSuccess2, value), tapjoyGetFullScreenAdSuccess);
				}
				while ((object)tapjoyGetFullScreenAdSuccess != tapjoyGetFullScreenAdSuccess2);
			}
		}

		public event TapjoyGetFullScreenAdFail onGetFullScreenAdFail
		{
			add
			{
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail = onGetFullScreenAdFail__BackingField;
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail2;
				do
				{
					tapjoyGetFullScreenAdFail2 = tapjoyGetFullScreenAdFail;
					tapjoyGetFullScreenAdFail = Interlocked.CompareExchange(ref onGetFullScreenAdFail__BackingField, (TapjoyGetFullScreenAdFail)Delegate.Combine(tapjoyGetFullScreenAdFail2, value), tapjoyGetFullScreenAdFail);
				}
				while ((object)tapjoyGetFullScreenAdFail != tapjoyGetFullScreenAdFail2);
			}
			remove
			{
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail = onGetFullScreenAdFail__BackingField;
				TapjoyGetFullScreenAdFail tapjoyGetFullScreenAdFail2;
				do
				{
					tapjoyGetFullScreenAdFail2 = tapjoyGetFullScreenAdFail;
					tapjoyGetFullScreenAdFail = Interlocked.CompareExchange(ref onGetFullScreenAdFail__BackingField, (TapjoyGetFullScreenAdFail)Delegate.Remove(tapjoyGetFullScreenAdFail2, value), tapjoyGetFullScreenAdFail);
				}
				while ((object)tapjoyGetFullScreenAdFail != tapjoyGetFullScreenAdFail2);
			}
		}

		public event TapjoyViewDidAppearEvent onViewDidAppear
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

		public event TapjoyViewDidDisappearEvent onViewDidDisappear
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

		public event TapjoyVideoStartEvent onVideoStart
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

		public event TapjoyVideoErrorEvent onVideoError
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

		public event TapjoyVideoCompleteEvent onVideoComplete
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

		public event TapjoyEarnedPointsEvent onEarnedPoints
		{
			add
			{
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent = onEarnedPoints__BackingField;
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent2;
				do
				{
					tapjoyEarnedPointsEvent2 = tapjoyEarnedPointsEvent;
					tapjoyEarnedPointsEvent = Interlocked.CompareExchange(ref onEarnedPoints__BackingField, (TapjoyEarnedPointsEvent)Delegate.Combine(tapjoyEarnedPointsEvent2, value), tapjoyEarnedPointsEvent);
				}
				while ((object)tapjoyEarnedPointsEvent != tapjoyEarnedPointsEvent2);
			}
			remove
			{
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent = onEarnedPoints__BackingField;
				TapjoyEarnedPointsEvent tapjoyEarnedPointsEvent2;
				do
				{
					tapjoyEarnedPointsEvent2 = tapjoyEarnedPointsEvent;
					tapjoyEarnedPointsEvent = Interlocked.CompareExchange(ref onEarnedPoints__BackingField, (TapjoyEarnedPointsEvent)Delegate.Remove(tapjoyEarnedPointsEvent2, value), tapjoyEarnedPointsEvent);
				}
				while ((object)tapjoyEarnedPointsEvent != tapjoyEarnedPointsEvent2);
			}
		}

		public event TapjoyGetPointsEvent onGetPoints
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

		public event TapjoyGetPointsErrorEvent onGetPointsError
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

		public event TapjoySpendPointsEvent onSpendPoints
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

		public event TapjoySpendPointsErrorEvent onSpendPointsError
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

		public event TapjoyGetOfferWallResponseEvent onGetOfferWallResponse
		{
			add
			{
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent = onGetOfferWallResponse__BackingField;
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent2;
				do
				{
					tapjoyGetOfferWallResponseEvent2 = tapjoyGetOfferWallResponseEvent;
					tapjoyGetOfferWallResponseEvent = Interlocked.CompareExchange(ref onGetOfferWallResponse__BackingField, (TapjoyGetOfferWallResponseEvent)Delegate.Combine(tapjoyGetOfferWallResponseEvent2, value), tapjoyGetOfferWallResponseEvent);
				}
				while ((object)tapjoyGetOfferWallResponseEvent != tapjoyGetOfferWallResponseEvent2);
			}
			remove
			{
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent = onGetOfferWallResponse__BackingField;
				TapjoyGetOfferWallResponseEvent tapjoyGetOfferWallResponseEvent2;
				do
				{
					tapjoyGetOfferWallResponseEvent2 = tapjoyGetOfferWallResponseEvent;
					tapjoyGetOfferWallResponseEvent = Interlocked.CompareExchange(ref onGetOfferWallResponse__BackingField, (TapjoyGetOfferWallResponseEvent)Delegate.Remove(tapjoyGetOfferWallResponseEvent2, value), tapjoyGetOfferWallResponseEvent);
				}
				while ((object)tapjoyGetOfferWallResponseEvent != tapjoyGetOfferWallResponseEvent2);
			}
		}

		public event TapjoyViewWillAppearEvent onViewWillAppear
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

		public event TapjoyViewWillDisappearEvent onViewWillDisappear
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

		public void Initialize(string appID, string appKey)
		{
			if (_androidBridge == null)
			{
				_androidBridge = new AndroidJavaObject("com.tapjoy.TapjoyBridge.TapjoyBridge");
			}
			_androidBridge.Set("enableDebugLogs", false);
			_androidBridge.Set("disableAndroidIDAsAnalyticsID", true);
			_androidBridge.Call("Initialize", appID, appKey);
		}

		public void SetGdprConsent(bool consent)
		{
			_androidBridge.Call("setGdprConsent", consent);
		}

		public void GetFullScreenAd(string placement)
		{
			_androidBridge.Call("GetFullScreenAd", placement);
		}

		public void ShowFullScreenAd(string placement)
		{
			_androidBridge.Call("ShowFullScreenAd", placement);
		}

		public void GetOfferWall(string eventname)
		{
			_androidBridge.Call("RequestOfferWall", eventname);
		}

		public void ShowOfferWall()
		{
			_androidBridge.Call("ShowOfferWall");
		}

		public bool IsOfferWallReady()
		{
			return _androidBridge.Call<bool>("IsOfferWallAvailable", new object[0]);
		}

		public void GetPoints()
		{
			_androidBridge.Call("GetPoints");
		}

		public void SpendPoints(int amount)
		{
			_androidBridge.Call("SpendPoints", amount);
		}

		private void _callback_onConnect(string result)
		{
			if (onConnect__BackingField != null)
			{
				onConnect__BackingField(result);
			}
		}

		private void _callback_onGetOfferWallResponse(string result)
		{
			if (onGetOfferWallResponse__BackingField != null)
			{
				onGetOfferWallResponse__BackingField(result);
			}
		}

		private void _callback_onGetPoints(string amount)
		{
			if (onGetPoints__BackingField != null)
			{
				onGetPoints__BackingField(amount);
			}
		}

		private void _callback_onGetPointsError()
		{
			if (onGetPointsError__BackingField != null)
			{
				onGetPointsError__BackingField();
			}
		}

		private void _callback_onSpendPoints(string amount)
		{
			if (onSpendPoints__BackingField != null)
			{
				onSpendPoints__BackingField(amount);
			}
		}

		private void _callback_onSpendPointsError()
		{
			if (onSpendPointsError__BackingField != null)
			{
				onSpendPointsError__BackingField();
			}
		}

		private void _callback_onEarnedPoints(string amount)
		{
			if (onEarnedPoints__BackingField != null)
			{
				onEarnedPoints__BackingField(amount);
			}
		}

		private void _callback_onViewWillAppear(string viewType)
		{
			if (onViewWillAppear__BackingField != null)
			{
				onViewWillAppear__BackingField(viewType);
			}
		}

		private void _callback_onViewDidAppear(string viewType)
		{
			if (onViewDidAppear__BackingField != null)
			{
				onViewDidAppear__BackingField(viewType);
			}
		}

		private void _callback_onViewWillDisappear(string viewType)
		{
			if (onViewWillDisappear__BackingField != null)
			{
				onViewWillDisappear__BackingField(viewType);
			}
		}

		private void _callback_onViewDidDisappear(string viewType)
		{
			if (onViewDidDisappear__BackingField != null)
			{
				onViewDidDisappear__BackingField(viewType);
			}
		}

		private void _callback_onGetFullScreenAdSuccess(string placement)
		{
			if (onGetFullScreenAdSuccess__BackingField != null)
			{
				onGetFullScreenAdSuccess__BackingField(placement);
			}
		}

		private void _callback_onGetFullScreenAdFail(string placement)
		{
			if (onGetFullScreenAdFail__BackingField != null)
			{
				onGetFullScreenAdFail__BackingField(placement);
			}
		}

		private void _callback_onVideoStart()
		{
			if (onVideoStart__BackingField != null)
			{
				onVideoStart__BackingField();
			}
		}

		private void _callback_onVideoError(string error)
		{
			if (onVideoError__BackingField != null)
			{
				onVideoError__BackingField(error);
			}
		}

		private void _callback_onVideoComplete()
		{
			if (onVideoComplete__BackingField != null)
			{
				onVideoComplete__BackingField();
			}
		}

		public static TapjoyAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("TapjoyAndroidReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<TapjoyAndroid>();
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("TapjoyAndroid: " + msg, context);
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
