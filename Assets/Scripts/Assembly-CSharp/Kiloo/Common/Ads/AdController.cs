using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdHandling.AdConstants;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UI.Popups;
using UnityEngine;

namespace Kiloo.Common.Ads
{
	public class AdController
	{
		private class CacheList
		{
			public readonly List<AdCacheSlot> list;

			public readonly int capacity;

			private int _slotToShowNext;

			public int count
			{
				get
				{
					return list.Count;
				}
			}

			public CacheList(int capacity)
			{
				this.capacity = capacity;
				list = new List<AdCacheSlot>(capacity);
			}

			public void Add(AdCacheSlot cacheSlot)
			{
				list.Add(cacheSlot);
			}

			public AdCacheSlot GetSlotToShow()
			{
				return list[_slotToShowNext];
			}

			public void AdvanceShowPointer()
			{
				_slotToShowNext++;
				if (_slotToShowNext >= count)
				{
					_slotToShowNext = 0;
				}
			}
		}

		public class EventHandler : AdEventListenerAdapter
		{
			private AdController _controller;

			public EventHandler(AdController controller)
			{
				_controller = controller;
			}

			public override void ShowedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				if (adType == AdType.Interstitial)
				{
					_controller._timeOfLastInterstitial = DateTime.Now;
					_controller._timeOfLastAdAtLocation[adLocationInfo] = DateTime.Now;
					_controller._interstitialStats.IncrementShowCount();
					if (_controller._interstitialStats.ReachedDailyLimit(_controller._settings))
					{
						foreach (IAdEventListener eventListener in _controller._eventListeners)
						{
							eventListener.ReachedDailyInterstitialLimit(_controller._settings.GetMaxInterstitialImpressionsPerDay());
						}
					}
					else if (_controller._interstitialStats.ReachedHourlyLimit(_controller._settings))
					{
						foreach (IAdEventListener eventListener2 in _controller._eventListeners)
						{
							eventListener2.ReachedHourlyInterstitialLimit(_controller._settings.GetMaxInterstitialImpressionsPerHour());
						}
					}
				}
				if (Globals.DEBUG_AD_SYSTEM_SINGLE_IMPRESSION)
				{
					AdCacheSlot adCacheSlotFromID = _controller.GetAdCacheSlotFromID(adCacheSlotInfo.id);
					if (adCacheSlotFromID != null && adCacheSlotFromID.GetCurrentAdProviderTier() == providerTier)
					{
						PriorityList priorityList = _controller._idToPriorityList[adCacheSlotFromID.waterfallID];
						priorityList.ChangeToNextProvider();
					}
				}
			}

			public override void DismissedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				if (adType == AdType.Interstitial)
				{
					_controller._timeOfLastInterstitial = DateTime.Now;
					_controller._timeOfLastAdAtLocation[adLocationInfo] = DateTime.Now;
				}
				CoroutineUtil.StopCoroutine(_controller._checkCachesCoroutine);
				_controller._checkCachesCoroutine = CoroutineUtil.StartCoroutine(_controller.CheckCacheExpiredCoroutine());
			}

			public override void FailedToInitAdProvider(string providerID)
			{
				AdSystemLog.LogError("Failed to initialize ad provider " + providerID);
			}

			public override void CachingTimedOut(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
			{
				AdSystemLog.LogError("Cache Slot: " + adCacheSlotInfo.id + " | provider:" + providerID + " timed out!");
				HandleFail(providerID, providerTier, adCacheSlotInfo);
			}

			public override void FailedToCacheAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
			{
				AdSystemLog.LogError("Failed to cache " + adCacheSlotInfo.id + ", " + providerID);
				HandleFail(providerID, providerTier, adCacheSlotInfo);
			}

			public override void CachedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
			{
			}

			private bool IncrementFailCount(string waterfallID)
			{
				return _controller._idToPriorityList[waterfallID].IncrementNumberOfFails();
			}

			public override void FailedToShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				AdSystemLog.LogError("Failed to show " + adCacheSlotInfo.id + ", " + providerID);
				CoroutineUtil.StopCoroutine(_controller._checkCachesCoroutine);
				_controller._checkCachesCoroutine = CoroutineUtil.StartCoroutine(_controller.CheckCacheExpiredCoroutine());
				HandleFail(providerID, providerTier, adCacheSlotInfo);
			}

