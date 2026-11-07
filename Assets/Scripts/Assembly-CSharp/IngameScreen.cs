using System;
using System.Collections;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using Missions;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using TopRun;
using UI;
using UI.Helpers;
using UI.Screens;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using WeeklyHunt;

public class IngameScreen : UIScreen
{
	private enum PowerUpType
	{
		_notset = 0,
		headstart = 1,
		scorebooster = 2
	}

	private enum SpeedometerState
	{
		inactive = 0,
		active = 1,
		hidden = 2
	}

	private const string ONLINESETTINGS_ALLOW_POWERUPS_ON_REVIVE_KEY = "consumables_allow_on_revive";

	private const bool ONLINESETTINGS_ALLOW_POWERUPS_ON_REVIVE_DEFAULT = true;

	[SerializeField]
	private UILabel scoreLabel;

	private Vector3 _originalScoreLabelPosition;

	public UILabel multiplierLabel;

	public UIAnchor multiplierLabelAnchor;

	public UILabel coinLabel;

	public UISprite scoreBG;

	private int _originalWidthForScoreBG;

	[SerializeField]
	private Transform _multiplier;

	private Vector3 _originalMultiplierPosition;

	public UISprite multiplierBG;

	public UISprite coinBG;

	public SlideinPowerupHelper slideinPowerupHelper;

	private bool _isMultiplierLabelUpdated;

	public Color SCOREBOOSTER_ACTIVE_COLOR = new Color(191f / 255f, 233f / 255f, 253f / 255f);

	private Color SCOREBOOSTER_ELECTRICBLUE = new Color(0f, 156f / 255f, 1f);

	private Color SCOREBOOSTER_WHITE = new Color(1f, 1f, 1f);

	public Color MULTIPLIER_LABEL_ORIGINAL_COLOR = new Color(1f, 219f / 255f, 0f);

	[SerializeField]
	private UIWidget[] _widgetFrames;

	[FormerlySerializedAs("itemHuntCounter")]
	[SerializeField]
	private GameObject _weeklyHuntCounter;

	[SerializeField]
	private GameObject pauseButton;

	[SerializeField]
	private GameObject debugPauseButton;

	[SerializeField]
	private UILabel _huntAmountLabel;

	[SerializeField]
	private UISprite _huntLabelBg;

	[SerializeField]
	private Transform _huntIcon;

	[SerializeField]
	private WeeklyHuntIconPositioning _huntBgPositioning;

	[SerializeField]
	private GameObject _turboHeadstartUI;

	[SerializeField]
	private Transform _turboHeadstartIndicator;

	[SerializeField]
	private UISprite[] _turboHeadstartGearBackgrounds = new UISprite[3];

	[SerializeField]
	private UILabel[] _turboHeadstartGearLabels = new UILabel[3];

	[SerializeField]
	private UISprite[] _turboHeadstartGearsGlow = new UISprite[3];

	[SerializeField]
	private UITexture _turboHeadstartBase;

	[SerializeField]
	private UIPanel _turboHeadstartPanel;

	private int _currentHeadstartGear;

	private float _currentHeadstartIndicatorZRotation;

	private readonly int[] _turboHeadstartGearZRotations = new int[4] { 74, 69, 41, 12 };

	[SerializeField]
	private GameObject _scoreBoosterUI;

	[SerializeField]
	private Transform _scoreBoosterIndicator;

	[SerializeField]
	private UISprite[] _scoreBoosterGearBackgrounds = new UISprite[3];

	[SerializeField]
	private UILabel[] _scoreBoosterGearLabels = new UILabel[3];

	[SerializeField]
	private UISprite[] _scoreBoosterGearsGlow = new UISprite[3];

	[SerializeField]
	private UITexture _scoreBoosterBase;

	[SerializeField]
	private UIPanel _scoreBoosterPanel;

	[SerializeField]
	private HoverboardClickingAnimationHelper _hoverboardClickingAnimationHelperPrefab;

	[SerializeField]
	private Transform _hoverboardClickAnimationAnchor;

	private int _currentScoreBoosterGear;

	private float _currentScoreBoosterIndicatorZRotation;

	private readonly int[] _scoreBoosterGearZRotations = new int[4] { -74, -69, -41, -12 };

	private bool _scoreboosterTimePeriodIsActive;

	private bool _jetpackIsActive;

	private const float ANIMATE_TO_NEXT_GEAR_SPEED_FACTOR = 1.2f;

	private const float ANIMATE_SPEEDOMETER_PANEL_ALPHA_SPEED_FACTOR = 3f;

	private const float ANIMATE_IN_SPEEDOMETER_GLOW_SPEED_FACTOR = 100f;

	private const float ANIMATE_OUT_SPEEDOMETER_GLOW_SPEED_FACTOR = 1.5f;

	private const float INACTIVE_SPEEDOMETER_ALPHA = 0.35f;

	private const float ACTIVE_SPEEDOMETER_ALPHA = 1f;

