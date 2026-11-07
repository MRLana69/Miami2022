using System;
using System.Collections.Generic;
using System.Linq;
using AdHandling.AdConstants;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using UnityEngine;

public class AdSettingsProvider : IAdSettingsProvider
{
	private const char PRIORITY_LIST_PROVIDER_SEPARATOR = ',';

	private const char PRIORITY_LIST_PROVIDER_COMPONENTS_SEPARATOR = ':';

	private const char PRIORITY_LIST_PROVIDER_PRIORITY_WEIGHT_SEPARATOR = '_';

	private const char PRIORITY_LIST_PROVIDER_TIER_SEPERATOR = '_';

	private const char CACHE_ID_LIST_LINE_SEPERATOR = ',';

	private const char CACHE_ID_LIST_NAME_SEPERATOR = ':';

	private const char CACHE_ID_LIST_SLOT_ID_SEPERATOR = ';';

	private const string INTERSTITIAL_GRACE_PERIOD_KEY = "ads_grace_period";

	private const string NUMBER_OF_FAILS_FOR_EACH_PROVIDER = "number_of_fails_for_interstitial";

	private const string GLOBAL_TIME_BETWEEN_INTERSTITIALS_KEY = "global_time_between_interstitials";

	private const string LIMIT_PER_HOUR = "interstitials_limit_per_hour";

	private const string LIMIT_PER_DAY = "interstitials_limit_per_day";

	private const string LOCK_ON_TIME_LIMIT_KEY = "lock_on_time_limit_{0}";

	private const string CACHE_TIMEOUT_LIMIT_ADTYPE_KEY = "cache_timeout_limit_{0}";

	private const string CACHE_TIMEOUT_LIMIT_PROVIDER_KEY = "cache_timeout_limit_{0}_{1}";

	private const string AD_LOCATION_TIME_LIMIT_KEY = "ad_location_{0}_time_between_interstitials";

	private const string AD_LOCATION_AVAILABILITY_KEY = "ad_location_{0}_enabled";

	private const string AD_CACHE_SLOT_AVAILABILITY_KEY = "ad_cache_slot_{0}_enabled";

	private const string AD_LOCATION_WAIT_WHILE_CACHING_TIME_LIMIT_KEY = "ad_location_{0}_{1}_wait_caching_limit";

	private const string PRIORITY_LIST_VERSION_KEY = "prio_list_{0}_version";

	private const string PROVIDER_MINIMUM_DEVICE_VERSION_KEY = "provider_{0}_{1}_minimum_device_version";

	private const string WATERFALL_LOCATION_LIST_KEY = "{0}_list";

	private const string WATERFALL_ADTYPE_LIST_KEY = "default_{0}_list";

	private const string INTERSTITIAL_PROVIDER_CACHE_IDS = "{0}_interstitial_cache_ids";

	private const string INTERSTITIAL_PROVIDER_CACHE_IDS_RESTRICTED = "{0}_interstitial_cache_ids_restricted";

	private const string VIDEO_PROVIDER_CACHE_IDS = "{0}_video_cache_ids";

	private const string VIDEO_PROVIDER_CACHE_IDS_RESTRICTED = "{0}_video_cache_ids_restricted";

	private const uint DEFAULT_INTERSTITIAL_GRACE_PERIOD = 72000u;

	private const int DEFAULT_NUMBER_OF_FAILS_FOR_PROVIDER = 1;

	private const int DEFAULT_GLOBAL_TIME_BETWEEN_INTERSTITIALS = 10;

	private const int DEFAULT_LIMIT_PER_HOUR = 5;

	private const int DEFAULT_LIMIT_PER_DAY = 20;

	private const int DEFAULT_LOCK_ON_TIME_LIMIT_INTERSTITIAL = 72000;

	private const int DEFAULT_LOCK_ON_TIME_LIMIT_VIDEO = 72000;

	private const int DEFAULT_CACHE_TIMEOUT_LIMIT = 10;

	private const bool DEFAULT_AD_LOCATION_AVAILABILITY = false;

	private const bool DEFAULT_CACHE_SLOT_AVAILABILITY = true;

	private const int DEFAULT_AD_LOCATION_TIME_LIMIT = 10;

	private const int DEFAULT_AD_LOCATION_WAIT_WHILE_CACHING_TIME_LIMIT = 10;

	private const string DEFAULT_PRIORITY_LIST_VERSION = "notset";

	private Dictionary<AdType, string> _debugPriorityListVersion = new Dictionary<AdType, string>();

