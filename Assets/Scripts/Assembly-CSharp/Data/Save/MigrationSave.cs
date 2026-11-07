using System;
using System.Collections.Generic;
using System.Linq;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Subway.Info;
using UI.Screens.RewardManagement;

namespace Data.Save
{
	public class MigrationSave : DataManager<MigrationSave>
	{
		private const char DATA_CHUNK_SEPARATOR = ';';

		private const char DATA_ASSIGNATION_SEPARATOR = '=';

		private const char DATA_VALUE_GROUP_SEPARATOR = ',';

		private const char DATA_VALUE_SEPARATOR = ':';

		private const char DATA_THEME_IDENTIFIER_SEPARATOR = '#';

		public static void MigrateUnlockedOutfits(Dictionary<Characters.CharacterId, int[]> migrateDict)
		{
			Dictionary<Characters.CharacterId, List<int>> dictionary = new Dictionary<Characters.CharacterId, List<int>>();
			foreach (KeyValuePair<Characters.CharacterId, int[]> item in migrateDict)
			{
				dictionary.Add(item.Key, item.Value.ToList());
			}
			DataManager<MigrationSave>.cloud.unlockedOutfits = dictionary;
		}

		public static void MigrateCharacters(int[] characterTokens)
		{
			for (int i = 0; i < Enum.GetValues(typeof(Characters.CharacterId)).Length; i++)
			{
				Characters.CharacterId characterId = (Characters.CharacterId)i;
				Characters.Model model = Characters.characterData[characterId];
				if (model.unlockType == PurchaseData.UnlockType.Tokens)
				{
					if (i < characterTokens.Length)
					{
						DataManager<MigrationSave>.cloud.collectedCharacterTokens[characterId] = characterTokens[i];
					}
					else
					{
						DataManager<MigrationSave>.cloud.collectedCharacterTokens[characterId] = 0;
					}
				}
				if (i < characterTokens.Length && model.Price <= characterTokens[i])
				{
					DataManager<MigrationSave>.cloud.unlockedCharacters.AddDistinct(characterId);
				}
			}
		}

		public static void MigrateBoards(string unlockedHoverboards)
		{
			if (!string.IsNullOrEmpty(unlockedHoverboards))
			{
				if (!unlockedHoverboards.Contains(Boards.BoardId.kitty.ToString()))
				{
					unlockedHoverboards = unlockedHoverboards.Replace("fortune", Boards.BoardId.kitty.ToString());
				}
				if (!unlockedHoverboards.Contains(Boards.BoardId.flamingo.ToString()))
				{
					unlockedHoverboards = unlockedHoverboards.Replace("miaminewboard", Boards.BoardId.flamingo.ToString());
				}
			}
			Dictionary<Boards.BoardId, bool> dictionary = Globals.convertStringToEnumBoolDictionary<Boards.BoardId>(unlockedHoverboards);
			foreach (KeyValuePair<Boards.BoardId, bool> item in dictionary)
			{
				if (item.Value)
				{
					DataManager<MigrationSave>.cloud.unlockedBoards.AddDistinct(item.Key);
				}
			}
		}

		public static CollectCoinsDummy[] ParseDummyData(string rawData)
		{
			Dictionary<int, CollectCoinsDummy> dictionary = new Dictionary<int, CollectCoinsDummy>();
			if (!string.IsNullOrEmpty(rawData))
			{
				string[] array = rawData.Split(';');
				int num = 0;
				string[] array2 = array;
				foreach (string rawDummyData in array2)
				{
					CollectCoinsDummy dummy;
					if (TryParseDummy(rawDummyData, out dummy))
					{
						dictionary.Add(num, dummy);
						num++;
						continue;
					}
					return null;
				}
				CollectCoinsDummy[] array3 = new CollectCoinsDummy[dictionary.Count];
				CollectCoinsDummy[] defaultDummyList = CollectCoinsDummyData.GetDefaultDummyList();
				bool flag = array3.Length == defaultDummyList.Length;
				for (int j = 0; j < dictionary.Count; j++)
				{
					array3[j] = dictionary[j];
					if (flag)
					{
						array3[j].name = defaultDummyList[j].name;
					}
				}
				return array3;
			}
			return null;
		}

		private static bool TryParseDummy(string rawDummyData, out CollectCoinsDummy dummy)
		{
			string[] array = rawDummyData.Split(':');
			if (array.Length == 4)
			{
				dummy = null;
				return false;
			}
			if (array.Length == 3)
			{
				try
				{
					dummy = new CollectCoinsDummy
					{
						hasBeenCollected = bool.Parse(array[1]),
						fakeProgress = int.Parse(array[2])
					};
					if (dummy.fakeProgress > 25)
					{
						dummy.fakeProgress -= 25;
					}
					return true;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			SDebug.LogError("Unexpected number of parameters: " + rawDummyData + ". Aborting parsing.");
			dummy = null;
			return false;
		}

		public static void SetPowerups(Dictionary<UpgradeType, int> powerups)
		{
			DataManager<MigrationSave>.cloud.powerups = powerups;
		}

		public static void SetUpgrades(Dictionary<UpgradeType, int> upgrades)
		{
			DataManager<MigrationSave>.cloud.upgrades = upgrades;
		}

		public static void MigratePendingRewards(string rewardsAsString)
		{
			if (string.IsNullOrEmpty(rewardsAsString))
			{
				return;
			}
			List<CelebrationReward> list = new List<CelebrationReward>();
			string[] array = rewardsAsString.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					CelebrationReward celebrationReward = new CelebrationReward();
					celebrationReward.PopulateFromString(text);
					list.Add(celebrationReward);
				}
			}
			DataManager<MigrationSave>.cloud.pendingRewards = list;
		}

