using System;
using System.Collections.Generic;
using Kiloo;
using Kiloo.Common.Ads;
using SYBO.Components.Analytics.Integration;
using TopRun.Internal;
using UI.Screens.StoreScreenInternal;

namespace Data.Save.Internal
{
	public class LocalData : DiskData
	{
		public bool hasMigratedPlayerInfo;

		public int migratedPlayerPrefsVersion;

		public DateTime scheduledFirstInterstitialShowTime = DateTime.MinValue;

		public Dictionary<string, string> lastShowedProviders = new Dictionary<string, string>();

		public Dictionary<AdType, string> lastShowedCacheSlotIDs = new Dictionary<AdType, string>();

		public int missionsVersion;

		public int syboAnalyticsGamesSinceFlush = AnalyticsIntegration.GamesSinceFlushPersistedDefault;

		public string syboAnalyticsPreviousClientVersion = AnalyticsIntegration.PreviousClientVersionPersistedDefault;

		public string syboAnalyticsOperatingSystem = AnalyticsIntegration.OperatingSystemPersistedDefault;

		public string syboAnalyticsDeviceID = AnalyticsIntegration.DeviceIDPersistedDefault;

		public string syboAnalyticsOSLanguage = AnalyticsIntegration.OSLanguagePersistedDefault;

		public DeviceInfo.PerformanceLevel syboAnalyticsPerformanceLevel = AnalyticsIntegration.PerformanceLevelPersistedDefault;

		public int syboAnalyticsGpuScore = AnalyticsIntegration.GpuScorePersistedDefault;

		public string syboAnalyticsOnlineSettingsLocale = AnalyticsIntegration.OnlineSettingsLocalePersistedDefault;

		public Dictionary<string, string> syboAnalyticsPendingOfferRewardToOfferIdMap = new Dictionary<string, string>();

		public string syboAnalyticsUserId = AnalyticsIntegration.AnalyticsUserIdPersistedDefault;

		public DateTime lastKnownServerTime = DateTime.MinValue;

		public DateTime serverTimeWhenWentOffline = DateTime.MaxValue;

		public bool isOffline;

		public string lastVersionRated = string.Empty;

		public DateTime lastTimeAppWasUpdatedOrInstalled = CDateTime.UtcNow;

		public bool cloudSaveEnabled;

		public DateTime latestSuccessfulCloudSaveSyncTime = DateTime.MinValue;

		public DateTime lastCloudSaveReminderShownTime = DateTime.MinValue;

		public bool cloudSaveEverEnabled;

		public bool wereNotificationsEnabled;

		public bool hasCloudSaveCelebrationBeenShown;

		public bool hasCloudSaveSuccessfullyPeeked;

		public bool cloudSaveEnabledByPeekRoutine;

		public int introductionPopupShownCount;

		public int nativeLoginCanceledVersion;

		public StoreEventFillerDealSaveData currentStoreFillerDeal = new StoreEventFillerDealSaveData();

		public bool allAssetBundlesDownloaded;

		public int latestAvailableAssetBundlesVersion;

		public int lastWeekMedalWasShown = -1;

		public PlayerIdsData playerIdsData = new PlayerIdsData();

		public RanksData ranksData = new RanksData();

		public int numberOfRunsSinceLastGuideline;

		public DateTime lastShownCloudSaveIntroduction = DateTime.MinValue;

		public bool cloudSaveOptedOut;

		public override void Obfuscate()
		{
		}

		public override void Deobfuscate()
		{
		}
	}
}