			private void HandleFail(string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
			{
				string currentProviderIDForCacheSlot = _controller.GetCurrentProviderIDForCacheSlot(adCacheSlotInfo);
				if (currentProviderIDForCacheSlot != providerID)
				{
					return;
				}
				string currentProviderTierForCacheSlot = _controller.GetCurrentProviderTierForCacheSlot(adCacheSlotInfo);
				if (!currentProviderTierForCacheSlot.IsNullOrEmpty() && !providerTier.IsNullOrEmpty() && currentProviderTierForCacheSlot != providerTier)
				{
					return;
				}
				AdCacheSlot adCacheSlotFromID = _controller.GetAdCacheSlotFromID(adCacheSlotInfo.id);
				if (adCacheSlotFromID == null)
				{
					AdSystemLog.LogError("Unknown ad cacheSlot " + adCacheSlotInfo.id);
					return;
				}
				PriorityList priorityList = _controller._idToPriorityList[adCacheSlotFromID.waterfallID];
				bool reachedLastProvider = priorityList.reachedLastProvider;
				if (IncrementFailCount(adCacheSlotFromID.waterfallID) && !reachedLastProvider && _controller.allowReCacheOnFail)
				{
					_controller.Cache(adCacheSlotFromID);
				}
			}
		}

		private readonly IAdSettingsProvider _settings;

		private readonly List<AdCacheSlot> _adCacheSlots;

		private readonly Dictionary<string, PriorityList> _idToPriorityList = new Dictionary<string, PriorityList>();

		private DateTime _timeOfLastInterstitial = DateTime.MinValue;

		private Dictionary<AdLocationInfo, DateTime> _timeOfLastAdAtLocation = new Dictionary<AdLocationInfo, DateTime>();

		private InterstitialStats _interstitialStats;

		private List<IAdEventListener> _eventListeners = new List<IAdEventListener>();

		private const int DEFAULT_NUMBER_OF_VIDEO_ADS_TO_CACHE = 2;

		private const int DEFAULT_NUMBER_OF_INTERSTITIALS_TO_CACHE = 1;

		private const float DEFAULT_AD_EXPIRY_MIN_CHECK_INTERVAL = 10f;

		private const float DEFAULT_AD_EXPIRY_MAX_CHECK_INTERVAL = 30f;

		private const string ONLINE_SETTINGS_KEY_AD_MIN_EXPIRY_CHECK_INTERVAL = "ad_expiry_min_check_interval";

		private const string ONLINE_SETTINGS_KEY_AD_MAX_EXPIRY_CHECK_INTERVAL = "ad_expiry_max_check_interval";

		private readonly Dictionary<AdType, CacheList> _cacheLists;

		private Coroutine _checkCachesCoroutine;

		private static float adExpiryMaxCheckInterval
		{
			get
			{
				return OnlineSettings.instance.GetValue("ad_expiry_max_check_interval", 30f);
			}
		}

		private static float adExpiryMinCheckInterval
		{
			get
			{
				return OnlineSettings.instance.GetValue("ad_expiry_min_check_interval", 10f);
			}
		}

		public bool allowReCacheOnFail { get; set; }

