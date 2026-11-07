using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UnityEngine;
using WordHunt;

namespace UI.Screens.PauseScreenInternal
{
	public class DailyChallengeRewardHelper : MonoBehaviour
	{
		[SerializeField]
		private Layout _rewardLayout;

		[SerializeField]
		private UISprite _coinIcon;

		[SerializeField]
		private UISprite _checkmark;

		[SerializeField]
		private UILabel _rewardLabel;

		[SerializeField]
		private GameObject _3BagsReward;

		[SerializeField]
		private GameObject _6BagsReward;

		[SerializeField]
		private GameObject _mysteryBoxReward;

		[SerializeField]
		private GameObject _superMysteryBoxReward;

		public void Refresh()
		{
			WordHuntManager.WordHuntPayload wordHuntPayload = WordHuntManager.GetWordHuntPayload();
			WordHuntManager.WordHuntReward rewardForDay = WordHuntManager.GetRewardForDay(wordHuntPayload.activeDayIndex);
			_mysteryBoxReward.SetActive(wordHuntPayload.activeDayIndex.IsOneOf(0, 1));
			_3BagsReward.SetActive(wordHuntPayload.activeDayIndex == 2);
			_6BagsReward.SetActive(wordHuntPayload.activeDayIndex == 3);
			_superMysteryBoxReward.SetActive(wordHuntPayload.activeDayIndex >= 4);
			_checkmark.gameObject.SetActive(wordHuntPayload.todayCompleted);
			_coinIcon.gameObject.SetActive(rewardForDay.type == WordHuntManager.WordHuntRewardType.Coins);
			if (rewardForDay.type == WordHuntManager.WordHuntRewardType.Coins)
			{
				_rewardLabel.text = rewardForDay.coins.ToString();
			}
			else if (rewardForDay.type == WordHuntManager.WordHuntRewardType.MysteryBox && rewardForDay.mysteryBoxType == MysteryBox.Type.Normal)
			{
				_rewardLabel.text = Strings.Get("DAILY_CHALLENGE_MYSTERY_BOX");
			}
			else
			{
				_rewardLabel.text = Strings.Get("DAILY_CHALLENGE_SUPER_MYSTERY_BOX_SHORT");
			}
			_rewardLayout.Align();
		}
	}
}
