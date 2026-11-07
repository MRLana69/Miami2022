using System;
using UnityEngine;

namespace UI.Leaderboard
{
	public class LeaderboardCell
	{
		public LeaderboardCellType cellType;

		public Friend friend;

		public LeaderboardDummyData.LeaderboardDummy dummyFriend;

		public int rank;

		public bool isPlayerRanked;

		public bool useDarkColor;

		public bool lockList;

		public string headerText;

		public Color headerColor;

		public bool adjustOverlay;

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
