using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Subway;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using UnityEngine;

public class SlideinPowerupHelper : MonoBehaviour
{
	private enum SlideinPowerup
	{
		_notset = 0,
		powerup1 = 1,
		powerup2 = 2
	}

	[SerializeField]
	private SlideinPowerupButton powerup1Button;

	[SerializeField]
	private SlideinPowerupButton powerup2Button;

	[SerializeField]
	private Collider powerup1ButtonCollider;

	[SerializeField]
	private Collider powerup2ButtonCollider;

	[SerializeField]
	private UILabel powerup1AmountLabel;

	[SerializeField]
	private UILabel powerup2AmountLabel;

	[SerializeField]
	private UISprite _powerup1ButtonGlow;

	[SerializeField]
	private UISprite _powerup2ButtonGlow;

	private SpringPosition powerup1Spring;

	private SpringPosition powerup2Spring;

	private Vector3 powerup1PositionOff = new Vector3(-100f, 160f, 0f);

	private Vector3 powerup2PositionOff = new Vector3(-100f, 60f, 0f);

	private Vector3 powerup1PositionOn = new Vector3(50f, 160f, 0f);

	private Vector3 powerup2PositionOn = new Vector3(50f, 60f, 0f);

	private static SlideinPowerupHelper instance;

	private bool _hasInited;

	private bool _initialButtonGlowHasStarted;

	private int _currentHeadstartGear;

	private int _currentScoreboosterGear;

	private const float _DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS = 5f;

	private const float ANIMATE_GLOW_SPEED = 5f;

	private const float SHORT_TIME_BETWEEN_ANIMATION = 0.1f;

	private const float LONG_TIME_BETWEEN_ANIMATION = 0.5f;

	private const float PAUSE_BETWEEN_ANIMATIONs = 1.4f;

	private const float DELAY_ANIMATION_AMOUNT_ON_PRESS_POWERUP_BUTTON = 1.5f;

	private const int MAX_AMOUNT_OF_SCOREBOOSTER_GEARS = 3;

	private const int MAX_AMOUNT_OF_HEADSTART_GEARS = 3;

	private const bool HIDE_ON_ZERO_PURCHASE = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<int> OnHeadstartActivated__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<int> OnScoreBoostActivated__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnButtonsShow__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnButtonsHide__BackingField;

	public float DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS
	{
		get
		{
			return 5f;
		}
	}

