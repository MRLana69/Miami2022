using System;
using Info;
using Kiloo.Common.Ads;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.FreeStuff
{
	public class VideoButtonPayload
	{
		public string iconBackgroundName { get; set; }

		public string iconName { get; set; }

		public Action onVideoWatched { get; set; }

		public Action onButtonClicked { get; set; }

		public Func<bool> isAvailable { get; set; }

		public Func<int> getRewardAmount { get; set; }

		public string title { get; set; }

		public string description { get; set; }

		public AdLocationInfo adLocation { get; set; }

		public CelebrationRewardType rewardType { get; set; }

		public UpgradeType upgradeType { get; set; }

		public UnlockItemSource unlockItemSource { get; set; }

		public Color32 firstColor { get; set; }

		public Color32 firstColorDark { get; set; }

		public Color32 firstHighlightColor { get; set; }

		public Color32 secondColor { get; set; }

		public Color32 secondColorDark { get; set; }

		public Color32 secondHighlightColor { get; set; }

		public Color32 titleColor { get; set; }

		public Color32 descriptionColor { get; set; }

		public string flurryName { get; set; }

		public bool alwaysOn { get; set; }

		public bool autoPlayOnFill { get; set; }
	}
}
