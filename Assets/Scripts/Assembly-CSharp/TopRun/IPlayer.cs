namespace TopRun
{
	public interface IPlayer
	{
		string publicKey { get; }

		int score { get; }

		bool isFriend { get; }

		FacebookProfile facebookProfile { get; }
	}
}
