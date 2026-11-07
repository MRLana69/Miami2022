using System;
using SYBO.Subway;
using UI.Screens;

namespace Missions
{
	public class CollectCoinsSingleRun : Award
	{
		private int _progress;

		private int _offset;

		public CollectCoinsSingleRun(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
			ResetProgress();
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return _progress - _offset;
		}

		public override bool IsGoalAccomplished(AwardGoal goal)
		{
			AwardSimpleGoal awardSimpleGoal = goal as AwardSimpleGoal;
			if (awardSimpleGoal != null && GetProgressForGoal(goal) == awardSimpleGoal.goal)
			{
				return true;
			}
			return false;
		}

		protected override void ActivateCurrentAwardTier()
		{
			GameStats.Instance.OnCoinsIncreased += OnCoinsIncreased;
			Game.OnGameStarted += OnRunStarted;
			GameOverScreen.onGameOverScreenShown += OnRunEnded;
			UIScreenController.OnNavigatedToScreen += OnFrontScreen;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			GameStats.Instance.OnCoinsIncreased -= OnCoinsIncreased;
			Game.OnGameStarted -= OnRunStarted;
			GameOverScreen.onGameOverScreenShown -= OnRunEnded;
			UIScreenController.OnNavigatedToScreen -= OnFrontScreen;
		}

		private void OnFrontScreen(string screenName)
		{
			if (screenName == "FrontScreen")
			{
				ResetProgress();
			}
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnRunStarted()
		{
			ResetProgress();
		}

		private void OnRunEnded()
		{
			CheckAndUpdateProgress();
		}

		private void OnCoinsIncreased(int count)
		{
			_progress += count;
		}

		private void ResetProgress()
		{
			_offset = 0;
			_progress = 0;
		}
	}
}
