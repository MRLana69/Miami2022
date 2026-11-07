using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using InApp;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UI.Breadcrumbs;
using UI.Screens.StoreScreenInternal;

namespace Data.Save
{
	public class UserSave : DataManager<UserSave>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onKnownServerTimeChange__BackingField;

		public static bool hasMigratedPlayerInfo
		{
			get
			{
				return DataManager<UserSave>.local.hasMigratedPlayerInfo;
			}
			set
			{
				DataManager<UserSave>.local.hasMigratedPlayerInfo = value;
			}
		}

		public static int migratedPlayerPrefsVersion
		{
			get
			{
				return DataManager<UserSave>.local.migratedPlayerPrefsVersion;
			}
			set
			{
				DataManager<UserSave>.local.migratedPlayerPrefsVersion = value;
			}
		}

		public static DateTime lastKnownServerTime
		{
			get
			{
				if (DebugSave.useLocalTimeInsteadOfServerTime)
				{
					return CDateTime.Now;
				}
				return DataManager<UserSave>.local.lastKnownServerTime;
			}
			set
			{
				DataManager<UserSave>.local.lastKnownServerTime = value;
				onKnownServerTimeChange__BackingField.SafeInvoke();
			}
		}

		public static DateTime serverTimeWhenWentOffline
		{
			get
			{
				return DataManager<UserSave>.local.serverTimeWhenWentOffline;
			}
			set
			{
				DataManager<UserSave>.local.serverTimeWhenWentOffline = value;
			}
		}

		public static bool isOffline
		{
			get
			{
				return DataManager<UserSave>.local.isOffline;
			}
			set
			{
				DataManager<UserSave>.local.isOffline = value;
			}
		}

		public static bool hasPurchasedOrRestored
		{
			get
			{
				return inAppConsumablePurchaseCount > 0 || inAppNonConsumablePurchaseCount > 0 || inAppRestoredPurchaseCount > 0 || inAppLegacyPurchaseCount > 0;
			}
		}

		public static int inAppLegacyPurchaseCount
		{
			get
			{
				return DataManager<UserSave>.cloud.inAppLegacyPurchaseCount;
			}
			set
			{
				DataManager<UserSave>.cloud.inAppLegacyPurchaseCount = value;
			}
		}

		public static int inAppConsumablePurchaseCount
		{
			get
			{
				return DataManager<UserSave>.cloud.inAppConsumablePurchaseCount;
			}
			set
			{
				DataManager<UserSave>.cloud.inAppConsumablePurchaseCount = value;
			}
		}

		public static int inAppNonConsumablePurchaseCount
		{
			get
			{
				return DataManager<UserSave>.cloud.inAppNonConsumablePurchaseCount;
			}
			set
			{
				DataManager<UserSave>.cloud.inAppNonConsumablePurchaseCount = value;
			}
		}

		public static int inAppRestoredPurchaseCount
		{
			get
			{
				return DataManager<UserSave>.cloud.inAppRestoredPurchaseCount;
			}
			set
			{
				DataManager<UserSave>.cloud.inAppRestoredPurchaseCount = value;
			}
		}

		public static Dictionary<string, string> inAppPurchaseHistory
		{
			get
			{
				return DataManager<UserSave>.cloud.inAppPurchaseHistory;
			}
			set
			{
				DataManager<UserSave>.cloud.inAppPurchaseHistory = value;
			}
		}

		public static bool isFreshInstall
		{
			get
			{
				return DataManager<UserSave>.cloud.isFreshInstall;
			}
			set
			{
				DataManager<UserSave>.cloud.isFreshInstall = value;
			}
		}

		public static DateTime lastDailyOnlineLog
		{
			get
			{
				return DataManager<UserSave>.cloud.lastDailyOnlineLog;
			}
			set
			{
				DataManager<UserSave>.cloud.lastDailyOnlineLog = value;
			}
		}

		public static DateTime lastPlayDate
		{
			get
			{
				return DataManager<UserSave>.cloud.lastPlayDate;
			}
			set
			{
				DataManager<UserSave>.cloud.lastPlayDate = value;
			}
		}

		public static DateTime lastQuitDate
		{
			get
			{
				return DataManager<UserSave>.cloud.lastQuitDate;
			}
			set
			{
				DataManager<UserSave>.cloud.lastQuitDate = value;
			}
		}

		public static InAppData.BundleTier lastPurchasedBundle
		{
			get
			{
				return DataManager<UserSave>.cloud.lastPurchasedBundle;
			}
			set
			{
				DataManager<UserSave>.cloud.lastPurchasedBundle = value;
			}
		}

