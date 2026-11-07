using System;
using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Subway.Info;
using UI.Screens.RewardManagement;
using UnityEngine;

public static class MysteryBox
{
	public enum Type
	{
		Normal = 0,
		Super = 1,
		Mini = 2,
		Token = 3
	}

	public class LootTableEntry
	{
		public CelebrationRewardType itemType;

		public float weight;

		public int min = 1;

		public int max = 1;

		public UpgradeType upgradeType;

		public Multipliers multiplier = Multipliers.NotSet;
	}

	public enum Multipliers
	{
		NotSet = 1,
		x2 = 2,
		x3 = 3,
		x4 = 4
	}

	public static readonly Characters.CharacterId[] TOKEN_REWARD_IDS;

	public static readonly Dictionary<Characters.CharacterId, int> characterTokenWeight;

	private static readonly LootTableEntry[][] _lootTables;

	public const int MYSTERY_BOX_JACKPOT_AMOUNT = 100000;

	public const int SUPER_MYSTERY_BOX_JACKPOT_AMOUNT = 300000;

	public const int DEFAULT_MEGA_JACKPOT_MYSTERYBOX = 900000;

	public const int DEFAULT_MEGA_JACKPOT_SUPER_MYSTERYBOX = 1500000;

	public const int MYSTERY_BOX_RARE_COINS_AMOUNT = 5000;

	public const int MYSTERY_BOX_SPECIAL_COINS_AMOUNT = 2500;

	public const int MYSTERY_BOX_SPECIAL_BOARDS_AMOUNT = 10;

	public const int SUPER_MYSTERY_BOX_RARE_COINS_AMOUNT = 50000;

	public const int SUPER_MYSTERY_BOX_SPECIAL_COINS_AMOUNT = 8000;

	public const int SUPER_MYSTERY_BOX_SPECIAL_BOARDS_AMOUNT = 20;

	public const int MINI_MYSTERY_BOX_COMMON_BOARDS_AMOUNT = 3;

	public const int MINI_MYSTERY_BOX_COMMON_COINS_AMOUNT = 200;

	public const int MINI_MYSTERY_BOX_RARE_KEYS_AMOUNT = 2;

	public const int TOKEN_MYSTERY_BOX_MIN_TOKEN_AMOUNT = 12;

	public const int TOKEN_MYSTERY_BOX_MAX_TOKEN_AMOUNT = 16;

	private static readonly CelebrationReward MYSTERYBOX_CONSOLATION_REWARD;

	private static readonly CelebrationReward SUPERMYSTERYBOX_CONSOLATION_REWARD;

	private static readonly CelebrationReward TOKENMYSTERYBOX_CONSOLATION_REWARD;

