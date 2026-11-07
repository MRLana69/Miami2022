#define ENABLE_ERROR_LOGS
using System;
using System.Diagnostics;
using System.Threading;
using Info;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Monetization.SupersonicPlugin.Internal
{
	public class SupersonicIOS : MonoBehaviour, ISupersonicPlatformBridge
	{
		private const string CALLBACK_RECEIVER_NAME = "SupersonicBridgeReceiver";

		private const string MEDIATION_SEGMENT_COPPA = "COPPA";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<bool> onInitFinished__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onRewardedVideoAdOpened__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onRewardedVideoAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<bool> onRewardedVideoAvailabilityChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onRewardedVideoAdStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onRewardedVideoAdEnded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoAdRewarded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onInterstitialAdReady__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onInterstitialAdLoadFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onInterstitialAdOpened__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onInterstitialAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onInterstitialAdShowSucceeded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onInterstitialAdShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onInterstitialAdClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onOfferWallAvailable__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onOfferWallOpened__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onOfferWallShowFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onOfferWallAdCredited__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onGetOfferWallCreditsFailed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onOfferWallClosed__BackingField;

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

		public event Action<string> onRewardedVideoAdShowFailed
		{
			add
			{
				Action<string> action = onRewardedVideoAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onRewardedVideoAdOpened
		{
			add
			{
				Action action = onRewardedVideoAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onRewardedVideoAdClosed
		{
			add
			{
				Action action = onRewardedVideoAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<bool> onRewardedVideoAvailabilityChanged
		{
			add
			{
				Action<bool> action = onRewardedVideoAvailabilityChanged__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAvailabilityChanged__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onRewardedVideoAvailabilityChanged__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAvailabilityChanged__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onRewardedVideoAdStarted
		{
			add
			{
				Action action = onRewardedVideoAdStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onRewardedVideoAdEnded
		{
			add
			{
				Action action = onRewardedVideoAdEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdEnded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onRewardedVideoAdEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdEnded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoAdRewarded
		{
			add
			{
				Action<string> action = onRewardedVideoAdRewarded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdRewarded__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoAdRewarded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoAdRewarded__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onInterstitialAdReady
		{
			add
			{
				Action action = onInterstitialAdReady__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdReady__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdReady__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdReady__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onInterstitialAdLoadFailed
		{
			add
			{
				Action<string> action = onInterstitialAdLoadFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdLoadFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onInterstitialAdLoadFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdLoadFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onInterstitialAdOpened
		{
			add
			{
				Action action = onInterstitialAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onInterstitialAdClosed
		{
			add
			{
				Action action = onInterstitialAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onInterstitialAdShowSucceeded
		{
			add
			{
				Action action = onInterstitialAdShowSucceeded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowSucceeded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdShowSucceeded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowSucceeded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onInterstitialAdShowFailed
		{
			add
			{
				Action<string> action = onInterstitialAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onInterstitialAdShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onInterstitialAdClicked
		{
			add
			{
				Action action = onInterstitialAdClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onInterstitialAdClicked__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInterstitialAdClicked__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onOfferWallAvailable
		{
			add
			{
				Action<string> action = onOfferWallAvailable__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallAvailable__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onOfferWallAvailable__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallAvailable__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onOfferWallOpened
		{
			add
			{
				Action action = onOfferWallOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallOpened__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onOfferWallOpened__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallOpened__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onOfferWallShowFailed
		{
			add
			{
				Action<string> action = onOfferWallShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallShowFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onOfferWallShowFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallShowFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onOfferWallAdCredited
		{
			add
			{
				Action<string> action = onOfferWallAdCredited__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallAdCredited__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onOfferWallAdCredited__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallAdCredited__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onGetOfferWallCreditsFailed
		{
			add
			{
				Action<string> action = onGetOfferWallCreditsFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGetOfferWallCreditsFailed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onGetOfferWallCreditsFailed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGetOfferWallCreditsFailed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onOfferWallClosed
		{
			add
			{
				Action action = onOfferWallClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onOfferWallClosed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onOfferWallClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("SupersonicIOS: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_SUPERSONIC_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		public void callback_onInitFinished(string msg)
		{
			onInitFinished__BackingField.SafeInvoke(msg == "YES");
		}

		public void callback_onRewardedVideoShowFail(string error)
		{
			onRewardedVideoAdShowFailed__BackingField.SafeInvoke(error);
		}

		public void callback_onRewardedVideoAdOpened()
		{
			onRewardedVideoAdOpened__BackingField.SafeInvoke();
		}

		public void callback_onRewardedVideoAdClosed()
		{
			onRewardedVideoAdClosed__BackingField.SafeInvoke();
		}

		public void callback_onVideoAvailabilityChanged(string state)
		{
			onRewardedVideoAvailabilityChanged__BackingField.SafeInvoke(state == "YES");
		}

		public void callback_onVideoStart()
		{
			onRewardedVideoAdStarted__BackingField.SafeInvoke();
		}

		public void callback_onVideoEnd()
		{
			onRewardedVideoAdEnded__BackingField.SafeInvoke();
		}

		public void callback_onRewardedVideoAdRewarded(string rewardInfo)
		{
			onRewardedVideoAdRewarded__BackingField.SafeInvoke(rewardInfo);
		}

		public void callback_onInterstitialReady()
		{
			onInterstitialAdReady__BackingField.SafeInvoke();
		}

		public void callback_onInterstitialLoadFailed(string error)
		{
			onInterstitialAdLoadFailed__BackingField.SafeInvoke(error);
		}

		public void callback_onInterstitialOpen()
		{
			onInterstitialAdOpened__BackingField.SafeInvoke();
		}

		public void callback_onInterstitialClose()
		{
			onInterstitialAdClosed__BackingField.SafeInvoke();
		}

		public void callback_onInterstitialShowSuccess()
		{
			onInterstitialAdShowSucceeded__BackingField.SafeInvoke();
		}

		public void callback_onInterstitialShowFailed(string error)
		{
			onInterstitialAdShowFailed__BackingField.SafeInvoke(error);
		}

		public void callback_onInterstitialClick()
		{
			onInterstitialAdClicked__BackingField.SafeInvoke();
		}

		public void callback_onOfferwallAvailable(string isAvailable)
		{
			onOfferWallAvailable__BackingField.SafeInvoke(isAvailable);
		}

		public void callback_onOfferwallOpened()
		{
			onOfferWallOpened__BackingField.SafeInvoke();
		}

		public void callback_onOfferwallShowFailed(string error)
		{
			onOfferWallShowFailed__BackingField.SafeInvoke(error);
		}

		public void callback_onOfferwallAdCredited(string credits)
		{
			onOfferWallAdCredited__BackingField.SafeInvoke(credits);
		}

		public void callback_onGetOfferwallCreditsFailed(string error)
		{
			onGetOfferWallCreditsFailed__BackingField.SafeInvoke(error);
		}

		public void callback_onOfferwallClosed()
		{
			onOfferWallClosed__BackingField.SafeInvoke();
		}

		public static SupersonicIOS CreatePlugin()
		{
			GameObject gameObject = new GameObject("SupersonicBridgeReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<SupersonicIOS>();
		}

		public void Init(string appKey, string userId)
		{
			_supersonicplugin_init(appKey, userId);
			if (AgeRestriction.isRestricted)
			{
				_supersonicplugin_setMediationSegment("COPPA");
			}
		}

		private static void _supersonicplugin_init(string appKey, string userId)
		{
		}

		private static void _supersonicplugin_validateIntegration()
		{
		}

		private static bool _supersonicplugin_isRewardedVideoAvailable()
		{
			return false;
		}

		private static void _supersonicplugin_loadInterstitial()
		{
		}

		private static bool _supersonicplugin_isInterstitialReady()
		{
			return false;
		}

		private static bool _supersonicplugin_isInterstitialPlacementCapped(string placementName)
		{
			return false;
		}

		private static void _supersonicplugin_showInterstitial()
		{
		}

		private static void _supersonicplugin_showInterstitialWithPlacement(string placementName)
		{
		}

		private static void _supersonicplugin_showRewardedVideo()
		{
		}

		private static void _supersonicplugin_showRewardedVideoWithPlacement(string placementName)
		{
		}

		private static void _supersonicplugin_shouldTrackNetworkState(bool shouldTrack)
		{
		}

		private static void _supersonicplugin_setDebugLogsEnabled(bool enabled)
		{
		}

		private static void _supersonicplugin_setMediationSegment(string segment)
		{
		}

		public void ValidateIntegration()
		{
			_supersonicplugin_validateIntegration();
		}

		public bool IsRewardedVideoAvailable()
		{
			return _supersonicplugin_isRewardedVideoAvailable();
		}

		public void LoadInterstitial()
		{
			_supersonicplugin_loadInterstitial();
		}

		public bool IsInterstitialReady()
		{
			return _supersonicplugin_isInterstitialReady();
		}

		public bool IsInterstitialPlacementCapped(string placementName)
		{
			return _supersonicplugin_isInterstitialPlacementCapped(placementName);
		}

		public void ShowInterstitial()
		{
			_supersonicplugin_showInterstitial();
		}

		public void ShowInterstitial(string placementName)
		{
			_supersonicplugin_showInterstitialWithPlacement(placementName);
		}

		public void ShowRewardedVideo()
		{
			_supersonicplugin_showRewardedVideo();
		}

		public void ShowRewardedVideo(string placementName)
		{
			_supersonicplugin_showRewardedVideoWithPlacement(placementName);
		}

		public void ShouldTrackNetworkState(bool shouldTrack)
		{
			_supersonicplugin_shouldTrackNetworkState(shouldTrack);
		}

		public void SetDebugLogsEnabled(bool enabled)
		{
			_supersonicplugin_setDebugLogsEnabled(enabled);
		}

		public void ShowOfferWall()
		{
			LogError("ShowOfferwall - Not supported on iOS");
		}

		public bool IsOfferWallAvailable()
		{
			LogError("IsOfferwallAvailable - Not supported on iOS");
			return false;
		}

		public void GetOfferWallCredits()
		{
			LogError("GetOfferwallCredits - Not supported on iOS");
		}
	}
}