		public static bool hasPaidOutFacebookReward
		{
			get
			{
				return DataManager<UserSave>.cloud.hasPaidOutFacebookReward;
			}
			set
			{
				DataManager<UserSave>.cloud.hasPaidOutFacebookReward = value;
			}
		}

		public static bool allAssetBundlesDownloaded
		{
			get
			{
				return DataManager<UserSave>.local.allAssetBundlesDownloaded;
			}
			set
			{
				DataManager<UserSave>.local.allAssetBundlesDownloaded = value;
			}
		}

		public static int latestAvailableAssetBundlesVersion
		{
			get
			{
				return DataManager<UserSave>.local.latestAvailableAssetBundlesVersion;
			}
			set
			{
				DataManager<UserSave>.local.latestAvailableAssetBundlesVersion = value;
			}
		}

		public static Dictionary<Characters.CharacterId, HashSet<int>> characterOutfitsSeen
		{
			private get
			{
				return DataManager<UserSave>.cloud.characterOutfitsSeen;
			}
			set
			{
				DataManager<UserSave>.cloud.characterOutfitsSeen = value;
			}
		}

		public static CollectCoinsDummy[] collectCoinsDummyData
		{
			get
			{
				return DataManager<UserSave>.cloud.collectCoinsDummyData;
			}
			set
			{
				DataManager<UserSave>.cloud.collectCoinsDummyData = value;
			}
		}

		public static bool hasSeenFrontScreenFirstTime
		{
			get
			{
				return DataManager<UserSave>.cloud.hasSeenFrontScreenFirstTime;
			}
			set
			{
				DataManager<UserSave>.cloud.hasSeenFrontScreenFirstTime = value;
			}
		}

		public static bool hasMadeOneValidPurchase
		{
			get
			{
				return DataManager<UserSave>.cloud.hasMadeOneValidPurchase;
			}
			set
			{
				DataManager<UserSave>.cloud.hasMadeOneValidPurchase = value;
			}
		}

		public static DateTime firstInstallDate
		{
			get
			{
				return DataManager<UserSave>.cloud.firstInstallDate;
			}
			set
			{
				DataManager<UserSave>.cloud.firstInstallDate = value;
			}
		}

		public static string firstInstalledVersion
		{
			get
			{
				return DataManager<UserSave>.cloud.firstInstalledVersion;
			}
			set
			{
				DataManager<UserSave>.cloud.firstInstalledVersion = value;
			}
		}

		public static bool hasAdRemovalUpgrade
		{
			get
			{
				return DataManager<UserSave>.cloud.hasAdRemovalUpgrade;
			}
			set
			{
				if (DataManager<UserSave>.cloud.hasAdRemovalUpgrade != value)
				{
					DataManager<UserSave>.cloud.hasAdRemovalUpgrade = value;
				}
			}
		}

		public static bool hasDoubleCoinsUpgrade
		{
			get
			{
				return DataManager<UserSave>.cloud.hasDoubleCoinsUpgrade;
			}
			set
			{
				DataManager<UserSave>.cloud.hasDoubleCoinsUpgrade = value;
			}
		}

		public static Dictionary<Characters.CharacterId, bool> hasCharacterBeenSeen
		{
			get
			{
				return DataManager<UserSave>.cloud.hasCharacterBeenSeen;
			}
			set
			{
				DataManager<UserSave>.cloud.hasCharacterBeenSeen = value;
			}
		}

		public static Dictionary<Boards.BoardId, bool> hasBoardBeenSeen
		{
			get
			{
				return DataManager<UserSave>.cloud.hasBoardBeenSeen;
			}
			set
			{
				DataManager<UserSave>.cloud.hasBoardBeenSeen = value;
			}
		}

		public static bool hasResetTokensForUnlockedCharacters
		{
			get
			{
				return DataManager<UserSave>.cloud.hasResetTokensForUnlockedCharacters;
			}
			set
			{
				DataManager<UserSave>.cloud.hasResetTokensForUnlockedCharacters = value;
			}
		}

		public static bool hasUserRecievedFreeTokenMysteryBox
		{
			get
			{
				return DataManager<UserSave>.cloud.hasUserRecievedFreeTokenMysteryBox;
			}
			set
			{
				DataManager<UserSave>.cloud.hasUserRecievedFreeTokenMysteryBox = value;
			}
		}

		public static Dictionary<EarnCoinsData.Id, string> earnCurrencyData
		{
			get
			{
				return DataManager<UserSave>.cloud.earnCurrencyData;
			}
			set
			{
				DataManager<UserSave>.cloud.earnCurrencyData = value;
			}
		}

		public static Dictionary<string, string> earnCurrencyCustomSocialLinkData
		{
			get
			{
				return DataManager<UserSave>.cloud.earnCurrencyCustomSocialLinkData;
			}
			set
			{
				DataManager<UserSave>.cloud.earnCurrencyCustomSocialLinkData = value;
			}
		}

