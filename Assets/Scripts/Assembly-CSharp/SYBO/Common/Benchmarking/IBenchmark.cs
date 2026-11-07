using SYBO.Common.Automation;

namespace SYBO.Common.Benchmarking
{
	public interface IBenchmark : IAutomationStepExecutor
	{
		IBenchmarkMetric[] Metrics { get; }
	}
}
