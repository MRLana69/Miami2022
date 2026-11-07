using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Components.Analytics.Integration;
using SYBO.Subway.Info;
using UI;
using UI.Screens;

public class PurchaseHandler
{
	public class SyboInGameCurrencyPurchaseEventArgs : EventArgs
	{
		public readonly long PurchaseId;

		public readonly string ItemTitleId;

		public readonly InAppItemModel.ItemType ItemType;

		public readonly long ItemBalanceAfter;

		public readonly long AmountReceived;

		public readonly ObtainableItemSource ItemSource;

		public readonly InGameCurrencyType InGameCurrencyType;

		public readonly long CurrencyBalanceAfter;

		public readonly long Price;

		public readonly long InstanceId;

		public readonly List<long> InstanceIds;

		public SyboInGameCurrencyPurchaseEventArgs(long purchaseId, string itemTitleId, InAppItemModel.ItemType itemType, long itemBalanceAfter, long amountReceived, ObtainableItemSource itemSource, InGameCurrencyType inGameCurrencyType, long currencyBalanceAfter, long price, long instanceId)
		{
			PurchaseId = purchaseId;
			ItemTitleId = itemTitleId;
			ItemType = itemType;
			ItemBalanceAfter = itemBalanceAfter;
			AmountReceived = amountReceived;
			ItemSource = itemSource;
			InGameCurrencyType = inGameCurrencyType;
			CurrencyBalanceAfter = currencyBalanceAfter;
			Price = price;
			InstanceId = instanceId;
		}

