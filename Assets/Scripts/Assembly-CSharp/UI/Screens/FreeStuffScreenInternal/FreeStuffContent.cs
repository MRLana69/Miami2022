using Info;
using UI.FreeStuff;

namespace UI.Screens.FreeStuffScreenInternal
{
	public static class FreeStuffContent
	{
		public static BigFreeStuffContainerPayload facebookLogin = new BigFreeStuffContainerPayload
		{
			containerType = BigShopContainerType.FacebookLogin,
			bgGradientTopColor = NGUIMath.HexToColor(795667711u),
			bgGradientBottomColor = NGUIMath.HexToColor(2093809663u),
			outlineGradientTopColor = NGUIMath.HexToColor(154958847u),
			outlineGradientBottomColor = NGUIMath.HexToColor(964539647u),
			glowColor = NGUIMath.HexToColor(3422158847u),
			titleTextColor = ColorData.instance.midnight,
			titleTextShadow = NGUIMath.HexToColor(2612721663u),
			descriptionTextColor = NGUIMath.HexToColor(uint.MaxValue),
			descriptionTextShadow = ColorData.instance.shadow,
			headerString = "FACEBOOK_GIFT",
			descriptionString = "FREE_STUFF_FACEBOOK_GIFT_DESC",
			buttonString = "SOCIAL_BUTTON_LOGIN",
			buttonLoadingString = "SOCIAL_BUTTON_FEEDBACK_CONNECTING",
			clickAction = () =>
			{
				FreeStuffFBManager.instance.FacebookLoginClicked();
			}
		};

		public static BigFreeStuffContainerPayload facebookGift = new BigFreeStuffContainerPayload
		{
			containerType = BigShopContainerType.FacebookGift,
			bgGradientTopColor = NGUIMath.HexToColor(795667711u),
			bgGradientBottomColor = NGUIMath.HexToColor(2093809663u),
			outlineGradientTopColor = NGUIMath.HexToColor(154958847u),
			outlineGradientBottomColor = NGUIMath.HexToColor(964539647u),
			glowColor = NGUIMath.HexToColor(3422158847u),
			titleTextColor = ColorData.instance.midnight,
			titleTextShadow = NGUIMath.HexToColor(2612721663u),
			descriptionTextColor = NGUIMath.HexToColor(uint.MaxValue),
			descriptionTextShadow = ColorData.instance.shadow,
			iconSpriteName = "icon_bigSuperMysteryBox",
			headerString = "FACEBOOK_GIFT",
			descriptionString = "EARN_COINS_FACEBOOKGIFT_DESCRIPTION",
			buttonString = "FREE_STUFF_FACEBOOK_GIFT_BUTTON",
			buttonLoadingString = "SOCIAL_BUTTON_FEEDBACK_CONNECTING",
			clickAction = () =>
			{
				EarnCoinsData.Trigger(EarnCoinsData.facebookGift);
				FreeStuffScreen.updateScreen = true;
			}
		};

		public static BigFreeStuffContainerPayload coinoffer = new BigFreeStuffContainerPayload
		{
			containerType = BigShopContainerType.CoinOffers,
			bgGradientTopColor = NGUIMath.HexToColor(4286841343u),
			bgGradientBottomColor = NGUIMath.HexToColor(4291559679u),
			outlineGradientTopColor = NGUIMath.HexToColor(3596747007u),
			outlineGradientBottomColor = NGUIMath.HexToColor(3985834239u),
			glowColor = NGUIMath.HexToColor(4294084095u),
			titleTextColor = ColorData.instance.mahogany,
			titleTextShadow = NGUIMath.HexToColor(4291199231u),
			descriptionTextColor = NGUIMath.HexToColor(uint.MaxValue),
			descriptionTextShadow = ColorData.instance.shadow,
			iconSpriteName = "Icon_coin_pouch_larger",
			headerString = "TAPJOY_OFFER_TITLE",
			descriptionString = "TAPJOY_OFFER_DESCRIPTION",
			buttonString = "FREE_STUFF_COIN_OFFERS_BUTTON",
			buttonLoadingString = "SOCIAL_BUTTON_FEEDBACK_CONNECTING",
			clickAction = () =>
			{
				OfferWallManager.instance.ShowOfferWall("Earn Coins Screen");
			}
		};