	public static SlideinPowerupHelper Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(SlideinPowerupHelper)) as SlideinPowerupHelper);
		}
	}

	public static event Action<int> OnHeadstartActivated
	{
		add
		{
			Action<int> action = OnHeadstartActivated__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnHeadstartActivated__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = OnHeadstartActivated__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnHeadstartActivated__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<int> OnScoreBoostActivated
	{
		add
		{
			Action<int> action = OnScoreBoostActivated__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnScoreBoostActivated__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = OnScoreBoostActivated__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnScoreBoostActivated__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnButtonsShow
	{
		add
		{
			Action action = OnButtonsShow__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnButtonsShow__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnButtonsShow__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnButtonsShow__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnButtonsHide
	{
		add
		{
			Action action = OnButtonsHide__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnButtonsHide__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnButtonsHide__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnButtonsHide__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Start()
	{
		if (!_hasInited)
		{
			InitHelper();
		}
		Game.OnGameRevived += OnGameRevived;
		Game.OnMenuScreenShown += OnMenuScreenShown;
		SellConsumable.completed += OnSellConsumableCompleted;
	}

	private void OnEnable()
	{
		GjInstance = this;
		_initialButtonGlowHasStarted = false;
		UIScreenController.OnChangedScreen += ScreenDidChange;
		Game.OnGameReviveOffer += OnGameReviveOffer;
		JetpackState state = GameStateMachine.Instance.GetState<JetpackState>(4);
		state.OnStart = (Action)Delegate.Combine(state.OnStart, new Action(OnJetpackStart));
		PogostickState state2 = GameStateMachine.Instance.GetState<PogostickState>(5);
		state2.OnStart = (Action)Delegate.Combine(state2.OnStart, new Action(OnPogostickStart));
	}

	private void OnDisable()
	{
		_gjBoosting = false;
		UIScreenController.OnChangedScreen -= ScreenDidChange;
		Game.OnGameReviveOffer -= OnGameReviveOffer;
		ResetHeadstartButtonGlowColor();
		ResetScoreboosterButtonGlowColor();
		JetpackState state = GameStateMachine.Instance.GetState<JetpackState>(4);
		state.OnStart = (Action)Delegate.Remove(state.OnStart, new Action(OnJetpackStart));
		PogostickState state2 = GameStateMachine.Instance.GetState<PogostickState>(5);
		state2.OnStart = (Action)Delegate.Remove(state2.OnStart, new Action(OnPogostickStart));
	}

	private void ScreenDidChange(string screenOrPopupName)
	{
		if (screenOrPopupName.Equals("SaveMePopup"))
		{
			HidePowerups();
		}
	}

	private void InitHelper()
	{
		powerup1Button.InitSlideinButton(this, 1);
		powerup2Button.InitSlideinButton(this, 2);
		powerup1ButtonCollider.enabled = false;
		powerup2ButtonCollider.enabled = false;
		powerup1Button.transform.localPosition = powerup1PositionOff;
		powerup2Button.transform.localPosition = powerup2PositionOff;
		UpdateAmountLabel(powerup1AmountLabel, InventorySave.GetAmount(UpgradeType.scorebooster));
		UpdateAmountLabel(powerup2AmountLabel, InventorySave.GetAmount(UpgradeType.headstart2000));
		InventorySave.powerupsChanged += OnUpgradeUsed;
		Game.Instance.OnPauseChange += OnPauseChange;
		_hasInited = true;
	}

	private void OnPauseChange(bool pause)
	{
		if (!pause)
		{
			ResumeButtonsGlowColor();
		}
	}

	private void OnGameReviveOffer()
	{
		HidePowerups();
	}

	private void OnGameRevived()
	{
		ResetHeadstartGear();
		GameStateMachine.Instance.GetState<HeadstartState>(3).ResetTurboHeadstart();
	}

	private void OnMenuScreenShown()
	{
		HidePowerups();
		ResetScoreboosterGear();
		ResetHeadstartGear();
	}

	private void OnDestroy()
	{
		InventorySave.powerupsChanged -= OnUpgradeUsed;
		Game.Instance.OnPauseChange -= OnPauseChange;
		Game.OnGameRevived -= OnGameRevived;
		Game.OnMenuScreenShown -= OnMenuScreenShown;
		SellConsumable.completed -= OnSellConsumableCompleted;
	}

	private void OnUpgradeUsed(UpgradeType upgradeType, int newAmount)
	{
		switch (upgradeType)
		{
		case UpgradeType.scorebooster:
			UpdateAmountLabel(powerup1AmountLabel, newAmount);
			if (newAmount == 0 && !SellConsumable.allowSellConsumablesDuringRun)
			{
				HidePowerup1();
			}
			break;
		case UpgradeType.headstart2000:
			UpdateAmountLabel(powerup2AmountLabel, newAmount);
			if (newAmount == 0 && !SellConsumable.allowSellConsumablesDuringRun)
			{
				HidePowerup2();
			}
			break;
		}
	}

	public void ShowPowerups()
	{
		if (!_hasInited)
		{
			InitHelper();
		}
		ResetHeadstartButtonGlowColor();
		ResetScoreboosterButtonGlowColor();
		_initialButtonGlowHasStarted = false;
		if (_currentScoreboosterGear < 3)
		{
			ShowPowerup1();
		}
		if (_currentHeadstartGear < 3)
		{
			ShowPowerup2();
		}
		CancelInvoke("HideScorebooster");
		CancelInvoke("HideHeadstart");
		HeadstartState state = GameStateMachine.Instance.GetState<HeadstartState>(3);
		state.OnHidTurboHeadstartButtons = (Action)Delegate.Combine(state.OnHidTurboHeadstartButtons, new Action(HideHeadstart));
		Invoke("HideScorebooster", 5f);
		Invoke("HideHeadstart", 5f);
		// só no começo da corrida (depois de reviver com chave o botão aparece de novo, mas aí não liga sozinho)
		if (TrainingOptions.AutoBoost && SaveMeManager.reviveCountInCurrentRun == 0)
		{
			GjAutoBoost();
		}
	}

	// Vantagem automática: liga a vantagem (headstart) no nível máximo (3 marchas) assim que a corrida começa.
	// Também dá para ligar na hora com a tecla B enquanto o botão de vantagem está na tela.
	// Usa as vantagens do inventário quando tem; se acabarem, liga mesmo assim.
	public static SlideinPowerupHelper GjInstance;

	private bool _gjBoosting;

	private void GjAutoBoost()
	{
		if (!_gjBoosting && _currentHeadstartGear < 3)
		{
			StartCoroutine(GjBoostRoutine(0.75f));
		}
	}

	public void GjBoostHotkey()
	{
		if (powerup2ButtonCollider.enabled && !_gjBoosting && _currentHeadstartGear < 3)
		{
			StartCoroutine(GjBoostRoutine(0f));
		}
	}

	// delay: a vantagem automática espera 1,5 s depois da largada; a tecla B liga na hora
	private IEnumerator GjBoostRoutine(float delay)
	{
		_gjBoosting = true;
		if (delay > 0f)
		{
			yield return new WaitForSecondsRealtime(delay);
		}
		while (_currentHeadstartGear < 3 && !Game.Instance.isDead)
		{
			if (!Game.Instance.IsPaused)
			{
				if (InventorySave.GetAmount(UpgradeType.headstart2000) > 0 && powerup2ButtonCollider.enabled)
				{
					SlideinPowerupClicked(2);
				}
				else
				{
					_currentHeadstartGear++;
					Action<int> onHeadstartActivated = OnHeadstartActivated__BackingField;
					if (onHeadstartActivated != null)
					{
						onHeadstartActivated(_currentHeadstartGear);
					}
					Game.Instance.Megaheadstart();
				}
			}
			// espera o jogo entrar na vantagem antes da próxima marcha
			yield return new WaitForSecondsRealtime(0.06f);
		}
		CancelInvoke("HideHeadstart");
		HideHeadstart();
		_gjBoosting = false;
	}

	private void HideScorebooster()
	{
		HidePowerup1();
	}

	private void HideHeadstart()
	{
		HeadstartState state = GameStateMachine.Instance.GetState<HeadstartState>(3);
		state.OnHidTurboHeadstartButtons = (Action)Delegate.Remove(state.OnHidTurboHeadstartButtons, new Action(HideHeadstart));
		HidePowerup2();
	}

	private void ResetHeadstartButtonGlowColor()
	{
		StopCoroutine("AnimateButtonBackground");
		_powerup2ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
	}

	private void ResetScoreboosterButtonGlowColor()
	{
		StopCoroutine("AnimateButtonBackground");
		_powerup1ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
	}

	private void ResetButtonGlowInvokes()
	{
		StopCoroutine("DelayResumeButtonsGlow");
		if (_currentScoreboosterGear < 3 || _currentHeadstartGear < 3)
		{
			StartCoroutine("DelayResumeButtonsGlow");
		}
	}

	private IEnumerator DelayResumeButtonsGlow()
	{
		yield return new WaitForSeconds(1.5f);
		ResumeButtonsGlowColor();
	}

	private void ResumeButtonsGlowColor()
	{
		if ((powerup2ButtonCollider.enabled || powerup1ButtonCollider.enabled) && base.gameObject.activeInHierarchy && base.gameObject.activeSelf)
		{
			_powerup2ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
			_powerup1ButtonGlow.color = GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR;
			StartCoroutine("AnimateButtonBackground");
		}
	}

	public void SlideinPowerupClicked(int powerupAsInt)
	{
		if (Game.Instance.IsPaused)
		{
			return;
		}
		bool success = false;
		if (powerupAsInt == 1 && powerup1ButtonCollider.enabled)
		{
			CancelInvoke("HideScorebooster");
			Invoke("HideScorebooster", 5f);
			ResetScoreboosterButtonGlowColor();
			ResetButtonGlowInvokes();
			int amount = InventorySave.GetAmount(UpgradeType.scorebooster);
			if (amount > 0)
			{
				_currentScoreboosterGear++;
				if (_currentScoreboosterGear <= 3)
				{
					Action<int> onScoreBoostActivated__BackingField = OnScoreBoostActivated__BackingField;
					if (onScoreBoostActivated__BackingField != null)
					{
						onScoreBoostActivated__BackingField(_currentScoreboosterGear);
					}
					GameStats.Instance.ScoreBoosterActivated = true;
					GameStats.Instance.ScoreBoosterTierIncrement();
					InventorySave.Use(UpgradeType.scorebooster);
					MissionsManager.Instance.PlayerDidThis(MissionTarget.ScoreBooster);
					success = true;
				}
				if (_currentScoreboosterGear >= 3)
				{
					HidePowerup1();
				}
			}
			else if (SellConsumable.allowSellConsumablesDuringRun)
			{
				SellConsumable.Sell(SellableConsumable.scorebooster);
			}
			else
			{
				HidePowerup1();
			}
		}
		else if (powerupAsInt == 2 && powerup2ButtonCollider.enabled)
		{
			CancelInvoke("HideHeadstart");
			Invoke("HideHeadstart", 5f);
			ResetHeadstartButtonGlowColor();
			ResetButtonGlowInvokes();
			int amount2 = InventorySave.GetAmount(UpgradeType.headstart2000);
			if (amount2 > 0)
			{
				_currentHeadstartGear++;
				if (_currentHeadstartGear <= 3)
				{
					Action<int> onHeadstartActivated__BackingField = OnHeadstartActivated__BackingField;
					if (onHeadstartActivated__BackingField != null)
					{
						onHeadstartActivated__BackingField(_currentHeadstartGear);
					}
					Game.Instance.Megaheadstart();
					InventorySave.Use(UpgradeType.headstart2000);
					MissionsManager.Instance.PlayerDidThis(MissionTarget.Headstart);
					success = true;
				}
				if (_currentHeadstartGear >= 3)
				{
					HidePowerup2();
				}
			}
			else if (SellConsumable.allowSellConsumablesDuringRun)
			{
				SellConsumable.Sell(SellableConsumable.headstart);
			}
			else
			{
				HidePowerup2();
			}
		}
		LogPowerupActivated(powerupAsInt, success);
	}

	public void ShowPowerup1()
	{
		powerup1Button.ResetPowerupButtonColorToInitial();
		if (InventorySave.GetAmount(UpgradeType.scorebooster) > 0 || (SellConsumable.allowSellConsumablesDuringRun && OnboardingSave.allowSellScoreboosterDuringRun))
		{
			powerup1Button.transform.localPosition = powerup1PositionOff;
			powerup1Spring = SpringPosition.Begin(powerup1Button.gameObject, powerup1PositionOn, 10f);
			powerup1Spring.ignoreTimeScale = true;
			SpringPosition springPosition = powerup1Spring;
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerupButtonAnimatedIn));
			powerup1ButtonCollider.enabled = true;
			OnboardingSave.allowSellScoreboosterDuringRun = true;
			OnButtonsShow__BackingField.SafeInvoke();
		}
	}

	public void ShowPowerup2()
	{
		powerup2Button.ResetPowerupButtonColorToInitial();
		if (InventorySave.GetAmount(UpgradeType.headstart2000) > 0 || (SellConsumable.allowSellConsumablesDuringRun && OnboardingSave.allowSellHeadstartDuringRun))
		{
			powerup2Button.transform.localPosition = powerup2PositionOff;
			powerup2Spring = SpringPosition.Begin(powerup2Button.gameObject, powerup2PositionOn, 10f);
			powerup2Spring.ignoreTimeScale = true;
			SpringPosition springPosition = powerup2Spring;
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerupButtonAnimatedIn));
			powerup2ButtonCollider.enabled = true;
			OnboardingSave.allowSellHeadstartDuringRun = true;
			OnButtonsShow__BackingField.SafeInvoke();
		}
	}

	public void HidePowerup1(bool instant = false)
	{
		powerup1ButtonCollider.enabled = false;
		if (!_hasInited)
		{
			InitHelper();
		}
		if (instant)
		{
			SpringPosition component = powerup1Button.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			powerup1Button.transform.position = powerup1PositionOff;
			ResetScoreboosterButtonGlowColor();
		}
		else
		{
			SpringPosition springPosition = SpringPosition.Begin(powerup1Button.gameObject, powerup1PositionOff, 10f);
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerup1ButtonAnimatedOut));
		}
		FireHideEventIfNecessary();
	}

	public void HidePowerup2(bool instant = false)
	{
		powerup2ButtonCollider.enabled = false;
		if (!_hasInited)
		{
			InitHelper();
		}
		if (instant)
		{
			SpringPosition component = powerup2Button.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			powerup2Button.transform.position = powerup2PositionOff;
			ResetHeadstartButtonGlowColor();
		}
		else
		{
			SpringPosition springPosition = SpringPosition.Begin(powerup2Button.gameObject, powerup2PositionOff, 10f);
			springPosition.onFinished = (SpringPosition.OnFinished)Delegate.Combine(springPosition.onFinished, new SpringPosition.OnFinished(OnPowerup2ButtonAnimatedOut));
		}
		FireHideEventIfNecessary();
	}

	private void FireHideEventIfNecessary()
	{
		if (!powerup2ButtonCollider.enabled && !powerup1ButtonCollider.enabled)
		{
			OnButtonsHide__BackingField.SafeInvoke();
		}
	}

	private void OnPowerup1ButtonAnimatedOut()
	{
		ResetScoreboosterButtonGlowColor();
	}

	private void OnPowerup2ButtonAnimatedOut()
	{
		ResetHeadstartButtonGlowColor();
	}

	private void OnPowerupButtonAnimatedIn()
	{
		if (_currentHeadstartGear != 0 || _currentScoreboosterGear != 0)
		{
			StopCoroutine("AnimateButtonBackground");
		}
		if (!_initialButtonGlowHasStarted)
		{
			StartCoroutine("AnimateButtonBackground");
			_initialButtonGlowHasStarted = true;
		}
	}

	private IEnumerator AnimateButtonBackground()
	{
		while (true)
		{
			if (ScoreboosterButtonIsActive())
			{
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup1ButtonGlow));
				yield return new WaitForSeconds(0.1f);
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup1ButtonGlow));
			}
			if (ScoreboosterButtonIsActive() && HeadstartButtonIsActive())
			{
				yield return new WaitForSeconds(0.5f);
			}
			if (HeadstartButtonIsActive())
			{
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup2ButtonGlow));
				yield return new WaitForSeconds(0.1f);
				yield return StartCoroutine(AnimateButtonBackgrounGlow(_powerup2ButtonGlow));
			}
			yield return new WaitForSeconds(1.4f);
		}
	}

	public bool ScoreboosterButtonIsActive()
	{
		return powerup1ButtonCollider.enabled;
	}

	public bool HeadstartButtonIsActive()
	{
		return powerup2ButtonCollider.enabled;
	}

	private IEnumerator AnimateButtonBackgrounGlow(UISprite sprite)
	{
		yield return StartCoroutine(AnimateSpriteColor(sprite, GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR, GlobalColors.FINAL_HEADSTART_BUTTON_GLOW_COLOR, 5f));
		StartCoroutine(AnimateSpriteColor(sprite, GlobalColors.FINAL_HEADSTART_BUTTON_GLOW_COLOR, GlobalColors.INITIAL_HEADSTART_BUTTON_GLOW_COLOR, 5f));
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

	private void UpdateAmountLabel(UILabel label, int amount)
	{
		if (amount > 0 || !SellConsumable.allowSellConsumablesDuringRun)
		{
			label.alignment = NGUIText.Alignment.Right;
			label.text = amount.ToString();
		}
		else
		{
			label.alignment = NGUIText.Alignment.Center;
			label.text = Strings.Get("CONSUMABLES_OWNED_AMOUNT_NONE");
		}
	}

	private void ResetScoreboosterGear()
	{
		_currentScoreboosterGear = 0;
	}

	private void ResetHeadstartGear()
	{
		_currentHeadstartGear = 0;
	}

	private void LogPowerupActivated(int powerup, bool success)
	{
		switch (powerup)
		{
		case 1:
			Flurry.LogEventWithAParameter("Boost scorebooster activated", "successfully", (!success) ? "no" : "yes");
			break;
		case 2:
			Flurry.LogEventWithAParameter("Boost headstart activated", "successfully", (!success) ? "no" : "yes");
			break;
		}
	}

	private void OnSellConsumableCompleted(UpgradeType upgradeType, int amountSold)
	{
		if (amountSold == 0)
		{
			switch (upgradeType)
			{
			case UpgradeType.scorebooster:
				HidePowerup1(true);
				break;
			case UpgradeType.headstart2000:
				HidePowerup2(true);
				break;
			}
		}
		IngameScreen.instance.RestartHidingScoreboosterSpeedometer(DELAY_AMOUNT_FOR_HIDING_THE_POWERUP_BUTTONS);
	}

	private void HidePowerups()
	{
		CancelInvoke("HideScorebooster");
		CancelInvoke("HideHeadstart");
		HidePowerup1();
		HidePowerup2();
	}

	private void OnJetpackStart()
	{
		HidePowerup2();
	}

	private void OnPogostickStart()
	{
		HidePowerup2();
	}
}
