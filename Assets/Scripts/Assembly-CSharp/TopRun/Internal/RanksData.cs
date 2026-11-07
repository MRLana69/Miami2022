using System.Collections.Generic;

namespace TopRun.Internal
{
	public class RanksData
	{
		public string tournamentId;

		public Dictionary<string, Rank> ranks = new Dictionary<string, Rank>();
	}
}
