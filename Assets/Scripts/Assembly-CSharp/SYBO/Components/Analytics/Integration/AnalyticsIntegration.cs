using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ABTest;
using Cloud;
using Data.Managers;
using Data.Save;
using InApp;
using InAppPurchase;
using Info;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Plugins.DeviceUtility;
using Missions;
using SYBO.Components.Analytics.Albatross;
using SYBO.Components.Json;
using SYBO.Components.Logging;
using SYBO.Components.Singletons;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Info;
using SYBO.Subway.Pickups;
using SYBO.Subway.Routes;
using UI.DoubleCoinsVideo;
using UI.Popups;
using UI.Popups.ConsumablesPopupInternal;
using UI.Screens.RewardManagement;
using UnityEngine;
using WordHunt;

namespace SYBO.Components.Analytics.Integration
{
	[PrefabPath("Analytics/AnalyticsIntegration")]
	public class AnalyticsIntegration : SingletonMonoBehaviour<AnalyticsIntegration>, IAnalyticsIntegrationTest, ICoroutineHandler
	{
		public const string PlatformConfig = "android";

		public const string PrefabPath = "Analytics/AnalyticsIntegration";

		public const int UnusedInt = -1;

		public static readonly string AnalyticsUserIdPersistedDefault;

		public static readonly string PreviousClientVersionPersistedDefault;

		public static readonly int GamesSinceFlushPersistedDefault;

		public static readonly int CurrentSessionPersistedDefault;

		public static readonly string OperatingSystemPersistedDefault;

		public static readonly string DeviceIDPersistedDefault;

		public static readonly string OSLanguagePersistedDefault;

		public static readonly DeviceInfo.PerformanceLevel PerformanceLevelPersistedDefault;

		public static readonly int GpuScorePersistedDefault;

		public static readonly string OnlineSettingsLocalePersistedDefault;

		public static readonly string UnhandledAbTestDataPersistedDefault;

		public static readonly SampleState SampleStateDefault;

		private const long ForegroundChangeThresholdMsTicks = 2000L;

		private const int MenuScreenHistoryCountTracked = 4;

		private const int MenuScreenHistoryCountReported = 3;

		private const float MaxFrameTimeSampleRangeMs = 2000f;

		private const int NoAbTestSegmentDefault = -1;

		private const string PrefsKeyPersistedLeavesForegroundTime = "LeavesForegroundTime";

		private static readonly string PrefsKeyPersistedLeavesForegroundTimeDefault;

		private const string PrefsKeyPersistedLeavesForegroundSessionDelta = "LeavesForegroundSessionDelta";

		private const int PrefsKeyPersistedLeavesForegroundSessionDeltaDefault = -1;

		private const int persistedSessionEventNumberDefault = -1;

		public const string OSKeyAnalyticsEnabled = "sybo_analytics_enabled";

		private const bool OSKeyAnalyticsEnabledDefault = false;

		public const string OSKeyAnalyticsFlushEnabled = "sybo_analytics_flush_enabled";

		private const bool OSKeyAnalyticsFlushEnabledDefault = true;

		public const string OSKeySamplingThreshold = "sybo_analytics_sampling_threshold";

		private const float OSKeySamplingThresholdDefault = 0f;

		public const string OSKeyEndpointBaseUrl = "sybo_analytics_endpoint_baseurl";

		private const string OSKeyEndpointBaseUrlDefault = null;

		public const string OSKeyMaxGamesWithoutFlush = "sybo_analytics_max_games_without_flush";

		private const int OSKeyMaxGamesWithoutFlushDefault = 10;

		public const string OSKeyGamesInSessionBeforeFirstFlush = "sybo_analytics_games_in_session_before_first_flush";

		private const int OSKeyGamesInSessionBeforeFirstFlushDefault = 1;

		public const string OSKeyOnlyValidatedPurchases = "sybo_analytics_only_validated_purchases";

		private const bool OSKeyOnlyValidatedPurchasesDefault = true;

		[SerializeField]
		private AnalyticsSettings _settings;

		[Header("Runtime Generated")]
		[SerializeField]
		private float _samplingKey;

		[SerializeField]
		private SampleState _sampleState;

		[Header("Editor Debugging")]
		[SerializeField]
		private bool _useDebugSettings;

		[SerializeField]
		private bool _useDebugSettingsOnce;

		[SerializeField]
		private AnalyticsSettings _debugSettings;

		private bool _usedDebugSettings;

		private SessionTracker _sessionTracker;

		private long _msTicksAtCurrentSessionStart;

		private AnalyticsManager _manager;

		private int _lastSession = CurrentSessionPersistedDefault;

		private int _lastGameIdInSession;

		private bool _isRunInProgress;

		private bool _isReadyToPostGameEvents;

		private bool _hasUnflushedPurchaseEvent;

		private bool _waitingToInitialize;

		private bool _subscribedEventPostingHandlers;

		private bool _subscribedToFrontScreenEvent;

		private bool _subscribedToInAppManager;

		private static bool _jsonClassInfoCached;

		private string _menuScreenLast;

		private Queue<string> _menuScreenQueue = new Queue<string>(4);

		private IPerformanceTracker _performanceProvider;

		private long _cachedMissionSkipPurchaseId = -1L;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<ISubwayBaseEvent> OnEventPosted__BackingField;

		AnalyticsManager IAnalyticsIntegrationTest.Manager
		{
			get
			{
				return _manager;
			}
		}

		public AnalyticsSettings DebugSettings
		{
			get
			{
				return _debugSettings;
			}
		}

		public bool OnlyValidatedPurchases
		{
			get
			{
				return _settings.OnlyValidatedPurchases;
			}
		}

		private bool IsSupportedOnDevice
		{
			get
			{
				return true;
			}
		}

		private bool IsPartOfSample
		{
			get
			{
				return _sampleState == SampleState.SampleKeyIncluded || _sampleState == SampleState.ExplicitIncluded;
			}
		}

		private bool IsActive
		{
			get
			{
				return _settings.Enabled && IsPartOfSample;
			}
		}

		private int GameCountInSession
		{
			get
			{
				return _lastGameIdInSession + 1;
			}
		}

