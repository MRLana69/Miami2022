using System.Collections.Generic;
using Info;
using SYBO.Subway.Info;
using UnityEngine;

namespace InApp
{
	public class InAppItemModel
	{
		public enum ItemType
		{
			Undefined = 0,
			Character = 1,
			CharacterOutfit = 2,
			Board = 3,
			BoardAbility = 4,
			Coins = 5,
			Keys = 6,
			Bundle = 7,
			DoubleCoins = 8,
			Hoverboards = 9,
			Headstarts = 10,
			Scoreboosters = 11,
			Jetpack = 12,
			SuperSneakers = 13,
			CoinMagnet = 14,
			DoubleMultiplier = 15,
			Gachabox = 16,
			SkipMission = 17,
			CharacterToken = 18,
			Trophy = 19,
			AdRemoval = 20,
			CharacterSkip = 21,
			tokenMysteryBox = 22
		}

		public enum OfferType
		{
			Normal = 0,
			Sale = 1,
			Promotion = 2
		}

		public class BundleItem
		{
			private Boards.BoardId _boardId;

			private Characters.CharacterId _characterId;

			private HoverboardThemes.ThemeIdentifier _themeIdentifier;

			public ItemType type { get; private set; }

			public int amount { get; private set; }

			public bool isBoardTypeValid { get; private set; }

			public bool isCharacterTypeValid { get; private set; }

			public bool isThemeIdentifierValid { get; private set; }

			public Boards.BoardId boardId
			{
				get
				{
					if (!isBoardTypeValid)
					{
						LogError("Invalid board type requested.");
					}
					return _boardId;
				}
			}

			public Characters.CharacterId characterId
			{
				get
				{
					if (!isCharacterTypeValid)
					{
						LogError("Invalid character type requested.");
					}
					return _characterId;
				}
			}

			public HoverboardThemes.ThemeIdentifier themeIdentifier
			{
				get
				{
					if (!isThemeIdentifierValid)
					{
						LogError("Invalid theme identifier requested.");
					}
					return _themeIdentifier;
				}
			}

			public BundleItem(ItemType type, int amount)
			{
				this.type = type;
				this.amount = amount;
			}

			public BundleItem(ItemType type, Characters.CharacterId characterId)
			{
				this.type = type;
				amount = 1;
				_characterId = characterId;
				isCharacterTypeValid = true;
			}

			public BundleItem(ItemType type, Boards.BoardId boardId)
			{
				this.type = type;
				amount = 1;
				_boardId = boardId;
				isBoardTypeValid = true;
			}

			public BundleItem(ItemType type, Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeIdentifier)
			{
				this.type = type;
				amount = 1;
				_boardId = boardId;
				isBoardTypeValid = true;
				_themeIdentifier = themeIdentifier;
				isThemeIdentifierValid = true;
			}
		}

		public string displayName = string.Empty;

		public string itemId = string.Empty;

		public int payoutAmount;

		public bool isConsumable;

		public string normalOfferId = string.Empty;

		public List<BundleItem> bundleContents;

		public ItemType itemType;

		public OfferType itemOfferType;

		private string _localizedPrize = string.Empty;

		public bool isValidated;

		public bool isDynamic;

		public string localizedPrice
		{
			get
			{
				if (isValidated)
				{
					return _localizedPrize;
				}
				return Strings.Get("BUY_INAPP_NOPRICE_DEFAULT");
			}
			set
			{
				_localizedPrize = value;
			}
		}

		public static ItemType GetItemTypeFromPowerup(UpgradeType upgradeType)
		{
			switch (upgradeType)
			{
			case UpgradeType.hoverboard:
				return ItemType.Hoverboards;
			case UpgradeType.headstart2000:
				return ItemType.Headstarts;
			case UpgradeType.scorebooster:
				return ItemType.Scoreboosters;
			case UpgradeType.mysterybox:
				return ItemType.Gachabox;
			case UpgradeType.tokenmysterybox:
				return ItemType.tokenMysteryBox;
			case UpgradeType.jetpack:
				return ItemType.Jetpack;
			case UpgradeType.supersneakers:
				return ItemType.SuperSneakers;
			case UpgradeType.coinmagnet:
				return ItemType.CoinMagnet;
			case UpgradeType.doubleMultiplier:
				return ItemType.DoubleMultiplier;
			default:
				SDebug.LogErrorFormat("Missing logic for deriving item type from powerup type '{0}'. Returning '{1}'.", upgradeType, ItemType.Undefined);
				return ItemType.Undefined;
			}
		}

		public string GetIconName()
		{
			return InAppDecorator.GetIconName(itemId);
		}

		public void ConvertToPromotion(int payoutIncrement)
		{
			payoutAmount += payoutIncrement;
			itemOfferType = OfferType.Promotion;
		}

		public List<int> GetAllThemesForBoard(Boards.BoardId boardId)
		{
			List<int> list = new List<int>();
			if (bundleContents != null)
			{
				foreach (BundleItem bundleContent in bundleContents)
				{
					if (bundleContent.isThemeIdentifierValid)
					{
						list.Add(HoverboardThemes.GetIndexForTheme(boardId, bundleContent.themeIdentifier));
					}
				}
			}
			return list;
		}

		public static InAppItemModel Clone(InAppItemModel objectToClone)
		{
			InAppItemModel inAppItemModel = new InAppItemModel();
			inAppItemModel.itemId = objectToClone.itemId;
			inAppItemModel.payoutAmount = objectToClone.payoutAmount;
			inAppItemModel.isConsumable = objectToClone.isConsumable;
			inAppItemModel.normalOfferId = objectToClone.normalOfferId;
			inAppItemModel.bundleContents = objectToClone.bundleContents;
			inAppItemModel.itemType = objectToClone.itemType;
			inAppItemModel.itemOfferType = objectToClone.itemOfferType;
			inAppItemModel.localizedPrice = objectToClone.localizedPrice;
			inAppItemModel.isValidated = objectToClone.isValidated;
			inAppItemModel.isDynamic = objectToClone.isDynamic;
			return inAppItemModel;
		}

		public override string ToString()
		{
			return string.Format("currencyItemModel:itemId->{0}", itemId);
		}

		private static void LogError(string msg, Object context = null)
		{
			SDebug.LogError("[InAppItemModel] " + msg, context);
		}
	}
}
