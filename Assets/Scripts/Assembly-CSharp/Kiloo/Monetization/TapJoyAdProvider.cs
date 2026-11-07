using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Monetization.TapjoyPlugin;
using UnityEngine;

namespace Kiloo.Monetization
{
	public class TapJoyAdProvider : AbstractAdProvider
	{
		public const string ID = "tapjoy";

		private List<string> _cachedIDs = new List<string>();

		private bool _completedVideo;

		private bool _addedListeners;

		private List<string> _allVideoCacheIDs = new List<string>();

		public override string id
		{
			get
			{
				return "tapjoy";
			}
		}

		public override bool supportsMulticaching
		{
			get
			{
				return false;
			}
		}

		protected override void CheckPlatformSupport(AdProviderConfiguration configuration)
		{
			bool flag = TapJoy.IsSupportedOnPlatform();
			bool flag2 = !string.IsNullOrEmpty(configuration.appID) && !string.IsNullOrEmpty(configuration.appSDKKey);
			_supportedAdTypes[AdType.Video] = flag && flag2;
		}

		protected override void OnInitialize(AdProviderConfiguration configuration)
		{
			if (!_addedListeners)
			{
				TapJoy.onInitializeFinished += OnInitializeFinished;
				TapJoy.onFullScreenAdSuccess += OnFullScreenAdSuccess;
				TapJoy.onFullScreenAdFail += OnFullScreenAdFail;
				TapJoy.onVideoError += OnVideoError;
				TapJoy.onViewDidAppear += OnVideoStart;
				TapJoy.onViewDidDisappear += OnViewDidDisappear;
				TapJoy.onVideoComplete += OnVideoComplete;
				_addedListeners = true;
			}
			try
			{
				if (!TapJoy.isInitialized)
				{
					TapJoy.Initialize(configuration.appID, configuration.appSDKKey);
				}
				else
				{
					FinishInitialization(true);
				}
			}
			catch (Exception)
			{
				FinishInitialization(false);
			}
		}

		private void OnVideoStart(string cacheID)
		{
			if (IsVideoCacheIDValid(cacheID))
			{
				_completedVideo = false;
				AbstractAdProvider.eventInvoker.ShowedAd(AdType.Video, id, GetTierForCacheId(cacheID), _showVideoCacheSlot, _showVideoAdLocation);
			}
		}

		private void OnViewDidDisappear(string cacheID)
		{
			if (IsVideoCacheIDValid(cacheID))
			{
				string tierForCacheId = GetTierForCacheId(cacheID);
				if (_completedVideo)
				{
					_completedVideo = false;
					AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, tierForCacheId, _showVideoCacheSlot, _showVideoAdLocation);
				}
				AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, tierForCacheId, _showVideoCacheSlot, _showVideoAdLocation);
			}
		}

		private void OnVideoComplete()
		{
			_completedVideo = true;
		}

		private bool IsVideoCacheIDValid(string cacheID)
		{
			if (_allVideoCacheIDs.Count == 0)
			{
				_allVideoCacheIDs = GetAllCacheIDsAsList(AdType.Video);
			}
			return _allVideoCacheIDs.Contains(cacheID);
		}

		private void OnVideoError(string error)
		{
			AbstractAdProvider.eventInvoker.FailedToShowAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
		}

		private void OnFullScreenAdSuccess(string cacheID)
		{
			if (!(cacheID == "Offerwall"))
			{
				_cachedIDs.Add(cacheID);
				if (_cacheIDToSlot.ContainsKey(cacheID))
				{
					AbstractAdProvider.eventInvoker.CachedAd(AdType.Video, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID], false);
					_cacheIDToSlot.Remove(cacheID);
				}
				else
				{
					LogError("Received cache success for cacheId not in dictionary: " + cacheID);
				}
			}
		}

		private void OnFullScreenAdFail(string cacheID)
		{
			string tierForCacheId = GetTierForCacheId(cacheID);
			if (_cachedIDs.Contains(cacheID))
			{
				_cachedIDs.Remove(cacheID);
			}
			if (_cacheIDToSlot.ContainsKey(cacheID))
			{
				AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, tierForCacheId, _cacheIDToSlot[cacheID]);
				_cacheIDToSlot.Remove(cacheID);
				return;
			}
			LogError("Received cache success for cacheId not in dictionary: " + cacheID);
			foreach (AdCacheSlotInfo item in _cacheIDToSlot.Values.Where((AdCacheSlotInfo location) => location.adType == AdType.Video))
			{
				AbstractAdProvider.eventInvoker.FailedToCacheAd(AdType.Video, id, tierForCacheId, item);
			}
			_cacheIDToSlot.RemoveWhere((KeyValuePair<string, AdCacheSlotInfo> kvp) => kvp.Value.adType == AdType.Interstitial);
		}

		private void OnInitializeFinished(string result)
		{
			FinishInitialization(result.ToLower() == "success");
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(AdType.Video, id, GetTierForCacheId(cacheID), adCacheSlotInfo);
			TapJoy.GetFullScreenAd(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			if (_cachedIDs.Contains(cacheID))
			{
				_cachedIDs.Remove(cacheID);
			}
			_showVideoCacheSlot = adCacheSlotInfo;
			_showVideoAdLocation = adLocationInfo;
			TapJoy.ShowFullScreenAd(cacheID);
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			return _cachedIDs.Contains(cacheID);
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("[TapJoy] " + msg, context);
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
