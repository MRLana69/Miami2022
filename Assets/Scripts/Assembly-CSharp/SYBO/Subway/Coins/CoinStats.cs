namespace SYBO.Subway.Coins
{
	public class CoinStats
	{
		public static CoinStats New = new CoinStats();

		public static CoinStats Old = new CoinStats();

		public int CreatedCoins { get; private set; }

		public int DestroyedCoins { get; private set; }

		public int ActivatedCoins { get; private set; }

		public int DeactivatedCoins { get; private set; }

		public int ActivatedSpawnPoints { get; private set; }

		public int DeactivatedSpawnPoints { get; private set; }

		public int DynamicCoinSpawns { get; private set; }

		public int DynamicCoinDespawns { get; private set; }

		public void NotifyCoinCreated()
		{
			CreatedCoins++;
		}

		public void NotifyCoinDestroyed()
		{
			DestroyedCoins++;
		}

		public void NotifyCoinActivated()
		{
			ActivatedCoins++;
		}

		public void NotifyCoinDeactivated()
		{
			DeactivatedCoins++;
		}

		public void NotifySpawnPointActivated(int coins)
		{
			ActivatedSpawnPoints += coins;
		}

		public void NotifySpawnPointDeactivated(int coins)
		{
			DeactivatedSpawnPoints += coins;
		}

		public void NotifyDynamicCoinSpawned()
		{
			DynamicCoinSpawns++;
		}

		public void NotifyDynamicCoinsDespawned(int coins)
		{
			DynamicCoinDespawns += coins;
		}
	}
}
