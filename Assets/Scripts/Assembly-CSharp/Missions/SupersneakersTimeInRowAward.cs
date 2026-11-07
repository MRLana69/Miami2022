using System;

namespace Missions
{
	public class SupersneakersTimeInRowAward : Award
	{
		public SupersneakersTimeInRowAward(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			SuperSneakers instance = SuperSneakers.Instance;
			if (instance != null)
			{
				float timeActiveInARow = instance.timeActiveInARow;
				return (int)timeActiveInARow / 60;
			}
			return 0;
		}

		public override bool IsGoalAccomplished(AwardGoal goal)
		{
			AwardSimpleGoal awardSimpleGoal = goal as AwardSimpleGoal;
			if (awardSimpleGoal != null && GetProgressForGoal(goal) >= awardSimpleGoal.goal)
			{
				return true;
			}
			return false;
		}

		protected override void ActivateCurrentAwardTier()
		{
			SuperSneakers.OnSupersneakersTimeActiveChange = (Action)Delegate.Combine(SuperSneakers.OnSupersneakersTimeActiveChange, new Action(OnTimeChanged));
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			SuperSneakers.OnSupersneakersTimeActiveChange = (Action)Delegate.Remove(SuperSneakers.OnSupersneakersTimeActiveChange, new Action(OnTimeChanged));
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnTimeChanged()
		{
			CheckAndUpdateProgress();
		}
	}
}
