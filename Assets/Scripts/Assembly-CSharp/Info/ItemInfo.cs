using System;
using Data.Save;
using InApp;
using SYBO.Subway.Info;

namespace Info
{
	public static class ItemInfo
	{
		public const int ItemAmountReceiveDefault = 1;

		private const string ItemTitleIdHoverboard = "hoverboard";

		private const string ItemTitleIdHeadstart = "headstart";

		private const string ItemTitleIdScorebooster = "scorebooster";

		private static readonly string[] SKIP_MISSION_IDS = new string[3] { "skipmission1", "skipmission2", "skipmission3" };

		public static string GetItemTitleId(UpgradeType upgradeType)
		{
			switch (upgradeType)
			{
			case UpgradeType.headstart2000:
				return "headstart";
			case UpgradeType.hoverboard:
				return "hoverboard";
			case UpgradeType.scorebooster:
				return "scorebooster";
			case UpgradeType.mysterybox:
			case UpgradeType.tokenmysterybox:
			case UpgradeType.jetpack:
			case UpgradeType.supersneakers:
			case UpgradeType.coinmagnet:
			case UpgradeType.doubleMultiplier:
				return upgradeType.ToString().ToLowerInvariant();
			default:
				SDebug.LogErrorFormat("Missing logic for deriving Item Title ID from powerup type '{0}'. Returning empty.", upgradeType);
				return string.Empty;
			}
		}

		public static string GetSkipMissionItemTitleId(int missionIndex)
		{
			return (missionIndex >= SKIP_MISSION_IDS.Length) ? string.Empty : SKIP_MISSION_IDS[missionIndex];
		}

		public static string GetItemTitleId(MysteryBox.Type boxType)
		{
			string text;
			switch (boxType)
			{
			case MysteryBox.Type.Normal:
				text = UpgradeType.mysterybox.ToString();
				break;
			case MysteryBox.Type.Token:
				text = UpgradeType.tokenmysterybox.ToString();
				break;
			case MysteryBox.Type.Super:
			case MysteryBox.Type.Mini:
				text = string.Format("{0}_{1}", UpgradeType.mysterybox, boxType);
				break;
			default:
				SDebug.LogErrorFormat("Missing logic for deriving Item Title ID from box type '{0}'. Returning empty.", boxType);
				text = string.Empty;
				break;
			}
			return text.ToLowerInvariant();
		}

		public static string GetItemTitleId(IngameShopItems.IngameShopItemType shopItemType)
		{
			switch (shopItemType)
			{
			case IngameShopItems.IngameShopItemType.fillercharactertokens:
				return string.Format("token_{0}", GetItemTitleId(UserSave.currentStoreFillerDeal.fillerDealTokenType));
			case IngameShopItems.IngameShopItemType.fillersupermysterybox:
				return string.Format("{0}_{1}", UpgradeType.mysterybox, MysteryBox.Type.Super);
			default:
				SDebug.LogErrorFormat("Missing logic for deriving Item Title ID from shop item type '{0}'. Returning empty.", shopItemType);
				return string.Empty;
			}
		}

		public static string GetItemTitleId(Characters.CharacterId characterId, Outfit outfit = null)
		{
			if (outfit == null)
			{
				Characters.Model model = Characters.characterData[characterId];
				return Strings.GetDefault(model.name).ToLowerInvariant();
			}
			Characters.Model model2 = Characters.characterData[characterId];
			return string.Format("{0}_{1}", Strings.GetDefault(model2.name).ToLowerInvariant(), Strings.GetDefault(outfit.title).ToLowerInvariant());
		}

		public static string GetItemTitleId(Boards.BoardId boardId)
		{
			Boards.Board board = Boards.boardData[boardId];
			return Strings.GetDefault(board.name).ToLowerInvariant();
		}

		public static string GetItemTitleId(Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeIdentifier)
		{
			HoverboardThemes.HoverboardTheme boardTheme = HoverboardThemes.hoverboardCustomThemes[boardId].Find((HoverboardThemes.HoverboardTheme x) => x.identifier == themeIdentifier);
			return GetItemTitleId(boardId, boardTheme);
		}

