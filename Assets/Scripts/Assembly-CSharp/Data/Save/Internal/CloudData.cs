using System;
using System.Collections.Generic;
using InApp;
using Info;
using Info.VideoRewardCooldowns;
using Kiloo.Common.Ads;
using Missions;
using SYBO.Components.Analytics;
using SYBO.Components.Analytics.Integration;
using SYBO.Subway.Info;
using TopRun.Internal;
using UI.Breadcrumbs;
using UI.Screens.RewardManagement;
using WeeklyHunt;

namespace Data.Save.Internal
{
	public class CloudData : DiskData
	{
		public int obfuscatedCoins = 0.EncryptDecryptXORValue();

		public int obfuscatedKeys = 5.EncryptDecryptXORValue();

		public int obfuscatedUnrewardedCoins = 0.EncryptDecryptXORValue();

		public Dictionary<UpgradeType, int> powerups = new Dictionary<UpgradeType, int>
		{
			{
				UpgradeType.hoverboard,
				3
			},
			{
				UpgradeType.headstart2000,
				0
			},
			{
				UpgradeType.scorebooster,
				0
			}
		};

		public Dictionary<UpgradeType, int> upgrades = new Dictionary<UpgradeType, int>
		{
			{
				UpgradeType.jetpack,
				0
			},
			{
				UpgradeType.supersneakers,
				0
			},
			{
				UpgradeType.coinmagnet,
				0
			},
			{
				UpgradeType.doubleMultiplier,
				0
			}
		};

		public List<CelebrationReward> pendingRewards = new List<CelebrationReward>();

		public Dictionary<Boards.BoardId, Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>> boardThemeData = new Dictionary<Boards.BoardId, Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>>();

		public List<Characters.CharacterId> unlockedCharacters = new List<Characters.CharacterId> { Characters.CharacterId.slick };

		public Dictionary<Characters.CharacterId, int> collectedCharacterTokens = new Dictionary<Characters.CharacterId, int>();

		public Dictionary<Characters.CharacterId, int> selectedOutfits = new Dictionary<Characters.CharacterId, int>();

		public Dictionary<Characters.CharacterId, List<int>> unlockedOutfits = new Dictionary<Characters.CharacterId, List<int>>();

		public List<Boards.BoardId> unlockedBoards = new List<Boards.BoardId>();

		public bool[] hasSkippedMissions = new bool[3];

		public int runsCompletedInCurrentMissionSet = -1;

		public int currentMissionSet = -1;

		public int[] currentMissionProgress = new int[3];

		public int missionSetCompletedCount;

		public int currentSkipForVideoMissionIndex;

		public DateTime currentSkipForVideoIndexSetAt = DateTime.MinValue;

		public DateTime lastTimeMissionWasSkippedForVideo = DateTime.MinValue;

		public bool hasSkippedMissionForVideoThisSet;

		public List<int> achievementProgress = new List<int>(41);

		public int unreportedGames;

		public DateTime unreportedGamesTimeStamp = DateTime.MinValue;

		public int highScore;

		public bool[] unlockedTrophies = new bool[Enum.GetValues(typeof(Trophies.Trophy)).Length];

		public Dictionary<AwardId, PersistentAwardProgress> awardsProgress = new Dictionary<AwardId, PersistentAwardProgress>();

		public DateTime weeklyGiftUnlockTime = DateTime.MinValue;

		public int[] statValues = new int[Enum.GetNames(typeof(Stat)).Length];

		public int[] mysteryBoxesOpened = new int[Enum.GetNames(typeof(MysteryBox.Type)).Length];

		public int completedRunCount;

		public Characters.CharacterId currentCharacterId;

		public Boards.BoardId currentBoardId;

		public Boards.BoardId previousBoardId;

