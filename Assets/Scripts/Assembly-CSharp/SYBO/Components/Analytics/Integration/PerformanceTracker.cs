using SYBO.Subway;
using SYBO.Subway.Benchmarking;

namespace SYBO.Components.Analytics.Integration
{
	public class PerformanceTracker : IPerformanceTracker
	{
		private const int MaxFrameTimeSampleCount = 10;

		private FramerateTracker _framerateTracker;

		public float AverageInGameFramerate
		{
			get
			{
				return _framerateTracker.GetFramerate();
			}
		}

		public float MaxFrameTimeMS
		{
			get
			{
				return _framerateTracker.MaxFrameTime * 1000f;
			}
		}

		public PerformanceTracker(float maxFrameTimeSampleRangeMS)
		{
			_framerateTracker = new FramerateTracker(maxFrameTimeSampleRangeMS * 0.001f, 10);
			Game.OnGameStarted += Game_OnGameStarted;
			Game.OnGameEnded += Game_OnGameEnded;
		}

		private void Game_OnGameStarted()
		{
			_framerateTracker.StartTracking();
		}

		private void Game_OnGameEnded()
		{
			_framerateTracker.EndTracking();
		}

		public void Reset()
		{
			_framerateTracker.Reset();
		}
	}
}
