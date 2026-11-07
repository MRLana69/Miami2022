using UI.Screens.RewardManagement;
using UnityEngine;

public class CoinsForVideoManager
{
	private static CoinsForVideoManager _instance;

	private NotEnoughCoinsLootTableEntry[] _lootTable;

	private const string VIDEO_AVAIL_YES = "Yes";

	private const string BUTTON_NAME_VIDEO = "Video";

	private const string BUTTON_NO_COOLDOWN = "No Cooldown";

	private const string BUTTON_COOLDOWN = "Cooldown_{0}_watched";

	public static CoinsForVideoManager instance
	{
		get
		{
			return _instance ?? (_instance = new CoinsForVideoManager());
		}
	}

	private CoinsForVideoManager()
	{
	}

	public int GetRewardAmount()
	{
		if (_lootTable == null)
		{
			CreateLootTable();
		}
		NotEnoughCoinsLootTableEntry notEnoughCoinsLootTableEntry = MakeRoll();
		return notEnoughCoinsLootTableEntry.amount;
	}

	private void CreateLootTable()
	{
		_lootTable = new NotEnoughCoinsLootTableEntry[12]
		{
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 35f,
				amount = 500
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 28f,
				amount = 600
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 21f,
				amount = 800
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 8f,
				amount = 1000
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 3f,
				amount = 1500
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 2f,
				amount = 2000
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 1f,
				amount = 2500
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.6f,
				amount = 3000
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.5f,
				amount = 4000
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.4f,
				amount = 5000
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.3f,
				amount = 7000
			},
			new NotEnoughCoinsLootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.2f,
				amount = 10000
			}
		};
	}

	private NotEnoughCoinsLootTableEntry MakeRoll()
	{
		float num = 0f;
		for (int i = 0; i < _lootTable.Length; i++)
		{
			NotEnoughCoinsLootTableEntry notEnoughCoinsLootTableEntry = _lootTable[i];
			num += notEnoughCoinsLootTableEntry.weight;
		}
		float num2 = Random.Range(0f, 1f) * num;
		float num3 = 0f;
		NotEnoughCoinsLootTableEntry result = null;
		for (int j = 0; j < _lootTable.Length; j++)
		{
			NotEnoughCoinsLootTableEntry notEnoughCoinsLootTableEntry2 = _lootTable[j];
			num3 += notEnoughCoinsLootTableEntry2.weight;
			if (num2 <= num3)
			{
				result = notEnoughCoinsLootTableEntry2;
				break;
			}
		}
		return result;
	}
}