		public SyboInGameCurrencyPurchaseEventArgs(long purchaseId, string itemTitleId, InAppItemModel.ItemType itemType, long itemBalanceAfter, long amountReceived, ObtainableItemSource itemSource, InGameCurrencyType inGameCurrencyType, long currencyBalanceAfter, long price, List<long> instanceIds)
			: this(purchaseId, itemTitleId, itemType, itemBalanceAfter, amountReceived, itemSource, inGameCurrencyType, currencyBalanceAfter, price, -1L)
		{
			InstanceIds = instanceIds;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<UpgradeType, int> onUpgradePurchase__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<Characters.CharacterId, int> characterPurchased__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action onFailedToPurchaseEventItemWithInapp__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private EventHandler<SyboInGameCurrencyPurchaseEventArgs> OnIngameCurrencyPurchaseCompleted__BackingField;

	private BoardSelectButton _btnBuyItemWithInapp;

	private static PurchaseHandler _instance;

	public static PurchaseHandler Instance
	{
		get
		{
			return _instance ?? (_instance = new PurchaseHandler());
		}
	}

	public event Action<UpgradeType, int> onUpgradePurchase
	{
		add
		{
			Action<UpgradeType, int> action = onUpgradePurchase__BackingField;
			Action<UpgradeType, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onUpgradePurchase__BackingField, (Action<UpgradeType, int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<UpgradeType, int> action = onUpgradePurchase__BackingField;
			Action<UpgradeType, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onUpgradePurchase__BackingField, (Action<UpgradeType, int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action<Characters.CharacterId, int> characterPurchased
	{
		add
		{
			Action<Characters.CharacterId, int> action = characterPurchased__BackingField;
			Action<Characters.CharacterId, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref characterPurchased__BackingField, (Action<Characters.CharacterId, int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<Characters.CharacterId, int> action = characterPurchased__BackingField;
			Action<Characters.CharacterId, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref characterPurchased__BackingField, (Action<Characters.CharacterId, int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private event Action onFailedToPurchaseEventItemWithInapp
	{
		add
		{
			Action action = onFailedToPurchaseEventItemWithInapp__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToPurchaseEventItemWithInapp__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onFailedToPurchaseEventItemWithInapp__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onFailedToPurchaseEventItemWithInapp__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event EventHandler<SyboInGameCurrencyPurchaseEventArgs> OnIngameCurrencyPurchaseCompleted
	{
		add
		{
			EventHandler<SyboInGameCurrencyPurchaseEventArgs> eventHandler = OnIngameCurrencyPurchaseCompleted__BackingField;
			EventHandler<SyboInGameCurrencyPurchaseEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnIngameCurrencyPurchaseCompleted__BackingField, (EventHandler<SyboInGameCurrencyPurchaseEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
		remove
		{
			EventHandler<SyboInGameCurrencyPurchaseEventArgs> eventHandler = OnIngameCurrencyPurchaseCompleted__BackingField;
			EventHandler<SyboInGameCurrencyPurchaseEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnIngameCurrencyPurchaseCompleted__BackingField, (EventHandler<SyboInGameCurrencyPurchaseEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public void TriggerOnFailedToPurchaseEventItemWithInapp()
	{
		if (onFailedToPurchaseEventItemWithInapp__BackingField != null)
		{
			onFailedToPurchaseEventItemWithInapp__BackingField();
			ResetOnFailedToPurchaseEventItemWithInapp();
		}
	}

	public void ResetOnFailedToPurchaseEventItemWithInapp()
	{
		onFailedToPurchaseEventItemWithInapp__BackingField = null;
	}

	public void PurchaseCharacter(Characters.CharacterId characterId, int outfit, int priceOverride = -1)
	{
		CharacterScreenManager instance = CharacterScreenManager.Instance;
		Characters.Model model = Characters.characterData[characterId];
		Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(characterId, outfit);
		bool flag = outfitForCharacter != null;
		PurchaseData.UnlockType unlockType = model.unlockType;
		int num = model.Price;
		bool flag2 = false;
		if (priceOverride > 0)
		{
			num = priceOverride;
		}
		if (flag)
		{
			Outfit outfit2 = outfitForCharacter;
			unlockType = outfit2.unlockType;
			num = outfit2.price;
		}
		if (unlockType != PurchaseData.UnlockType.Coins && unlockType != PurchaseData.UnlockType.Keys)
		{
			instance.CharacterPurchaseFailure();
			return;
		}
		CheckForDebugFreePurchases(unlockType, num);
		int num2 = InventorySave.coins;
		InAppItemModel.ItemType type = InAppItemModel.ItemType.Coins;
		if (unlockType == PurchaseData.UnlockType.Keys)
		{
			num2 = InventorySave.keys;
			type = InAppItemModel.ItemType.Keys;
		}
		if (num2 >= num)
		{
			switch (unlockType)
			{
			case PurchaseData.UnlockType.Coins:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num);
				InventorySave.DecreaseCoins(num, characterId.ToString(), (outfit != 0) ? outfit.ToString() : "N/A");
				if (Characters.characterData[characterId].missionTargetKey != MissionTarget.none)
				{
					MissionsManager.Instance.PlayerDidThis(Characters.characterData[characterId].missionTargetKey);
				}
				break;
			case PurchaseData.UnlockType.Keys:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, num);
				InventorySave.DecreaseKeys(num, characterId.ToString(), (outfit != 0) ? outfit.ToString() : "N/A");
				break;
			}
			if (flag)
			{
				CharacterManager.UnlockOutfit(characterId, outfit);
			}
			else
			{
				CharacterManager.UnlockCharacter(characterId);
			}
			flag2 = true;
			instance.CharacterPurchaseSuccessful(characterId, outfit);
			characterPurchased__BackingField.SafeInvoke(characterId, outfit);
		}
		else
		{
			InAppManager.instance.SuggestInAppPopup(num, Strings.Get(model.name), type);
			instance.CharacterPurchaseFailure();
		}
		if (flag2)
		{
			InAppItemModel.ItemType itemType = ((!flag) ? InAppItemModel.ItemType.Character : InAppItemModel.ItemType.CharacterOutfit);
			string itemTitleId = ItemInfo.GetItemTitleId(characterId, outfitForCharacter);
			InGameCurrencyType ingameCurrencyTypeFromUnlockType = GetIngameCurrencyTypeFromUnlockType(unlockType);
			long ingameCurrencyAmount = GetIngameCurrencyAmount(ingameCurrencyTypeFromUnlockType);
			RaiseSyboInGameCurrencyPurchaseEvent(itemTitleId, itemType, 1L, 1L, ObtainableItemSource.IngameCurrencyPurchase, ingameCurrencyTypeFromUnlockType, ingameCurrencyAmount, num);
		}
	}

	public void StartPurchaseCharacterWithInapp(Characters.CharacterId characterId)
	{
		onFailedToPurchaseEventItemWithInapp__BackingField = OnPurchaseCharacterWithInappFailed;
		Characters.Model model = Characters.characterData[characterId];
		string inappId = InAppData.ConvertCharacterTypeToDynamicPurchaseId(characterId);
		string characterName = Strings.Get(model.name);
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			InAppData.PrintDynamicIdsForCharacterTypeForAllPlatforms(characterId);
		}
		InAppManager.instance.BuyExpiredLimitedCharacter(inappId, characterName, this);
	}

	public void OnPurchaseCharacterWithInappSucceded(Characters.CharacterId characterId, string inappId)
	{
		CharacterScreenManager.Instance.CharacterPurchaseSuccessful(characterId, 0);
		characterPurchased__BackingField.SafeInvoke(characterId, 0);
	}

	public void OnPurchaseCharacterWithInappFailed()
	{
		CharacterScreenManager.Instance.CharacterPurchaseFailure();
	}

	private static void CheckForDebugFreePurchases(PurchaseData.UnlockType unlockType, int price)
	{
		if (!Globals.DEBUG_FREE_PURCHASES)
		{
			return;
		}
		switch (unlockType)
		{
		case PurchaseData.UnlockType.Coins:
			if (InventorySave.coins < price)
			{
				InventorySave.IncreaseCoins(price, string.Empty);
			}
			break;
		case PurchaseData.UnlockType.Keys:
			if (InventorySave.keys < price)
			{
				InventorySave.IncreaseKeys(price, string.Empty);
			}
			break;
		}
	}

	public void OnPurchaseBoardWithInappSucceded(string inappId, Boards.BoardId boardId)
	{
		if (_btnBuyItemWithInapp != null)
		{
			_btnBuyItemWithInapp.PurchaseSuccessful();
		}
	}

	public void OnPurchaseBoardWithInappFailed()
	{
		if (_btnBuyItemWithInapp != null)
		{
			_btnBuyItemWithInapp.PurchaseFailure();
		}
	}

	public void StartPurchaseBoardWithInapp(Boards.BoardId boardId, BoardSelectButton sender)
	{
		onFailedToPurchaseEventItemWithInapp__BackingField = OnPurchaseBoardWithInappFailed;
		_btnBuyItemWithInapp = sender;
		string inappId = InAppData.ConvertBoardTypeToDynamicPurchaseId(boardId);
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			InAppData.PrintDynamicIdsForBoardTypeForAllPlatforms(boardId);
		}
		InAppManager.instance.BuyExpiredLimitedBoard(inappId, this);
	}

	public void PurchaseKeysIfNeeded(int amountToSaveMe)
	{
		InAppManager.instance.SuggestInAppPopup(amountToSaveMe, "Keys popup", InAppItemModel.ItemType.Keys);
	}

	public void PurchaseHoverboard(Boards.BoardId boardId, BoardSelectButton sender)
	{
		Boards.Board board = Boards.boardData[boardId];
		if (MakeHoverboardPurchase(boardId, board))
		{
			if (sender != null)
			{
				sender.PurchaseSuccessful();
			}
			InGameCurrencyType ingameCurrencyTypeFromUnlockType = GetIngameCurrencyTypeFromUnlockType(board.unlockType);
			long ingameCurrencyAmount = GetIngameCurrencyAmount(ingameCurrencyTypeFromUnlockType);
			RaiseSyboInGameCurrencyPurchaseEvent(ItemInfo.GetItemTitleId(boardId), InAppItemModel.ItemType.Board, 1L, 1L, ObtainableItemSource.IngameCurrencyPurchase, ingameCurrencyTypeFromUnlockType, ingameCurrencyAmount, board.price);
		}
		else if (sender != null)
		{
			sender.PurchaseFailure();
		}
	}

	public void PurchaseHoverboard(Boards.BoardId boardId, Action onPurchaseSuccess = null, Action onPurchaseFailure = null)
	{
		Boards.Board board = Boards.boardData[boardId];
		if (MakeHoverboardPurchase(boardId, board))
		{
			if (onPurchaseSuccess != null)
			{
				onPurchaseSuccess();
			}
			InGameCurrencyType ingameCurrencyTypeFromUnlockType = GetIngameCurrencyTypeFromUnlockType(board.unlockType);
			long ingameCurrencyAmount = GetIngameCurrencyAmount(ingameCurrencyTypeFromUnlockType);
			RaiseSyboInGameCurrencyPurchaseEvent(ItemInfo.GetItemTitleId(boardId), InAppItemModel.ItemType.Board, 1L, 1L, ObtainableItemSource.IngameCurrencyPurchase, ingameCurrencyTypeFromUnlockType, ingameCurrencyAmount, board.price);
		}
		else if (onPurchaseFailure != null)
		{
			onPurchaseFailure();
		}
	}

	private static bool MakeHoverboardPurchase(Boards.BoardId boardId, Boards.Board boardData)
	{
		int price = boardData.price;
		PurchaseData.UnlockType unlockType = boardData.unlockType;
		MissionTarget myTask = MissionTarget.SpendCoin;
		InAppItemModel.ItemType type = InAppItemModel.ItemType.Coins;
		int num = InventorySave.coins;
		if (unlockType == PurchaseData.UnlockType.Keys)
		{
			myTask = MissionTarget.SpendKeys;
			type = InAppItemModel.ItemType.Keys;
			num = InventorySave.keys;
		}
		bool flag = false;
		if (Globals.DEBUG_FREE_PURCHASES)
		{
			if (num < price)
			{
				num += price;
			}
			flag = true;
		}
		if (num >= price)
		{
			MissionsManager.Instance.PlayerDidThis(myTask, price);
			BoardManager.UnlockBoard(boardId);
			flag = true;
		}
		else
		{
			InAppManager.instance.SuggestInAppPopup(price, Strings.Get(boardData.name), type);
		}
		if (unlockType == PurchaseData.UnlockType.Keys && flag)
		{
			InventorySave.DecreaseKeys(price, boardId.ToString());
		}
		else if (flag)
		{
			InventorySave.DecreaseCoins(price, boardId.ToString());
		}
		return flag;
	}

	public void PurchaseHoverboardTheme(Boards.BoardId boardId, int themeIndex, BoardSelectButton sender)
	{
		HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[boardId][themeIndex];
		if (MakeHoverboardThemePurchase(boardId, hoverboardTheme))
		{
			sender.PurchaseSuccessful(true, themeIndex);
			InGameCurrencyType ingameCurrencyTypeFromUnlockType = GetIngameCurrencyTypeFromUnlockType(hoverboardTheme.unlockType);
			long ingameCurrencyAmount = GetIngameCurrencyAmount(ingameCurrencyTypeFromUnlockType);
			RaiseSyboInGameCurrencyPurchaseEvent(ItemInfo.GetItemTitleId(boardId, hoverboardTheme), InAppItemModel.ItemType.BoardAbility, 1L, 1L, ObtainableItemSource.IngameCurrencyPurchase, ingameCurrencyTypeFromUnlockType, ingameCurrencyAmount, hoverboardTheme.price);
		}
		else
		{
			sender.PurchaseFailure();
		}
	}

	private static bool MakeHoverboardThemePurchase(Boards.BoardId boardId, HoverboardThemes.HoverboardTheme ht)
	{
		int price = ht.price;
		PurchaseData.UnlockType unlockType = ht.unlockType;
		MissionTarget myTask = MissionTarget.SpendCoin;
		InAppItemModel.ItemType type = InAppItemModel.ItemType.Coins;
		int num = InventorySave.coins;
		if (unlockType == PurchaseData.UnlockType.Keys)
		{
			myTask = MissionTarget.SpendKeys;
			type = InAppItemModel.ItemType.Keys;
			num = InventorySave.keys;
		}
		bool flag = false;
		if (Globals.DEBUG_FREE_PURCHASES)
		{
			if (num < price)
			{
				num += price;
			}
			flag = true;
		}
		if (num >= price)
		{
			MissionsManager.Instance.PlayerDidThis(myTask, price);
			BoardManager.SetThemeAcquired(boardId, ht.identifier);
			BoardManager.SetThemeActive(boardId, ht.identifier, true);
			flag = true;
		}
		else
		{
			InAppManager.instance.SuggestInAppPopup(price, Strings.Get(ht.title), type);
		}
		if (unlockType == PurchaseData.UnlockType.Keys && flag)
		{
			InventorySave.DecreaseKeys(price, boardId.ToString(), ht.GetAnalyticsReadablePowerName());
		}
		else if (flag)
		{
			InventorySave.DecreaseCoins(price, boardId.ToString(), ht.GetAnalyticsReadablePowerName());
		}
		return flag;
	}

	public void PurchaseUpgrade(UpgradeType type, IBuyButton sender)
	{
		PurchaseUpgrade(type, sender.OnPurchaseSuccessful, sender.OnPurchaseFailure);
	}

	public void PurchaseUpgrade(UpgradeType upgradeType, Action onPurchaseSuccessful, Action onPurchaseFailure)
	{
		int num = ((Upgrades.upgrades[upgradeType].numberOfTiers != 0) ? Upgrades.upgrades[upgradeType].GetPriceToIncreaseTier(InventorySave.GetTier(upgradeType)) : Upgrades.upgrades[upgradeType].GetPriceToIncreaseTier(0));
		if (upgradeType == UpgradeType.tokenmysterybox && !UserSave.hasUserRecievedFreeTokenMysteryBox)
		{
			num = 0;
			UserSave.hasUserRecievedFreeTokenMysteryBox = true;
		}
		int num2 = InventorySave.coins;
		PurchaseData.UnlockType priceType = Upgrades.upgrades[upgradeType].priceType;
		if (priceType == PurchaseData.UnlockType.Keys)
		{
			num2 = InventorySave.keys;
		}
		if (Globals.DEBUG_FREE_PURCHASES && num2 < num)
		{
			if (priceType == PurchaseData.UnlockType.Keys)
			{
				InventorySave.IncreaseKeys(num, string.Empty);
			}
			else
			{
				InventorySave.IncreaseCoins(num, string.Empty);
			}
		}
		if (num2 >= num)
		{
			int num3 = 1;
			int num4 = 1;
			switch (upgradeType)
			{
			case UpgradeType.hoverboard:
			case UpgradeType.headstart2000:
			case UpgradeType.scorebooster:
				InventorySave.IncreaseAmount(upgradeType, num4);
				num3 = InventorySave.GetAmount(upgradeType);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num);
				if (upgradeType == UpgradeType.headstart2000)
				{
					MissionsManager.Instance.PlayerDidThis(MissionTarget.HaveHeadStartLarge);
				}
				break;
			case UpgradeType.jetpack:
			case UpgradeType.supersneakers:
			case UpgradeType.coinmagnet:
			case UpgradeType.doubleMultiplier:
				InventorySave.IncreaseTier(upgradeType);
				num3 = InventorySave.GetTier(upgradeType);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.HaveUpgrades);
				break;
			case UpgradeType.mysterybox:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, num);
				MissionsManager.Instance.PlayerDidThis(MissionTarget.BuyMysterybox);
				break;
			case UpgradeType.tokenmysterybox:
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, num);
				break;
			default:
				SDebug.LogErrorFormat("Missing logic for handling purchase of powerup '{0}'. Fail purchase.", upgradeType);
				onPurchaseFailure.SafeInvoke();
				break;
			}
			if (priceType == PurchaseData.UnlockType.Keys)
			{
				InventorySave.DecreaseKeys(num, upgradeType.ToString(), (InventorySave.GetTier(upgradeType) != 0) ? ("Tier " + InventorySave.GetTier(upgradeType)) : "N/A");
			}
			else
			{
				InventorySave.DecreaseCoins(num, upgradeType.ToString(), (InventorySave.GetTier(upgradeType) != 0) ? ("Tier " + InventorySave.GetTier(upgradeType)) : "N/A");
			}
			InAppItemModel.ItemType itemTypeFromPowerup = InAppItemModel.GetItemTypeFromPowerup(upgradeType);
			long num5 = RewardManager.GenerateRewardUid();
			RaiseSyboInGameCurrencyPurchaseEvent(ItemInfo.GetItemTitleId(upgradeType), itemTypeFromPowerup, num3, num4, ObtainableItemSource.IngameCurrencyPurchase, (priceType != PurchaseData.UnlockType.Keys) ? InGameCurrencyType.Coins : InGameCurrencyType.Keys, (priceType != PurchaseData.UnlockType.Keys) ? InventorySave.coins : InventorySave.keys, num, num5);
			if (itemTypeFromPowerup == InAppItemModel.ItemType.Gachabox)
			{
				RewardManager.SetNextRewardUid(num5);
			}
			onPurchaseSuccessful.SafeInvoke();
			if (onUpgradePurchase__BackingField != null)
			{
				onUpgradePurchase__BackingField(upgradeType, num3);
			}
		}
		else
		{
			if (priceType == PurchaseData.UnlockType.Keys)
			{
				InAppManager.instance.SuggestInAppPopup(num, Strings.Get(Upgrades.upgrades[upgradeType].name), InAppItemModel.ItemType.Keys);
			}
			else
			{
				InAppManager.instance.SuggestInAppPopup(num, Strings.Get(Upgrades.upgrades[upgradeType].name), InAppItemModel.ItemType.Coins);
			}
			onPurchaseFailure.SafeInvoke();
		}
		SaveManager.Save();
	}

	public void PurchaseShopItem(IngameShopItems.IngameShopItemType shopItemType, int amount, int tier = 0, Action<List<long>> onPurchaseSuccessful = null, Action onPurchaseFailure = null)
	{
		int num = IngameShopItems.instance.GetItem(shopItemType).prices[tier];
		int num2 = InventorySave.coins;
		PurchaseData.UnlockType priceType = IngameShopItems.instance.GetItem(shopItemType).priceType;
		if (priceType == PurchaseData.UnlockType.Keys)
		{
			num2 = InventorySave.keys;
		}
		if (Globals.DEBUG_FREE_PURCHASES && num2 < num)
		{
			if (priceType == PurchaseData.UnlockType.Keys)
			{
				InventorySave.IncreaseKeys(num, string.Empty);
			}
			else
			{
				InventorySave.IncreaseCoins(num, string.Empty);
			}
		}
		if (num2 >= num)
		{
			if (priceType == PurchaseData.UnlockType.Keys)
			{
				InventorySave.DecreaseKeys(num, shopItemType.ToString());
			}
			else
			{
				InventorySave.DecreaseCoins(num, shopItemType.ToString());
			}
			InAppItemModel.ItemType itemType = AnalyticsHelper.ItemTypeForIngameShopItemType(shopItemType);
			int num3 = AnalyticsHelper.GetItemBalanceForIngameShopItem(shopItemType) + amount;
			List<long> list = new List<long>();
			if (shopItemType == IngameShopItems.IngameShopItemType.fillersupermysterybox)
			{
				for (int i = 0; i < amount; i++)
				{
					list.Add(RewardManager.GenerateRewardUid());
				}
				RaiseSyboInGameCurrencyPurchaseMultiEvent(ItemInfo.GetItemTitleId(shopItemType), itemType, num3, amount, ObtainableItemSource.IngameCurrencyPurchase, (priceType != PurchaseData.UnlockType.Keys) ? InGameCurrencyType.Coins : InGameCurrencyType.Keys, (priceType != PurchaseData.UnlockType.Keys) ? InventorySave.coins : InventorySave.keys, num, list);
			}
			else
			{
				list.Add(RewardManager.GenerateRewardUid());
				RaiseSyboInGameCurrencyPurchaseEvent(ItemInfo.GetItemTitleId(shopItemType), itemType, num3, amount, ObtainableItemSource.IngameCurrencyPurchase, (priceType != PurchaseData.UnlockType.Keys) ? InGameCurrencyType.Coins : InGameCurrencyType.Keys, (priceType != PurchaseData.UnlockType.Keys) ? InventorySave.coins : InventorySave.keys, num, list[0]);
			}
			onPurchaseSuccessful.SafeInvoke(list);
		}
		else
		{
			if (priceType == PurchaseData.UnlockType.Keys)
			{
				InAppManager.instance.SuggestInAppPopup(num, Strings.Get(IngameShopItems.instance.GetItem(shopItemType).nameStringID), InAppItemModel.ItemType.Keys);
			}
			else
			{
				InAppManager.instance.SuggestInAppPopup(num, Strings.Get(IngameShopItems.instance.GetItem(shopItemType).nameStringID), InAppItemModel.ItemType.Coins);
			}
			onPurchaseFailure.SafeInvoke();
		}
		SaveManager.Save();
	}

	public void PurchaseSkipMission(int missionIndex, IBuyButton sender)
	{
		PurchaseSkipMission(missionIndex, sender.OnPurchaseSuccessful, sender.OnPurchaseFailure);
	}

	public void PurchaseSkipMission(int missionIndex, Action onPurchaseSuccessful, Action onPurchaseFailure)
	{
		int price = MissionsData.skipData[missionIndex].GetPrice();
		if (Globals.DEBUG_FREE_PURCHASES && InventorySave.coins < price)
		{
			InventorySave.IncreaseCoins(price, string.Empty);
		}
		if (InventorySave.coins >= price)
		{
			int num = 1;
			int num2 = 1;
			MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendCoin, price, missionIndex);
			InventorySave.DecreaseCoins(price, "SkipMission");
			InAppItemModel.ItemType itemType = InAppItemModel.ItemType.SkipMission;
			long instanceId = RewardManager.GenerateRewardUid();
			RaiseSyboInGameCurrencyPurchaseEvent(ItemInfo.GetSkipMissionItemTitleId(missionIndex), itemType, num, num2, ObtainableItemSource.IngameCurrencyPurchase, InGameCurrencyType.Coins, InventorySave.coins, price, instanceId);
			onPurchaseSuccessful();
		}
		else
		{
			InAppManager.instance.SuggestInAppPopup(price, Strings.Get(MissionsData.skipData[missionIndex].name), InAppItemModel.ItemType.Coins);
			onPurchaseFailure();
		}
	}

	private void RaiseSyboInGameCurrencyPurchaseEvent(string itemTitleId, InAppItemModel.ItemType itemType, long itemBalanceAfter, long amountReceived, ObtainableItemSource itemSource, InGameCurrencyType inGameCurrencyType, long currencyBalanceAfter, long price)
	{
		RaiseSyboInGameCurrencyPurchaseEvent(itemTitleId, itemType, itemBalanceAfter, amountReceived, itemSource, inGameCurrencyType, currencyBalanceAfter, price, SubwayRandom.DateTimeTicksRandomOffset());
	}

	private void RaiseSyboInGameCurrencyPurchaseEvent(string itemTitleId, InAppItemModel.ItemType itemType, long itemBalanceAfter, long amountReceived, ObtainableItemSource itemSource, InGameCurrencyType inGameCurrencyType, long currencyBalanceAfter, long price, long instanceId)
	{
		if (OnIngameCurrencyPurchaseCompleted__BackingField != null)
		{
			if (price == 0)
			{
				inGameCurrencyType = InGameCurrencyType.None;
				currencyBalanceAfter = 0L;
				itemSource = ObtainableItemSource.FreeUnlock;
			}
			SyboInGameCurrencyPurchaseEventArgs e = new SyboInGameCurrencyPurchaseEventArgs(SubwayRandom.DateTimeTicksRandomOffset(), itemTitleId, itemType, itemBalanceAfter, amountReceived, itemSource, inGameCurrencyType, currencyBalanceAfter, price, instanceId);
			OnIngameCurrencyPurchaseCompleted__BackingField(this, e);
		}
	}

	private void RaiseSyboInGameCurrencyPurchaseMultiEvent(string itemTitleId, InAppItemModel.ItemType itemType, long itemBalanceAfter, long amountReceived, ObtainableItemSource itemSource, InGameCurrencyType inGameCurrencyType, long currencyBalanceAfter, long price, List<long> instanceIds)
	{
		SyboInGameCurrencyPurchaseEventArgs e = new SyboInGameCurrencyPurchaseEventArgs(SubwayRandom.DateTimeTicksRandomOffset(), itemTitleId, itemType, itemBalanceAfter, amountReceived, itemSource, inGameCurrencyType, currencyBalanceAfter, price, instanceIds);
		OnIngameCurrencyPurchaseCompleted__BackingField(this, e);
	}

	private InGameCurrencyType GetIngameCurrencyTypeFromUnlockType(PurchaseData.UnlockType unlockType)
	{
		switch (unlockType)
		{
		case PurchaseData.UnlockType.Coins:
			return InGameCurrencyType.Coins;
		case PurchaseData.UnlockType.Keys:
			return InGameCurrencyType.Keys;
		default:
			return InGameCurrencyType.None;
		}
	}

	private long GetIngameCurrencyAmount(InGameCurrencyType unlockType)
	{
		switch (unlockType)
		{
		case InGameCurrencyType.Coins:
			return InventorySave.coins;
		case InGameCurrencyType.Keys:
			return InventorySave.keys;
		default:
			return 0L;
		}
	}
}
