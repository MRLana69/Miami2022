namespace SYBO.Components.Analytics.Integration
{
	public interface IPerformanceTracker
	{
		float AverageInGameFramerate { get; }

		float MaxFrameTimeMS { get; }

		void Reset();
	}
}
