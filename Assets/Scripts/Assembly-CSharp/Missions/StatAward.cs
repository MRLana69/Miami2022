using System;
using Data.Save;

namespace Missions
{
	public class StatAward : Award
	{
		private bool _shouldMigrateToUseStatOffset;

		public StatAward(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
			if (!savedProgress.hasMigratedToUseStatOffset && awardData.migrateToUseStatOffset && awardData.useStatOffset)
			{
				_shouldMigrateToUseStatOffset = true;
			}
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			AwardStatGoal awardStatGoal = goal as AwardStatGoal;
			if (awardStatGoal != null)
			{
				return awardStatGoal.stat.GetValue() - OffsetForStat(awardStatGoal.stat);
			}
			return 0;
		}

		public override bool IsGoalAccomplished(AwardGoal goal)
		{
			AwardStatGoal awardStatGoal = goal as AwardStatGoal;
			if (awardStatGoal != null && awardStatGoal.stat.GetValue() >= awardStatGoal.goal + OffsetForStat(awardStatGoal.stat))
			{
				return true;
			}
			return false;
		}

		private int OffsetForStat(Stat stat)
		{
			if (savedProgress.statOffset.ContainsKey(stat) && _awardData.useStatOffset)
			{
				if (_shouldMigrateToUseStatOffset)
				{
					return 0;
				}
				return savedProgress.statOffset[stat];
			}
			return 0;
		}

		protected override void ActivateCurrentAwardTier()
		{
			PlayerSave.statChanged += OnStatChanged;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			PlayerSave.statChanged -= OnStatChanged;
		}

		protected override void RecordNewOffsets()
		{
			AwardGoal[] listGoals = _awardProgressTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
				if (awardStatGoal != null)
				{
					Stat stat = awardStatGoal.stat;
					int value = stat.GetValue();
					savedProgress.statOffset[stat] = value;
				}
			}
			savedProgress.hasMigratedToUseStatOffset = true;
			_shouldMigrateToUseStatOffset = false;
		}

		private void OnStatChanged(Stat stat, int newValue)
		{
			bool flag = true;
			AwardGoal[] listGoals = _awardProgressTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
				if (awardStatGoal != null)
				{
					if (stat == awardStatGoal.stat)
					{
						flag = flag && stat.GetValue() >= awardGoal.goal + OffsetForStat(stat);
						continue;
					}
					flag = false;
					break;
				}
				flag = false;
				break;
			}
			if (flag)
			{
				PlayerSave.statChanged -= OnStatChanged;
			}
			CheckAndUpdateProgress();
		}
	}
}
