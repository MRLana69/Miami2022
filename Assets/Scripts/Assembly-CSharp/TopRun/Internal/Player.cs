namespace TopRun.Internal
{
	internal class Player : IPlayer
	{
		public string publicKey { get; internal set; }

		public int score { get; internal set; }

		public bool isFriend { get; internal set; }

		public FacebookProfile facebookProfile { get; internal set; }
	}
}
