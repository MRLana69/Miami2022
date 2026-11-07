using InApp;
using Kiloo.Common.Extensions;
using UI;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

public static class ShopCellManager
{
	public static StoreContainerSmall.StoreContainerSmallPayload GetCellPayloadForOffer(InAppItemModel offer)
	{
		Color lineThroughColor = Color.black;
		Color color = Color.black;
		Color valueEffectColor = ColorData.instance.white;
		string empty = string.Empty;
		bool showSale = false;
		Vector3[] starGlintPositions = new Vector3[0];
		string text = FormatValueText(offer.payoutAmount);
		string text2 = "0";
		string normalOfferId = offer.normalOfferId;
		if (!normalOfferId.IsNullOrEmpty())
		{
			text2 = FormatValueText(InAppData.inAppData[offer.normalOfferId].payoutAmount);
		}
		Color fillTopColor;
		Color fillBottomColor;
		Color frameTopColor;
		Color frameBottomColor;
		string valueText;
		Color valueColor;
		if (offer.itemType == InAppItemModel.ItemType.Coins)
		{
			fillTopColor = NGUIMath.HexToColor(42258943u);
			fillBottomColor = NGUIMath.HexToColor(433581311u);
			frameTopColor = NGUIMath.HexToColor(358124031u);
			frameBottomColor = NGUIMath.HexToColor(459974143u);
			valueText = Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_COINS").Populate(text);
			empty = Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_COINS").Populate(text2);
			valueColor = ColorData.instance.midnight;
		}
		else
		{
			fillTopColor = NGUIMath.HexToColor(3984002815u);
			fillBottomColor = ColorData.instance.sun;
			frameTopColor = NGUIMath.HexToColor(3109290495u);
			frameBottomColor = NGUIMath.HexToColor(3731161599u);
			valueText = Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_KEYS").Populate(text);
			empty = Strings.Get("COIN_BUTTON_HELPER_NUMBER_OF_KEYS").Populate(text2);
			valueColor = ColorData.instance.mahogany;
		}
		if (offer.itemOfferType == InAppItemModel.OfferType.Promotion)
		{
			fillTopColor = NGUIMath.HexToColor(2803199u);
			fillBottomColor = NGUIMath.HexToColor(2897543167u);
			frameTopColor = NGUIMath.HexToColor(204441855u);
			frameBottomColor = NGUIMath.HexToColor(288986623u);
			valueColor = NGUIMath.HexToColor(1471743u);
			valueEffectColor = ColorData.instance.lightShadow;
			color = NGUIMath.HexToColor(543672575u);
			lineThroughColor = color;
			showSale = true;
		}
		string localizedPrice = offer.localizedPrice;
		string iconName = offer.GetIconName();
		Color lightShadow = ColorData.instance.lightShadow;
		Color lineThroughShadowColor = lightShadow;
		Color bannerTopColor = NGUIMath.HexToColor(4285612031u);
		Color bannerBottomColor = NGUIMath.HexToColor(4180952831u);
		bool showMostPopular = InAppManager.ShouldShowMostPopular(offer);
		string itemId = offer.itemId;
		StoreContainerSmall.StoreContainerSmallPayload storeContainerSmallPayload = new StoreContainerSmall.StoreContainerSmallPayload();
		storeContainerSmallPayload.frameTopColor = frameTopColor;
		storeContainerSmallPayload.frameBottomColor = frameBottomColor;
		storeContainerSmallPayload.fillTopColor = fillTopColor;
		storeContainerSmallPayload.fillBottomColor = fillBottomColor;
		storeContainerSmallPayload.iconName = iconName;
		storeContainerSmallPayload.valueText = valueText;
		storeContainerSmallPayload.valueColor = valueColor;
		storeContainerSmallPayload.valueEffectColor = valueEffectColor;
		storeContainerSmallPayload.showMostPopular = showMostPopular;
		storeContainerSmallPayload.priceText = localizedPrice;
		storeContainerSmallPayload.oldValueColor = color;
		storeContainerSmallPayload.oldValueEffectColor = lightShadow;
		storeContainerSmallPayload.lineThroughColor = lineThroughColor;
		storeContainerSmallPayload.lineThroughShadowColor = lineThroughShadowColor;
		storeContainerSmallPayload.showSale = showSale;
		storeContainerSmallPayload.bannerTopColor = bannerTopColor;
		storeContainerSmallPayload.bannerBottomColor = bannerBottomColor;
		storeContainerSmallPayload.oldValueText = empty;
		storeContainerSmallPayload.starGlintPositions = starGlintPositions;
		storeContainerSmallPayload.onButtonClicked = () =>
		{
			InAppManager.instance.StartPurchase(itemId);
		};
		return storeContainerSmallPayload;
	}

	private static string FormatValueText(int value)
	{
		if (value > 99999)
		{
			return value.FormatSI();
		}
		return value.ToString();
	}
}
