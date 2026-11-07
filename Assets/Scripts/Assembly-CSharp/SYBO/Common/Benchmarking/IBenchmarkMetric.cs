using System.Collections.Generic;

namespace SYBO.Common.Benchmarking
{
	public interface IBenchmarkMetric
	{
		void Initialize(int initialSampleCount);

		void Cleanup();

		void Update(float unscaledDeltaTime);

		void Sample();

		List<string> Serialize();
	}
}