	private const float HIDDEN_SPEEDOMETER_ALPHA = 0f;

	private const int GEAR_GLOW_INITIAL_WIDTH = 40;

	private const int GEAR_GLOW_FINAL_WIDTH = 61;

	private const int GEAR_GLOW_INITIAL_HEIGHT = 46;

	private const int GEAR_GLOW_FINAL_HEIGHT = 70;

	private const int SCORE_ADJUST_GEAR_1_AMOUNT = 1000;

	private const int SCORE_ADJUST_GEAR_2_AMOUNT = 400;

	private const int SCORE_ADJUST_GEAR_3_AMOUNT = 250;

	private const float SCOREBOOSTER_TIMEPERIOD_THIRD_TIER = 3f;

	private SpeedometerState _currentScoreboosterState = SpeedometerState.hidden;

	private SpeedometerState _currentHeadstartState = SpeedometerState.hidden;

	private WeeklyHuntData.WeeklyHuntPeriodData _currentWeekHuntPrize;

	private int _huntCollectedItems;

	private int _huntMaxItems;

	private bool _hasCalculatedHuntLabelOffsets;

	private float _huntLabelFrontOffset;

	private float _huntLabelBackOffset;

	private int score = -1;

	private float lerpFactor;

	private float lerpFactorCoins;

	private bool doLastIteration;

	private int lastKnowDigits;

	private int digits;

	private const int DIGITS_THAT_FIT_WITHOUT_CHANGEING_THE_BOX_SIZE = 6;

	private const int DIST_PR_DIGIT = 11;

	private int _lastKnownMultiDigits;

	private int _multiDigits;

	public static bool allowPowerupsOnRevive
	{
		get
		{
			return OnlineSettings.instance.GetValue("consumables_allow_on_revive", true);
		}
	}

	public int reviveCount { get; private set; }

	public static IngameScreen instance { get; private set; }

	private int extraNumberOfChars
	{
		get
		{
			return Mathf.Clamp(digits - 6, 0, 28);
		}
	}

	private void Awake()
	{
		instance = this;
	}

	protected override void Init()
	{
		base.Init();
		ScoreMultiplierInfo.onScoreMultiplierChanged += SetMultiplierLabel;
		GameStats.Instance.OnCoinsChanged += OnCoinsChanged;
		Game.OnGameStarted += OnGameStarted;
		Game.OnMenuScreenShown += OnMenuScreenShown;
		Game.OnGameRevived += OnGameRevived;
		Game.Instance.OnPauseChange += OnPauseChange;
		multiplierLabel.color = MULTIPLIER_LABEL_ORIGINAL_COLOR;
		multiplierLabel.text = "x" + ScoreMultiplierInfo.rawMultiplier;
		SetMultiplierLabel();
		scoreLabel.text = GameStats.Instance.Score.ToString();
		_originalMultiplierPosition = _multiplier.localPosition;
		_originalScoreLabelPosition = scoreLabel.cachedTransform.localPosition;
		_originalWidthForScoreBG = scoreBG.width;
		_currentScoreBoosterIndicatorZRotation = _scoreBoosterGearZRotations[0];
		_currentHeadstartIndicatorZRotation = _turboHeadstartGearZRotations[0];
		if (SafeArea.IsRestricted)
		{
			UIWidget[] widgetFrames = _widgetFrames;
			foreach (UIWidget uIWidget in widgetFrames)
			{
				uIWidget.topAnchor.absolute = -SafeArea.OFFSET_TOP;
				uIWidget.bottomAnchor.absolute = SafeArea.OFFSET_BOTTOM;
			}
		}
		if (HoverboardClickAnimationManager.HasPossibilityOfShowingClickAnimation())
		{
			HoverboardClickAnimationManager.hoverboardClickingAnimationHelper = UnityEngine.Object.Instantiate(_hoverboardClickingAnimationHelperPrefab, _hoverboardClickAnimationAnchor.transform);
		}
	}

	public void OnDestroy()
	{
		ScoreMultiplierInfo.onScoreMultiplierChanged -= SetMultiplierLabel;
		GameStats.Instance.OnCoinsChanged -= OnCoinsChanged;
		Game.OnGameStarted -= OnGameStarted;
		Game.OnMenuScreenShown -= OnMenuScreenShown;
		Game.OnGameRevived -= OnGameRevived;
		Game.Instance.OnPauseChange -= OnPauseChange;
	}

