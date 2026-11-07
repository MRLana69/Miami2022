using Info;
using UI.Screens.RewardManagement;

namespace UI.VideoAdPrizes
{
	public class VideoAdPrize
	{
		public VideoAdPrizeType prizeType { get; private set; }

		private int amount { get; set; }

		public bool hasBeenRewarded { get; private set; }

		public VideoAdPrize(VideoAdPrizeType prizeType, int amount = 1)
		{
			this.prizeType = prizeType;
			this.amount = amount;
			hasBeenRewarded = false;
		}

		private CelebrationReward CreateCelebrationReward()
		{
			switch (prizeType)
			{
			case VideoAdPrizeType.SuperMysteryBox:
				return CreateMysteryBox(MysteryBox.Type.Super, CelebrationRewardOrigin.SuperMysteryBox);
			case VideoAdPrizeType.MysteryBox:
				return CreateMysteryBox(MysteryBox.Type.Normal, CelebrationRewardOrigin.MysteryBox);
			case VideoAdPrizeType.MiniMysteryBox:
				return CreateMysteryBox(MysteryBox.Type.Mini, CelebrationRewardOrigin.MysteryBoxMini);
			case VideoAdPrizeType.Keys:
			case VideoAdPrizeType.DailyVideoPrize:
				return CreateCurrencyReward(amount, CelebrationRewardType.keys);
			case VideoAdPrizeType.Coins:
				return CreateCurrencyReward(amount, CelebrationRewardType.coins);
			default:
				SDebug.LogError(string.Concat("The prize type ", prizeType, " was out of range, of the defined prizes"));
				return new CelebrationReward();
			}
		}

		private CelebrationReward CreateMysteryBox(MysteryBox.Type mysteryBoxType, CelebrationRewardOrigin celebrationRewardOrigin)
		{
			CelebrationReward celebrationReward = MysteryBox.Roll(mysteryBoxType);
			celebrationReward.CelebrationRewardOrigin = celebrationRewardOrigin;
			celebrationReward.syboAnalyticsSource = ObtainableItemSource.RewardedVideo;
			return celebrationReward;
		}

		private CelebrationReward CreateCurrencyReward(int amountOfCurrencyToGive, CelebrationRewardType celebrationRewardType)
		{
			CelebrationReward celebrationReward = new CelebrationReward();
			celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.UnlockItem;
			celebrationReward.amount = amountOfCurrencyToGive;
			celebrationReward.rewardType = celebrationRewardType;
			return celebrationReward;
		}

		public void GiveReward(UnlockItemSource rewardSource)
		{
			hasBeenRewarded = true;
			if (prizeType == VideoAdPrizeType.DailyVideoPrize)
			{
				Stat.VideoAdsWatchedForKeys.IncrementValue();
			}
			CelebrationReward celebrationReward = CreateCelebrationReward();
			celebrationReward.unlockItemSource = rewardSource;
			celebrationReward.syboAnalyticsSourceMetaOrNull = rewardSource.ToString();
			RewardManager.AddRewardToUnlock(celebrationReward);
			UIScreenController.Instance.PushCelebrationPopup();
		}

		public string AsString()
		{
			string text = GetPrizeTypeAsString();
			if (amount > 1)
			{
				text = text + "_" + amount;
			}
			return text;
		}

		private string GetPrizeTypeAsString()
		{
			if (prizeType == VideoAdPrizeType.DailyVideoPrize)
			{
				return VideoAdPrizeType.Keys.ToString();
			}
			return prizeType.ToString();
		}
	}
}