		public static void MigrateBoardThemeData(string data)
		{
			Dictionary<Boards.BoardId, Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>> dictionary = new Dictionary<Boards.BoardId, Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>>();
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(';');
				string[] array2 = array;
				foreach (string text in array2)
				{
					try
					{
						int num = text.IndexOf('=');
						string value = text.Substring(0, num);
						string rawValues = text.Substring(num + 1);
						dictionary.Add((Boards.BoardId)Enum.Parse(typeof(Boards.BoardId), value, true), GetCustomizationValues(rawValues));
					}
					catch (Exception ex)
					{
						SDebug.LogError("HoverboardCustomizationData parse failed for item ERROR = " + ex.ToString());
					}
				}
			}
			DataManager<MigrationSave>.cloud.boardThemeData = dictionary;
		}

		private static Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> GetCustomizationValues(string rawValues)
		{
			Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary = new Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>();
			if (!string.IsNullOrEmpty(rawValues))
			{
				string[] array = rawValues.Split(',');
				int num = array.Length;
				if (num > 0 && num <= 2)
				{
					for (int i = 0; i < num; i++)
					{
						string[] array2 = array[i].Split('#');
						string value = array2[0];
						string[] array3 = array2[1].Split(':');
						if (array3.Length == 3)
						{
							BoardThemeData boardThemeData = new BoardThemeData();
							boardThemeData.isOwned = bool.Parse(array3[0]);
							boardThemeData.isActive = bool.Parse(array3[1]);
							boardThemeData.hasBeenSeen = bool.Parse(array3[2]);
							BoardThemeData value2 = boardThemeData;
							dictionary.Add((HoverboardThemes.ThemeIdentifier)Enum.Parse(typeof(HoverboardThemes.ThemeIdentifier), value), value2);
						}
						else
						{
							SDebug.LogError("Incorrect number of values for a specific theme.");
						}
					}
					return dictionary;
				}
				SDebug.LogError("Incorrect number of themes for a specific board.");
			}
			return null;
		}

		public static Dictionary<EarnCoinsData.Id, string> MigrateEarnCurrencyData(string rawList)
		{
			string[] separator = new string[2] { "=", ";" };
			Dictionary<EarnCoinsData.Id, string> dictionary = new Dictionary<EarnCoinsData.Id, string>();
			if (!string.IsNullOrEmpty(rawList))
			{
				string[] array = rawList.Split(separator, StringSplitOptions.None);
				for (int i = 0; i < array.Length - 1; i++)
				{
					string value = array[i];
					string value2 = array[i + 1];
					if (Enum.IsDefined(typeof(EarnCoinsData.Id), value))
					{
						EarnCoinsData.Id key = (EarnCoinsData.Id)Enum.Parse(typeof(EarnCoinsData.Id), value, true);
						dictionary[key] = value2;
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<AwardId, PersistentAwardProgress> MigrateAwardProgress(string rawProgress)
		{
			Dictionary<AwardId, PersistentAwardProgress> dictionary = new Dictionary<AwardId, PersistentAwardProgress>();
			if (!string.IsNullOrEmpty(rawProgress))
			{
				string[] array = rawProgress.Split(';');
				string[] array2 = array;
				foreach (string text in array2)
				{
					try
					{
						int num = text.IndexOf('=');
						string value = text.Substring(0, num);
						string val = text.Substring(num + 1);
						PersistentAwardProgress value2 = ParseAwardProgress(val);
						dictionary.Add((AwardId)Enum.Parse(typeof(AwardId), value, true), value2);
					}
					catch (Exception)
					{
					}
				}
			}
			return dictionary;
		}

		private static PersistentAwardProgress ParseAwardProgress(string val)
		{
			try
			{
				string[] array = val.Split(',');
				if (array.Length >= 5)
				{
					PersistentAwardProgress persistentAwardProgress = new PersistentAwardProgress();
					persistentAwardProgress.activeTierType = (AwardTierType)Enum.Parse(typeof(AwardTierType), array[0], true);
					persistentAwardProgress.currentActiveAwardState = (AwardState)Enum.Parse(typeof(AwardState), array[1], true);
					persistentAwardProgress.progressTierType = (AwardTierType)Enum.Parse(typeof(AwardTierType), array[2], true);
					persistentAwardProgress.currentProgressAwardState = (AwardState)Enum.Parse(typeof(AwardState), array[3], true);
					persistentAwardProgress.lastActiveStateChangeDateTime = Utils.StringToDateTime(array[4], DateTime.MinValue);
					PersistentAwardProgress persistentAwardProgress2 = persistentAwardProgress;
					if (array.Length >= 6)
					{
						int result;
						if (int.TryParse(array[5], out result))
						{
							persistentAwardProgress2.offset = result;
						}
						else
						{
							string[] array2 = array[5].Split('%');
							string[] array3 = array2;
							foreach (string text in array3)
							{
								int num = text.IndexOf('#');
								string value = text.Substring(0, num);
								int value2 = int.Parse(text.Substring(num + 1));
								Stat key = (Stat)Enum.Parse(typeof(Stat), value, true);
								if (!persistentAwardProgress2.statOffset.ContainsKey(key))
								{
									persistentAwardProgress2.statOffset.Add(key, value2);
								}
								else
								{
									persistentAwardProgress2.statOffset[key] = value2;
								}
							}
							if (array.Length >= 7)
							{
								persistentAwardProgress2.hasMigratedToUseStatOffset = bool.Parse(array[6]);
							}
						}
					}
					return persistentAwardProgress2;
				}
			}
			catch
			{
				return new PersistentAwardProgress();
			}
			return new PersistentAwardProgress();
		}
	}
}
