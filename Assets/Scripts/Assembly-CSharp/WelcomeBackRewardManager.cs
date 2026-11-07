using System;
using System.Diagnostics;
using Data.Save;
using Info;
using Kiloo.Common.Plugins.Notifications;
using Kiloo.Common.Utility;
using UI.Popups.WelcomeBackRewardPopupInternal;
using UI.Screens.RewardManagement;
using UnityEngine;

public class WelcomeBackRewardManager
{
	private const int HOURS_IN_A_DAY = 24;

	private const string MIN_HOURS_OFFLINE_TO_SHOW_WELCOME_BACK_REWARD_ONLINE_SETTINGS_KEY = "min_hours_offline_to_show_welcome_back_reward";

	private const float MIN_HOURS_OFFLINE_TO_SHOW_WELCOME_BACK_REWARD_DEFAULT_VALUE = 72f;

	private const string WELCOME_BACK_REWARD_TYPE_ONLINE_SETTINGS_KEY = "welcome_back_reward_type";

	private const CelebrationRewardType WELCOME_BACK_REWARD_TYPE_DEFAULT_VALUE = CelebrationRewardType.keys;

	private const string WELCOME_BACK_REWARD_AMOUNT_ONLINE_SETTINGS_KEY = "welcome_back_reward_amount";

	private const int WELCOME_BACK_REWARD_AMOUNT_DEFAULT_VALUE = 3;

	private static readonly WelcomeBackRewardItem[] _bonusOfferItems = new WelcomeBackRewardItem[7]
	{
		new WelcomeBackRewardItem
		{
			itemType = CelebrationRewardType.powerup,
			amount = 2,
			upgradeType = UpgradeType.hoverboard
		},
		new WelcomeBackRewardItem
		{
			itemType = CelebrationRewardType.powerup,
			amount = 4,
			upgradeType = UpgradeType.hoverboard
		},
		new WelcomeBackRewardItem
		{
			itemType = CelebrationRewardType.powerup,
			amount = 1,
			upgradeType = UpgradeType.headstart2000
		},
		new WelcomeBackRewardItem
		{
			itemType = CelebrationRewardType.powerup,
			amount = 2,
			upgradeType = UpgradeType.headstart2000
		},
		new WelcomeBackRewardItem
		{
			itemType = CelebrationRewardType.powerup,
			amount = 2,
			upgradeType = UpgradeType.scorebooster
		},
		new WelcomeBackRewardItem
		{
			itemType = CelebrationRewardType.coins,
			amount = 1000
		},
		new WelcomeBackRewardItem
		{
			itemType = CelebrationRewardType.coins,
			amount = 2000
		}
	};

	private static bool _shouldShowWelcomeBackRewardPopupInThisSession;

	public static CelebrationRewardType welcomeBackRewardType
	{
		get
		{
			return (CelebrationRewardType)Enum.Parse(typeof(CelebrationRewardType), OnlineSettings.instance.GetValue("welcome_back_reward_type", CelebrationRewardType.keys.ToString()));
		}
	}

	public static int welcomeBackRewardAmount
	{
		get
		{
			return OnlineSettings.instance.GetValue("welcome_back_reward_amount", 3);
		}
	}

	public static void Init()
	{
		OnlineSettings instance = OnlineSettings.instance;
		instance.onOnlineSettingsDownloadResult = (Action<bool>)Delegate.Combine(instance.onOnlineSettingsDownloadResult, new Action<bool>(OnOnlineSettingsDownloadResult));
		ApplicationState.quit += OnPlayerWentOffline;
		ApplicationState.paused += OnPlayerWentOffline;
		UserSave.onKnownServerTimeChange += OnLastKnownServerTimeChanged;
	}

	public static void ShowPopupIfNeeded()
	{
		if (CanShowPopup() && _shouldShowWelcomeBackRewardPopupInThisSession)
		{
			UIScreenController.Instance.QueuePopup("WelcomeBackRewardPopup");
			_shouldShowWelcomeBackRewardPopupInThisSession = false;
		}
	}

	private static bool CanShowPopup()
	{
		return OnboardingSave.isTutorialCompleted;
	}

	private static TimeSpan PeriodHasBeenOffline()
	{
		return UserSave.lastKnownServerTime - UserSave.serverTimeWhenWentOffline;
	}

	private static void OnLastKnownServerTimeChanged()
	{
		OnPlayerWentOnline();
	}

	private static void OnOnlineSettingsDownloadResult(bool downloadSuccess)
	{
		if (!downloadSuccess)
		{
			OnPlayerWentOffline();
		}
		else if (DebugSave.useLocalTimeInsteadOfServerTime)
		{
			OnPlayerWentOnline();
		}
	}

	private static void OnPlayerWentOnline()
	{
		if (UserSave.isOffline)
		{
			_shouldShowWelcomeBackRewardPopupInThisSession = PeriodHasBeenOffline().TotalHours >= (double)GetMinHoursOfflineToShowWelcomeBackReward();
			UserSave.isOffline = false;
		}
	}

	private static float GetMinHoursOfflineToShowWelcomeBackReward()
	{
		return OnlineSettings.instance.GetValue("min_hours_offline_to_show_welcome_back_reward", 72f);
	}

	private static void OnPlayerWentOffline()
	{
		UserSave.isOffline = true;
		UserSave.serverTimeWhenWentOffline = UserSave.lastKnownServerTime;
		Notifications.instance.ScheduleLocalNotification(DateTime.Now + TimeSpan.FromHours(GetMinHoursOfflineToShowWelcomeBackReward()), Strings.Get("WELCOME_BACK_REWARD_NOTIFICATION_TITLE"), Strings.Get("WELCOME_BACK_REWARD_NOTIFICATION_MESSAGE"), "cpush.wav");
	}

	public static WelcomeBackRewardItem GetRandomBonusOfferItem()
	{
		if (Globals.DEBUG_WELCOME_BACK_REWARD_POPUP_ENABLED && Globals.DEBUG_WELCOME_BACK_REWARD_LOOT_ITEM_TYPE != CelebrationRewardType._notset)
		{
			WelcomeBackRewardItem welcomeBackRewardItem = new WelcomeBackRewardItem();
			welcomeBackRewardItem.itemType = Globals.DEBUG_WELCOME_BACK_REWARD_LOOT_ITEM_TYPE;
			welcomeBackRewardItem.upgradeType = Globals.DEBUG_WELCOME_BACK_REWARD_LOOT_ITEM_UPGRADE_TYPE;
			welcomeBackRewardItem.amount = Globals.DEBUG_WELCOME_BACK_REWARD_LOOT_ITEM_AMOUNT;
			return welcomeBackRewardItem;
		}
		return _bonusOfferItems[UnityEngine.Random.Range(0, _bonusOfferItems.Length)];
	}

	[Conditional("ENABLE_WELCOME_BACK_REWARD_LOGS")]
	public static void Log(object message)
	{
	}

	public static void LogWarning(object message)
	{
	}

	public static void LogError(object message)
	{
		SDebug.LogError("[WelcomeBackRewardManager] " + message);
	}
}
