using System.Collections.Generic;
using System.Linq;
using Data.Managers;
using SYBO.Subway.Info;
using UnityEngine;

namespace Info
{
	public class Outfits : ScriptableObject
	{
		private static Outfits _instance;

		public OutfitSet[] outfitSets;

		public static Outfits instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<Outfits>("Info/Outfits");
				}
				return _instance;
			}
		}

		public static List<Outfit> TryGetOutfitsForCharacter(Characters.CharacterId character)
		{
			List<Outfit> list = null;
			OutfitSet[] array = instance.outfitSets;
			foreach (OutfitSet outfitSet in array)
			{
				if (outfitSet.characterId == character)
				{
					if (list == null)
					{
						list = new List<Outfit>();
					}
					list.AddRange(outfitSet.outfitList);
				}
			}
			return list;
		}

		public static Outfit GetOutfitForCharacter(Characters.CharacterId character, int outfitIndex)
		{
			if (outfitIndex == 0)
			{
				return null;
			}
			List<Outfit> list = TryGetOutfitsForCharacter(character);
			if (list != null && list.Count >= outfitIndex)
			{
				return list[outfitIndex - 1];
			}
			return null;
		}

		public static int GetOutfitRequirementIndex(Characters.CharacterId character, int outfitIndex)
		{
			int result = -1;
			Outfit outfitForCharacter = GetOutfitForCharacter(character, outfitIndex);
			if (outfitForCharacter != null && outfitForCharacter.unlockType == PurchaseData.UnlockType.Tokens)
			{
				List<Outfit> list = TryGetOutfitsForCharacter(character);
				for (int num = outfitForCharacter.uiPriority - 1; num >= 0; num--)
				{
					for (int i = 0; i < list.Count; i++)
					{
						Outfit outfit = list[i];
						if (outfit.uiPriority == num && outfit.unlockType == PurchaseData.UnlockType.Tokens)
						{
							return i;
						}
					}
				}
			}
			return result;
		}

		public static int GetNextTokenUnlockableOutfitIndex(Characters.CharacterId characterId)
		{
			List<Outfit> list = TryGetOutfitsForCharacter(characterId);
			if (!CharacterManager.IsCharacterOwned(characterId) || list == null)
			{
				return -1;
			}
			int result = -1;
			int num = int.MaxValue;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].unlockType == PurchaseData.UnlockType.Tokens && list[i].uiPriority < num && !CharacterManager.IsOutfitUnlocked(characterId, i + 1))
					{
						num = list[i].uiPriority;
						result = i;
					}
				}
			}
			return result;
		}

		public static bool HasCharacter(Characters.CharacterId key)
		{
			return instance.outfitSets.Any((OutfitSet outfitSet) => outfitSet.characterId == key);
		}

		public static int GetThemeCount(Characters.CharacterId character)
		{
			List<Outfit> list = TryGetOutfitsForCharacter(character);
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}
	}
}
