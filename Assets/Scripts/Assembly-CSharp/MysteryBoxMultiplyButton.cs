using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Managers;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using UI;
using UI.FreeStuff;
using UI.Screens.RewardManagement;
using UnityEngine;

public class MysteryBoxMultiplyButton : MonoBehaviour
{
	[SerializeField]
	private RewardForVideoButton _multiplyButton;

	[SerializeField]
	private GameObject _stickerGameObject;

	[SerializeField]
	private Animation _stickerAnimation;

	private const string ONLINESETTINGS_MYSTERY_BOX_DOUBLE_REWARD_CHANCE_KEY = "mystery_box_double_reward_chance";

	private const int DEFAULT_MYSTERY_BOX_DOUBLE_REWARD_CHANCE = 100;

	private const string ONLINESETTINGS_MYSTERY_BOX_DOUBLE_REWARD_ALLOW_KEYS_KEY = "mystery_box_double_reward_allow_keys";

	private const bool DEFAULT_MYSTERY_BOX_DOUBLE_REWARD_ALLOW_KEYS = false;

	private const string ONLINESETTINGS_ALWAYS_ON_AD_BUTTONS_DOUBLE_UP = "ad_buttons_double_up_always_on";

	private const bool ONLINESETTINGS_ALWAYS_ON_AD_BUTTONS_DEFAULT_VALUE = true;

	private CelebrationReward _rewardToMultiply;

	private bool _wasRewardMultiplied;

	private bool _isButtonEnabled;

	private Dictionary<CelebrationRewardType, Action> _multiplyRewardCallbacks = new Dictionary<CelebrationRewardType, Action>(0);

	private Coroutine _animateButtonCoroutine;

	private Coroutine _animateAlphaCoroutine;

	private VideoButtonPayload _videoButtonPayload;

	private bool _hasSetDefaultButtonColorOnce;

	private int chance
	{
		get
		{
			return OnlineSettings.instance.GetValue("mystery_box_double_reward_chance", 100);
		}
	}

	private VideoButtonPayload videoButtonPayload
	{
		get
		{
			if (_videoButtonPayload == null)
			{
				_videoButtonPayload = new VideoButtonPayload
				{
					title = "CELEBRATION_POPUP_DOUBLE_UP",
					isAvailable = () => true,
					onVideoWatched = OnVideoWatched,
					adLocation = AdLocations.DOUBLE_UP_VIDEOS,
					firstColor = ColorData.instance.grass,
					firstColorDark = ColorData.instance.avocado,
					firstHighlightColor = ColorData.instance.lime,
					secondColor = ColorData.instance.grass,
					secondColorDark = ColorData.instance.avocado,
					secondHighlightColor = ColorData.instance.lime,
					titleColor = ColorData.instance.white,
					descriptionColor = ColorData.instance.white,
					alwaysOn = true,
					autoPlayOnFill = true
				};
			}
			return _videoButtonPayload;
		}
	}

	private bool ChanceRoll()
	{
		return chance > UnityEngine.Random.Range(0, 100);
	}

	private bool IsVideoAvailable()
	{
		return AdSystemManager.IsCached(AdLocations.DOUBLE_UP_VIDEOS);
	}

	public bool IsAdButtonAlwaysOnEnabled()
	{
		return OnlineSettings.instance.GetValue("ad_buttons_double_up_always_on", true);
	}

