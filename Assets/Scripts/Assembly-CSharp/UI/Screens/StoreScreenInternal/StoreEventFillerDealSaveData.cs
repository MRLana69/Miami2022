using System;
using SYBO.Subway.Info;

namespace UI.Screens.StoreScreenInternal
{
	public class StoreEventFillerDealSaveData
	{
		public DateTime currentStoreFillerDealStartTime = DateTime.MinValue;

		public bool currentStoreFillerDealPurchased;

		public IngameShopItems.IngameShopItemType fillerDealType;

		public Characters.CharacterId fillerDealTokenType = Characters.CharacterId.facebookRewardCharacter01;

		public int fillerDealIndex;
	}
}
