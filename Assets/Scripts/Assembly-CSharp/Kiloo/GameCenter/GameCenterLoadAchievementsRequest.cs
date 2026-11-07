namespace Kiloo.GameCenter
{
	public class GameCenterLoadAchievementsRequest : GameCenterRequest
	{
		public GameCenterPlugin.Achievement[] result { get; internal set; }
	}
}
