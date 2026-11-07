namespace Kiloo.GameCenter
{
	public class GameCenterReportAchievementsRequest : GameCenterRequest
	{
		public GameCenterPlugin.Achievement[] achievements { get; set; }

		public bool showsCompletionBanner { get; set; }

		public GameCenterReportAchievementsRequest(GameCenterPlugin.Achievement[] achievements, bool showsCompletionBanner)
		{
			this.achievements = achievements;
			this.showsCompletionBanner = showsCompletionBanner;
		}

		public GameCenterReportAchievementsRequest(GameCenterPlugin.Achievement achievement, bool showsCompletionBanner)
			: this(new GameCenterPlugin.Achievement[1] { achievement }, showsCompletionBanner)
		{
		}

		public GameCenterReportAchievementsRequest(string identifier, float percentComplete, bool showsCompletionBanner)
			: this(new GameCenterPlugin.Achievement
			{
				identifier = identifier,
				percentComplete = percentComplete
			}, showsCompletionBanner)
		{
		}
	}
}
