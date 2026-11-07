using System;
using System.Collections.Generic;

namespace TopRun.Internal
{
	public class PlayerIdsData
	{
		public class PlayerIdData
		{
			public string publicKey;

			public string name;

			public DateTime? expiry;
		}

		public Dictionary<string, PlayerIdData> players = new Dictionary<string, PlayerIdData>();
	}
}
