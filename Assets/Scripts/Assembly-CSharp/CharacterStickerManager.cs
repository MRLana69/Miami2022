using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using Info;
using SYBO.Subway.Info;

public static class CharacterStickerManager
{
	public static bool ShouldShowCharacterThemesStickerFlow()
	{
		return OnboardingSave.ShouldShowStickersForCharacterThemes();
	}

	public static bool IsAnyCharacterWithFreeTheme()
	{
		foreach (Characters.CharacterId item in Characters.characterOrder)
		{
			List<Outfit> list = Outfits.TryGetOutfitsForCharacter(item);
			if (list == null || list.Count <= 0)
			{
				continue;
			}
			int num = 1;
			foreach (Outfit item2 in list)
			{
				if (item2.price == 0 && !CharacterManager.IsOutfitUnlocked(item, num) && !IsLimitedCharacter(item))
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public static bool HasCharacterFreeTheme(Characters.CharacterId id)
	{
		List<Outfit> list = Outfits.TryGetOutfitsForCharacter(id);
		if (list != null && list.Count > 0)
		{
			int num = 1;
			foreach (Outfit item in list)
			{
				if (item.price == 0 && !CharacterManager.IsOutfitUnlocked(id, num) && !IsLimitedCharacter(id))
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public static bool HasCharacterNewTheme(Characters.CharacterId id)
	{
		List<Outfit> list = Outfits.TryGetOutfitsForCharacter(id);
		if (list != null && list.Count > 0)
		{
			int num = 1;
			foreach (Outfit item in list)
			{
				if (item.isNewInThisUpdate && !HasThemeBeenSeenForCharacter(id, num))
				{
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public static int GetNumberOfNewCharactersOrThemes()
	{
		int num = 0;
		if (!UserSave.isFreshInstall)
		{
			foreach (Characters.CharacterId item in Characters.characterOrder)
			{
				if (!CharacterManager.IsCharacterAvailable(item))
				{
					continue;
				}
				Characters.Model model = Characters.characterData[item];
				if (model.isNewInThisUpdate && !CharacterManager.HasCharacterBeenSeen(item) && !CharacterManager.IsCharacterOwned(item))
				{
					num++;
				}
				List<Outfit> list = Outfits.TryGetOutfitsForCharacter(item);
				if (list == null || list.Count <= 0)
				{
					continue;
				}
				int num2 = 1;
				foreach (Outfit item2 in list)
				{
					if (item2.isNewInThisUpdate && !HasThemeBeenSeenForCharacter(item, num2))
					{
						num++;
					}
					num2++;
				}
			}
		}
		return num;
	}

	public static bool HasThemeBeenSeenForCharacter(Characters.CharacterId id, int index)
	{
		return UserSave.IsCharacterOutfitSeen(id, index);
	}

	public static bool IsThemeUnlockedForCharacter(Characters.CharacterId id, int index)
	{
		return CharacterManager.IsOutfitUnlocked(id, index);
	}

	public static bool HasCharacterBeenSeen(Characters.CharacterId id)
	{
		return CharacterManager.HasCharacterBeenSeen(id);
	}

	public static bool IsCharacterUnlocked(Characters.CharacterId id)
	{
		return CharacterManager.IsCharacterOwned(id);
	}

	private static bool IsLimitedCharacter(Characters.CharacterId characterId)
	{
		bool result = false;
		if (Characters.characterData.ContainsKey(characterId))
		{
			result = Characters.characterData[characterId].hasOnlineSettings;
		}
		return result;
	}

	public static bool IsCharacterUnlocked(Characters.CharacterId id, Characters.Model model)
	{
		return CharacterManager.IsCharacterOwned(id, model);
	}
}
