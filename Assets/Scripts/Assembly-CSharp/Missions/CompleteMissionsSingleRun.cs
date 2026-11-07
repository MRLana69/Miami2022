using System;
using SYBO.Subway;

namespace Missions
{
	public class CompleteMissionsSingleRun : Award
	{
		private int _progress;

		private int _offset;

		public CompleteMissionsSingleRun(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
			_progress = 0;
			_offset = 0;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return _progress - _offset;
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
			MissionsManager.onMissionComplete += OnMissionCompleted;
			Game.OnGameStarted += OnRunStarted;
			Game.OnMenuScreenShown += OnRunEnded;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			MissionsManager.onMissionComplete -= OnMissionCompleted;
			Game.OnGameStarted -= OnRunStarted;
			Game.OnMenuScreenShown -= OnRunEnded;
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnRunStarted()
		{
			_offset = 0;
			_progress = 0;
		}

		private void OnRunEnded()
		{
			_offset = _progress;
		}

		private void OnMissionCompleted(string description, int missionIndex)
		{
			if (MissionsManager.Instance.inRun)
			{
				_progress++;
				CheckAndUpdateProgress();
			}
		}
	}
}
