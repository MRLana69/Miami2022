using System.Collections;
using System.Collections.Generic;

namespace Data.Migration
{
	public class PlayerInfoData
	{
		public Dictionary<Key, string> mainPlayerInfoData;

		public Dictionary<PowerupTypeLegacy, int> upgradeAmounts;

		public Dictionary<PowerupTypeLegacy, int> upgradeTiers;

		public IDictionary topRunManager;

		public IDictionary topRunPlayerIds;

		public IDictionary topRunRanks;

		public Dictionary<string, Friend.Status> socialData;

		public IDictionary<string, object> adSystemData;
	}
}