	static MysteryBox()
	{
		TOKEN_REWARD_IDS = new Characters.CharacterId[5]
		{
			Characters.CharacterId.slick,
			Characters.CharacterId.tricky,
			Characters.CharacterId.fresh,
			Characters.CharacterId.spike,
			Characters.CharacterId.yutani
		};
		characterTokenWeight = new Dictionary<Characters.CharacterId, int>
		{
			{
				Characters.CharacterId.slick,
				25
			},
			{
				Characters.CharacterId.tricky,
				35
			},
			{
				Characters.CharacterId.fresh,
				20
			},
			{
				Characters.CharacterId.spike,
				10
			},
			{
				Characters.CharacterId.yutani,
				10
			}
		};
		MYSTERYBOX_CONSOLATION_REWARD = new CelebrationReward
		{
			rewardType = CelebrationRewardType.coins,
			amount = 500
		};
		SUPERMYSTERYBOX_CONSOLATION_REWARD = new CelebrationReward
		{
			rewardType = CelebrationRewardType.coins,
			amount = 1500
		};
		TOKENMYSTERYBOX_CONSOLATION_REWARD = new CelebrationReward
		{
			rewardType = CelebrationRewardType.keys,
			amount = 6
		};
		Type[] array = (Type[])Enum.GetValues(typeof(Type));
		_lootTables = new LootTableEntry[array.Length][];
		_lootTables[0] = new LootTableEntry[20]
		{
			new LootTableEntry
			{
				itemType = CelebrationRewardType.token,
				weight = 30f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.token,
				weight = 15f,
				min = 2,
				max = 2,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.trophy,
				weight = 1f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 6.89f,
				min = 200,
				max = 300,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 5f,
				min = 500,
				max = 500,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 6f,
				min = 800,
				max = 800,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 7f,
				min = 1000,
				max = 1000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 3f,
				min = 1500,
				max = 1500,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 1f,
				min = 2500,
				max = 2500,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.1f,
				min = 5000,
				max = 5000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 3f,
				min = 1,
				max = 1,
				upgradeType = UpgradeType.hoverboard,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 6f,
				min = 2,
				max = 2,
				upgradeType = UpgradeType.hoverboard,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 10f,
				min = 3,
				max = 3,
				upgradeType = UpgradeType.hoverboard,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1f,
				min = 4,
				max = 10,
				upgradeType = UpgradeType.hoverboard,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 2f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1.7f,
				min = 1,
				max = 1,
				upgradeType = UpgradeType.headstart2000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.2f,
				min = 2,
				max = 2,
				upgradeType = UpgradeType.headstart2000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.1f,
				min = 3,
				max = 3,
				upgradeType = UpgradeType.headstart2000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1f,
				min = 1,
				max = 1,
				upgradeType = UpgradeType.scorebooster,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.01f,
				min = 100000,
				max = 100000
			}
		};
		_lootTables[1] = new LootTableEntry[24]
		{
			new LootTableEntry
			{
				itemType = CelebrationRewardType.token,
				weight = 30f,
				min = 3,
				max = 3,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.trophy,
				weight = 6f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 15.52f,
				min = 1500,
				max = 1500,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 8f,
				min = 4,
				max = 8,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 2.5f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 11f,
				min = 2000,
				max = 2000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1.8f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 7f,
				min = 3000,
				max = 3000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 3f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 1.5f,
				min = 2,
				max = 2,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1f,
				min = 2,
				max = 2,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.7f,
				min = 10,
				max = 20,
				upgradeType = UpgradeType.hoverboard,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 4f,
				min = 5000,
				max = 5000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 1f,
				min = 3,
				max = 3,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.8f,
				min = 3,
				max = 3,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 2f,
				min = 2,
				max = 2,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 1.5f,
				min = 8000,
				max = 8000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 1f,
				min = 3,
				max = 3,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.scorebooster
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.4f,
				min = 5,
				max = 5,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.3f,
				min = 40,
				max = 40,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.4f,
				min = 15000,
				max = 15000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.3f,
				min = 20000,
				max = 20000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.2f,
				min = 50000,
				max = 50000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.08f,
				min = 300000,
				max = 300000
			}
		};
		_lootTables[2] = new LootTableEntry[14]
		{
			new LootTableEntry
			{
				itemType = CelebrationRewardType.token,
				weight = 25f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x3
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 20f,
				min = 100,
				max = 100,
				multiplier = Multipliers.x4
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 15f,
				min = 200,
				max = 200,
				multiplier = Multipliers.x4
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 9f,
				min = 500,
				max = 500,
				multiplier = Multipliers.x3
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 5f,
				min = 800,
				max = 800,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 1.5f,
				min = 1000,
				max = 1000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.coins,
				weight = 0.5f,
				min = 2000,
				max = 2000,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 6f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x4,
				upgradeType = UpgradeType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 7f,
				min = 2,
				max = 2,
				multiplier = Multipliers.x3,
				upgradeType = UpgradeType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 8f,
				min = 3,
				max = 3,
				multiplier = Multipliers.x2,
				upgradeType = UpgradeType.hoverboard
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 1.8f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x3
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.keys,
				weight = 0.2f,
				min = 2,
				max = 2,
				multiplier = Multipliers.x2
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.5f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x3,
				upgradeType = UpgradeType.headstart2000
			},
			new LootTableEntry
			{
				itemType = CelebrationRewardType.powerup,
				weight = 0.5f,
				min = 1,
				max = 1,
				multiplier = Multipliers.x3,
				upgradeType = UpgradeType.scorebooster
			}
		};
		_lootTables[3] = new LootTableEntry[1]
		{
			new LootTableEntry
			{
				itemType = CelebrationRewardType.token,
				weight = 1f,
				min = 12,
				max = 16
			}
		};
	}