		public AdController(IAdSettingsProvider settings, ICollection<AdCacheSlotInfo> adCacheSlots, InterstitialStats interstitialStats)
		{
			_settings = settings;
			_adCacheSlots = new List<AdCacheSlot>(adCacheSlots.Count);
			_cacheLists = new Dictionary<AdType, CacheList>
			{
				{
					AdType.Video,
					new CacheList(2)
				},
				{
					AdType.Interstitial,
					new CacheList(1)
				}
			};
			_interstitialStats = interstitialStats;
			allowReCacheOnFail = true;
			AbstractAdProvider.MapAdProviders(settings);
			AdConstants.constantsChanged += AbstractAdProvider.HandleAdConstantsChanged;
			foreach (AdCacheSlotInfo adCacheSlot2 in adCacheSlots)
			{
				if (!_settings.GetAvailabilityForCacheSlot(adCacheSlot2.id))
				{
					continue;
				}
				string waterfallID = GetWaterfallID(adCacheSlot2);
				AdType adType = adCacheSlot2.adType;
				if (waterfallID != null)
				{
					if (!_idToPriorityList.ContainsKey(waterfallID))
					{
						PriorityList value = CreatePriorityListForID(waterfallID, adType, settings, adCacheSlot2);
						_idToPriorityList.Add(waterfallID, value);
					}
					AdCacheSlot adCacheSlot = new AdCacheSlot(adCacheSlot2, waterfallID);
					AdProviderInfo currentProviderInfo = GetCurrentProviderInfo(adCacheSlot);
					adCacheSlot.SetSettingsProvider(settings);
					adCacheSlot.SetAdProvider(currentProviderInfo);
					_adCacheSlots.Add(adCacheSlot);
					CacheList cacheList = _cacheLists[adType];
					cacheList.Add(adCacheSlot);
					AddEventListener(adCacheSlot);
				}
			}
			foreach (KeyValuePair<AdType, CacheList> cacheList2 in _cacheLists)
			{
				if (cacheList2.Value.count < cacheList2.Value.capacity && (cacheList2.Key != AdType.Interstitial || _settings.AreInterstitialsAllowed()))
				{
					AdSystemLog.LogError(string.Format("{0} Cache Pool Size set to {1} but only {2} slots found!", cacheList2.Key, cacheList2.Value.capacity, cacheList2.Value.count));
				}
			}
			_checkCachesCoroutine = CoroutineUtil.StartCoroutine(CheckCacheExpiredCoroutine());
		}

		public void UpdateInterstitialStats(InterstitialStats stats)
		{
			_interstitialStats = stats;
		}

		public void AddEventListener(IAdEventListener eventListener)
		{
			AbstractAdProvider.AddEventListener(eventListener);
			_eventListeners.Add(eventListener);
		}

		public void RemoveEventListener(IAdEventListener eventListener)
		{
			AbstractAdProvider.RemoveEventListener(eventListener);
			_eventListeners.Remove(eventListener);
		}

		public bool IsCached(AdLocationInfo adLocationInfo)
		{
			if (!_settings.GetAvailabilityForLocation(adLocationInfo.id))
			{
				return false;
			}
			return GetCachedSlots(adLocationInfo.adType).Count > 0;
		}

		private List<AdCacheSlot> GetCachedSlots(AdType adType)
		{
			return _cacheLists[adType].list.FindAll((AdCacheSlot cacheSlot) => cacheSlot.IsCached());
		}

