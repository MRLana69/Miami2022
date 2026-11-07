using System;
using SYBO.Subway;
using SYBO.Subway.Characters;

namespace Missions
{
	public class ScorePointsSingleLane : Award
	{
		private Character _character;

		private int _offset;

		public ScorePointsSingleLane(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
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
			Game.OnGameStarted += OnRunStarted;
			Game.OnMenuScreenShown += OnRunEnded;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			GameStats.Instance.OnScoreChanged -= OnScoreChanged;
			Game.OnGameStarted -= OnRunStarted;
			Game.OnMenuScreenShown -= OnRunEnded;
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnRunStarted()
		{
			_character = Character.Instance;
			if (_character != null)
			{
				_character.OnChangeTrack += OnLaneChanged;
			}
			_offset = 0;
		}

		private void OnRunEnded()
		{
			if (_character != null)
			{
				_character.OnChangeTrack -= OnLaneChanged;
			}
			_offset = GameStats.Instance.Score;
		}

		private void OnScoreChanged()
		{
			CheckAndUpdateProgress();
		}

		private void OnLaneChanged(Character.OnChangeTrackDirection direction)
		{
			_offset = GameStats.Instance.Score;
		}
	}
}