		public static event Action<ISubwayBaseEvent> OnEventPosted
		{
			add
			{
				Action<ISubwayBaseEvent> action = OnEventPosted__BackingField;
				Action<ISubwayBaseEvent> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnEventPosted__BackingField, (Action<ISubwayBaseEvent>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<ISubwayBaseEvent> action = OnEventPosted__BackingField;
				Action<ISubwayBaseEvent> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnEventPosted__BackingField, (Action<ISubwayBaseEvent>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		static AnalyticsIntegration()
		{
			AnalyticsUserIdPersistedDefault = string.Empty;
			PreviousClientVersionPersistedDefault = string.Empty;
			GamesSinceFlushPersistedDefault = 0;
			CurrentSessionPersistedDefault = -1;
			OperatingSystemPersistedDefault = string.Empty;
			DeviceIDPersistedDefault = string.Empty;
			OSLanguagePersistedDefault = string.Empty;
			PerformanceLevelPersistedDefault = DeviceInfo.PerformanceLevel.Low;
			GpuScorePersistedDefault = -1;
			OnlineSettingsLocalePersistedDefault = string.Empty;
			UnhandledAbTestDataPersistedDefault = string.Empty;
			SampleStateDefault = SampleState.Unsampled;
			PrefsKeyPersistedLeavesForegroundTimeDefault = string.Empty;
			_jsonClassInfoCached = false;
			ABTestManager.onTagAdded = (Action<string, int>)Delegate.Combine(ABTestManager.onTagAdded, new Action<string, int>(ABTestManager_OnTagAdded));
		}

		private void Start()
		{
			Singleton<LogManager>.GetInstance();
			if (Application.isPlaying)
			{
				_samplingKey = AnalyticsSave.analyticsSamplingKey;
				_sampleState = SyboAnalyticsSave.syboAnalyticsSampleState;
				InitializeFromSettings();
				OnlineSettings instance = OnlineSettings.instance;
				instance.onOnlineSettingsDownloadResult = (Action<bool>)Delegate.Combine(instance.onOnlineSettingsDownloadResult, new Action<bool>(OnlineSettings_OnDownloaded));
			}
			UIScreenController.OnAwake += UIScreenController_OnAwake;
		}

		private static void ABTestManager_OnTagAdded(string testTag, int segmentIndex)
		{
			try
			{
				if (segmentIndex == -1)
				{
					Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : ABTestManager_OnTagAdded: received testTag '{1}' without valid segmentIndex", Time.time, testTag);
				}
				else if (SingletonMonoBehaviour<AnalyticsIntegration>.Instance._manager == null || SingletonMonoBehaviour<AnalyticsIntegration>.Instance._waitingToInitialize)
				{
					SyboAnalyticsSave.SyboAnalyticsAddUnhandledAbTestData(testTag, segmentIndex);
				}
				else
				{
					SingletonMonoBehaviour<AnalyticsIntegration>.Instance.PostEvent(new JoinedABTestEvent(testTag, segmentIndex));
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in ABTestManager_OnTagAdded: {1}", Time.time, arg);
			}
		}

		private void UIScreenController_OnAwake()
		{
			UIScreenController.OnAwake -= UIScreenController_OnAwake;
			UIScreenController.OnChangedScreen += UIScreenController_OnChangedScreen;
		}

		protected override void OnDestroy()
		{
			UpdateEventHandlersPostingAnalyticsEvents(false);
			if (_manager != null)
			{
				_manager.Shutdown();
			}
			base.OnDestroy();
		}

		private void OnlineSettings_OnDownloaded(bool success)
		{
			if (success)
			{
				bool flag = Game.IsInstantiated && Game.Instance.IsInGame.Value;
				if (!flag)
				{
					InitializeFromSettings();
				}
				else if (!_waitingToInitialize)
				{
					UpdateFrontScreenHandler(true);
				}
				_waitingToInitialize = _waitingToInitialize || flag;
			}
		}

		private void InitializeSettings()
		{
			bool flag = _useDebugSettingsOnce && _usedDebugSettings;
			if (Application.isEditor && _useDebugSettings && !flag)
			{
				_settings.CopyFrom(_debugSettings);
				_usedDebugSettings = true;
				return;
			}
			_settings.Enabled = IsSupportedOnDevice && OnlineSettings.instance.GetValue("sybo_analytics_enabled", false);
			_settings.FlushEnabled = OnlineSettings.instance.GetValue("sybo_analytics_flush_enabled", true);
			_settings.SamplingThreshold = OnlineSettings.instance.GetValue("sybo_analytics_sampling_threshold", 0f);
			_settings.BaseURL = OnlineSettings.instance.GetValue("sybo_analytics_endpoint_baseurl", null);
			_settings.UserAgentString = GetUserAgentString();
			_settings.MaxGamesWithoutFlush = OnlineSettings.instance.GetValue("sybo_analytics_max_games_without_flush", 10);
			_settings.GamesInSessionBeforeFirstFlush = OnlineSettings.instance.GetValue("sybo_analytics_games_in_session_before_first_flush", 1);
			_settings.OnlyValidatedPurchases = OnlineSettings.instance.GetValue("sybo_analytics_only_validated_purchases", true);
			if (Application.isEditor && _settings.BaseURL != null && _settings.BaseURL.Contains("prod"))
			{
				Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Seems like trying to use production endpoint in Editor. Disabling", Time.time);
				_settings.Enabled = false;
			}
		}

		private void InitializeSampleState()
		{
			if (_settings.Enabled)
			{
				bool flag = 0f < _settings.SamplingThreshold && _samplingKey <= _settings.SamplingThreshold;
				if (_sampleState == SampleState.Unsampled && flag)
				{
					_sampleState = SampleState.SampleKeyIncluded;
					SyboAnalyticsSave.syboAnalyticsSampleState = _sampleState;
				}
			}
		}

		private void InitializeFromSettings()
		{
			bool isActive = IsActive;
			bool isPartOfSample = IsPartOfSample;
			InitializeSettings();
			InitializeSampleState();
			if (_sessionTracker == null)
			{
				_sessionTracker = new SessionTracker(_settings.SessionTimeoutSeconds);
			}
			bool flag = !isActive && IsActive;
			bool flag2 = isActive != IsActive;
			bool flag3 = !isPartOfSample && IsPartOfSample;
			bool flag4 = HasUserInPlayerInfo();
			if (_settings.Enabled)
			{
				if (_manager != null && _manager.Endpoint.BaseURL != _settings.BaseURL)
				{
					UpdateEndpoint();
				}
				bool flag5 = _manager == null && (!flag4 || IsPartOfSample);
				bool flag6 = _manager != null && flag;
				if (flag5)
				{
					Initialize();
				}
				else if (flag6)
				{
					PostSessionAndForegroundEvents();
				}
				if (!IsPartOfSample)
				{
					SingletonMonoBehaviour<AggregatedDataTracking>.Instance.Initialize(flag4, _settings.SessionTimeoutSeconds);
				}
				else if (flag3 && flag4)
				{
					AddUserToSample(JoinedSampleSource.SampleThreshold, -1L);
				}
			}
			if (flag2)
			{
				_sessionTracker.RegisterLastActiveMsTicks();
			}
			UpdateEventHandlersPostingAnalyticsEvents(IsActive);
		}

		private static void CacheJsonClassInfo()
		{
			if (_jsonClassInfoCached)
			{
				return;
			}
			_jsonClassInfoCached = true;
			Type typeFromHandle = typeof(AnalyticsEvent);
			Type[] types = Assembly.GetAssembly(typeFromHandle).GetTypes();
			Type[] array = types;
			foreach (Type type in array)
			{
				if (type.IsSubclassOf(typeFromHandle))
				{
					JsonClassInfo.GetJsonTypeReflection(type);
					JsonClassInfo.GetJsonTypeReflection(typeof(TransferEvent<>).MakeGenericType(type));
				}
			}
		}

		private bool HasUserInPlayerInfo()
		{
			return !string.IsNullOrEmpty(SyboAnalyticsSave.syboAnalyticsUserId);
		}

		private void UpdateEndpoint()
		{
			_manager.Endpoint = CreatePostAnalyticsProvider();
		}

		private PostAnalyticsProvider CreatePostAnalyticsProvider()
		{
			MinimalHTTPPost httpLayer = new MinimalHTTPPost(this, _settings.WebRequestTimeoutSeconds);
			return new PostAnalyticsProvider(_settings.BaseURL, _settings.ApplicationID, _settings.ApiVersion, _settings.UserAgentString, _settings.Key, httpLayer);
		}

		private void UpdateEventHandlersPostingAnalyticsEvents(bool subscribe)
		{
			UpdateFrontScreenHandler(subscribe);
			if (subscribe && !_subscribedEventPostingHandlers)
			{
				RewardManager.OnMysteryBoxAddedForUnlock += RewardManager_OnMysteryBoxAddedForUnlock;
				RewardManager.OnRewardPaidOut += RewardManager_OnRewardPaidOut;
				CelebrationPopup.OnRewardMultiplied += CelebrationPopup_OnRewardMultiplied;
				WordHuntManager.OnDailyLetterCoinsRewarded += PlayerInfo_OnDailyLetterCoinsRewarded;
				OfferWallManager.instance.onPendingRewardsReceived += OfferWallManager_OnPendingRewardsReceived;
				DoubleCoinsVideo.onDoubleCoinsReceived = (Action<CelebrationReward>)Delegate.Combine(DoubleCoinsVideo.onDoubleCoinsReceived, new Action<CelebrationReward>(OnDoubleCoinsReceived));
				SellConsumablesHelper.OnRewardWithVideo = (Action<InAppItemModel.ItemType, int>)Delegate.Combine(SellConsumablesHelper.OnRewardWithVideo, new Action<InAppItemModel.ItemType, int>(OnRewardConsumablesWithVideo));
				EarnCoinsData.OnRewardURLOpened += EarnCoinsData_OnRewardURLOpened;
				FacebookRewardsInfo.OnFacebookRewardPaidOut += FacebookRewardsPopup_OnFacebookRewardPaidOut;
				FacebookRewardsInfo.OnFacebookCharacterReceived += FacebookRewardsPopup_OnFacebookCharacterReceived;
				FriendHelperCrew.OnFriendCoinsReceived += FriendHelperCrew_OnFriendCoinsReceived;
				Game.OnGameStarted += Game_OnGameStarted;
				Game.OnRunStarted += Game_OnRunStarted;
				Game.OnPlayerDied += Game_OnPlayerDied;
				Game.OnGameEnded += Game_OnGameEnded;
				PurchaseHandler.Instance.OnIngameCurrencyPurchaseCompleted += PurchaseHandler_OnIngameCurrencyPurchaseCompleted;
				CharacterNameEventManager.OnCharacterSkippedWithKeys += CharacterNameEventManager_OnCharacterSkippedWithKeys;
				CharacterNameEventManager.OnCharacterSkippedWithVideo += CharacterNameEventManager_OnCharacterSkippedWithVideo;
				MissionsManager.onMissionComplete += MissionManager_OnMissionComplete;
				MissionsManager instance = MissionsManager.Instance;
				instance.OnMissionSkipped = (Action<bool, int>)Delegate.Combine(instance.OnMissionSkipped, new Action<bool, int>(MissionManager_OnMissionSkipped));
				CloudSave.onCloudSaveDisabled += CloudSave_OnDisableCloudSave;
				CloudSave.onCloudSaveRestored += CloudSave_OnRestore;
				PromoCodeManager instance2 = PromoCodeManager.instance;
				instance2.promoCodeUsed = (Action<object, PromoCodeEventArgs>)Delegate.Combine(instance2.promoCodeUsed, new Action<object, PromoCodeEventArgs>(HandlePromotionCodeConfirmed));
				_subscribedEventPostingHandlers = true;
			}
			else if (!subscribe && _subscribedEventPostingHandlers)
			{
				Game.OnMenuScreenShown -= Game_OnMenuScreenShown;
				RewardManager.OnMysteryBoxAddedForUnlock -= RewardManager_OnMysteryBoxAddedForUnlock;
				RewardManager.OnRewardPaidOut -= RewardManager_OnRewardPaidOut;
				CelebrationPopup.OnRewardMultiplied -= CelebrationPopup_OnRewardMultiplied;
				WordHuntManager.OnDailyLetterCoinsRewarded -= PlayerInfo_OnDailyLetterCoinsRewarded;
				OfferWallManager.instance.onPendingRewardsReceived -= OfferWallManager_OnPendingRewardsReceived;
				DoubleCoinsVideo.onDoubleCoinsReceived = (Action<CelebrationReward>)Delegate.Remove(DoubleCoinsVideo.onDoubleCoinsReceived, new Action<CelebrationReward>(OnDoubleCoinsReceived));
				SellConsumablesHelper.OnRewardWithVideo = (Action<InAppItemModel.ItemType, int>)Delegate.Remove(SellConsumablesHelper.OnRewardWithVideo, new Action<InAppItemModel.ItemType, int>(OnRewardConsumablesWithVideo));
				EarnCoinsData.OnRewardURLOpened -= EarnCoinsData_OnRewardURLOpened;
				FacebookRewardsInfo.OnFacebookRewardPaidOut -= FacebookRewardsPopup_OnFacebookRewardPaidOut;
				FacebookRewardsInfo.OnFacebookCharacterReceived -= FacebookRewardsPopup_OnFacebookCharacterReceived;
				FriendHelperCrew.OnFriendCoinsReceived -= FriendHelperCrew_OnFriendCoinsReceived;
				Game.OnGameStarted -= Game_OnGameStarted;
				Game.OnRunStarted -= Game_OnRunStarted;
				Game.OnPlayerDied -= Game_OnPlayerDied;
				Game.OnGameEnded -= Game_OnGameEnded;
				PurchaseHandler.Instance.OnIngameCurrencyPurchaseCompleted -= PurchaseHandler_OnIngameCurrencyPurchaseCompleted;
				CharacterNameEventManager.OnCharacterSkippedWithKeys -= CharacterNameEventManager_OnCharacterSkippedWithKeys;
				CharacterNameEventManager.OnCharacterSkippedWithVideo -= CharacterNameEventManager_OnCharacterSkippedWithVideo;
				MissionsManager.onMissionComplete -= MissionManager_OnMissionComplete;
				MissionsManager instance3 = MissionsManager.Instance;
				instance3.OnMissionSkipped = (Action<bool, int>)Delegate.Remove(instance3.OnMissionSkipped, new Action<bool, int>(MissionManager_OnMissionSkipped));
				CloudSave.onCloudSaveDisabled -= CloudSave_OnDisableCloudSave;
				CloudSave.onCloudSaveRestored -= CloudSave_OnRestore;
				PromoCodeManager instance4 = PromoCodeManager.instance;
				instance4.promoCodeUsed = (Action<object, PromoCodeEventArgs>)Delegate.Remove(instance4.promoCodeUsed, new Action<object, PromoCodeEventArgs>(HandlePromotionCodeConfirmed));
				_subscribedEventPostingHandlers = false;
			}
			if (_settings.Enabled)
			{
				if (!_subscribedToInAppManager)
				{
					InAppManager.instance.onPurchaseSuccess += InAppManager_OnPurchaseSuccess;
					_subscribedToInAppManager = true;
				}
			}
			else if (_subscribedToInAppManager)
			{
				if (InAppManager.instance != null)
				{
					InAppManager.instance.onPurchaseSuccess -= InAppManager_OnPurchaseSuccess;
				}
				_subscribedToInAppManager = false;
			}
		}

		private void UpdateFrontScreenHandler(bool subscribe)
		{
			if (subscribe && !_subscribedToFrontScreenEvent)
			{
				Game.OnMenuScreenShown += Game_OnMenuScreenShown;
				_subscribedToFrontScreenEvent = true;
			}
			else if (!subscribe && _subscribedToFrontScreenEvent)
			{
				Game.OnMenuScreenShown -= Game_OnMenuScreenShown;
				_subscribedToFrontScreenEvent = false;
			}
		}

		private void Initialize()
		{
			CacheJsonClassInfo();
			_performanceProvider = new PerformanceTracker(2000f);
			InitializeManager();
		}

		private void InitializeManager()
		{
			try
			{
				if (_manager != null)
				{
					return;
				}
				SessionStartedEvent.GetIncrementedSessionID = GetIncrementedSessionID;
				BaseSessionEvent.GetSessionID = GetSessionID;
				BaseSessionEvent.GetSessionDelta = GetSessionDelta;
				BaseSessionEvent.GetIncrementedSessionEventNumber = GetIncrementedSessionEventNumber;
				BaseSessionEvent.GetDeviceID = GetDeviceID;
				BaseSessionEvent.GetOperatingSystem = GetOperatingSystem;
				AnalyticsEvent.GetSamplingKey = GetSamplingKey;
				PostAnalyticsProvider endpoint = CreatePostAnalyticsProvider();
				PersistentAnalyticsEventLog persistentAnalyticsEventLog = new PersistentAnalyticsEventLog(DeviceUtility.GetUserDataPath(), _settings.ChunkSizeThresholdKB, _settings.StorageLimitKB);
				string syboAnalyticsUserId = SyboAnalyticsSave.syboAnalyticsUserId;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				string bundleVersion = DeviceUtility.GetBundleVersion();
				LogWarningIfWindowsPhone();
				if (string.IsNullOrEmpty(syboAnalyticsUserId))
				{
					if (persistentAnalyticsEventLog.HasData)
					{
						persistentAnalyticsEventLog.Wipe();
					}
					_manager = new AnalyticsManager(endpoint, persistentAnalyticsEventLog, this);
					syboAnalyticsUserId = (SyboAnalyticsSave.syboAnalyticsUserId = (SyboAnalyticsSave.localSyboAnalyticsUserId = GetNewUserId()));
					_manager.ChangeUserID(syboAnalyticsUserId);
					flag = true;
					flag2 = true;
					SyboAnalyticsSave.syboAnalyticsPreviousClientVersion = bundleVersion;
				}
				else
				{
					_manager = new AnalyticsManager(endpoint, persistentAnalyticsEventLog, this);
					_manager.SetUserID(syboAnalyticsUserId);
					string syboAnalyticsPreviousClientVersion = SyboAnalyticsSave.syboAnalyticsPreviousClientVersion;
					if (syboAnalyticsPreviousClientVersion != bundleVersion)
					{
						flag3 = true;
						flag = IsPartOfSample;
						SyboAnalyticsSave.syboAnalyticsPreviousClientVersion = bundleVersion;
					}
					else
					{
						flag = IsPartOfSample && _lastGameIdInSession == 0 && persistentAnalyticsEventLog.HasData;
					}
				}
				_manager.GameVersion = DeviceUtility.GetBundleVersion();
				if (SyboAnalyticsSave.syboAnalyticsCurrentSession != CurrentSessionPersistedDefault)
				{
					PostLeavesForegroundEvent(false);
				}
				SubwaySessionStartedEvent subwaySessionStartedEvent = new SubwaySessionStartedEvent();
				subwaySessionStartedEvent.HighScore = PlayerSave.highScore;
				subwaySessionStartedEvent.CoinBalance = InventorySave.coins;
				subwaySessionStartedEvent.KeyBalance = InventorySave.keys;
				PostEvent(subwaySessionStartedEvent, flag2);
				if (flag2)
				{
					SubwayAccountCreatedEvent eventData = new SubwayAccountCreatedEvent(AnalyticsSave.flurryUserId, "android", UserSave.firstInstallDate);
					PostEvent(eventData, true);
				}
				if (flag3)
				{
					SubwayClientUpdatedEvent eventData2 = new SubwayClientUpdatedEvent(AnalyticsSave.flurryUserId, "android", UserSave.firstInstallDate);
					PostEvent(eventData2);
				}
				PostDeviceInfoIfDataChanged();
				PostEvent(new SubwayAppEntersForegroundEvent());
				if (flag2 && _sampleState == SampleState.SampleKeyIncluded)
				{
					AggregatedDataTracking.DataContainer dataContainer = new AggregatedDataTracking.DataContainer(AggregatedDataTracking.DataCompleteness.Full, 5);
					dataContainer.SessionCount++;
					SyboAnalyticsSave.syboAggregatedDataString = AggregatedDataTracking.DataContainer.Serialize(dataContainer);
					AddUserToSample(JoinedSampleSource.SampleThreshold, -1L);
				}
				_isReadyToPostGameEvents = !Game.IsInstantiated || (Game.IsInstantiated && !Game.Instance.IsInGame.Value);
				if (flag)
				{
					Flush();
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in InitializeManager: {1}", Time.time, arg);
			}
		}

		private static string GetNewUserId()
		{
			return Guid.NewGuid().ToString();
		}

		private string GetUserAgentString()
		{
			string bundleVersion = DeviceUtility.GetBundleVersion();
			string[] array = bundleVersion.Split('.');
			string text = array[0].PadLeft(2, '0');
			string text2 = array[1].PadLeft(2, '0');
			string text3 = array[2].PadLeft(2, '0');
			return string.Format("{0}/{1}.{2}.{3}", _settings.ApplicationID, text, text2, text3);
		}

		private void AddUserToSample(JoinedSampleSource joinedSampleSource, long sourceId)
		{
			PostJoinedSampleEvent(joinedSampleSource, sourceId);
			HandleNewAbTestTags();
		}

		private void PostJoinedSampleEvent(JoinedSampleSource joinedSampleSource, long sourceId)
		{
			try
			{
				AggregatedDataTracking.DataContainer dataContainer = null;
				string syboAggregatedDataString = SyboAnalyticsSave.syboAggregatedDataString;
				dataContainer = (string.IsNullOrEmpty(syboAggregatedDataString) ? new AggregatedDataTracking.DataContainer(AggregatedDataTracking.DataCompleteness.None) : AggregatedDataTracking.DataContainer.Deserialize(syboAggregatedDataString));
				JoinedSampleEvent eventData = new JoinedSampleEvent(joinedSampleSource, sourceId, dataContainer.DataCompleteness, Mathf.Max(dataContainer.UniqueDaysPlayed, 1), dataContainer.SessionCount, InventorySave.keys, dataContainer.KeysReceived, InventorySave.coins, dataContainer.CoinsReceived, dataContainer.MissingKeysCount, dataContainer.MissingCoinsCount, PlayerSave.highScore);
				PostEvent(eventData);
				foreach (AggregatedDataTracking.UnsampledPurchaseData unsampledPurchase in dataContainer.UnsampledPurchases)
				{
					UnsampledIngameCurrencyPurchasesEvent eventData2 = new UnsampledIngameCurrencyPurchasesEvent(unsampledPurchase.ItemTitleId, unsampledPurchase.Count);
					PostEvent(eventData2);
				}
				if (SingletonMonoBehaviour<AggregatedDataTracking>.IsInstantiated())
				{
					SingletonMonoBehaviour<AggregatedDataTracking>.Instance.StopAggregatedDataTracking();
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in PostJoinedSampleEvent: {1}", Time.time, arg);
			}
		}

		private string GetDeviceID()
		{
			string deviceIdentifier = DeviceUtility.GetDeviceIdentifier();
			if (!string.IsNullOrEmpty(deviceIdentifier))
			{
				using (SHA256Managed sHA256Managed = new SHA256Managed())
				{
					byte[] bytes = Encoding.Unicode.GetBytes(deviceIdentifier);
					byte[] inArray = sHA256Managed.ComputeHash(bytes);
					return Convert.ToBase64String(inArray);
				}
			}
			return null;
		}

		private void PostDeviceInfoIfDataChanged()
		{
			try
			{
				DeviceInfoEvent deviceInfoEvent = new DeviceInfoEvent();
				if (SyboAnalyticsSave.syboAnalyticsOperatingSystem != deviceInfoEvent.DeviceOS || SyboAnalyticsSave.syboAnalyticsDeviceID != deviceInfoEvent.DeviceID || SyboAnalyticsSave.syboAnalyticsOSLanguage != deviceInfoEvent.Language || SyboAnalyticsSave.syboAnalyticsPerformanceLevel != deviceInfoEvent.GetPerformanceLevel() || SyboAnalyticsSave.syboAnalyticsGpuScore != deviceInfoEvent.GpuScore || SyboAnalyticsSave.syboAnalyticsOnlineSettingsLocale != deviceInfoEvent.OnlineSettingsLocale)
				{
					PostEvent(deviceInfoEvent, true);
				}
				SyboAnalyticsSave.syboAnalyticsOperatingSystem = deviceInfoEvent.DeviceOS;
				SyboAnalyticsSave.syboAnalyticsDeviceID = deviceInfoEvent.DeviceID;
				SyboAnalyticsSave.syboAnalyticsOSLanguage = deviceInfoEvent.Language;
				SyboAnalyticsSave.syboAnalyticsPerformanceLevel = deviceInfoEvent.GetPerformanceLevel();
				SyboAnalyticsSave.syboAnalyticsGpuScore = deviceInfoEvent.GpuScore;
				SyboAnalyticsSave.syboAnalyticsOnlineSettingsLocale = deviceInfoEvent.OnlineSettingsLocale;
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in PostDeviceInfoIfDataChanged: {1}", Time.time, arg);
			}
		}

		private void OnValidate()
		{
			if (_settings != null)
			{
				_settings.SetDefaults();
			}
		}

		private void Reset()
		{
			_settings.SetDefaults();
			_debugSettings.SetDebugDefaults();
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (_manager != null && IsActive)
			{
				if (focusStatus)
				{
					PostSessionAndForegroundEvents();
				}
				else
				{
					PlayerPrefs.SetString("LeavesForegroundTime", AnalyticsEvent.GetEventTime().ToBinary().ToString());
					PlayerPrefs.SetInt("LeavesForegroundSessionDelta", GetSessionDelta());
					PlayerPrefs.Save();
				}
				_sessionTracker.RegisterLastActiveMsTicks();
			}
		}

		private void OnApplicationQuit()
		{
			if (IsActive)
			{
				PostLeavesForegroundEvent(true);
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				Flush();
			}
		}

		public void Flush()
		{
			if (_manager != null && IsPartOfSample && _settings.FlushEnabled)
			{
				if (!_manager.HasValidUserID)
				{
					Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Attempted to flush events before initializing user ID", Time.time);
					return;
				}
				SyboAnalyticsSave.syboAnalyticsGamesSinceFlush = 0;
				_hasUnflushedPurchaseEvent = false;
				_manager.Flush();
			}
		}

		private void PostSessionAndForegroundEvents()
		{
			try
			{
				long mSTicksSinceLastActive = _sessionTracker.GetMSTicksSinceLastActive();
				bool flag = IsTicksSinceLastActiveOverForegroundChangeThreshold(mSTicksSinceLastActive);
				if (flag)
				{
					PostLeavesForegroundEvent(false);
				}
				if ((!Game.IsInstantiated || !Game.Instance.IsInGame.Value) && _sessionTracker.ShouldStartNewSession(mSTicksSinceLastActive))
				{
					SubwaySessionStartedEvent subwaySessionStartedEvent = new SubwaySessionStartedEvent();
					subwaySessionStartedEvent.HighScore = PlayerSave.highScore;
					subwaySessionStartedEvent.CoinBalance = InventorySave.coins;
					subwaySessionStartedEvent.KeyBalance = InventorySave.keys;
					PostEvent(subwaySessionStartedEvent);
				}
				if (flag)
				{
					PostEvent(new SubwayAppEntersForegroundEvent());
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in PostSessionAndForegroundEvents: {1}", Time.time, arg);
			}
		}

		private bool IsTicksSinceLastActiveOverForegroundChangeThreshold(long msTicksSinceLastActive)
		{
			return 2000 < msTicksSinceLastActive;
		}

		private void PostLeavesForegroundEvent(bool skipTimeThresholdCheck)
		{
			try
			{
				string value = PlayerPrefs.GetString("LeavesForegroundTime", PrefsKeyPersistedLeavesForegroundTimeDefault);
				if (skipTimeThresholdCheck)
				{
					PostEvent(new SubwayAppLeavesForegroundEvent());
				}
				else if (!string.IsNullOrEmpty(value) && IsTicksSinceLastActiveOverForegroundChangeThreshold(_sessionTracker.GetMSTicksSinceLastActive()))
				{
					long dateData = Convert.ToInt64(value);
					DateTime eventTime = DateTime.FromBinary(dateData);
					int sessionDelta = PlayerPrefs.GetInt("LeavesForegroundSessionDelta", -1);
					PostEvent(new SubwayAppLeavesForegroundEvent(eventTime, sessionDelta));
				}
				PlayerPrefs.SetString("LeavesForegroundTime", PrefsKeyPersistedLeavesForegroundTimeDefault);
				PlayerPrefs.SetInt("LeavesForegroundSessionDelta", -1);
				PlayerPrefs.Save();
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in PostLeavesForegroundEvent: {1}", Time.time, arg);
			}
		}

		private int GetIncrementedSessionID()
		{
			int result = ++SyboAnalyticsSave.syboAnalyticsCurrentSession;
			_msTicksAtCurrentSessionStart = AnalyticsManager.GetMsTicksSinceDeviceStartup();
			SyboAnalyticsSave.persistedSessionEventNumber = -1;
			return result;
		}

		private int GetSessionID()
		{
			return SyboAnalyticsSave.syboAnalyticsCurrentSession;
		}

		private int GetSessionDelta()
		{
			long num = (AnalyticsManager.GetMsTicksSinceDeviceStartup() - _msTicksAtCurrentSessionStart) / 1000;
			return (int)num;
		}

		private int GetIncrementedSessionEventNumber()
		{
			SyboAnalyticsSave.persistedSessionEventNumber++;
			return SyboAnalyticsSave.persistedSessionEventNumber;
		}

		private float GetSamplingKey()
		{
			return _samplingKey;
		}

		private static string GetOperatingSystem()
		{
			string text = SystemInfo.operatingSystem;
			if (Application.platform == RuntimePlatform.Android)
			{
				text = AnalyticsHelper.GetAndroidDeviceOsString(text);
			}
			return text;
		}

		private static string GetPlatformVendor()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return "google";
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return "apple";
			}
			if (Application.platform == RuntimePlatform.MetroPlayerARM || Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64)
			{
				return "microsoft";
			}
			if (Application.isEditor)
			{
				return "editor";
			}
			Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Could not determine purchase platform!", Time.time);
			return null;
		}

		private void PostEvent<T>(T eventData, bool ignoreSampleCheck = false) where T : AnalyticsEvent
		{
			if (!_manager.HasValidUserID)
			{
				Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Attempted to post '{1}' event before initializing user ID", Time.time, eventData.EventName);
				return;
			}
			bool flag = ignoreSampleCheck || IsPartOfSample;
			if (_settings.Enabled && flag)
			{
				_manager.PostEvent(eventData);
				if (OnEventPosted__BackingField != null)
				{
					OnEventPosted__BackingField(eventData as ISubwayBaseEvent);
				}
			}
			_hasUnflushedPurchaseEvent = _hasUnflushedPurchaseEvent || typeof(SubwayPurchaseEvent).IsAssignableFrom(typeof(T));
		}

		private void Game_OnMenuScreenShown()
		{
			if (_waitingToInitialize)
			{
				InitializeFromSettings();
				_waitingToInitialize = false;
			}
			_isReadyToPostGameEvents = true;
			HandleNewAbTestTags();
		}

		private void Game_OnGameStarted()
		{
			try
			{
				SyboAnalyticsSave.syboAnalyticsGamesSinceFlush++;
				if (!_isReadyToPostGameEvents)
				{
					return;
				}
				if (_lastSession != SyboAnalyticsSave.syboAnalyticsCurrentSession)
				{
					_lastGameIdInSession = 0;
				}
				else
				{
					_lastGameIdInSession++;
				}
				_lastSession = SyboAnalyticsSave.syboAnalyticsCurrentSession;
				Characters.Model model = Characters.characterData[PlayerSave.currentCharacterId];
				Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(PlayerSave.currentCharacterId, PlayerSave.currentOutfitIndex);
				string characterOutfit = null;
				if (outfitForCharacter != null)
				{
					characterOutfit = Strings.GetDefault(outfitForCharacter.title);
				}
				Boards.Board board = Boards.boardData[PlayerSave.currentBoardId];
				List<HoverboardThemes.HoverboardTheme> list = HoverboardThemes.TryGetCustomThemesForBoard(PlayerSave.currentBoardId);
				string text = null;
				string text2 = null;
				string text3 = ",";
				if (list != null)
				{
					foreach (HoverboardThemes.HoverboardTheme item in list)
					{
						if (BoardManager.IsThemeActive(PlayerSave.currentBoardId, item.identifier))
						{
							if (text != null)
							{
								text += text3;
								text2 += text3;
							}
							text += item.power;
							text2 += Strings.GetDefault(item.title);
						}
					}
				}
				GameStartedEvent gameStartedEvent = new GameStartedEvent();
				gameStartedEvent.GameID = _lastGameIdInSession;
				gameStartedEvent.Character = Strings.GetDefault(model.name);
				gameStartedEvent.CharacterOutfit = characterOutfit;
				gameStartedEvent.Board = Strings.GetDefault(board.name);
				gameStartedEvent.BoardAbilityTypesAdded = text;
				gameStartedEvent.BoardAbilityDescriptionsAdded = text2;
				gameStartedEvent.PlayerLevel = (short)ScoreMultiplierInfo.rawMultiplier;
				gameStartedEvent.HighScore = PlayerSave.highScore;
				gameStartedEvent.CoinBalance = InventorySave.coins;
				gameStartedEvent.KeyBalance = InventorySave.keys;
				gameStartedEvent.HoverboardBalance = InventorySave.GetAmount(UpgradeType.hoverboard);
				gameStartedEvent.HeadStartBalance = InventorySave.GetAmount(UpgradeType.headstart2000);
				gameStartedEvent.ScoreBoosterBalance = InventorySave.GetAmount(UpgradeType.scorebooster);
				PostEvent(gameStartedEvent);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in Game_OnGameStarted: {1}", Time.time, arg);
			}
		}

		private void Game_OnRunStarted(bool wasPlayerRevived, bool revivedWithAds, bool adsWasAvailable, bool adsFailed)
		{
			try
			{
				_isRunInProgress = true;
				if (_isReadyToPostGameEvents && wasPlayerRevived)
				{
					PostEvent(new GameContinuedEvent(_lastGameIdInSession, false, revivedWithAds, adsWasAvailable, adsFailed));
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in Game_OnGameStarted. {1}", Time.time, arg);
			}
		}

		private void Game_OnGameEnded()
		{
			if (_isRunInProgress)
			{
				LogRunSummary();
				_isRunInProgress = false;
			}
			FlushIfNeeded();
		}

		private void FlushIfNeeded()
		{
			if (IsPartOfSample)
			{
				bool flag = _settings.GamesInSessionBeforeFirstFlush != -1 && _settings.GamesInSessionBeforeFirstFlush == GameCountInSession && 0 < GameCountInSession;
				bool flag2 = _settings.MaxGamesWithoutFlush != -1 && _settings.MaxGamesWithoutFlush <= SyboAnalyticsSave.syboAnalyticsGamesSinceFlush;
				if (flag || flag2)
				{
					Flush();
				}
			}
		}

		private void LogRunSummary()
		{
			try
			{
				if (_isReadyToPostGameEvents)
				{
					GameStats instance = GameStats.Instance;
					RunSummaryEvent runSummaryEvent = new RunSummaryEvent();
					runSummaryEvent.GameID = _lastGameIdInSession;
					runSummaryEvent.Score = instance.Score;
					int powerupActivationCountLastRun = instance.GetPowerupActivationCountLastRun(UpgradeType.hoverboard);
					int mysteryPowerupActivationCountLastRun = instance.GetMysteryPowerupActivationCountLastRun(UpgradeType.hoverboard);
					int num = powerupActivationCountLastRun - mysteryPowerupActivationCountLastRun;
					if (num < 0)
					{
						Log.Analytics.LogErrorFormat("Computed consumed hoverboard counts as {0}. Report 0.", num);
						num = 0;
					}
					runSummaryEvent.HoverboardsUsed = (short)num;
					runSummaryEvent.HeadStartsUsed = (short)instance.GetPowerupActivationCountLastRun(UpgradeType.headstart2000);
					runSummaryEvent.ScoreMultiplierCollected = instance.DoubleMultiplierPickupsCollectedRun;
					runSummaryEvent.ScoreMultiplierSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.MultiplierScore2x);
					runSummaryEvent.ScoreBoosterUsed = (short)instance.ScoreBoosterTierRun;
					runSummaryEvent.CoinsCollected = instance.CoinsCollectedRun;
					runSummaryEvent.CoinsCollectedRaw = instance.CoinsCollectedRawRun;
					runSummaryEvent.CoinsSpawned = instance.CoinsSpawnedRun;
					runSummaryEvent.CoinMagnetsCollected = instance.CoinMagnetPickupsCollectedRun;
					runSummaryEvent.CoinMagnetsSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.CoinMagnet);
					runSummaryEvent.JetpacksCollected = instance.JetpackPickupsCollectedRun;
					runSummaryEvent.JetpacksSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.JetPack);
					runSummaryEvent.SuperSneakersCollected = instance.SuperSneakerPickupsCollectedRun;
					runSummaryEvent.SuperSneakersSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.SuperSneakers);
					runSummaryEvent.MysteryBoxesCollected = instance.MysteryBoxPickupsCollectedRun;
					runSummaryEvent.MysteryBoxesSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.MysteryBox);
					runSummaryEvent.SuperMysteryBoxesCollected = instance.SuperMysteryBoxPickupsCollectedRun;
					runSummaryEvent.SuperMysteryBoxesSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.SuperMysteryBox);
					runSummaryEvent.KeysCollected = instance.KeyPickupsCollectedRun;
					runSummaryEvent.KeysSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.Key);
					runSummaryEvent.DailyLettersCollected = instance.DailyLetterPickupsCollectedRun;
					runSummaryEvent.DailyLettersSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.LetterHuntDaily);
					runSummaryEvent.CharacterLettersCollected = instance.CharacterLetterPickupsCollectedRun;
					runSummaryEvent.CharacterLettersSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.LetterHuntCharacter);
					runSummaryEvent.TokensCollected = instance.HuntTokensCollectedRun;
					runSummaryEvent.TokensSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.TokenHunt);
					runSummaryEvent.PogoSticksCollected = instance.PogoStickPickupsCollectedRun;
					runSummaryEvent.PogoSticksSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.PogoStick);
					runSummaryEvent.MysteryPowerupsSpawned = instance.GetPickupCountSpawnedLastRun(PickupType.MysteryPowerup);
					runSummaryEvent.MysteryPowerupHoverboardsCollected = instance.GetMysteryPowerupActivationCountLastRun(UpgradeType.hoverboard);
					runSummaryEvent.MysteryPowerupDoubleCoinsCollected = instance.GetMysteryPowerupActivationCountLastRun(UpgradeType.doubleCoinsMysteryPowerup);
					runSummaryEvent.MysteryPowerupDoubleJumpsCollected = instance.GetMysteryPowerupActivationCountLastRun(UpgradeType.doubleJump);
					runSummaryEvent.MysteryPowerupPlusMultipliersCollected = instance.GetMysteryPowerupActivationCountLastRun(UpgradeType.plusMultiplier);
					runSummaryEvent.NumberOfStumbles = instance.StumblesRun;
					runSummaryEvent.Framerate = Mathf.RoundToInt(_performanceProvider.AverageInGameFramerate);
					PostEvent(runSummaryEvent);
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in LogRunSummary. {1}", Time.time, arg);
			}
		}

		private void Game_OnPlayerDied(bool wasPlayerAutoRevived)
		{
			try
			{
				if (_isReadyToPostGameEvents)
				{
					_isRunInProgress = false;
					Game instance = Game.Instance;
					RouteManager instance2 = SingletonMonoBehaviour<RouteManager>.Instance;
					RouteManager.CharacterChunks characterChunksNear = instance2.CharacterChunksNear;
					PlayerDiedEvent playerDiedEvent = new PlayerDiedEvent();
					playerDiedEvent.GameID = _lastGameIdInSession;
					playerDiedEvent.RunTime = (int)instance.ElapsedRunDuration;
					playerDiedEvent.DeathCause = instance.Character.LastDeathCause.ToString();
					playerDiedEvent.LastObstacle = instance.Character.LastObstacleType.ToString();
					playerDiedEvent.Speed = (int)instance.SpeedAtLastObstacleHit;
					playerDiedEvent.Chunk = characterChunksNear.Current.ReportedName;
					if (!string.IsNullOrEmpty(characterChunksNear.PreviousChunkReportedName))
					{
						playerDiedEvent.ChunkPrev = characterChunksNear.PreviousChunkReportedName;
					}
					if (characterChunksNear.Next != null)
					{
						playerDiedEvent.ChunkNext = characterChunksNear.Next.ReportedName;
					}
					playerDiedEvent.PositionInChunk = (int)characterChunksNear.Current.transform.InverseTransformPoint(instance.Character.TransformPosition).z;
					playerDiedEvent.Lane = instance.Character.GetLaneFromIndex(instance.Character.TrackIndexCurrent).ToString();
					playerDiedEvent.MaxFrameTime2s = Mathf.RoundToInt(_performanceProvider.MaxFrameTimeMS);
					PostEvent(playerDiedEvent);
					LogRunSummary();
					if (wasPlayerAutoRevived)
					{
						PostEvent(new GameContinuedEvent(_lastGameIdInSession, wasPlayerAutoRevived, false, true, false));
					}
					_performanceProvider.Reset();
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in Game_OnPlayerDied: {1}", Time.time, arg);
			}
		}

		private void UIScreenController_OnChangedScreen(string screenName)
		{
			if (_menuScreenLast != screenName)
			{
				if (_menuScreenQueue.Count != 0 && 4 <= _menuScreenQueue.Count)
				{
					_menuScreenQueue.Dequeue();
				}
				_menuScreenQueue.Enqueue(screenName);
				_menuScreenLast = screenName;
			}
			if (screenName == "FrontScreen" && _hasUnflushedPurchaseEvent)
			{
				Flush();
			}
		}

		private void PurchaseHandler_OnIngameCurrencyPurchaseCompleted(object sender, PurchaseHandler.SyboInGameCurrencyPurchaseEventArgs e)
		{
			try
			{
				int? gameNumberOrNull = null;
				if (Game.Instance.IsInGame.Value)
				{
					gameNumberOrNull = _lastGameIdInSession;
				}
				if (e.ItemSource == ObtainableItemSource.IngameCurrencyPurchase)
				{
					SubwayIngameCurrencyPurchaseEvent eventData = new SubwayIngameCurrencyPurchaseEvent(e.ItemTitleId, e.ItemType, e.PurchaseId, GetMenusBeforeString(true), gameNumberOrNull, e.InGameCurrencyType, -e.Price, e.CurrencyBalanceAfter);
					PostEvent(eventData);
					if (e.InstanceIds != null)
					{
						foreach (long instanceId in e.InstanceIds)
						{
							SubwayItemAddedEvent eventData2 = new SubwayItemAddedEvent(e.ItemSource, e.PurchaseId, null, e.ItemTitleId, e.ItemType, 1L, 1L, instanceId);
							PostEvent(eventData2);
						}
					}
					else
					{
						SubwayItemAddedEvent eventData3 = new SubwayItemAddedEvent(e.ItemSource, e.PurchaseId, null, e.ItemTitleId, e.ItemType, e.ItemBalanceAfter, e.AmountReceived, e.InstanceId);
						PostEvent(eventData3);
					}
				}
				else if (e.ItemSource == ObtainableItemSource.FreeUnlock)
				{
					SubwayItemAddedEvent eventData4 = new SubwayItemAddedEvent(e.ItemSource, -1L, null, e.ItemTitleId, e.ItemType, e.ItemBalanceAfter, e.AmountReceived);
					PostEvent(eventData4);
				}
				if (e.ItemType == InAppItemModel.ItemType.SkipMission)
				{
					_cachedMissionSkipPurchaseId = e.InstanceId;
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in PurchaseHandler_OnIngameCurrencyPurchaseCompleted. {1}", Time.time, arg);
			}
		}

		private void InAppManager_OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			try
			{
				if (wasRestored)
				{
					return;
				}
				bool flag = validationResult != null && validationResult.receiptStatus == ReceiptStatus.Valid;
				if (_settings.OnlyValidatedPurchases && InAppPurchaseProvider.IsValidationSupportedOnCurrentPlatform && !flag)
				{
					return;
				}
				InAppItemModel value;
				if (!InAppData.inAppData.TryGetValue(productId, out value))
				{
					Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Could not find item info for productId '{0}'. Cancel logging.", productId);
					return;
				}
				string itemTitleId;
				if (!ItemInfo.TryGetItemTitleId(value, out itemTitleId))
				{
					Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Undefined Item Title ID for productId '{0}'. Cancel logging.", productId);
				}
				long num = SubwayRandom.DateTimeTicksRandomOffset();
				if (!IsPartOfSample)
				{
					_sampleState = SampleState.ExplicitIncluded;
					SyboAnalyticsSave.syboAnalyticsSampleState = _sampleState;
					if (_manager == null)
					{
						Initialize();
					}
					else
					{
						PostEvent(new SubwayAppEntersForegroundEvent());
					}
					AddUserToSample(JoinedSampleSource.RealCurrencyPurchase, num);
					foreach (ABTestData.ABTestDataItem testDatum in ABTestData.testData)
					{
						if (ABTestManager.IsActive(testDatum.identifier))
						{
							string text = ABTestManager.GetTag(testDatum.identifier);
							int payloadInt = ABTestManager.GetPayloadInt(testDatum.identifier);
							PostEvent(new JoinedABTestEvent(text, payloadInt));
						}
					}
				}
				float localCurrencyPrice = 0f;
				ProductRequestResultInfo productInfo;
				if (InAppPurchaseProvider.instance.TryGetLatestRequestedProductInfoForProductId(productId, out productInfo))
				{
					string localizedPrice = productInfo.localizedPrice;
					localCurrencyPrice = AnalyticsHelper.GetLocalCurrencyPriceAsFloat(localizedPrice, value.itemType, itemTitleId);
				}
				else
				{
					Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Could not fetch product info for item type {0} with item title id '{1}'.", Time.time, value.itemType, itemTitleId);
				}
				string menusBeforeString = GetMenusBeforeString(true);
				int? gameNumberOrNull = null;
				if (Game.IsInstantiated && Game.Instance.IsInGame.Value)
				{
					gameNumberOrNull = _lastGameIdInSession;
				}
				SubwayRealCurrencyPurchaseEvent eventData = new SubwayRealCurrencyPurchaseEvent(itemTitleId, value.itemType, num, menusBeforeString, gameNumberOrNull, GetPlatformVendor(), localCurrencyPrice);
				PostEvent(eventData);
				if (value.itemType != InAppItemModel.ItemType.Bundle)
				{
					int itemPayoutAmount = AnalyticsHelper.GetItemPayoutAmount(value);
					int itemBalance = AnalyticsHelper.GetItemBalance(value.itemType);
					SubwayItemAddedEvent eventData2 = new SubwayItemAddedEvent(ObtainableItemSource.RealCurrencyPurchase, num, null, itemTitleId, value.itemType, itemBalance, itemPayoutAmount);
					PostEvent(eventData2);
					return;
				}
				foreach (InAppItemModel.BundleItem bundleContent in value.bundleContents)
				{
					string itemTitleId2 = "bundle_" + ItemInfo.GetItemTitleId(bundleContent);
					int itemBalance2 = AnalyticsHelper.GetItemBalance(bundleContent.type);
					int amount = bundleContent.amount;
					SubwayItemAddedEvent eventData3 = new SubwayItemAddedEvent(ObtainableItemSource.RealCurrencyPurchase, num, null, itemTitleId2, bundleContent.type, itemBalance2, amount);
					PostEvent(eventData3);
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in InAppManager_OnPurchaseSuccess: {1}", Time.time, arg);
			}
		}

		private void OnRewardConsumablesWithVideo(InAppItemModel.ItemType type, int amount)
		{
			UnlockItemSource unlockItemSource = UnlockItemSource.NotSet;
			switch (type)
			{
			case InAppItemModel.ItemType.Hoverboards:
				unlockItemSource = UnlockItemSource.FreeHoverboards;
				break;
			case InAppItemModel.ItemType.Headstarts:
				unlockItemSource = UnlockItemSource.FreeHeadstart;
				break;
			case InAppItemModel.ItemType.Scoreboosters:
				unlockItemSource = UnlockItemSource.FreeScoreBooster;
				break;
			}
			SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.RewardedVideo, -1L, unlockItemSource.ToString(), ItemInfo.GetItemTitleId(type), type, InventorySave.GetAmount(UpgradeType.hoverboard), amount);
			PostEvent(eventData);
		}

		private string GetMenusBeforeString(bool excludeLastMenuIfNotPurchaseOrigin)
		{
			string text = string.Empty;
			List<string> list = new List<string>(_menuScreenQueue);
			if (excludeLastMenuIfNotPurchaseOrigin && 1 < list.Count)
			{
				int index = list.Count - 2;
				int index2 = list.Count - 1;
				if (list[index] == "NotEnoughCurrencyPopup" || list[index2] == "CelebrationPopup")
				{
					list.RemoveAt(index2);
				}
			}
			int num = Math.Max(0, list.Count - 3);
			for (int i = num; i < list.Count; i++)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += ",";
				}
				text += list[i];
			}
			return text;
		}

		private bool IsRewardMysteryBox(CelebrationReward reward)
		{
			return reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBoxMini || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TokenMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox;
		}

		private void RewardManager_OnMysteryBoxAddedForUnlock(CelebrationReward reward)
		{
			try
			{
				if (IsRewardMysteryBox(reward) && reward.syboAnalyticsSource != ObtainableItemSource.IngameCurrencyPurchase)
				{
					MysteryBox.Type boxType = RewardManager.ConvertRewardOriginToBoxType(reward.CelebrationRewardOrigin);
					SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(reward.syboAnalyticsSource, reward.syboAnalyticsSourceId, reward.syboAnalyticsSourceMetaOrNull, ItemInfo.GetItemTitleId(boxType), InAppItemModel.ItemType.Gachabox, 1L, 1L, reward.Uid);
					PostEvent(eventData);
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in RewardManager_OnMysteryBoxAddedForUnlock: {1}", Time.time, arg);
			}
		}

		private void RewardManager_OnRewardPaidOut(CelebrationReward reward)
		{
			try
			{
				if (AnalyticsHelper.ShouldTrackRewardPayout(reward))
				{
					ObtainableItemSource itemSource;
					long sourceId;
					string sourceMeta;
					string itemTitleId;
					InAppItemModel.ItemType itemType;
					long itemBalanceAfter;
					long itemDelta;
					AnalyticsHelper.ConvertRewardToItemAddedParameters(reward, out itemSource, out sourceId, out sourceMeta, out itemTitleId, out itemType, out itemBalanceAfter, out itemDelta);
					SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(itemSource, sourceId, sourceMeta, itemTitleId, itemType, itemBalanceAfter, itemDelta);
					PostEvent(eventData);
				}
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in RewardManager_OnRewardPaidOut. {1}", Time.time, arg);
			}
		}

		private void OnDoubleCoinsReceived(CelebrationReward reward)
		{
			try
			{
				ObtainableItemSource itemSource;
				long sourceId;
				string sourceMeta;
				string itemTitleId;
				InAppItemModel.ItemType itemType;
				long itemBalanceAfter;
				long itemDelta;
				AnalyticsHelper.ConvertRewardToItemAddedParameters(reward, out itemSource, out sourceId, out sourceMeta, out itemTitleId, out itemType, out itemBalanceAfter, out itemDelta);
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(itemSource, sourceId, sourceMeta, itemTitleId, itemType, itemBalanceAfter, itemDelta);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in RewardManager_OnRewardPaidOut: {1}", Time.time, arg);
			}
		}

		private void CelebrationPopup_OnRewardMultiplied(CelebrationReward reward)
		{
			try
			{
				long sourceId;
				string sourceMeta;
				string itemTitleId;
				InAppItemModel.ItemType itemType;
				long itemBalanceAfter;
				long itemDelta;
				ObtainableItemSource itemSource;
				AnalyticsHelper.ConvertRewardToItemAddedParameters(reward, out itemSource, out sourceId, out sourceMeta, out itemTitleId, out itemType, out itemBalanceAfter, out itemDelta);
				itemSource = ObtainableItemSource.GachaboxDuplication;
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(itemSource, sourceId, sourceMeta, itemTitleId, itemType, itemBalanceAfter, itemDelta);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in RewardManager_OnRewardPaidOut: {1}", Time.time, arg);
			}
		}

		private void PlayerInfo_OnDailyLetterCoinsRewarded(int amount)
		{
			try
			{
				InAppItemModel.ItemType itemType = InAppItemModel.ItemType.Coins;
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.DailyLetters, -1L, null, ItemInfo.GetItemTitleId(itemType), itemType, AnalyticsHelper.GetItemBalance(itemType), amount);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in PlayerInfo_OnDailyLetterCoinsRewarded: {1}", Time.time, arg);
			}
		}

		private void OfferWallManager_OnPendingRewardsReceived(int rewardAmount)
		{
			try
			{
				if (GetPendingRewardAmount() == rewardAmount)
				{
					Dictionary<string, string> dictionary = SyboAnalyticsSave.SyboAnalyticsGetPendingOfferRewards();
					{
						foreach (string key in dictionary.Keys)
						{
							int result;
							if (!int.TryParse(key, out result))
							{
								result = -1;
							}
							long result2;
							if (!long.TryParse(dictionary[key], out result2))
							{
								result2 = -1L;
							}
							SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.OfferWall, result2, null, ItemInfo.GetItemTitleId(InAppItemModel.ItemType.Coins), InAppItemModel.ItemType.Coins, InventorySave.coins, result);
							PostEvent(eventData);
							SyboAnalyticsSave.SyboAnalyticsRemovePendingOffer(key);
						}
						return;
					}
				}
				SubwayItemAddedEvent eventData2 = new SubwayItemAddedEvent(ObtainableItemSource.OfferWall, -1L, null, ItemInfo.GetItemTitleId(InAppItemModel.ItemType.Coins), InAppItemModel.ItemType.Coins, InventorySave.coins, rewardAmount);
				PostEvent(eventData2);
				SyboAnalyticsSave.SyboAnalyticsClearPendingOffers();
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in OfferWallManager_OnPendingRewardsReceived: {1}", Time.time, arg);
			}
		}

		private int GetPendingRewardAmount()
		{
			int num = 0;
			foreach (string key in SyboAnalyticsSave.SyboAnalyticsGetPendingOfferRewards().Keys)
			{
				int result = 0;
				int.TryParse(key, out result);
				num += result;
			}
			return num;
		}

		private void EarnCoinsData_OnRewardURLOpened(string id, int coinAmount)
		{
			try
			{
				InAppItemModel.ItemType itemType = InAppItemModel.ItemType.Coins;
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.SocialMedia, -1L, id.ToLowerInvariant(), ItemInfo.GetItemTitleId(itemType), itemType, AnalyticsHelper.GetItemBalance(itemType), coinAmount);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in EarnCoinsData_OnRewardURLOpened: {1}", Time.time, arg);
			}
		}

		private void FacebookRewardsPopup_OnFacebookRewardPaidOut(FacebookRewardsInfo.FacebookRewardType rewardType, int amount)
		{
			try
			{
				InAppItemModel.ItemType itemType = AnalyticsHelper.ConvertFacebookRewardToItemType(rewardType);
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.SocialMedia, -1L, EarnCoinsData.Id.FacebookLogin.ToString().ToLowerInvariant(), ItemInfo.GetItemTitleId(itemType), itemType, AnalyticsHelper.GetItemBalance(itemType), amount);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in FacebookRewardsPopup_OnFacebookRewardPaidOut: {1}", Time.time, arg);
			}
		}

		private void FacebookRewardsPopup_OnFacebookCharacterReceived(Characters.CharacterId characterId, int outfitIndex)
		{
			try
			{
				InAppItemModel.ItemType itemType = InAppItemModel.ItemType.Character;
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.SocialMedia, -1L, EarnCoinsData.Id.FacebookLogin.ToString().ToLowerInvariant(), ItemInfo.GetItemTitleId(characterId, Outfits.GetOutfitForCharacter(characterId, outfitIndex)), itemType, AnalyticsHelper.GetItemBalance(itemType, characterId), 1L);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in FacebookRewardsPopup_OnFacebookCharacterReceived: {1}", Time.time, arg);
			}
		}

		private void FriendHelperCrew_OnFriendCoinsReceived(int coinAmount)
		{
			try
			{
				InAppItemModel.ItemType itemType = InAppItemModel.ItemType.Coins;
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.FacebookFriends, -1L, null, ItemInfo.GetItemTitleId(itemType), itemType, AnalyticsHelper.GetItemBalance(itemType), coinAmount);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in FriendHelperCrew_OnFriendCoinsReceived: {1}", Time.time, arg);
			}
		}

		private void CharacterNameEventManager_OnCharacterSkippedWithKeys(Characters.CharacterId characterId, int amount)
		{
			SubwayIngameCurrencyPurchaseEvent subwayIngameCurrencyPurchaseEvent = new SubwayIngameCurrencyPurchaseEvent(ItemInfo.GetItemTitleId(InAppItemModel.ItemType.CharacterSkip), InAppItemModel.ItemType.CharacterSkip, SubwayRandom.DateTimeTicksRandomOffset(), GetMenusBeforeString(true), null, InGameCurrencyType.Keys, -amount, AnalyticsHelper.GetItemBalance(InAppItemModel.ItemType.Keys));
			PostEvent(subwayIngameCurrencyPurchaseEvent);
			PostCharacterSkippedEvent(characterId, ObtainableItemSource.IngameCurrencyPurchase, subwayIngameCurrencyPurchaseEvent.PurchaseId);
		}

		private void CharacterNameEventManager_OnCharacterSkippedWithVideo(Characters.CharacterId characterId)
		{
			PostCharacterSkippedEvent(characterId, ObtainableItemSource.RewardedVideo, -1L);
		}

		private void PostCharacterSkippedEvent(Characters.CharacterId characterId, ObtainableItemSource sourceType, long sourceId = -1L)
		{
			SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(sourceType, sourceId, null, "skipcharacter_" + ItemInfo.GetItemTitleId(characterId), InAppItemModel.ItemType.CharacterSkip, 1L, 1L);
			PostEvent(eventData);
		}

		private void MissionManager_OnMissionComplete(string message, int missionIndex)
		{
			try
			{
				short missionIndex2 = Convert.ToInt16(missionIndex + 1);
				MissionCompletedEvent eventData = new MissionCompletedEvent(AnalyticsHelper.GetSyboAnalyticsMissionSetString(), missionIndex2, _cachedMissionSkipPurchaseId);
				PostEvent(eventData);
			}
			catch (Exception arg)
			{
				Log.Analytics.LogErrorFormat("[SYBO Analytics] - {0:0.00}s : Exception in MissionManager_OnMissionComplete: {1}", Time.time, arg);
			}
			_cachedMissionSkipPurchaseId = -1L;
		}

		private void MissionManager_OnMissionSkipped(bool skippedWithVideo, int missionIndex)
		{
			if (skippedWithVideo)
			{
				string sourceMetaOrNull = null;
				string topScreenName = UIScreenController.Instance.GetTopScreenName();
				if (topScreenName == "ChallengesScreen")
				{
					sourceMetaOrNull = UnlockItemSource.VideoAdPrizeChallenges.ToString();
				}
				else if (topScreenName == "PauseUI")
				{
					sourceMetaOrNull = UnlockItemSource.VideoAdPrizePause.ToString();
				}
				long num = SubwayRandom.DateTimeTicksRandomOffset();
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.RewardedVideo, -1L, sourceMetaOrNull, ItemInfo.GetSkipMissionItemTitleId(missionIndex), InAppItemModel.ItemType.SkipMission, -1L, 1L, num);
				_cachedMissionSkipPurchaseId = num;
				PostEvent(eventData);
			}
		}

		private void HandleNewAbTestTags()
		{
			if (!IsActive)
			{
				return;
			}
			foreach (KeyValuePair<string, string> syboAnalyticsUnhandledAbTestDatum in SyboAnalyticsSave.syboAnalyticsUnhandledAbTestData)
			{
				string key = syboAnalyticsUnhandledAbTestDatum.Key;
				string value = syboAnalyticsUnhandledAbTestDatum.Value;
				int result;
				if (int.TryParse(value, out result) && result != -1)
				{
					PostEvent(new JoinedABTestEvent(key, result));
				}
			}
			SyboAnalyticsSave.SyboAnalyticsClearUnhandledAbTestData();
		}

		private void HandlePromotionCodeConfirmed(object sender, PromoCodeEventArgs e)
		{
			if (e.numCharacters > 0)
			{
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.PromotionCode, -1L, e.code, ItemInfo.GetItemTitleId(InAppItemModel.ItemType.Character), InAppItemModel.ItemType.Character, AnalyticsHelper.GetItemBalance(InAppItemModel.ItemType.Character), e.numCharacters);
				PostEvent(eventData);
			}
			if (e.numBoards > 0)
			{
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.PromotionCode, -1L, e.code, ItemInfo.GetItemTitleId(InAppItemModel.ItemType.Board), InAppItemModel.ItemType.Board, AnalyticsHelper.GetItemBalance(InAppItemModel.ItemType.Board), e.numBoards);
				PostEvent(eventData);
			}
			if (e.numCoins > 0)
			{
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.PromotionCode, -1L, e.code, ItemInfo.GetItemTitleId(InAppItemModel.ItemType.Coins), InAppItemModel.ItemType.Coins, AnalyticsHelper.GetItemBalance(InAppItemModel.ItemType.Coins), e.numCoins);
				PostEvent(eventData);
			}
			if (e.numKeys > 0)
			{
				SubwayItemAddedEvent eventData = new SubwayItemAddedEvent(ObtainableItemSource.PromotionCode, -1L, e.code, ItemInfo.GetItemTitleId(InAppItemModel.ItemType.Keys), InAppItemModel.ItemType.Keys, AnalyticsHelper.GetItemBalance(InAppItemModel.ItemType.Keys), e.numCoins);
				PostEvent(eventData);
			}
		}

		private void CloudSave_OnDisableCloudSave()
		{
		}

		private void CloudSave_OnRestore()
		{
		}

		private void LogWarningIfWindowsPhone()
		{
		}

		void IAnalyticsIntegrationTest.SimulateOnlineSettingsDownload()
		{
			OnlineSettings_OnDownloaded(true);
		}

		void IAnalyticsIntegrationTest.GenerateFakeSessionEvents(int count)
		{
			int highScore = PlayerSave.highScore;
			int coins = InventorySave.coins;
			int keys = InventorySave.keys;
			for (int i = 0; i < count; i++)
			{
				SubwaySessionStartedEvent subwaySessionStartedEvent = new SubwaySessionStartedEvent();
				subwaySessionStartedEvent.HighScore = highScore;
				subwaySessionStartedEvent.CoinBalance = coins;
				subwaySessionStartedEvent.KeyBalance = keys;
				PostEvent(subwaySessionStartedEvent);
			}
		}

		void ICoroutineHandler.StartCoroutine(IEnumerator coroutine)
		{
			StartCoroutine(coroutine);
		}
	}
}
