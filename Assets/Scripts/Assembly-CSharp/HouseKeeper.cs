#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ABTest;
using AdHandling.AdConstants;
using AppRefresh;
using Cloud;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Analytics;
using Kiloo.Common.Backend;
using Kiloo.Common.HTTP;
using Kiloo.Common.LocalizationManager;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Plugins.Notifications;
using Kiloo.Common.Utility;
using Missions;
using Onboarding;
using SYBO.Subway;
using SYBO.Subway.Info;
using TopRun;
using UI.Screens;
using UI.VideoAdPrizes;
using UnityEngine;
using WordHunt;

public class HouseKeeper : MonoBehaviour
{
	public delegate void OnLateUpdate();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static OnLateUpdate onLateUpdate__BackingField;

	private const string APPLICATION_AUTHENTICITY_KEY = "applicationAuthenticity";

	private const string SFX_BEFORE_APPLICATION_LOSTFOCUS = "sfxBeforeLostFocus";

	private const string MUSIC_BEFORE_APPLICATION_LOSTFOCUS = "musicBeforeLostFocus";

	private const string RTL_ALIGNED_LANGUAGES_REGEX = "\\p{IsArabic}|\\p{IsHebrew}|\\p{IsSyriac}|\\p{IsThaana}";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action secondFrameEnded__BackingField;

	private bool _hasAddedAgeRestrictionEventListener;

	private static bool _onlineServicesInitialized = false;

	private int[] _dailyKeysBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private int[] _dailyHighscoreBrackets = new int[8] { 50000, 100000, 200000, 400000, 1000000, 5000000, 10000000, 50000000 };

	private int[] _consumableHoverboardBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private int[] _headstartBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private int[] _scoreBoosterBrackets = new int[15]
	{
		100, 120, 140, 160, 180, 200, 220, 240, 260, 280,
		300, 400, 1000, 1500, 2000
	};

	private const string ANDROID_IN_APP_BILLING_GAMEOBJECT_NAME = "InAppBillingAndroidGameObject";

	private static Action<bool> delegateFacebookPayoutPopupAction = QueueFacebookPayoutPopupIfNeeded;

	private const float MIN_REFRESH_INTERVAL = 3600f;

	private const string REFRESH_INTERVAL_KEY = "refreshinterval";

	private static float _onlineSettingsLastDownloadTime = float.NegativeInfinity;

	public static event OnLateUpdate onLateUpdate
	{
		add
		{
			OnLateUpdate onLateUpdate = onLateUpdate__BackingField;
			OnLateUpdate onLateUpdate2;
			do
			{
				onLateUpdate2 = onLateUpdate;
				onLateUpdate = Interlocked.CompareExchange(ref onLateUpdate__BackingField, (OnLateUpdate)Delegate.Combine(onLateUpdate2, value), onLateUpdate);
			}
			while ((object)onLateUpdate != onLateUpdate2);
		}
		remove
		{
			OnLateUpdate onLateUpdate = onLateUpdate__BackingField;
			OnLateUpdate onLateUpdate2;
			do
			{
				onLateUpdate2 = onLateUpdate;
				onLateUpdate = Interlocked.CompareExchange(ref onLateUpdate__BackingField, (OnLateUpdate)Delegate.Remove(onLateUpdate2, value), onLateUpdate);
			}
			while ((object)onLateUpdate != onLateUpdate2);
		}
	}

