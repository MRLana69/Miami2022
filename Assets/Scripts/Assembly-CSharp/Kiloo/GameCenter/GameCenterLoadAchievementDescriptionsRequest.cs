namespace Kiloo.GameCenter
{
	public class GameCenterLoadAchievementDescriptionsRequest : GameCenterRequest
	{
		public GameCenterPlugin.AchievementDescription[] result { get; internal set; }
	}
}