		public Dictionary<AwardId, bool> awardIsNew = new Dictionary<AwardId, bool>
		{
			{
				AwardId.COLLECT_OLD_TROPHY_ITEMS_MB,
				true
			},
			{
				AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB,
				true
			},
			{
				AwardId.COLLECT_X_COINS_FROM_FRIENDS,
				true
			},
			{
				AwardId.OWN_X_CHARACTERS,
				true
			},
			{
				AwardId.COMPLETE_X_MISSIONS,
				true
			},
			{
				AwardId.COMPLETE_DAILY_CHALLANGES_INROW,
				true
			},
			{
				AwardId.OWN_X_BOARDS,
				true
			},
			{
				AwardId.TOPRUN_RANK5_OR_BETTER,
				true
			},
			{
				AwardId.PICKUP_POWERUPS,
				true
			},
			{
				AwardId.OWN_CHARACTER_OUTFITS,
				true
			},
			{
				AwardId.SCORE_POINTS_SINGLE_RUN_NO_COINS,
				true
			},
			{
				AwardId.COMPLETE_MISSIONS_SINGLE_RUN,
				true
			},
			{
				AwardId.TOPRUN_RANK_1,
				true
			},
			{
				AwardId.HAVE_SUPERSNICKERS_ACTIVE_X_MIN_IN_A_ROW,
				true
			},
			{
				AwardId.COLLECT_COINS_SINGLE_RUN,
				true
			},
			{
				AwardId.POKE_FRIENDS_X_TIMES,
				true
			},
			{
				AwardId.PICK_X_KEYS_INGAME,
				true
			},
			{
				AwardId.OPEN_X_MYSTERY_BOXES,
				true
			},
			{
				AwardId.BEAT_X_TOPRUN_FRIENDS,
				true
			},
			{
				AwardId.SCORE_POINTS_SINGLE_RUN_NO_JUMP_OR_ROLL,
				true
			},
			{
				AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES,
				true
			},
			{
				AwardId.WIN_X_JACKPOTS,
				true
			},
			{
				AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES_IN_ONE_WEEKEND,
				true
			},
			{
				AwardId.OPEN_X_SUPER_MYSTERY_BOXES,
				true
			},
			{
				AwardId.COMPLETE_HUNT_SINGLE_RUN_X_TIMES,
				true
			},
			{
				AwardId.TOPRUN_RANK_0,
				true
			},
			{
				AwardId.SCORE_POINTS_SINGLE_LANE,
				true
			},
			{
				AwardId.WATCH_VIDEO_FOR_KEYS,
				true
			}
		};

		public Dictionary<AwardId, bool> awardHasPayedOut = new Dictionary<AwardId, bool>
		{
			{
				AwardId.COLLECT_OLD_TROPHY_ITEMS_MB,
				false
			},
			{
				AwardId.COLLECT_OLD_TROPHY_ITEMS_SMB,
				false
			},
			{
				AwardId.COLLECT_X_COINS_FROM_FRIENDS,
				false
			},
			{
				AwardId.OWN_X_CHARACTERS,
				false
			},
			{
				AwardId.COMPLETE_X_MISSIONS,
				false
			},
			{
				AwardId.COMPLETE_DAILY_CHALLANGES_INROW,
				false
			},
			{
				AwardId.OWN_X_BOARDS,
				false
			},
			{
				AwardId.TOPRUN_RANK5_OR_BETTER,
				false
			},
			{
				AwardId.PICKUP_POWERUPS,
				false
			},
			{
				AwardId.OWN_CHARACTER_OUTFITS,
				false
			},
			{
				AwardId.SCORE_POINTS_SINGLE_RUN_NO_COINS,
				false
			},
			{
				AwardId.COMPLETE_MISSIONS_SINGLE_RUN,
				false
			},
			{
				AwardId.TOPRUN_RANK_1,
				false
			},
			{
				AwardId.HAVE_SUPERSNICKERS_ACTIVE_X_MIN_IN_A_ROW,
				false
			},
			{
				AwardId.COLLECT_COINS_SINGLE_RUN,
				false
			},
			{
				AwardId.POKE_FRIENDS_X_TIMES,
				false
			},
			{
				AwardId.PICK_X_KEYS_INGAME,
				false
			},
			{
				AwardId.OPEN_X_MYSTERY_BOXES,
				false
			},
			{
				AwardId.BEAT_X_TOPRUN_FRIENDS,
				false
			},
			{
				AwardId.SCORE_POINTS_SINGLE_RUN_NO_JUMP_OR_ROLL,
				false
			},
			{
				AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES,
				false
			},
			{
				AwardId.WIN_X_JACKPOTS,
				false
			},
			{
				AwardId.COMPLETE_X_WORDY_WEEKEND_CHALLENGES_IN_ONE_WEEKEND,
				false
			},
			{
				AwardId.OPEN_X_SUPER_MYSTERY_BOXES,
				false
			},
			{
				AwardId.COMPLETE_HUNT_SINGLE_RUN_X_TIMES,
				false
			},
			{
				AwardId.TOPRUN_RANK_0,
				false
			},
			{
				AwardId.SCORE_POINTS_SINGLE_LANE,
				false
			},
			{
				AwardId.WATCH_VIDEO_FOR_KEYS,
				false
			}
		};

