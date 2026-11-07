namespace TopRun.Internal
{
	public class Rank : IRank
	{
		public int rank { get; private set; }

		public string country { get; private set; }

		public Rank(string country, int rank)
		{
			this.country = country;
			this.rank = rank;
		}
	}
}
