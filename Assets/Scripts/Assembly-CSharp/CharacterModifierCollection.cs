using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class CharacterModifierCollection
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private EventHandler<CharacterModifierCollectionChangedEventArgs> OnChanged__BackingField;

	private CoinMagnet coinMagnet;

	private SuperSneakers superSneakers;

	private Hoverboard hoverboard;

	private DoubleScoreMultiplier doubleScoreMultiplier;

	private DoubleCoins doubleCoins;

	private DoubleJump doubleJump;

	private PlusMultiplier plusMultiplier;

	private List<CharacterModifier> modifiers = new List<CharacterModifier>();

	public static float TIME_LEFT_FOR_EXPIRE_SOON = 1f;

	private List<CharacterModifier> deadModifiers = new List<CharacterModifier>();

	public CoinMagnet CoinMagnet
	{
		get
		{
			return coinMagnet;
		}
	}

	public SuperSneakers SuperSneakes
	{
		get
		{
			return superSneakers;
		}
	}

	public Hoverboard Hoverboard
	{
		get
		{
			return hoverboard;
		}
	}

	public DoubleScoreMultiplier DoubleScoreMultiplier
	{
		get
		{
			return doubleScoreMultiplier;
		}
	}

	public DoubleCoins DoubleCoins
	{
		get
		{
			return doubleCoins;
		}
	}

	public DoubleJump DoubleJump
	{
		get
		{
			return doubleJump;
		}
	}

	public PlusMultiplier PlusMultiplier
	{
		get
		{
			return plusMultiplier;
		}
	}

	public event EventHandler<CharacterModifierCollectionChangedEventArgs> OnChanged
	{
		add
		{
			EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler = OnChanged__BackingField;
			EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnChanged__BackingField, (EventHandler<CharacterModifierCollectionChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
		remove
		{
			EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler = OnChanged__BackingField;
			EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnChanged__BackingField, (EventHandler<CharacterModifierCollectionChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public CharacterModifierCollection()
	{
		coinMagnet = UnityEngine.Object.FindObjectOfType(typeof(CoinMagnet)) as CoinMagnet;
		superSneakers = UnityEngine.Object.FindObjectOfType(typeof(SuperSneakers)) as SuperSneakers;
		hoverboard = Hoverboard.Instance;
		doubleScoreMultiplier = UnityEngine.Object.FindObjectOfType(typeof(DoubleScoreMultiplier)) as DoubleScoreMultiplier;
		doubleCoins = UnityEngine.Object.FindObjectOfType(typeof(DoubleCoins)) as DoubleCoins;
		doubleJump = UnityEngine.Object.FindObjectOfType(typeof(DoubleJump)) as DoubleJump;
		plusMultiplier = UnityEngine.Object.FindObjectOfType(typeof(PlusMultiplier)) as PlusMultiplier;
	}

	public void Add(CharacterModifier modifier)
	{
		if (!modifiers.Contains(modifier))
		{
			modifiers.Add(modifier);
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Added, modifier));
			modifier.Prepare();
			modifier.Current = modifier.Begin();
		}
		else
		{
			modifier.Reset();
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Reset, modifier));
			modifier.Prepare();
			modifier.Current = modifier.Begin();
		}
	}

	public void Update()
	{
		if (modifiers.Count <= 0)
		{
			return;
		}
		deadModifiers.Clear();
		foreach (CharacterModifier modifier in modifiers)
		{
			if (!modifier.Paused && !modifier.Current.MoveNext())
			{
				deadModifiers.Add(modifier);
				RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Added, modifier));
			}
		}
		if (deadModifiers.Count <= 0)
		{
			return;
		}
		foreach (CharacterModifier deadModifier in deadModifiers)
		{
			modifiers.Remove(deadModifier);
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Removed, deadModifier));
		}
	}

	public bool IsActive(CharacterModifier modifier)
	{
		return modifiers.Contains(modifier);
	}

	public void StopWithNoEnding()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			modifier.Stop = CharacterModifier.StopSignal.STOP_NO_ENDING;
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.StoppedNoEnding, modifier));
		}
	}

	public void Stop()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			modifier.Stop = CharacterModifier.StopSignal.STOP;
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Stopped, modifier));
		}
	}

	public void ExpireSoon()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			modifier.ExpireSoon();
		}
	}

	public void Pause()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			modifier.Pause();
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Paused, modifier));
		}
	}

	public void PauseInJetpackMode()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			if (modifier.ShouldPauseInJetpack)
			{
				modifier.Pause();
				RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Paused, modifier));
			}
		}
	}

	public void PauseInAlternateRoutes()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			if (modifier.ShouldPauseInAlternateRoutes)
			{
				modifier.Pause();
				RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Paused, modifier));
			}
		}
	}

	public void Resume()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			modifier.Resume();
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Resumed, modifier));
		}
	}

	public void Reset()
	{
		foreach (CharacterModifier modifier in modifiers)
		{
			modifier.Reset();
			RaiseOnEventChanged(new CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType.Reset, modifier));
		}
		modifiers.Clear();
	}

	private void RaiseOnEventChanged(CharacterModifierCollectionChangedEventArgs characterModifierCollectionChangedEventArgs)
	{
		if (OnChanged__BackingField != null)
		{
			OnChanged__BackingField(this, characterModifierCollectionChangedEventArgs);
		}
	}
}
