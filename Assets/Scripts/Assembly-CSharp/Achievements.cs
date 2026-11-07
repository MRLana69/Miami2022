using Data.Save;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;

public class Achievements
{
	public const int ACHIEVEMENT_COUNT = 41;

	private string[] _achievementIds = new string[41]
	{
		"Banker", "MagneticCenter", "Score_400000_points.", "OverwhelmingPower", "Simon_says_jump", "ABC", "Happy_Birthday", "Boxing_day", "Welcome_back", "One_legged",
		"Batter_Up", "Tonyhawk", "ItsABird", "Attractive", "pussinboots", "DoubleorNothing", "AllMissionsCompleted", "Elite", "FullThrottle", "TrickyTreasure",
		"MeepMeep", "UnlockKing", "GothGirl", "UnlockNinja", "UnlockFrank", "FoxyCleopatra", "BestFriends", "AllTrophiesCollected", "HaveChar11", "HaveChar12",
		"HaveChar13", "HaveChar14", "HaveChar15", "FacebookLoggedIn", "LandOnTrain", "ActivePowerUps", "MissionSetSingleRun", "ScoreWithoutPowerups", "HoverBoardMaster", "MysteryBoxGrandPrize",
		"HaveYutani"
	};

	private Mission[] _achievementArray = new Mission[41]
	{
		new Mission(MissionType.EarnCoin, 50000),
		new Mission(MissionType.CoinsWithMagnet, 2000),
		new Mission(MissionType.ScoreSingleRun, 400000),
		new Mission(MissionType.Powerups, 100),
		new Mission(MissionType.GuardJump, 100),
		new Mission(MissionType.Letters, 26),
		new Mission(MissionType.MysteryBoxes, 25),
		new Mission(MissionType.BuyMysterybox, 10),
		new Mission(MissionType.DailyQuests, 10),
		new Mission(MissionType.BumpBarrier, 50),
		new Mission(MissionType.HoverBoard, 40),
		new Mission(MissionType.HoverBoardExpire, 20),
		new Mission(MissionType.JetpackSingleRun, 5),
		new Mission(MissionType.MagnetsSingleRun, 6),
		new Mission(MissionType.SuperSneakersSingleRun, 6),
		new Mission(MissionType.DoubleMultiplierSingleRun, 6),
		new Mission(MissionType.ReachMissionSet, 29),
		new Mission(MissionType.HaveUpgrades, 20),
		new Mission(MissionType.HaveHeadStartLarge, 10),
		new Mission(MissionType.NoCoinsWithoutScore, 30000),
		new Mission(MissionType.Headstart, 15),
		new Mission(MissionType.HaveKing, 1),
		new Mission(MissionType.HaveLucy, 1),
		new Mission(MissionType.HaveNinja, 1),
		new Mission(MissionType.HaveFrank, 1),
		new Mission(MissionType.HaveFrizzy, 1),
		new Mission(MissionType.PokeFriend, 10),
		new Mission(MissionType.HaveTrophiesFirstAchievements, 8),
		new Mission(MissionType.HaveTag, 1),
		new Mission(MissionType.HaveTasha, 1),
		new Mission(MissionType.HaveZoe, 1),
		new Mission(MissionType.HaveBrody, 1),
		new Mission(MissionType.HavePrinceK, 1),
		new Mission(MissionType.FacebookLoggedIn, 1),
		new Mission(MissionType.LandOnTrainInRow, 15),
		new Mission(MissionType.ActivePowerups, 3),
		new Mission(MissionType.MissionSetSingleRun, 2),
		new Mission(MissionType.NoPowerUpsWithoutScore, 200000),
		new Mission(MissionType.HoverBoardExpireSingleRun, 6),
		new Mission(MissionType.MysteryBoxGrandPrize, 1),
		new Mission(MissionType.HaveYutani, Characters.characterData[Characters.CharacterId.yutani].Price)
	};

	private static Achievements _instance;

	public string[] achievementIds
	{
		get
		{
			return _achievementIds;
		}
	}

	public Mission[] achievementArray
	{
		get
		{
			return _achievementArray;
		}
	}

	public static Achievements Instance
	{
		get
		{
			return _instance ?? (_instance = new Achievements());
		}
	}

	public int GetNumberOfHoverboardsUsed()
	{
		int achievementIndexInArray = GetAchievementIndexInArray(MissionType.HoverBoard);
		if (achievementArray.ValidIndex(achievementIndexInArray))
		{
			return PlayerSave.GetAchievementProgress(achievementIndexInArray);
		}
		return 0;
	}

	public int GetAchievementIndexInArray(MissionType type)
	{
		for (int i = 0; i < 41; i++)
		{
			if (achievementArray[i].type == type)
			{
				return i;
			}
		}
		return -1;
	}
}
