namespace SYBO.Common.Benchmarking
{
	public class FrameTimeAboveMetric : BenchmarkMetric<int>
	{
		private float _frameTimeThreshold;

		private int _numberOfOccurences;

		public FrameTimeAboveMetric(float frameTimeThreshold)
		{
			_frameTimeThreshold = frameTimeThreshold;
		}

		public override void Cleanup()
		{
			base.Cleanup();
			_numberOfOccurences = 0;
		}

		public override void Update(float unscaledDeltaTime)
		{
			if (unscaledDeltaTime > _frameTimeThreshold)
			{
				_numberOfOccurences++;
			}
		}

		public override void Sample()
		{
			_samples.Add(_numberOfOccurences);
			_numberOfOccurences = 0;
		}
	}
}
