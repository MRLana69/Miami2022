using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using InApp;
using Info;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UI.Screens.RewardManagement;

namespace Data.Save
{
	public class InventorySave : DataManager<InventorySave>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action coinsChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action keysChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<UpgradeType, int> powerupsChanged__BackingField;

		public static int keys
		{
			get
			{
				return DataManager<InventorySave>.cloud.obfuscatedKeys.EncryptDecryptXORValue();
			}
			private set
			{
				DataManager<InventorySave>.cloud.obfuscatedKeys = value.EncryptDecryptXORValue();
				keysChanged__BackingField.SafeInvoke();
			}
		}

		public static int coins
		{
			get
			{
				return DataManager<InventorySave>.cloud.obfuscatedCoins.EncryptDecryptXORValue();
			}
			private set
			{
				DataManager<InventorySave>.cloud.obfuscatedCoins = value.EncryptDecryptXORValue();
				coinsChanged__BackingField.SafeInvoke();
			}
		}

		public static int unrewardedCoins
		{
			get
			{
				return DataManager<InventorySave>.cloud.obfuscatedUnrewardedCoins.EncryptDecryptXORValue();
			}
			set
			{
				DataManager<InventorySave>.cloud.obfuscatedUnrewardedCoins = value.EncryptDecryptXORValue();
			}
		}

		public static List<CelebrationReward> pendingRewards
		{
			get
			{
				return DataManager<InventorySave>.cloud.pendingRewards;
			}
		}

		public static Dictionary<Boards.BoardId, Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>> boardThemeData
		{
			get
			{
				return DataManager<InventorySave>.cloud.boardThemeData;
			}
		}

		public static CelebrationReward lastAddedReward { get; private set; }

		public static Dictionary<Characters.CharacterId, int> selectedOutfits
		{
			get
			{
				return DataManager<InventorySave>.cloud.selectedOutfits;
			}
		}

		public static Dictionary<Characters.CharacterId, List<int>> unlockedOutfits
		{
			get
			{
				return DataManager<InventorySave>.cloud.unlockedOutfits;
			}
		}

		public static List<Characters.CharacterId> unlockedCharacters
		{
			get
			{
				return DataManager<InventorySave>.cloud.unlockedCharacters;
			}
		}

		public static Dictionary<Characters.CharacterId, int> collectedCharacterTokens
		{
			get
			{
				return DataManager<InventorySave>.cloud.collectedCharacterTokens;
			}
		}

		public static List<Boards.BoardId> unlockedBoards
		{
			get
			{
				return DataManager<InventorySave>.cloud.unlockedBoards;
			}
		}

		public static Dictionary<UpgradeType, int> upgrades
		{
			get
			{
				return DataManager<InventorySave>.cloud.upgrades;
			}
		}

