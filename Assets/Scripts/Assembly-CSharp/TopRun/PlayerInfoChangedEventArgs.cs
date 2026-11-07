using System;

namespace TopRun
{
	public class PlayerInfoChangedEventArgs : EventArgs
	{
		public IPlayer player { get; internal set; }
	}
}