		public static string GetItemTitleId(Boards.BoardId boardId, HoverboardThemes.HoverboardTheme boardTheme)
		{
			Boards.Board board = Boards.boardData[boardId];
			return string.Format("{0}_{1}_{2}", Strings.GetDefault(board.name).ToLowerInvariant(), boardTheme.power.ToString().ToLowerInvariant(), Strings.GetDefault(boardTheme.title).ToLowerInvariant());
		}

		public static string GetItemTitleId(InAppItemModel.ItemType itemType)
		{
			switch (itemType)
			{
			case InAppItemModel.ItemType.Hoverboards:
				return "hoverboard";
			case InAppItemModel.ItemType.Headstarts:
				return "headstart";
			case InAppItemModel.ItemType.Scoreboosters:
				return "scorebooster";
			case InAppItemModel.ItemType.Coins:
			case InAppItemModel.ItemType.Keys:
			case InAppItemModel.ItemType.DoubleCoins:
			case InAppItemModel.ItemType.CharacterSkip:
				return itemType.ToString().ToLowerInvariant();
			default:
				SDebug.LogErrorFormat("Missing logic for deriving Item Title ID from item type '{0}'. Returning empty.", itemType);
				return string.Empty;
			}
		}

		public static string GetItemTitleId(InAppItemModel.BundleItem bundleItem)
		{
			switch (bundleItem.type)
			{
			case InAppItemModel.ItemType.Board:
				return GetItemTitleId(bundleItem.boardId);
			case InAppItemModel.ItemType.BoardAbility:
				return GetItemTitleId(bundleItem.boardId, bundleItem.themeIdentifier);
			default:
				return GetItemTitleId(bundleItem.type);
			}
		}

		public static bool TryGetItemTitleId(InAppItemModel item, out string itemTitleId)
		{
			itemTitleId = string.Empty;
			if (item == null || item.itemId == null)
			{
				SDebug.LogErrorFormat("Can't determine Item Title ID from null {0} or without defined item/product ID", typeof(InAppItemModel).Name);
				return false;
			}
			try
			{
				switch (item.itemType)
				{
				case InAppItemModel.ItemType.Character:
				{
					Characters.CharacterId charReturned;
					InAppData.TryConvertDynamicPurchaseIdToCharacterType(item.itemId, out charReturned);
					itemTitleId = GetItemTitleId(charReturned);
					break;
				}
				case InAppItemModel.ItemType.Board:
				{
					Boards.BoardId boardReturned;
					InAppData.TryConvertDynamicBoardIdToBoardType(item.itemId, out boardReturned);
					itemTitleId = GetItemTitleId(boardReturned);
					break;
				}
				case InAppItemModel.ItemType.Coins:
				case InAppItemModel.ItemType.Keys:
				case InAppItemModel.ItemType.Bundle:
				case InAppItemModel.ItemType.DoubleCoins:
				case InAppItemModel.ItemType.AdRemoval:
					itemTitleId = InAppData.StripProductPrefixes(item.itemId);
					break;
				case InAppItemModel.ItemType.Hoverboards:
				case InAppItemModel.ItemType.Headstarts:
				case InAppItemModel.ItemType.Scoreboosters:
					itemTitleId = GetItemTitleId(item.itemType);
					break;
				default:
					SDebug.LogErrorFormat("Missing logic for deriving Item Title ID from item with product ID '{0}'", item.itemId);
					itemTitleId = string.Empty;
					break;
				}
			}
			catch (Exception ex)
			{
				SDebug.LogErrorFormat("Failed to determine Item Title ID for item '{0}'. Exception:\n{1}", item.itemId, ex.Message);
				itemTitleId = string.Empty;
			}
			itemTitleId = itemTitleId.Replace("_", string.Empty).ToLowerInvariant();
			return !string.IsNullOrEmpty(itemTitleId);
		}
	}
}
