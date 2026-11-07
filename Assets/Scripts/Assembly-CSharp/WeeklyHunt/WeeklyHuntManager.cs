using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Info;
using Kiloo;
using SYBO.Subway;
using UI.Screens.RewardManagement;
using UI.SlideIns;

namespace WeeklyHunt
{
	public static class WeeklyHuntManager
	{
		private static bool _hasInited;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action itemCountUpdated__BackingField;

		public static bool canCollectAllTokensInOneRun;

		private static Dictionary<int, WeeklyHuntData.WeeklyHuntPeriodData> huntPeriodData
		{
			get
			{
				return WeeklyHuntData.GetHuntData();
			}
		}

		public static event Action itemCountUpdated
		{
			add
			{
				Action action = itemCountUpdated__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref itemCountUpdated__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = itemCountUpdated__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref itemCountUpdated__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static bool IsAnyPeriodActive()
		{
			InitIfNeeded();
			WeeklyHuntData.PrizePeriodId activePeriod = WeeklyHuntData.GetActivePeriod();
			if (activePeriod != WeeklyHuntData.PrizePeriodId.None)
			{
				WeeklyHuntData.WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[(int)activePeriod];
				if (weeklyHuntPeriodData.itemsCollected < weeklyHuntPeriodData.itemsToCollect)
				{
					if (weeklyHuntPeriodData.itemsCollected == 0)
					{
						canCollectAllTokensInOneRun = true;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		public static bool IsHuntStillActive()
		{
			InitIfNeeded();
			WeeklyHuntData.WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[1];
			WeeklyHuntData.WeeklyHuntPeriodData weeklyHuntPeriodData2 = ((!WeeklyHuntData.hideLastPeriod) ? huntPeriodData[4] : huntPeriodData[3]);
			if (DateTime.Compare(weeklyHuntPeriodData2.periodEndDate, CDateTime.UtcNow) >= 0 && DateTime.Compare(weeklyHuntPeriodData.periodStartDate, CDateTime.UtcNow) <= 0)
			{
				return true;
			}
			return false;
		}

		public static WeeklyHuntData.PeriodStatus GetPeriodStatus(WeeklyHuntData.PrizePeriodId period)
		{
			InitIfNeeded();
			WeeklyHuntData.WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[(int)period];
			DateTime periodEndDate = weeklyHuntPeriodData.periodEndDate;
			DateTime periodStartDate = weeklyHuntPeriodData.periodStartDate;
			if (DateTime.Compare(periodEndDate, CDateTime.UtcNow) >= 0 && DateTime.Compare(periodStartDate, CDateTime.UtcNow) <= 0)
			{
				return WeeklyHuntData.PeriodStatus.Current;
			}
			if (DateTime.Compare(periodEndDate, CDateTime.UtcNow) < 0)
			{
				return WeeklyHuntData.PeriodStatus.Expired;
			}
			return WeeklyHuntData.PeriodStatus.Unstarted;
		}

		public static WeeklyHuntData.WeeklyHuntPeriodData GetPrizeDataForPeriod(WeeklyHuntData.PrizePeriodId period)
		{
			InitIfNeeded();
			if (period == WeeklyHuntData.PrizePeriodId.None)
			{
				if (WeeklyHuntData.hideLastPeriod)
				{
					return huntPeriodData[3];
				}
				return huntPeriodData[4];
			}
			return huntPeriodData[(int)period];
		}

		public static bool CanSpawnToken()
		{
			bool result = false;
			if (IsAnyPeriodActive())
			{
				WeeklyHuntData.WeeklyHuntPeriodData prizeDataForPeriod = GetPrizeDataForPeriod(WeeklyHuntData.GetActivePeriod());
				result = prizeDataForPeriod.itemsCollected < prizeDataForPeriod.itemsToCollect;
			}
			return result;
		}

		public static bool GetHiddenUntilUnlockedForPeriod(WeeklyHuntData.PrizePeriodId period)
		{
			InitIfNeeded();
			if (period == WeeklyHuntData.PrizePeriodId.None)
			{
				return false;
			}
			return huntPeriodData[(int)period].hiddenUntilUnlocked;
		}

		private static List<CelebrationReward> GetRewardsForPeriod(WeeklyHuntData.PrizePeriodId period)
		{
			List<CelebrationReward> list = new List<CelebrationReward>();
			CelebrationReward celebrationReward = new CelebrationReward();
			if (huntPeriodData.ContainsKey((int)period))
			{
				WeeklyHuntData.PrizeData prizeForPeriod = GetPrizeForPeriod(period);
				if (prizeForPeriod.celebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeNormal)
				{
					celebrationReward.CelebrationRewardOrigin = prizeForPeriod.celebrationRewardOrigin;
					celebrationReward.rewardType = prizeForPeriod.rewardType;
					celebrationReward.amount = prizeForPeriod.amount;
					if (prizeForPeriod.rewardType == CelebrationRewardType.powerup)
					{
						celebrationReward.upgradeType = prizeForPeriod.upgradeType;
					}
					else if (prizeForPeriod.rewardType == CelebrationRewardType.character)
					{
						celebrationReward.characterId = prizeForPeriod.characterId;
					}
					else if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard)
					{
						celebrationReward.boardId = prizeForPeriod.boardId;
						BoardManager.UnlockBoard(celebrationReward.boardId);
					}
					else if (prizeForPeriod.rewardType == CelebrationRewardType.token)
					{
						celebrationReward.characterId = prizeForPeriod.characterId;
					}
					list.Add(celebrationReward);
				}
				else if (prizeForPeriod.celebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || prizeForPeriod.celebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox)
				{
					for (int i = 0; i < prizeForPeriod.amount; i++)
					{
						celebrationReward = MysteryBox.Roll(MysteryBox.Type.Super);
						celebrationReward.CelebrationRewardOrigin = prizeForPeriod.celebrationRewardOrigin;
						list.Add(celebrationReward);
					}
				}
				else if (prizeForPeriod.celebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || prizeForPeriod.celebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox)
				{
					for (int j = 0; j < prizeForPeriod.amount; j++)
					{
						celebrationReward = MysteryBox.Roll(MysteryBox.Type.Normal);
						celebrationReward.CelebrationRewardOrigin = prizeForPeriod.celebrationRewardOrigin;
						list.Add(celebrationReward);
					}
				}
				else if (prizeForPeriod.celebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox)
				{
					for (int k = 0; k < prizeForPeriod.amount; k++)
					{
						celebrationReward = MysteryBox.Roll(MysteryBox.Type.Token);
						celebrationReward.CelebrationRewardOrigin = prizeForPeriod.celebrationRewardOrigin;
						list.Add(celebrationReward);
					}
				}
				else if (prizeForPeriod.celebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeHiddenPrize)
				{
					celebrationReward.CelebrationRewardOrigin = prizeForPeriod.celebrationRewardOrigin;
					celebrationReward.rewardType = prizeForPeriod.rewardType;
					celebrationReward.amount = prizeForPeriod.amount;
					if (prizeForPeriod.rewardType == CelebrationRewardType.powerup)
					{
						celebrationReward.upgradeType = prizeForPeriod.upgradeType;
					}
					else if (prizeForPeriod.rewardType == CelebrationRewardType.character)
					{
						celebrationReward.characterId = prizeForPeriod.characterId;
					}
					else if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard)
					{
						celebrationReward.boardId = prizeForPeriod.boardId;
						BoardManager.UnlockBoard(celebrationReward.boardId);
					}
					else if (prizeForPeriod.rewardType == CelebrationRewardType.token)
					{
						celebrationReward.characterId = prizeForPeriod.characterId;
					}
					list.Add(celebrationReward);
				}
				else
				{
					celebrationReward = MysteryBox.Roll(MysteryBox.Type.Normal);
					celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.WeeklyChallengeNormal;
					list.Add(celebrationReward);
				}
			}
			foreach (CelebrationReward item in list)
			{
				item.syboAnalyticsSource = ObtainableItemSource.WeeklyHunt;
				item.syboAnalyticsSourceId = -1L;
				item.syboAnalyticsSourceMetaOrNull = period.ToString().ToLowerInvariant();
			}
			return list;
		}

		public static void ItemPickedUp()
		{
			InitIfNeeded();
			WeeklyHuntData.PrizePeriodId activePeriod = WeeklyHuntData.GetActivePeriod();
			WeeklyHuntData.WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[(int)activePeriod];
			if (weeklyHuntPeriodData.itemsCollected < weeklyHuntPeriodData.itemsToCollect)
			{
				weeklyHuntPeriodData.itemsCollected++;
				huntPeriodData[(int)activePeriod] = weeklyHuntPeriodData;
				WeeklyHuntData.SaveNewProgress(false);
				if (weeklyHuntPeriodData.itemsCollected == weeklyHuntPeriodData.itemsToCollect)
				{
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.WeeklyHunt
					});
					List<CelebrationReward> rewardsForPeriod = GetRewardsForPeriod(activePeriod);
					foreach (CelebrationReward item in rewardsForPeriod)
					{
						RewardManager.AddRewardToUnlock(item);
						if (item.rewardType == CelebrationRewardType.specialBoard)
						{
							BoardManager.UnlockBoard(item.boardId);
						}
					}
					string text = WeeklyHuntData.StartChallengeDateTime.Day + "-" + WeeklyHuntData.StartChallengeDateTime.Month + "-" + WeeklyHuntData.StartChallengeDateTime.Year;
					text += string.Format(":{0}", (int)activePeriod);
					Flurry.LogEventWithAParameter("Weekly Challenge Completed", "Id", text);
					OnWeekCompleted();
				}
			}
			if (itemCountUpdated__BackingField != null)
			{
				itemCountUpdated__BackingField();
			}
		}

		private static void InitIfNeeded()
		{
			if (!_hasInited)
			{
				_hasInited = true;
				Game.OnGameStarted += OnRunStarted;
				Game.OnMenuScreenShown += OnRunEnded;
			}
		}

		private static void OnRunStarted()
		{
			WeeklyHuntData.PrizePeriodId activePeriod = WeeklyHuntData.GetActivePeriod();
			if (activePeriod != WeeklyHuntData.PrizePeriodId.None)
			{
				WeeklyHuntData.WeeklyHuntPeriodData weeklyHuntPeriodData = huntPeriodData[(int)activePeriod];
				if (weeklyHuntPeriodData.itemsCollected == 0)
				{
					canCollectAllTokensInOneRun = true;
				}
			}
		}

		private static void OnRunEnded()
		{
			canCollectAllTokensInOneRun = false;
		}

		private static void OnWeekCompleted()
		{
			if (canCollectAllTokensInOneRun)
			{
				Stat.HuntsCompletedInOneRun.IncrementValue();
			}
		}

		public static bool IsBoardAwardedInCurrentPeriod(Boards.BoardId boardId)
		{
			WeeklyHuntData.PrizePeriodId activePeriod = WeeklyHuntData.GetActivePeriod();
			if (activePeriod != WeeklyHuntData.PrizePeriodId.None)
			{
				WeeklyHuntData.PrizeData prizeForPeriod = GetPrizeForPeriod(activePeriod);
				return prizeForPeriod.rewardType == CelebrationRewardType.specialBoard && prizeForPeriod.boardId == boardId;
			}
			return false;
		}

		public static int ItemsCollectedForBoardType(Boards.BoardId boardId)
		{
			foreach (int key in huntPeriodData.Keys)
			{
				WeeklyHuntData.PrizePeriodId prizePeriodId = (WeeklyHuntData.PrizePeriodId)key;
				WeeklyHuntData.PrizeData prizeForPeriod = GetPrizeForPeriod(prizePeriodId);
				if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard && prizeForPeriod.boardId == boardId)
				{
					return huntPeriodData[(int)prizePeriodId].itemsCollected;
				}
			}
			return 0;
		}

		public static int ItemsToCollectForBoardType(Boards.BoardId boardId)
		{
			foreach (int key in huntPeriodData.Keys)
			{
				WeeklyHuntData.PrizeData prizeForPeriod = GetPrizeForPeriod((WeeklyHuntData.PrizePeriodId)key);
				if (prizeForPeriod.rewardType == CelebrationRewardType.specialBoard && prizeForPeriod.boardId == boardId)
				{
					return huntPeriodData[key].itemsToCollect;
				}
			}
			return 0;
		}

		public static WeeklyHuntData.PrizeData GetPrizeForPeriod(WeeklyHuntData.PrizePeriodId periodId)
		{
			if (WeeklyHuntData.IsAlternativeHuntPrizeActiveForPeriod(periodId))
			{
				return huntPeriodData[(int)periodId].alternativePrize;
			}
			return huntPeriodData[(int)periodId].mainPrize;
		}

		public static WeeklyHuntData.WeeklyHuntPrize DeterminePrize(WeeklyHuntData.PrizeData prizeData)
		{
			switch (prizeData.celebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
				return WeeklyHuntData.WeeklyHuntPrize.SuperMysteryBox;
			case CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox:
				return WeeklyHuntData.WeeklyHuntPrize.TokenMysteryBox;
			default:
				switch (prizeData.rewardType)
				{
				case CelebrationRewardType.keys:
					return WeeklyHuntData.WeeklyHuntPrize.Key;
				case CelebrationRewardType.coins:
					return WeeklyHuntData.WeeklyHuntPrize.Coin;
				case CelebrationRewardType.specialBoard:
					switch (prizeData.boardId)
					{
					case Boards.BoardId.londonBoard01:
						return WeeklyHuntData.WeeklyHuntPrize.ChristmasBoard;
					case Boards.BoardId.rioBoard02:
						return WeeklyHuntData.WeeklyHuntPrize.RioBoard2;
					case Boards.BoardId.londonBoard03:
						return WeeklyHuntData.WeeklyHuntPrize.NorthpoleBoard2016;
					case Boards.BoardId.cherry:
						return WeeklyHuntData.WeeklyHuntPrize.CherryBoard;
					case Boards.BoardId.chick:
						return WeeklyHuntData.WeeklyHuntPrize.ChickyBoard;
					case Boards.BoardId.washingtonBoard01:
						return WeeklyHuntData.WeeklyHuntPrize.EagleBoard;
					}
					SDebug.LogError("Unknown BoardType");
					break;
				}
				switch (prizeData.upgradeType)
				{
				case UpgradeType.hoverboard:
					return WeeklyHuntData.WeeklyHuntPrize.Hoverboard;
				case UpgradeType.headstart2000:
					return WeeklyHuntData.WeeklyHuntPrize.Headstart;
				case UpgradeType.scorebooster:
					return WeeklyHuntData.WeeklyHuntPrize.ScoreBooster;
				default:
					return WeeklyHuntData.WeeklyHuntPrize.NotAvailable;
				}
			}
		}

		public static string GetPrizeTitle(WeeklyHuntData.PrizeData prizeData)
		{
			switch (DeterminePrize(prizeData))
			{
			case WeeklyHuntData.WeeklyHuntPrize.Coin:
				return Strings.Get("WEEKLYHUNT_PRIZE_COIN_TITLE_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.Key:
				return Strings.Get("WEEKLYHUNT_PRIZE_KEYS_TITLE_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.SuperMysteryBox:
				if (prizeData.amount == 1)
				{
					return Strings.Get("WEEKLYHUNT_PRIZE_SUPERMYSTERYBOX_TITLE_SHORT");
				}
				return Strings.Get("WEEKLYHUNT_PRIZE_SUPERMYSTERYBOXES_TITLE_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.TokenMysteryBox:
				if (prizeData.amount == 1)
				{
					return Strings.Get("WEEKLYHUNT_PRIZE_TOKENBOX_TITLE_SHORT");
				}
				return Strings.Get("WEEKLYHUNT_PRIZE_TOKENBOXES_TITLE_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.Hoverboard:
				return Strings.Get("WEEKLYHUNT_PRIZE_HOVERBOARD_TITLE_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.ScoreBooster:
				return Strings.Get("WEEKLYHUNT_PRIZE_SCOREBOOSTER_TITLE_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.Headstart:
				return Strings.Get("WEEKLYHUNT_PRIZE_HEADSTART_TITLE_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.ChristmasBoard:
				return Strings.Get("HOVERBOARDS_LONDONBOARD01");
			case WeeklyHuntData.WeeklyHuntPrize.RioBoard2:
				return Strings.Get("WEEKLYHUNT_PRIZE_RIO_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.NorthpoleBoard2016:
				return Strings.Get("HOVERBOARDS_LONDONBOARD03");
			case WeeklyHuntData.WeeklyHuntPrize.CherryBoard:
				return Strings.Get("WEEKLYHUNT_PRIZE_CHERRY_SHORT");
			case WeeklyHuntData.WeeklyHuntPrize.ChickyBoard:
				return Strings.Get("WEEKLYHUNT_PRIZE_CHICKY_BOARD");
			case WeeklyHuntData.WeeklyHuntPrize.EagleBoard:
				return Strings.Get("WEEKLYHUNT_PRIZE_EAGLE_BOARD");
			default:
				return string.Empty;
			}
		}

		public static TimeSpan GetTimeLeftForActivePeriod()
		{
			WeeklyHuntData.PrizePeriodId activePeriod = WeeklyHuntData.GetActivePeriod();
			return huntPeriodData[(int)activePeriod].periodEndDate - CDateTime.UtcNow;
		}

		public static DateTime GetEndTimeForActivePeriod()
		{
			WeeklyHuntData.PrizePeriodId activePeriod = WeeklyHuntData.GetActivePeriod();
			return huntPeriodData[(int)activePeriod].periodEndDate;
		}
	}
}
