using System.Collections;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Inputs;
using UnityEngine;

public class SlideinPowerupButton : UIButtonSender
{
	public enum SlideInButtonType
	{
		scorebooster = 0,
		headstart = 1
	}

	private SlideinPowerupHelper _helper;

	private int _powerupNumber = -1;

	[SerializeField]
	private GameObject _headstartModel;

	[SerializeField]
	private UISprite _background;

	private int _counter;

	private const float FADE_OUT_ANIMATION_SPEED_FACTOR = 2f;

	[SerializeField]
	private SlideInButtonType buttonType;

	public void InitSlideinButton(SlideinPowerupHelper helper, int powerupNumber)
	{
		_helper = helper;
		_powerupNumber = powerupNumber;
		_counter = 0;
	}

	private void OnDisable()
	{
		ResetPowerupButtonColorToInitial();
	}

	protected override void Send(bool isPressed)
	{
		if (_powerupNumber == -1)
		{
			SDebug.LogError(base.name + " was used without being initialized", this);
			return;
		}
		if (isPressed)
		{
			_headstartModel.GetComponent<Renderer>().material.SetColor("_Color", new Color32(100, 100, 100, 0));
			_background.color = new Color32(0, 0, 0, 220);
		}
		else
		{
			_headstartModel.GetComponent<Renderer>().material.SetColor("_Color", new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0));
			_background.color = new Color32(0, 0, 0, 128);
		}
		if (_counter % 2 != 0)
		{
			_helper.SlideinPowerupClicked(_powerupNumber);
		}
		_counter++;
	}

	protected override void Send()
	{
		SingletonMonoBehaviour<InputManager>.Instance.WasButtonClicked = true;
		if (_powerupNumber == -1)
		{
			SDebug.LogError(base.name + " was used without being initialized", this);
		}
		else
		{
			_helper.SlideinPowerupClicked(_powerupNumber);
		}
	}

	public void OnPressButton()
	{
		if (!Game.Instance.IsPaused)
		{
			switch (buttonType)
			{
			case SlideInButtonType.scorebooster:
				SetPowerup1Color(true);
				break;
			case SlideInButtonType.headstart:
				SetPowerup2Color(true);
				break;
			}
		}
	}

	public void OnReleaseButton()
	{
		if (!Game.Instance.IsPaused)
		{
			switch (buttonType)
			{
			case SlideInButtonType.scorebooster:
				SetPowerup1Color(false);
				break;
			case SlideInButtonType.headstart:
				SetPowerup2Color(false);
				break;
			}
		}
	}

	private void SetPowerup2Color(bool animateIn)
	{
		if (animateIn)
		{
			_background.color = GlobalColors.ACTIVE_HEADSTART_BUTTON_BACKGROUND_COLOR;
		}
		else
		{
			StartCoroutine(AnimateSpriteColor(_background, GlobalColors.ACTIVE_HEADSTART_BUTTON_BACKGROUND_COLOR, GlobalColors.INACTIVE_HEADSTART_BUTTON_BACKGROUND_COLOR, 2f));
		}
	}

	private void SetPowerup1Color(bool animateIn)
	{
		if (animateIn)
		{
			_background.color = GlobalColors.ACTIVE_SCOREBOOSTER_BUTTON_BACKGROUND_COLOR;
		}
		else
		{
			StartCoroutine(AnimateSpriteColor(_background, GlobalColors.ACTIVE_SCOREBOOSTER_BUTTON_BACKGROUND_COLOR, GlobalColors.INACTIVE_SCOREBOOSTER_BUTTON_BACKGROUND_COLOR, 2f));
		}
	}

	private IEnumerator AnimateSpriteColor(UISprite spriteToAnimate, Color32 startValue, Color32 endValue, float speedFactor)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.color = Color.Lerp(startValue, endValue, animationLerpFactor);
			yield return null;
		}
	}

	public void ResetPowerupButtonColorToInitial()
	{
		switch (buttonType)
		{
		case SlideInButtonType.scorebooster:
			_background.color = GlobalColors.INACTIVE_SCOREBOOSTER_BUTTON_BACKGROUND_COLOR;
			break;
		case SlideInButtonType.headstart:
			_background.color = GlobalColors.INACTIVE_HEADSTART_BUTTON_BACKGROUND_COLOR;
			break;
		}
	}
}
