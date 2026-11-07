using System;

namespace Info.VideoRewardCooldowns
{
	public class Cooldown
	{
		public string id { get; set; }

		public int totalCharges { get; set; }

		public int usedCharges { get; set; }

		public int seconds { get; set; }

		public DateTime startTime { get; set; }
	}
}
