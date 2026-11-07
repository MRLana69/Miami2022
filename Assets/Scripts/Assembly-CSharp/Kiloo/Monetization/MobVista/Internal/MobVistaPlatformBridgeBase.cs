using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Kiloo.Monetization.MobVista.Internal
{
	public abstract class MobVistaPlatformBridgeBase : MonoBehaviour, IMobVistaPlatformBridge
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoShowEvent onVideoShow__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdShowFailedEvent onVideoAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdCloseEvent onVideoAdClose__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdWatchedEvent onVideoAdWatched__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoLoadFailedEvent onVideoLoadFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoLoaded onVideoLoaded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdClicked onVideoAdClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoShowEvent onInterstitialShow__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdShowFailedEvent onInterstitialAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdCloseEvent onInterstitialAdClose__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdWatchedEvent onInterstitialAdWatched__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoLoadFailedEvent onInterstitialLoadFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoLoaded onInterstitialLoaded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MobVistaVideoAdClicked onInterstitialAdClicked__BackingField;

		protected abstract string platform { get; }

		public event MobVistaVideoShowEvent onVideoShow
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

		public event MobVistaVideoAdShowFailedEvent onVideoAdShowFailed
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

		public event MobVistaVideoAdCloseEvent onVideoAdClose
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

		public event MobVistaVideoAdWatchedEvent onVideoAdWatched
		{
			add
			{
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent = onVideoAdWatched__BackingField;
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent2;
				do
				{
					mobVistaVideoAdWatchedEvent2 = mobVistaVideoAdWatchedEvent;
					mobVistaVideoAdWatchedEvent = Interlocked.CompareExchange(ref onVideoAdWatched__BackingField, (MobVistaVideoAdWatchedEvent)Delegate.Combine(mobVistaVideoAdWatchedEvent2, value), mobVistaVideoAdWatchedEvent);
				}
				while ((object)mobVistaVideoAdWatchedEvent != mobVistaVideoAdWatchedEvent2);
			}
			remove
			{
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent = onVideoAdWatched__BackingField;
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent2;
				do
				{
					mobVistaVideoAdWatchedEvent2 = mobVistaVideoAdWatchedEvent;
					mobVistaVideoAdWatchedEvent = Interlocked.CompareExchange(ref onVideoAdWatched__BackingField, (MobVistaVideoAdWatchedEvent)Delegate.Remove(mobVistaVideoAdWatchedEvent2, value), mobVistaVideoAdWatchedEvent);
				}
				while ((object)mobVistaVideoAdWatchedEvent != mobVistaVideoAdWatchedEvent2);
			}
		}

		public event MobVistaVideoLoadFailedEvent onVideoLoadFailed
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

		public event MobVistaVideoLoaded onVideoLoaded
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

		public event MobVistaVideoAdClicked onVideoAdClicked
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

		public event MobVistaVideoShowEvent onInterstitialShow
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

		public event MobVistaVideoAdShowFailedEvent onInterstitialAdShowFailed
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

		public event MobVistaVideoAdCloseEvent onInterstitialAdClose
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

		public event MobVistaVideoAdWatchedEvent onInterstitialAdWatched
		{
			add
			{
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent = onInterstitialAdWatched__BackingField;
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent2;
				do
				{
					mobVistaVideoAdWatchedEvent2 = mobVistaVideoAdWatchedEvent;
					mobVistaVideoAdWatchedEvent = Interlocked.CompareExchange(ref onInterstitialAdWatched__BackingField, (MobVistaVideoAdWatchedEvent)Delegate.Combine(mobVistaVideoAdWatchedEvent2, value), mobVistaVideoAdWatchedEvent);
				}
				while ((object)mobVistaVideoAdWatchedEvent != mobVistaVideoAdWatchedEvent2);
			}
			remove
			{
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent = onInterstitialAdWatched__BackingField;
				MobVistaVideoAdWatchedEvent mobVistaVideoAdWatchedEvent2;
				do
				{
					mobVistaVideoAdWatchedEvent2 = mobVistaVideoAdWatchedEvent;
					mobVistaVideoAdWatchedEvent = Interlocked.CompareExchange(ref onInterstitialAdWatched__BackingField, (MobVistaVideoAdWatchedEvent)Delegate.Remove(mobVistaVideoAdWatchedEvent2, value), mobVistaVideoAdWatchedEvent);
				}
				while ((object)mobVistaVideoAdWatchedEvent != mobVistaVideoAdWatchedEvent2);
			}
		}

		public event MobVistaVideoLoadFailedEvent onInterstitialLoadFailed
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

		public event MobVistaVideoLoaded onInterstitialLoaded
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

		public event MobVistaVideoAdClicked onInterstitialAdClicked
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

		public abstract void Initialize(string appId, string sdkKey, bool consent);

		public abstract bool IsFullScreenAdLoaded(string placementId);

		public abstract void LoadFullScreenAd(string placementId);

		public abstract void ShowFullScreenAd(string placementId);

		public abstract bool IsInterstitialAdLoaded(string placementId);

		public abstract void LoadInterstitialAd(string placementId);

		public abstract void ShowInterstitialAd(string placementId);

		public abstract void EnableDebugLogs();

		public abstract void DisableDebugLogs();

		public void CallBackOnVideoAdShow(string placement)
		{
			if (onVideoShow__BackingField != null)
			{
				onVideoShow__BackingField(placement);
			}
		}

		public void CallBackOnVideoAdClose(string placement)
		{
			if (onVideoAdClose__BackingField != null)
			{
				onVideoAdClose__BackingField(placement);
			}
		}

		public void CallBackOnVideoAdWatched(string placement)
		{
			if (onVideoAdWatched__BackingField != null)
			{
				onVideoAdWatched__BackingField(placement);
			}
		}

		public void CallBackOnVideoAdShowFailed(string placement)
		{
			if (onVideoAdShowFailed__BackingField != null)
			{
				onVideoAdShowFailed__BackingField(placement);
			}
		}

		public void CallBackOnVideoLoadFailed(string placement)
		{
			if (onVideoLoadFailed__BackingField != null)
			{
				onVideoLoadFailed__BackingField(placement);
			}
		}

		public void CallBackOnVideoLoaded(string cacheID)
		{
			if (onVideoLoaded__BackingField != null)
			{
				onVideoLoaded__BackingField(cacheID);
			}
		}

		public void CallBackOnVideoAdClicked(string placement)
		{
			if (onVideoAdClicked__BackingField != null)
			{
				onVideoAdClicked__BackingField(placement);
			}
		}

		public void CallBackOnInterstitialAdShow(string placement)
		{
			if (onInterstitialShow__BackingField != null)
			{
				onInterstitialShow__BackingField(placement);
			}
		}

		public void CallBackOnInterstitialAdClose(string placement)
		{
			if (onInterstitialAdClose__BackingField != null)
			{
				onInterstitialAdClose__BackingField(placement);
			}
		}

		public void CallBackOnInterstitialAdWatched(string placement)
		{
			if (onInterstitialAdWatched__BackingField != null)
			{
				onInterstitialAdWatched__BackingField(placement);
			}
		}

		public void CallBackOnInterstitialAdShowFailed(string placement)
		{
			if (onInterstitialAdShowFailed__BackingField != null)
			{
				onInterstitialAdShowFailed__BackingField(placement);
			}
		}

		public void CallBackOnInterstitialLoadFailed(string placement)
		{
			if (onInterstitialLoadFailed__BackingField != null)
			{
				onInterstitialLoadFailed__BackingField(placement);
			}
		}

		public void CallBackOnInterstitialLoaded(string cacheID)
		{
			if (onInterstitialLoaded__BackingField != null)
			{
				onInterstitialLoaded__BackingField(cacheID);
			}
		}

		public void CallBackOnInterstitialAdClicked(string cacheID)
		{
			if (onInterstitialAdClicked__BackingField != null)
			{
				onInterstitialAdClicked__BackingField(cacheID);
			}
		}

		public void CallBackOnEndcardShow(string id)
		{
		}

		public void CallBackOnVideoComplete(string id)
		{
		}

		protected void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("Mobvista" + platform + ": " + msg, context);
		}

		protected void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_MOBVISTA_LOGS")]
		protected void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
