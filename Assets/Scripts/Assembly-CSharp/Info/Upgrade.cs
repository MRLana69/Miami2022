using UnityEngine;

namespace Info
{
	public class Upgrade
	{
		public string name;

		public string nameTwoLines;

		public string description;

		public string mysteryBoxDescription;

		public int numberOfTiers;

		public float speed;

		public float landSpeed;

		public int spawnProbability;

		public PurchaseData.UnlockType priceType = PurchaseData.UnlockType.Coins;

		public string iconNameIngame;

		public string iconNameMenu;

		public int weight;

		public float[] durations { private get; set; }

		public int[] prices { private get; set; }

		public string GetName()
		{
			if (!string.IsNullOrEmpty(Strings.Get(nameTwoLines)))
			{
				return nameTwoLines;
			}
			return name;
		}

		public int GetPriceToIncreaseTier(int tier)
		{
			return prices[Mathf.Clamp(tier, 0, prices.Length - 1)];
		}

		public float GetDuration(int tier)
		{
			return durations[Mathf.Clamp(tier, 0, durations.Length - 1)];
		}
	}
}
