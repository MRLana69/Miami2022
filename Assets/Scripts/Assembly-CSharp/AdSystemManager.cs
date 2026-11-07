using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using UnityEngine;
using Utilities;

public class AdSystemManager : AdEventListenerAdapter
{
	public const string LAST_SHOWED_PROVIDER_SAVED_SUFFIX = "_(saved)";

	private const string ONLINE_SETTINGS_ALLOW_RECACHE_DURING_RUN_KEY = "allow_recache_during_run";

	private bool _isInitialized;

	private bool _showAllowed = true;

	private AdController _adController;

	private AdLocationInfo _currentlyShowingAdAtLocation;

	private readonly Dictionary<string, string> _lastShowedProviderPerSlot = new Dictionary<string, string>();

	private Dictionary<AdType, string> _lastShowedSlotsPerAdType;

	private ScreenOrientation _currentScreenOrientation = ScreenOrientation.Portrait;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action onAdSystemInitialized__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onWillShowVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onShowedVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onFinishedWatchingVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onFailedToShowVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onDismissedVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, bool> onCachedVideo__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo> onFailedToCacheVideo__BackingField;

	private Action _videoReward;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onWillShowInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onShowedInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onFailedToShowInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo, AdLocationInfo> onDismissedInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo> onCachedInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<AdCacheSlotInfo> onFailedToCacheInterstitial__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onPriorityListWrapped__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onChangedProvider__BackingField;

	private static AdSystemManager _instance;

	public static bool isInitialized
	{
		get
		{
			return _instance != null && _instance._isInitialized;
		}
	}

