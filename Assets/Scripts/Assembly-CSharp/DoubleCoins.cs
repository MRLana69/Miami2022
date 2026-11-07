using System.Collections;
using Info;
using UnityEngine;

public class DoubleCoins : CharacterModifier
{
	public float ratio;

	public AudioClipInfo powerDownSound;

	public static bool HasDoubleCoinsModifier;

	public static int numberOfAnimationBlinks;

	private static DoubleCoins instance;

	public static DoubleCoins Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(DoubleCoins)) as DoubleCoins);
		}
	}

	public void Awake()
	{
		instance = this;
	}

	public override IEnumerator Begin()
	{
		numberOfAnimationBlinks = 0;
		GameStats.Instance.PickedUpPowerupsIncrement();
		base.Paused = false;
		stop = StopSignal.DONT_STOP;
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.doubleCoinsMysteryPowerup, base.TriggeredByMysteryPowerup);
		float duration = base.Powerup.timeLeft;
		StartDoubleCoins();
		while (base.Powerup.timeLeft >= 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = base.Powerup.timeLeft / duration;
			yield return null;
		}
		StopDoubleCoins();
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
		StopDoubleCoins();
	}

	public void StartDoubleCoins()
	{
		UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Upgrades.upgrades[UpgradeType.doubleCoinsMysteryPowerup].name), true);
		HasDoubleCoinsModifier = true;
	}

	public void StopDoubleCoins()
	{
		HasDoubleCoinsModifier = false;
		numberOfAnimationBlinks = 0;
	}
}
