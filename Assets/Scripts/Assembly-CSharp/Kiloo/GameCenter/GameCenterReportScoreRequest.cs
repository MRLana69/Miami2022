namespace Kiloo.GameCenter
{
	public class GameCenterReportScoreRequest : GameCenterRequest
	{
		public string category { get; set; }

		public long scoreValue { get; set; }

		public ulong scoreContext { get; set; }
	}
}