	public static event Action onAdSystemInitialized
	{
		add
		{
			Action action = onAdSystemInitialized__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onAdSystemInitialized__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onAdSystemInitialized__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onAdSystemInitialized__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onWillShowVideo
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onWillShowVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onWillShowVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onShowedVideo
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onShowedVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onShowedVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onFinishedWatchingVideo
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onFinishedWatchingVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFinishedWatchingVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onFinishedWatchingVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFinishedWatchingVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onFailedToShowVideo
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onFailedToShowVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onFailedToShowVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onDismissedVideo
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onDismissedVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onDismissedVideo__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedVideo__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, bool> onCachedVideo
	{
		add
		{
			Action<AdCacheSlotInfo, bool> action = onCachedVideo__BackingField;
			Action<AdCacheSlotInfo, bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedVideo__BackingField, (Action<AdCacheSlotInfo, bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, bool> action = onCachedVideo__BackingField;
			Action<AdCacheSlotInfo, bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedVideo__BackingField, (Action<AdCacheSlotInfo, bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo> onFailedToCacheVideo
	{
		add
		{
			Action<AdCacheSlotInfo> action = onFailedToCacheVideo__BackingField;
			Action<AdCacheSlotInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheVideo__BackingField, (Action<AdCacheSlotInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo> action = onFailedToCacheVideo__BackingField;
			Action<AdCacheSlotInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheVideo__BackingField, (Action<AdCacheSlotInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onWillShowInterstitial
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onWillShowInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onWillShowInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onWillShowInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onShowedInterstitial
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onShowedInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onShowedInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onShowedInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onFailedToShowInterstitial
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onFailedToShowInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onFailedToShowInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToShowInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo, AdLocationInfo> onDismissedInterstitial
	{
		add
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onDismissedInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo, AdLocationInfo> action = onDismissedInterstitial__BackingField;
			Action<AdCacheSlotInfo, AdLocationInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onDismissedInterstitial__BackingField, (Action<AdCacheSlotInfo, AdLocationInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo> onCachedInterstitial
	{
		add
		{
			Action<AdCacheSlotInfo> action = onCachedInterstitial__BackingField;
			Action<AdCacheSlotInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedInterstitial__BackingField, (Action<AdCacheSlotInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo> action = onCachedInterstitial__BackingField;
			Action<AdCacheSlotInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCachedInterstitial__BackingField, (Action<AdCacheSlotInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<AdCacheSlotInfo> onFailedToCacheInterstitial
	{
		add
		{
			Action<AdCacheSlotInfo> action = onFailedToCacheInterstitial__BackingField;
			Action<AdCacheSlotInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheInterstitial__BackingField, (Action<AdCacheSlotInfo>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<AdCacheSlotInfo> action = onFailedToCacheInterstitial__BackingField;
			Action<AdCacheSlotInfo> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToCacheInterstitial__BackingField, (Action<AdCacheSlotInfo>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onPriorityListWrapped
	{
		add
		{
			Action<string> action = onPriorityListWrapped__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPriorityListWrapped__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onPriorityListWrapped__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onPriorityListWrapped__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> onChangedProvider
	{
		add
		{
			Action<string> action = onChangedProvider__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onChangedProvider__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onChangedProvider__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onChangedProvider__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private AdSystemManager()
	{
		Initialize();
	}

	public static void CreateAdSystemManager()
	{
		if (_instance == null)
		{
			_instance = new AdSystemManager();
		}
		else
		{
			AdSystemLog.LogWarning("AdSystemManager.CreateAdSystemManager was called, but an instance had already been created! Ignoring.");
		}
		Cache();
	}

	private void Initialize()
	{
		if (AgeRestriction.waitForAgeRestriction)
		{
			AdSystemLog.LogError("AdSystemManager initialized before AgeRestriction has been setup!\n" + Environment.StackTrace);
		}
		_adController = new AdController(new AdSettingsProvider(), AdCacheSlots.adCacheSlots, AdsSave.interstitialStats);
		_adController.AddEventListener(this);
		_adController.AddEventListener(new AdSystemEventLogger());
		_adController.AddEventListener(new AdController.EventHandler(_adController));
		_adController.AddEventListener(new AdEventLogger());
		UIScreenController.OnWillShowScreen += HandleOnWillShowScreen;
		LoadLastShowedProviders();
		AppRefreshHandler.hasRefreshed += AppHasRefreshed;
		_isInitialized = true;
		CoroutineUtil.StartCoroutine(CoroutineUtil.ExecuteActionNextFrame(() =>
		{
			onAdSystemInitialized__BackingField.SafeInvoke();
		}));
	}

	private void AppHasRefreshed(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.Data))
		{
			_adController.UpdateInterstitialStats(AdsSave.interstitialStats);
		}
	}

	public static bool IsCached(AdLocationInfo info)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsCached(info);
	}

	private static void Cache()
	{
		if (_instance != null && _instance._isInitialized)
		{
			_instance._adController.Cache();
		}
	}

	public static void Show(AdLocationInfo adLocationInfo, Action videoPayout = null)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			AdSystemLog.LogError("Show called, but Ad System has not been initialised!");
		}
		else
		{
			if (!_instance._showAllowed)
			{
				return;
			}
			_instance._showAllowed = false;
			if (adLocationInfo.adType == AdType.Video)
			{
				_instance.SaveCurrentOrientation();
				_instance._videoReward = videoPayout;
			}
			if (Globals.DEBUG_SKIP_VIDEO_ADS && adLocationInfo.adType == AdType.Video)
			{
				_instance.SkipVideoAd();
				_instance._showAllowed = true;
				return;
			}
			_instance.ShowAd(adLocationInfo);
			CoroutineUtil.StartCoroutine(CoroutineUtil.ExecuteActionNextFrame(() =>
			{
				_instance._showAllowed = true;
			}));
		}
	}

	private void SkipVideoAd()
	{
		FinishedWatchingVideo(string.Empty, string.Empty, AdCacheSlots.UNDETERMINED_SLOT, AdLocations.UNDETERMINED_LOCATION);
		DismissedAd(AdType.Video, string.Empty, string.Empty, AdCacheSlots.UNDETERMINED_SLOT, AdLocations.UNDETERMINED_LOCATION);
	}

	private void ShowAd(AdLocationInfo adLocationInfo)
	{
		_currentlyShowingAdAtLocation = adLocationInfo;
		if (_adController.Show(adLocationInfo))
		{
			ShowTransitionOverlay(adLocationInfo.adType, adLocationInfo);
		}
	}

	public static string GetLastShowedProviderForCacheSlot(AdCacheSlotInfo cacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			AdSystemLog.LogError("GetLastShowedProviderForLocation called, but Ad System has not been initialised!");
			return string.Empty;
		}
		string value;
		_instance._lastShowedProviderPerSlot.TryGetValue(cacheSlotInfo.id, out value);
		return value ?? string.Empty;
	}

	public static string GetLastPlayedCacheSlotID(AdType adType)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			AdSystemLog.LogError("GetLastPlayedLocation called, but Ad System has not been initialised!");
			return string.Empty;
		}
		string value;
		_instance._lastShowedSlotsPerAdType.TryGetValue(adType, out value);
		return value ?? string.Empty;
	}

	public static string GetCurrentProviderIDForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return _instance._adController.GetCurrentProviderIDForCacheSlot(adCacheSlotInfo);
	}

	public static string GetCurrentProviderTierForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return _instance._adController.GetCurrentProviderTierForCacheSlot(adCacheSlotInfo);
	}

	public static string GetHighestPriorityCachedProviderTierForSlot(AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return _instance._adController.GetHighestPriorityCachedProviderTierForSlot(adCacheSlotInfo);
	}

	public static string GetNextProviderInWaterfall(AdLocationInfo adLocationInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return _instance._adController.GetNextProviderInWaterfall(adLocationInfo);
	}

	public static List<AdProviderInfo> GetProvidersForCacheSlot(AdCacheSlot adCacheSlot)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return null;
		}
		return _instance._adController.GetProvidersForCacheSlot(adCacheSlot);
	}

	public static AdCacheSlotInfo GetAdCacheSlotInfoFromID(string adCacheSlotId)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return AdCacheSlots.UNDETERMINED_SLOT;
		}
		AdCacheSlot adCacheSlotFromID = _instance._adController.GetAdCacheSlotFromID(adCacheSlotId);
		if (adCacheSlotFromID == null)
		{
			AdSystemLog.LogError("Couldn't find ad cacheSlot for ID: " + adCacheSlotId);
			return AdCacheSlots.UNDETERMINED_SLOT;
		}
		return adCacheSlotFromID.info;
	}

	public static bool GetAvailabilityForLocation(string locationID)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.GetAvailabilityForLocation(locationID);
	}

	public static bool GetAvailabilityForCacheSlot(string cacheSlotId)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.GetAvailabilityForAdCacheSlot(cacheSlotId);
	}

	public static bool AreInterstitialsAllowed()
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.AreInterstitialsAllowed();
	}

	public static bool HasPriorityListWrapped(string cacheSlotId)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.HasPriorityListWrapped(cacheSlotId);
	}

	public static bool HaveAllCachePoolPriorityListsWrapped(AdLocationInfo currentAdLocationInfo)
	{
		if (_instance == null || !_instance._isInitialized || currentAdLocationInfo == null)
		{
			return false;
		}
		foreach (AdCacheSlotInfo adCacheSlot in AdCacheSlots.adCacheSlots)
		{
			if (currentAdLocationInfo.adType == adCacheSlot.adType && _instance._adController.GetAvailabilityForAdCacheSlot(adCacheSlot.id) && !_instance._adController.HasPriorityListWrapped(adCacheSlot.id))
			{
				return false;
			}
		}
		return true;
	}

	public override void WillShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo cacheSlotInfo, AdLocationInfo adLocationInfo)
	{
		UpdateLastShowedProviders(cacheSlotInfo, providerID, providerTier, false);
		UpdateLastShowedAdCacheSlots(adType, cacheSlotInfo, true);
		switch (adType)
		{
		case AdType.Interstitial:
			onWillShowInterstitial__BackingField.SafeInvoke(cacheSlotInfo, adLocationInfo);
			break;
		case AdType.Video:
			onWillShowVideo__BackingField.SafeInvoke(cacheSlotInfo, adLocationInfo);
			break;
		}
	}

	public override void CachedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
	{
		switch (adType)
		{
		case AdType.Video:
			onCachedVideo__BackingField.SafeInvoke(adCacheSlotInfo, wasAlreadyCached);
			break;
		case AdType.Interstitial:
			onCachedInterstitial__BackingField.SafeInvoke(adCacheSlotInfo);
			break;
		}
	}

	public override void FailedToCacheAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo)
	{
		if (!(_adController.GetCurrentProviderIDForCacheSlot(adCacheSlotInfo) != providerID))
		{
			switch (adType)
			{
			case AdType.Video:
				onFailedToCacheVideo__BackingField.SafeInvoke(adCacheSlotInfo);
				break;
			case AdType.Interstitial:
				onFailedToCacheInterstitial__BackingField.SafeInvoke(adCacheSlotInfo);
				break;
			}
		}
	}

	public override void ShowedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
	{
		UpdateLastShowedProviders(adCacheSlotInfo, providerID, providerTier, false);
		UpdateLastShowedAdCacheSlots(adType, adCacheSlotInfo, false);
		ShowOverlay(adType, adCacheSlotInfo);
		switch (adType)
		{
		case AdType.Video:
			DisableAudio();
			onShowedVideo__BackingField.SafeInvoke(adCacheSlotInfo, adLocationInfo);
			break;
		case AdType.Interstitial:
			onShowedInterstitial__BackingField.SafeInvoke(adCacheSlotInfo, adLocationInfo);
			break;
		}
	}

	public override void FinishedWatchingVideo(string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
	{
		HideOverlay(AdType.Video);
		EnableAudio();
		GiveReward();
		onFinishedWatchingVideo__BackingField.SafeInvoke(adCacheSlotInfo, adLocationInfo);
		if (adCacheSlotInfo != AdCacheSlots.UNDETERMINED_SLOT)
		{
			Cache();
		}
	}

	public override void FailedToShowAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
	{
		HideOverlay(adType);
		_currentlyShowingAdAtLocation = null;
		switch (adType)
		{
		case AdType.Video:
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("SLIDEIN_VIDEO_PLAY_ERROR"));
			onFailedToShowVideo__BackingField.SafeInvoke(adCacheSlotInfo, adLocationInfo);
			if (adCacheSlotInfo != AdCacheSlots.UNDETERMINED_SLOT)
			{
				Cache();
			}
			break;
		case AdType.Interstitial:
			onFailedToShowInterstitial__BackingField.SafeInvoke(adCacheSlotInfo, adLocationInfo);
			break;
		}
	}

	public override void DismissedAd(AdType adType, string providerID, string providerTier, AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
	{
		HideOverlay(adType);
		_currentlyShowingAdAtLocation = null;
		switch (adType)
		{
		case AdType.Video:
			EnableAudio();
			ResetOrientationIfNeeded();
			onDismissedVideo__BackingField.SafeInvoke(adCacheSlotInfo, adLocationInfo);
			break;
		case AdType.Interstitial:
			onDismissedInterstitial__BackingField.SafeInvoke(adCacheSlotInfo, adLocationInfo);
			break;
		}
	}

	public override void ChangedProvider(AdType adType, AdProviderInfo newProviderInfo, string changedWaterfallID)
	{
		onChangedProvider__BackingField.SafeInvoke(changedWaterfallID);
	}

	public override void PriorityListWrapped(string waterfallID)
	{
		onPriorityListWrapped__BackingField.SafeInvoke(waterfallID);
	}

	private void HandleOnWillShowScreen(string screenName)
	{
		bool value = OnlineSettings.instance.GetValue("allow_recache_during_run", false);
		_adController.allowReCacheOnFail = screenName != "IngameUI" || value;
	}

	private void GiveReward()
	{
		if (_videoReward != null)
		{
			_videoReward.SafeInvoke();
			_videoReward = null;
		}
		else
		{
			SDebug.LogError("Tried to GiveReward but _videoReward is null!");
		}
	}

	private void ShowTransitionOverlay(AdType adType, AdLocationInfo adLocationInfo)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			UIScreenController.Instance.ShowTransitionAdsOverlay();
			break;
		case AdType.Video:
			UIScreenController.Instance.ShowVideoAdsTransitionOverlay(adLocationInfo);
			break;
		default:
			SDebug.LogError("Unable to find AdsOverlay for adType: " + adType);
			break;
		}
	}

	private void ShowOverlay(AdType adType, AdCacheSlotInfo adCacheSlotInfo)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			UIScreenController.Instance.ShowAdsOverlay();
			break;
		case AdType.Video:
			UIScreenController.Instance.ShowVideoAdsOverlay(adCacheSlotInfo);
			break;
		default:
			SDebug.LogError("Unable to find AdsOverlay for adType: " + adType);
			break;
		}
	}

	private void HideOverlay(AdType adType)
	{
		switch (adType)
		{
		case AdType.Interstitial:
			UIScreenController.Instance.HideAdsOverlay();
			break;
		case AdType.Video:
			UIScreenController.Instance.HideVideoAdsOverlay(true);
			break;
		default:
			SDebug.LogError("Unable to find AdsOverlay for adType: " + adType);
			break;
		}
	}

	private void DisableAudio()
	{
		HouseKeeper.SaveAudioOptionsBeforeVideoAds(Settings.optionSfx, Settings.optionMusic);
		Settings.DisableAllSoundOptions();
	}

	private void EnableAudio()
	{
		HouseKeeper.TryRestoreSoundOptionAfterLostFocusIfNeeded();
	}

	private void SaveCurrentOrientation()
	{
		_currentScreenOrientation = ScreenOrientation.Portrait;
		if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
		{
			_currentScreenOrientation = ScreenOrientation.PortraitUpsideDown;
		}
	}

	private void ResetOrientationIfNeeded()
	{
		if (Screen.width > Screen.height)
		{
			Screen.orientation = _currentScreenOrientation;
			Kiloo.Common.Timer.Timer.ExecuteWithDelay(1f, () =>
			{
				Screen.autorotateToLandscapeLeft = false;
				Screen.autorotateToLandscapeRight = false;
				Screen.autorotateToPortrait = true;
				Screen.autorotateToPortraitUpsideDown = SafeArea.PortraitUpsideDownIsAllowed;
				Screen.orientation = ScreenOrientation.AutoRotation;
			});
		}
	}

	private void LoadLastShowedProviders()
	{
		Dictionary<string, string> lastShowedProviders = AdsSave.lastShowedProviders;
		if (lastShowedProviders != null)
		{
			foreach (KeyValuePair<string, string> item in lastShowedProviders)
			{
				_lastShowedProviderPerSlot[item.Key] = item.Value + "_(saved)";
			}
		}
		_lastShowedSlotsPerAdType = AdsSave.lastShowedCacheSlots;
	}

	private void UpdateLastShowedProviders(AdCacheSlotInfo adCacheSlotInfo, string providerID, string providerTier, bool shouldSave)
	{
		_lastShowedProviderPerSlot[adCacheSlotInfo.id] = ((!providerTier.IsNullOrEmpty()) ? providerTier : providerID);
		AdsSave.lastShowedProviders = _lastShowedProviderPerSlot;
		if (shouldSave)
		{
			SaveManager.Save();
		}
	}

	private void UpdateLastShowedAdCacheSlots(AdType adType, AdCacheSlotInfo adCacheSlotInfo, bool shouldSave)
	{
		_lastShowedSlotsPerAdType[adType] = adCacheSlotInfo.id;
		AdsSave.lastShowedCacheSlots = _lastShowedSlotsPerAdType;
		if (shouldSave)
		{
			SaveManager.Save();
		}
	}

	public static string GetIdOfAdlocationBeingShown()
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return string.Empty;
		}
		return (_instance._currentlyShowingAdAtLocation == null) ? string.Empty : _instance._currentlyShowingAdAtLocation.id;
	}

	public static Dictionary<string, bool> GetDebugProviderListState(AdType adType)
	{
		if (_instance == null || !_instance._isInitialized || !Globals.DEBUG)
		{
			AdSystemLog.LogError("GetCacheIdDefinitionsForDebug called, but Ad System has not been initialized or debug is off");
			return null;
		}
		return _instance._adController.GetDebugProviderListState(adType);
	}

	public static List<AdProviderInfo> DebugGetProviderListForSlot(AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized || !Globals.DEBUG)
		{
			AdSystemLog.LogError("DebugGetProviderListForSlot called, but Ad System has not been initialized or debug is off");
			return null;
		}
		return _instance._adController.DebugGetProviderListForSlot(adCacheSlotInfo);
	}

	public static string DebugGetCacheIdForProvider(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized || !Globals.DEBUG)
		{
			AdSystemLog.LogError("DebugGetCacheIdForProvider called, but Ad System has not been initialized or debug is off");
			return null;
		}
		return _instance._adController.DebugGetCacheIdForProvider(adProviderInfo, adCacheSlotInfo);
	}

	public static void DebugShowAdOnSlot(AdCacheSlotInfo adCacheSlot, Action videoPayout = null)
	{
		if (_instance == null || !_instance._isInitialized || !Globals.DEBUG)
		{
			AdSystemLog.LogError("DebugShowAdOnSlot called, but Ad System has not been initialized or debug is off");
			return;
		}
		if (adCacheSlot.adType == AdType.Video)
		{
			_instance._videoReward = videoPayout;
		}
		_instance._adController.DebugShowAdOnSlot(adCacheSlot);
	}

	public static void DebugCacheAdOnSlot(AdCacheSlotInfo adCacheSlot)
	{
		if (_instance == null || !_instance._isInitialized || !Globals.DEBUG)
		{
			AdSystemLog.LogError("DebugCacheAdOnSlot called, but Ad System has not been initialized or debug is off");
		}
		else
		{
			_instance._adController.DebugCacheAdOnSlot(adCacheSlot);
		}
	}

	public static void DebugShowAdForProvider(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo, Action videoPayout = null)
	{
		if (_instance == null || !_instance._isInitialized || !Globals.DEBUG)
		{
			AdSystemLog.LogError("DebugShowAdOnProvider called, but Ad System has not been initialized or debug is off");
			return;
		}
		if (adCacheSlotInfo.adType == AdType.Video)
		{
			_instance._videoReward = videoPayout;
		}
		_instance._adController.DebugShowAdForProvider(adProviderInfo, adCacheSlotInfo);
	}

	public static void DebugCacheAdForProvider(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlot)
	{
		if (_instance == null || !_instance._isInitialized || !Globals.DEBUG)
		{
			AdSystemLog.LogError("DebugCacheAdOnSlot called, but Ad System has not been initialized or debug is off");
		}
		else
		{
			_instance._adController.DebugCacheAdForProvider(adProviderInfo, adCacheSlot);
		}
	}

	public static void ChangeToNextProviderDebug()
	{
		if (_instance != null && _instance._isInitialized)
		{
			_instance._adController.ChangeToNextProvider();
			_instance._adController.Cache();
		}
	}

	public static void UpdateDebugPriorityListVersion(AdType adType)
	{
		if (_instance != null && _instance._isInitialized)
		{
			_instance._adController.SetDebugPriorityListVersion(adType, Guid.NewGuid().ToString());
		}
	}

	public static bool IsCachingDebug(AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsCaching(adCacheSlotInfo);
	}

	public static bool IsCachedDebug(AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsCachedDebug(adCacheSlotInfo);
	}

	public static bool IsProviderCachingDebug(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsProviderCachingDebug(adProviderInfo, adCacheSlotInfo);
	}

	public static bool IsProviderCachedDebug(AdProviderInfo adProviderInfo, AdCacheSlotInfo adCacheSlotInfo)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			return false;
		}
		return _instance._adController.IsProviderCachedDebug(adProviderInfo, adCacheSlotInfo);
	}

	public static void DebugCache()
	{
		Cache();
	}
}