		public static event Action coinsChanged
		{
			add
			{
				Action action = coinsChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref coinsChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = coinsChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref coinsChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action keysChanged
		{
			add
			{
				Action action = keysChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref keysChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = keysChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref keysChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<UpgradeType, int> powerupsChanged
		{
			add
			{
				Action<UpgradeType, int> action = powerupsChanged__BackingField;
				Action<UpgradeType, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref powerupsChanged__BackingField, (Action<UpgradeType, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<UpgradeType, int> action = powerupsChanged__BackingField;
				Action<UpgradeType, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref powerupsChanged__BackingField, (Action<UpgradeType, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static void SetKeys(int newAmount)
		{
			keys = newAmount;
		}

		public static void SetCoins(int newAmount)
		{
			coins = newAmount;
		}

		public static void IncreaseCoins(int amount, string sourceId)
		{
			coins += amount;
			LogEarnedEvent(amount, InAppItemModel.ItemType.Coins, sourceId);
		}

		public static void DecreaseCoins(int amount, string itemId, string secondaryItemId = "N/A")
		{
			coins -= amount;
			LogSpentEvent(amount, InAppItemModel.ItemType.Coins, itemId, secondaryItemId);
		}

		public static void IncreaseKeys(int amount, string sourceId)
		{
			keys += amount;
			LogEarnedEvent(amount, InAppItemModel.ItemType.Keys, sourceId);
		}

		public static void DecreaseKeys(int amount, string itemId, string secondaryItemId = "N/A")
		{
			LogSpentEvent(amount, InAppItemModel.ItemType.Keys, itemId, secondaryItemId);
			keys -= amount;
		}

		private static void LogEarnedEvent(int amount, InAppItemModel.ItemType itemType, string sourceId)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Amount", amount.ToString());
			dictionary.Add("currency_id", itemType.ToString());
			dictionary.Add("source_id", sourceId);
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("Soft currency earned", parameters);
		}

		private static void LogSpentEvent(int amount, InAppItemModel.ItemType itemType, string itemId, string secondaryItemId)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Amount", amount.ToString());
			dictionary.Add("currency_id", itemType.ToString());
			dictionary.Add("item_id", itemId);
			dictionary.Add("item_id_2", secondaryItemId);
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("Soft currency spent", parameters);
		}

		public static void RewardUnrewardedCoins()
		{
			if (unrewardedCoins > 0)
			{
				coins += unrewardedCoins;
				unrewardedCoins = 0;
			}
		}

		public static void Use(UpgradeType type)
		{
			if (DataManager<InventorySave>.cloud.powerups.ContainsKey(type) && DataManager<InventorySave>.cloud.powerups[type] > 0)
			{
				DataManager<InventorySave>.cloud.powerups[type]--;
				powerupsChanged__BackingField.SafeInvoke(type, DataManager<InventorySave>.cloud.powerups[type]);
			}
		}

		public static int GetAmount(UpgradeType type)
		{
			return DataManager<InventorySave>.cloud.powerups.ContainsKey(type) ? DataManager<InventorySave>.cloud.powerups[type] : 0;
		}

		public static int GetTier(UpgradeType type)
		{
			return upgrades.ContainsKey(type) ? upgrades[type] : 0;
		}

		public static void IncreaseAmount(UpgradeType type, int amount)
		{
			if (DataManager<InventorySave>.cloud.powerups.ContainsKey(type))
			{
				DataManager<InventorySave>.cloud.powerups[type] += amount;
				powerupsChanged__BackingField.SafeInvoke(type, DataManager<InventorySave>.cloud.powerups[type]);
			}
		}

		public static void IncreaseTier(UpgradeType type)
		{
			if (upgrades.ContainsKey(type))
			{
				upgrades[type]++;
			}
		}

		public static void ResetTier(UpgradeType type)
		{
			if (upgrades.ContainsKey(type))
			{
				upgrades[type] = 0;
			}
		}

		public static void AddPendingReward(CelebrationReward reward, Func<CelebrationRewardOrigin, int> getRewardPriority)
		{
			foreach (CelebrationReward pendingReward in pendingRewards)
			{
				if (pendingReward.Uid == reward.Uid)
				{
					return;
				}
			}
			lastAddedReward = reward;
			pendingRewards.Add(reward);
			pendingRewards.Sort((CelebrationReward first, CelebrationReward second) => (first.CelebrationRewardOrigin != second.CelebrationRewardOrigin) ? getRewardPriority(first.CelebrationRewardOrigin).CompareTo(getRewardPriority(second.CelebrationRewardOrigin)) : (-1));
		}

		public static void RemovePendingReward(CelebrationReward rewardToRemove)
		{
			CelebrationReward celebrationReward = pendingRewards.Find((CelebrationReward reward) => reward.Uid == rewardToRemove.Uid);
			if (celebrationReward == null)
			{
				if (rewardToRemove.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox)
				{
					celebrationReward = pendingRewards.Find((CelebrationReward reward) => reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox);
				}
				else if (rewardToRemove.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox)
				{
					celebrationReward = pendingRewards.Find((CelebrationReward reward) => reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox);
				}
			}
			if (celebrationReward != null)
			{
				pendingRewards.Remove(celebrationReward);
				if (lastAddedReward != null && lastAddedReward.Uid == celebrationReward.Uid)
				{
					lastAddedReward = null;
				}
			}
		}

		public static int GetOutfitCount()
		{
			int num = 0;
			foreach (KeyValuePair<Characters.CharacterId, List<int>> unlockedOutfit in unlockedOutfits)
			{
				List<int> value = unlockedOutfit.Value;
				if (value != null)
				{
					num += value.Count;
				}
			}
			return num;
		}
	}
}