	private bool IsMultipliable(CelebrationReward reward)
	{
		bool result = false;
		switch (reward.rewardType)
		{
		case CelebrationRewardType.keys:
			result = OnlineSettings.instance.GetValue("mystery_box_double_reward_allow_keys", false);
			break;
		case CelebrationRewardType.token:
			result = CharacterManager.IsTokenUseful(reward.characterId, reward, true);
			break;
		case CelebrationRewardType.coins:
			result = true;
			break;
		case CelebrationRewardType.powerup:
			switch (reward.upgradeType)
			{
			case UpgradeType.hoverboard:
				result = true;
				break;
			case UpgradeType.headstart2000:
				result = true;
				break;
			case UpgradeType.scorebooster:
				result = true;
				break;
			}
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	private bool IsValidOrigin(CelebrationReward reward)
	{
		switch (reward.CelebrationRewardOrigin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
		case CelebrationRewardOrigin.MysteryBoxMini:
		case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			return true;
		default:
			return false;
		}
	}

	public bool TryEnable(CelebrationReward rewardToMultiply, Dictionary<CelebrationRewardType, Action> doubleRewardCallbacks)
	{
		_wasRewardMultiplied = false;
		bool flag = IsValidOrigin(rewardToMultiply);
		bool flag2 = IsMultipliable(rewardToMultiply);
		bool flag3 = IsVideoAvailable();
		bool flag4 = ChanceRoll();
		bool flag5 = IsAdButtonAlwaysOnEnabled() || flag3;
		bool flag6 = (_isButtonEnabled = flag && flag2 && flag4 && flag5);
		_multiplyButton.gameObject.SetActive(flag6);
		_multiplyButton.Init(videoButtonPayload);
		if (flag6)
		{
			_rewardToMultiply = rewardToMultiply;
			_multiplyRewardCallbacks = doubleRewardCallbacks;
			if (_animateButtonCoroutine != null)
			{
				CoroutineUtil.StopCoroutine(_animateButtonCoroutine);
			}
			_animateButtonCoroutine = CoroutineUtil.StartCoroutine(AnimateButton());
			SetDefaultButtonColorOnce();
		}
		if (flag && flag2)
		{
			string arg = ((!flag4) ? "No Chance" : "Chance");
			string arg2 = ((!flag3) ? "No Fill" : "Fill");
			string argValue = string.Format("{0} - {1}", arg, arg2);
			Flurry.LogEventWithAParameter("Attempted to show double up button", "Result", argValue);
			MysteryBox.Multipliers multiplierForReward = MysteryBox.GetMultiplierForReward(_rewardToMultiply);
			_multiplyButton.UpdateText(multiplierForReward, _rewardToMultiply);
		}
		return flag6;
	}

	private IEnumerator AnimateButton()
	{
		float alphaDuration = 0.2f;
		float bounceDuration = 1f;
		ResetAnimationState();
		if (_animateAlphaCoroutine != null)
		{
			CoroutineUtil.StopCoroutine(_animateAlphaCoroutine);
		}
		_animateAlphaCoroutine = CoroutineUtil.StartCoroutine(AnimateAlpha(_multiplyButton.GetComponent<UIWidget>(), alphaDuration, 0f, 1f));
		yield return new WaitForSeconds(alphaDuration);
		_stickerGameObject.SetActive(true);
		float p = 0f;
		Time.timeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		while (p < bounceDuration)
		{
			_stickerGameObject.transform.localScale = Vector3.one * Easings.ElasticEaseOut(p / bounceDuration);
			p += Time.deltaTime;
			yield return null;
		}
		_stickerAnimation.Play();
	}

	private void ResetAnimationState()
	{
		_stickerGameObject.SetActive(false);
		_multiplyButton.GetComponent<UIWidget>().alpha = 1f;
		_stickerGameObject.transform.localScale = Vector3.one;
		_stickerAnimation.Stop();
		_stickerAnimation.clip.SampleAnimation(_stickerGameObject, 0f);
	}

	public void DisableButton()
	{
		if (_isButtonEnabled && _rewardToMultiply != null && !_wasRewardMultiplied)
		{
			FlurryLogMultiplyResultAs(false);
		}
		_wasRewardMultiplied = false;
		_isButtonEnabled = false;
		_multiplyButton.gameObject.SetActive(false);
	}

	[UsedImplicitly]
	public void OnVideoWatched()
	{
		_wasRewardMultiplied = true;
		Flurry.LogEvent("Double up button clicked");
		Flurry.LogEvent("Double up button shown");
		DisableButton();
		OnMultipliedReward();
	}

	private void OnMultipliedReward()
	{
		if (_rewardToMultiply != null && IsMultipliable(_rewardToMultiply))
		{
			Action value;
			if (_multiplyRewardCallbacks.TryGetValue(_rewardToMultiply.rewardType, out value) && value != null)
			{
				value();
			}
			FlurryLogMultiplyResultAs(true);
		}
		_rewardToMultiply = null;
	}

	private void OnEnable()
	{
		DisableButton();
		_rewardToMultiply = null;
	}

	private IEnumerator AnimateAlpha(UIWidget widget, float duration, float from, float to)
	{
		widget.alpha = from;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			widget.alpha = Mathf.Lerp(from, to, factor);
			yield return null;
		}
	}

	public void FlurryLogMultiplyResultAs(bool multiplied)
	{
		string arg = ((_rewardToMultiply.rewardType != CelebrationRewardType.powerup) ? _rewardToMultiply.rewardType.ToString() : _rewardToMultiply.upgradeType.ToString());
		string arg2 = ((_rewardToMultiply.rewardType != CelebrationRewardType.coins) ? _rewardToMultiply.amount.ToString() : GroupCoinsForFlurryLogging(_rewardToMultiply.amount));
		string arg3 = string.Empty;
		switch (MysteryBox.GetMultiplierForReward(_rewardToMultiply))
		{
		case MysteryBox.Multipliers.x2:
			arg3 = ((!multiplied) ? "Not Doubled" : "Doubled");
			break;
		case MysteryBox.Multipliers.x3:
			arg3 = ((!multiplied) ? "Not Tripled" : "Tripled");
			break;
		case MysteryBox.Multipliers.x4:
			arg3 = ((!multiplied) ? "Not Quadrupled" : "Quadrupled");
			break;
		}
		string argValue = string.Format("{0} {1} {2}", arg2, arg, arg3);
		Flurry.LogEventWithAParameter("Double up choice", "Result", argValue);
	}

	public string GroupCoinsForFlurryLogging(int originalCoinAmount)
	{
		if (originalCoinAmount < 500)
		{
			int num = originalCoinAmount / 10 * 10;
			int num2 = num + 9;
			return num + "-" + num2;
		}
		return originalCoinAmount.ToString();
	}

	private void SetDefaultButtonColorOnce()
	{
		if (!_hasSetDefaultButtonColorOnce)
		{
			UIButtonColor component = _multiplyButton.GetComponent<UIButtonColor>();
			if (component != null)
			{
				Color defaultColor = component.defaultColor;
				component.defaultColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 1f);
			}
			_hasSetDefaultButtonColorOnce = true;
		}
	}

	[Conditional("MYSTERY_BOX_DOUBLE_REWARD_LOGS")]
	private void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
