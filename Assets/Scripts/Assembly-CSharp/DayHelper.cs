using System;
using JetBrains.Annotations;
using UnityEngine;
using WordHunt;

[Obsolete("This is deprecated. --Nicki.")]
public class DayHelper : MonoBehaviour
{
	[UsedImplicitly]
	public enum DayType
	{
		NotSet = 0,
		Regular = 1,
		LastDay = 2
	}

	[SerializeField]
	private DayType _dayType;

	[SerializeField]
	private UILabel _dayLabel;

	[SerializeField]
	private UILabel _rewardLabel;

	[SerializeField]
	private UILabel _checkBackLabel;

	[SerializeField]
	private UISprite _checkMark;

	[SerializeField]
	private UISprite _divider;

	[SerializeField]
	private UISprite _backgroundInactive;

	[SerializeField]
	private UISprite _backgroundActive;

	[SerializeField]
	private UISprite _checkbackBackground;

	[SerializeField]
	private UISprite[] _gradientEffectSprites;

	[SerializeField]
	private UISprite _glowEffect;

	[SerializeField]
	private bool _forceActive;

	[SerializeField]
	private bool _unlockColors;

	private Color32 _inactiveDayColor = new Color32(9, 42, 83, byte.MaxValue);

	private Color32 _activeDayColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 _inactiveDayShadowColor = new Color32(0, 0, 0, 0);

	private Color32 _activeDayShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 _inactiveRewardColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 _activeRewardColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 _inactiveRewardShadowColor = new Color32(0, 36, 80, byte.MaxValue);

	private Color32 _activeRewardShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 _inactiveLastDayDayColor = new Color32(125, 27, 5, byte.MaxValue);

	private Color32 _activeLastDayDayColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 _inactiveLastDayDayShadowColor = new Color32(0, 0, 0, 0);

	private Color32 _activeLastDayDayShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 _inactiveLastDayRewardShadowColor = new Color32(252, 233, 171, byte.MaxValue);

	private Color32 _activeLastDayRewardShadowColor = new Color32(26, 116, 30, byte.MaxValue);

	private Color32 _activeLastDayBackgroundColor = new Color32(157, 234, 5, byte.MaxValue);

	private Color32 _inactiveLastDayBackgroundColor = new Color32(248, 225, 62, byte.MaxValue);

	private Color32 _activeLastDayGradientColor = new Color32(102, 178, 0, byte.MaxValue);

	private Color32 _inactiveLastDayGradientColor = new Color32(245, 169, 40, byte.MaxValue);

	private Color32 _activeLastDayGlow = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 180);

	private Color32 _inactiveLastDayGlow = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private int _dayIndex = -1;

	public void Init(int dayIndex)
	{
		_dayIndex = dayIndex;
		WordHuntManager.WordHuntReward rewardForDay = WordHuntManager.GetRewardForDay(_dayIndex);
		switch (rewardForDay.type)
		{
		case WordHuntManager.WordHuntRewardType.Coins:
			_rewardLabel.text = rewardForDay.coins.ToString();
			break;
		case WordHuntManager.WordHuntRewardType.MysteryBox:
			_rewardLabel.text = Strings.Get((rewardForDay.mysteryBoxType != MysteryBox.Type.Normal) ? "DAILY_CHALLENGE_SUPER_MYSTERY_BOX" : "DAILY_CHALLENGE_MYSTERY_BOX");
			break;
		default:
			SDebug.LogError("Unhandled reward type: " + rewardForDay.type);
			break;
		}
		_forceActive = false;
		_unlockColors = false;
	}

	public void RefreshDay()
	{
		if (_unlockColors)
		{
			return;
		}
		bool todayCompleted;
		int num = Mathf.Clamp(WordHuntManager.GetWordHuntDaysInARow(out todayCompleted), 0, 5);
		int num2 = ((!todayCompleted) ? num : (num - 1));
		if (_dayType == DayType.LastDay)
		{
			if (num2 >= 4 || _forceActive)
			{
				_backgroundInactive.color = _activeLastDayBackgroundColor;
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_TODAY_1");
				_dayLabel.color = _activeLastDayDayColor;
				_dayLabel.effectColor = _activeLastDayDayShadowColor;
				_rewardLabel.color = _activeLastDayDayColor;
				_rewardLabel.effectColor = _activeLastDayRewardShadowColor;
				if (todayCompleted)
				{
					_checkMark.enabled = true;
					_checkbackBackground.enabled = true;
					_rewardLabel.enabled = false;
					_checkBackLabel.enabled = true;
				}
				else
				{
					_checkMark.enabled = false;
					_checkbackBackground.enabled = false;
					_rewardLabel.enabled = true;
					_checkBackLabel.enabled = false;
				}
				UISprite[] gradientEffectSprites = _gradientEffectSprites;
				foreach (UISprite uISprite in gradientEffectSprites)
				{
					uISprite.color = _activeLastDayGradientColor;
				}
				_glowEffect.color = _activeLastDayGlow;
			}
			else
			{
				_backgroundInactive.color = _inactiveLastDayBackgroundColor;
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_DAY_5");
				_dayLabel.color = _inactiveLastDayDayColor;
				_dayLabel.effectColor = _inactiveLastDayDayShadowColor;
				_rewardLabel.effectColor = _inactiveLastDayRewardShadowColor;
				_checkMark.enabled = false;
				_checkbackBackground.enabled = false;
				_rewardLabel.enabled = true;
				_checkBackLabel.enabled = false;
				UISprite[] gradientEffectSprites2 = _gradientEffectSprites;
				foreach (UISprite uISprite2 in gradientEffectSprites2)
				{
					uISprite2.color = _inactiveLastDayGradientColor;
				}
				_glowEffect.color = _inactiveLastDayGlow;
			}
		}
		else if (_dayType == DayType.Regular)
		{
			if (num2 == _dayIndex)
			{
				_divider.enabled = true;
				_backgroundActive.enabled = true;
				_backgroundInactive.enabled = false;
				_dayLabel.text = Strings.Get("DAILY_CHALLENGE_TODAY_2");
				_dayLabel.color = _activeDayColor;
				_dayLabel.effectColor = _activeDayShadowColor;
				_rewardLabel.color = _activeRewardColor;
				_rewardLabel.effectColor = _activeRewardShadowColor;
				_checkMark.enabled = todayCompleted;
			}
			else if (num2 < _dayIndex)
			{
				_divider.enabled = false;
				_backgroundActive.enabled = false;
				_backgroundInactive.enabled = true;
				_dayLabel.text = string.Format(Strings.Get("DAILY_CHALLENGE_DAY"), _dayIndex + 1);
				_dayLabel.color = _inactiveDayColor;
				_dayLabel.effectColor = _inactiveDayShadowColor;
				_rewardLabel.color = _inactiveRewardColor;
				_rewardLabel.effectColor = _inactiveRewardShadowColor;
				_checkMark.enabled = false;
			}
			else
			{
				_divider.enabled = false;
				_backgroundActive.enabled = false;
				_backgroundInactive.enabled = true;
				_dayLabel.text = string.Format(Strings.Get("DAILY_CHALLENGE_DAY"), _dayIndex + 1);
				_dayLabel.color = _inactiveDayColor;
				_dayLabel.effectColor = _inactiveDayShadowColor;
				_rewardLabel.color = _inactiveRewardColor;
				_rewardLabel.effectColor = _inactiveRewardShadowColor;
				_checkMark.enabled = true;
			}
		}
		else
		{
			SDebug.LogError("Day was not set properly.", base.gameObject);
		}
	}
}
