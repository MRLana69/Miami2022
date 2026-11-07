namespace Kiloo.Common.HTTP
{
	public enum RequestState
	{
		None = 0,
		Waiting = 1,
		Reading = 2,
		Done = 3,
		Canceled = 4,
		TimedOut = 5,
		Error = 6
	}
}
