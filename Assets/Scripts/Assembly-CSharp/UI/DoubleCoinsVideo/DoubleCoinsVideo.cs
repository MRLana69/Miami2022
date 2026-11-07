using System;
using Info;
using Kiloo.Common.Extensions;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.DoubleCoinsVideo
{
	public abstract class DoubleCoinsVideo : MonoBehaviour
	{
		public static Action<CelebrationReward> onDoubleCoinsReceived;

		protected DoubleCoinsVideoPayload _payload;

		private int _amount;

		private const string DOUBLE_COINS_VIDEO_PAYOUT_CAP_KEY = "DoubleCoinsVideoPayoutCap";

		private const int DEFAULT_CAP = 3000;

		public static int payoutCap
		{
			get
			{
				int num = OnlineSettings.instance.GetValue("DoubleCoinsVideoPayoutCap", 3000);
				if (num < 1)
				{
					num = int.MaxValue;
				}
				return num;
			}
		}

		public abstract void Setup(DoubleCoinsVideoPayload payload);

		public virtual void SetCoinVisualValue(int coinValue)
		{
			_amount = coinValue;
		}

		protected void WatchVideo()
		{
			AdSystemManager.Show(AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS, () =>
			{
				_payload.onWatchVideo.SafeInvoke();
				if (onDoubleCoinsReceived != null)
				{
					CelebrationReward argument = new CelebrationReward
					{
						CelebrationRewardOrigin = CelebrationRewardOrigin.UnlockItem,
						rewardType = CelebrationRewardType.doubleCoinsVideo,
						syboAnalyticsSourceMetaOrNull = UnlockItemSource.GameOver.ToString(),
						unlockItemSource = UnlockItemSource.GameOver,
						amount = _amount
					};
					onDoubleCoinsReceived.SafeInvoke(argument);
				}
			});
		}
	}
}
