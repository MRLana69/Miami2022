using System;

namespace SYBO.Common.Benchmarking
{
	public class GCCallsMetric : BenchmarkMetric<int>
	{
		private int _previousGCCalls;

		public override void Initialize(int initialSampleCount)
		{
			base.Initialize(initialSampleCount);
			_previousGCCalls = GC.CollectionCount(0);
		}

		public override void Update(float unscaledDeltaTime)
		{
		}

		public override void Sample()
		{
			int num = GC.CollectionCount(0);
			int item = num - _previousGCCalls;
			_previousGCCalls = num;
			_samples.Add(item);
		}
	}
}
