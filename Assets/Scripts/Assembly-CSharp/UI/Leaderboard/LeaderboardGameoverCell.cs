using System;

namespace UI.Leaderboard
{
	public class LeaderboardGameoverCell
	{
		public LeaderboardGameoverCellType cellType;

		public Friend friend;

		public LeaderboardDummyData.LeaderboardDummy dummyFriend;

		public int rank;

		public bool isPlayerRanked;

		public bool useDarkColour;

		public bool lockList;

		public bool noFriends;

		public Action OnUpdateCell;

		public void UpdateCell()
		{
			if (OnUpdateCell != null)
			{
				OnUpdateCell();
			}
		}
	}
}
