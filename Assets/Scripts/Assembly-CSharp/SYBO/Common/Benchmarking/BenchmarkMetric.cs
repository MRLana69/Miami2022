using System.Collections.Generic;
using System.Text;

namespace SYBO.Common.Benchmarking
{
	public abstract class BenchmarkMetric<T> : IBenchmarkMetric
	{
		protected List<T> _samples;

		public virtual void Initialize(int initialSampleCount)
		{
			_samples = new List<T>(initialSampleCount);
		}

		public virtual void Cleanup()
		{
			_samples.Clear();
		}

		public abstract void Update(float unscaledDeltaTime);

		public abstract void Sample();

		public virtual List<string> Serialize()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = _samples.Count;
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(_samples[i].ToString());
				if (i < count - 1)
				{
					stringBuilder.Append(Benchmark.SeparatorCharacter);
				}
			}
			List<string> list = new List<string>();
			list.Add(stringBuilder.ToString());
			return list;
		}
	}
}