		public static string lastVersionRated
		{
			get
			{
				return DataManager<UserSave>.local.lastVersionRated;
			}
			set
			{
				DataManager<UserSave>.local.lastVersionRated = value;
			}
		}

		public static DateTime lastTimeAVideoForKeysWasSeen
		{
			get
			{
				return DataManager<UserSave>.cloud.lastTimeAVideoForKeysWasSeen;
			}
			set
			{
				DataManager<UserSave>.cloud.lastTimeAVideoForKeysWasSeen = value;
			}
		}

		public static DateTime lastTimeAppWasUpdatedOrInstalled
		{
			get
			{
				return DataManager<UserSave>.local.lastTimeAppWasUpdatedOrInstalled;
			}
			set
			{
				DataManager<UserSave>.local.lastTimeAppWasUpdatedOrInstalled = value;
			}
		}

		public static bool wereNotificationsEnabled
		{
			get
			{
				return DataManager<UserSave>.local.wereNotificationsEnabled;
			}
			set
			{
				DataManager<UserSave>.local.wereNotificationsEnabled = value;
			}
		}

		public static int numberOfBreadCrumbsShownOnFrontPage
		{
			get
			{
				return DataManager<UserSave>.cloud.numberOfBreadCrumbsShownOnFrontPage;
			}
			set
			{
				DataManager<UserSave>.cloud.numberOfBreadCrumbsShownOnFrontPage = value;
			}
		}

		public static int ageRestrictionInputVersion
		{
			get
			{
				return DataManager<UserSave>.cloud.ageRestrictionInputVersion;
			}
			set
			{
				DataManager<UserSave>.cloud.ageRestrictionInputVersion = value;
			}
		}

		public static int ageRestrictionInputMonth
		{
			get
			{
				return DataManager<UserSave>.cloud.ageRestrictionInputMonth;
			}
			set
			{
				DataManager<UserSave>.cloud.ageRestrictionInputMonth = value;
			}
		}

		public static int ageRestrictionInputYear
		{
			get
			{
				return DataManager<UserSave>.cloud.ageRestrictionInputYear;
			}
			set
			{
				DataManager<UserSave>.cloud.ageRestrictionInputYear = value;
			}
		}

		public static DateTime welcomePackStartTime
		{
			get
			{
				return DataManager<UserSave>.cloud.welcomePackStartTime;
			}
			set
			{
				DataManager<UserSave>.cloud.welcomePackStartTime = value;
			}
		}

		public static bool cloudSaveEnabled
		{
			get
			{
				return DataManager<UserSave>.local.cloudSaveEnabled;
			}
			set
			{
				DataManager<UserSave>.local.cloudSaveEnabled = value;
			}
		}

		public static DateTime latestSuccessfulCloudSaveSyncTime
		{
			get
			{
				return DataManager<UserSave>.local.latestSuccessfulCloudSaveSyncTime;
			}
			set
			{
				DataManager<UserSave>.local.latestSuccessfulCloudSaveSyncTime = value;
			}
		}

		public static DateTime lastCloudSaveReminderShownTime
		{
			get
			{
				return DataManager<UserSave>.local.lastCloudSaveReminderShownTime;
			}
			set
			{
				DataManager<UserSave>.local.lastCloudSaveReminderShownTime = value;
			}
		}

		public static bool hasCloudSaveCelebrationBeenShown
		{
			get
			{
				return DataManager<UserSave>.local.hasCloudSaveCelebrationBeenShown;
			}
			set
			{
				DataManager<UserSave>.local.hasCloudSaveCelebrationBeenShown = value;
			}
		}

		public static int introductionPopupShownCount
		{
			get
			{
				return DataManager<UserSave>.local.introductionPopupShownCount;
			}
			set
			{
				DataManager<UserSave>.local.introductionPopupShownCount = value;
			}
		}

		public static bool hasCloudSaveSuccessfullyPeeked
		{
			get
			{
				return DataManager<UserSave>.local.hasCloudSaveSuccessfullyPeeked;
			}
			set
			{
				DataManager<UserSave>.local.hasCloudSaveSuccessfullyPeeked = value;
			}
		}

		public static bool cloudSaveEverEnabled
		{
			get
			{
				return DataManager<UserSave>.local.cloudSaveEverEnabled;
			}
			set
			{
				DataManager<UserSave>.local.cloudSaveEverEnabled = value;
			}
		}

		public static bool cloudSaveEnabledByPeekRoutine
		{
			get
			{
				return DataManager<UserSave>.local.cloudSaveEnabledByPeekRoutine;
			}
			set
			{
				DataManager<UserSave>.local.cloudSaveEnabledByPeekRoutine = value;
			}
		}

