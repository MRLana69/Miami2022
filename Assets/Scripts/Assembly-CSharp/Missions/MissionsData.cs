using System.Collections.Generic;

namespace Missions
{
	public static class MissionsData
	{
		public const int MISSION_COUNT_IN_SET = 3;

		public const int MISSIONS_VERSION = 3;

		public static List<MissionIndicator> missionsModificationsForStoryline = new List<MissionIndicator>
		{
			new MissionIndicator(1, 6, 2),
			new MissionIndicator(1, 13, 0),
			new MissionIndicator(1, 19, 0),
			new MissionIndicator(1, 23, 0),
			new MissionIndicator(3, 7, 1),
			new MissionIndicator(3, 23, 0)
		};

		public static List<MissionIndicator> missionsModificationsForRepeatable = new List<MissionIndicator>
		{
			new MissionIndicator(3, 38, 2)
		};

		public static Dictionary<int, int[]> missionsInsertedForRepeatable = new Dictionary<int, int[]> { 
		{
			1,
			new int[20]
			{
				2, 4, 5, 7, 10, 12, 16, 21, 25, 29,
				31, 33, 35, 38, 39, 40, 41, 42, 43, 44
			}
		} };

		public static Mission[][] repeatableMissions = new Mission[44][]
		{
			new Mission[3]
			{
				new Mission(MissionType.JumpTrainSingleRun, 4),
				new Mission(MissionType.JetpackSingleRun, 3),
				new Mission(MissionType.CoinsWithMagnet, 1200)
			},
			new Mission[3]
			{
				new Mission(MissionType.ActivePowerups, 3),
				new Mission(MissionType.ScoreBooster, 5),
				new Mission(MissionType.CollectCoinsLeftLane, 1500)
			},
			new Mission[3]
			{
				new Mission(MissionType.DodgeBarriersSingleRun, 30),
				new Mission(MissionType.MagnetsSingleRun, 5),
				new Mission(MissionType.Headstart, 10)
			},
			new Mission[3]
			{
				new Mission(MissionType.TimeDeath, 5),
				new Mission(MissionType.SpendKeys, 4),
				new Mission(MissionType.RollLeft, 400)
			},
			new Mission[3]
			{
				new Mission(MissionType.OneOfEachPowerup, 5),
				new Mission(MissionType.EarnCoinWithoutTouchingGround, 100),
				new Mission(MissionType.CollectCoinsRightLane, 1500)
			},
			new Mission[3]
			{
				new Mission(MissionType.HoverBoardExpireSingleRun, 4),
				new Mission(MissionType.DoubleMultiplierSingleRun, 4),
				new Mission(MissionType.DailyQuestInRow, 3)
			},
			new Mission[3]
			{
				new Mission(MissionType.GetExactlyAmountOfCoins, 300),
				new Mission(MissionType.SkipMission, 1),
				new Mission(MissionType.ScoreWithScorebooster, 2000000)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpBush, 5),
				new Mission(MissionType.MysteryBoxes, 10),
				new Mission(MissionType.Roll, 300)
			},
			new Mission[3]
			{
				new Mission(MissionType.GetExactlyAmountOfCoins, 512),
				new Mission(MissionType.PickUpPowerJumpersSingleRun, 5),
				new Mission(MissionType.PickupKeysSingleRun, 3)
			},
			new Mission[3]
			{
				new Mission(MissionType.StayInOneLane, 30),
				new Mission(MissionType.Jetpack, 20),
				new Mission(MissionType.CoinsWithJetpack, 2000)
			},
			new Mission[3]
			{
				new Mission(MissionType.JumpSingleRun, 70),
				new Mission(MissionType.SuperSneakersSingleRun, 5),
				new Mission(MissionType.NoPowerUpsWithoutScore, 120000)
			},
			new Mission[3]
			{
				new Mission(MissionType.MagnetsSingleRun, 5),
				new Mission(MissionType.RollSingleRun, 100),
				new Mission(MissionType.PickupKeys, 5)
			},
			new Mission[3]
			{
				new Mission(MissionType.StayInOneLane, 20),
				new Mission(MissionType.BumpTrainSingleRun, 12),
				new Mission(MissionType.SpendCoin, 9000)
			},
			new Mission[3]
			{
				new Mission(MissionType.ScoreSingleRun, 300000),
				new Mission(MissionType.NoJumpsWithoutScore, 25000),
				new Mission(MissionType.Score, 5000000)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpBarrier, 40),
				new Mission(MissionType.BumpBarrierSingleRun, 4),
				new Mission(MissionType.Jump, 300)
			},
			new Mission[3]
			{
				new Mission(MissionType.GetExactlyAmountOfCoins, 200),
				new Mission(MissionType.CollectCoinsCenterLane, 1500),
				new Mission(MissionType.NoPowerUpsWithoutScore, 120000)
			},
			new Mission[3]
			{
				new Mission(MissionType.EarnCoinSingleRun, 800),
				new Mission(MissionType.OneOfEachPowerup, 5),
				new Mission(MissionType.Letters, 12)
			},
			new Mission[3]
			{
				new Mission(MissionType.JumpTrain, 50),
				new Mission(MissionType.NoCoinsWithoutScore, 10000),
				new Mission(MissionType.CollectCoinsWithPowerJumper, 200)
			},
			new Mission[3]
			{
				new Mission(MissionType.RollSingleRun, 70),
				new Mission(MissionType.BuyMysterybox, 10),
				new Mission(MissionType.EarnCoin, 20000)
			},
			new Mission[3]
			{
				new Mission(MissionType.LandOnTrainInRow, 10),
				new Mission(MissionType.NoRollsWithoutScore, 25000),
				new Mission(MissionType.DodgeBarriers, 150)
			},
			new Mission[3]
			{
				new Mission(MissionType.SpendKeys, 5),
				new Mission(MissionType.Magnets, 20),
				new Mission(MissionType.PickupKeys, 10)
			},
			new Mission[3]
			{
				new Mission(MissionType.HoverBoard, 20),
				new Mission(MissionType.HoverBoardExpire, 8),
				new Mission(MissionType.Powerups, 50)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpLightSignalSingleRun, 5),
				new Mission(MissionType.RollCenter, 200),
				new Mission(MissionType.BumpLightSignal, 40)
			},
			new Mission[3]
			{
				new Mission(MissionType.OneOfEachPowerup, 5),
				new Mission(MissionType.DoubleMultiplierSingleRun, 5),
				new Mission(MissionType.NoPowerUpsWithoutScore, 140000)
			},
			new Mission[3]
			{
				new Mission(MissionType.Headstart, 5),
				new Mission(MissionType.BumpBarrierSingleRun, 8),
				new Mission(MissionType.EarnCoinWithoutTouchingGround, 200)
			},
			new Mission[3]
			{
				new Mission(MissionType.Jetpack, 12),
				new Mission(MissionType.JetpackSingleRun, 4),
				new Mission(MissionType.Headstart, 12)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpTrainSingleRun, 18),
				new Mission(MissionType.CollectCoinsWithPowerJumper, 300),
				new Mission(MissionType.ActivePowerups, 4)
			},
			new Mission[3]
			{
				new Mission(MissionType.EarnCoinSingleRun, 1000),
				new Mission(MissionType.CoinsWithMagnet, 1500),
				new Mission(MissionType.EarnCoin, 20000)
			},
			new Mission[3]
			{
				new Mission(MissionType.Jump, 400),
				new Mission(MissionType.SuperSneakers, 20),
				new Mission(MissionType.NoJumpsWithoutScore, 70000)
			},
			new Mission[3]
			{
				new Mission(MissionType.HoverBoardExpireSingleRun, 5),
				new Mission(MissionType.BuyMysterybox, 10),
				new Mission(MissionType.SpendCoin, 12000)
			},
			new Mission[3]
			{
				new Mission(MissionType.ScoreBooster, 5),
				new Mission(MissionType.RollCenter, 100),
				new Mission(MissionType.Score, 3000000)
			},
			new Mission[3]
			{
				new Mission(MissionType.StayInOneLane, 30),
				new Mission(MissionType.MagnetsSingleRun, 5),
				new Mission(MissionType.Magnets, 20)
			},
			new Mission[3]
			{
				new Mission(MissionType.DoubleMultiplierSingleRun, 6),
				new Mission(MissionType.LandOnTrainInRow, 12),
				new Mission(MissionType.BeatOwnHighscore, 1)
			},
			new Mission[3]
			{
				new Mission(MissionType.NoJumpsWithoutScore, 30000),
				new Mission(MissionType.NoRollsWithoutScore, 30000),
				new Mission(MissionType.NoPowerUpsWithoutScore, 160000)
			},
			new Mission[3]
			{
				new Mission(MissionType.MysteryBoxes, 10),
				new Mission(MissionType.PickupKeysSingleRun, 2),
				new Mission(MissionType.DailyQuests, 3)
			},
			new Mission[3]
			{
				new Mission(MissionType.RollRight, 100),
				new Mission(MissionType.PickUpPowerJumpersSingleRun, 4),
				new Mission(MissionType.ScoreWithScorebooster, 1000000)
			},
			new Mission[3]
			{
				new Mission(MissionType.LandOnTrainInRow, 12),
				new Mission(MissionType.HoverBoardExpireSingleRun, 10),
				new Mission(MissionType.Powerups, 40)
			},
			new Mission[3]
			{
				new Mission(MissionType.GetExactlyAmountOfCoins, 200),
				new Mission(MissionType.EarnCoinWithoutTouchingGround, 300),
				new Mission(MissionType.PickUpPowerJumpers, 10)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpBarrierSingleRun, 6),
				new Mission(MissionType.BuyMysterybox, 15),
				new Mission(MissionType.Letters, 15)
			},
			new Mission[3]
			{
				new Mission(MissionType.NoCoinsWithoutScore, 16000),
				new Mission(MissionType.NoRollsWithoutScore, 32000),
				new Mission(MissionType.CoinsWithMagnet, 4000)
			},
			new Mission[3]
			{
				new Mission(MissionType.JumpSingleRun, 90),
				new Mission(MissionType.DodgeBarriersSingleRun, 40),
				new Mission(MissionType.DodgeBarriers, 200)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpLightSignalSingleRun, 8),
				new Mission(MissionType.MysteryBoxes, 12),
				new Mission(MissionType.Roll, 400)
			},
			new Mission[3]
			{
				new Mission(MissionType.HoverBoard, 30),
				new Mission(MissionType.HoverBoardExpireSingleRun, 8),
				new Mission(MissionType.CoinsWithJetpack, 3000)
			},
			new Mission[3]
			{
				new Mission(MissionType.SuperSneakers, 24),
				new Mission(MissionType.SuperSneakersSingleRun, 6),
				new Mission(MissionType.DailyQuests, 4)
			}
		};

		public static Mission[][] storylineMissions = new Mission[29][]
		{
			new Mission[3]
			{
				new Mission(MissionType.EarnCoin, 500),
				new Mission(MissionType.ScoreSingleRun, 1000),
				new Mission(MissionType.Powerups, 2)
			},
			new Mission[3]
			{
				new Mission(MissionType.EarnCoinSingleRun, 200),
				new Mission(MissionType.Jump, 20),
				new Mission(MissionType.SuperSneakers, 2)
			},
			new Mission[3]
			{
				new Mission(MissionType.Tokens, 2),
				new Mission(MissionType.Roll, 30),
				new Mission(MissionType.SpendCoin, 2000)
			},
			new Mission[3]
			{
				new Mission(MissionType.DailyQuests, 1),
				new Mission(MissionType.DodgeBarriers, 20),
				new Mission(MissionType.ScoreSingleRun, 6000)
			},
			new Mission[3]
			{
				new Mission(MissionType.EarnCoin, 2500),
				new Mission(MissionType.JumpSingleRun, 30),
				new Mission(MissionType.BuyMysterybox, 1)
			},
			new Mission[3]
			{
				new Mission(MissionType.HoverBoard, 1),
				new Mission(MissionType.Magnets, 5),
				new Mission(MissionType.BeatOwnHighscore, 1)
			},
			new Mission[3]
			{
				new Mission(MissionType.Jetpack, 2),
				new Mission(MissionType.CoinsWithJetpack, 50),
				new Mission(MissionType.Headstart, 1)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpTrainSingleRun, 3),
				new Mission(MissionType.CoinsWithMagnet, 40),
				new Mission(MissionType.TimeDeath, 10)
			},
			new Mission[3]
			{
				new Mission(MissionType.HoverBoardExpire, 1),
				new Mission(MissionType.MysteryBoxes, 2),
				new Mission(MissionType.RollSingleRun, 30)
			},
			new Mission[3]
			{
				new Mission(MissionType.ScoreSingleRun, 20000),
				new Mission(MissionType.Powerups, 12),
				new Mission(MissionType.JumpTrain, 2)
			},
			new Mission[3]
			{
				new Mission(MissionType.NoCoinsWithoutScore, 4000),
				new Mission(MissionType.RollCenter, 50),
				new Mission(MissionType.EarnCoin, 5000)
			},
			new Mission[3]
			{
				new Mission(MissionType.DailyQuests, 2),
				new Mission(MissionType.DodgeBarriers, 40),
				new Mission(MissionType.SuperSneakers, 5)
			},
			new Mission[3]
			{
				new Mission(MissionType.ScoreBooster, 4),
				new Mission(MissionType.CoinsWithMagnet, 160),
				new Mission(MissionType.MagnetsSingleRun, 2)
			},
			new Mission[3]
			{
				new Mission(MissionType.PickUpPowerJumpers, 4),
				new Mission(MissionType.RollSingleRun, 40),
				new Mission(MissionType.EarnCoinSingleRun, 400)
			},
			new Mission[3]
			{
				new Mission(MissionType.Score, 100000),
				new Mission(MissionType.Jetpack, 5),
				new Mission(MissionType.BumpLightSignal, 12)
			},
			new Mission[3]
			{
				new Mission(MissionType.DailyQuests, 3),
				new Mission(MissionType.SuperSneakersSingleRun, 3),
				new Mission(MissionType.JumpTrain, 4)
			},
			new Mission[3]
			{
				new Mission(MissionType.ScoreSingleRun, 50000),
				new Mission(MissionType.SpendCoin, 4000),
				new Mission(MissionType.Magnets, 15)
			},
			new Mission[3]
			{
				new Mission(MissionType.JetpackSingleRun, 2),
				new Mission(MissionType.BumpTrainSingleRun, 6),
				new Mission(MissionType.HoverBoardExpire, 3)
			},
			new Mission[3]
			{
				new Mission(MissionType.PickupKeys, 3),
				new Mission(MissionType.MagnetsSingleRun, 3),
				new Mission(MissionType.Tokens, 5)
			},
			new Mission[3]
			{
				new Mission(MissionType.Score, 250000),
				new Mission(MissionType.JumpSingleRun, 40),
				new Mission(MissionType.PickUpPowerJumpersSingleRun, 4)
			},
			new Mission[3]
			{
				new Mission(MissionType.BumpBarrier, 15),
				new Mission(MissionType.BuyMysterybox, 3),
				new Mission(MissionType.CoinsWithMagnet, 240)
			},
			new Mission[3]
			{
				new Mission(MissionType.DodgeBarriers, 80),
				new Mission(MissionType.SpendCoin, 8000),
				new Mission(MissionType.SuperSneakersSingleRun, 4)
			},
			new Mission[3]
			{
				new Mission(MissionType.EarnCoinWithoutTouchingGround, 50),
				new Mission(MissionType.JumpTrain, 10),
				new Mission(MissionType.Jetpack, 15)
			},
			new Mission[3]
			{
				new Mission(MissionType.CollectCoinsWithPowerJumper, 500),
				new Mission(MissionType.RollCenter, 200),
				new Mission(MissionType.DailyQuests, 4)
			},
			new Mission[3]
			{
				new Mission(MissionType.EarnCoin, 15000),
				new Mission(MissionType.ScoreSingleRun, 120000),
				new Mission(MissionType.JumpTrainSingleRun, 3)
			},
			new Mission[3]
			{
				new Mission(MissionType.RollSingleRun, 50),
				new Mission(MissionType.Score, 500000),
				new Mission(MissionType.NoCoinsWithoutScore, 12000)
			},
			new Mission[3]
			{
				new Mission(MissionType.Tokens, 10),
				new Mission(MissionType.BuyMysterybox, 6),
				new Mission(MissionType.BumpLightSignal, 20)
			},
			new Mission[3]
			{
				new Mission(MissionType.JumpSingleRun, 50),
				new Mission(MissionType.HoverBoard, 12),
				new Mission(MissionType.HoverBoardExpire, 4)
			},
			new Mission[3]
			{
				new Mission(MissionType.EarnCoinSingleRun, 750),
				new Mission(MissionType.BumpBarrier, 25),
				new Mission(MissionType.ScoreSingleRun, 250000)
			}
		};

		public static readonly Dictionary<MissionType, MissionTemplate> missionTemplates = new Dictionary<MissionType, MissionTemplate>
		{
			{
				MissionType.EarnCoin,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_EARNCOIN_DESCRIPTION_SINGLE",
					description = "MISSION_EARNCOIN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_EARNCOIN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_EARNCOIN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.EarnCoin
				}
			},
			{
				MissionType.EarnCoinSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_EARNCOIN_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_EARNCOIN_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_EARNCOIN_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_EARNCOIN_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.EarnCoin,
					singleRun = true
				}
			},
			{
				MissionType.SpendCoin,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SPENDCOIN_DESCRIPTION_SINGLE",
					description = "MISSION_SPENDCOIN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SPENDCOIN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SPENDCOIN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.SpendCoin
				}
			},
			{
				MissionType.Score,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SCORE_DESCRIPTION_SINGLE",
					description = "MISSION_SCORE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SCORE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SCORE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Score
				}
			},
			{
				MissionType.JumpTrain,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JUMPTRAIN_DESCRIPTION_SINGLE",
					description = "MISSION_JUMPTRAIN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JUMPTRAIN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JUMPTRAIN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.JumpTrain
				}
			},
			{
				MissionType.JumpTrainSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JUMPTRAIN_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_JUMPTRAIN_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JUMPTRAIN_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JUMPTRAIN_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.JumpTrain,
					singleRun = true
				}
			},
			{
				MissionType.Jump,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JUMP_DESCRIPTION_SINGLE",
					description = "MISSION_JUMP_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JUMP_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JUMP_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Jump
				}
			},
			{
				MissionType.JumpSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JUMP_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_JUMP_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JUMP_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JUMP_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Jump,
					singleRun = true
				}
			},
			{
				MissionType.Roll,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ROLL_DESCRIPTION_SINGLE",
					description = "MISSION_ROLL_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ROLL_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ROLL_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Roll
				}
			},
			{
				MissionType.RollSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ROLL_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_ROLL_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ROLL_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ROLL_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Roll,
					singleRun = true
				}
			},
			{
				MissionType.RollLeft,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ROLL_LEFT_DESCRIPTION_SINGLE",
					description = "MISSION_ROLL_LEFT_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ROLL_LEFT_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ROLL_LEFT_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.RollLeft
				}
			},
			{
				MissionType.RollCenter,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ROLL_CENTER_DESCRIPTION_SINGLE",
					description = "MISSION_ROLL_CENTER_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ROLL_CENTER_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ROLL_CENTER_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.RollCenter
				}
			},
			{
				MissionType.RollRight,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ROLL_RIGHT_DESCRIPTION_SINGLE",
					description = "MISSION_ROLL_RIGHT_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ROLL_RIGHT_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ROLL_RIGHT_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.RollRight
				}
			},
			{
				MissionType.RollUnderBarriers,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ROLL_UNDERBARRIERS_DESCRIPTION_SINGLE",
					description = "MISSION_ROLL_UNDERBARRIERS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ROLL_UNDERBARRIERS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ROLL_UNDERBARRIERS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.RollUnderBarriers
				}
			},
			{
				MissionType.JumpBarriers,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JUMP_BARRIERS_DESCRIPTION_SINGLE",
					description = "MISSION_JUMP_BARRIERS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JUMP_BARRIERS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JUMP_BARRIERS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.JumpBarriers
				}
			},
			{
				MissionType.DieToTrain,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_DIETOTRAIN_DESCRIPTION_SINGLE",
					description = "MISSION_DIETOTRAIN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_DIETOTRAIN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_DIETOTRAIN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.DieToTrain
				}
			},
			{
				MissionType.Jetpack,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JETPACK_DESCRIPTION_SINGLE",
					description = "MISSION_JETPACK_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JETPACK_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JETPACK_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Jetpack
				}
			},
			{
				MissionType.JetpackSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JETPACK_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_JETPACK_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JETPACK_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JETPACK_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Jetpack,
					singleRun = true
				}
			},
			{
				MissionType.SuperSneakers,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SUPERSNEAKERS_DESCRIPTION_SINGLE",
					description = "MISSION_SUPERSNEAKERS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SUPERSNEAKERS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SUPERSNEAKERS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.SuperSneakers
				}
			},
			{
				MissionType.SuperSneakersSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SUPERSNEAKERS_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_SUPERSNEAKERS_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SUPERSNEAKERS_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SUPERSNEAKERS_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.SuperSneakers,
					singleRun = true
				}
			},
			{
				MissionType.Letters,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_LETTERS_DESCRIPTION_SINGLE",
					description = "MISSION_LETTERS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_LETTERS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_LETTERS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Letters
				}
			},
			{
				MissionType.Magnets,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_MGANETS_DESCRIPTION_SINGLE",
					description = "MISSION_MAGNETS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_MAGNETS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_MAGNETS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Magnets
				}
			},
			{
				MissionType.MagnetsSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_MGANETS_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_MAGNETS_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_MAGNETS_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_MAGNETS_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Magnets,
					singleRun = true
				}
			},
			{
				MissionType.BeatFriends,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BEATFRIENDS_DESCRIPTION_SINGLE",
					description = "MISSION_BEATFRIENDS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BEATFRIENDS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BEATFRIENDS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BeatFriends
				}
			},
			{
				MissionType.BeatFriendsSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BEATFRIENDS_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_BEATFRIENDS_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BEATFRIENDS_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BEATFRIENDS_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BeatFriends,
					singleRun = true
				}
			},
			{
				MissionType.DailyQuests,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_DAILYQUESTS_DESCRIPTION_SINGLE",
					description = "MISSION_DAILYQUESTS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_DAILYQUESTS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_DAILYQUESTS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.DailyQuests
				}
			},
			{
				MissionType.Tokens,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_TOKENS_DESCRIPTION_SINGLE",
					description = "MISSION_TOKENS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_TOKENS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_TOKENS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Tokens
				}
			},
			{
				MissionType.DodgeBarriers,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_DODGE_BARRIERS_DESCRIPTION_SINGLE",
					description = "MISSION_DODGE_BARRIERS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_DODGE_BARRIERS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_DODGE_BARRIERS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.DodgeBarriers
				}
			},
			{
				MissionType.CrashBarriers,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_CRASH_BARRIERS_DESCRIPTION_SINGLE",
					description = "MISSION_CRASH_BARRIERS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_CRASH_BARRIERS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_CRASH_BARRIERS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CrashBarriers
				}
			},
			{
				MissionType.CrashBarriersSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_CRASH_BARRIERS_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_CRASH_BARRIERS_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_CRASH_BARRIERS_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_CRASH_BARRIERS_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CrashBarriers,
					singleRun = true
				}
			},
			{
				MissionType.CrashTrains,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_CRASH_TRAINS_DESCRIPTION_SINGLE",
					description = "MISSION_CRASH_TRAINS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_CRASH_TRAINS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_CRASH_TRAINS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CrashTrains
				}
			},
			{
				MissionType.BumpTrainSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_CRASH_TRAINS_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_CRASH_TRAINS_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_CRASH_TRAINS_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_CRASH_TRAINS_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BumpTrain,
					singleRun = true
				}
			},
			{
				MissionType.Powerups,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_POWERUPS_DESCRIPTION_SINGLE",
					description = "MISSION_POWERUPS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_POWERUPS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_POWERUPS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Powerups
				}
			},
			{
				MissionType.Headstart,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_HEADSTART_DESCRIPTION_SINGLE",
					description = "MISSION_HEADSTART_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_HEADSTART_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_HEADSTART_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Headstart
				}
			},
			{
				MissionType.CoinsWithMagnet,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_COINS_WITH_MAGNET_DESCRIPTION_SINGLE",
					description = "MISSION_COINS_WITH_MAGNET_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_COINS_WITH_MAGNET_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_COINS_WITH_MAGNET_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CoinsWithMagnet
				}
			},
			{
				MissionType.BuyMysterybox,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BUY_MISTERYBOX_DESCRIPTION_SINGLE",
					description = "MISSION_BUY_MISTERYBOX_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BUY_MISTERYBOX_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BUY_MISTERYBOX_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BuyMysterybox
				}
			},
			{
				MissionType.CollectCoinPouch,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_COLLECTCOIN_POUCH_DESCRIPTION_SINGLE",
					description = "MISSION_COLLECTCOIN_POUCH_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_COLLECTCOIN_POUCH_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_COLLECTCOIN_POUCH_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CollectCoinPouch
				}
			},
			{
				MissionType.TimeDeath,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_TIMEDEATH_DESCRIPTION_SINGLE",
					description = "MISSION_TIMEDEATH_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_TIMEDEATH_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_TIMEDEATH_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.TimeDeath,
					singleRun = true,
					completeIfLess = true
				}
			},
			{
				MissionType.MysteryBoxes,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_MYSTERYBOXES_DESCRIPTION_SINGLE",
					description = "MISSION_MYSTERYBOXES_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_MYSTERYBOXES_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_MYSTERYBOXES_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.MysteryBoxes
				}
			},
			{
				MissionType.BumpBarrier,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BUMP_BARRIER_DESCRIPTION_SINGLE",
					description = "MISSION_BUMP_BARRIER_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BUMP_BARRIER_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BUMP_BARRIER_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BumpBarrier
				}
			},
			{
				MissionType.BumpLightSignal,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BUMP_LIGHTSIGNAL_DESCRIPTION_SINGLE",
					description = "MISSION_BUMP_LIGHTSIGNAL_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BUMP_LIGHTSIGNAL_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BUMP_LIGHTSIGNAL_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BumpLightSignal
				}
			},
			{
				MissionType.BumpBush,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BUMP_BUSH_DESCRIPTION_SINGLE",
					description = "MISSION_BUMP_BUSH_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BUMP_BUSH_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BUMP_BUSH_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BumpBush
				}
			},
			{
				MissionType.BumpTrain,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BUMP_TRAIN_DESCRIPTION_SINGLE",
					description = "MISSION_BUMP_TRAIN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BUMP_TRAIN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BUMP_TRAIN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BumpTrain
				}
			},
			{
				MissionType.HoverBoard,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_HOVERBOARD_DESCRIPTION_SINGLE",
					description = "MISSION_HOVERBOARD_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_HOVERBOARD_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_HOVERBOARD_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.HoverBoard
				}
			},
			{
				MissionType.HoverBoardExpire,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_HOVERBOARD_EXPIRE_DESCRIPTION_SINGLE",
					description = "MISSION_HOVERBOARD_EXPIRE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_HOVERBOARD_EXPIRE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_HOVERBOARD_EXPIRE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.HoverBoardExpire
				}
			},
			{
				MissionType.ScoreSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SCORE_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_SCORE_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SCORE_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SCORE_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.Score,
					singleRun = true
				}
			},
			{
				MissionType.NoCoinsWithoutScore,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_NOCOINS_WITHOUTSCORE_DESCRIPTION_SINGLE",
					description = "MISSION_NOCOINS_WITHOUTSCORE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_NOCOINS_WITHOUTSCORE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_NOCOINS_WITHOUTSCORE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.NoCoinsWithoutScore,
					singleRun = true,
					resetDuringRun = true
				}
			},
			{
				MissionType.DoubleMultiplierSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_DOUBLE_MULTIPLIER_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_DOUBLE_MULTIPLIER_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_DOUBLE_MULTIPLIER_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_DOUBLE_MULTIPLIER_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.DoubleMultiplier,
					singleRun = true
				}
			},
			{
				MissionType.ReachMissionSet,
				new MissionTemplate
				{
					missionTarget = MissionTarget.ReachMissionSet
				}
			},
			{
				MissionType.GuardJump,
				new MissionTemplate
				{
					missionTarget = MissionTarget.GuardJump
				}
			},
			{
				MissionType.HaveUpgrades,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveUpgrades
				}
			},
			{
				MissionType.HaveHeadStartLarge,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveHeadStartLarge
				}
			},
			{
				MissionType.HaveKing,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveKing
				}
			},
			{
				MissionType.HaveLucy,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveLucy
				}
			},
			{
				MissionType.HaveNinja,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveNinja
				}
			},
			{
				MissionType.HaveFrank,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveFrank
				}
			},
			{
				MissionType.HaveFrizzy,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveFrizzy
				}
			},
			{
				MissionType.HaveTag,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveTag
				}
			},
			{
				MissionType.HaveTasha,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveTasha
				}
			},
			{
				MissionType.HaveZoe,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveZoe
				}
			},
			{
				MissionType.HaveBrody,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveBrody
				}
			},
			{
				MissionType.HavePrinceK,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HavePrinceK
				}
			},
			{
				MissionType.HaveYutani,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveYutani
				}
			},
			{
				MissionType.DodgeBarriersSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_DODGEBARRIERS_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_DODGEBARRIERS_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_DODGEBARRIERS_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_DODGEBARRIERS_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.DodgeBarriers,
					singleRun = true
				}
			},
			{
				MissionType.HoverBoardExpireSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_HOVERBOARD_EXPIRE_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_HOVERBOARD_EXPIRE_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_HOVERBOARD_EXPIRE_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_HOVERBOARD_EXPIRE_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.HoverBoardExpire,
					singleRun = true
				}
			},
			{
				MissionType.BumpBarrierSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BUMPBARRIER_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_BUMPBARRIER_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BUMPBARRIER_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BUMPBARRIER_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BumpBarrier,
					singleRun = true
				}
			},
			{
				MissionType.BumpLightSignalSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BUMP_LIGHTSIGNAL_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_BUMP_LIGHTSIGNAL_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BUMP_LIGHTSIGNAL_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BUMP_LIGHTSIGNAL_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BumpLightSignal,
					singleRun = true
				}
			},
			{
				MissionType.FacebookLoggedIn,
				new MissionTemplate
				{
					missionTarget = MissionTarget.FacebookLoggedIn
				}
			},
			{
				MissionType.MysteryBoxGrandPrize,
				new MissionTemplate
				{
					missionTarget = MissionTarget.MysteryBoxGrandPrize
				}
			},
			{
				MissionType.MissionSetSingleRun,
				new MissionTemplate
				{
					missionTarget = MissionTarget.MissionSet,
					singleRun = true
				}
			},
			{
				MissionType.LandOnTrainInRow,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_JUMPONTRAIN_INROW_DESCRIPTION_SINGLE",
					description = "MISSION_JUMPONTRAIN_INROW_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_JUMPONTRAIN_INROW_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_JUMPONTRAIN_INROW_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.LandOnTrainInRow
				}
			},
			{
				MissionType.StayInOneLane,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_STAYIN_ONELINE_DESCRIPTION_SINGLE",
					description = "MISSION_STAYIN_ONELINE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_STAYIN_ONELINE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_STAYIN_ONELINE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.StayInOneLane,
					singleRun = true,
					resetDuringRun = true
				}
			},
			{
				MissionType.DailyQuestInRow,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_DAILYQUEST_INROW_DESCRIPTION_SINGLE",
					description = "MISSION_DAILYQUEST_INROW_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_DAILYQUEST_INROW_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_DAILYQUEST_INROW_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.DailyQuestInRow
				}
			},
			{
				MissionType.NoJumpsWithoutScore,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_NOJUMPS_WITHOUT_SCORE_DESCRIPTION_SINGLE",
					description = "MISSION_NOJUMPS_WITHOUT_SCORE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_NOJUMPS_WITHOUT_SCORE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_NOJUMPS_WITHOUT_SCORE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.NoJumpsWithoutScore,
					singleRun = true,
					resetDuringRun = true
				}
			},
			{
				MissionType.NoRollsWithoutScore,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_NOROLLS_WITHOUT_SCORE_DESCRIPTION_SINGLE",
					description = "MISSION_NOROLLS_WITHOUT_SCORE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_NOROLLS_WITHOUT_SCORE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_NOROLLS_WITHOUT_SCORE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.NoRollsWithoutScore,
					singleRun = true,
					resetDuringRun = true
				}
			},
			{
				MissionType.NoPowerUpsWithoutScore,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_NOPOWERUPS_WITHOUT_SCORE_DESCRIPTION_SINGLE",
					description = "MISSION_NOPOWERUPS_WITHOUT_SCORE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_NOPOWERUPS_WITHOUT_SCORE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_NOPOWERUPS_WITHOUT_SCORE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.NoPowerUpsWithoutScore,
					singleRun = true,
					resetDuringRun = true
				}
			},
			{
				MissionType.OneOfEachPowerup,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ONE_OF_EACH_POWERUP_DESCRIPTION_SINGLE",
					description = "MISSION_ONE_OF_EACH_POWERUP_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ONE_OF_EACH_POWERUP_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ONE_OF_EACH_POWERUP_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.OneOfEachPowerup,
					singleRun = true
				}
			},
			{
				MissionType.HaveTrophiesFirstAchievements,
				new MissionTemplate
				{
					missionTarget = MissionTarget.HaveTrophiesFirstAchievements
				}
			},
			{
				MissionType.none,
				new MissionTemplate
				{
					missionTarget = MissionTarget.none
				}
			},
			{
				MissionType.ScoreBooster,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SCOORE_BOOSTER_DESCRIPTION_SINGLE",
					description = "MISSION_SCOORE_BOOSTER_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SCOORE_BOOSTER_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SCOORE_BOOSTER_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.ScoreBooster
				}
			},
			{
				MissionType.BeatOwnHighscore,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_BEATOWN_HIGHSCORE_DESCRIPTION_SINGLE",
					description = "MISSION_BEATOWN_HIGHSCORE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_BEATOWN_HIGHSCORE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_BEATOWN_HIGHSCORE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.BeatOwnHighscore
				}
			},
			{
				MissionType.PokeFriend,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_POKE_ONE_FRIEND_DESCRIPTION_SINGLE",
					description = "MISSION_POKE_ONE_FRIEND_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_POKE_ONE_FRIEND_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_POKE_ONE_FRIEND_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.PokeFriend
				}
			},
			{
				MissionType.GetExactlyAmountOfCoins,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_GET_AMOUNT_OF_COINS_DESCRIPTION_SINGLE",
					description = "MISSION_GET_AMOUNT_OF_COINS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_GET_AMOUNT_OF_COINS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_GET_AMOUNT_OF_COINS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.GetExactlyAmountOfCoins,
					completeIfEqual = true
				}
			},
			{
				MissionType.ActivePowerups,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_ACTIVE_POWERUPS_DESCRIPTION_SINGLE",
					description = "MISSION_ACTIVE_POWERUPS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_ACTIVE_POWERUPS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_ACTIVE_POWERUPS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.ActivePowerups
				}
			},
			{
				MissionType.CoinsWithJetpack,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_COINSWITH_JETPACK_DESCRIPTION_SINGLE",
					description = "MISSION_COINSWITH_JETPACK_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_COINSWITH_JETPACK_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_COINSWITH_JETPACK_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CoinsWithJetpack
				}
			},
			{
				MissionType.ScoreWithScorebooster,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SCOOREWITH_SCORE_BOOSTER_DESCRIPTION_SINGLE",
					description = "MISSION_SCOOREWITH_SCORE_BOOSTER_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SCOOREWITH_SCORE_BOOSTER_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SCOOREWITH_SCORE_BOOSTER_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.ScoreWithScorebooster
				}
			},
			{
				MissionType.EarnCoinWithoutTouchingGround,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_EARNCOIN_WITHOUT_TOUCHING_GROUND_DESCRIPTION_SINGLE",
					description = "MISSION_EARNCOIN_WITHOUT_TOUCHING_GROUND_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_EARNCOIN_WITHOUT_TOUCHING_GROUND_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_EARNCOIN_WITHOUT_TOUCHING_GROUND_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.EarnCoinWithoutTouchingGround,
					singleRun = true,
					resetDuringRun = true
				}
			},
			{
				MissionType.SkipMission,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SKIP_MISSION_DESCRIPTION_SINGLE",
					description = "MISSION_SKIP_MISSION_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SKIP_MISSION_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SKIP_MISSION_DESCRIPTION",
					missionTarget = MissionTarget.SkipMission
				}
			},
			{
				MissionType.PickupKeysSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_PICKUP_KEYS_SINGLERUN_DESCRIPTION_SINGLE",
					description = "MISSION_PICKUP_KEYS_SINGLERUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_PICKUP_KEYS_SINGLERUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_PICKUP_KEYS_SINGLERUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.PickupKeys,
					singleRun = true
				}
			},
			{
				MissionType.PickupKeys,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_PICKUP_KEYS_DESCRIPTION_SINGLE",
					description = "MISSION_PICKUP_KEYS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_PICKUP_KEYS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_PICKUP_KEYS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.PickupKeys
				}
			},
			{
				MissionType.SpendKeys,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_SPEND_KEYS_DESCRIPTION_SINGLE",
					description = "MISSION_SPEND_KEYS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_SPEND_KEYS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_SPEND_KEYS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.SpendKeys
				}
			},
			{
				MissionType.CollectCoinsLeftLane,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_COLLECT_COINS_LEFTLANE_DESCRIPTION_SINGLE",
					description = "MISSION_COLLECT_COINS_LEFTLANE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_COLLECT_COINS_LEFTLANE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_COLLECT_COINS_LEFTLANE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CollectCoinsLeftLane
				}
			},
			{
				MissionType.CollectCoinsCenterLane,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_COLLECT_COINS_CENTERLANE_DESCRIPTION_SINGLE",
					description = "MISSION_COLLECT_COINS_CENTERLANE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_COLLECT_COINS_CENTERLANE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_COLLECT_COINS_CENTERLANE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CollectCoinsCenterLane
				}
			},
			{
				MissionType.CollectCoinsRightLane,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_COLLECT_COINS_RIGHTLANE_DESCRIPTION_SINGLE",
					description = "MISSION_COLLECT_COINS_RIGHTLANE_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_COLLECT_COINS_RIGHTLANE_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_COLLECT_COINS_RIGHTLANE_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CollectCoinsRightLane
				}
			},
			{
				MissionType.PickUpPowerJumpers,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_PICKUP_POWER_JUMPERS_DESCRIPTION_SINGLE",
					description = "MISSION_PICKUP_POWER_JUMPERS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_PICKUP_POWER_JUMPERS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_PICKUP_POWER_JUMPERS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.PickUpPowerJumpers
				}
			},
			{
				MissionType.PickUpPowerJumpersSingleRun,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_PICKUP_POWER_JUMPERS_SINGLE_RUN_DESCRIPTION_SINGLE",
					description = "MISSION_PICKUP_POWER_JUMPERS_SINGLE_RUN_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_PICKUP_POWER_JUMPERS_SINGLE_RUN_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_PICKUP_POWER_JUMPERS_SINGLE_RUN_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.PickUpPowerJumpersSingleRun,
					singleRun = true
				}
			},
			{
				MissionType.CollectCoinsWithPowerJumper,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_COLLECT_COINS_WITH_POWER_JUMPER_DESCRIPTION_SINGLE",
					description = "MISSION_COLLECT_COINS_WITH_POWER_JUMPER_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_COLLECT_COINS_WITH_POWER_JUMPER_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_COLLECT_COINS_WITH_POWER_JUMPER_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.CollectCoinsWithPowerJumper
				}
			},
			{
				MissionType.WinDuels,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_WIN_DUELS_DESCRIPTION_SINGLE",
					description = "MISSION_WIN_DUELS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_WIN_DUELS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_WIN_DUELS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.WinDuels
				}
			},
			{
				MissionType.DuelFriends,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_DUEL_FRIENDS_DESCRIPTION_SINGLE",
					description = "MISSION_DUEL_FRIENDS_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_DUEL_FRIENDS_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_DUEL_FRIENDS_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.DuelFriends
				}
			},
			{
				MissionType.WinDuelsInARow,
				new MissionTemplate
				{
					descriptionSingle = "MISSION_WIN_DUELS_IN_A_ROW_DESCRIPTION_SINGLE",
					description = "MISSION_WIN_DUELS_IN_A_ROW_DESCRIPTION",
					ultraShortDescriptionSingle = "MISSION_WIN_DUELS_IN_A_ROW_ULTRASHORT_DESCRIPTION_SINGLE",
					ultraShortDescription = "MISSION_WIN_DUELS_IN_A_ROW_ULTRASHORT_DESCRIPTION",
					missionTarget = MissionTarget.WinDuelsInARow
				}
			}
		};

		public static MissionSkip[] skipData = new MissionSkip[3]
		{
			new MissionSkip("UPGRADES_SKIP_MISSION_1", 1500, 100),
			new MissionSkip("UPGRADES_SKIP_MISSION_2", 1500, 100),
			new MissionSkip("UPGRADES_SKIP_MISSION_3", 1500, 100)
		};
	}
}