	public static void TestRolls(int numberOfRolls, Type mysteryBoxType = Type.Normal)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int i = 0; i < numberOfRolls; i++)
		{
			CelebrationReward celebrationReward = Roll(mysteryBoxType);
			string text = celebrationReward.rewardType.ToString();
			if (celebrationReward.rewardType == CelebrationRewardType.powerup)
			{
				text += celebrationReward.upgradeType;
			}
			text += celebrationReward.amount;
			if (mysteryBoxType == Type.Normal)
			{
				if (celebrationReward.rewardType == CelebrationRewardType.coins && celebrationReward.amount.IsBetween(199, 301))
				{
					text = "coins200-300";
				}
				if (celebrationReward.rewardType == CelebrationRewardType.powerup && celebrationReward.upgradeType == UpgradeType.hoverboard && celebrationReward.amount.IsBetween(3, 11))
				{
					text = "poweruphoverboard4-10";
				}
			}
			int value;
			dictionary.TryGetValue(text, out value);
			value++;
			dictionary.AddOrSet(text, value);
		}
		int num = 0;
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			num += item.Value;
		}
		foreach (KeyValuePair<string, int> item2 in dictionary)
		{
		}
	}

	public static CelebrationReward Roll(Type mysteryBoxType)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		PlayerSave.IncrementMysteryBoxesOpened(mysteryBoxType);
		switch (mysteryBoxType)
		{
		case Type.Normal:
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 1)
			{
				celebrationReward.rewardType = CelebrationRewardType.coins;
				celebrationReward.amount = 1000;
				return celebrationReward;
			}
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 2)
			{
				celebrationReward.rewardType = CelebrationRewardType.token;
				celebrationReward.amount = 1;
				Characters.CharacterId characterId2 = Characters.CharacterId.tricky;
				if (!CharacterManager.IsTokenUseful(Characters.CharacterId.tricky) && CharacterManager.IsTokenUseful(Characters.CharacterId.fresh))
				{
					characterId2 = Characters.CharacterId.fresh;
				}
				else if (!CharacterManager.IsTokenUseful(Characters.CharacterId.fresh))
				{
					celebrationReward.rewardType = CelebrationRewardType.coins;
					celebrationReward.amount = UnityEngine.Random.Range(200, 300);
					return celebrationReward;
				}
				celebrationReward.characterId = characterId2;
				return celebrationReward;
			}
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 3)
			{
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.amount = 3;
				celebrationReward.upgradeType = UpgradeType.hoverboard;
				return celebrationReward;
			}
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 4)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
			break;
		case Type.Super:
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 1)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
			break;
		case Type.Mini:
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 1)
			{
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.upgradeType = UpgradeType.scorebooster;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 2)
			{
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = 1;
				return celebrationReward;
			}
			break;
		case Type.Token:
			if (PlayerSave.GetMysteryBoxesOpened(mysteryBoxType) == 1 && CharacterManager.IsTokenUseful(Characters.CharacterId.slick))
			{
				celebrationReward.rewardType = CelebrationRewardType.token;
				celebrationReward.amount = UnityEngine.Random.Range(12, 16);
				Characters.CharacterId characterId = Characters.CharacterId.slick;
				celebrationReward.characterId = characterId;
				return celebrationReward;
			}
			break;
		}
		List<Trophies.Trophy> availableTrophyList = GetAvailableTrophyList(mysteryBoxType);
		List<Characters.CharacterId> availableTokens = GetAvailableTokens();
		LootTableEntry lootTableEntry = null;
		if (mysteryBoxType == Type.Token && !AreTokensAvailable())
		{
			Upgrade upgrade = Upgrades.upgrades[UpgradeType.tokenmysterybox];
			lootTableEntry = new LootTableEntry();
			lootTableEntry.itemType = CelebrationRewardType.keys;
			lootTableEntry.min = (lootTableEntry.max = upgrade.GetPriceToIncreaseTier(0));
		}
		else
		{
			lootTableEntry = MakeRoll(mysteryBoxType);
			while ((lootTableEntry.itemType == CelebrationRewardType.trophy && availableTrophyList.Count <= 0) || (lootTableEntry.itemType == CelebrationRewardType.token && availableTokens.Count <= 0))
			{
				lootTableEntry = MakeRoll(mysteryBoxType);
			}
		}
		if (lootTableEntry == null)
		{
			SDebug.LogError("A reward was empty, how did this happen?");
		}
		if (lootTableEntry.itemType == CelebrationRewardType.coins)
		{
			celebrationReward.rewardType = CelebrationRewardType.coins;
			celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
			if (celebrationReward.amount >= 5000)
			{
				MissionsManager.Instance.PlayerDidThis(MissionTarget.MysteryBoxGrandPrize);
			}
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.BiggerJackpot);
			if (((celebrationReward.amount == 100000 && mysteryBoxType == Type.Normal) || (celebrationReward.amount == 300000 && mysteryBoxType == Type.Super)) && activeEventManager != null)
			{
				celebrationReward.amount = GetMegaJackpotAmount(mysteryBoxType);
			}
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.keys)
		{
			celebrationReward.rewardType = CelebrationRewardType.keys;
			celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.powerup)
		{
			celebrationReward.rewardType = CelebrationRewardType.powerup;
			celebrationReward.upgradeType = lootTableEntry.upgradeType;
			celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.token)
		{
			if (availableTokens.Count > 0)
			{
				celebrationReward.rewardType = CelebrationRewardType.token;
				celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
				celebrationReward.characterId = GetRandomCharacterTokenTypeUsingWeights(availableTokens);
			}
			else
			{
				switch (mysteryBoxType)
				{
				case Type.Normal:
					celebrationReward = MYSTERYBOX_CONSOLATION_REWARD;
					break;
				case Type.Super:
					celebrationReward = SUPERMYSTERYBOX_CONSOLATION_REWARD;
					break;
				case Type.Token:
					celebrationReward = TOKENMYSTERYBOX_CONSOLATION_REWARD;
					break;
				}
			}
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.trophy)
		{
			if (availableTrophyList.Count <= 0)
			{
				celebrationReward = ((mysteryBoxType != Type.Normal) ? SUPERMYSTERYBOX_CONSOLATION_REWARD : MYSTERYBOX_CONSOLATION_REWARD);
			}
			else
			{
				celebrationReward.rewardType = CelebrationRewardType.trophy;
				celebrationReward.amount = UnityEngine.Random.Range(lootTableEntry.min, lootTableEntry.max);
				celebrationReward.trophyType = availableTrophyList[UnityEngine.Random.Range(0, availableTrophyList.Count)];
			}
		}
		else if (lootTableEntry.itemType == CelebrationRewardType.medal)
		{
			celebrationReward.rewardType = CelebrationRewardType.medal;
			celebrationReward.amount = 1;
		}
		return celebrationReward;
	}

	public static int GetMegaJackpotAmount(Type mysteryBoxType)
	{
		int num = 0;
		num = ((mysteryBoxType != Type.Normal) ? OnlineSettings.instance.GetValue("event_jackpot_supermysterybox_value", 1500000) : OnlineSettings.instance.GetValue("event_jackpot_mysterybox_value", 900000));
		if (num <= 0)
		{
			num = ((mysteryBoxType != Type.Normal) ? 1500000 : 900000);
		}
		return num;
	}

	public static LootTableEntry MakeRoll(Type mysteryBoxType)
	{
		float num = 0f;
		LootTableEntry[] array = _lootTables[(int)mysteryBoxType];
		foreach (LootTableEntry lootTableEntry in array)
		{
			num += lootTableEntry.weight;
		}
		float num2 = UnityEngine.Random.Range(0f, 1f) * num;
		float num3 = 0f;
		LootTableEntry result = null;
		foreach (LootTableEntry lootTableEntry2 in array)
		{
			num3 += lootTableEntry2.weight;
			if (num2 <= num3)
			{
				result = lootTableEntry2;
				break;
			}
		}
		return result;
	}

	private static Characters.CharacterId GetRandomCharacterTokenTypeUsingWeights(List<Characters.CharacterId> validCharacters)
	{
		Characters.CharacterId result = Characters.CharacterId.tricky;
		int num = 0;
		foreach (Characters.CharacterId validCharacter in validCharacters)
		{
			num += characterTokenWeight[validCharacter];
		}
		int num2 = new System.Random().Next(0, num) + 1;
		int num3 = 0;
		for (int i = 0; i < validCharacters.Count; i++)
		{
			num3 += characterTokenWeight[validCharacters[i]];
			if (num2 <= num3)
			{
				result = validCharacters[i];
				break;
			}
		}
		return result;
	}

	private static List<Trophies.Trophy> GetAvailableTrophyList(Type mysteryBoxType)
	{
		Trophies.Trophy[] array = Enum.GetValues(typeof(Trophies.Trophy)) as Trophies.Trophy[];
		List<Trophies.Trophy> list = new List<Trophies.Trophy>();
		Trophies.Trophy[] array2 = array;
		foreach (Trophies.Trophy trophy in array2)
		{
			TrophyData value;
			if (Trophies.ShouldSpawnTrophy(trophy) && Trophies.TROPHY_DATA.TryGetValue(trophy, out value) && value.obtainSource == mysteryBoxType)
			{
				list.Add(trophy);
			}
		}
		return list;
	}

	public static bool AreTokensAvailable()
	{
		return GetAvailableTokens().Count > 0;
	}

	private static List<Characters.CharacterId> GetAvailableTokens()
	{
		List<Characters.CharacterId> list = new List<Characters.CharacterId>();
		Characters.CharacterId[] tOKEN_REWARD_IDS = TOKEN_REWARD_IDS;
		foreach (Characters.CharacterId characterId in tOKEN_REWARD_IDS)
		{
			if (CharacterManager.IsTokenUseful(characterId))
			{
				list.Add(characterId);
			}
		}
		return list;
	}

	public static Multipliers GetMultiplierForReward(CelebrationReward rewardToMultiply)
	{
		Type type;
		switch (rewardToMultiply.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			type = Type.Normal;
			break;
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
		case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			type = Type.Super;
			break;
		case CelebrationRewardOrigin.MysteryBoxMini:
			type = Type.Mini;
			break;
		default:
			return Multipliers.NotSet;
		}
		LootTableEntry[] array = _lootTables[(int)type];
		if (array == null)
		{
			return Multipliers.NotSet;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].itemType == rewardToMultiply.rewardType && array[i].min <= rewardToMultiply.amount && array[i].max >= rewardToMultiply.amount)
			{
				if (array[i].itemType != CelebrationRewardType.powerup)
				{
					return array[i].multiplier;
				}
				if (array[i].upgradeType == rewardToMultiply.upgradeType)
				{
					return array[i].multiplier;
				}
			}
		}
		return Multipliers.x2;
	}
}