		public static BigFreeStuffContainerPayload coinsForVideo = new BigFreeStuffContainerPayload
		{
			containerType = BigShopContainerType.FreeCoins,
			bgGradientTopColor = NGUIMath.HexToColor(12295679u),
			bgGradientBottomColor = NGUIMath.HexToColor(2515913471u),
			outlineGradientTopColor = NGUIMath.HexToColor(92634623u),
			outlineGradientBottomColor = NGUIMath.HexToColor(297382911u),
			glowColor = NGUIMath.HexToColor(3724541439u),
			titleTextColor = NGUIMath.HexToColor(4999423u),
			titleTextShadow = NGUIMath.HexToColor(2363675391u),
			descriptionTextColor = NGUIMath.HexToColor(uint.MaxValue),
			descriptionTextShadow = ColorData.instance.shadow,
			iconSpriteName = "coin_pack_4",
			useBigIconSize = true,
			headerString = "COINS_FOR_AD_BUTTON_POPUP_TITLE",
			descriptionString = "FREE_STUFF_COINS_FOR_AD_DESC",
			buttonString = "FREE_STUFF_WATCH_VIDEO",
			buttonLoadingString = "EARN_COINS_LOADING",
			adPayload = RewardForVideoButtonHelper.coinsForVideo
		};

		public static SmallFreeStuffContainerPayload scoreBooster = new SmallFreeStuffContainerPayload
		{
			bgGradientTopColor = NGUIMath.HexToColor(42258943u),
			bgGradientBottomColor = NGUIMath.HexToColor(15073279u),
			outlineGradientTopColor = NGUIMath.HexToColor(358124031u),
			outlineGradientBottomColor = NGUIMath.HexToColor(459974143u),
			glowColor = NGUIMath.HexToColor(uint.MaxValue),
			titleTextColor = ColorData.instance.midnight,
			titleTextShadow = ColorData.instance.lightShadow,
			descriptionTextColor = NGUIMath.HexToColor(5143295u),
			descriptionTextShadow = ColorData.instance.lightShadow,
			iconSpriteName = "icon_upgrade_score_booster",
			headerString = "FREE_STUFF_SCORE_BOOSTER",
			descriptionString = "x1",
			buttonString = "FREE",
			buttonLoadingString = "EARN_COINS_LOADING",
			adPayload = RewardForVideoButtonHelper.scoreBoosterForVideo
		};

		public static SmallFreeStuffContainerPayload headstart = new SmallFreeStuffContainerPayload
		{
			bgGradientTopColor = NGUIMath.HexToColor(42258943u),
			bgGradientBottomColor = NGUIMath.HexToColor(15073279u),
			outlineGradientTopColor = NGUIMath.HexToColor(358124031u),
			outlineGradientBottomColor = NGUIMath.HexToColor(459974143u),
			glowColor = NGUIMath.HexToColor(uint.MaxValue),
			titleTextColor = ColorData.instance.midnight,
			titleTextShadow = ColorData.instance.lightShadow,
			descriptionTextColor = NGUIMath.HexToColor(5143295u),
			descriptionTextShadow = ColorData.instance.lightShadow,
			iconSpriteName = "icon_upgrade_headstart",
			headerString = "FREE_STUFF_VIDEO_REWARD_HEADSTART_TITLE",
			descriptionString = "x1",
			buttonString = "FREE",
			buttonLoadingString = "EARN_COINS_LOADING",
			adPayload = RewardForVideoButtonHelper.headstartForVideo
		};

		public static SmallFreeStuffContainerPayload hoverboard = new SmallFreeStuffContainerPayload
		{
			bgGradientTopColor = NGUIMath.HexToColor(42258943u),
			bgGradientBottomColor = NGUIMath.HexToColor(15073279u),
			outlineGradientTopColor = NGUIMath.HexToColor(358124031u),
			outlineGradientBottomColor = NGUIMath.HexToColor(459974143u),
			glowColor = NGUIMath.HexToColor(uint.MaxValue),
			titleTextColor = ColorData.instance.midnight,
			titleTextShadow = ColorData.instance.lightShadow,
			descriptionTextColor = NGUIMath.HexToColor(5143295u),
			descriptionTextShadow = ColorData.instance.lightShadow,
			iconSpriteName = "icon_upgrade_hoverboard",
			headerString = "FREE_STUFF_VIDEO_REWARD_HOVERBOARD_TITLE",
			descriptionString = "x3",
			buttonString = "FREE",
			buttonLoadingString = "EARN_COINS_LOADING",
			adPayload = RewardForVideoButtonHelper.hoverboardForVideo
		};
	}
}
