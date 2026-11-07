using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Info;
using SYBO.Subway.Characters;
using UnityEngine;

public class SuperSneakers : CharacterModifier
{
	public delegate void OnSwitchToSuperSneakersDelegate();

	public delegate void SuperSneakerOnStopDelegate();

	[HideInInspector]
	public bool isActive;

	private Character character;

	private CharacterRendering characterRendering;

	public AudioClipInfo powerDownSound;

	private LongMagnet longMagnet;

	public float timeActiveInARow;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnSwitchToSuperSneakersDelegate OnSwitchToSuperSneakers__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SuperSneakerOnStopDelegate SuperSneakerOnStop__BackingField;

	public static Action OnSupersneakersTimeActiveChange;

	private static SuperSneakers instance;

	public override bool ShouldPauseInAlternateRoutes
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldPauseInJetpack
	{
		get
		{
			return true;
		}
	}

	public static SuperSneakers Instance
	{
		get
		{
			return instance ?? (instance = UnityEngine.Object.FindObjectOfType(typeof(SuperSneakers)) as SuperSneakers);
		}
	}

	public event OnSwitchToSuperSneakersDelegate OnSwitchToSuperSneakers
	{
		add
		{
			OnSwitchToSuperSneakersDelegate onSwitchToSuperSneakersDelegate = OnSwitchToSuperSneakers__BackingField;
			OnSwitchToSuperSneakersDelegate onSwitchToSuperSneakersDelegate2;
			do
			{
				onSwitchToSuperSneakersDelegate2 = onSwitchToSuperSneakersDelegate;
				onSwitchToSuperSneakersDelegate = Interlocked.CompareExchange(ref OnSwitchToSuperSneakers__BackingField, (OnSwitchToSuperSneakersDelegate)Delegate.Combine(onSwitchToSuperSneakersDelegate2, value), onSwitchToSuperSneakersDelegate);
			}
			while ((object)onSwitchToSuperSneakersDelegate != onSwitchToSuperSneakersDelegate2);
		}
		remove
		{
			OnSwitchToSuperSneakersDelegate onSwitchToSuperSneakersDelegate = OnSwitchToSuperSneakers__BackingField;
			OnSwitchToSuperSneakersDelegate onSwitchToSuperSneakersDelegate2;
			do
			{
				onSwitchToSuperSneakersDelegate2 = onSwitchToSuperSneakersDelegate;
				onSwitchToSuperSneakersDelegate = Interlocked.CompareExchange(ref OnSwitchToSuperSneakers__BackingField, (OnSwitchToSuperSneakersDelegate)Delegate.Remove(onSwitchToSuperSneakersDelegate2, value), onSwitchToSuperSneakersDelegate);
			}
			while ((object)onSwitchToSuperSneakersDelegate != onSwitchToSuperSneakersDelegate2);
		}
	}

	public event SuperSneakerOnStopDelegate SuperSneakerOnStop
	{
		add
		{
			SuperSneakerOnStopDelegate superSneakerOnStopDelegate = SuperSneakerOnStop__BackingField;
			SuperSneakerOnStopDelegate superSneakerOnStopDelegate2;
			do
			{
				superSneakerOnStopDelegate2 = superSneakerOnStopDelegate;
				superSneakerOnStopDelegate = Interlocked.CompareExchange(ref SuperSneakerOnStop__BackingField, (SuperSneakerOnStopDelegate)Delegate.Combine(superSneakerOnStopDelegate2, value), superSneakerOnStopDelegate);
			}
			while ((object)superSneakerOnStopDelegate != superSneakerOnStopDelegate2);
		}
		remove
		{
			SuperSneakerOnStopDelegate superSneakerOnStopDelegate = SuperSneakerOnStop__BackingField;
			SuperSneakerOnStopDelegate superSneakerOnStopDelegate2;
			do
			{
				superSneakerOnStopDelegate2 = superSneakerOnStopDelegate;
				superSneakerOnStopDelegate = Interlocked.CompareExchange(ref SuperSneakerOnStop__BackingField, (SuperSneakerOnStopDelegate)Delegate.Remove(superSneakerOnStopDelegate2, value), superSneakerOnStopDelegate);
			}
			while ((object)superSneakerOnStopDelegate != superSneakerOnStopDelegate2);
		}
	}

	public void Awake()
	{
		instance = this;
		longMagnet = base.gameObject.GetComponent<LongMagnet>();
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
	}

	public override void Pause()
	{
		base.Pause();
		character.IsJumpingHigher = false;
		isActive = false;
	}

	public override void Resume()
	{
		base.Resume();
		character.IsJumpingHigher = true;
		isActive = true;
	}

	public override void ExpireSoon()
	{
		base.Powerup.timeLeft = CharacterModifierCollection.TIME_LEFT_FOR_EXPIRE_SOON;
	}

	public void StopHack()
	{
		isActive = false;
		characterRendering.CharacterModel.MeshSuperSneaker.enabled = false;
	}

	public override void Prepare()
	{
		GameStats.Instance.PickedUpPowerupsIncrement();
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.supersneakers);
	}

	public override IEnumerator Begin()
	{
		character.IsJumpingHigher = true;
		base.Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		characterRendering.CharacterModel.MeshSuperSneaker.enabled = true;
		isActive = true;
		longMagnet.Activate();
		if (OnSwitchToSuperSneakers__BackingField != null)
		{
			OnSwitchToSuperSneakers__BackingField();
		}
		stop = StopSignal.DONT_STOP;
		while (base.Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			if (!base.Paused)
			{
				timeActiveInARow += Time.deltaTime;
				Action onSupersneakersTimeActiveChange = OnSupersneakersTimeActiveChange;
				if (onSupersneakersTimeActiveChange != null)
				{
					onSupersneakersTimeActiveChange();
				}
			}
			yield return null;
		}
		timeActiveInARow = 0f;
		characterRendering.CharacterModel.MeshSuperSneaker.enabled = false;
		isActive = false;
		if (!longMagnet.ShouldBeActive)
		{
			longMagnet.Deactivate();
		}
		if (base.Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
		character.IsJumpingHigher = false;
		if (SuperSneakerOnStop__BackingField != null)
		{
			SuperSneakerOnStop__BackingField();
		}
	}
}
