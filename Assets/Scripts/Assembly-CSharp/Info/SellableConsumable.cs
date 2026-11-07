namespace Info
{
	public class SellableConsumable
	{
		public static readonly SellableConsumable headstart = new SellableConsumable(UpgradeType.headstart2000, Upgrades.upgrades[UpgradeType.headstart2000].GetPriceToIncreaseTier(0), "icon_upgrades_headstart2000_large", "CONSUMABLES_POPUP_HEADER_HEADSTART", "HEADSTART_POPUP_DESCRIPTION_LONG");

		public static readonly SellableConsumable scorebooster = new SellableConsumable(UpgradeType.scorebooster, Upgrades.upgrades[UpgradeType.scorebooster].GetPriceToIncreaseTier(0), "icon_score_multiplier", "CONSUMABLES_POPUP_HEADER_SCOREBOOSTER", "SCOREBOOSTER_POPUP_DESCRIPTION_LONG");

		public static readonly SellableConsumable hoverboard = new SellableConsumable(UpgradeType.hoverboard, Upgrades.upgrades[UpgradeType.hoverboard].GetPriceToIncreaseTier(0), "icon_upgrades_hoverboard_large", "CONSUMABLES_POPUP_HEADER_HOVERBOARD", "HOVERBOARD_POPUP_DESCRIPTION_LONG");

		public UpgradeType upgradeType { get; private set; }

		public int price { get; private set; }

		public string spriteName { get; private set; }

		public string header { get; private set; }

		public string description { get; private set; }

		private SellableConsumable(UpgradeType upgradeType, int price, string spriteName, string header, string description)
		{
			this.upgradeType = upgradeType;
			this.price = price;
			this.spriteName = spriteName;
			this.header = header;
			this.description = description;
		}
	}
}