		public bool awardsFirstLoaded;

		public string weeklyHuntProgressVersion = string.Empty;

		public List<bool> hasLoggedWeeklyHuntPeriod = new List<bool> { true, true, true, true };

		public List<bool> requiredAlternativeHuntPrize = new List<bool> { false, false, false, false };

		public WeeklyHuntProgressData weeklyHuntProgressData = new WeeklyHuntProgressData();

		public int wordHuntWordsInRow;

		public bool wordHuntPayedOut;

		public int wordHuntUnlockedMask;

		public int wordHuntLastPayoutDayOfYear;

		public CharacterNameEventData characterNameEventData = new CharacterNameEventData();

		public long lastEventID;

		public long lastTimeAnEventHasBeenTried;

		public long lastTimeAnEventWasStarted;

		public long lastTimeEventPopupWasForceShown;

		public string lastEventItemID = string.Empty;

		public bool shouldRestoreItemAfterEvent;

		public string wordHuntDailyWord = string.Empty;

		public DateTime wordHuntExpireTime = DateTime.UtcNow;

		public SampleState syboAnalyticsSampleState = AnalyticsIntegration.SampleStateDefault;

		public string syboAnalyticsUserId = AnalyticsIntegration.AnalyticsUserIdPersistedDefault;

		public string syboAggregatedDataString = string.Empty;

		public Dictionary<string, string> syboAnalyticsUnhandledAbTestData = new Dictionary<string, string>();

		public int syboAnalyticsCurrentSession = AnalyticsIntegration.CurrentSessionPersistedDefault;

		public bool kilooAnalyticsHasLoggedSyboUserId;

		public int persistedSessionEventNumber = -1;

		public ManagerData managerData = new ManagerData();

		public string lastTopRunResultAwardedID = string.Empty;

		public string pendingTopRunResultID = string.Empty;

		public int pendingTopRunResultsScore = -1;

		public int pendingTopRunResultsRank = -1;

		public int pendingTopRunResultsWeek = -1;

		public int pendingTopRunBeatenFriendsAward;

		public bool behavioralAdsAllowed;

		public InterstitialStats interstitialStats = new InterstitialStats();

		public Dictionary<UpgradeType, int> consumableSeenVideosCount = new Dictionary<UpgradeType, int>();

		public Dictionary<UpgradeType, DateTime> consumableVideoSeenAt = new Dictionary<UpgradeType, DateTime>();

		public Cooldowns cooldowns = new Cooldowns();

		public int inAppLegacyPurchaseCount;

		public int inAppConsumablePurchaseCount;

		public int inAppRestoredPurchaseCount;

		public int inAppNonConsumablePurchaseCount;

		public bool isFreshInstall = true;

		public bool hasUserRunAppBefore;

		public DateTime lastDailyOnlineLog = DateTime.MinValue;

		public DateTime lastPlayDate = DateTime.MinValue;

		public DateTime lastQuitDate = DateTime.MinValue;

		public InAppData.BundleTier lastPurchasedBundle;

		public bool hasPaidOutFacebookReward;

		public bool hasSeenFrontScreenFirstTime;

		public bool hasMadeOneValidPurchase;

		public string firstInstalledVersion = string.Empty;

		public DateTime firstInstallDate = DateTime.MinValue;

		public bool hasDoubleCoinsUpgrade;

		public bool hasAdRemovalUpgrade;

		public Dictionary<Characters.CharacterId, bool> hasCharacterBeenSeen = new Dictionary<Characters.CharacterId, bool>();

		public Dictionary<Boards.BoardId, bool> hasBoardBeenSeen = new Dictionary<Boards.BoardId, bool>();

		public bool hasResetTokensForUnlockedCharacters;

		public bool hasUserRecievedFreeTokenMysteryBox;

		public Dictionary<Characters.CharacterId, HashSet<int>> characterOutfitsSeen = new Dictionary<Characters.CharacterId, HashSet<int>>();

