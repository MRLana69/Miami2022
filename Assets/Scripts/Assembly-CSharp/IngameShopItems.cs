using System;
using Info;
using UnityEngine;

public class IngameShopItems : ScriptableObject
{
	public enum IngameShopItemType
	{
		fillercharactertokens = 0,
		fillersupermysterybox = 1
	}

	[Serializable]
	public class IngameShopItem
	{
		public string nameStringID;

		public string descriptionStringID;

		public string mysteryBoxDescriptionStringID;

		[SerializeField]
		private int[] _prices;

		[SerializeField]
		private int[] _payoutAmounts;

		public PurchaseData.UnlockType priceType = PurchaseData.UnlockType.Coins;

		public string iconNameMenu;

		public int weight;

		public int[] prices
		{
			get
			{
				return _prices;
			}
		}

		public int[] payoutAmounts
		{
			get
			{
				return _payoutAmounts;
			}
		}
	}

	private static IngameShopItems _instance;

	[SerializeField]
	private IngameShopItem _fillerCharacterTokens;

	[SerializeField]
	private IngameShopItem _fillerSuperMysteryBox;

	public static IngameShopItems instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<IngameShopItems>("Info/IngameShopItems");
			}
			return _instance;
		}
	}

	public IngameShopItem GetItem(IngameShopItemType type)
	{
		switch (type)
		{
		case IngameShopItemType.fillercharactertokens:
			return _fillerCharacterTokens;
		case IngameShopItemType.fillersupermysterybox:
			return _fillerSuperMysteryBox;
		default:
			return null;
		}
	}
}
