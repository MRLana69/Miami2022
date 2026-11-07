using SYBO.Common.Benchmarking;

namespace SYBO.Common.Automation.Samplers
{
	public class IntervalSampler : IMetricSampler
	{
		private float _sampleInterval;

		protected float _timeSinceLastSample;

		public IntervalSampler(float sampleInterval)
		{
			_sampleInterval = sampleInterval;
		}

		public void Initialize()
		{
		}

		public void Cleanup()
		{
			_timeSinceLastSample = 0f;
		}

		public virtual bool Update(float unscaledDeltaTime)
		{
			_timeSinceLastSample += unscaledDeltaTime;
			if (_timeSinceLastSample >= _sampleInterval)
			{
				_timeSinceLastSample -= _sampleInterval;
				return true;
			}
			return false;
		}
	}
}
