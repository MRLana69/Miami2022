namespace Kiloo.GameCenter
{
	public class GameCenterLoadLeaderboardRequest : GameCenterRequest
	{
		public string category { get; set; }

		public GameCenterPlugin.LeaderboardTimeScope timeScope { get; set; }

		public GameCenterPlugin.LeaderboardPlayerScope playerScope { get; set; }

		public int rangeBegin { get; set; }

		public int rangeCount { get; set; }

		public string[] playerIDs { get; set; }

		public GameCenterPlugin.Leaderboard result { get; internal set; }
	}
}
