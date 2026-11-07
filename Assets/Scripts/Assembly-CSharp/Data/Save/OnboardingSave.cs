using System;

namespace Data.Save
{
	public class OnboardingSave : DataManager<OnboardingSave>
	{
		public static bool allowSellHeadstartDuringRun
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.allowSellHeadstartDuringRun;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.allowSellHeadstartDuringRun = value;
			}
		}

		public static bool allowSellScoreboosterDuringRun
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.allowSellScoreboosterDuringRun;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.allowSellScoreboosterDuringRun = value;
			}
		}

		public static bool hasCollectedFromFriends
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.hasCollectedFromFriends;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.hasCollectedFromFriends = value;
			}
		}

		public static bool hasShownCollectPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.hasShownCollectPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.hasShownCollectPopup = value;
			}
		}

		public static bool hasShownFacebookPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.hasShownFacebookPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.hasShownFacebookPopup = value;
			}
		}

		public static bool hasShownHoverboardPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.hasShownHoverboardPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.hasShownHoverboardPopup = value;
			}
		}

		public static bool hasShownMissionIntroPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.hasShownMissionIntroPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.hasShownMissionIntroPopup = value;
			}
		}

		public static bool hasShownEndGameMissionPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.hasShownEndGameMissionPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.hasShownEndGameMissionPopup = value;
			}
		}

		public static bool isTutorialCompleted
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.isTutorialCompleted;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.isTutorialCompleted = value;
			}
		}

		public static int numberOfRunsSinceLastGuideline
		{
			get
			{
				return DataManager<OnboardingSave>.local.numberOfRunsSinceLastGuideline;
			}
			set
			{
				DataManager<OnboardingSave>.local.numberOfRunsSinceLastGuideline = value;
			}
		}

		public static int currentSessionCount
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.currentSessionCount;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.currentSessionCount = value;
			}
		}

		public static int currentHoverboardTutorialStep
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.currentHoverboardTutorialStep;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.currentHoverboardTutorialStep = value;
			}
		}

		public static int numberOfHoverboardsUsed
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.numberOfHoverboardsUsed;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.numberOfHoverboardsUsed = value;
			}
		}

		public static int firstNonTutorialRunSession
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.firstNotInTutorialRunSession;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.firstNotInTutorialRunSession = value;
			}
		}

		public static bool shouldShowCollectPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.shouldShowCollectPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.shouldShowCollectPopup = value;
			}
		}

		public static bool shouldShowFacebookPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.shouldShowFacebookPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.shouldShowFacebookPopup = value;
			}
		}

		public static bool shouldShowHoverboardPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.shouldShowHoverboardPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.shouldShowHoverboardPopup = value;
			}
		}

		public static bool shouldShowMissionIntroPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.shouldShowMissionIntroductionPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.shouldShowMissionIntroductionPopup = value;
			}
		}

		public static bool shouldShowEndGameMissionPopup
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.shouldShowEndGameMissionPopup;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.shouldShowEndGameMissionPopup = value;
			}
		}

		public static int lastShownTopRunIntroPopupVersionNumber
		{
			get
			{
				return DataManager<OnboardingSave>.cloud.lastShownTopRunIntroPopupVersionNumber;
			}
			set
			{
				DataManager<OnboardingSave>.cloud.lastShownTopRunIntroPopupVersionNumber = value;
			}
		}

		public static DateTime lastShownCloudSaveIntroduction
		{
			get
			{
				return DataManager<OnboardingSave>.local.lastShownCloudSaveIntroduction;
			}
			set
			{
				DataManager<OnboardingSave>.local.lastShownCloudSaveIntroduction = value;
			}
		}

		public static bool ShouldShowStickersForCharacterThemes()
		{
			return hasShownHoverboardPopup || hasShownCollectPopup || hasShownFacebookPopup || hasShownMissionIntroPopup;
		}

		public static bool ShouldShowStickersForHoverboards()
		{
			return hasShownHoverboardPopup || hasShownCollectPopup || hasShownFacebookPopup || hasShownMissionIntroPopup;
		}
	}
}
