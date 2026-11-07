using Info;
using Info.VideoRewardCooldowns;
using UI.Popups.ConsumablesPopupInternal;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.FreeStuff
{
	public static class RewardForVideoButtonHelper
	{
		public static VideoButtonPayload coinsForVideo = new VideoButtonPayload
		{
			iconName = "coin_pack_4",
			title = "COINS_FOR_AD_BUTTON_POPUP_TITLE",
			description = "COINS_FOR_AD_BUTTON_DESCRIPTION",
			isAvailable = () => CooldownManager.HasCharge("coins"),
			onVideoWatched = () =>
			{
				CooldownManager.Consume("coins");
			},
			getRewardAmount = CoinsForVideoManager.instance.GetRewardAmount,
			adLocation = AdLocations.FREE_COINS_FREE_VIDEOS,
			rewardType = CelebrationRewardType.coins,
			unlockItemSource = UnlockItemSource.FreeCoins,
			firstColor = ColorData.instance.peacock,
			firstColorDark = ColorData.instance.royale,
			firstHighlightColor = SkyHighlight(),
			secondColor = ColorData.instance.dutch,
			secondColorDark = ColorData.instance.safran,
			secondHighlightColor = SunHighlight(),
			titleColor = ColorData.instance.sun,
			descriptionColor = ColorData.instance.white,
			flurryName = "Coins"
		};

		public static VideoButtonPayload scoreBoosterForVideo = new VideoButtonPayload
		{
			iconName = "icon_upgrades_scorebooster",
			title = "FREE_STUFF_VIDEO_REWARD_SCORE_BOOSTER_TITLE",
			description = "FREE_STUFF_VIDEO_REWARD_SCORE_BOOSTER_DESCRIPTION",
			isAvailable = () => CooldownManager.HasCharge("score_booster"),
			onVideoWatched = () =>
			{
				CooldownManager.Consume("score_booster");
			},
			getRewardAmount = () => OnlineSettings.instance.GetValue(SellConsumablesHelper.videoRewardAmountOSKeys[UpgradeType.scorebooster], SellConsumablesHelper.defaultVideoRewardAmounts[UpgradeType.scorebooster]),
			adLocation = AdLocations.SCOREBOOSTER_FREE_VIDEOS,
			rewardType = CelebrationRewardType.powerup,
			upgradeType = UpgradeType.scorebooster,
			unlockItemSource = UnlockItemSource.FreeScoreBooster,
			firstColor = ColorData.instance.peacock,
			firstColorDark = ColorData.instance.royale,
			firstHighlightColor = SkyHighlight(),
			secondColor = ColorData.instance.dutch,
			secondColorDark = ColorData.instance.safran,
			secondHighlightColor = SunHighlight(),
			titleColor = ColorData.instance.sun,
			descriptionColor = ColorData.instance.white,
			flurryName = "Score Booster"
		};

		public static VideoButtonPayload hoverboardForVideo = new VideoButtonPayload
		{
			iconName = "icon_upgrades_hoverboard",
			title = "FREE_STUFF_VIDEO_REWARD_HOVERBOARD_TITLE",
			description = "FREE_STUFF_VIDEO_REWARD_HOVERBOARD_FORMAT",
			isAvailable = () => CooldownManager.HasCharge("hoverboard"),
			onVideoWatched = () =>
			{
				CooldownManager.Consume("hoverboard");
			},
			getRewardAmount = () => OnlineSettings.instance.GetValue(SellConsumablesHelper.videoRewardAmountOSKeys[UpgradeType.hoverboard], SellConsumablesHelper.defaultVideoRewardAmounts[UpgradeType.hoverboard]),
			adLocation = AdLocations.HOVERBOARD_FREE_VIDEOS,
			rewardType = CelebrationRewardType.powerup,
			upgradeType = UpgradeType.hoverboard,
			unlockItemSource = UnlockItemSource.FreeHoverboards,
			firstColor = ColorData.instance.peacock,
			firstColorDark = ColorData.instance.royale,
			firstHighlightColor = SkyHighlight(),
			secondColor = ColorData.instance.dutch,
			secondColorDark = ColorData.instance.safran,
			secondHighlightColor = SunHighlight(),
			titleColor = ColorData.instance.sun,
			descriptionColor = ColorData.instance.white,
			flurryName = "Hoverboards"
		};

		public static VideoButtonPayload headstartForVideo = new VideoButtonPayload
		{
			iconName = "icon_upgrades_headstart2000",
			title = "FREE_STUFF_VIDEO_REWARD_HEADSTART_TITLE",
			description = "FREE_STUFF_VIDEO_REWARD_HEADSTART_DESCRIPTION",
			isAvailable = () => CooldownManager.HasCharge("headstart"),
			onVideoWatched = () =>
			{
				CooldownManager.Consume("headstart");
			},
			getRewardAmount = () => OnlineSettings.instance.GetValue(SellConsumablesHelper.videoRewardAmountOSKeys[UpgradeType.headstart2000], SellConsumablesHelper.defaultVideoRewardAmounts[UpgradeType.headstart2000]),
			adLocation = AdLocations.HEADSTART_FREE_VIDEOS,
			rewardType = CelebrationRewardType.powerup,
			upgradeType = UpgradeType.headstart2000,
			unlockItemSource = UnlockItemSource.FreeHeadstart,
			firstColor = ColorData.instance.peacock,
			firstColorDark = ColorData.instance.royale,
			firstHighlightColor = SkyHighlight(),
			secondColor = ColorData.instance.dutch,
			secondColorDark = ColorData.instance.safran,
			secondHighlightColor = SunHighlight(),
			titleColor = ColorData.instance.sun,
			descriptionColor = ColorData.instance.white,
			flurryName = "Headstart"
		};

		private static Color32 SkyHighlight()
		{
			Color32 result = ColorData.instance.sky;
			result.a = 153;
			return result;
		}

		private static Color32 SunHighlight()
		{
			Color32 result = ColorData.instance.sun;
			result.a = 102;
			return result;
		}
	}
}
