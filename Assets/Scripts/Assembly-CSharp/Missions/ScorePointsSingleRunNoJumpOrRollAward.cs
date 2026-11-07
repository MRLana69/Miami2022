using System;
using SYBO.Subway;

namespace Missions
{
	public class ScorePointsSingleRunNoJumpOrRollAward : Award
	{
		private int _offset;

		public ScorePointsSingleRunNoJumpOrRollAward(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GameStats.Instance.Score - _offset;
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
			GameStats.Instance.OnScoreChanged += OnScoreChanged;
			GameStats.Instance.OnJumpsChanged += OnJumpsChanged;
			GameStats.Instance.OnRollsChanged += OnRollsChanged;
			Game.OnGameStarted += OnRunStarted;
			Game.OnMenuScreenShown += OnRunEnded;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			GameStats.Instance.OnScoreChanged -= OnScoreChanged;
			GameStats.Instance.OnJumpsChanged -= OnJumpsChanged;
			GameStats.Instance.OnRollsChanged -= OnRollsChanged;
			Game.OnGameStarted -= OnRunStarted;
			Game.OnMenuScreenShown -= OnRunEnded;
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnRunStarted()
		{
			_offset = 0;
		}

		private void OnRunEnded()
		{
			_offset = GameStats.Instance.Score;
		}

		private void OnScoreChanged()
		{
			CheckAndUpdateProgress();
		}

		private void OnJumpsChanged()
		{
			_offset = GameStats.Instance.Score;
		}

		private void OnRollsChanged()
		{
			_offset = GameStats.Instance.Score;
		}
	}
}
