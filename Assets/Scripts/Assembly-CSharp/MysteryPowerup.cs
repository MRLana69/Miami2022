using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using Info;
using SYBO.Subway;
using UnityEngine;

public class MysteryPowerup
{
	private const float randomBoardDuration = 15f;

	private const int randomBoardWeight = 22;

	private const string randomBoardIconName = "icon_random_board";

	private static readonly Dictionary<UpgradeType, CharacterModifier> mysteryPowerupCharacterModifiers = new Dictionary<UpgradeType, CharacterModifier>
	{
		{
			UpgradeType.hoverboard,
			Game.Instance.Modifiers.Hoverboard
		},
		{
			UpgradeType.doubleCoinsMysteryPowerup,
			Game.Instance.Modifiers.DoubleCoins
		},
		{
			UpgradeType.doubleJump,
			Game.Instance.Modifiers.DoubleJump
		},
		{
			UpgradeType.plusMultiplier,
			Game.Instance.Modifiers.PlusMultiplier
		}
	};

	private static readonly List<Boards.BoardId> excludedBoards = new List<Boards.BoardId>();

	private static CharacterModifier SelectRandomCharacterModifier()
	{
		CharacterModifier result = Game.Instance.Modifiers.DoubleCoins;
		List<Upgrade> list = new List<Upgrade>();
		List<UpgradeType> list2 = new List<UpgradeType>();
		int num = 0;
		foreach (UpgradeType key in mysteryPowerupCharacterModifiers.Keys)
		{
			Upgrade upgrade = Upgrades.upgrades[key];
			if (key == UpgradeType.hoverboard)
			{
				if (Hoverboard.Instance.isActive || Hoverboard.Instance.IsInCooldown)
				{
					continue;
				}
				num += 22;
			}
			else
			{
				num += upgrade.weight;
			}
			list.Add(upgrade);
			list2.Add(key);
		}
		int num2 = Random.Range(0, num);
		float num3 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			Upgrade upgrade2 = list[i];
			num3 = ((list2[i] != UpgradeType.hoverboard) ? (num3 + (float)upgrade2.weight) : (num3 + 22f));
			if ((float)num2 <= num3)
			{
				result = mysteryPowerupCharacterModifiers[list2[i]];
				break;
			}
		}
		return result;
	}

	public static string GetRandomBoardIconName()
	{
		return "icon_random_board";
	}

	public static CharacterModifier GetRandomModifier()
	{
		CharacterModifier characterModifier = null;
		characterModifier = ((!Globals.DEBUG_IS_HOVERBOARD_ALWAYS_REWARDED_FOR_MYSERY_POWERUP) ? SelectRandomCharacterModifier() : Game.Instance.Modifiers.Hoverboard);
		if (characterModifier == Game.Instance.Modifiers.Hoverboard)
		{
			Hoverboard.Instance.IsARewardFromMysteryPowerup = true;
		}
		return characterModifier;
	}

	public static List<Boards.BoardId> GetRandomBoardList()
	{
		List<Boards.BoardId> list = new List<Boards.BoardId>();
		foreach (Boards.BoardId item in Boards.boardOrder)
		{
			if ((BoardManager.IsBoardOwned(item) || BoardManager.IsBoardActive(item)) && !excludedBoards.Contains(item) && PlayerSave.currentBoardId != item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static float GetRandomHoverboardDuration()
	{
		return 15f;
	}
}
