using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Info;
using Missions;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Routes;
using UnityEngine;

public class Hoverboard : CharacterModifier
{
	public enum BoardAbility
	{
		None = 0,
		Jumper = 1,
		Lowrider = 2,
		HighSpeed = 3,
		Speed = 4,
		DoubleJump = 5,
		Glider = 6,
		Teleport = 7,
		Vanity = 8
	}

	public delegate void OnSwitchToHoverboardDelegate(HoverboardBundle hoverbooardBundle, BoardAbility[] abilities);

	public delegate void OnEndHoverboardDelegate();

	public delegate void OnHoverboardJumpDelegate();

	public delegate void OnRunDelegate();

	public delegate void OnSpeedStartDelegate();

	public delegate void OnSpeedEndDelegate();

	private bool isJumper;

	private bool isLowrider;

	private bool isHighSpeedBoard;

	private bool isSpeedBoard;

	private bool isGlider;

	private bool isVTol;

	private bool isTeleporter;

	private bool isDoubleJump;

	private bool isVanity;

	[OptionalField]
	public GameObject currentHoverboard;

	public AudioClipInfo powerDownSound;

	public float cooldownDstance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	private GameObject hoverboardRoot;

	public float WaitForParticlesDelay;

	public float RemoveObstaclesDistance = 250f;

	private LongMagnet longMagnet;

	private Character character;

	[HideInInspector]
	public bool isActive;

	public bool _isInCooldown;

	private bool __isAllowed = true;

	[HideInInspector]
	private bool _isARewardFromMysteryPowerup;

	public AudioClipInfo CrashSound;

	public AudioClipInfo StartSound;

	private static Hoverboard instance;

	private Boards.BoardId _currentRandomBoard;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnSwitchToHoverboardDelegate OnSwitchToHoverboard__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnEndHoverboardDelegate OnEndHoverboard__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnHoverboardJumpDelegate OnJump__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnRunDelegate OnRun__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnSpeedStartDelegate OnSpeedStart__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnSpeedEndDelegate OnSpeedEnd__BackingField;

	public bool IsJumper
	{
		get
		{
			return isJumper;
		}
	}

	public bool IsLowrider
	{
		get
		{
			return isLowrider;
		}
	}

	public bool IsHighSpeedBoard
	{
		get
		{
			return isHighSpeedBoard;
		}
	}

	public bool IsSpeedBoard
	{
		get
		{
			return isSpeedBoard;
		}
	}

	public bool IsGlider
	{
		get
		{
			return isGlider;
		}
	}

	public bool IsVTol
	{
		get
		{
			return isVTol;
		}
	}

	public bool IsTeleporter
	{
		get
		{
			return isTeleporter;
		}
	}

	public bool IsDoubleJump
	{
		get
		{
			return isDoubleJump;
		}
	}

	public bool IsVanity
	{
		get
		{
			return isVanity;
		}
	}

	public bool IsInCooldown
	{
		get
		{
			return _isInCooldown;
		}
	}

	public bool IsAllowed
	{
		get
		{
			return __isAllowed;
		}
		set
		{
			__isAllowed = value;
		}
	}

	public bool IsARewardFromMysteryPowerup
	{
		get
		{
			return _isARewardFromMysteryPowerup;
		}
		set
		{
			_isARewardFromMysteryPowerup = value;
		}
	}

	public override bool ShouldPauseInJetpack
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldPauseInAlternateRoutes
	{
		get
		{
			return true;
		}
	}

