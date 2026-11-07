using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;
using WordHunt;

namespace UI.Pages.DailyChallengePageInternal
{
	public class FinalDayHelper : DayHelper
	{
		private class DailyWordPayload
		{
			public bool mostRecentlyCompletedIsToday;

			public int activeDayIndex;
		}

		[SerializeField]
		private UILabel _dayLabel;

		[SerializeField]
		private UILabel _rewardLabel;

		[SerializeField]
		private UISprite _checkMark;

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private Color _activeLabelColor;

		[SerializeField]
		private Color _activeLabelShadowColor;

		[SerializeField]
		private Color _inactiveLabelColor;

		[SerializeField]
		private Color _inactiveLabelShadowColor;

		[Header("Effects")]
		[SerializeField]
		private UISprite[] _beams;

		[SerializeField]
		private UISprite _glow;

		private const int DEFAULT_CHECKMARK_SIZE = 40;

		private const int MAX_CHECKMARK_SIZE = 100;

		private const int MIN_CHECKMARK_SIZE = 34;

		private const float DEFAULT_ASPECT = 1.5f;

		public override void Init()
		{
			RefreshDay();
		}

		[Button(null)]
		public override void RefreshDay()
		{
			SetupDay(GetData());
		}

		[ContextMenu("Complete Today")]
		[Button(null)]
		private void CompleteToday()
		{
			SetupDay(new DailyWordPayload
			{
				activeDayIndex = 5,
				mostRecentlyCompletedIsToday = true
			});
		}

		[ContextMenu("Activate Today")]
		[Button(null)]
		private void ActivateToday()
		{
			SetupDay(new DailyWordPayload
			{
				activeDayIndex = 5,
				mostRecentlyCompletedIsToday = false
			});
		}

		[ContextMenu("Deactivate Today")]
		[Button(null)]
		private void DeactivateToday()
		{
			SetupDay(new DailyWordPayload
			{
				activeDayIndex = 1,
				mostRecentlyCompletedIsToday = false
			});
		}

		private DailyWordPayload GetData()
		{
			bool todayCompleted;
			int num = Mathf.Clamp(WordHuntManager.GetWordHuntDaysInARow(out todayCompleted), 0, 5);
			int activeDayIndex = ((!todayCompleted) ? num : (num - 1));
			DailyWordPayload dailyWordPayload = new DailyWordPayload();
			dailyWordPayload.activeDayIndex = activeDayIndex;
			dailyWordPayload.mostRecentlyCompletedIsToday = todayCompleted;
			return dailyWordPayload;
		}

		private void SetupDay(DailyWordPayload payload)
		{
			_rewardLabel.text = Strings.Get("DAILY_CHALLENGE_SUPER_MYSTERY_BOX");
			if (payload.activeDayIndex >= 4)
			{
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_TODAY_2");
				_dayLabel.color = _activeLabelColor;
				_dayLabel.effectColor = _activeLabelShadowColor;
				_rewardLabel.color = _activeLabelColor;
				_rewardLabel.effectColor = _activeLabelShadowColor;
				_background.color = ColorData.instance.lime;
				_checkMark.gameObject.SetActive(payload.mostRecentlyCompletedIsToday);
				if (_checkMark.gameObject.activeInHierarchy)
				{
					_checkMark.height = Mathf.Clamp((40f * (1.5f - NGUITools.screenSize.x / NGUITools.screenSize.y)).RoundToInt(), 34, 100);
				}
			}
			else
			{
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_DAY_5");
				_dayLabel.color = _inactiveLabelColor;
				_dayLabel.effectColor = _inactiveLabelShadowColor;
				_rewardLabel.color = _inactiveLabelColor;
				_rewardLabel.effectColor = _inactiveLabelShadowColor;
				_background.color = ColorData.instance.yolk;
				_checkMark.gameObject.SetActive(false);
			}
			UISprite[] beams = _beams;
			foreach (UISprite uISprite in beams)
			{
				uISprite.color = ColorData.instance.stripeColor;
			}
			_glow.color = ColorData.instance.glowColor;
		}

		public void Hide2DOverlaySprites()
		{
			_checkMark.gameObject.SetActive(false);
		}
	}
}
