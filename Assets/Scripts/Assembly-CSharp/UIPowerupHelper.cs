using System;
using Data.Save;
using Info;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using UnityEngine;

public class UIPowerupHelper : MonoBehaviour
{
	private enum FadeTarget
	{
		white = 0,
		green = 1,
		darkgreen = 2,
		none = 3
	}

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UISprite sliderBG;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite iconBG;

	[SerializeField]
	private UISprite iconBGMystery;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite foreground;

	[SerializeField]
	private GameObject container;

	private Color originalColor;

	private float sliderSteps;

	private bool _amountLabelRepositioned;

	private const float AMOUNT_LABEL_HIGH_RES_OFFSET = 0.5f;

	private const float COLOR_CHANGE_SPEED = 8.2343f;

	private IngameScreen _ingameScreen;

	private bool _inverse;

	private ActivePowerup _powerup;

	private Color32[] _colorTargets = new Color32[3]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(149, 192, 34, byte.MaxValue),
		new Color32(84, 118, 64, byte.MaxValue)
	};

	private FadeTarget currentFadeTarget = FadeTarget.none;

	private FadeTarget oldFadeTarget = FadeTarget.none;

	private Color storedColor;

	private float lerpTime = 1f;

	private IngameScreen ingameScreen
	{
		get
		{
			if (_ingameScreen == null)
			{
				_ingameScreen = IngameScreen.instance;
			}
			return _ingameScreen;
		}
	}

	private void Awake()
	{
		originalColor = ingameScreen.MULTIPLIER_LABEL_ORIGINAL_COLOR;
	}

	private void OnDisable()
	{
		ReturnToNormal();
	}

	private void OnEnable()
	{
		sliderSteps = (int)slider.foregroundWidget.localSize.x;
	}

	private void ReturnToNormal()
	{
		lerpTime = 1f;
		_inverse = false;
		if (PlusMultiplier.plusMultiplierEnabled)
		{
			ingameScreen.multiplierLabel.color = GlobalColors.MULTIPLIER_LIGHT_PURPLE;
		}
		else if (GameStats.Instance.ScoreBoosterActivated || GameStats.Instance.CharacterDayEventActivated)
		{
			ingameScreen.multiplierLabel.color = ingameScreen.SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (ingameScreen.multiplierLabel.color != originalColor)
		{
			ingameScreen.multiplierLabel.color = originalColor;
		}
	}

	public void HidePowerupSlot()
	{
		if (_powerup != null && _powerup.type == UpgradeType.doubleMultiplier)
		{
			ReturnToNormal();
		}
	}

	public void setContainerPosition(float x, float y, float z)
	{
		container.transform.localPosition = new Vector3(x, y, z);
	}

	public int getSliderWidth()
	{
		return sliderBG.width + iconBG.width - 5;
	}

	public float getSliderHeight()
	{
		return iconBG.height;
	}

	public void SetPowerupSlot(ActivePowerup powerup)
	{
		_powerup = powerup;
		if (_powerup.type == UpgradeType.plusMultiplier || _powerup.type == UpgradeType.doubleJump || _powerup.type == UpgradeType.doubleCoinsMysteryPowerup || (_powerup.type == UpgradeType.hoverboard && Hoverboard.Instance.IsARewardFromMysteryPowerup))
		{
			iconBG.enabled = false;
			iconBGMystery.enabled = true;
		}
		else
		{
			iconBG.enabled = true;
			iconBGMystery.enabled = false;
		}
		if (powerup.type == UpgradeType.hoverboard && Hoverboard.Instance.IsARewardFromMysteryPowerup)
		{
			icon.spriteName = MysteryPowerup.GetRandomBoardIconName();
		}
		else
		{
			icon.spriteName = Upgrades.upgrades[powerup.type].iconNameIngame;
		}
		icon.MakePixelPerfect();
		FlyingState flyingState = GameStateMachine.Instance.TryGetCurrentState<FlyingState>();
		float num = ((flyingState == null || (powerup.type != UpgradeType.jetpack && powerup.type != UpgradeType.headstart2000)) ? (powerup.timeLeft / Upgrades.GetUpgradeDuration(powerup.type)) : flyingState.GetTotalRemainingRatio());
		if (sliderSteps != 0f)
		{
			slider.value = num * sliderSteps / sliderSteps;
		}
		else
		{
			sliderSteps = slider.foregroundWidget.localSize.x;
		}
		if (powerup.type == UpgradeType.hoverboard && !Hoverboard.Instance.IsARewardFromMysteryPowerup)
		{
			amountLabel.text = InventorySave.GetAmount(powerup.type).ToString();
			amountLabel.gameObject.SetActive(true);
			if (DeviceInfo.Instance.isHighres && !_amountLabelRepositioned)
			{
				amountLabel.transform.localPosition = new Vector3(amountLabel.transform.localPosition.x + 0.5f, amountLabel.transform.localPosition.y, amountLabel.transform.localPosition.z);
				_amountLabelRepositioned = true;
			}
		}
		else
		{
			amountLabel.gameObject.SetActive(false);
		}
		if (powerup.type == UpgradeType.doubleMultiplier)
		{
			FadingToWhiteToGreenToDarkGreen(_inverse);
		}
		if (powerup.timeLeft < 0f)
		{
			if (slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(slider.gameObject, false);
			}
			icon.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * (float)Math.PI * 4f));
			if (iconBG.enabled)
			{
				iconBG.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * (float)Math.PI * 4f));
			}
			else
			{
				iconBGMystery.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * (float)Math.PI * 4f));
			}
		}
		else
		{
			if (!slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(slider.gameObject, true);
			}
			icon.color = Color.white;
			if (iconBG.enabled)
			{
				iconBG.color = Color.white;
			}
			else
			{
				iconBGMystery.color = Color.white;
			}
		}
	}

	private void FadingToWhiteToGreenToDarkGreen(bool sendInverse)
	{
		if (sendInverse)
		{
			if (lerpTime <= 1f)
			{
				currentFadeTarget = FadeTarget.green;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = ingameScreen.multiplierLabel.color;
				}
				ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, _colorTargets[(int)currentFadeTarget], lerpTime);
			}
			else if (lerpTime <= 2f)
			{
				currentFadeTarget = FadeTarget.white;
				if (currentFadeTarget != oldFadeTarget)
				{
					storedColor = ingameScreen.multiplierLabel.color;
				}
				ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, _colorTargets[(int)currentFadeTarget], lerpTime - 1f);
			}
			else
			{
				currentFadeTarget = FadeTarget.none;
				_inverse = false;
				lerpTime = 0f;
			}
		}
		else if (lerpTime <= 1f)
		{
			currentFadeTarget = FadeTarget.green;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = ingameScreen.multiplierLabel.color;
			}
			ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, _colorTargets[(int)currentFadeTarget], lerpTime);
		}
		else if (lerpTime <= 2f)
		{
			currentFadeTarget = FadeTarget.darkgreen;
			if (currentFadeTarget != oldFadeTarget)
			{
				storedColor = ingameScreen.multiplierLabel.color;
			}
			ingameScreen.multiplierLabel.color = Color.Lerp(storedColor, _colorTargets[(int)currentFadeTarget], lerpTime - 1f);
		}
		else
		{
			currentFadeTarget = FadeTarget.none;
			_inverse = true;
			lerpTime = 0f;
		}
		lerpTime += Time.deltaTime * 8.2343f;
		oldFadeTarget = currentFadeTarget;
	}
}