	private static List<AdProviderInfo> _allInterstitialProviders = new List<AdProviderInfo>
	{
		new AdProviderInfo
		{
			providerID = "supersonic",
			priority = 1,
			weight = 100f,
			providerTier = "supersonic_default"
		},
		new AdProviderInfo
		{
			providerID = "inmobi",
			priority = 2,
			weight = 100f,
			providerTier = "inmobi_default"
		},
		new AdProviderInfo
		{
			providerID = "chartboost",
			priority = 3,
			weight = 100f,
			providerTier = "chartboost_default"
		},
		new AdProviderInfo
		{
			providerID = "facebook",
			priority = 4,
			weight = 100f,
			providerTier = "facebook_default"
		},
		new AdProviderInfo
		{
			providerID = "mobvista",
			priority = 6,
			weight = 100f,
			providerTier = "mobvista_default"
		},
		new AdProviderInfo
		{
			providerID = "superawesome",
			priority = 7,
			weight = 100f,
			providerTier = "superawesome_default"
		}
	};

	private static List<AdProviderInfo> _allVideoProviders = new List<AdProviderInfo>
	{
		new AdProviderInfo
		{
			providerID = "supersonic",
			priority = 1,
			weight = 100f,
			providerTier = "supersonic_default"
		},
		new AdProviderInfo
		{
			providerID = "adcolony",
			priority = 2,
			weight = 100f,
			providerTier = "adcolony_default"
		},
		new AdProviderInfo
		{
			providerID = "tapjoy",
			priority = 3,
			weight = 100f,
			providerTier = "tapjoy_default"
		},
		new AdProviderInfo
		{
			providerID = "inmobi",
			priority = 4,
			weight = 100f,
			providerTier = "inmobi_default"
		},
		new AdProviderInfo
		{
			providerID = "vungle",
			priority = 5,
			weight = 100f,
			providerTier = "vungle_default"
		},
		new AdProviderInfo
		{
			providerID = "chartboost",
			priority = 6,
			weight = 100f,
			providerTier = "chartboost_default"
		},
		new AdProviderInfo
		{
			providerID = "facebook",
			priority = 7,
			weight = 100f,
			providerTier = "facebook_default"
		},
		new AdProviderInfo
		{
			providerID = "mobvista",
			priority = 8,
			weight = 100f,
			providerTier = "mobvista_default"
		},
		new AdProviderInfo
		{
			providerID = "superawesome",
			priority = 9,
			weight = 100f,
			providerTier = "superawesome_default"
		}
	};

	private static List<string> _interstitialProviderIDs = new List<string>();

	private static List<string> _videoProviderIDs = new List<string>();

	private Dictionary<string, Dictionary<AdCacheSlotInfo, string>> _interstitialCacheIDs = new Dictionary<string, Dictionary<AdCacheSlotInfo, string>>();

	private Dictionary<string, Dictionary<AdCacheSlotInfo, string>> _videoCacheIDs = new Dictionary<string, Dictionary<AdCacheSlotInfo, string>>();

	private readonly Dictionary<string, bool> _debugVideoProviderListState = new Dictionary<string, bool>();

	private readonly Dictionary<string, bool> _debugInterstitialProviderListState = new Dictionary<string, bool>();

	private static readonly Dictionary<string, AdType> _mappedAdTypesForLocations = new Dictionary<string, AdType>();

