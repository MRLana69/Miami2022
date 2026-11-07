using System;
using System.Collections.Generic;

namespace Missions
{
	public class PersistentAwardProgress
	{
		public AwardTierType activeTierType;

		public AwardTierType progressTierType;

		public AwardState currentActiveAwardState;

		public AwardState currentProgressAwardState;

		public DateTime lastActiveStateChangeDateTime = DateTime.MinValue;

		public int offset = -1;

		public Dictionary<Stat, int> statOffset = new Dictionary<Stat, int>();

		public bool hasMigratedToUseStatOffset;
	}
}
