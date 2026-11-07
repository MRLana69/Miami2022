using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace Data.Managers
{
	public static class CharacterManager
	{
		private static readonly CultureInfo DATETIME_CULTUREINFO = new CultureInfo("da-DK");

		private static List<KeyValuePair<Characters.CharacterId, Characters.Model>> _characterList = null;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Characters.CharacterId, bool> characterUnlocked__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Characters.CharacterId> characterTokenCollected__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Characters.CharacterId, int> outfitUnlocked__BackingField;

		public static event Action<Characters.CharacterId, bool> characterUnlocked
		{
			add
			{
				Action<Characters.CharacterId, bool> action = characterUnlocked__BackingField;
				Action<Characters.CharacterId, bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterUnlocked__BackingField, (Action<Characters.CharacterId, bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Characters.CharacterId, bool> action = characterUnlocked__BackingField;
				Action<Characters.CharacterId, bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterUnlocked__BackingField, (Action<Characters.CharacterId, bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<Characters.CharacterId> characterTokenCollected
		{
			add
			{
				Action<Characters.CharacterId> action = characterTokenCollected__BackingField;
				Action<Characters.CharacterId> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterTokenCollected__BackingField, (Action<Characters.CharacterId>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Characters.CharacterId> action = characterTokenCollected__BackingField;
				Action<Characters.CharacterId> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref characterTokenCollected__BackingField, (Action<Characters.CharacterId>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<Characters.CharacterId, int> outfitUnlocked
		{
			add
			{
				Action<Characters.CharacterId, int> action = outfitUnlocked__BackingField;
				Action<Characters.CharacterId, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref outfitUnlocked__BackingField, (Action<Characters.CharacterId, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Characters.CharacterId, int> action = outfitUnlocked__BackingField;
				Action<Characters.CharacterId, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref outfitUnlocked__BackingField, (Action<Characters.CharacterId, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static void Init()
		{
			AppRefreshHandler.hasRefreshed += OnHasRefreshed;
			CheckResetTokensForUnlockedCharacters();
		}

		private static void OnHasRefreshed(RefreshType type)
		{
			if (type == RefreshType.Data)
			{
				CheckResetTokensForUnlockedCharacters();
			}
		}

		public static int AmountOfUsefulTokens(Characters.CharacterId characterId)
		{
			int num = 0;
			if (!IsCharacterOwned(characterId))
			{
				num += Characters.characterData[characterId].Price;
			}
			List<Outfit> list = Outfits.TryGetOutfitsForCharacter(characterId);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (!IsOutfitUnlocked(characterId, i + 1) && list[i].unlockType == PurchaseData.UnlockType.Tokens)
					{
						num += list[i].price;
					}
				}
			}
			return num - GetCollectedTokens(characterId);
		}

		public static bool IsTokenUseful(Characters.CharacterId characterId, CelebrationReward currentReward = null, bool isDoubling = false)
		{
			int num = AmountOfUsefulTokens(characterId);
			int num2 = 0;
			for (int i = 0; i < InventorySave.pendingRewards.Count; i++)
			{
				if (InventorySave.pendingRewards[i].rewardType == CelebrationRewardType.token && InventorySave.pendingRewards[i].characterId == characterId)
				{
					num2 += InventorySave.pendingRewards[i].amount;
				}
			}
			if (num <= num2)
			{
				return false;
			}
			if (num > 0)
			{
				return true;
			}
			return false;
		}

		public static void UnlockCharacter(Characters.CharacterId characterId, bool unlockedByRestore = false)
		{
			InventorySave.unlockedCharacters.AddDistinct(characterId);
			string argValue = Strings.Get(Characters.characterData[characterId].name).ToLower();
			Flurry.LogEventWithAParameter("Character unlocked", "Id", argValue);
			characterUnlocked__BackingField.SafeInvoke(characterId, unlockedByRestore);
		}

		private static void CheckResetTokensForUnlockedCharacters()
		{
			if (UserSave.hasResetTokensForUnlockedCharacters)
			{
				return;
			}
			for (int i = 0; i < InventorySave.unlockedCharacters.Count; i++)
			{
				Characters.CharacterId key = InventorySave.unlockedCharacters[i];
				Characters.Model model = Characters.characterData[key];
				if (InventorySave.collectedCharacterTokens.ContainsKey(key) && model.unlockType == PurchaseData.UnlockType.Tokens)
				{
					InventorySave.collectedCharacterTokens[key] = 0;
				}
			}
			UserSave.hasResetTokensForUnlockedCharacters = true;
		}

		public static void LockCharacter(Characters.CharacterId characterId)
		{
			InventorySave.unlockedCharacters.Remove(characterId);
		}

		public static void CollectToken(Characters.CharacterId characterId, int amount)
		{
			if (!InventorySave.collectedCharacterTokens.ContainsKey(characterId))
			{
				InventorySave.collectedCharacterTokens.Add(characterId, amount);
			}
			else
			{
				InventorySave.collectedCharacterTokens[characterId] += amount;
			}
			characterTokenCollected__BackingField.SafeInvoke(characterId);
			HandleUnlockForToken(characterId);
		}

		private static void HandleUnlockForToken(Characters.CharacterId characterId)
		{
			int value;
			if (!InventorySave.collectedCharacterTokens.TryGetValue(characterId, out value) || value <= 0)
			{
				return;
			}
			if (!IsCharacterOwned(characterId))
			{
				if (Globals.DEBUG_ALL_CHARS)
				{
					UnlockCharacter(characterId);
				}
				Characters.Model model = Characters.characterData[characterId];
				if (model.unlockType == PurchaseData.UnlockType.AlwaysUnlocked)
				{
					UnlockCharacter(characterId);
				}
				if (value >= model.Price)
				{
					UnlockCharacter(characterId);
					InventorySave.collectedCharacterTokens[characterId] -= Characters.characterData[characterId].Price;
					HandleUnlockForToken(characterId);
				}
				return;
			}
			List<Outfit> list = Outfits.TryGetOutfitsForCharacter(characterId);
			if (list != null)
			{
				int nextTokenUnlockableOutfitIndex = Outfits.GetNextTokenUnlockableOutfitIndex(characterId);
				if (nextTokenUnlockableOutfitIndex >= 0 && value >= list[nextTokenUnlockableOutfitIndex].price)
				{
					UnlockOutfit(characterId, nextTokenUnlockableOutfitIndex + 1);
					InventorySave.collectedCharacterTokens[characterId] -= list[nextTokenUnlockableOutfitIndex].price;
					HandleUnlockForToken(characterId);
				}
			}
		}

		public static bool IsCharacterOwned(Characters.CharacterId characterId)
		{
			if (Globals.DEBUG_ALL_CHARS)
			{
				return true;
			}
			Characters.Model model = Characters.characterData[characterId];
			if (model.unlockType == PurchaseData.UnlockType.AlwaysUnlocked)
			{
				return true;
			}
			return InventorySave.unlockedCharacters.Contains(characterId);
		}

		public static bool IsCharacterOwned(Characters.CharacterId characterId, Characters.Model characterModel)
		{
			if (Globals.DEBUG_ALL_CHARS)
			{
				return true;
			}
			if (characterModel.unlockType == PurchaseData.UnlockType.AlwaysUnlocked)
			{
				return true;
			}
			for (int i = 0; i < InventorySave.unlockedCharacters.Count; i++)
			{
				if (InventorySave.unlockedCharacters[i] == characterId)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetCollectedTokens(Characters.CharacterId characterId)
		{
			int value;
			if (InventorySave.collectedCharacterTokens.TryGetValue(characterId, out value))
			{
				return value;
			}
			return 0;
		}

		public static int GetOutfitIndex(Characters.CharacterId characterId)
		{
			int value;
			if (InventorySave.selectedOutfits.TryGetValue(characterId, out value))
			{
				List<Outfit> list = Outfits.TryGetOutfitsForCharacter(characterId);
				if (list != null && list.Count >= value)
				{
					return value;
				}
			}
			return 0;
		}

		public static void SetOutfitSelectionIndex(Characters.CharacterId character, int outfitIndex)
		{
			if (Application.isEditor)
			{
				List<Outfit> list = Outfits.TryGetOutfitsForCharacter(character);
				if (list != null && (list.Count < outfitIndex || outfitIndex < 0))
				{
					SDebug.LogError("The outift index is too great " + outfitIndex + " for character: " + character);
				}
			}
			if (InventorySave.selectedOutfits.ContainsKey(character))
			{
				InventorySave.selectedOutfits.Remove(character);
			}
			if (outfitIndex > 0)
			{
				InventorySave.selectedOutfits.Add(character, outfitIndex);
			}
		}

		public static void UnlockOutfit(Characters.CharacterId character, int outfitIndex)
		{
			if (!InventorySave.unlockedOutfits.ContainsKey(character))
			{
				InventorySave.unlockedOutfits.Add(character, new List<int>());
			}
			InventorySave.unlockedOutfits[character].AddDistinct(outfitIndex);
			outfitUnlocked__BackingField.SafeInvoke(character, outfitIndex);
		}

		public static bool IsOutfitUnlocked(Characters.CharacterId character, int outfitIndex)
		{
			if (Globals.DEBUG_ALL_CHARS)
			{
				return true;
			}
			if (outfitIndex == 0)
			{
				return true;
			}
			List<int> value;
			InventorySave.unlockedOutfits.TryGetValue(character, out value);
			return value != null && value.Any((int t) => t == outfitIndex);
		}

		public static bool HasCharacterBeenSeen(Characters.CharacterId characterId)
		{
			if (!Characters.characterData[characterId].isNewInThisUpdate)
			{
				return true;
			}
			if (UserSave.hasCharacterBeenSeen.ContainsKey(characterId))
			{
				return UserSave.hasCharacterBeenSeen[characterId];
			}
			return false;
		}

		public static void MarkCharacterAsSeen(Characters.CharacterId characterId)
		{
			if (UserSave.hasCharacterBeenSeen.ContainsKey(characterId))
			{
				UserSave.hasCharacterBeenSeen[characterId] = true;
			}
			else
			{
				UserSave.hasCharacterBeenSeen.Add(characterId, true);
			}
		}

		private static DateTime DefaultExpirationDateForCharacter(Characters.CharacterId cId)
		{
			Characters.Model model = Characters.characterData[cId];
			if (model.defaultExpirationDate != DateTime.MinValue)
			{
				return model.defaultExpirationDate;
			}
			return DateTime.MaxValue;
		}

		public static DateTime CharacterExpirationDate(Characters.CharacterId characterId)
		{
			string key = "character_expiretime_" + Characters.characterData[characterId].modelName;
			DateTime result = DateTime.MinValue;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				result = DefaultExpirationDateForCharacter(characterId);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
			{
				try
				{
					result = DateTime.Parse(valueString, DATETIME_CULTUREINFO);
				}
				catch (FormatException)
				{
					result = DefaultExpirationDateForCharacter(characterId);
				}
			}
			return result;
		}

		public static DateTime CharacterStartAvailabilityDate(Characters.CharacterId characterId)
		{
			Characters.Model model = Characters.characterData[characterId];
			string key = "character_starttime_" + model.modelName;
			DateTime result = DateTime.MinValue;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				result = ((!(model.defaultStartDate != DateTime.MinValue)) ? DateTime.MinValue : model.defaultStartDate);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
			{
				try
				{
					result = DateTime.Parse(valueString, DATETIME_CULTUREINFO);
				}
				catch (FormatException)
				{
				}
			}
			return result;
		}

		public static bool IsCharacterAvailable(Characters.CharacterId characterId)
		{
			if (Globals.DEBUG_ALL_CHARS || Globals.DEBUG_DISPLAY_ALL_CHARS)
			{
				return true;
			}
			Characters.Model model = Characters.characterData[characterId];
			if (!model.hasOnlineSettings)
			{
				return true;
			}
			string key = "character_expiretime_" + model.modelName;
			string valueString;
			if (OnlineSettings.instance.HasValue(key) && OnlineSettings.instance.TryGetValue(key, out valueString) && valueString == "disabled")
			{
				return false;
			}
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType eventType = activeEventManager.GetEventType();
				if (eventType == SSEventType.ExpiredLimitedCharacter || eventType == SSEventType.CharacterDay)
				{
					bool found = false;
					Characters.CharacterId eventValue = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
					if (eventValue == characterId)
					{
						return true;
					}
				}
			}
			DateTime t = CharacterStartAvailabilityDate(characterId);
			DateTime t2 = CharacterExpirationDate(characterId);
			if (DateTime.Compare(t2, CDateTime.Now) > 0 && DateTime.Compare(t, CDateTime.Now) < 0)
			{
				return true;
			}
			return false;
		}

		public static TimeSpan CharacterExpirationTimespan(Characters.CharacterId characterId)
		{
			string key = "character_expiretime_" + characterId;
			DateTime dateTime = CDateTime.Now;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				dateTime = ((!(Characters.characterData[characterId].defaultExpirationDate != DateTime.MinValue)) ? DateTime.MaxValue : Characters.characterData[characterId].defaultExpirationDate);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString))
			{
				dateTime = ((!(valueString == "disabled")) ? CharacterExpirationDate(characterId) : CDateTime.Now);
			}
			if (dateTime == DateTime.MinValue)
			{
				dateTime = CDateTime.Now;
			}
			return dateTime - CDateTime.Now;
		}

		public static List<KeyValuePair<Characters.CharacterId, Characters.Model>> GetAvailableCharacters(bool forceRefresh = false)
		{
			if (_characterList == null || _characterList.Count == 0 || forceRefresh)
			{
				_characterList = GenerateAvailableCharacters();
			}
			return _characterList;
		}

		// Coloca no save todos os personagens e trajes do jogo, para ficarem desbloqueados de verdade,
		// e não só pela opção DEBUG_ALL_CHARS. É salvo no próximo save do jogo.
		private static void GjUnlockAllInSave()
		{
			foreach (Characters.CharacterId item in Characters.characterOrder)
			{
				if (!InventorySave.unlockedCharacters.Contains(item))
				{
					InventorySave.unlockedCharacters.Add(item);
				}
				int themes = Outfits.GetThemeCount(item);
				if (themes <= 0)
				{
					continue;
				}
				List<int> owned;
				if (!InventorySave.unlockedOutfits.TryGetValue(item, out owned))
				{
					owned = new List<int>();
					InventorySave.unlockedOutfits[item] = owned;
				}
				for (int i = 1; i <= themes; i++)
				{
					if (!owned.Contains(i))
					{
						owned.Add(i);
					}
				}
			}
		}

		private static List<KeyValuePair<Characters.CharacterId, Characters.Model>> GenerateAvailableCharacters()
		{
			try
			{
				GjUnlockAllInSave();
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogWarning("GjUnlockAllInSave: " + e.Message);
			}
			List<KeyValuePair<Characters.CharacterId, Characters.Model>> list = new List<KeyValuePair<Characters.CharacterId, Characters.Model>>();
			foreach (Characters.CharacterId item in Characters.characterOrder)
			{
				Characters.Model model = Characters.characterData[item];
				if ((IsCharacterOwned(item) || IsCharacterAvailable(item)) && (!AgeRestriction.isRestricted || model.unlockType != PurchaseData.UnlockType.Facebook || IsCharacterOwned(item)) && (model.unlockType != PurchaseData.UnlockType.HiddenUntilUnlocked || IsCharacterOwned(item)))
				{
					list.Add(new KeyValuePair<Characters.CharacterId, Characters.Model>(item, model));
				}
			}
			return list;
		}
	}
}
