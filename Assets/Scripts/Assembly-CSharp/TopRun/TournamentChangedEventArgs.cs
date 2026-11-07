using System;

namespace TopRun
{
	public class TournamentChangedEventArgs : EventArgs
	{
		public bool tournamentDidChange { get; internal set; }

		public bool tournamentWasUpdated { get; internal set; }
	}
}