		public CollectCoinsDummy[] collectCoinsDummyData = CollectCoinsDummyData.GetDefaultDummyList();

		public Dictionary<EarnCoinsData.Id, string> earnCurrencyData = new Dictionary<EarnCoinsData.Id, string>();

		public Dictionary<string, string> earnCurrencyCustomSocialLinkData = new Dictionary<string, string>();

		public Dictionary<string, string> inAppPurchaseHistory = new Dictionary<string, string>();

		public int numberOfBreadCrumbsShownOnFrontPage;

		public int ageRestrictionInputVersion;

		public int ageRestrictionInputMonth;

		public int ageRestrictionInputYear;

		public Breadcrumb.BreadcrumbSave breadcrumbs = new Breadcrumb.BreadcrumbSave();

		public SSEventType lastEventTypeShown;

		public DateTime lastEventShownTimestamp = DateTime.MinValue;

		public string lastSeenBundleVersion = string.Empty;

		public string previouslySeenBundleVersion = string.Empty;

		public DateTime lastTimeAVideoForKeysWasSeen = DateTime.MinValue;

		public DateTime welcomePackStartTime = DateTime.MinValue;

		public int currentIntroVideoAdPrizeIndex;

		public int currentRandomVideoAdPrizeIndex;

		public int currentVideoAdPrizeSeed;

		public int videosWatchedSinceDailyKeys;

		public List<string> usedPromoCodes = new List<string>();

		public List<string> usedPromoCodesIds = new List<string>();

		public DateTime dailyGiftLastGiven = DateTime.MinValue;

		public Dictionary<string, int> offerWallCoinsPaidOut = new Dictionary<string, int>();

		public Dictionary<string, Friend.Status> friendStatus = new Dictionary<string, Friend.Status>();

		public bool allowSellHeadstartDuringRun;

		public bool allowSellScoreboosterDuringRun;

		public bool hasCollectedFromFriends;

		public bool hasShownCollectPopup;

		public bool hasShownFacebookPopup;

		public bool hasShownHoverboardPopup;

		public bool hasShownMissionIntroPopup;

		public bool hasShownEndGameMissionPopup;

		public bool isTutorialCompleted;

		public bool shouldShowCollectPopup;

		public bool shouldShowFacebookPopup;

		public bool shouldShowHoverboardPopup;

		public bool shouldShowMissionIntroductionPopup;

		public bool shouldShowEndGameMissionPopup;

		public int lastShownTopRunIntroPopupVersionNumber;

		public int currentSessionCount;

		public int numberOfHoverboardsUsed;

		public int firstNotInTutorialRunSession;

		public int currentHoverboardTutorialStep;

		public bool neverAskForRating;

		public string language = string.Empty;

		public bool soundEffectsEnabled = true;

		public bool musicEnabled = true;

		public bool topRunChallengersEnabled = true;

		public bool remoteNotificationsEnabled = true;

		public int localNotificationsEnabled = -1;

		public bool hasLoggedStaticData;

		public DateTime lastLoggedDailyData = DateTime.MinValue;

		public float analyticsSamplingKey = (float)new SyboRandom().NextDouble();

		public DateTime abTestLastDailyEventsReportDate = DateTime.MinValue;

		public string abTestPlayerSeed = new SyboRandom().NextUint().ToString();

		public string abTestTagData = string.Empty;

		public string flurryUserId = string.Empty;

		public bool hasLoggedGameCenterLogin;

		public bool hasLoggedFacebookLogin;

		public Dictionary<string, int> hasLoggedFlurryDailyEventSocial = new Dictionary<string, int>();

		public override void Obfuscate()
		{
			obfuscatedCoins = obfuscatedCoins.EncryptDecryptXORValue();
			obfuscatedKeys = obfuscatedKeys.EncryptDecryptXORValue();
			obfuscatedUnrewardedCoins = obfuscatedUnrewardedCoins.EncryptDecryptXORValue();
		}

		public override void Deobfuscate()
		{
			obfuscatedCoins = obfuscatedCoins.EncryptDecryptXORValue();
			obfuscatedKeys = obfuscatedKeys.EncryptDecryptXORValue();
			obfuscatedUnrewardedCoins = obfuscatedUnrewardedCoins.EncryptDecryptXORValue();
		}
	}
}
