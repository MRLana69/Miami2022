using System.Collections;
using Info;
using UnityEngine;

public class PlusMultiplier : CharacterModifier
{
	public float ratio;

	public AudioClipInfo powerDownSound;

	public static bool plusMultiplierEnabled;

	private static PlusMultiplier instance;

	public static PlusMultiplier Instance
	{
		get
		{
			return instance ?? (instance = Object.FindObjectOfType(typeof(PlusMultiplier)) as PlusMultiplier);
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.PickedUpPowerupsIncrement();
		base.Paused = false;
		stop = StopSignal.DONT_STOP;
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.plusMultiplier, base.TriggeredByMysteryPowerup);
		float duration = base.Powerup.timeLeft;
		StartPlusMultiplier();
		while (base.Powerup.timeLeft >= 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = base.Powerup.timeLeft / duration;
			yield return null;
		}
		StopPlusMultiplier();
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
		plusMultiplierEnabled = false;
	}

	public void StartPlusMultiplier()
	{
		UIScreenController.Instance.messageHelper.ShowMessage(Strings.Get(Upgrades.upgrades[UpgradeType.plusMultiplier].name), true);
		plusMultiplierEnabled = true;
		ChangeTheMultiplier();
	}

	public void StopPlusMultiplier()
	{
		plusMultiplierEnabled = false;
		ResetLabelColour();
		ChangeTheMultiplier();
	}

	private void ResetLabelColour()
	{
		UIScreenController uIScreenController = UIScreenController.Instance;
		IngameScreen ingameScreen = uIScreenController.GetScreenFromCache(uIScreenController.GetTopScreenName()) as IngameScreen;
		if (ingameScreen != null)
		{
			ingameScreen.ResetMultiplierLabelColour();
		}
	}

	private void ChangeTheMultiplier()
	{
		ScoreMultiplierInfo.TriggerOnScoreMultiplierChanged();
	}
}
