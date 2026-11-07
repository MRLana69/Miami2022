using System.Collections;
using Info;

public class DoubleScoreMultiplier : CharacterModifier
{
	public float ratio;

	public AudioClipInfo powerDownSound;

	private static bool _doubleScoreActive;

	public static bool doubleScoreActive
	{
		get
		{
			return _doubleScoreActive;
		}
		private set
		{
			if (_doubleScoreActive != value)
			{
				_doubleScoreActive = value;
				ScoreMultiplierInfo.TriggerOnScoreMultiplierChanged();
			}
		}
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.PickedUpPowerupsIncrement();
		base.Paused = false;
		stop = StopSignal.DONT_STOP;
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.doubleMultiplier);
		float duration = base.Powerup.timeLeft;
		doubleScoreActive = true;
		while (base.Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = base.Powerup.timeLeft / duration;
			yield return null;
		}
		doubleScoreActive = false;
		if (base.Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public override void Reset()
	{
		base.Reset();
		doubleScoreActive = false;
	}
}
