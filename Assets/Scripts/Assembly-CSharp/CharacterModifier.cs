using System.Collections;
using Info;
using UnityEngine;

public abstract class CharacterModifier : MonoBehaviour
{
	public enum StopSignal
	{
		DONT_STOP = 0,
		STOP = 1,
		STOP_NO_ENDING = 2
	}

	protected StopSignal stop;

	private bool _paused;

	public ActivePowerup _powerup;

	private IEnumerator current;

	public bool Paused
	{
		get
		{
			return _paused;
		}
		protected set
		{
			_paused = value;
		}
	}

	public ActivePowerup Powerup
	{
		get
		{
			return _powerup;
		}
		protected set
		{
			_powerup = value;
		}
	}

	public bool TriggeredByMysteryPowerup { get; set; }

	public StopSignal Stop
	{
		get
		{
			return stop;
		}
		set
		{
			stop = value;
		}
	}

	public virtual bool ShouldPauseInJetpack
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShouldPauseInAlternateRoutes
	{
		get
		{
			return false;
		}
	}

	public bool IsAllowedInAlternateRoutes
	{
		get
		{
			return !ShouldPauseInAlternateRoutes;
		}
	}

	public IEnumerator Current
	{
		get
		{
			return current;
		}
		set
		{
			current = value;
		}
	}

	public virtual void Prepare()
	{
	}

	public virtual IEnumerator Begin()
	{
		yield break;
	}

	public virtual void Pause()
	{
		_paused = true;
		if (Powerup != null)
		{
			Powerup.Paused = true;
		}
	}

	public virtual void Resume()
	{
		_paused = false;
		if (Powerup != null)
		{
			Powerup.Paused = false;
		}
	}

	public virtual void Reset()
	{
		TriggeredByMysteryPowerup = false;
		Resume();
	}

	public virtual void ExpireSoon()
	{
	}
}
