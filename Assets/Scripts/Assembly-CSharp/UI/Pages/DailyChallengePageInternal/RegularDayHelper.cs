using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UI.Generic.Layout;
using UnityEngine;
using WordHunt;

namespace UI.Pages.DailyChallengePageInternal
{
	public class RegularDayHelper : DayHelper
	{
		[SerializeField]
		private UILabel _dayLabel;

		[SerializeField]
		private UILabel _rewardLabel;

		[SerializeField]
		private UISprite _checkMark;

		[SerializeField]
		private UISprite _divider;

		[SerializeField]
		private UISprite _backgroundActive;

		[SerializeField]
		private UISprite _rewardCoinIcon;

		[SerializeField]
		private Layout _rewardLayout;

		[SerializeField]
		private GameObject[] _rewards;

		private int _dayIndex;

		public void SetIndex(int index)
		{
			_dayIndex = index;
		}

		public override void Init()
		{
			WordHuntManager.WordHuntReward rewardForDay = WordHuntManager.GetRewardForDay(_dayIndex);
			_backgroundActive.gradientTop = ColorData.instance.wordHuntCurrentDayGradientTop;
			_backgroundActive.gradientBottom = ColorData.instance.wordHuntCurrentDayGradientBottom;
			_rewardCoinIcon.gameObject.SetActive(rewardForDay.type == WordHuntManager.WordHuntRewardType.Coins);
			if (rewardForDay.type == WordHuntManager.WordHuntRewardType.Coins)
			{
				_rewardLabel.text = rewardForDay.coins.ToString();
			}
			else if (rewardForDay.type == WordHuntManager.WordHuntRewardType.MysteryBox)
			{
				_rewardLabel.text = Strings.Get("DAILY_CHALLENGE_MYSTERY_BOX");
			}
			for (int i = 0; i < _rewards.Length; i++)
			{
				_rewards[i].SetActive(false);
			}
			_rewards[_dayIndex].SetActive(true);
			_rewardLayout.Align();
			RefreshDay();
		}

		[Button(null)]
		public override void RefreshDay()
		{
			SetupDay(WordHuntManager.GetWordHuntPayload());
		}

		[ContextMenu("Complete Today")]
		[Button(null)]
		private void CompleteToday()
		{
			SetupDay(new WordHuntManager.WordHuntPayload
			{
				activeDayIndex = _dayIndex,
				todayCompleted = true
			});
		}

		[ContextMenu("Activate Today")]
		[Button(null)]
		private void ActivateToday()
		{
			SetupDay(new WordHuntManager.WordHuntPayload
			{
				activeDayIndex = _dayIndex,
				todayCompleted = false
			});
		}

		[ContextMenu("Deactivate Today")]
		[Button(null)]
		private void DeactivateToday()
		{
			SetupDay(new WordHuntManager.WordHuntPayload
			{
				activeDayIndex = 1,
				todayCompleted = false
			});
		}

		private bool ShouldActivateDivider(int dayIndex, int activeDayIndex)
		{
			return (!activeDayIndex.IsBetween(dayIndex - 1, dayIndex + 1) || activeDayIndex > 3) && dayIndex != 3;
		}

		private void SetupDay(WordHuntManager.WordHuntPayload huntPayload)
		{
			_divider.gameObject.SetActive(ShouldActivateDivider(_dayIndex, huntPayload.activeDayIndex));
			_backgroundActive.gameObject.SetActive(huntPayload.activeDayIndex == _dayIndex);
			if (huntPayload.activeDayIndex == _dayIndex)
			{
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_TODAY_2");
				_dayLabel.color = ColorData.instance.white;
				_dayLabel.effectStyle = UILabel.Effect.Shadow;
				_checkMark.gameObject.SetActive(huntPayload.todayCompleted);
			}
			else if (huntPayload.activeDayIndex < _dayIndex)
			{
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_DAY").Populate(_dayIndex + 1);
				_dayLabel.color = ColorData.instance.midnight;
				_dayLabel.effectStyle = UILabel.Effect.None;
				_checkMark.gameObject.SetActive(false);
			}
			else
			{
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_DAY").Populate(_dayIndex + 1);
				_dayLabel.color = ColorData.instance.midnight;
				_dayLabel.effectStyle = UILabel.Effect.None;
				_checkMark.gameObject.SetActive(true);
			}
		}
	}
}
