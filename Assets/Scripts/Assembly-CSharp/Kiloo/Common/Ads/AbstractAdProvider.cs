using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace Kiloo.Common.Ads
{
	public abstract class AbstractAdProvider : AdEventListenerAdapter
	{
		private enum InitState
		{
			Uninitialized = 0,
			Initializing = 1,
			Initialized = 2,
			Unsupported = 3
		}

		protected class EventListenerInvoker : IAdEventListener
		{
			private readonly List<IAdEventListener> _listeners = new List<IAdEventListener>();

			public void AddToStartOfList(IAdEventListener listener)
			{
				_listeners.Insert(0, listener);
			}

			public void AddListener(IAdEventListener listener)
			{
				_listeners.Add(listener);
			}

			public void RemoveListener(IAdEventListener listener)
			{
				_listeners.Remove(listener);
			}

			public void Clear()
			{
				_listeners.Clear();
			}

			public void WillShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.WillShowAd(adType, providerID, providerTier, adCacheSlotInfo, adLocationInfo);
				}
			}

			public void ShowedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.ShowedAd(adType, providerID, providerTier, adCacheSlotInfo, adLocationInfo);
				}
			}

			public void CachedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.CachedAd(adType, providerID, providerTier, adCacheSlotInfo, wasAlreadyCached);
				}
			}

			public void DismissedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.DismissedAd(adType, providerID, providerTier, adCacheSlotInfo, adLocationInfo);
				}
			}

			public void StartedCachingAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.StartedCachingAd(adType, providerID, providerTier, adCacheSlotInfo);
				}
			}

			public void FinishedWatchingVideo(string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.FinishedWatchingVideo(providerID, providerTier, adCacheSlotInfo, adLocationInfo);
				}
			}

			public void FailedToCacheAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.FailedToCacheAd(adType, providerID, providerTier, adCacheSlotInfo);
				}
			}

			public void FailedToShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.FailedToShowAd(adType, providerID, providerTier, adCacheSlotInfo, adLocationInfo);
				}
			}

			public void FailedToInitAdProvider(string providerID)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.FailedToInitAdProvider(providerID);
				}
			}

			public void CachingTimedOut(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.CachingTimedOut(adType, providerID, providerTier, adCacheSlotInfo);
				}
			}

			public void ClickedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
			{
				foreach (IAdEventListener item in CloneListeners())
				{
					item.ClickedAd(adType, providerID, providerTier, adCacheSlotInfo, adLocationInfo);
				}
			}

			private List<IAdEventListener> CloneListeners()
			{
				return new List<IAdEventListener>(_listeners);
			}

			public void ReachedDailyInterstitialLimit(int limit)
			{
				throw new InvalidOperationException("This event should not be invoked by the ad provider!");
			}

			public void ReachedHourlyInterstitialLimit(int limit)
			{
				throw new InvalidOperationException("This event should not be invoked by the ad provider!");
			}

			public void ReachedNewUserInterstitialLimit(int limit)
			{
				throw new InvalidOperationException("This event should not be invoked by the ad provider!");
			}

			public void ReachedTimeBetweenAdsInterstitialLimit(int limit)
			{
				throw new InvalidOperationException("This event should not be invoked by the ad provider!");
			}

			public void ChangedProvider(AdType adType, AdProviderInfo newProviderInfo, string changedWaterfallID)
			{
				throw new InvalidOperationException("This event should not be invoked by the ad provider!");
			}

			public void PriorityListWrapped(string waterfallID)
			{
				throw new InvalidOperationException("This event should not be invoked by the ad provider!");
			}
		}

		protected static readonly EventListenerInvoker eventInvoker = new EventListenerInvoker();

		protected static readonly AdCacheSlotInfo DEFAULT_SHOW_SLOT = AdCacheSlots.UNDETERMINED_SLOT;

		protected static readonly AdLocationInfo DEFAULT_SHOW_LOCATION = AdLocations.UNDETERMINED_LOCATION;

		protected AdCacheSlotInfo _showInterstitialCacheSlot = DEFAULT_SHOW_SLOT;

		protected AdCacheSlotInfo _showVideoCacheSlot = DEFAULT_SHOW_SLOT;

		protected AdLocationInfo _showInterstitialAdLocation = DEFAULT_SHOW_LOCATION;

		protected AdLocationInfo _showVideoAdLocation = DEFAULT_SHOW_LOCATION;

		protected AdCacheSlotInfo _showCacheSlot = DEFAULT_SHOW_SLOT;

		protected AdLocationInfo _showAdLocation = DEFAULT_SHOW_LOCATION;

		protected readonly Dictionary<string, AdCacheSlotInfo> _cacheIDToSlot = new Dictionary<string, AdCacheSlotInfo>();

		protected readonly Dictionary<string, string> _cacheIDToTier = new Dictionary<string, string>();

		protected readonly Dictionary<AdCacheSlotInfo, string> _cacheSlotToTier = new Dictionary<AdCacheSlotInfo, string>();

		private static readonly Dictionary<string, AbstractAdProvider> _idToAbstractAdProvider = new Dictionary<string, AbstractAdProvider>();

		private static readonly Dictionary<string, AbstractAdProvider> _idToDebugAdProvider = new Dictionary<string, AbstractAdProvider>();

		private InitState _initState;

		private bool? _isLoadedAdConstantsRestricted;

		private static IAdSettingsProvider _settingsProvider;

		private Dictionary<AdType, bool> _hasCheckedPlatformSupport = new Dictionary<AdType, bool>
		{
			{
				AdType.Interstitial,
				false
			},
			{
				AdType.Video,
				false
			}
		};

		protected readonly Dictionary<AdType, bool> _supportedAdTypes = new Dictionary<AdType, bool>();

		private AdType[] _allAdTypes = new AdType[2]
		{
			AdType.Interstitial,
			AdType.Video
		};

		private static Dictionary<string, AbstractAdProvider> idToAdProvider
		{
			get
			{
				if (Globals.DEBUG_USE_DEBUG_AD_PROVIDER)
				{
					return _idToDebugAdProvider;
				}
				return _idToAbstractAdProvider;
			}
		}

		public abstract string id { get; }

		public abstract bool supportsMulticaching { get; }

		public virtual List<AdType> adTypesWithClickCallbackSupport
		{
			get
			{
				return new List<AdType>();
			}
		}

		public bool isCaching { get; private set; }

		public bool isInitializing
		{
			get
			{
				return _initState == InitState.Initializing;
			}
		}

		public bool isInitialized
		{
			get
			{
				return _initState == InitState.Initialized;
			}
		}

		protected abstract void CheckPlatformSupport(AdProviderConfiguration configuration);

		private void Initialize(AdProviderConfiguration configuration)
		{
			if (_initState == InitState.Uninitialized)
			{
				if (!IsAnyTypeSupportedOnPlatform())
				{
					_initState = InitState.Unsupported;
					eventInvoker.FailedToInitAdProvider(id);
				}
				else
				{
					_initState = InitState.Initializing;
					_isLoadedAdConstantsRestricted = configuration.isRestricted;
					OnInitialize(configuration);
				}
			}
		}

		public void SetCurrentTier(AdCacheSlotInfo adCacheSlotInfo, string providerTier)
		{
			_cacheSlotToTier[adCacheSlotInfo] = providerTier;
		}

		public string GetCurrentTier(AdCacheSlotInfo adCacheSlotInfo)
		{
			if (adCacheSlotInfo != null && _cacheSlotToTier.ContainsKey(adCacheSlotInfo))
			{
				return _cacheSlotToTier[adCacheSlotInfo];
			}
			return string.Empty;
		}

		public bool IsSupportedOnPlatform(AdType adType)
		{
			if (!_hasCheckedPlatformSupport[adType])
			{
				CheckPlatformSupport(_settingsProvider.GetConfigurationForProvider(id));
				CheckHasCacheIDs(_settingsProvider.GetAllCacheIDsForProvider(id, adType), adType);
				CheckOnlineSettingRestriction();
				_hasCheckedPlatformSupport[adType] = true;
			}
			bool value;
			return _supportedAdTypes.TryGetValue(adType, out value) && value;
		}

		private void CheckHasCacheIDs(Dictionary<AdCacheSlotInfo, List<string>> cacheIDs, AdType adType)
		{
			bool flag = false;
			foreach (KeyValuePair<AdCacheSlotInfo, List<string>> cacheID in cacheIDs)
			{
				if (cacheID.Value == null || cacheID.Value.Count <= 0)
				{
					continue;
				}
				foreach (string item in cacheID.Value)
				{
					if (!string.IsNullOrEmpty(item))
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				_supportedAdTypes[adType] = false;
			}
		}

		private void CheckOnlineSettingRestriction()
		{
			AdType[] allAdTypes = _allAdTypes;
			foreach (AdType adType in allAdTypes)
			{
				bool value;
				string supportedDeviceVersion;
				if (!_supportedAdTypes.TryGetValue(adType, out value) || !value || !_settingsProvider.TryGetProviderOverrideSupportedOnPlatform(adType, id, out supportedDeviceVersion))
				{
					continue;
				}
				string oSVersion = DeviceUtility.GetOSVersion();
				try
				{
					if (global::Utility.CompareVersions(oSVersion, supportedDeviceVersion) < 0)
					{
						_supportedAdTypes[adType] = false;
					}
				}
				catch (FormatException ex)
				{
					AdSystemLog.LogError("CheckOnlineSettingRestriction: Failed to parse versions for comparison: " + oSVersion + " and " + supportedDeviceVersion + "  : " + ex);
				}
			}
		}

		private bool IsAnyTypeSupportedOnPlatform()
		{
			AdType[] allAdTypes = _allAdTypes;
			foreach (AdType adType in allAdTypes)
			{
				if (IsSupportedOnPlatform(adType))
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnInitialize(AdProviderConfiguration configuration)
		{
		}

		protected void FinishInitialization(bool success)
		{
			if (success)
			{
				_initState = InitState.Initialized;
				eventInvoker.AddToStartOfList(this);
			}
			else
			{
				_initState = InitState.Uninitialized;
				eventInvoker.FailedToInitAdProvider(id);
			}
		}

		public static void ClearStaticCollections()
		{
			idToAdProvider.Clear();
			eventInvoker.Clear();
		}

		public static void MapAdProviders(IAdSettingsProvider settingsProvider)
		{
			_settingsProvider = settingsProvider;
			if (_idToDebugAdProvider.Count <= 0)
			{
				MapDebugAdProvider();
			}
			if (_idToAbstractAdProvider.Count <= 0)
			{
				MapAbstractAdProviders(settingsProvider);
			}
		}

		private static void MapDebugAdProvider()
		{
			foreach (Type abstractAdProviderSubType in GetAbstractAdProviderSubTypes())
			{
				AbstractAdProvider abstractAdProvider = (AbstractAdProvider)Activator.CreateInstance(abstractAdProviderSubType);
				DebugAdProvider value = new DebugAdProvider(abstractAdProvider.id, abstractAdProvider.supportsMulticaching);
				_idToDebugAdProvider.Add(abstractAdProvider.id, value);
			}
		}

		private static void MapAbstractAdProviders(IAdSettingsProvider settingsProvider)
		{
			string[] adProvidersToInitialize = settingsProvider.GetAdProvidersToInitialize();
			foreach (Type abstractAdProviderSubType in GetAbstractAdProviderSubTypes())
			{
				AbstractAdProvider abstractAdProvider = (AbstractAdProvider)Activator.CreateInstance(abstractAdProviderSubType);
				if (adProvidersToInitialize.Contains(abstractAdProvider.id))
				{
					AdProviderConfiguration configurationForProvider = settingsProvider.GetConfigurationForProvider(abstractAdProvider.id);
					abstractAdProvider.Initialize(configurationForProvider);
				}
				_idToAbstractAdProvider.Add(abstractAdProvider.id, abstractAdProvider);
			}
		}

		private static IEnumerable<Type> GetAbstractAdProviderSubTypes()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				IEnumerable<Type> subTypes = from t in assembly.GetTypes()
					where t.IsSubclassOf(typeof(AbstractAdProvider))
					select t;
				foreach (Type subType in subTypes)
				{
					if (subType != typeof(DebugAdProvider))
					{
						yield return subType;
					}
				}
			}
		}

		public static void HandleAdConstantsChanged()
		{
			foreach (KeyValuePair<string, AbstractAdProvider> item in idToAdProvider)
			{
				if (item.Value.isInitialized || item.Value.isInitializing)
				{
					AdProviderConfiguration configurationForProvider = _settingsProvider.GetConfigurationForProvider(item.Key);
					bool? isLoadedAdConstantsRestricted = item.Value._isLoadedAdConstantsRestricted;
					if (isLoadedAdConstantsRestricted != true && isLoadedAdConstantsRestricted.HasValue && configurationForProvider.isRestricted)
					{
						item.Value._initState = InitState.Unsupported;
					}
				}
			}
		}

		public static bool IsProviderSupportedOnPlatform(string id, AdType adType)
		{
			AbstractAdProvider value;
			if (idToAdProvider.TryGetValue(id, out value))
			{
				return value.IsSupportedOnPlatform(adType);
			}
			AdSystemLog.LogError("Unknown provider ID " + id);
			return false;
		}

		protected List<string> GetAllCacheIDsAsList(AdType adType)
		{
			if (_settingsProvider == null)
			{
				return new List<string>();
			}
			Dictionary<AdCacheSlotInfo, List<string>> allCacheIDsForProvider = _settingsProvider.GetAllCacheIDsForProvider(id, adType);
			List<string> list = new List<string>();
			if (allCacheIDsForProvider == null || allCacheIDsForProvider.Count == 0)
			{
				return new List<string>();
			}
			foreach (List<string> value in allCacheIDsForProvider.Values)
			{
				if (value != null && value.Count != 0)
				{
					list.AddRange(value);
				}
			}
			return list;
		}

		public static AbstractAdProvider GetAdProvider(string id)
		{
			AbstractAdProvider abstractAdProvider;
			if (idToAdProvider.ContainsKey(id))
			{
				abstractAdProvider = idToAdProvider[id];
				AdProviderConfiguration configurationForProvider = _settingsProvider.GetConfigurationForProvider(id);
				abstractAdProvider.Initialize(configurationForProvider);
			}
			else if (Application.isEditor)
			{
				abstractAdProvider = new DebugAdProvider(id, false);
				abstractAdProvider.Initialize(new AdProviderConfiguration());
				idToAdProvider[id] = abstractAdProvider;
			}
			else
			{
				AdSystemLog.LogError("Unknown provider ID " + id);
				abstractAdProvider = null;
			}
			return abstractAdProvider;
		}

		public static void AddEventListener(IAdEventListener listener)
		{
			eventInvoker.AddListener(listener);
		}

		public static void RemoveEventListener(IAdEventListener listener)
		{
			eventInvoker.RemoveListener(listener);
		}

		public bool HasCachedAd(AdType adType, string cacheID)
		{
			if (!isInitialized || string.IsNullOrEmpty(cacheID))
			{
				return false;
			}
			return (adType != AdType.Video) ? HasCachedInterstitial(cacheID) : HasCachedVideo(cacheID);
		}

		public string GetTierForCacheId(string cacheID)
		{
			if (_cacheIDToTier.ContainsKey(cacheID))
			{
				return _cacheIDToTier[cacheID];
			}
			return string.Empty;
		}

		public bool isCachingSlot(AdCacheSlotInfo info)
		{
			return isCaching && _cacheIDToSlot.ContainsValue(info);
		}

		public void CacheAd(AdType adType, AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			if (!isInitialized || string.IsNullOrEmpty(cacheID))
			{
				AdSystemLog.LogError("Unable to cache ad, provider is not initialized or cacheID is null");
				eventInvoker.FailedToCacheAd(adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			}
			else
			{
				if (!supportsMulticaching && isCaching)
				{
					return;
				}
				if (HasCachedAd(adType, cacheID))
				{
					eventInvoker.CachedAd(adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo, true);
					return;
				}
				isCaching = true;
				_cacheIDToTier[cacheID] = GetCurrentTier(adCacheSlotInfo);
				if (adType == AdType.Video)
				{
					CacheVideo(adCacheSlotInfo, cacheID);
				}
				else
				{
					CacheInterstitial(adCacheSlotInfo, cacheID);
				}
			}
		}

		public bool ShowAd(AdType adType, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			if (!isInitialized || string.IsNullOrEmpty(cacheID))
			{
				AdSystemLog.LogError("Unable to show ad, provider is not initialized or cacheID is null");
				eventInvoker.FailedToShowAd(adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo, adLocationInfo);
				return false;
			}
			if (!HasCachedAd(adType, cacheID))
			{
				eventInvoker.FailedToShowAd(adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo, adLocationInfo);
				return false;
			}
			eventInvoker.WillShowAd(adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo, adLocationInfo);
			if (adType == AdType.Video)
			{
				ShowVideo(adCacheSlotInfo, adLocationInfo, cacheID);
			}
			else
			{
				ShowInterstitial(adCacheSlotInfo, adLocationInfo, cacheID);
			}
			return true;
		}

		public void TimeoutCaching(AdType adType, AdCacheSlotInfo adCacheSlotInfo)
		{
			eventInvoker.CachingTimedOut(adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
			OnCacheTimeout(adType, adCacheSlotInfo);
		}

		public void FailCaching(AdType adType, AdCacheSlotInfo adCacheSlotInfo)
		{
			eventInvoker.FailedToCacheAd(adType, id, GetCurrentTier(adCacheSlotInfo), adCacheSlotInfo);
		}

		protected virtual void OnCacheTimeout(AdType adType, AdCacheSlotInfo adCacheSlotInfo)
		{
		}

		protected virtual void ShowInterstitial(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocactionInfo, string cacheID)
		{
			throw new NotSupportedException("Ad provider " + id + ", does not support interstitials");
		}

		protected virtual void CacheInterstitial(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			throw new NotSupportedException("Ad provider " + id + ", does not support interstitials");
		}

		protected virtual void ShowVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, string cacheID)
		{
			throw new NotSupportedException("Ad provider " + id + ", does not support videos");
		}

		protected virtual void CacheVideo(AdCacheSlotInfo adCacheSlotInfo, string cacheID)
		{
			throw new NotSupportedException("Ad provider " + id + ", does not support videos");
		}

		protected virtual bool HasCachedVideo(string cacheID)
		{
			throw new NotSupportedException("Ad provider " + id + ", does not support videos");
		}

		protected virtual bool HasCachedInterstitial(string cacheID)
		{
			throw new NotSupportedException("Ad provider " + id + ", does not support interstitials");
		}

		public override void CachedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
		{
			OnFinishedCaching(providerID);
		}

		public override void FailedToCacheAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
		{
			OnFinishedCaching(providerID);
		}

		public override void CachingTimedOut(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
		{
			OnFinishedCaching(providerID);
		}

		private void OnFinishedCaching(string providerID)
		{
			if (providerID == id)
			{
				isCaching = false;
			}
		}
	}
}