	public static event Action secondFrameEnded
	{
		add
		{
			Action action = secondFrameEnded__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref secondFrameEnded__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = secondFrameEnded__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref secondFrameEnded__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	[UsedImplicitly]
	private void Awake()
	{
		DeviceUtility.Init();
		IOSViewController.ShowStatusBar(false);
		IOSViewController.SetStatusBarStyle(IOSStatusBarStyle.White);
		IOSViewController.SetStatusBarAnimation(IOSStatusBarAnimation.None);
		IOSViewController.SetProtectedEdges(IOSUIRectEdge.None);
		IOSViewController.ShowHomeIndicator(true);
		// No navegador, 60 fixo faz a Unity desenhar por setTimeout (trava e perde quadros).
		// -1 = desenha a cada quadro da tela (requestAnimationFrame): mais FPS e mais liso, até 120/144 Hz.
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 0;
			QualitySettings.antiAliasing = 0;
		}
		else
		{
			Application.targetFrameRate = 60;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Flurry.StartSession(AdConstants.FlurryApiKey);
		SaveManager.Init();
		Flurry.HandleFlurryUserIDBackup();
		AppRefreshHandler.hasRefreshed += OnAppHasRefreshed;
		InitLocale();
		Notifications.Init(false);
		InitOnlineServicesIfNeeded();
		WelcomeBackRewardManager.Init();
		RefreshOnlineSettingsAndInappsIfNeeded();
		SSEventsManagerController.instance.Initialize();
		HookUpListenersToOnlineSettings();
		TryRestoreSoundOptionAfterLostFocusIfNeeded();
		SocialManager.Init();
		SocialManager.instance.friendsConsolidatedHandler += OnFriendsConsolidated;
		if (SocialManager.instance.consolidatedFriendsCompleted)
		{
			OnFriendsConsolidated(true);
		}
		TopRunNotificationSlideInManager.Init();
		InAppManager.Init();
		Layers.Init();
		FirstPurchaseBonusManager.StartStateCheckCoroutine();
		CancellAllLocalNotifications();
		if (!UserSave.hasUserRunAppBefore)
		{
			UserSave.hasUserRunAppBefore = true;
			OnboardingSave.shouldShowCollectPopup = false;
			UserSave.lastTimeAppWasUpdatedOrInstalled = CDateTime.UtcNow;
			UserSave.firstInstallDate = CDateTime.UtcNow;
			UserSave.firstInstalledVersion = DeviceUtility.GetBundleVersion();
		}
		PostFlurryEventsOnceADay();
		LogApplicationAuthenticity();
		AwardsManager.instance.InitAwards();
		WordHuntManager.instance.DownloadDaily();
		OfferWallManager.instance.Initialize();
		Manager.Initialize();
		RunTracker.Initialize();
		ABTestManager.Initialize();
		AddFpsCounter();
		InRunLogging.Init();
		NotificationsObserver.instance.onNotificationPressed += OnOpenedThroughNotification;
		InventorySave.RewardUnrewardedCoins();
		VerifyCharactersAndBoardsAmount();
		CharacterManager.Init();
		BillboardAdsManager.Initialize();
		MemoryWarningHandler.StartTracking();
		OnboardingSave.currentSessionCount++;
	}

	private void OnAppHasRefreshed(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.Data))
		{
			Flurry.HandleFlurryUserIDBackup();
			Flurry.SetUserId();
			CancellAllLocalNotifications();
		}
	}

	private void InitLocale()
	{
		Strings.Init(LocaleData.instance);
		Strings.language = Strings.DetermineBestLanguage();
	}

	[UsedImplicitly]
	private void LateUpdate()
	{
		if (onLateUpdate__BackingField != null)
		{
			onLateUpdate__BackingField();
		}
		if (Globals.DEBUG_TEST_FPS_SPIKES)
		{
			DebugSimulateFpsSpikes();
		}
	}

	private IEnumerator Start()
	{
		StartCoroutine(OnSecondFrameEnded());
		yield return StartCoroutine(CloudSave.StartUpRoutine());
	}

	private IEnumerator OnSecondFrameEnded()
	{
		while (Time.renderedFrameCount < 2)
		{
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		Action handler = secondFrameEnded__BackingField;
		if (handler != null)
		{
			handler();
		}
	}

	private void DebugSimulateFpsSpikes()
	{
		if (!Globals.DEBUG_TEST_FPS_SPIKES)
		{
			return;
		}
		int num = 5;
		if ((int)Time.realtimeSinceStartup % num == 0)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			while ((double)realtimeSinceStartup + 0.5 > (double)Time.realtimeSinceStartup)
			{
			}
		}
	}

	private void AddFpsCounter()
	{
		GameObject gameObject = new GameObject("FpsCounter");
		gameObject.name = "FpsCounter";
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		InRunAverageFPSCounter inRunAverageFPSCounter = gameObject.AddComponent<InRunAverageFPSCounter>();
		inRunAverageFPSCounter.Initialize();
	}

	private void LogApplicationAuthenticity()
	{
		if (!PlayerPrefs.HasKey("applicationAuthenticity"))
		{
			string argValue = "N/A";
			if (Application.genuineCheckAvailable)
			{
				argValue = ((!Application.genuine) ? "App is not authentic" : "App is authentic");
			}
			Flurry.LogEventWithAParameter("Application Authenticity", "Type", argValue);
			PlayerPrefs.SetString("applicationAuthenticity", "set");
		}
	}

