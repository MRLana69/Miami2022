using UnityEngine;

namespace Missions
{
	public class MissionSkip
	{
		public string name { get; private set; }

		public int rawPrice { get; private set; }

		public int priceIncrease { get; private set; }

		public MissionSkip(string name, int rawPrice, int priceIncrease)
		{
			this.name = name;
			this.rawPrice = rawPrice;
			this.priceIncrease = priceIncrease;
		}

		public int GetPrice()
		{
			return rawPrice + priceIncrease * Mathf.Clamp(MissionsManager.Instance.currentMissionSet, 0, MissionsManager.Instance.missionSetStoryCount);
		}
	}
}
