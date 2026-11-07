namespace SYBO.Common.Benchmarking
{
	public interface IMetricSampler
	{
		void Initialize();

		void Cleanup();

		bool Update(float unscaledDeltaTime);
	}
}