		public bool IsCaching(AdCacheSlotInfo adCacheSlotInfo)
		{
			bool result = false;
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				result = adCacheSlotFromID.isCurrentProviderCaching;
			}
			else
			{
				AdSystemLog.LogError("Unknown ad cacheSlot ID " + adCacheSlotInfo.id);
			}
			return result;
		}

		private IEnumerator CheckCacheExpiredCoroutine()
		{
			float adExpiryCheckInterval = adExpiryMinCheckInterval;
			while (true)
			{
				yield return new WaitForSeconds(adExpiryCheckInterval);
				bool foundExpiredAd = false;
				foreach (CacheList value in _cacheLists.Values)
				{
					foreach (AdCacheSlot item in value.list)
					{
						if (!item.isCurrentProviderCaching && !item.IsCurrentProviderCached())
						{
							item.Cache();
							foundExpiredAd = true;
							break;
						}
					}
				}
				adExpiryCheckInterval = ((!foundExpiredAd) ? adExpiryMaxCheckInterval : adExpiryMinCheckInterval);
			}
		}

		public void Cache(AdCacheSlot specificCacheSlot = null)
		{
			foreach (KeyValuePair<AdType, CacheList> cacheList in _cacheLists)
			{
				foreach (AdCacheSlot item in cacheList.Value.list)
				{
					if ((specificCacheSlot == null || item == specificCacheSlot) && (DebugSettingsPopup.forceRefreshAdSystem || !item.IsCurrentProviderCached()))
					{
						_idToPriorityList[item.waterfallID].wrapped = false;
						if (HasPriorityListChanged(item) || !_idToPriorityList[item.waterfallID].hasProviders)
						{
							UpdatePriorityListForID(item.waterfallID, item.info.adType, item.info);
						}
						AdProviderInfo currentProviderInfo = GetCurrentProviderInfo(item);
						if (currentProviderInfo == null && !Globals.DEBUG_VIDEOADS_PROVIDERS && !Globals.DEBUG_INTERSTITIAL_PROVIDERS)
						{
							return;
						}
						item.SetAdProvider(currentProviderInfo);
						item.Cache();
					}
				}
			}
		}

		public bool Show(AdLocationInfo adLocationInfo)
		{
			if (!AdLocations.adLocations.Contains(adLocationInfo))
			{
				AdSystemLog.LogError("Unknown ad location with ID: " + adLocationInfo.id);
				return false;
			}
			if (!_settings.GetAvailabilityForLocation(adLocationInfo.id))
			{
				AdSystemLog.LogWarning("Show called on location: " + adLocationInfo.id + " but it has been disabled.");
				return false;
			}
			if (adLocationInfo.adType == AdType.Interstitial)
			{
				if (Application.isEditor && !Globals.DEBUG_INTERSTITIAL_IN_EDITOR)
				{
					return false;
				}
				if (!_settings.AreInterstitialsAllowed())
				{
					return false;
				}
				if (_interstitialStats.ReachedLimit(_settings))
				{
					return false;
				}
				if (!_interstitialStats.hasSeenFirstInterstitial && CDateTime.UtcNow < _settings.GetScheduledFirstInterstitialShowTime())
				{
					foreach (IAdEventListener eventListener in _eventListeners)
					{
						eventListener.ReachedNewUserInterstitialLimit((int)_settings.GetGracePeriodValue());
					}
					return false;
				}
			}
			TimeSpan globalInterval = _settings.GetInterstitialShowInterval();
			bool flag = DateTime.Now > _timeOfLastInterstitial + globalInterval;
			if (_settings.OverrideInterstitialShowIntervalForLocation(adLocationInfo.id, ref globalInterval))
			{
				DateTime dateTime = ((!_timeOfLastAdAtLocation.ContainsKey(adLocationInfo)) ? DateTime.MinValue : _timeOfLastAdAtLocation[adLocationInfo]);
				flag = DateTime.Now > dateTime + globalInterval && DateTime.Now > _timeOfLastInterstitial + globalInterval;
			}
			bool dEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT = Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT;
			bool flag2 = _timeOfLastInterstitial > DateTime.Now;
			bool flag3 = flag || flag2 || dEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT;
			if (adLocationInfo.adType == AdType.Video || flag3)
			{
				AdCacheSlot adCacheSlot = null;
				CacheList cacheList = _cacheLists[adLocationInfo.adType];
				if (!cacheList.list.Exists((AdCacheSlot location) => location.IsCached()))
				{
					string eventName = string.Format("Ad Location: {0}", adLocationInfo.id);
					string argValue = string.Format("{0};{1}", "null", "No cached ad");
					Flurry.LogEventWithAParameter(eventName, "Outcome", argValue);
					AdSystemLog.LogError("Show called from location: " + adLocationInfo.id + ", but no video ads are cached!");
					Cache();
					return false;
				}
				for (int num = 0; num < cacheList.count; num++)
				{
					if (adCacheSlot != null && adCacheSlot.IsCached())
					{
						break;
					}
					adCacheSlot = cacheList.GetSlotToShow();
					cacheList.AdvanceShowPointer();
				}
				if (adCacheSlot == null)
				{
					AdSystemLog.LogError("!!! Show called for location: " + adLocationInfo.id + " and though it wasn't caught by the no ad cached checks, no cached ad was found!");
					return false;
				}
				string eventName2 = string.Format("Ad Location: {0}", adLocationInfo.id);
				string argValue2 = string.Format("{0};{1}", adCacheSlot.GetCurrentAdProviderTier(), "Success");
				Flurry.LogEventWithAParameter(eventName2, "Outcome", argValue2);
				if (adCacheSlot.Show(adLocationInfo))
				{
					CoroutineUtil.StopCoroutine(_checkCachesCoroutine);
					return true;
				}
				return false;
			}
			foreach (IAdEventListener eventListener2 in _eventListeners)
			{
				eventListener2.ReachedTimeBetweenAdsInterstitialLimit((int)globalInterval.TotalSeconds);
			}
			return false;
		}

		public void ChangeToNextProvider()
		{
			foreach (PriorityList value in _idToPriorityList.Values)
			{
				value.ChangeToNextProvider();
			}
		}

		public string GetInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AdCacheSlot adCacheSlot in _adCacheSlots)
			{
				AbstractAdProvider currentProvider = GetCurrentProvider(adCacheSlot);
				stringBuilder.AppendLine("Loc: " + adCacheSlot.info.id + " Provider:" + ((currentProvider == null) ? "none" : currentProvider.id));
				stringBuilder.AppendLine("Cache status: " + adCacheSlot.IsCached());
			}
			return stringBuilder.ToString();
		}

		public void SetDebugPriorityListVersion(AdType adType, string version)
		{
			_settings.SetDebugPriorityListVersion(adType, version);
		}

		public bool IsCachedDebug(AdCacheSlotInfo adCacheSlotInfo)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			return adCacheSlotFromID != null && adCacheSlotFromID.IsCached();
		}

		public bool IsProviderCachedDebug(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			return adCacheSlotFromID != null && adCacheSlotFromID.HasProviderTierCachedAdDebug(adProviderInfo);
		}

		public bool IsProviderCachingDebug(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			return adCacheSlotFromID != null && adCacheSlotFromID.IsProviderTierCachingAdDebug(adProviderInfo);
		}

		public Dictionary<string, bool> GetDebugProviderListState(AdType adType)
		{
			if (_settings != null)
			{
				return _settings.GetDebugProviderListState(adType);
			}
			return null;
		}

		public List<AdProviderInfo> DebugGetProviderListForSlot(AdCacheSlotInfo adCacheSlotInfo)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				return adCacheSlotFromID.DebugGetProviderListForSlot(adCacheSlotInfo);
			}
			return null;
		}

		public void DebugShowAdOnSlot(AdCacheSlotInfo adCacheSlot)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlot.id);
			if (adCacheSlotFromID != null)
			{
				adCacheSlotFromID.Show(AdLocations.UNDETERMINED_LOCATION);
			}
		}

		public void DebugCacheAdOnSlot(AdCacheSlotInfo adCacheSlot)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlot.id);
			if (adCacheSlotFromID != null)
			{
				adCacheSlotFromID.Cache();
			}
		}

		public void DebugShowAdForProvider(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				adCacheSlotFromID.DebugShowAdOnProviderTier(adProviderInfo);
			}
		}

		public void DebugCacheAdForProvider(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				adCacheSlotFromID.DebugCacheAdOnProviderTier(adProviderInfo);
			}
		}

		public string DebugGetCacheIdForProvider(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				return adCacheSlotFromID.DebugGetCacheIdForProvider(adProviderInfo);
			}
			return string.Empty;
		}

		public string GetCurrentProviderIDForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
		{
			string result = string.Empty;
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				AbstractAdProvider currentProvider = GetCurrentProvider(adCacheSlotFromID);
				if (currentProvider != null)
				{
					result = currentProvider.id;
				}
			}
			return result;
		}

		public string GetCurrentProviderTierForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
		{
			string result = string.Empty;
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				result = adCacheSlotFromID.GetCurrentAdProviderTier();
			}
			return result;
		}

		public string GetHighestPriorityCachedProviderTierForSlot(AdCacheSlotInfo adCacheSlotInfo)
		{
			string result = string.Empty;
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(adCacheSlotInfo.id);
			if (adCacheSlotFromID != null)
			{
				result = adCacheSlotFromID.GetHighestPriorityCachedProviderTier();
			}
			return result;
		}

		public string GetNextProviderInWaterfall(AdLocationInfo adLocationInfo)
		{
			CacheList cacheList = _cacheLists[adLocationInfo.adType];
			string text = string.Empty;
			if (cacheList == null || cacheList.list == null)
			{
				return text;
			}
			AdCacheSlot adCacheSlot = cacheList.GetSlotToShow();
			if (adCacheSlot == null || !adCacheSlot.IsCached())
			{
				foreach (AdCacheSlot item in cacheList.list)
				{
					if (item.IsCached())
					{
						adCacheSlot = item;
					}
				}
			}
			if (adCacheSlot != null)
			{
				text = GetHighestPriorityCachedProviderTierForSlot(adCacheSlot.info);
				if (text.IsNullOrEmpty())
				{
					text = adCacheSlot.GetCurrentAdProviderTier();
				}
			}
			return text;
		}

		public uint GetGracePeriodValue()
		{
			return _settings.GetGracePeriodValue();
		}

		public bool GetAvailabilityForLocation(string locationID)
		{
			AdLocationInfo adLocationInfo = AdLocations.adLocations.FirstOrDefault((AdLocationInfo locationInfo) => locationInfo.id == locationID);
			if (adLocationInfo == null)
			{
				return false;
			}
			return _settings.GetAvailabilityForLocation(locationID);
		}

		public bool GetAvailabilityForAdCacheSlot(string cacheSlotId)
		{
			AdCacheSlotInfo adCacheSlotInfo = AdCacheSlots.adCacheSlots.FirstOrDefault((AdCacheSlotInfo cacheSlotInfo) => cacheSlotInfo.id == cacheSlotId);
			if (adCacheSlotInfo == null)
			{
				return false;
			}
			return _settings.GetAvailabilityForCacheSlot(cacheSlotId);
		}

		public bool AreInterstitialsAllowed()
		{
			return _settings.AreInterstitialsAllowed();
		}

		public bool HasPriorityListWrapped(string cacheSlotId)
		{
			AdCacheSlot adCacheSlotFromID = GetAdCacheSlotFromID(cacheSlotId);
			return adCacheSlotFromID != null && _idToPriorityList[adCacheSlotFromID.waterfallID].wrapped;
		}

		private bool HasPriorityListChanged(AdCacheSlot adCacheSlot)
		{
			string latestVersionForWaterfall = _settings.GetLatestVersionForWaterfall(adCacheSlot);
			return _interstitialStats.HasVersionChangedForList(adCacheSlot.waterfallID, latestVersionForWaterfall);
		}

		private void UpdatePriorityListForID(string waterfallID, AdType adType, AdCacheSlotInfo adCacheSlotInfo)
		{
			if (_idToPriorityList.ContainsKey(waterfallID))
			{
				_idToPriorityList[waterfallID] = CreatePriorityListForID(waterfallID, adType, _settings, adCacheSlotInfo);
			}
		}

		private PriorityList CreatePriorityListForID(string waterfallID, AdType adType, IAdSettingsProvider settings, AdCacheSlotInfo adCacheSlotInfo)
		{
			PriorityList list = new PriorityList(waterfallID, adType, settings, adCacheSlotInfo);
			list.changedProvider += () =>
			{
				foreach (IAdEventListener eventListener in _eventListeners)
				{
					eventListener.ChangedProvider(adType, list.currentProvider, waterfallID);
				}
			};
			list.resetToTop += () =>
			{
				foreach (IAdEventListener eventListener2 in _eventListeners)
				{
					eventListener2.PriorityListWrapped(waterfallID);
				}
			};
			return list;
		}

		[CanBeNull]
		public string GetWaterfallID(AdCacheSlotInfo adCacheSlot)
		{
			string waterfallID = _settings.GetWaterfallID(adCacheSlot.id);
			if (!string.IsNullOrEmpty(waterfallID))
			{
				return waterfallID;
			}
			return _settings.GetWaterfallID(adCacheSlot.adType);
		}

		public List<AdProviderInfo> GetProvidersForCacheSlot(AdCacheSlot adCacheSlot)
		{
			PriorityList value;
			if (_idToPriorityList.TryGetValue(adCacheSlot.waterfallID, out value))
			{
				return value.GetProviderList();
			}
			return null;
		}

		[CanBeNull]
		private AbstractAdProvider GetCurrentProvider(AdCacheSlot adCacheSlot)
		{
			PriorityList value;
			if (!_idToPriorityList.TryGetValue(adCacheSlot.waterfallID, out value) || !value.hasProviders)
			{
				return null;
			}
			value.CheckIfProviderExpired();
			return AbstractAdProvider.GetAdProvider(value.currentProviderID);
		}

		[CanBeNull]
		private AdProviderInfo GetCurrentProviderInfo(AdCacheSlot adCacheSlot)
		{
			PriorityList value;
			if (!_idToPriorityList.TryGetValue(adCacheSlot.waterfallID, out value) || !value.hasProviders)
			{
				return null;
			}
			value.CheckIfProviderExpired();
			return value.currentProvider;
		}

		[CanBeNull]
		public AdCacheSlot GetAdCacheSlotFromID(string adCacheSlotId)
		{
			AdCacheSlot adCacheSlot = _adCacheSlots.FirstOrDefault((AdCacheSlot l) => l.info.id == adCacheSlotId);
			if (adCacheSlot == null)
			{
				AdSystemLog.LogError("Could not find an ad cacheSlot with the specified ID: " + adCacheSlotId + " | " + Environment.StackTrace);
			}
			return adCacheSlot;
		}
	}
}
