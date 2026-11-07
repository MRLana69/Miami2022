namespace Kiloo.GameCenter
{
	public class GameCenterRequest
	{
		public float timeout { get; set; }

		public bool didTimeout { get; internal set; }

		public bool isDone { get; internal set; }

		public string error { get; internal set; }
	}
}