		public static int nativeLoginCanceledVersion
		{
			get
			{
				return DataManager<UserSave>.local.nativeLoginCanceledVersion;
			}
			set
			{
				DataManager<UserSave>.local.nativeLoginCanceledVersion = value;
			}
		}

		public static bool hasUserRunAppBefore
		{
			get
			{
				return DataManager<UserSave>.cloud.hasUserRunAppBefore;
			}
			set
			{
				DataManager<UserSave>.cloud.hasUserRunAppBefore = value;
			}
		}

		public static Breadcrumb.BreadcrumbSave breadcrumbs
		{
			get
			{
				return DataManager<UserSave>.cloud.breadcrumbs;
			}
			set
			{
				DataManager<UserSave>.cloud.breadcrumbs = value;
			}
		}

		public static SSEventType lastEventTypeShown
		{
			get
			{
				return DataManager<UserSave>.cloud.lastEventTypeShown;
			}
			set
			{
				DataManager<UserSave>.cloud.lastEventTypeShown = value;
			}
		}

		public static DateTime lastEventShownTimestamp
		{
			get
			{
				return DataManager<UserSave>.cloud.lastEventShownTimestamp;
			}
			set
			{
				DataManager<UserSave>.cloud.lastEventShownTimestamp = value;
			}
		}

		public static string lastSeenBundleVersion
		{
			get
			{
				return DataManager<UserSave>.cloud.lastSeenBundleVersion;
			}
			set
			{
				DataManager<UserSave>.cloud.lastSeenBundleVersion = value;
			}
		}

		public static string previouslySeenBundleVersion
		{
			get
			{
				return DataManager<UserSave>.cloud.previouslySeenBundleVersion;
			}
			set
			{
				DataManager<UserSave>.cloud.previouslySeenBundleVersion = value;
			}
		}

		public static List<string> promoCodes
		{
			get
			{
				return DataManager<UserSave>.cloud.usedPromoCodes;
			}
			set
			{
				DataManager<UserSave>.cloud.usedPromoCodes = value;
			}
		}

		public static List<string> promoCodeIds
		{
			get
			{
				return DataManager<UserSave>.cloud.usedPromoCodesIds;
			}
			set
			{
				DataManager<UserSave>.cloud.usedPromoCodesIds = value;
			}
		}

		public static DateTime dailyGiftLastGiven
		{
			get
			{
				return DataManager<UserSave>.cloud.dailyGiftLastGiven;
			}
			set
			{
				DataManager<UserSave>.cloud.dailyGiftLastGiven = value;
			}
		}

		public static StoreEventFillerDealSaveData currentStoreFillerDeal
		{
			get
			{
				return DataManager<UserSave>.local.currentStoreFillerDeal;
			}
			set
			{
				DataManager<UserSave>.local.currentStoreFillerDeal = value;
			}
		}

		public static event Action onKnownServerTimeChange
		{
			add
			{
				Action action = onKnownServerTimeChange__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onKnownServerTimeChange__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onKnownServerTimeChange__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onKnownServerTimeChange__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static void OutfitSeen(Characters.CharacterId character, int index)
		{
			HashSet<int> value;
			if (characterOutfitsSeen.TryGetValue(character, out value))
			{
				value.Add(index);
				return;
			}
			characterOutfitsSeen[character] = new HashSet<int> { index };
		}

		public static bool IsCharacterOutfitSeen(Characters.CharacterId character, int index)
		{
			if (index == 0)
			{
				return false;
			}
			HashSet<int> value;
			if (characterOutfitsSeen.TryGetValue(character, out value))
			{
				return value.Contains(index);
			}
			return false;
		}

		public static void IncrementNumberOfBreadCrumbsShownOnFrontPage()
		{
			numberOfBreadCrumbsShownOnFrontPage++;
		}

		public static int GetOfferWallCoinsPaidOut(string providerId)
		{
			return DataManager<UserSave>.cloud.offerWallCoinsPaidOut.ContainsKey(providerId) ? DataManager<UserSave>.cloud.offerWallCoinsPaidOut[providerId] : 0;
		}

		public static void IncrementOfferWallCoinsPaidOut(string providerId, int coins)
		{
			if (!DataManager<UserSave>.cloud.offerWallCoinsPaidOut.ContainsKey(providerId))
			{
				DataManager<UserSave>.cloud.offerWallCoinsPaidOut.Add(providerId, coins);
			}
			else
			{
				DataManager<UserSave>.cloud.offerWallCoinsPaidOut[providerId] += coins;
			}
		}
	}
}
