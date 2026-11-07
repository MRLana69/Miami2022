using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using UnityEngine;

namespace Kiloo.Monetization.AdBridgeCommon
{
	public class AdBridgeCallbackReceiver : MonoBehaviour
	{
		private bool _hasWatchedVideo;

		private AdBridgeLogger _adBridgeLogger;

		private AbstractAdProvider _adProvider;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdCallback onAdLoaded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdCallback onAdLoadFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdCallback onAdShowed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdCallback onAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdCallback onAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdCallback onAdWatched__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AdCallback onAdClicked__BackingField;

		public event AdCallback onAdLoaded
		{
			add
			{
				AdCallback adCallback = onAdLoaded__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdLoaded__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
			remove
			{
				AdCallback adCallback = onAdLoaded__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdLoaded__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
		}

		public event AdCallback onAdLoadFailed
		{
			add
			{
				AdCallback adCallback = onAdLoadFailed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdLoadFailed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
			remove
			{
				AdCallback adCallback = onAdLoadFailed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdLoadFailed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
		}

		public event AdCallback onAdShowed
		{
			add
			{
				AdCallback adCallback = onAdShowed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdShowed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
			remove
			{
				AdCallback adCallback = onAdShowed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdShowed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
		}

		public event AdCallback onAdShowFailed
		{
			add
			{
				AdCallback adCallback = onAdShowFailed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdShowFailed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
			remove
			{
				AdCallback adCallback = onAdShowFailed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdShowFailed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
		}

		public event AdCallback onAdClosed
		{
			add
			{
				AdCallback adCallback = onAdClosed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdClosed__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
			remove
			{
				AdCallback adCallback = onAdClosed__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdClosed__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
		}

		public event AdCallback onAdWatched
		{
			add
			{
				AdCallback adCallback = onAdWatched__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdWatched__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
			remove
			{
				AdCallback adCallback = onAdWatched__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdWatched__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
		}

		public event AdCallback onAdClicked
		{
			add
			{
				AdCallback adCallback = onAdClicked__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdClicked__BackingField, (AdCallback)Delegate.Combine(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
			remove
			{
				AdCallback adCallback = onAdClicked__BackingField;
				AdCallback adCallback2;
				do
				{
					adCallback2 = adCallback;
					adCallback = Interlocked.CompareExchange(ref onAdClicked__BackingField, (AdCallback)Delegate.Remove(adCallback2, value), adCallback);
				}
				while ((object)adCallback != adCallback2);
			}
		}

		protected AdBridgeCallbackReceiver()
		{
		}

		protected void Initialize(AdBridgeLogger adBridgeLogger, AbstractAdProvider adProvider)
		{
			_adBridgeLogger = adBridgeLogger;
			_adProvider = adProvider;
		}

		[UsedImplicitly]
		private void CallbackOnAdLoaded(string serializedMessage)
		{
			CallbackMessage callbackMessage = AdBridgeDeserializer.Deserialize<CallbackMessage>(serializedMessage);
			Log("CallbackOnAdLoaded " + callbackMessage.placementId);
			if (onAdLoaded__BackingField != null)
			{
				onAdLoaded__BackingField(callbackMessage.placementId, callbackMessage.adType);
			}
		}

		[UsedImplicitly]
		private void CallbackOnAdLoadFailed(string serializedMessage)
		{
			CallbackMessage callbackMessage = AdBridgeDeserializer.Deserialize<CallbackMessage>(serializedMessage);
			Log("CallbackOnAdLoadFailed " + callbackMessage.placementId);
			if (onAdLoadFailed__BackingField != null)
			{
				onAdLoadFailed__BackingField(callbackMessage.placementId, callbackMessage.adType);
			}
		}

		[UsedImplicitly]
		private void CallbackOnAdClicked(string serializedMessage)
		{
			CallbackMessage callbackMessage = AdBridgeDeserializer.Deserialize<CallbackMessage>(serializedMessage);
			Log("CallbackOnAdClicked " + callbackMessage.placementId);
			if (onAdClicked__BackingField != null)
			{
				onAdClicked__BackingField(callbackMessage.placementId, callbackMessage.adType);
			}
		}

		[UsedImplicitly]
		private void CallbackOnAdShowed(string serializedMessage)
		{
			CallbackMessage callbackMessage = AdBridgeDeserializer.Deserialize<CallbackMessage>(serializedMessage);
			Log("CallbackOnAdShowed " + callbackMessage.placementId);
			if (onAdShowed__BackingField != null)
			{
				onAdShowed__BackingField(callbackMessage.placementId, callbackMessage.adType);
			}
		}

		[UsedImplicitly]
		private void CallbackOnAdShowFailed(string serializedMessage)
		{
			CallbackMessage callbackMessage = AdBridgeDeserializer.Deserialize<CallbackMessage>(serializedMessage);
			Log("CallbackOnAdShowFailed " + callbackMessage.placementId);
			if (onAdShowFailed__BackingField != null)
			{
				onAdShowFailed__BackingField(callbackMessage.placementId, callbackMessage.adType);
			}
		}

		[UsedImplicitly]
		private void CallbackOnAdClosed(string serializedMessage)
		{
			CallbackMessage callbackMessage = AdBridgeDeserializer.Deserialize<CallbackMessage>(serializedMessage);
			Log("CallbackOnAdClosed " + callbackMessage.placementId);
			if (onAdClosed__BackingField != null)
			{
				onAdClosed__BackingField(callbackMessage.placementId, callbackMessage.adType);
			}
		}

		[UsedImplicitly]
		private void CallbackOnAdWatched(string serializedMessage)
		{
			CallbackMessage callbackMessage = AdBridgeDeserializer.Deserialize<CallbackMessage>(serializedMessage);
			Log("CallbackOnAdWatched " + callbackMessage.placementId);
			if (onAdWatched__BackingField != null)
			{
				onAdWatched__BackingField(callbackMessage.placementId, callbackMessage.adType);
			}
		}

		public void SubscribeAdListenerToAdBridgeEvents(IAdEventListener eventListener, string providerTier, Func<AdCacheSlotInfo> getAdCacheSlot, Func<AdLocationInfo> getAdLocation)
		{
			onAdLoaded += (string placementId, AdType adType) =>
			{
				eventListener.CachedAd(adType, _adProvider.id, providerTier, getAdCacheSlot(), false);
			};
			onAdLoadFailed += (string placementId, AdType adType) =>
			{
				eventListener.FailedToCacheAd(adType, _adProvider.id, providerTier, getAdCacheSlot());
			};
			onAdShowed += (string placementId, AdType adType) =>
			{
				eventListener.ShowedAd(adType, _adProvider.id, providerTier, getAdCacheSlot(), getAdLocation());
			};
			onAdClicked += (string placementId, AdType adType) =>
			{
				eventListener.ClickedAd(adType, _adProvider.id, providerTier, getAdCacheSlot(), getAdLocation());
			};
			onAdClosed += (string placementId, AdType adType) =>
			{
				OnAdClosed(adType, eventListener, providerTier, getAdCacheSlot, getAdLocation);
			};
			onAdWatched += (string placementId, AdType adType) =>
			{
				_hasWatchedVideo = true;
			};
		}

		private void OnAdClosed(AdType adType, IAdEventListener eventListener, string providerTier, Func<AdCacheSlotInfo> getAdCacheSlot, Func<AdLocationInfo> getAdLocation)
		{
			if (_hasWatchedVideo)
			{
				_hasWatchedVideo = false;
				eventListener.FinishedWatchingVideo(_adProvider.id, providerTier, getAdCacheSlot(), getAdLocation());
			}
			eventListener.DismissedAd(adType, _adProvider.id, providerTier, getAdCacheSlot(), getAdLocation());
		}

		protected void Log(string message)
		{
			_adBridgeLogger.Log(message);
		}

		public static T CreateAdBridgeCallbackReceiver<T>(string callbackReceiverName, AdBridgeLogger adBridgeLogger, AbstractAdProvider adProvider) where T : AdBridgeCallbackReceiver
		{
			GameObject gameObject = new GameObject(callbackReceiverName);
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			T result = gameObject.AddComponent<T>();
			result.Initialize(adBridgeLogger, adProvider);
			return result;
		}
	}
}
