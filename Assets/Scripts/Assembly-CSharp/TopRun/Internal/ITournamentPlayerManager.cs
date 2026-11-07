namespace TopRun.Internal
{
	internal interface ITournamentPlayerManager
	{
		IPlayer GetPlayerByPublicKey(string publicKey);
	}
}
