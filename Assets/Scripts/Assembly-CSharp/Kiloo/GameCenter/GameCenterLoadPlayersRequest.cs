namespace Kiloo.GameCenter
{
	public class GameCenterLoadPlayersRequest : GameCenterRequest
	{
		public string[] playerIDs { get; internal set; }

		public GameCenterPlugin.Player[] result { get; internal set; }
	}
}
