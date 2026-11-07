namespace SYBO.Components.Analytics.Integration
{
	public interface ISubwayBaseEvent
	{
		string UserId { get; }

		string EventId { get; }

		int SessionId { get; }

		int SessionEventId { get; }
	}
}
