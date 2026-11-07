using System;
using System.Collections.Generic;
using AppRefresh;
using Data.Save;
using Info;

namespace Missions
{
	public class AwardsManager
	{
		private static AwardsManager _instance;

		private List<Award> _listAwards = new List<Award>();

		private static bool _hasInited;

		public static AwardsManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AwardsManager();
				}
				return _instance;
			}
		}

		private AwardsManager()
		{
			AppRefreshHandler.hasRefreshed += OnHasRefreshed;
		}

		private void OnHasRefreshed(RefreshType refreshType)
		{
			if (refreshType.HasFlag(RefreshType.Data) && _hasInited)
			{
				_hasInited = false;
				InitAwards();
			}
		}

		private void UpdateTheNewStatusForTheAwardsOnCleanInstall()
		{
			if (PlayerSave.awardsFirstLoaded)
			{
				return;
			}
			PlayerSave.awardsFirstLoaded = true;
			bool flag = true;
			foreach (Award listAward in _listAwards)
			{
				if (!listAward.isNew)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			foreach (Award listAward2 in _listAwards)
			{
				listAward2.isNew = false;
			}
		}

		public void InitAwards()
		{
			if (_hasInited)
			{
				return;
			}
			_hasInited = true;
			Dictionary<AwardId, PersistentAwardProgress> awardsProgress = PlayerSave.awardsProgress;
			_listAwards.Clear();
			AwardStaticData[] awardStaticDataList = AwardsData.GetAwardStaticDataList();
			AwardStaticData[] array = awardStaticDataList;
			foreach (AwardStaticData awardStaticData in array)
			{
				PersistentAwardProgress value;
				if (!awardsProgress.TryGetValue(awardStaticData.id, out value))
				{
					value = new PersistentAwardProgress();
				}
				Award award = null;
				switch (awardStaticData.awardType)
				{
				case AwardStaticData.AwardType.Stat:
					award = new StatAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.DailyChallengeInRow:
					award = new DailyChallengesInRowAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.OwnCharacters:
					award = new OwnCharactersAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.OwnBoards:
					award = new OwnBoardsAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.OwnCharacterOutfits:
					award = new OwnCharactersOutfitsAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.ScorePointsSingleRunNoCoins:
					award = new ScorePointsSingleRunNoCoinsAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.CompleteMissionsSingleRun:
					award = new CompleteMissionsSingleRun(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.HaveSupersneakersXTime:
					award = new SupersneakersTimeInRowAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.CollectCoinsSingleRun:
					award = new CollectCoinsSingleRun(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.ScorePointsSingleRunNoJumpOrRoll:
					award = new ScorePointsSingleRunNoJumpOrRollAward(awardStaticData, value, OnMissionStateChanged);
					break;
				case AwardStaticData.AwardType.ScorePointsSingleLane:
					award = new ScorePointsSingleLane(awardStaticData, value, OnMissionStateChanged);
					break;
				}
				if (award != null)
				{
					_listAwards.Add(award);
				}
			}
			foreach (Award listAward in _listAwards)
			{
				listAward.Activate();
				listAward.SetBackToInProgress();
			}
			UpdateTheNewStatusForTheAwardsOnCleanInstall();
			SaveAllAwards();
		}

		private void InitAwardsIfNeeded()
		{
			if (!_hasInited)
			{
				InitAwards();
			}
		}

		private void OnMissionStateChanged(Award award)
		{
			SaveAwardProgress(award);
		}

		private void SaveAwardProgress(Award award)
		{
			PlayerSave.awardsProgress[award.awardsData.id] = award.GetPersistentProgress();
		}

		private void SaveAllAwards()
		{
			foreach (Award listAward in _listAwards)
			{
				SaveAwardProgress(listAward);
			}
		}

		public List<Award> GetAwardsForState(AwardState state)
		{
			InitAwardsIfNeeded();
			List<Award> list = new List<Award>();
			foreach (Award listAward in _listAwards)
			{
				if (listAward.BelongsToState(state) && ShouldInsertAwardToList(listAward))
				{
					list.Add(listAward);
				}
			}
			if (state == AwardState.Collectible || state == AwardState.Finished)
			{
				list.Sort(SortAwardByDate);
			}
			return list;
		}

		public List<Award> GetSortedListAwards(out bool containsCollectibleAwards)
		{
			List<Award> list = new List<Award>();
			containsCollectibleAwards = false;
			List<Award> awardsForState = GetAwardsForState(AwardState.Collectible);
			if (awardsForState.Count > 0)
			{
				containsCollectibleAwards = true;
				list.AddRange(awardsForState);
			}
			List<Award> awardsForState2 = GetAwardsForState(AwardState.InProgress);
			if (awardsForState2.Count > 0)
			{
				awardsForState2.Sort((Award a1, Award a2) =>
				{
					int num = a2.GetProgressAsPercentage().CompareTo(a1.GetProgressAsPercentage());
					return (num != 0) ? num : a2.activeTierType.CompareTo(a1.activeTierType);
				});
				list.AddRange(awardsForState2);
			}
			list.AddRange(GetAwardsForState(AwardState.Finished));
			return list;
		}

		public int GetNumberOfNewVisibleAwards()
		{
			if (_listAwards == null)
			{
				InitAwards();
			}
			int num = 0;
			foreach (Award listAward in _listAwards)
			{
				if (listAward.isNew && ShouldInsertAwardToList(listAward))
				{
					num++;
				}
			}
			return num;
		}

		public int GetTotalAwardTiers()
		{
			int num = Enum.GetNames(typeof(AwardTierType)).Length - 1;
			int num2 = Enum.GetNames(typeof(AwardId)).Length - 1;
			return num * num2;
		}

		public int GetTotalCompletedTiers()
		{
			int num = 0;
			num += GetNumberOfFinishedAwardsExactTier(AwardTierType.Diamond) * 4;
			num += GetNumberOfFinishedAwardsExactTier(AwardTierType.Gold) * 3;
			num += GetNumberOfFinishedAwardsExactTier(AwardTierType.Silver) * 2;
			return num + GetNumberOfFinishedAwardsExactTier(AwardTierType.Bronze);
		}

		public float GetAwardsPercentageComplete()
		{
			return (float)GetTotalCompletedTiers() / (float)GetTotalAwardTiers() * 100f;
		}

		public int GetNumberOfFinishedAwardsExactTier(AwardTierType awardTierType)
		{
			InitAwardsIfNeeded();
			int num = 0;
			foreach (Award listAward in _listAwards)
			{
				if (listAward != null && listAward.HasCompletedTierExact(awardTierType))
				{
					num++;
				}
			}
			return num;
		}

		private bool ShouldInsertAwardToList(Award award)
		{
			if (AgeRestriction.isRestricted && award.awardsData.useFacebook)
			{
				return false;
			}
			return true;
		}

		private int SortAwardByDate(Award a, Award b)
		{
			return DateTime.Compare(b.lastStateChangeDateTime, a.lastStateChangeDateTime);
		}

		public Award GetAwardForId(AwardId id)
		{
			InitAwardsIfNeeded();
			foreach (Award listAward in _listAwards)
			{
				if (listAward.awardsData.id == id)
				{
					return listAward;
				}
			}
			return null;
		}

		public bool ShouldSpawnOldTrophyForStat(Stat stat)
		{
			InitAwardsIfNeeded();
			foreach (Award listAward in _listAwards)
			{
				if (listAward.awardsData.awardType != AwardStaticData.AwardType.Stat)
				{
					continue;
				}
				AwardGoal[] goals = listAward.goals;
				AwardGoal[] array = goals;
				foreach (AwardGoal awardGoal in array)
				{
					AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
					if (awardStatGoal != null && stat == awardStatGoal.stat)
					{
						if (listAward.awardState != AwardState.InProgress)
						{
							return false;
						}
						return !listAward.IsGoalAccomplished(awardStatGoal);
					}
				}
			}
			return false;
		}
	}
}
