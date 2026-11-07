namespace SYBO.Common.Benchmarking
{
	public interface IBenchmarkProcessor
	{
		char SeparatorCharacter { get; }

		void Process(IBenchmark benchmark);
	}
}