	public static Hoverboard Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(Hoverboard)) as Hoverboard);
		}
	}

	public event OnSwitchToHoverboardDelegate OnSwitchToHoverboard
	{
		add
		{
			OnSwitchToHoverboardDelegate onSwitchToHoverboardDelegate = OnSwitchToHoverboard__BackingField;
			OnSwitchToHoverboardDelegate onSwitchToHoverboardDelegate2;
			do
			{
				onSwitchToHoverboardDelegate2 = onSwitchToHoverboardDelegate;
				onSwitchToHoverboardDelegate = Interlocked.CompareExchange(ref OnSwitchToHoverboard__BackingField, (OnSwitchToHoverboardDelegate)Delegate.Combine(onSwitchToHoverboardDelegate2, value), onSwitchToHoverboardDelegate);
			}
			while ((object)onSwitchToHoverboardDelegate != onSwitchToHoverboardDelegate2);
		}
		remove
		{
			OnSwitchToHoverboardDelegate onSwitchToHoverboardDelegate = OnSwitchToHoverboard__BackingField;
			OnSwitchToHoverboardDelegate onSwitchToHoverboardDelegate2;
			do
			{
				onSwitchToHoverboardDelegate2 = onSwitchToHoverboardDelegate;
				onSwitchToHoverboardDelegate = Interlocked.CompareExchange(ref OnSwitchToHoverboard__BackingField, (OnSwitchToHoverboardDelegate)Delegate.Remove(onSwitchToHoverboardDelegate2, value), onSwitchToHoverboardDelegate);
			}
			while ((object)onSwitchToHoverboardDelegate != onSwitchToHoverboardDelegate2);
		}
	}

	public event OnEndHoverboardDelegate OnEndHoverboard
	{
		add
		{
			OnEndHoverboardDelegate onEndHoverboardDelegate = OnEndHoverboard__BackingField;
			OnEndHoverboardDelegate onEndHoverboardDelegate2;
			do
			{
				onEndHoverboardDelegate2 = onEndHoverboardDelegate;
				onEndHoverboardDelegate = Interlocked.CompareExchange(ref OnEndHoverboard__BackingField, (OnEndHoverboardDelegate)Delegate.Combine(onEndHoverboardDelegate2, value), onEndHoverboardDelegate);
			}
			while ((object)onEndHoverboardDelegate != onEndHoverboardDelegate2);
		}
		remove
		{
			OnEndHoverboardDelegate onEndHoverboardDelegate = OnEndHoverboard__BackingField;
			OnEndHoverboardDelegate onEndHoverboardDelegate2;
			do
			{
				onEndHoverboardDelegate2 = onEndHoverboardDelegate;
				onEndHoverboardDelegate = Interlocked.CompareExchange(ref OnEndHoverboard__BackingField, (OnEndHoverboardDelegate)Delegate.Remove(onEndHoverboardDelegate2, value), onEndHoverboardDelegate);
			}
			while ((object)onEndHoverboardDelegate != onEndHoverboardDelegate2);
		}
	}

	public event OnHoverboardJumpDelegate OnJump
	{
		add
		{
			OnHoverboardJumpDelegate onHoverboardJumpDelegate = OnJump__BackingField;
			OnHoverboardJumpDelegate onHoverboardJumpDelegate2;
			do
			{
				onHoverboardJumpDelegate2 = onHoverboardJumpDelegate;
				onHoverboardJumpDelegate = Interlocked.CompareExchange(ref OnJump__BackingField, (OnHoverboardJumpDelegate)Delegate.Combine(onHoverboardJumpDelegate2, value), onHoverboardJumpDelegate);
			}
			while ((object)onHoverboardJumpDelegate != onHoverboardJumpDelegate2);
		}
		remove
		{
			OnHoverboardJumpDelegate onHoverboardJumpDelegate = OnJump__BackingField;
			OnHoverboardJumpDelegate onHoverboardJumpDelegate2;
			do
			{
				onHoverboardJumpDelegate2 = onHoverboardJumpDelegate;
				onHoverboardJumpDelegate = Interlocked.CompareExchange(ref OnJump__BackingField, (OnHoverboardJumpDelegate)Delegate.Remove(onHoverboardJumpDelegate2, value), onHoverboardJumpDelegate);
			}
			while ((object)onHoverboardJumpDelegate != onHoverboardJumpDelegate2);
		}
	}

	public event OnRunDelegate OnRun
	{
		add
		{
			OnRunDelegate onRunDelegate = OnRun__BackingField;
			OnRunDelegate onRunDelegate2;
			do
			{
				onRunDelegate2 = onRunDelegate;
				onRunDelegate = Interlocked.CompareExchange(ref OnRun__BackingField, (OnRunDelegate)Delegate.Combine(onRunDelegate2, value), onRunDelegate);
			}
			while ((object)onRunDelegate != onRunDelegate2);
		}
		remove
		{
			OnRunDelegate onRunDelegate = OnRun__BackingField;
			OnRunDelegate onRunDelegate2;
			do
			{
				onRunDelegate2 = onRunDelegate;
				onRunDelegate = Interlocked.CompareExchange(ref OnRun__BackingField, (OnRunDelegate)Delegate.Remove(onRunDelegate2, value), onRunDelegate);
			}
			while ((object)onRunDelegate != onRunDelegate2);
		}
	}

	public event OnSpeedStartDelegate OnSpeedStart
	{
		add
		{
			OnSpeedStartDelegate onSpeedStartDelegate = OnSpeedStart__BackingField;
			OnSpeedStartDelegate onSpeedStartDelegate2;
			do
			{
				onSpeedStartDelegate2 = onSpeedStartDelegate;
				onSpeedStartDelegate = Interlocked.CompareExchange(ref OnSpeedStart__BackingField, (OnSpeedStartDelegate)Delegate.Combine(onSpeedStartDelegate2, value), onSpeedStartDelegate);
			}
			while ((object)onSpeedStartDelegate != onSpeedStartDelegate2);
		}
		remove
		{
			OnSpeedStartDelegate onSpeedStartDelegate = OnSpeedStart__BackingField;
			OnSpeedStartDelegate onSpeedStartDelegate2;
			do
			{
				onSpeedStartDelegate2 = onSpeedStartDelegate;
				onSpeedStartDelegate = Interlocked.CompareExchange(ref OnSpeedStart__BackingField, (OnSpeedStartDelegate)Delegate.Remove(onSpeedStartDelegate2, value), onSpeedStartDelegate);
			}
			while ((object)onSpeedStartDelegate != onSpeedStartDelegate2);
		}
	}

	public event OnSpeedEndDelegate OnSpeedEnd
	{
		add
		{
			OnSpeedEndDelegate onSpeedEndDelegate = OnSpeedEnd__BackingField;
			OnSpeedEndDelegate onSpeedEndDelegate2;
			do
			{
				onSpeedEndDelegate2 = onSpeedEndDelegate;
				onSpeedEndDelegate = Interlocked.CompareExchange(ref OnSpeedEnd__BackingField, (OnSpeedEndDelegate)Delegate.Combine(onSpeedEndDelegate2, value), onSpeedEndDelegate);
			}
			while ((object)onSpeedEndDelegate != onSpeedEndDelegate2);
		}
		remove
		{
			OnSpeedEndDelegate onSpeedEndDelegate = OnSpeedEnd__BackingField;
			OnSpeedEndDelegate onSpeedEndDelegate2;
			do
			{
				onSpeedEndDelegate2 = onSpeedEndDelegate;
				onSpeedEndDelegate = Interlocked.CompareExchange(ref OnSpeedEnd__BackingField, (OnSpeedEndDelegate)Delegate.Remove(onSpeedEndDelegate2, value), onSpeedEndDelegate);
			}
			while ((object)onSpeedEndDelegate != onSpeedEndDelegate2);
		}
	}

	public void Awake()
	{
		instance = this;
	}

	public void intialize()
	{
		Game.Instance.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
		character = Character.Instance;
		hoverboardRoot = character.CharacterModel.AttachPoint1.gameObject;
		longMagnet = base.gameObject.GetComponent<LongMagnet>();
	}

	private void Game_OnCharacterModelRecreated()
	{
		hoverboardRoot = character.CharacterModel.AttachPoint1.gameObject;
	}

	public override void Reset()
	{
		base.Reset();
		character.ImmuneToCriticalHit = false;
		character.CharacterController.enabled = true;
		character.CharacterTrigger.enabled = true;
		hoverboardRoot.SetActive(false);
		isActive = false;
		SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInGameDefault;
		character.HoverboardCrashParticleSystem.gameObject.SetActive(false);
	}

	public void SetupAbilitiesAndVisuals(HoverboardBundle hoverboardBundle)
	{
		List<BoardAbility> list = new List<BoardAbility>();
		Renderer[] componentsInChildren = hoverboardBundle.GetComponentsInChildren<Renderer>();
		HoverboardBundle.CustomSet[] customSets = hoverboardBundle.CustomSets;
		if (customSets.Length > 0)
		{
			bool flag = true;
			if (customSets.Length > 1 && BoardManager.IsCustomSetEquippedForBoard(PlayerSave.currentBoardId, 0) && !customSets[1].useDefaultmodel)
			{
				flag = false;
			}
			if (customSets.Length > 2 && BoardManager.IsCustomSetEquippedForBoard(PlayerSave.currentBoardId, 1) && !customSets[2].useDefaultmodel)
			{
				flag = false;
			}
			for (int i = 0; i < customSets.Length; i++)
			{
				if ((i != 0 || !flag) && (i != 1 || !BoardManager.IsCustomSetEquippedForBoard(PlayerSave.currentBoardId, 0)) && (i != 2 || !BoardManager.IsCustomSetEquippedForBoard(PlayerSave.currentBoardId, 1)))
				{
					continue;
				}
				list.Add(customSets[i].ability);
				if (customSets[i].baseModelPrefab != null)
				{
					hoverboardBundle.AddCustomBaseModel(i, false);
					Renderer[] array = componentsInChildren;
					foreach (Renderer renderer in array)
					{
						renderer.enabled = false;
					}
				}
				hoverboardBundle.AddCustomSetModelAndEffect(i, false, true);
				if (customSets[i].ability == BoardAbility.Jumper)
				{
					isJumper = true;
					if (customSets[i].useSuperSneakers)
					{
						character.CharacterModel.MeshSuperSneaker.enabled = true;
					}
					longMagnet.Activate();
					playThruster componentInChildren = hoverboardBundle.gameObject.GetComponentInChildren<playThruster>();
					if (componentInChildren != null && componentInChildren.jumpStyle == playThruster.JumpStyle.Super)
					{
						componentInChildren.Initialize();
					}
				}
				else if (customSets[i].ability == BoardAbility.Lowrider)
				{
					isLowrider = true;
					character.SqueezeCollider.Add(this);
				}
				else if (customSets[i].ability == BoardAbility.HighSpeed)
				{
					if (OnSpeedStart__BackingField != null)
					{
						OnSpeedStart__BackingField();
					}
					isHighSpeedBoard = true;
					Game.Instance.ActivateHighSpeed(Game.SpeedBoostType.High);
				}
				else if (customSets[i].ability == BoardAbility.Speed)
				{
					if (OnSpeedStart__BackingField != null)
					{
						OnSpeedStart__BackingField();
					}
					isSpeedBoard = true;
					Game.Instance.ActivateHighSpeed(Game.SpeedBoostType.Normal);
				}
				else if (customSets[i].ability == BoardAbility.Glider)
				{
					isGlider = true;
					longMagnet.Activate();
					GliderAnimation componentInChildren2 = hoverboardBundle.gameObject.GetComponentInChildren<GliderAnimation>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.AddEvents();
					}
				}
				else if (customSets[i].ability == BoardAbility.Teleport)
				{
					isTeleporter = true;
				}
				else if (customSets[i].ability == BoardAbility.DoubleJump)
				{
					isDoubleJump = true;
					playThruster componentInChildren3 = hoverboardBundle.gameObject.GetComponentInChildren<playThruster>();
					if (componentInChildren3 != null && componentInChildren3.jumpStyle == playThruster.JumpStyle.Double)
					{
						componentInChildren3.Initialize();
					}
				}
				else if (customSets[i].ability == BoardAbility.Vanity)
				{
					isVanity = true;
				}
			}
		}
		if (OnSwitchToHoverboard__BackingField != null)
		{
			OnSwitchToHoverboard__BackingField(hoverboardBundle, list.ToArray());
		}
	}

	public void ResetBoardAndAbilitys()
	{
		character.CharacterModel.RemoveCurrentHoverboard();
		if (isJumper)
		{
			if (!longMagnet.ShouldBeActive)
			{
				longMagnet.Deactivate();
			}
			if (!SuperSneakers.Instance.isActive)
			{
				if (SuperSneakers.Instance._powerup == null || !SuperSneakers.Instance._powerup.Paused)
				{
					character.CharacterModel.MeshSuperSneaker.enabled = false;
				}
			}
			else
			{
				SuperSneakers.Instance.Resume();
			}
			isJumper = false;
		}
		if (isLowrider)
		{
			character.SqueezeCollider.Remove(this);
			isLowrider = false;
		}
		if (isHighSpeedBoard)
		{
			if (OnSpeedEnd__BackingField != null)
			{
				OnSpeedEnd__BackingField();
			}
			Game.Instance.DeactivateHighSpeed();
			isHighSpeedBoard = false;
		}
		if (isSpeedBoard)
		{
			if (OnSpeedEnd__BackingField != null)
			{
				OnSpeedEnd__BackingField();
			}
			Game.Instance.DeactivateHighSpeed();
			isSpeedBoard = false;
		}
		if (isGlider)
		{
			isGlider = false;
			if (!longMagnet.ShouldBeActive)
			{
				longMagnet.Deactivate();
			}
		}
		if (isTeleporter)
		{
			isTeleporter = false;
		}
		if (isDoubleJump)
		{
			isDoubleJump = false;
		}
	}

	public override IEnumerator Begin()
	{
		if (!IsAllowed || _isInCooldown)
		{
			yield break;
		}
		hoverboardRoot.SetActive(true);
		HoverboardBundle newBoard = null;
		OnboardingSave.numberOfHoverboardsUsed++;
		if (!_isARewardFromMysteryPowerup)
		{
			InventorySave.Use(UpgradeType.hoverboard);
			newBoard = character.CharacterModel.AddEquippedHoverboard();
		}
		else
		{
			ResetBoardAndAbilitys();
			newBoard = character.CharacterModel.AddRandomHoverboard();
			_currentRandomBoard = newBoard.GetComponent<HoverboardBundle>().boardId;
			string message = Strings.Get(Boards.boardData[_currentRandomBoard].name) + " " + Strings.Get("MYSTERY_POWERUP_RANDOM_HOVERBOARD_TEXT");
			UIScreenController.Instance.messageHelper.ShowMessage(message, true);
		}
		SetupAbilitiesAndVisuals(newBoard);
		MissionsManager.Instance.PlayerDidThis(MissionTarget.HoverBoard);
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.hoverboard, base.TriggeredByMysteryPowerup);
		base.Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		isActive = true;
		So.Instance.playSound(StartSound);
		character.CharacterPickupParticleSystem.PickedUpDefaultPowerUp();
		character.ImmuneToCriticalHit = true;
		stop = StopSignal.DONT_STOP;
		while (base.Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			yield return null;
		}
		character.ImmuneToCriticalHit = false;
		isActive = false;
		ResetBoardAndAbilitys();
		if (stop == StopSignal.DONT_STOP)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.HoverBoardExpire);
			So.Instance.playSound(powerDownSound);
			_isInCooldown = true;
			RaiseOnEndHoverboard();
			if (character.IsFalling || character.IsJumping)
			{
				if (OnJump__BackingField != null)
				{
					OnJump__BackingField();
				}
			}
			else if (OnRun__BackingField != null)
			{
				OnRun__BackingField();
			}
			UnityEngine.Object.Destroy(newBoard);
		}
		if (stop != StopSignal.STOP)
		{
			yield break;
		}
		isActive = false;
		character.ImmuneToCriticalHit = false;
		_isInCooldown = true;
		character.HoverboardCrashParticleSystem.gameObject.SetActive(true);
		character.HoverboardCrashParticleSystem.Play();
		PlayCrashSound();
		RaiseOnEndHoverboard();
		if (OnJump__BackingField != null)
		{
			OnJump__BackingField();
		}
		UnityEngine.Object.Destroy(newBoard);
		float timeLeft = WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		SingletonMonoBehaviour<RouteManager>.Instance.CharacterRoute.Builder.HideLogic(character.GamePosition.z, RemoveObstaclesDistance * Game.Instance.CurrentSpeedNormalized);
		character.Revive();
		if (character.IsRolling)
		{
			character.EndRoll();
		}
		float newSlowMotionDistance = slowMotionDistance * Game.Instance.CurrentSpeedNormalized;
		float newCoolDownDist = cooldownDstance * Game.Instance.CurrentSpeedNormalized;
		float distanceLeft = newSlowMotionDistance;
		bool didStopCooldown = false;
		while (distanceLeft > 0f)
		{
			distanceLeft -= Game.Instance.CurrentLevelSpeed * Time.deltaTime;
			newCoolDownDist -= Game.Instance.CurrentLevelSpeed * Time.deltaTime;
			if (newCoolDownDist < 0f && !didStopCooldown)
			{
				character.ImmuneToCriticalHit = false;
				didStopCooldown = true;
			}
			FinishRandomBoard();
			yield return null;
		}
		character.HoverboardCrashParticleSystem.gameObject.SetActive(false);
		UnityEngine.Object.Destroy(newBoard);
	}

	public void PlayCrashSound()
	{
		So.Instance.playSound(CrashSound);
	}

	public override void Pause()
	{
		base.Pause();
		ResetBoardAndAbilitys();
		bool flag = PlayerSave.currentBoardId == Boards.BoardId.speedboard;
		bool flag2 = PlayerSave.currentBoardId == Boards.BoardId.hotrod;
		if (flag || flag2)
		{
			Game.Instance.DeactivateHighSpeed();
		}
	}

	public override void Resume()
	{
		base.Resume();
		if (base.Powerup.timeLeft > 0f)
		{
			if (!IsARewardFromMysteryPowerup)
			{
				HoverboardBundle hoverboardBundle = character.CharacterModel.AddEquippedHoverboard();
				SetupAbilitiesAndVisuals(hoverboardBundle);
			}
			else
			{
				HoverboardBundle hoverboardBundle2 = character.CharacterModel.AddHoverboard(_currentRandomBoard.ToString(), false);
				SetupAbilitiesAndVisuals(hoverboardBundle2);
			}
		}
	}

	private void FinishRandomBoard()
	{
		_currentRandomBoard = Boards.BoardId.normal;
	}

	public override void ExpireSoon()
	{
		base.Powerup.timeLeft = CharacterModifierCollection.TIME_LEFT_FOR_EXPIRE_SOON;
	}

	public void HardReset()
	{
		isActive = false;
		_isInCooldown = false;
		if (OnboardingSave.isTutorialCompleted)
		{
			IsAllowed = true;
		}
		else
		{
			IsAllowed = false;
		}
		_isARewardFromMysteryPowerup = false;
		FinishRandomBoard();
	}

	private void RaiseOnEndHoverboard()
	{
		if (OnEndHoverboard__BackingField != null)
		{
			OnEndHoverboard__BackingField();
		}
	}
}
