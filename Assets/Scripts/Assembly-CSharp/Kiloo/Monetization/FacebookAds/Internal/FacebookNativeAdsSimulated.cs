using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Kiloo.Monetization.FacebookAds.Internal
{
	public class FacebookNativeAdsSimulated : MonoBehaviour, IFacebookNativeAdsBridge
	{
		private bool _adReady;

		private bool _adLoading;

		private bool _initialized;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<bool> onInitialize__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdLoaded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdShown__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeAdError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeClicked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onNativeLoggedImpression__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoDidLoad__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoError__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoCompleted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoClosed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> onRewardedVideoStarted__BackingField;

		private const string CALLBACK_RECEIVER_NAME = "FacebookAdSimulatedReceiver";

		public event Action<bool> onInitialize
		{
			add
			{
				Action<bool> action = onInitialize__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitialize__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onInitialize__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onInitialize__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdLoaded
		{
			add
			{
				Action<string> action = onNativeAdLoaded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdLoaded__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdLoaded__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdLoaded__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdShown
		{
			add
			{
				Action<string> action = onNativeAdShown__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdShown__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdShown__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdShown__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdClosed
		{
			add
			{
				Action<string> action = onNativeAdClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdClosed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdClosed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeAdError
		{
			add
			{
				Action<string> action = onNativeAdError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdError__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeAdError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeAdError__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeClicked
		{
			add
			{
				Action<string> action = onNativeClicked__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeClicked__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeClicked__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeClicked__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onNativeLoggedImpression
		{
			add
			{
				Action<string> action = onNativeLoggedImpression__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeLoggedImpression__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onNativeLoggedImpression__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNativeLoggedImpression__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoDidLoad
		{
			add
			{
				Action<string> action = onRewardedVideoDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoDidLoad__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoDidLoad__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoDidLoad__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoError
		{
			add
			{
				Action<string> action = onRewardedVideoError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoError__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoError__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoError__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoCompleted
		{
			add
			{
				Action<string> action = onRewardedVideoCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoCompleted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoCompleted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoClosed
		{
			add
			{
				Action<string> action = onRewardedVideoClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoClosed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoClosed__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoClosed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> onRewardedVideoStarted
		{
			add
			{
				Action<string> action = onRewardedVideoStarted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoStarted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = onRewardedVideoStarted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onRewardedVideoStarted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void Initialize()
		{
			onInitialize__BackingField.SafeInvoke(true);
			_initialized = true;
		}

		public bool IsNativeAdLoaded(string cacheID)
		{
			return _adReady;
		}

		public bool IsVideoAdLoaded(string cacheID)
		{
			return false;
		}

		public void CacheNativeAd(string cacheID)
		{
			if (_initialized && !_adLoading)
			{
				_adLoading = true;
				_adReady = false;
			}
		}

		public void LoadVideoAd(string cacheID)
		{
		}

		public void ShowVideo(string cacheID)
		{
		}

		public void ShowInterstitial(string cacheID)
		{
		}

		public void LogExternalClick(string cacheID)
		{
		}

		public void LogExternalImpression(string cacheID)
		{
		}

		public void AddTestDevice(string deviceID)
		{
		}

		public void SetDebugLogs(bool enableLogs)
		{
		}

		public static FacebookNativeAdsSimulated CreatePlugin()
		{
			GameObject gameObject = new GameObject("FacebookAdSimulatedReceiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<FacebookNativeAdsSimulated>();
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("FacebookNativeAdsSimulated: " + msg, context);
		}

		[Conditional("ENABLE_FACEBOOKADS_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_FACEBOOKADS_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