	private void OnPauseChange(bool pause)
	{
		if (!pause && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
		{
			if (_currentHeadstartState == SpeedometerState.active)
			{
				SpeedometerPauseHandle(_turboHeadstartPanel, PowerUpType.headstart);
			}
			if (_currentScoreboosterState == SpeedometerState.active)
			{
				SpeedometerPauseHandle(_scoreBoosterPanel, PowerUpType.scorebooster);
			}
		}
	}

	protected override void Show()
	{
		base.Show();
		if (Game.Instance == null)
		{
			SDebug.LogError("You must be running the wrong scene");
			return;
		}
		if (Game.Instance.IsPaused && UIScreenController.Instance.IsApplicationFocused)
		{
			CountdownPopupManager.instance.StartCountdown();
		}
		if (!CountdownPopupManager.instance.isCountdownActive)
		{
			MissionsManager.Instance.inRun = true;
			multiplierLabel.color = MULTIPLIER_LABEL_ORIGINAL_COLOR;
			multiplierLabel.text = "x" + ScoreMultiplierInfo.rawMultiplier;
			if (_weeklyHuntCounter != null)
			{
				_weeklyHuntCounter.SetActive(WeeklyHuntManager.IsHuntStillActive() && WeeklyHuntManager.IsAnyPeriodActive());
			}
		}
		SocialManager.instance.pausePhotoDownloading = true;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.pauseRefreshing = true;
		}
		SetPauseButtonVisibility(true);
		debugPauseButton.SetActive(Globals.DEBUG_USE_TIMESCALE_PAUSE_BUTTON);
		SlideinPowerupHelper.OnHeadstartActivated += OnHeadstartActivated;
		HeadstartState state = GameStateMachine.Instance.GetState<HeadstartState>(3);
		state.OnDeactivateTurboHeadstart = (Action)Delegate.Combine(state.OnDeactivateTurboHeadstart, new Action(OnHeadstartDeactivated));
		SlideinPowerupHelper.OnScoreBoostActivated += OnScoreboosterActivated;
		Game.OnGameReviveOffer += OnGameReviveOffer;
		IOSViewController.SetProtectedEdges(IOSUIRectEdge.Bottom);
		if (OnboardingSave.isTutorialCompleted)
		{
			HoverboardClickAnimationManager.CheckIfShouldShowClickAnimation();
		}
	}

