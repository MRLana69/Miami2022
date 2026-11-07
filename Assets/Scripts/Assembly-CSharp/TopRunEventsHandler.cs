using System;
using System.Diagnostics;
using System.Threading;
using TopRun;

public class TopRunEventsHandler
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private EventHandler<TournamentChangedEventArgs> OnTopRunChangedEvent__BackingField;

	private static TopRunEventsHandler _instance;

	public static TopRunEventsHandler instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TopRunEventsHandler();
				TopRunRoot.manager.tournamentChanged += instance.TriggerOnTopRunChanged;
			}
			return _instance;
		}
	}

	public event EventHandler<TournamentChangedEventArgs> OnTopRunChangedEvent
	{
		add
		{
			EventHandler<TournamentChangedEventArgs> eventHandler = OnTopRunChangedEvent__BackingField;
			EventHandler<TournamentChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnTopRunChangedEvent__BackingField, (EventHandler<TournamentChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
		remove
		{
			EventHandler<TournamentChangedEventArgs> eventHandler = OnTopRunChangedEvent__BackingField;
			EventHandler<TournamentChangedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnTopRunChangedEvent__BackingField, (EventHandler<TournamentChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	private TopRunEventsHandler()
	{
	}

	public virtual void TriggerOnTopRunChanged(object o, TournamentChangedEventArgs eventArg = null)
	{
		if (eventArg == null)
		{
			TournamentChangedEventArgs e = new TournamentChangedEventArgs();
			e.tournamentWasUpdated = true;
			eventArg = e;
		}
		if (OnTopRunChangedEvent__BackingField != null)
		{
			OnTopRunChangedEvent__BackingField(this, eventArg);
		}
	}
}