	private void HookUpListenersToOnlineSettings()
	{
		OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsSetupFinished);
		OnlineSettings instance = OnlineSettings.instance;
		instance.onOnlineSettingsDownloadResult = (Action<bool>)Delegate.Combine(instance.onOnlineSettingsDownloadResult, new Action<bool>(OnOnlineSettingsDownloadedWithResult));
	}

	private void OnOnlineSettingsSetupFinished()
	{
		if (AgeRestriction.waitForAgeRestriction)
		{
			if (!_hasAddedAgeRestrictionEventListener)
			{
				AgeRestriction.restrictionChanged += InitializeAdsSystem;
				_hasAddedAgeRestrictionEventListener = true;
			}
		}
		else
		{
			InitializeAdsSystem();
			BillboardAdsManager.InitializeAdvertySdk();
		}
	}

	private void OnOnlineSettingsDownloadedWithResult(bool result)
	{
		if (!IsDateToday(UserSave.lastDailyOnlineLog))
		{
			Flurry.LogEventWithAParameter("Daily Check of Online Status", (!result) ? "Offline Country" : "Online Country", Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale()));
			UserSave.lastDailyOnlineLog = CDateTime.UtcNow;
		}
	}

	private void InitializeAdsSystem()
	{
		if (!Globals.DEBUG || !DebugSave.disableAdSystem)
		{
			if (_hasAddedAgeRestrictionEventListener)
			{
				AgeRestriction.restrictionChanged -= InitializeAdsSystem;
				_hasAddedAgeRestrictionEventListener = false;
			}
			AdSystemManager.CreateAdSystemManager();
		}
	}

	public static void InitOnlineServicesIfNeeded()
	{
		if (!_onlineServicesInitialized)
		{
			WebClient.timeout = 10f;
			WebClient.ClearCache(WebClient.ClearCacheFlags.Expired | WebClient.ClearCacheFlags.Old, 2419200f);
			KilooWebClient.InitAutomatically("subway_surfers");
			TopRunRoot.Init();
			OnlineSettingsManager.Initialize();
			_onlineServicesInitialized = true;
		}
	}

	private void RefreshTopRun()
	{
		IManager manager = TopRunRoot.manager;
		if (manager == null)
		{
			LogError("TopRun Manager instance is null!");
		}
		else
		{
			manager.RefreshTournament();
		}
	}

	private void RegisterAllBundlesDownloaded()
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			if (!IsDateToday(UserSave.lastQuitDate))
			{
				string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
				bool allAssetBundlesDownloaded = UserSave.allAssetBundlesDownloaded;
				Flurry.LogEventWithAParameter("All Asset Bundle Downloads completed", "All Bundles Downloaded?;" + string.Format("All Bundles Downloaded {0} in Country", allAssetBundlesDownloaded), allAssetBundlesDownloaded + ";" + contryCodeFromLocale);
			}
			UserSave.lastQuitDate = CDateTime.UtcNow;
		}
	}

	private void RegisterLocalNotificationStatus()
	{
		if (!Application.isEditor && !DeviceUtility.isKindleDevice && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
		{
			Flurry.LogEventWithAParameter("State of notifications", "Enabled", Notifications.instance.areNotificationsEnabled.ToString());
		}
	}

	private void RegisterReminderStatus()
	{
		Flurry.LogEventWithAParameter("State of reminders", "Enabled", Settings.optionReminder.ToString());
	}

	private void RegisterDailyActiveCharacter()
	{
		string argValue = string.Concat(PlayerSave.currentCharacterId, "_Theme", PlayerSave.currentOutfitIndex);
		Flurry.LogEventWithAParameter("Currently active character", "Id", argValue);
	}

	private void RegisterDailyActiveBoard()
	{
		Boards.BoardId currentBoardId = PlayerSave.currentBoardId;
		string text = currentBoardId.ToString();
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(currentBoardId))
		{
			foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[currentBoardId])
			{
				if (BoardManager.IsThemeActive(currentBoardId, item.identifier))
				{
					text = text + "_" + item.identifier;
				}
			}
		}
		Flurry.LogEventWithAParameter("Currently active board", "Id", text);
	}

	private void RegisterDailyCoinsAmount()
	{
		int coins = InventorySave.coins;
		string argValue = ((coins <= 10000) ? RoundUpToTargetValue(coins, 1000) : ((coins <= 100000) ? RoundUpToTargetValue(coins, 10000) : ((coins <= 1000000) ? RoundUpToTargetValue(coins, 100000) : ((coins > 3000000) ? (3000000 + "+") : RoundUpToTargetValue(coins, 500000)))));
		Flurry.LogEventWithAParameter("Daily report of coins", "Amount", argValue);
	}

	private void RegisterDailyKeysAmount()
	{
		int keys = InventorySave.keys;
		string bracketForValue = GetBracketForValue(keys, _dailyKeysBrackets, true);
		Flurry.LogEventWithAParameter("Daily keys report", "Amount", bracketForValue);
	}

	private void RegisterDailyHighscoreReport()
	{
		string bracketForValue = GetBracketForValue(PlayerSave.highScore, _dailyHighscoreBrackets, false);
		Flurry.LogEventWithAParameter("Daily highscore report", "Value", bracketForValue);
	}

	private void RegisterFacebookStatus()
	{
		string argValue = ((!SocialManager.instance.facebookIsLoggedIn) ? "offline" : "online");
		Flurry.LogEventWithAParameter("Facebook status", "Status", argValue);
	}

	private string RoundUpToTargetValue(int total, int factor)
	{
		string result = string.Empty;
		for (int i = 1; i <= 10; i++)
		{
			if (total <= i * factor)
			{
				int num = i * factor;
				int num2 = num - factor;
				result = num2 + " - " + num;
				break;
			}
		}
		return result;
	}

	private void RegisterDevicePerformanceLevel()
	{
		string argValue = ((DeviceInfo.Instance.performanceLevel != DeviceInfo.PerformanceLevel.High) ? "Low" : "High");
		Flurry.LogEventWithAParameter("Device performance", "Level", argValue);
	}

	private void RegisterOwnedCharactersAndOutfits()
	{
		int num = 0;
		int regularCharactersOwned = 0;
		int limitedCharactersOwned = 0;
		int tokenCharactersOwned = 0;
		int facebookCharactersOwned = 0;
		int outfitsOwned = 0;
		foreach (Characters.CharacterId item in Characters.characterOrder)
		{
			IncrementTypeOfOwnedCharacter(item, ref regularCharactersOwned, ref limitedCharactersOwned, ref tokenCharactersOwned, ref facebookCharactersOwned);
			IncrementOwnedOutfits(item, ref outfitsOwned);
		}
		num = regularCharactersOwned + limitedCharactersOwned + tokenCharactersOwned + facebookCharactersOwned;
		Flurry.LogEventWithSeveralParameters("Characters owned", "total;regular;limited;token;facebook", num + ";" + regularCharactersOwned + ";" + limitedCharactersOwned + ";" + tokenCharactersOwned + ";" + facebookCharactersOwned);
		Flurry.LogEventWithAParameter("Character outfits owned", "Amount", outfitsOwned.ToString());
	}

	private void IncrementTypeOfOwnedCharacter(Characters.CharacterId character, ref int regularCharactersOwned, ref int limitedCharactersOwned, ref int tokenCharactersOwned, ref int facebookCharactersOwned)
	{
		if (!Characters.characterData.ContainsKey(character) || !CharacterManager.IsCharacterOwned(character))
		{
			return;
		}
		Characters.Model model = Characters.characterData[character];
		if (model.hasOnlineSettings)
		{
			limitedCharactersOwned++;
			return;
		}
		switch (model.unlockType)
		{
		case PurchaseData.UnlockType.Facebook:
			if (SocialManager.instance.facebookIsLoggedIn)
			{
				facebookCharactersOwned++;
			}
			break;
		case PurchaseData.UnlockType.Tokens:
			tokenCharactersOwned++;
			break;
		default:
			regularCharactersOwned++;
			break;
		}
	}

	private void IncrementOwnedOutfits(Characters.CharacterId character, ref int outfitsOwned)
	{
		List<Outfit> list = Outfits.TryGetOutfitsForCharacter(character);
		if (list == null || list.Count <= 0)
		{
			return;
		}
		int num = 1;
		for (int i = 0; i < list.Count; i++)
		{
			if (CharacterManager.IsOutfitUnlocked(character, num))
			{
				outfitsOwned++;
			}
			num++;
		}
	}

	public void RegisterOwnedHoverboardsAndAbilities()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Boards.BoardId item in Boards.boardOrder)
		{
			if (BoardManager.IsBoardOwned(item) && Boards.boardData.ContainsKey(item))
			{
				Boards.Board board = Boards.boardData[item];
				if (board.hasOnlineTimeLimit)
				{
					num3++;
				}
				else
				{
					num2++;
				}
			}
		}
		foreach (KeyValuePair<Boards.BoardId, List<HoverboardThemes.HoverboardTheme>> hoverboardCustomTheme in HoverboardThemes.hoverboardCustomThemes)
		{
			Boards.BoardId key = hoverboardCustomTheme.Key;
			foreach (HoverboardThemes.HoverboardTheme item2 in HoverboardThemes.hoverboardCustomThemes[key])
			{
				if (BoardManager.IsThemeOwned(key, item2.identifier))
				{
					num4++;
				}
			}
		}
		num = num2 + num3;
		Flurry.LogEventWithSeveralParameters("Hoverboards owned", "total;regular;limited", num + ";" + num2 + ";" + num3);
		Flurry.LogEventWithAParameter("Hoverboard abilities owned", "Amount", num4.ToString());
	}

	private void RegisterOwnedConsumableHoverboards()
	{
		int amount = InventorySave.GetAmount(UpgradeType.hoverboard);
		string bracketForValue = GetBracketForValue(amount, _consumableHoverboardBrackets, true);
		Flurry.LogEventWithAParameter("Daily report of consumable hoverboards", "Amount", bracketForValue);
	}

	private void RegisterOwnedHeadstarts()
	{
		int amount = InventorySave.GetAmount(UpgradeType.headstart2000);
		string bracketForValue = GetBracketForValue(amount, _headstartBrackets, true);
		Flurry.LogEventWithAParameter("Daily report of headstarts owned", "Amount", bracketForValue);
	}

	private void RegisterOwnedScoreBoosters()
	{
		int amount = InventorySave.GetAmount(UpgradeType.scorebooster);
		string bracketForValue = GetBracketForValue(amount, _scoreBoosterBrackets, true);
		Flurry.LogEventWithAParameter("Daily report of score boosters owned", "Amount", bracketForValue);
	}

	private void RegisterTotalAmountOfFinishedDiamondAwards()
	{
		Flurry.LogEventWithAParameter("Total amount of diamond awards finished", "Completed Diamond Awards", AwardsManager.instance.GetNumberOfFinishedAwardsExactTier(AwardTierType.Diamond).ToString());
	}

	private void RegisterTotalAmountOfFinishedGoldAwards()
	{
		Flurry.LogEventWithAParameter("Total amount of gold awards finished", "Completed Gold Awards", AwardsManager.instance.GetNumberOfFinishedAwardsExactTier(AwardTierType.Gold).ToString());
	}

	private void RegisterTotalAmountOfFinishedSilverAwards()
	{
		Flurry.LogEventWithAParameter("Total amount of silver awards finished", "Completed Silver Awards", AwardsManager.instance.GetNumberOfFinishedAwardsExactTier(AwardTierType.Silver).ToString());
	}

	private void RegisterBundleIdentifier()
	{
		Flurry.LogEventWithAParameter("The current bundle identifier", "Daily bundle identifier", DeviceUtility.GetBundleIdentifier());
	}

	private void RegisterLocale()
	{
		Flurry.LogEventWithAParameter("Device locale", "Locale", DeviceUtility.GetLocale());
	}

	private string GetBracketForValue(int value, int[] bracket, bool withDetailedFirstBracket)
	{
		int num = 0;
		int num2 = 0;
		if (value <= bracket[0])
		{
			if (withDetailedFirstBracket)
			{
				return value.ToString();
			}
			num = 0;
			num2 = bracket[0];
			return string.Format("{0}-{1}", num, num2);
		}
		for (int i = 0; i < bracket.Length - 1; i++)
		{
			if (value <= bracket[i + 1])
			{
				num = bracket[i] + 1;
				num2 = bracket[i + 1];
				return string.Format("{0}-{1}", num, num2);
			}
		}
		int num3 = bracket[bracket.Length - 1];
		return num3 + "+";
	}

	private void RegisterFirstInstallDate()
	{
		DateTime firstInstallDate = UserSave.firstInstallDate;
		string empty = string.Empty;
		empty = ((!(firstInstallDate != DateTime.MinValue)) ? "na" : (firstInstallDate.ToUnixTime() * 1000).ToString());
		Flurry.LogEventWithAParameter("First install date", "Unix time in millis", empty);
	}

	private void PostFlurryEventsOnceADay()
	{
		if (!IsDateToday(UserSave.lastPlayDate))
		{
			RegisterLocalNotificationStatus();
			RegisterReminderStatus();
			RegisterDailyCoinsAmount();
			RegisterDailyKeysAmount();
			RegisterDailyActiveCharacter();
			RegisterDailyActiveBoard();
			RegisterFacebookStatus();
			RegisterDevicePerformanceLevel();
			RegisterOwnedCharactersAndOutfits();
			RegisterOwnedHoverboardsAndAbilities();
			RegisterOwnedConsumableHoverboards();
			RegisterOwnedHeadstarts();
			RegisterTotalAmountOfFinishedDiamondAwards();
			RegisterTotalAmountOfFinishedGoldAwards();
			RegisterTotalAmountOfFinishedSilverAwards();
			RegisterOwnedScoreBoosters();
			RegisterFirstInstallDate();
			RegisterBundleIdentifier();
			RegisterLocale();
			RegisterDailyHighscoreReport();
			StorageSizeLogging.RegisterStorageUsage();
			int numberOfDaysSinceLastPlay = GetNumberOfDaysSinceLastPlay();
			if (numberOfDaysSinceLastPlay >= 1)
			{
				Flurry.LogEventWithAParameter("Days passed since last play", "Result", numberOfDaysSinceLastPlay.ToString(NumberFormatInfo.InvariantInfo));
			}
			UserSave.lastPlayDate = CDateTime.UtcNow;
		}
	}

	[UsedImplicitly]
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Flurry.EndSession();
			PostFlurryEventsOnceADay();
			ScheduleLocalNotificationsIfNecessary();
			RegisterAllBundlesDownloaded();
			SaveManager.Save();
			CloudSave.Commit();
		}
		else
		{
			Flurry.StartSession(AdConstants.FlurryApiKey);
			CancellAllLocalNotifications();
			WordHuntManager.instance.DownloadDaily();
			RefreshOnlineSettingsAndInappsIfNeeded();
			CloudSave.StartAppResume();
		}
	}

	[UsedImplicitly]
	private void OnApplicationQuit()
	{
		ScheduleLocalNotificationsIfNecessary();
		RegisterAllBundlesDownloaded();
		OnboardingTracking.OnApplicationQuit();
		SaveManager.Save();
		CloudSave.Commit();
	}

	private int GetNumberOfDaysSinceLastPlay()
	{
		int result = 0;
		DateTime lastPlayDate = UserSave.lastPlayDate;
		DateTime utcNow = CDateTime.UtcNow;
		if (utcNow.Date > lastPlayDate.Date)
		{
			result = (int)(utcNow - lastPlayDate).TotalDays;
		}
		return result;
	}

	private bool IsDateToday(DateTime lastDate)
	{
		return lastDate.Date == CDateTime.UtcNow.Date;
	}

	private void CancellAllLocalNotifications()
	{
		if (!DeviceUtility.isKindleDevice)
		{
			Notifications.instance.ClearAllLocalNotifications();
		}
	}

	private void ScheduleLocalNotificationsIfNecessary()
	{
		bool flag = false;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("enable_local_notifications", out valueString))
		{
			try
			{
				flag = !bool.Parse(valueString);
			}
			catch (FormatException)
			{
				LogWarning(string.Format("Unable to convert '{0}'.", valueString));
				LogWarning("Reading from online failed ");
			}
		}
		if (Settings.optionReminder && !flag)
		{
			ScheduleDailyWordNotificationIfNecessary();
			ScheduleTopRunNotificationIfNecessary();
			ScheduleDailyVideoNotificationIfNecessary();
			ScheduleDailyGiftNotificationIfNecessary();
		}
	}

	private void ScheduleDailyWordNotificationIfNecessary()
	{
		TimeSpan wordHuntTimeLeft = EventsSave.wordHuntTimeLeft;
		if (!(wordHuntTimeLeft > TimeSpan.Zero))
		{
			return;
		}
		DateTime dateTime = DateTime.Today.AddHours(17.0);
		string title = Strings.Get("LOCAL_NOTIFICATION_TITLE");
		string localNotificationMessage = GetLocalNotificationMessage();
		TimeSpan timeSpan = dateTime - CDateTime.Now;
		bool todayCompleted;
		int wordHuntDaysInARow = WordHuntManager.GetWordHuntDaysInARow(out todayCompleted);
		if (wordHuntDaysInARow <= 0)
		{
			return;
		}
		int hour = CDateTime.Now.Add(wordHuntTimeLeft).AddMinutes(30.0).Hour;
		if (hour == 17)
		{
			if (todayCompleted)
			{
				DateTime time = dateTime;
				time += TimeSpan.FromDays(1.0);
				time -= TimeSpan.FromHours(1.0);
				ScheduleNotification(time, title, localNotificationMessage);
			}
			else if (wordHuntTimeLeft > TimeSpan.FromHours(1.0))
			{
				ScheduleNotification(dateTime.Subtract(TimeSpan.FromHours(1.0)), title, localNotificationMessage);
			}
		}
		else if (todayCompleted)
		{
			if (wordHuntTimeLeft < timeSpan)
			{
				ScheduleNotification(dateTime, title, localNotificationMessage);
			}
			else if (wordHuntTimeLeft > timeSpan)
			{
				ScheduleNotification(dateTime.AddDays(1.0), title, localNotificationMessage);
			}
		}
		else if (wordHuntTimeLeft > timeSpan && timeSpan > TimeSpan.Zero)
		{
			ScheduleNotification(dateTime, title, localNotificationMessage);
		}
	}

	private void ScheduleTopRunNotificationIfNecessary()
	{
		IManager manager = TopRunRoot.manager;
		DateTime dateTime = DateTime.Today.AddHours(9.0);
		DateTime dateTime2 = DateTime.Today.AddHours(21.0);
		DateTime dateTime3 = DateTime.Today.AddDays(1.0).AddHours(9.0);
		DateTime dateTime4 = DateTime.Today.AddDays(1.0).AddHours(21.0);
		if (manager == null || manager.tournament == null)
		{
			return;
		}
		DateTime officialEndTimeUTC = manager.tournament.officialEndTimeUTC;
		DateTime dateTime5 = officialEndTimeUTC - new TimeSpan(0, 8, 0, 0);
		string message = FormatMessageWithNameIfPossible("LOCAL_NOTIFICATION_MESSAGE_TOPRUN_ENDING", "LOCAL_NOTIFICATION_MESSAGE_TOPRUN_ENDING_PERSONAL");
		if (dateTime5.Date == DateTime.Today)
		{
			if (dateTime < dateTime5 && dateTime5 < dateTime2)
			{
				ScheduleNotification(dateTime5, Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
			}
			else if (dateTime5 < dateTime && dateTime < officialEndTimeUTC)
			{
				ScheduleNotification(dateTime, Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
			}
		}
		else if (dateTime5.Date == DateTime.Today.AddDays(1.0))
		{
			if (dateTime3 < dateTime5 && dateTime5 < dateTime4)
			{
				ScheduleNotification(dateTime5, Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
			}
			else if (dateTime5 < dateTime3 && dateTime3 < officialEndTimeUTC)
			{
				ScheduleNotification(dateTime3, Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
			}
		}
	}

	private void ScheduleDailyVideoNotificationIfNecessary()
	{
		DateTime lastTimeAVideoForKeysWasSeen = VideoAdPrizeManager.instance.GetLastTimeAVideoForKeysWasSeen();
		DateTime dateTime = CDateTime.Now.AddDays(-1.0);
		DateTime now = CDateTime.Now;
		DateTime lastTimeAppWasUpdatedOrInstalled = UserSave.lastTimeAppWasUpdatedOrInstalled;
		string message = FormatMessageWithNameIfPossible("LOCAL_NOTIFICATION_MESSAGE_DAILY_VIDEO", "LOCAL_NOTIFICATION_MESSAGE_DAILY_VIDEO_PERSONAL");
		if (lastTimeAVideoForKeysWasSeen.Date == dateTime.Date || lastTimeAppWasUpdatedOrInstalled.Date == dateTime.Date)
		{
			ScheduleNotification(LocalNotificationsManager.GetNextNotificationTimeForVideos(), Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
		}
		else if (lastTimeAVideoForKeysWasSeen.Date == now.Date || lastTimeAppWasUpdatedOrInstalled.Date == now.Date)
		{
			ScheduleNotification(LocalNotificationsManager.GetNextNotificationTimeForVideos().AddDays(1.0), Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
		}
	}

	private void ScheduleDailyGiftNotificationIfNecessary()
	{
		DateTime dailyGiftLastGiven = UserSave.dailyGiftLastGiven;
		DateTime dateTime = CDateTime.Now.AddDays(-1.0);
		DateTime now = CDateTime.Now;
		DateTime lastTimeAppWasUpdatedOrInstalled = UserSave.lastTimeAppWasUpdatedOrInstalled;
		string message = FormatMessageWithNameIfPossible("LOCAL_NOTIFICATION_MESSAGE_DAILY_GIFT", "LOCAL_NOTIFICATION_MESSAGE_DAILY_GIFT_PERSONAL");
		if (dailyGiftLastGiven.Date == dateTime.Date || lastTimeAppWasUpdatedOrInstalled.Date == dateTime.Date)
		{
			ScheduleNotification(now.Date.AddHours(9.0), Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
		}
		else if (dailyGiftLastGiven.Date == now.Date || lastTimeAppWasUpdatedOrInstalled.Date == now.Date)
		{
			ScheduleNotification(now.Date.AddDays(1.0).AddHours(9.0), Strings.Get("LOCAL_NOTIFICATION_TITLE"), message);
		}
	}

	private void ScheduleNotification(DateTime time, string title, string message)
	{
		if (time.CompareTo(CDateTime.Now) > 0 && !DeviceUtility.isKindleDevice)
		{
			Notifications.instance.ScheduleLocalNotification(time, title, message, "cpush.wav");
		}
	}

	private string GetLocalNotificationMessage()
	{
		WordHuntManager.WordHuntReward rewardForDay = WordHuntManager.GetRewardForDay(EventsSave.wordHuntWordsInRow);
		if (rewardForDay.type == WordHuntManager.WordHuntRewardType.MysteryBox)
		{
			if (rewardForDay.mysteryBoxType == MysteryBox.Type.Super)
			{
				return FormatMessageWithNameIfPossible("LOCAL_NOTIFICATION_MESSAGE_SMB", "LOCAL_NOTIFICATION_MESSAGE_SMB_PERSONAL");
			}
			return FormatMessageWithNameIfPossible("LOCAL_NOTIFICATION_MESSAGE_MB", "LOCAL_NOTIFICATION_MESSAGE_MB_PERSONAL");
		}
		int num = 0;
		if (rewardForDay.type == WordHuntManager.WordHuntRewardType.Coins)
		{
			num = rewardForDay.coins;
		}
		return FormatMessageWithNameIfPossible("LOCAL_NOTIFICATION_MESSAGE_COINS", "LOCAL_NOTIFICATION_MESSAGE_COINS_PERSONAL", num);
	}

	public static string FormatMessageWithNameIfPossible(string unidentifiedMessageID, string identifiedMessageID, params object[] extraFormatArgs)
	{
		if (SocialManager.instance.HasFacebookProfile && !Regex.IsMatch(SocialManager.instance.localUserRealName, "\\p{IsArabic}|\\p{IsHebrew}|\\p{IsSyriac}|\\p{IsThaana}"))
		{
			object[] array = new object[extraFormatArgs.Length + 1];
			array[0] = SocialManager.instance.localUserRealName;
			Array.Copy(extraFormatArgs, 0, array, 1, extraFormatArgs.Length);
			return string.Format(Strings.Get(identifiedMessageID), array);
		}
		return string.Format(Strings.Get(unidentifiedMessageID), extraFormatArgs);
	}

	[UsedImplicitly]
	private static void MakeIOSNativeLog(string logMessage, string stackTrace, LogType type)
	{
		DeviceUtility.MakeIOSNSLog(string.Format("{0}{1}===={1}{2}", logMessage, Environment.NewLine, stackTrace));
	}

	private void OnFriendsConsolidated(bool success)
	{
		SocialManager instance = SocialManager.instance;
		int num = 0;
		bool facebookIsLoggedIn = instance.facebookIsLoggedIn;
		if (facebookIsLoggedIn && !UserSave.hasPaidOutFacebookReward)
		{
			num |= 1;
		}
		TriggerActionForFacebookPayout(facebookIsLoggedIn);
		SaveManager.Save();
		if (num != 0)
		{
			instance.AnnounceToFriends(num);
		}
		RefreshTopRun();
	}

	private static void TriggerActionForFacebookPayout(bool isLoggedInToFacebook)
	{
		Action<bool> action = delegateFacebookPayoutPopupAction;
		if (action != null)
		{
			action(isLoggedInToFacebook);
		}
	}

	private static void QueueFacebookPayoutPopupIfNeeded(bool isLoggedInToFacebook)
	{
		if (Game.IsInstantiated && !Game.Instance.IsInGame.Value && isLoggedInToFacebook && !UserSave.hasPaidOutFacebookReward)
		{
			UIScreenController.QueueFacebookPayoutPopup();
		}
	}

	public static void SetFacebookPayoutAction(Action<bool> action)
	{
		delegateFacebookPayoutPopupAction = action;
	}

	public static void SetFacebookPayoutActionToDefault()
	{
		delegateFacebookPayoutPopupAction = QueueFacebookPayoutPopupIfNeeded;
	}

	public static void SaveAudioOptionsBeforeVideoAds(bool sfxEnabled, bool musicEnabled)
	{
		PlayerPrefs.SetInt("sfxBeforeLostFocus", sfxEnabled ? 1 : 0);
		PlayerPrefs.SetInt("musicBeforeLostFocus", musicEnabled ? 1 : 0);
	}

	public static void DeleteSoundOptionBeforeVideoAds()
	{
		if (PlayerPrefs.HasKey("sfxBeforeLostFocus"))
		{
			PlayerPrefs.DeleteKey("sfxBeforeLostFocus");
		}
		if (PlayerPrefs.HasKey("musicBeforeLostFocus"))
		{
			PlayerPrefs.DeleteKey("musicBeforeLostFocus");
		}
	}

	public static void TryRestoreSoundOptionAfterLostFocusIfNeeded()
	{
		if (PlayerPrefs.HasKey("sfxBeforeLostFocus"))
		{
			Settings.optionSfx = PlayerPrefs.GetInt("sfxBeforeLostFocus", 1) != 0;
		}
		if (PlayerPrefs.HasKey("musicBeforeLostFocus"))
		{
			Settings.optionMusic = PlayerPrefs.GetInt("musicBeforeLostFocus", 1) != 0;
		}
		DeleteSoundOptionBeforeVideoAds();
	}

	private void VerifyCharactersAndBoardsAmount()
	{
	}

	public static void RefreshOnlineSettingsAndInappsIfNeeded()
	{
		float num = 3600f;
		string valueString;
		if (OnlineSettings.instance.TryGetValue("refreshinterval", out valueString))
		{
			float result;
			if (float.TryParse(valueString, out result))
			{
				if (result >= 10f)
				{
					num = result;
				}
				else
				{
					LogError("OnlineSettings refresh interval too small: " + num);
				}
			}
			else
			{
				LogError("Failed to parse Onlinesettings refresh interval from: " + valueString);
			}
		}
		if (Time.realtimeSinceStartup > _onlineSettingsLastDownloadTime + num)
		{
			_onlineSettingsLastDownloadTime = Time.realtimeSinceStartup;
			if (!OnlineSettings.instance.isDownloading)
			{
				OnlineSettings.instance.DownloadNow();
			}
			OnlineSettingsManager.DownloadNow();
			if (InAppManager.isInitialized)
			{
				InAppManager.instance.RequestIAPInfo();
			}
		}
		else if (InAppManager.isInitialized && !InAppManager.instance.ProductRequestSucceeded())
		{
			InAppManager.instance.RequestIAPInfo();
		}
	}

	private void OnOpenedThroughNotification(Notification notification)
	{
		string text = string.Empty;
		string jobID = null;
		Dictionary<string, object> userInfo = notification.userInfo;
		if (userInfo != null && userInfo["type"] != null)
		{
			text = userInfo["type"].ToString();
			if (text == "job")
			{
				jobID = userInfo["jobid"].ToString();
			}
		}
		LogApplicationWasLaunchedFromNotification(notification.message, text, jobID);
	}

	private void LogApplicationWasLaunchedFromNotification(string message, string type = "", string jobID = "")
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		char[] array = message.ToCharArray();
		foreach (char c in array)
		{
			int num = c;
			if (num > 31 && num < 127 && num != 59)
			{
				stringBuilder.Append(c);
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			stringBuilder.Append(" [IllegalChars]");
		}
		Flurry.LogEventWithSeveralParameters("PushNotificationOpen", "Message;Type;JobID", string.Concat(stringBuilder, ";", type, ";", jobID));
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_HOUSEKEEPER_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
