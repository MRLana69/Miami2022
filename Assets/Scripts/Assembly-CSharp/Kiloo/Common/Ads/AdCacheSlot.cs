using System;
using System.Collections;
using System.Collections.Generic;
using Info;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Ads
{
	public class AdCacheSlot : AdEventListenerAdapter
	{
		private const float CHECK_TIMEOUT_INTERVAL = 0.5f;

		private const float INITIALIZE_WAIT_TIME_SECONDS = 5f;

		private const bool DEFAULT_SHOULD_TRAVERSE_WATERFALL_ON_SHOW = true;

		private const bool DEFAULT_SHOULD_TRAVERSE_WATERFALL_FOR_IS_CACHED = true;

		private const string ONLINE_SETTINGS_KEY_SHOULD_TRAVERSE_WATERFALL_ON_SHOW = "should_traverse_waterfall_on_show";

		private const string ONLINE_SETTINGS_KEY_SHOULD_TRAVERSE_WATERFALL_FOR_IS_CACHED = "should_traverse_waterfall_for_is_cached";

		private AbstractAdProvider _adProvider;

		private AdProviderInfo _adProviderInfo;

		private IAdSettingsProvider _settingsProvider;

		private IEnumerator _cacheCoroutine;

		private bool _shouldRestartIsCachingCheck;

		private IEnumerator _unavailableCoroutine;

		public readonly AdCacheSlotInfo info;

		public readonly string waterfallID;

		private bool hasProvider
		{
			get
			{
				return _adProvider != null;
			}
		}

		public bool isCurrentProviderCaching
		{
			get
			{
				return hasProvider && _adProvider.isCachingSlot(info);
			}
		}

		public AdCacheSlot(AdCacheSlotInfo info, string waterfallID)
		{
			this.info = info;
			this.waterfallID = waterfallID;
		}

		public void SetSettingsProvider(IAdSettingsProvider settingsProvider)
		{
			_settingsProvider = settingsProvider;
		}

		public void SetAdProvider(AdProviderInfo newProviderInfo)
		{
			if (newProviderInfo == null)
			{
				AdSystemLog.LogError("Provider info was null while setting provider for slot - " + info.id);
				return;
			}
			AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(newProviderInfo.providerID);
			if (adProvider != null && adProvider.IsSupportedOnPlatform(info.adType))
			{
				_adProvider = adProvider;
				_adProviderInfo = newProviderInfo;
				_adProvider.SetCurrentTier(info, newProviderInfo.providerTier);
			}
			else
			{
				_adProvider = null;
				_adProviderInfo = null;
			}
		}

		public string GetCurrentAdProviderTier()
		{
			return (_adProviderInfo != null) ? _adProviderInfo.providerTier : "null";
		}

		private string GetCacheId(string providerTier)
		{
			return _settingsProvider.GetCacheID(providerTier, info);
		}

		public string GetHighestPriorityCachedProviderTier()
		{
			List<AdProviderInfo> sortedProviderList = GetSortedProviderList();
			foreach (AdProviderInfo item in sortedProviderList)
			{
				AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(item.providerID);
				if (adProvider.HasCachedAd(info.adType, GetCacheId(item.providerTier)))
				{
					return item.providerTier;
				}
			}
			return "none";
		}

		public bool IsCached()
		{
			if (IsCurrentProviderCached())
			{
				return true;
			}
			if (OnlineSettings.instance.GetValue("should_traverse_waterfall_for_is_cached", true))
			{
				List<AdProviderInfo> sortedProviderList = GetSortedProviderList();
				if (sortedProviderList != null)
				{
					foreach (AdProviderInfo item in sortedProviderList)
					{
						AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(item.providerID);
						if (adProvider.HasCachedAd(info.adType, GetCacheId(item.providerTier)))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool IsCurrentProviderCached()
		{
			return hasProvider && _adProvider.HasCachedAd(info.adType, GetCacheId(_adProviderInfo.providerTier));
		}

		public bool Show(AdLocationInfo adLocationInfo)
		{
			if (!hasProvider)
			{
				AdSystemLog.LogError("Provider was null for slot: " + info.id);
				return false;
			}
			if (OnlineSettings.instance.GetValue("should_traverse_waterfall_on_show", true))
			{
				List<AdProviderInfo> sortedProviderList = GetSortedProviderList();
				if (sortedProviderList != null)
				{
					foreach (AdProviderInfo item in sortedProviderList)
					{
						AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(item.providerID);
						if (adProvider.HasCachedAd(info.adType, GetCacheId(item.providerTier)))
						{
							return adProvider.ShowAd(info.adType, info, adLocationInfo, GetCacheId(item.providerTier));
						}
					}
				}
				return false;
			}
			return _adProvider.ShowAd(info.adType, info, adLocationInfo, GetCacheId(_adProviderInfo.providerTier));
		}

		private List<AdProviderInfo> GetSortedProviderList()
		{
			List<AdProviderInfo> providersForCacheSlot = AdSystemManager.GetProvidersForCacheSlot(this);
			if (providersForCacheSlot != null)
			{
				providersForCacheSlot.Sort();
				return providersForCacheSlot;
			}
			return null;
		}

		public void Cache()
		{
			if (!hasProvider)
			{
				AdSystemLog.LogWarning("Unable to cache cacheSlotId" + info.id + ", provider was null");
				return;
			}
			if (!_adProvider.supportsMulticaching && _adProvider.HasCachedAd(info.adType, GetCacheId(_adProviderInfo.providerTier)))
			{
				_adProvider.CacheAd(info.adType, info, GetCacheId(_adProviderInfo.providerTier));
				return;
			}
			CoroutineUtil.StopCoroutine(_unavailableCoroutine);
			_unavailableCoroutine = WaitWhileUnavailableThenCache();
			CoroutineUtil.StartCoroutine(_unavailableCoroutine);
		}

		private IEnumerator WaitWhileConditionOrUntilTimedOut(Func<bool> condition, float timeoutInSeconds)
		{
			float startTime = Time.realtimeSinceStartup;
			float elapsedTime = 0f;
			while (condition() && elapsedTime < timeoutInSeconds)
			{
				elapsedTime = Time.realtimeSinceStartup - startTime;
				yield return null;
			}
		}

		private IEnumerator WaitWhileUnavailableThenCache()
		{
			float startTime = Time.realtimeSinceStartup;
			Func<bool> isInitializingFunc = () => hasProvider && _adProvider.isInitializing;
			Func<bool> isCachingFunc = () => hasProvider && _adProvider.isInitialized && _adProvider.isCaching && !_adProvider.supportsMulticaching;
			int waitTimeout = (hasProvider ? _settingsProvider.GetProviderWaitWhileCachingTimeLimit(info.id, _adProvider.id) : 0);
			yield return WaitWhileConditionOrUntilTimedOut(isInitializingFunc, 5f);
			do
			{
				_shouldRestartIsCachingCheck = false;
				if (isCachingFunc())
				{
					yield return WaitWhileConditionOrUntilTimedOut(isCachingFunc, waitTimeout);
				}
			}
			while (_shouldRestartIsCachingCheck);
			bool isInitializing = isInitializingFunc();
			bool isStillCaching = isCachingFunc();
			if (hasProvider)
			{
				if (_adProvider.isInitialized)
				{
					_adProvider.CacheAd(info.adType, info, GetCacheId(_adProviderInfo.providerTier));
					yield break;
				}
				_adProvider.FailCaching(info.adType, info);
				AdSystemLog.LogWarning("WaitWhileUnavailableThenCache - Initialisation of provider " + _adProvider.id + " failed!");
			}
			else
			{
				AdSystemLog.LogWarning("WaitWhileUnavailableThenCache - Does not have ad provider");
			}
		}

		private IEnumerator CacheCoroutine()
		{
			float startTime = Time.realtimeSinceStartup;
			float elapsedTime = 0f;
			double cacheTimeoutLimit = _settingsProvider.GetCacheTimeoutLimit(_adProvider.id, info.adType).TotalSeconds;
			if (!isCurrentProviderCaching)
			{
				AdSystemLog.LogWarning("isCaching was false on entering AdCacheSlot.CacheCoroutine. Check order of events when Ad Provider triggers StartedCachingAd.");
				yield return null;
			}
			while (isCurrentProviderCaching)
			{
				if ((double)elapsedTime > cacheTimeoutLimit)
				{
					AdSystemLog.LogWarning("Timeout for cache slot " + info.id + " from " + _adProvider.id + ". Elapsed time: " + elapsedTime);
					_adProvider.TimeoutCaching(info.adType, info);
					break;
				}
				elapsedTime = Time.realtimeSinceStartup - startTime;
				yield return Wait.For(0.5f).UnscaledSeconds().StartCoroutine();
			}
			if (isCurrentProviderCaching)
			{
				AdSystemLog.LogError("CacheCoroutine for cache slot: " + info.id + " finished but the provider " + _adProvider.id + " hasn't cached it's ad!");
			}
		}

		public override void StartedCachingAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
		{
			if (hasProvider && _adProvider.id == providerID && adCacheSlotInfo != info)
			{
				_shouldRestartIsCachingCheck = true;
			}
			if (info == adCacheSlotInfo)
			{
				CoroutineUtil.StopCoroutine(_cacheCoroutine);
				_cacheCoroutine = CacheCoroutine();
				CoroutineUtil.StartCoroutine(_cacheCoroutine);
			}
		}

		public override void ChangedProvider(AdType adType, AdProviderInfo newProviderInfo, string changedWaterfallID)
		{
			if (hasProvider && info.adType == adType && waterfallID == changedWaterfallID)
			{
				SetAdProvider(newProviderInfo);
			}
		}

		public override void DismissedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			MakeSureAdIsCached(providerID, adCacheSlotInfo);
		}

		public override void FinishedWatchingVideo(string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			MakeSureAdIsCached(providerID, adCacheSlotInfo);
		}

		public override void FailedToShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			MakeSureAdIsCached(providerID, adCacheSlotInfo);
		}

		public override void FailedToCacheAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
		{
			if (hasProvider && providerID == _adProvider.id && adType != info.adType)
			{
				_shouldRestartIsCachingCheck = true;
			}
		}

		private void MakeSureAdIsCached(string providerID, AdCacheSlotInfo adCacheSlotInfo)
		{
			if (hasProvider && providerID == _adProvider.id)
			{
				if (adCacheSlotInfo != info)
				{
					_shouldRestartIsCachingCheck = true;
				}
				if (adCacheSlotInfo == info && !_adProvider.HasCachedAd(info.adType, GetCacheId(_adProviderInfo.providerTier)))
				{
					Cache();
				}
			}
		}

		public bool HasProviderTierCachedAdDebug(AdProviderInfo adProviderInfo)
		{
			AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(adProviderInfo.providerID);
			return adProvider != null && adProvider.HasCachedAd(info.adType, GetCacheId(adProviderInfo.providerTier));
		}

		public bool IsProviderTierCachingAdDebug(AdProviderInfo adProviderInfo)
		{
			AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(adProviderInfo.providerID);
			return adProvider != null && adProvider.isCachingSlot(info);
		}

		public void DebugCacheAdOnProviderTier(AdProviderInfo adProviderInfo)
		{
			AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(adProviderInfo.providerID);
			if (adProvider != null)
			{
				adProvider.CacheAd(info.adType, info, GetCacheId(adProviderInfo.providerTier));
			}
		}

		public void DebugShowAdOnProviderTier(AdProviderInfo adProviderInfo)
		{
			AbstractAdProvider adProvider = AbstractAdProvider.GetAdProvider(adProviderInfo.providerID);
			if (adProvider != null)
			{
				adProvider.ShowAd(info.adType, info, AdLocations.UNDETERMINED_LOCATION, GetCacheId(adProviderInfo.providerTier));
			}
		}

		public string DebugGetCacheIdForProvider(AdProviderInfo adProviderInfo)
		{
			return GetCacheId(adProviderInfo.providerTier);
		}

		public List<AdProviderInfo> DebugGetProviderListForSlot(AdCacheSlotInfo adCacheSlotInfo)
		{
			return GetSortedProviderList();
		}
	}
}