	public virtual IList<AdProviderInfo> GetPriorityList(string waterfallID, AdType adType, AdCacheSlotInfo adCacheSlotInfo)
	{
		if ((Globals.DEBUG_INTERSTITIAL_PROVIDERS && adType == AdType.Interstitial) || (Globals.DEBUG_VIDEOADS_PROVIDERS && adType == AdType.Video))
		{
			return GetDebugPriorityList(adType);
		}
		CacheProviderIDs();
		List<AdProviderInfo> list = new List<AdProviderInfo>();
		if (!TryGettingCacheIdDefinitionsFromOnlineSettings(adType))
		{
			AdSystemLog.LogError("Failed to get cache id definitions when creating priority list for waterfall " + waterfallID);
			return list;
		}
		string valueString;
		if (!OnlineSettings.instance.TryGetValue(waterfallID, out valueString))
		{
			waterfallID = GetWaterfallID(adType);
			OnlineSettings.instance.TryGetValue(waterfallID, out valueString);
		}
		if (string.IsNullOrEmpty(valueString))
		{
			AdSystemLog.LogError("Priority list ID: " + waterfallID + " was not found in online settings!");
			return list;
		}
		valueString = valueString.Replace("\r\n", string.Empty).Replace("\n", string.Empty);
		string[] array = valueString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length <= 0)
		{
			AdSystemLog.LogError("Priority list ID: " + waterfallID + " has no content!");
			return list;
		}
		Dictionary<string, Dictionary<AdCacheSlotInfo, string>> cacheIdDefinitionsForAdType = GetCacheIdDefinitionsForAdType(adType);
		List<string> list2 = ((adType != AdType.Interstitial) ? _videoProviderIDs : _interstitialProviderIDs);
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			if (array3.Length < 2)
			{
				AdSystemLog.LogError("The provider information: '" + array3.ToContentString() + "' is not formatted correctly in online settings");
				continue;
			}
			string text2 = array3[0];
			if (!cacheIdDefinitionsForAdType.ContainsKey(text2))
			{
				AdSystemLog.LogWarning("The provider tier: " + text2 + " in the list: " + waterfallID + " does not have a cache id definition");
				continue;
			}
			if (!cacheIdDefinitionsForAdType[text2].ContainsKey(adCacheSlotInfo))
			{
				AdSystemLog.LogWarning("The provider tier: " + text2 + " in the list: " + waterfallID + " does not have a cache id defined for Ad cache slot : " + adCacheSlotInfo.id);
				continue;
			}
			string[] array4 = array3[0].Split('_');
			string text3 = array4[0];
			if (!list2.Contains(text3))
			{
				AdSystemLog.LogError("The provider with ID: " + text3 + " in the list: " + waterfallID + " is not registered as a valid provider for ad type: " + adType);
				continue;
			}
			string[] array5 = array3[1].Split('_');
			int result;
			int result2;
			if (array5.Length < 2)
			{
				AdSystemLog.LogError("The priority or weight of the provider: " + text3 + " in the list: " + waterfallID + " is not formatted correctly!");
			}
			else if (!int.TryParse(array5[0], out result) || !int.TryParse(array5[1], out result2))
			{
				AdSystemLog.LogError("The priority or weight of the provider: " + text3 + " in the list: " + waterfallID + " is not the expected data type!");
			}
			else if (!AbstractAdProvider.IsProviderSupportedOnPlatform(text3, adType))
			{
				AdSystemLog.LogError(string.Concat("The provider with ID: ", text3, " and adType: ", adType, " is not supported on this platform."));
			}
			else
			{
				AdProviderInfo adProviderInfo = new AdProviderInfo();
				adProviderInfo.providerID = text3;
				adProviderInfo.priority = result;
				adProviderInfo.weight = result2;
				adProviderInfo.providerTier = text2;
				AdProviderInfo item = adProviderInfo;
				list.Add(item);
			}
		}
		return list;
	}

	private bool TryGettingCacheIdDefinitionsFromOnlineSettings(AdType adType)
	{
		string format = ((!AgeRestriction.isRestricted) ? "{0}_interstitial_cache_ids" : "{0}_interstitial_cache_ids_restricted");
		string format2 = ((!AgeRestriction.isRestricted) ? "{0}_video_cache_ids" : "{0}_video_cache_ids_restricted");
		CacheProviderIDs();
		if (adType == AdType.Interstitial)
		{
			_interstitialCacheIDs = new Dictionary<string, Dictionary<AdCacheSlotInfo, string>>();
			foreach (string interstitialProviderID in _interstitialProviderIDs)
			{
				string onlineSettingsKey = string.Format(format, interstitialProviderID);
				ParseCacheIdDefinition(onlineSettingsKey, _interstitialCacheIDs, AdType.Interstitial);
				if (!IsCacheIdsLoadedForProviderWithAdType(interstitialProviderID, AdType.Interstitial))
				{
					TryGettingCacheIdsWithMobileDeviceTypeKey(onlineSettingsKey);
				}
			}
		}
		else
		{
			_videoCacheIDs = new Dictionary<string, Dictionary<AdCacheSlotInfo, string>>();
			foreach (string videoProviderID in _videoProviderIDs)
			{
				string onlineSettingsKey2 = string.Format(format2, videoProviderID);
				ParseCacheIdDefinition(onlineSettingsKey2, _videoCacheIDs, AdType.Video);
			}
		}
		return HasCacheIdDefinitions(adType);
	}

	private bool IsCacheIdsLoadedForProviderWithAdType(string providerId, AdType adType)
	{
		Dictionary<string, Dictionary<AdCacheSlotInfo, string>> cacheIdDefinitionsForAdType = GetCacheIdDefinitionsForAdType(adType);
		return cacheIdDefinitionsForAdType.ContainsKey(providerId) && cacheIdDefinitionsForAdType[providerId].Count > 0;
	}

	private void TryGettingCacheIdsWithMobileDeviceTypeKey(string onlineSettingsKey)
	{
		string arg = DeviceInfo.Instance.GetMobileDeviceType().ToLowerString();
		string onlineSettingsKey2 = string.Format("{0}_{1}", onlineSettingsKey, arg);
		ParseCacheIdDefinition(onlineSettingsKey2, _interstitialCacheIDs, AdType.Interstitial);
	}

	private bool HasCacheIdDefinitions(AdType adType)
	{
		Dictionary<string, Dictionary<AdCacheSlotInfo, string>> cacheIdDefinitionsForAdType = GetCacheIdDefinitionsForAdType(adType);
		return cacheIdDefinitionsForAdType != null && cacheIdDefinitionsForAdType.Count > 0;
	}

	private Dictionary<string, Dictionary<AdCacheSlotInfo, string>> GetCacheIdDefinitionsForAdType(AdType adType)
	{
		return (adType != AdType.Interstitial) ? _videoCacheIDs : _interstitialCacheIDs;
	}

	private static void ParseCacheIdDefinition(string onlineSettingsKey, IDictionary<string, Dictionary<AdCacheSlotInfo, string>> cacheIdDictionary, AdType adType)
	{
		string valueString;
		if (!OnlineSettings.instance.TryGetValue(onlineSettingsKey, out valueString))
		{
			AdSystemLog.LogWarning("Error getting online settings for CacheID definitions for " + onlineSettingsKey);
			return;
		}
		valueString = valueString.Replace("\r\n", string.Empty).Replace("\n", string.Empty);
		string[] array = valueString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length <= 0)
		{
			AdSystemLog.LogError("Error parsing online settings for cache id definition for key: " + onlineSettingsKey + " - no content found");
			return;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			if (array3.Length < 2)
			{
				AdSystemLog.LogError("Error while parsing cache Id list for key " + onlineSettingsKey + " invalid line content : " + array3.ToContentString());
				continue;
			}
			string text2 = array3[0];
			string text3 = array3[1];
			if (text2.IsNullOrEmpty() || text3.IsNullOrEmpty())
			{
				AdSystemLog.LogError("Error while parsing cache Id list for key " + onlineSettingsKey + " name or cache ids empty");
				continue;
			}
			cacheIdDictionary[text2] = new Dictionary<AdCacheSlotInfo, string>();
			string[] array4 = text3.Split(';');
			int num = 0;
			string[] array5 = array4;
			foreach (string value in array5)
			{
				cacheIdDictionary[text2].Add(AdCacheSlots.GetCacheSlotForIndex(adType, num), value);
				num++;
			}
		}
	}

	public virtual TimeSpan GetCacheTimeoutLimit(string providerID, AdType adType)
	{
		string key = string.Format("cache_timeout_limit_{0}", adType).ToLower();
		int value = OnlineSettings.instance.GetValue(key, 10);
		string key2 = string.Format("cache_timeout_limit_{0}_{1}", adType, providerID).ToLower();
		int value2 = OnlineSettings.instance.GetValue(key2, value);
		return TimeSpan.FromSeconds(value2);
	}

	public virtual TimeSpan? GetTimeLimitForAdProvider(string providerID, AdType adType)
	{
		string key = string.Format("lock_on_time_limit_{0}", adType).ToLower();
		int defaultValue;
		switch (adType)
		{
		case AdType.Interstitial:
			defaultValue = 72000;
			break;
		case AdType.Video:
			defaultValue = 72000;
			break;
		default:
			return null;
		}
		return TimeSpan.FromSeconds(OnlineSettings.instance.GetValue(key, defaultValue));
	}

	public virtual TimeSpan GetInterstitialShowInterval()
	{
		int value = OnlineSettings.instance.GetValue("global_time_between_interstitials", 10);
		return TimeSpan.FromSeconds(value);
	}

	public virtual uint GetGracePeriodValue()
	{
		return OnlineSettings.instance.GetValue("ads_grace_period", 72000u);
	}

	public virtual DateTime GetScheduledFirstInterstitialShowTime()
	{
		DateTime result = DateTime.MinValue;
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_TIME_LIMIT || Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_GRACE_PERIOD)
		{
			return result;
		}
		if (AdsSave.scheduledFirstInterstitialShowTime != DateTime.MinValue)
		{
			if (CDateTime.UtcNow > UserSave.firstInstallDate)
			{
				result = AdsSave.scheduledFirstInterstitialShowTime;
			}
		}
		else
		{
			uint gracePeriodValue = GetGracePeriodValue();
			result = (AdsSave.scheduledFirstInterstitialShowTime = UserSave.firstInstallDate + TimeSpan.FromSeconds(gracePeriodValue));
		}
		return result;
	}

	public virtual bool GetAvailabilityForLocation(string locationID)
	{
		bool flag = false;
		AdType? adType;
		if (!_mappedAdTypesForLocations.ContainsKey(locationID))
		{
			adType = GetAdTypeForLocationID(locationID);
			if (adType.HasValue)
			{
				_mappedAdTypesForLocations.Add(locationID, adType.Value);
			}
			else
			{
				AdSystemLog.LogError("The cacheSlot or location with id: " + locationID + " does NOT have an AdType assigned to it. Please assign an AdType!");
			}
		}
		else
		{
			adType = _mappedAdTypesForLocations[locationID];
		}
		if (adType.HasValue && ((Globals.DEBUG_VIDEOADS_PROVIDERS && adType == AdType.Video) || (Globals.DEBUG_INTERSTITIAL_PROVIDERS && adType.GetValueOrDefault() == AdType.Interstitial && adType.HasValue)))
		{
			foreach (AdLocationInfo adLocation in AdLocations.adLocations)
			{
				if (adLocation.id == locationID)
				{
					flag = true;
				}
			}
		}
		else
		{
			string key = string.Format("ad_location_{0}_enabled", locationID);
			flag = OnlineSettings.instance.GetValue(key, false);
			if (adType.HasValue && adType.Value == AdType.Interstitial)
			{
				if (Application.systemLanguage == SystemLanguage.Chinese)
				{
					return true;
				}
				if (flag)
				{
					flag = AreInterstitialsAllowed();
				}
			}
		}
		return flag;
	}

	public bool GetAvailabilityForCacheSlot(string adCacheSlotID)
	{
		if (string.IsNullOrEmpty(adCacheSlotID) || adCacheSlotID == AdCacheSlots.UNDETERMINED_SLOT.id)
		{
			AdSystemLog.LogWarning("GetAvailability - Slot id is empty or undetermined");
			return false;
		}
		string key = string.Format("ad_cache_slot_{0}_enabled", adCacheSlotID);
		bool flag = OnlineSettings.instance.GetValue(key, true);
		AdType? adTypeForCacheSlotID = GetAdTypeForCacheSlotID(adCacheSlotID);
		if (adTypeForCacheSlotID.HasValue && adTypeForCacheSlotID.Value == AdType.Interstitial)
		{
			if (Application.systemLanguage == SystemLanguage.Chinese)
			{
				return true;
			}
			if (flag)
			{
				flag = AreInterstitialsAllowed();
			}
		}
		return flag;
	}

	public virtual bool AreInterstitialsAllowed()
	{
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IN_APP_PURCHASES)
		{
			return true;
		}
		bool hasPurchasedOrRestored = UserSave.hasPurchasedOrRestored;
		return !hasPurchasedOrRestored;
	}

	public virtual int GetProviderWaitWhileCachingTimeLimit(string adLocationID, string providerID)
	{
		string key = string.Format("ad_location_{0}_{1}_wait_caching_limit", adLocationID, providerID);
		return OnlineSettings.instance.GetValue(key, 10);
	}

	public virtual bool TryGetProviderOverrideSupportedOnPlatform(AdType adType, string providerID, out string supportedDeviceVersion)
	{
		string key = string.Format("provider_{0}_{1}_minimum_device_version", providerID, adType).ToLower();
		return OnlineSettings.instance.TryGetValue(key, out supportedDeviceVersion);
	}

	public virtual bool OverrideInterstitialShowIntervalForLocation(string locationID, ref TimeSpan globalInterval)
	{
		string key = string.Format("ad_location_{0}_time_between_interstitials", locationID);
		if (OnlineSettings.instance.HasValue(key))
		{
			globalInterval = TimeSpan.FromSeconds(OnlineSettings.instance.GetValue(key, 10));
			return true;
		}
		return false;
	}

	public virtual Dictionary<AdCacheSlotInfo, string> GetCacheIDsForProviderTier(string providerTier, AdType adType)
	{
		if (!HasCacheIdDefinitions(adType) && !TryGettingCacheIdDefinitionsFromOnlineSettings(adType))
		{
			AdSystemLog.LogError("GetCacheIDForProvider - Could not get cache id definitions from Online Settings for provider Tier " + providerTier + " and adType " + adType);
			return null;
		}
		Dictionary<string, Dictionary<AdCacheSlotInfo, string>> cacheIdDefinitionsForAdType = GetCacheIdDefinitionsForAdType(adType);
		if (cacheIdDefinitionsForAdType.ContainsKey(providerTier))
		{
			return cacheIdDefinitionsForAdType[providerTier];
		}
		return null;
	}

	public virtual Dictionary<AdCacheSlotInfo, List<string>> GetAllCacheIDsForProvider(string provider, AdType adType)
	{
		Dictionary<AdCacheSlotInfo, List<string>> dictionary = new Dictionary<AdCacheSlotInfo, List<string>>();
		if (!HasCacheIdDefinitions(adType) && !TryGettingCacheIdDefinitionsFromOnlineSettings(adType))
		{
			AdSystemLog.LogError("GetAllCacheIDsForProvider - Could not get cache id definitions from Online Settings for provider " + provider + " and adType " + adType);
			return dictionary;
		}
		Dictionary<string, Dictionary<AdCacheSlotInfo, string>> cacheIdDefinitionsForAdType = GetCacheIdDefinitionsForAdType(adType);
		foreach (KeyValuePair<string, Dictionary<AdCacheSlotInfo, string>> item in cacheIdDefinitionsForAdType)
		{
			if (!item.Key.StartsWith(provider))
			{
				continue;
			}
			foreach (KeyValuePair<AdCacheSlotInfo, string> item2 in item.Value)
			{
				if (!dictionary.ContainsKey(item2.Key))
				{
					dictionary.Add(item2.Key, new List<string>());
				}
				dictionary[item2.Key].Add(item2.Value);
			}
		}
		return dictionary;
	}

	public virtual string GetCacheID(string adProviderTier, AdCacheSlotInfo adCacheSlotInfo)
	{
		Dictionary<AdCacheSlotInfo, string> cacheIDsForProviderTier = GetCacheIDsForProviderTier(adProviderTier, adCacheSlotInfo.adType);
		string value;
		if (cacheIDsForProviderTier != null && cacheIDsForProviderTier.TryGetValue(adCacheSlotInfo, out value))
		{
			return value;
		}
		AdSystemLog.LogError(string.Format("Cache ID not found in AdSettingsProvider.GetCacheID for provider: {0} and cacheSlot: {1} dict size: {2}", adProviderTier, adCacheSlotInfo.id, (cacheIDsForProviderTier != null) ? cacheIDsForProviderTier.Count.ToString() : "null"));
		return string.Empty;
	}

	public virtual string GetWaterfallID(string adLocationID)
	{
		string text = string.Empty;
		foreach (AdCacheSlotInfo adCacheSlot in AdCacheSlots.adCacheSlots)
		{
			if (adLocationID == adCacheSlot.id)
			{
				text = string.Format("{0}_list", adLocationID);
				break;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			AdSystemLog.LogError("WaterfallID is not set for locationID: " + adLocationID);
		}
		return text;
	}

	public virtual string GetWaterfallID(AdType adType)
	{
		return string.Format("default_{0}_list", adType).ToLower();
	}

	public virtual string GetLatestVersionForWaterfall(AdCacheSlot adCacheSlot)
	{
		if ((Globals.DEBUG_INTERSTITIAL_PROVIDERS && adCacheSlot.info.adType == AdType.Interstitial) || (Globals.DEBUG_VIDEOADS_PROVIDERS && adCacheSlot.info.adType == AdType.Video))
		{
			string value;
			_debugPriorityListVersion.TryGetValue(adCacheSlot.info.adType, out value);
			return value;
		}
		string key = string.Format("prio_list_{0}_version", adCacheSlot.waterfallID);
		return OnlineSettings.instance.GetValue(key, "notset");
	}

	public virtual string[] GetAdProvidersToInitialize()
	{
		return new string[0];
	}

	private string[] GetAdColonyZoneIDs()
	{
		Dictionary<AdCacheSlotInfo, List<string>> allCacheIDsForProvider = GetAllCacheIDsForProvider("adcolony", AdType.Video);
		List<string> list = new List<string>();
		if (allCacheIDsForProvider == null || allCacheIDsForProvider.Count == 0)
		{
			return new string[0];
		}
		foreach (AdCacheSlotInfo adCacheSlot in AdCacheSlots.adCacheSlots)
		{
			if (allCacheIDsForProvider.ContainsKey(adCacheSlot))
			{
				List<string> list2 = allCacheIDsForProvider[adCacheSlot];
				if (list2 != null && list2.Count != 0)
				{
					list.AddRange(list2);
				}
			}
		}
		return list.ToArray();
	}

	public virtual AdProviderConfiguration GetConfigurationForProvider(string providerID)
	{
		AdProviderConfiguration adProviderConfiguration = new AdProviderConfiguration();
		adProviderConfiguration.isRestricted = AdConstants.instance.isRestricted;
		AdProviderConfiguration adProviderConfiguration2 = adProviderConfiguration;
		switch (providerID)
		{
		case "inmobi":
			adProviderConfiguration2.appID = AdConstants.instance.InmobiAccountId;
			break;
		case "adcolony":
			adProviderConfiguration2.appID = AdConstants.instance.AdColonyAppId;
			adProviderConfiguration2.zoneIDs = GetAdColonyZoneIDs();
			break;
		case "chartboost":
			adProviderConfiguration2.appID = AdConstants.instance.ChartboostAppId;
			adProviderConfiguration2.appSignature = AdConstants.instance.ChartboostAppSignature;
			break;
		case "tapjoy":
			adProviderConfiguration2.appID = AdConstants.instance.TapJoyAppId;
			adProviderConfiguration2.appSDKKey = AdConstants.instance.TapJoyAppKey;
			break;
		case "supersonic":
			adProviderConfiguration2.appID = AdConstants.instance.SupersonicAppId;
			break;
		case "vungle":
			adProviderConfiguration2.appID = AdConstants.instance.VungleAppId;
			break;
		case "mobvista":
			adProviderConfiguration2.appID = AdConstants.instance.MobVistaAppId;
			adProviderConfiguration2.appSDKKey = AdConstants.instance.MobVistaAppSdkKey;
			break;
		default:
			AdSystemLog.LogError("Configuration is not defined for " + providerID);
			break;
		case "facebook":
		case "superawesome":
			break;
		}
		return adProviderConfiguration2;
	}

	public virtual int GetFailCountTolerance(string adProvider, AdType adType)
	{
		return OnlineSettings.instance.GetValue("number_of_fails_for_interstitial", 1);
	}

	public virtual int GetMaxInterstitialImpressionsPerHour()
	{
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IMPRESSION_LIMIT)
		{
			return int.MaxValue;
		}
		return OnlineSettings.instance.GetValue("interstitials_limit_per_hour", 5);
	}

	public virtual int GetMaxInterstitialImpressionsPerDay()
	{
		if (Globals.DEBUG_INTERSTITIAL_PROVIDERS_IGNORE_IMPRESSION_LIMIT)
		{
			return int.MaxValue;
		}
		return OnlineSettings.instance.GetValue("interstitials_limit_per_day", 20);
	}

	private static void CacheProviderIDs()
	{
		if (_videoProviderIDs.Count == 0)
		{
			for (int i = 0; i < _allVideoProviders.Count; i++)
			{
				if (!_videoProviderIDs.Contains(_allVideoProviders[i].providerID))
				{
					_videoProviderIDs.Add(_allVideoProviders[i].providerID);
				}
			}
		}
		if (_interstitialProviderIDs.Count != 0)
		{
			return;
		}
		for (int j = 0; j < _allInterstitialProviders.Count; j++)
		{
			if (!_interstitialProviderIDs.Contains(_allInterstitialProviders[j].providerID))
			{
				_interstitialProviderIDs.Add(_allInterstitialProviders[j].providerID);
			}
		}
	}

	private List<AdProviderInfo> GetDebugPriorityList(AdType adType)
	{
		List<AdProviderInfo> list = new List<AdProviderInfo>();
		Dictionary<string, bool> dictionary = ((adType != AdType.Interstitial) ? _debugVideoProviderListState : _debugInterstitialProviderListState);
		int num = 1;
		foreach (KeyValuePair<string, bool> item2 in dictionary)
		{
			if (item2.Value)
			{
				string providerIdFromTier = GetProviderIdFromTier(item2.Key);
				AdProviderInfo adProviderInfo = new AdProviderInfo();
				adProviderInfo.providerID = providerIdFromTier;
				adProviderInfo.priority = num;
				adProviderInfo.weight = 100f;
				adProviderInfo.providerTier = item2.Key;
				AdProviderInfo item = adProviderInfo;
				list.Add(item);
				num++;
			}
		}
		return list;
	}

	public virtual void SetDebugPriorityListVersion(AdType adType, string value)
	{
		_debugPriorityListVersion[adType] = value;
	}

	private AdType? GetAdTypeForLocationID(string location)
	{
		AdType? result = null;
		foreach (AdLocationInfo adLocation in AdLocations.adLocations)
		{
			if (adLocation.id == location)
			{
				result = adLocation.adType;
			}
		}
		return result;
	}

	private AdType? GetAdTypeForCacheSlotID(string cacheSlotId)
	{
		AdType? result = null;
		foreach (AdCacheSlotInfo adCacheSlot in AdCacheSlots.adCacheSlots)
		{
			if (adCacheSlot.id == cacheSlotId)
			{
				result = adCacheSlot.adType;
			}
		}
		return result;
	}

	private List<string> GetProviderTierList(AdType adType)
	{
		TryGettingCacheIdDefinitionsFromOnlineSettings(adType);
		List<string> list = ((adType != AdType.Video) ? _interstitialCacheIDs.Keys.ToList() : _videoCacheIDs.Keys.ToList());
		List<string> list2 = new List<string>();
		foreach (string item in list)
		{
			string providerIdFromTier = GetProviderIdFromTier(item);
			if (AbstractAdProvider.IsProviderSupportedOnPlatform(providerIdFromTier, adType))
			{
				list2.Add(item);
			}
		}
		return list2;
	}

	private string GetProviderIdFromTier(string tier)
	{
		if (tier.IsNullOrEmpty())
		{
			return string.Empty;
		}
		string[] array = tier.Split('_');
		return array[0];
	}

	public Dictionary<string, bool> GetDebugProviderListState(AdType adType)
	{
		Dictionary<string, bool> dictionary = ((adType != AdType.Interstitial) ? _debugVideoProviderListState : _debugInterstitialProviderListState);
		if (dictionary.Count > 0 || !Globals.DEBUG)
		{
			return dictionary;
		}
		switch (adType)
		{
		case AdType.Video:
		{
			List<string> providerTierList2 = GetProviderTierList(AdType.Video);
			if (providerTierList2 == null)
			{
				break;
			}
			foreach (string item in providerTierList2)
			{
				if ((Globals.DEBUG_VIDEOADS_PROVIDERS_ADCOLONY && item.StartsWith("adcolony")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_CHARTBOOST && item.StartsWith("chartboost")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_FACEBOOK && item.StartsWith("facebook")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_INMOBI && item.StartsWith("inmobi")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_MOBVISTA && item.StartsWith("mobvista")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_SUPERSONIC && item.StartsWith("supersonic")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_TAPJOY && item.StartsWith("tapjoy")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_VUNGLE && item.StartsWith("vungle")) || (Globals.DEBUG_VIDEOADS_PROVIDERS_SUPER_AWESOME && item.StartsWith("superawesome")))
				{
					dictionary.Add(item, true);
				}
				else
				{
					dictionary.Add(item, false);
				}
			}
			break;
		}
		case AdType.Interstitial:
		{
			List<string> providerTierList = GetProviderTierList(AdType.Interstitial);
			if (providerTierList == null)
			{
				break;
			}
			foreach (string item2 in providerTierList)
			{
				if ((Globals.DEBUG_INTERSTITIAL_PROVIDER_CHARTBOOST && item2.StartsWith("chartboost")) || (Globals.DEBUG_INTERSTITIAL_PROVIDER_FACEBOOK && item2.StartsWith("facebook")) || (Globals.DEBUG_INTERSTITIAL_PROVIDER_INMOBI && item2.StartsWith("inmobi")) || (Globals.DEBUG_INTERSTITIAL_PROVIDER_SUPERSONIC && item2.StartsWith("supersonic")) || (Globals.DEBUG_INTERSTITIAL_PROVIDER_MOBVISTA && item2.StartsWith("mobvista")) || (Globals.DEBUG_INTERSTITIAL_PROVIDER_SUPERAWESOME && item2.StartsWith("superawesome")))
				{
					dictionary.Add(item2, true);
				}
				else
				{
					dictionary.Add(item2, false);
				}
			}
			break;
		}
		}
		return dictionary;
	}
}
