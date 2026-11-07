using System.Collections;
using Info;
using SYBO.Subway.Characters;
using UnityEngine;

public class DoubleJump : CharacterModifier
{
	public float ratio;

	public AudioClipInfo powerDownSound;

	public static bool doubleJumpEnabled;

	private static DoubleJump instance;

	private Character character;

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

	public static DoubleJump Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(DoubleJump)) as DoubleJump);
		}
	}

	private void Awake()
	{
		instance = this;
		character = Character.Instance;
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.PickedUpPowerupsIncrement();
		base.Paused = false;
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.doubleJump, base.TriggeredByMysteryPowerup);
		float duration = base.Powerup.timeLeft;
		StartDoubleJump();
		stop = StopSignal.DONT_STOP;
		while (base.Powerup.timeLeft >= 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = base.Powerup.timeLeft / duration;
			yield return null;
		}
		StopDoubleJump();
		if (base.Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public override void ExpireSoon()
	{
		base.Powerup.timeLeft = CharacterModifierCollection.TIME_LEFT_FOR_EXPIRE_SOON;
	}

	public override void Reset()
	{
		base.Reset();
		StopDoubleJump();
	}

	public override void Pause()
	{
		base.Pause();
		doubleJumpEnabled = false;
		character.IsDoubleJumping = false;
		if (character.CharacterModel.DoubleJumpers != null)
		{
			character.CharacterModel.DoubleJumpers.enabled = false;
		}
	}

	public override void Resume()
	{
		base.Resume();
		doubleJumpEnabled = true;
		character.IsDoubleJumping = true;
		if (character.CharacterModel.DoubleJumpers != null)
		{
			character.CharacterModel.DoubleJumpers.enabled = true;
		}
	}

	public void StartDoubleJump()
	{
		UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Upgrades.upgrades[UpgradeType.doubleJump].name), true);
		doubleJumpEnabled = true;
		character.IsDoubleJumping = true;
		if (character.CharacterModel.DoubleJumpers != null)
		{
			character.CharacterModel.DoubleJumpers.enabled = true;
		}
	}

	public void StopDoubleJump()
	{
		base.Paused = false;
		doubleJumpEnabled = false;
		character.IsDoubleJumping = false;
		if (character.CharacterModel.DoubleJumpers != null)
		{
			character.CharacterModel.DoubleJumpers.enabled = false;
		}
	}
}