	protected override void Hide()
	{
		base.Hide();
		SetMultiplierLabel();
		UIScreenController uIScreenController = UIScreenController.Instance;
		if (uIScreenController.isShowingPopup && uIScreenController.GetCurrentPopupName() == "CountdownPopup")
		{
			CountdownPopupManager.instance.StopCountdown();
		}
		SocialManager.instance.pausePhotoDownloading = false;
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.pauseRefreshing = false;
		}
		WeeklyHuntManager.itemCountUpdated -= UpdateHuntLabel;
		SlideinPowerupHelper.OnHeadstartActivated -= OnHeadstartActivated;
		HeadstartState state = GameStateMachine.Instance.GetState<HeadstartState>(3);
		state.OnDeactivateTurboHeadstart = (Action)Delegate.Remove(state.OnDeactivateTurboHeadstart, new Action(OnHeadstartDeactivated));
		SlideinPowerupHelper.OnScoreBoostActivated -= OnScoreboosterActivated;
		Game.OnGameReviveOffer -= OnGameReviveOffer;
		IOSViewController.SetProtectedEdges(IOSUIRectEdge.None);
	}

	public override bool ShouldShowTopBar()
	{
		return false;
	}

	public override void BackButtonPressed()
	{
		UIScreenController.Instance.PushScreen("PauseUI");
	}

	public void SetPauseButtonVisibility(bool isActive)
	{
		pauseButton.SetActive(isActive);
	}

	public void ToggleDebugPauseTimescale()
	{
		if (0f < SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale)
		{
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = 0f;
		}
		else if (SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale == 0f)
		{
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInGameDefault;
		}
		else
		{
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
		}
	}

	[UsedImplicitly]
	public void PauseClicked()
	{
		UIScreenController.Instance.PushScreen("PauseUI");
	}

	private void SetMultiplierLabel()
	{
		if ((GameStats.Instance.ScoreBoosterActivated || GameStats.Instance.CharacterDayEventActivated || PlusMultiplier.plusMultiplierEnabled) && !DoubleScoreMultiplier.doubleScoreActive)
		{
			int num = int.Parse(multiplierLabel.text.Substring(multiplierLabel.text.IndexOf("x") + 1));
			if (num < ScoreMultiplierInfo.scoreMultiplier)
			{
				int num2 = ((!(lerpFactor > 1f)) ? num : (num + 1));
				_isMultiplierLabelUpdated = false;
				doLastIteration = false;
				multiplierLabel.text = "x" + num2;
			}
			else
			{
				multiplierLabel.text = "x" + ScoreMultiplierInfo.scoreMultiplier;
				if (doLastIteration)
				{
					_isMultiplierLabelUpdated = true;
				}
				doLastIteration = true;
			}
		}
		else
		{
			multiplierLabel.text = "x" + ScoreMultiplierInfo.scoreMultiplier;
		}
		ResizeMultiplierBox();
	}

	private void FadeFromWhiteToDarkBlue()
	{
		multiplierLabel.color = Color.Lerp(SCOREBOOSTER_WHITE, SCOREBOOSTER_ELECTRICBLUE, lerpFactor);
		if (multiplierLabel.text.Contains(ScoreMultiplierInfo.scoreMultiplier.ToString()) && doLastIteration)
		{
			multiplierLabel.color = Color.Lerp(SCOREBOOSTER_WHITE, SCOREBOOSTER_ACTIVE_COLOR, lerpFactor);
		}
	}

	public void ResetMultiplierLabelColour()
	{
		if (GameStats.Instance.ScoreBoosterActivated || GameStats.Instance.CharacterDayEventActivated)
		{
			multiplierLabel.color = SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (multiplierLabel.color != MULTIPLIER_LABEL_ORIGINAL_COLOR)
		{
			multiplierLabel.color = MULTIPLIER_LABEL_ORIGINAL_COLOR;
		}
	}

	private void OnHeadstartActivated(int headstartTier)
	{
		_jetpackIsActive = true;
		_currentHeadstartGear = headstartTier;
		ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.active, PowerUpType.headstart, _turboHeadstartPanel);
		AnimateHeadstartIndicator();
		AnimateGearAndGlow(PowerUpType.headstart, _currentHeadstartGear, _turboHeadstartGearsGlow, _turboHeadstartGearBackgrounds, _turboHeadstartGearLabels);
		switch (_currentHeadstartGear)
		{
		case 1:
			GameStats.Instance.IncreaseAmountNeededToAdjustScoreBy(1000);
			break;
		case 2:
			GameStats.Instance.IncreaseAmountNeededToAdjustScoreBy(400);
			break;
		case 3:
			GameStats.Instance.IncreaseAmountNeededToAdjustScoreBy(250);
			break;
		}
		if (_currentScoreboosterState == SpeedometerState.hidden)
		{
			if (_currentScoreBoosterGear > 0)
			{
				ReenableActiveScoreboosters();
			}
			else
			{
				ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.inactive, PowerUpType.scorebooster, _scoreBoosterPanel);
			}
		}
	}

	private void OnHeadstartDeactivated()
	{
		_jetpackIsActive = false;
		if (!_scoreboosterTimePeriodIsActive)
		{
			ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.hidden, PowerUpType.headstart, _turboHeadstartPanel);
			ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.hidden, PowerUpType.scorebooster, _scoreBoosterPanel);
		}
	}

	private void SetScoreboosterTimePeriodIsActive()
	{
		_scoreboosterTimePeriodIsActive = false;
	}

	private void OnScoreboosterActivated(int scoreboosterTier)
	{
		if (!slideinPowerupHelper.ScoreboosterButtonIsActive() && !slideinPowerupHelper.HeadstartButtonIsActive())
		{
			RestartHidingScoreboosterSpeedometer(3f);
		}
		else
		{
			RestartHidingScoreboosterSpeedometer(slideinPowerupHelper.DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS);
		}
		_currentScoreBoosterGear = scoreboosterTier;
		ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.active, PowerUpType.scorebooster, _scoreBoosterPanel);
		AnimateScoreboosterIndicator();
		AnimateGearAndGlow(PowerUpType.scorebooster, _currentScoreBoosterGear, _scoreBoosterGearsGlow, _scoreBoosterGearBackgrounds, _scoreBoosterGearLabels);
		if (_currentHeadstartState == SpeedometerState.hidden)
		{
			ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.inactive, PowerUpType.headstart, _turboHeadstartPanel);
		}
	}

	private void DeactivateScoreboosterSpeedometer()
	{
		if (base.gameObject.activeSelf && ((_currentHeadstartState == SpeedometerState.active && !_jetpackIsActive) || _currentHeadstartState == SpeedometerState.inactive))
		{
			ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.hidden, PowerUpType.headstart, _turboHeadstartPanel);
			ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.hidden, PowerUpType.scorebooster, _scoreBoosterPanel);
		}
	}

	private void ChangeSpeedometerState(SpeedometerState currentState, SpeedometerState nextState, PowerUpType powerUpType, UIPanel speedometerPanel)
	{
		if (nextState != SpeedometerState.hidden)
		{
			ToggleSpeedoMeterActive(nextState, powerUpType);
		}
		float correspondingAlphaValue = GetCorrespondingAlphaValue(currentState);
		float correspondingAlphaValue2 = GetCorrespondingAlphaValue(nextState);
		CoroutineUtil.StartCoroutine(AnimateSpeedoMeterPanel(powerUpType, nextState, correspondingAlphaValue, correspondingAlphaValue2, speedometerPanel));
	}

	private float GetCorrespondingAlphaValue(SpeedometerState state)
	{
		switch (state)
		{
		case SpeedometerState.hidden:
			return 0f;
		case SpeedometerState.active:
			return 1f;
		case SpeedometerState.inactive:
			return 0.35f;
		default:
			return 0f;
		}
	}

	private IEnumerator AnimateSpeedoMeterPanel(PowerUpType type, SpeedometerState nextState, float startValue, float endValue, UIPanel uiSpeedoMeterPanel)
	{
		yield return CoroutineUtil.StartCoroutine(AnimatePanelAlpha(startValue, endValue, uiSpeedoMeterPanel));
		if (nextState == SpeedometerState.hidden)
		{
			switch (type)
			{
			case PowerUpType.headstart:
				HideTurboHeadstartSpeedoMeter();
				break;
			case PowerUpType.scorebooster:
				HideScoreboosterSpeedoMeter();
				break;
			}
		}
	}

	private void ToggleSpeedoMeterActive(SpeedometerState state, PowerUpType type)
	{
		bool flag = state == SpeedometerState.active;
		switch (type)
		{
		case PowerUpType.headstart:
		{
			_turboHeadstartUI.SetActive(true);
			_turboHeadstartIndicator.gameObject.SetActive(flag);
			_turboHeadstartBase.color = ((!flag) ? GlobalColors.INACTIVE_GEAR_BACKGROUND_COLOR : GlobalColors.ACTIVE_HEADSTART_GEAR_COUNT_COLOR);
			UILabel[] turboHeadstartGearLabels = _turboHeadstartGearLabels;
			foreach (UILabel uILabel2 in turboHeadstartGearLabels)
			{
				uILabel2.enabled = flag;
			}
			_currentHeadstartState = (flag ? SpeedometerState.active : SpeedometerState.inactive);
			break;
		}
		case PowerUpType.scorebooster:
		{
			_scoreBoosterUI.SetActive(true);
			_scoreBoosterIndicator.gameObject.SetActive(flag);
			_scoreBoosterBase.color = ((!flag) ? GlobalColors.INACTIVE_GEAR_BACKGROUND_COLOR : GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_COUNT_COLOR);
			UILabel[] scoreBoosterGearLabels = _scoreBoosterGearLabels;
			foreach (UILabel uILabel in scoreBoosterGearLabels)
			{
				uILabel.enabled = flag;
			}
			_currentScoreboosterState = (flag ? SpeedometerState.active : SpeedometerState.inactive);
			break;
		}
		}
	}

	private void OnGameReviveOffer()
	{
		SetPauseButtonVisibility(false);
		if (_currentScoreboosterState != SpeedometerState.hidden)
		{
			ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.hidden, PowerUpType.scorebooster, _scoreBoosterPanel);
		}
		if (_currentHeadstartState != SpeedometerState.hidden)
		{
			ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.hidden, PowerUpType.headstart, _turboHeadstartPanel);
		}
	}

	private void SpeedometerPauseHandle(UIPanel uiPanel, PowerUpType powerUpType)
	{
		if (uiPanel != null && uiPanel.alpha > 0f && uiPanel.gameObject.activeSelf)
		{
			switch (powerUpType)
			{
			case PowerUpType.headstart:
				AnimateHeadstartIndicator();
				break;
			case PowerUpType.scorebooster:
				AnimateScoreboosterIndicator();
				break;
			}
		}
	}

	private IEnumerator AnimatePanelAlpha(float startValue, float endValue, UIPanel uiSpeedoMeterPanel)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * 3f;
			float panelAlpha = Mathf.Lerp(startValue, endValue, animationLerpFactor);
			uiSpeedoMeterPanel.alpha = panelAlpha;
			yield return null;
		}
	}

	private IEnumerator AnimateSpriteColor(UISprite spriteToAnimate, Color32 startValue, Color32 endValue, float speedFactor)
	{
		yield return new WaitForSeconds(0.2f);
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.color = Color.Lerp(startValue, endValue, animationLerpFactor);
			yield return null;
		}
	}

	private IEnumerator AnimateSpriteSize(UISprite spriteToAnimate, int startHeightValue, int endHeightValue, int startWidthValue, int endWidthValue, float speedFactor)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			spriteToAnimate.width = Mathf.CeilToInt(Mathf.Lerp(startWidthValue, endWidthValue, animationLerpFactor));
			spriteToAnimate.height = Mathf.CeilToInt(Mathf.Lerp(startHeightValue, endHeightValue, animationLerpFactor));
			yield return null;
		}
	}

	private void AnimateGearAndGlow(PowerUpType type, int currentGear, UISprite[] gearGlows, UISprite[] gearBackgrounds, UILabel[] gearLabels)
	{
		gearGlows[currentGear - 1].color = GlobalColors.FINAL_GEAR_GLOW_COLOR;
		ActivateGear(type, gearBackgrounds[currentGear - 1], gearLabels[currentGear - 1]);
		CoroutineUtil.StartCoroutine(AnimateSpriteColor(gearGlows[currentGear - 1], GlobalColors.FINAL_GEAR_GLOW_COLOR, GlobalColors.INITIAL_GEAR_GLOW_COLOR, 1.5f));
		CoroutineUtil.StartCoroutine(AnimateSpriteSize(gearGlows[currentGear - 1], 46, 70, 40, 61, 1.5f));
	}

	private void SetActiveGears(PowerUpType type, int currentGear, UISprite[] gearGlows, UISprite[] gearBackgrounds, UILabel[] gearLabels)
	{
		for (int i = 0; i < currentGear; i++)
		{
			ActivateGearNoAnimation(type, gearBackgrounds[i], gearLabels[i]);
		}
	}

	private void HideTurboHeadstartSpeedoMeter()
	{
		_turboHeadstartUI.SetActive(false);
		_turboHeadstartPanel.alpha = 0f;
		_currentHeadstartGear = 0;
		_currentHeadstartIndicatorZRotation = _turboHeadstartGearZRotations[0];
		_turboHeadstartIndicator.localRotation = Quaternion.Euler(new Vector3(0f, 0f, _turboHeadstartGearZRotations[0]));
		DeactivateGears(_turboHeadstartGearBackgrounds, _turboHeadstartGearLabels);
		ResetGlow(_turboHeadstartGearsGlow);
		_currentHeadstartState = SpeedometerState.hidden;
		_jetpackIsActive = false;
	}

	private void HideScoreboosterSpeedoMeter()
	{
		_scoreBoosterUI.SetActive(false);
		_scoreBoosterPanel.alpha = 0f;
		_currentScoreBoosterIndicatorZRotation = _scoreBoosterGearZRotations[0];
		_scoreBoosterIndicator.localRotation = Quaternion.Euler(new Vector3(0f, 0f, _scoreBoosterGearZRotations[0]));
		DeactivateGears(_scoreBoosterGearBackgrounds, _scoreBoosterGearLabels);
		ResetGlow(_scoreBoosterGearsGlow);
		_currentScoreboosterState = SpeedometerState.hidden;
	}

	private void ResetGlow(UISprite[] gearGlows)
	{
		foreach (UISprite uISprite in gearGlows)
		{
			uISprite.height = 46;
			uISprite.width = 40;
			uISprite.color = GlobalColors.INITIAL_GEAR_GLOW_COLOR;
		}
	}

	private void ActivateGear(PowerUpType type, UISprite background, UILabel count)
	{
		background.color = Color.white;
		switch (type)
		{
		case PowerUpType.headstart:
			CoroutineUtil.StartCoroutine(AnimateSpriteColor(background, Color.white, GlobalColors.ACTIVE_HEADSTART_GEAR_BACKGROUND_COLOR, 100f));
			count.color = GlobalColors.ACTIVE_HEADSTART_GEAR_COUNT_COLOR;
			break;
		case PowerUpType.scorebooster:
			CoroutineUtil.StartCoroutine(AnimateSpriteColor(background, Color.white, GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_BACKGROUND_COLOR, 100f));
			count.color = GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_COUNT_COLOR;
			break;
		}
		count.effectStyle = UILabel.Effect.None;
	}

	private void ActivateGearNoAnimation(PowerUpType type, UISprite background, UILabel count)
	{
		switch (type)
		{
		case PowerUpType.headstart:
			background.color = GlobalColors.ACTIVE_HEADSTART_GEAR_BACKGROUND_COLOR;
			count.color = GlobalColors.ACTIVE_HEADSTART_GEAR_COUNT_COLOR;
			break;
		case PowerUpType.scorebooster:
			background.color = GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_BACKGROUND_COLOR;
			count.color = GlobalColors.ACTIVE_SCOREBOOSTER_GEAR_COUNT_COLOR;
			break;
		}
		count.effectStyle = UILabel.Effect.None;
	}

	private void DeactivateGears(UISprite[] gearBackgrounds, UILabel[] gearLabels)
	{
		foreach (UISprite uISprite in gearBackgrounds)
		{
			uISprite.color = GlobalColors.INACTIVE_GEAR_BACKGROUND_COLOR;
		}
		foreach (UILabel uILabel in gearLabels)
		{
			uILabel.color = GlobalColors.INACTIVE_GEAR_COUNT_COLOR;
			uILabel.effectStyle = UILabel.Effect.Shadow;
		}
	}

	private void AnimateHeadstartIndicator()
	{
		StopCoroutine("AnimateHeadstartIndicatorGear");
		StartCoroutine("AnimateHeadstartIndicatorGear");
	}

	private IEnumerator AnimateHeadstartIndicatorGear()
	{
		float startIndicatorRotation = _currentHeadstartIndicatorZRotation;
		float endIndicatorRotation = _turboHeadstartGearZRotations[_currentHeadstartGear];
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * 1.2f;
			_currentHeadstartIndicatorZRotation = Mathf.Lerp(startIndicatorRotation, endIndicatorRotation, Mathf.Sin(animationLerpFactor * (float)Math.PI * 0.5f));
			Vector3 rotation = new Vector3(0f, 0f, _currentHeadstartIndicatorZRotation);
			_turboHeadstartIndicator.localRotation = Quaternion.Euler(rotation);
			yield return null;
		}
	}

	private void SetScoreboosterIndicator(int gear)
	{
		float currentScoreBoosterIndicatorZRotation = _scoreBoosterGearZRotations[gear];
		_currentScoreBoosterIndicatorZRotation = currentScoreBoosterIndicatorZRotation;
		Vector3 euler = new Vector3(0f, 0f, _currentScoreBoosterIndicatorZRotation);
		_scoreBoosterIndicator.localRotation = Quaternion.Euler(euler);
	}

	private void AnimateScoreboosterIndicator()
	{
		StopCoroutine("AnimateScoreboosterIndicatorGear");
		StartCoroutine("AnimateScoreboosterIndicatorGear");
	}

	private IEnumerator AnimateScoreboosterIndicatorGear()
	{
		float startIndicatorRotation = _currentScoreBoosterIndicatorZRotation;
		float endIndicatorRotation = _scoreBoosterGearZRotations[_currentScoreBoosterGear];
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * 1.2f;
			_currentScoreBoosterIndicatorZRotation = Mathf.Lerp(startIndicatorRotation, endIndicatorRotation, Mathf.Sin(animationLerpFactor * (float)Math.PI * 0.5f));
			Vector3 rotation = new Vector3(0f, 0f, _currentScoreBoosterIndicatorZRotation);
			_scoreBoosterIndicator.localRotation = Quaternion.Euler(rotation);
			yield return null;
		}
	}

	private void Update()
	{
		UpdateMultiplierLabel();
		GameStats.Instance.CalculateScore();
		if (score != GameStats.Instance.Score)
		{
			SetScoreLabel();
		}
		if (Game.Instance.isReadyForSlideinPowerups && !Game.Instance.IsRunningOnTutorialTrack)
		{
			Game.Instance.isReadyForSlideinPowerups = false;
			slideinPowerupHelper.ShowPowerups();
		}
		AnimateCoinsLabelIfDoubleCoins();
	}

	private void AnimateCoinsLabelIfDoubleCoins()
	{
		if (DoubleCoins.HasDoubleCoinsModifier)
		{
			lerpFactorCoins += Time.deltaTime * 2f;
			if (lerpFactorCoins > 1f)
			{
				if (DoubleCoins.numberOfAnimationBlinks < 2)
				{
					DoubleCoins.numberOfAnimationBlinks++;
					lerpFactorCoins = 0f;
				}
				else
				{
					coinLabel.color = GlobalColors.COINS_LIGHT_PURPLE;
				}
			}
			else
			{
				coinLabel.color = Color.Lerp(GlobalColors.COINS_LIGHT_PURPLE, GlobalColors.COINS_DARK_PURPLE, lerpFactorCoins);
			}
		}
		else
		{
			coinLabel.color = GlobalColors.COINS_ORIGINAL_COLOR;
			lerpFactorCoins = 0f;
		}
	}

	private void UpdateMultiplierLabel()
	{
		if ((GameStats.Instance.ScoreBoosterActivated || GameStats.Instance.CharacterDayEventActivated || PlusMultiplier.plusMultiplierEnabled) && !_isMultiplierLabelUpdated && !DoubleScoreMultiplier.doubleScoreActive)
		{
			if (!PlusMultiplier.plusMultiplierEnabled)
			{
				lerpFactor += Time.deltaTime * 2f;
			}
			else
			{
				lerpFactor += Time.deltaTime * 4f;
			}
			if (lerpFactor > 1f)
			{
				SetMultiplierLabel();
				lerpFactor = 0f;
			}
			else if (!PlusMultiplier.plusMultiplierEnabled)
			{
				FadeFromWhiteToDarkBlue();
			}
			else if (!doLastIteration)
			{
				multiplierLabel.color = Color.Lerp(GlobalColors.MULTIPLIER_DARK_PURPLE, GlobalColors.MULTIPLIER_LIGHT_PURPLE, lerpFactor);
			}
		}
	}

	private void OnCoinsChanged()
	{
		coinLabel.text = GameStats.Instance.CoinsCollected.ToString();
		ResizeCoinBox();
	}

	private void OnGameStarted()
	{
		lastKnowDigits = 0;
		digits = 0;
		if (!Game.Instance.isReadyForSlideinPowerups)
		{
			slideinPowerupHelper.HidePowerup1(true);
			slideinPowerupHelper.HidePowerup2(true);
		}
		HideTurboHeadstartSpeedoMeter();
		HideScoreboosterSpeedoMeter();
	}

	private void SetScoreLabel()
	{
		score = GameStats.Instance.Score;
		digits = Utility.NumberOfDigits(score);
		string text;
		switch (digits)
		{
		case 1:
			text = "00000";
			break;
		case 2:
			text = "0000";
			break;
		case 3:
			text = "000";
			break;
		case 4:
			text = "00";
			break;
		case 5:
			text = "0";
			break;
		default:
			text = string.Empty;
			break;
		}
		scoreLabel.text = text + score;
		if (lastKnowDigits != digits)
		{
			ResizeScoreBox();
			lastKnowDigits = digits;
		}
	}

	private void ResizeScoreBox()
	{
		_multiplier.localPosition = new Vector3(_originalMultiplierPosition.x - (float)(11 * extraNumberOfChars), _originalMultiplierPosition.y, 0f);
		scoreLabel.cachedTransform.localPosition = new Vector3(_originalScoreLabelPosition.x - (float)(11 * extraNumberOfChars), _originalScoreLabelPosition.y, 0f);
		scoreBG.width = _originalWidthForScoreBG + 11 * extraNumberOfChars;
	}

	private void ResizeCoinBox()
	{
		int length = coinLabel.text.Length;
		float num = 64f;
		if (length > 1)
		{
			num += (float)(12 * (length - 1));
		}
		if ((float)coinBG.width != num)
		{
			coinBG.width = Mathf.RoundToInt(num);
		}
	}

	private void ResizeMultiplierBox()
	{
		_multiDigits = multiplierLabel.text.Length;
		if (_lastKnownMultiDigits != _multiDigits)
		{
			float f = 34f;
			if (_multiDigits == 3)
			{
				f = 44f;
			}
			else if (_multiDigits >= 4)
			{
				f = 50f;
			}
			multiplierBG.width = Mathf.RoundToInt(f);
			multiplierLabelAnchor.enabled = true;
			_lastKnownMultiDigits = _multiDigits;
		}
	}

	private void SynchronizeHuntData()
	{
		_currentWeekHuntPrize = WeeklyHuntManager.GetPrizeDataForPeriod(WeeklyHuntData.GetActivePeriod());
		_huntMaxItems = _currentWeekHuntPrize.itemsToCollect;
		_huntCollectedItems = _currentWeekHuntPrize.itemsCollected;
		if (_huntCollectedItems < 0)
		{
			_huntCollectedItems = 0;
		}
		else if (_huntCollectedItems > _huntMaxItems)
		{
			_huntCollectedItems = _huntMaxItems;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			UpdateHuntLabel();
		}
	}

	public void UpdateHuntLabel()
	{
		SynchronizeHuntData();
		if (_huntAmountLabel != null)
		{
			_huntAmountLabel.enabled = WeeklyHuntManager.IsHuntStillActive();
			_huntAmountLabel.text = _huntCollectedItems + "/" + _huntMaxItems;
		}
		if (!_hasCalculatedHuntLabelOffsets)
		{
			_huntBgPositioning.ForceUpdate();
			_huntLabelFrontOffset = _huntLabelBg.transform.localPosition.x - _huntAmountLabel.transform.localPosition.x;
			_huntLabelBackOffset = _huntIcon.localPosition.x - _huntAmountLabel.transform.localPosition.x;
			_hasCalculatedHuntLabelOffsets = true;
		}
		_huntLabelBg.width = Mathf.RoundToInt((float)_huntAmountLabel.width + Mathf.Abs(_huntLabelFrontOffset) + Mathf.Abs(_huntLabelBackOffset) + WeeklyHuntIconPositioning.HUNT_BG_SCALE_OFFSET);
		if (_weeklyHuntCounter.activeInHierarchy)
		{
			_weeklyHuntCounter.transform.localPosition = WeeklyHuntIconPositioning.INGAME_COUNTER_GAMEOBJECT_POSITION;
		}
	}

	private void OnEnable()
	{
		WeeklyHuntManager.itemCountUpdated += UpdateHuntLabel;
		UpdateHuntLabel();
	}

	private void OnMenuScreenShown()
	{
		ResetScoreboosterGear();
		reviveCount = 0;
	}

	private void ResetScoreboosterGear()
	{
		_currentScoreBoosterGear = 0;
	}

	private void OnGameRevived()
	{
		ReenableActiveScoreboosters();
		reviveCount++;
	}

	private void ReenableActiveScoreboosters()
	{
		if (_currentScoreBoosterGear != 0)
		{
			ChangeSpeedometerState(_currentScoreboosterState, SpeedometerState.active, PowerUpType.scorebooster, _scoreBoosterPanel);
			SetActiveGears(PowerUpType.scorebooster, _currentScoreBoosterGear, _scoreBoosterGearsGlow, _scoreBoosterGearBackgrounds, _scoreBoosterGearLabels);
			SetScoreboosterIndicator(_currentScoreBoosterGear);
			if (_currentHeadstartState == SpeedometerState.hidden)
			{
				ChangeSpeedometerState(_currentHeadstartState, SpeedometerState.inactive, PowerUpType.headstart, _turboHeadstartPanel);
			}
			RestartHidingScoreboosterSpeedometer(slideinPowerupHelper.DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS);
		}
	}

	public void RestartHidingScoreboosterSpeedometer(float hideTime)
	{
		CancelInvoke("SetScoreboosterTimePeriodIsActive");
		CancelInvoke("DeactivateScoreboosterSpeedometer");
		_scoreboosterTimePeriodIsActive = true;
		StartHidingScoreboosterSpeedometer(hideTime);
	}

	private void StartHidingScoreboosterSpeedometer(float time)
	{
		Invoke("SetScoreboosterTimePeriodIsActive", time);
		Invoke("DeactivateScoreboosterSpeedometer", time);
	}
}
