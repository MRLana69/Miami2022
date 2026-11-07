using System;
using Data.Managers;
using Info;

namespace Missions
{
	public class OwnBoardsAward : Award
	{
		public OwnBoardsAward(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
		}

		private int GetNumberOfOwnedBoards()
		{
			int num = 0;
			foreach (Boards.BoardId item in Boards.boardOrder)
			{
				if (BoardManager.IsBoardOwned(item))
				{
					num++;
				}
			}
			return num;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GetNumberOfOwnedBoards();
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
			BoardManager.boardUnlocked += BoardUnlocked;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			BoardManager.boardUnlocked -= BoardUnlocked;
		}

		protected override void RecordNewOffsets()
		{
		}

		private void BoardUnlocked(Boards.BoardId boardId)
		{
			CheckAndUpdateProgress();
		}
	}
}
