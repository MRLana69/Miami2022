using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common.Ads;
using Kiloo.Monetization.AdColonyPlugin;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Monetization
{
	public class AdColonyAdProvider : AbstractAdProvider
	{
		public const string ID = "adcolony";

		private bool _addedListeners;

		private bool _receivedReward;

		private readonly HashSet<string> _availableVideos = new HashSet<string>();

		public override string id
		{
			get
			{
				return "adcolony";
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
			bool flag = AdColony.IsSupportedOnPlatform();
			bool flag2 = !string.IsNullOrEmpty(configuration.appID) && configuration.zoneIDs != null && configuration.zoneIDs.Length > 0;
			_supportedAdTypes[AdType.Video] = flag && flag2;
		}

		protected override bool HasCachedVideo(string cacheID)
		{
			if (IsVideoExpired(cacheID))
			{
				_availableVideos.Remove(cacheID);
			}
			return _availableVideos.Contains(cacheID);
		}

		private bool IsVideoExpired(string cacheID)
		{
			return AdColony.IsVideoExpired(cacheID);
		}

		protected override void OnInitialize(AdProviderConfiguration configuration)
		{
			if (!_addedListeners)
			{
				AdColony.onInitFinished += () =>
				{
					FinishInitialization(true);
				};
				AdColony.onAdReward += (bool success) =>
				{
					_receivedReward = success;
				};
				AdColony.onAdClosed += () =>
				{
					if (_receivedReward)
					{
						_receivedReward = false;
						AbstractAdProvider.eventInvoker.FinishedWatchingVideo(id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
					}
					AbstractAdProvider.eventInvoker.DismissedAd(AdType.Video, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
				};
				AdColony.onCachingFailed += (string cacheID) =>
				{
					string tierForCacheId = GetTierForCacheId(cacheID);
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						AbstractAdProvider.eventInvoker.FailedToCacheAd(_cacheIDToSlot[cacheID].adType, id, tierForCacheId, _cacheIDToSlot[cacheID]);
						_cacheIDToSlot.Remove(cacheID);
					}
					else
					{
						foreach (AdCacheSlotInfo value in _cacheIDToSlot.Values)
						{
							AbstractAdProvider.eventInvoker.FailedToCacheAd(value.adType, id, tierForCacheId, value);
						}
						_cacheIDToSlot.Clear();
					}
				};
				AdColony.onCachingSuccess += (string cacheID) =>
				{
					if (_cacheIDToSlot.ContainsKey(cacheID))
					{
						_availableVideos.Add(cacheID);
						AbstractAdProvider.eventInvoker.CachedAd(_cacheIDToSlot[cacheID].adType, id, GetTierForCacheId(cacheID), _cacheIDToSlot[cacheID], false);
						_cacheIDToSlot.Remove(cacheID);
					}
				};
				_addedListeners = true;
			}
			try
			{
				AdColony.Configure(configuration.appID, configuration.zoneIDs);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				FinishInitialization(false);
			}
		}

		protected override void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			_cacheIDToSlot[cacheID] = adCacheSlotInfo;
			AbstractAdProvider.eventInvoker.StartedCachingAd(adCacheSlotInfo.adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			AdColony.LoadVideoAd(cacheID);
		}

		protected override void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			_showVideoCacheSlot = adCacheSlotInfo;
			_showVideoAdLocation = adLocationInfo;
			_availableVideos.Remove(cacheID);
			AdStarted();
			AdColony.PlayVideoAd(cacheID);
		}

		private void AdStarted()
		{
			_receivedReward = false;
			AbstractAdProvider.eventInvoker.ShowedAd(_showVideoCacheSlot.adType, id, GetCurrentTier(_showVideoCacheSlot), _showVideoCacheSlot, _showVideoAdLocation);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("AdColony: " + msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_ADCOLONY_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
