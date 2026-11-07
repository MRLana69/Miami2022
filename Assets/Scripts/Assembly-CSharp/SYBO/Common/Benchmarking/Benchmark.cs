using SYBO.Common.Automation;

namespace SYBO.Common.Benchmarking
{
	public abstract class Benchmark : AutomationStepExecutor, IBenchmark, IAutomationStepExecutor
	{
		private IBenchmarkMetric[] _metrics;

		private IMetricSampler[] _samplers;

		public IBenchmarkMetric[] Metrics
		{
			get
			{
				return _metrics;
			}
		}

		protected virtual int InitialSampleCount
		{
			get
			{
				return 0;
			}
		}

		public static IBenchmarkProcessor Processor { get; set; }

		public static char SeparatorCharacter
		{
			get
			{
				return (Processor == null) ? ' ' : Processor.SeparatorCharacter;
			}
		}

		public Benchmark(IAutomationStep[] steps, IBenchmarkMetric[] metrics, IMetricSampler[] samplers)
			: base(steps)
		{
			_metrics = metrics;
			_samplers = samplers;
		}

		public override void Initialize()
		{
			base.Initialize();
			InitializeBenchmark(InitialSampleCount);
		}

		private void InitializeBenchmark(int initialSampleCount)
		{
			if (Processor == null)
			{
				Processor = CreateProcessor();
			}
			for (int i = 0; i < _metrics.Length; i++)
			{
				_metrics[i].Initialize(initialSampleCount);
			}
			for (int j = 0; j < _samplers.Length; j++)
			{
				_samplers[j].Initialize();
			}
		}

		public override void Update(float unscaledDeltaTime)
		{
			if (base.IsDone)
			{
				return;
			}
			base.Update(unscaledDeltaTime);
			for (int i = 0; i < _metrics.Length; i++)
			{
				_metrics[i].Update(unscaledDeltaTime);
			}
			bool flag = false;
			for (int j = 0; j < _samplers.Length; j++)
			{
				flag |= _samplers[j].Update(unscaledDeltaTime);
			}
			if (flag)
			{
				for (int k = 0; k < _metrics.Length; k++)
				{
					_metrics[k].Sample();
				}
			}
		}

		public override void Terminate()
		{
			if (_metrics.Length > 0 && _samplers.Length > 0 && Processor != null)
			{
				Processor.Process(this);
			}
			Processor = null;
			for (int i = 0; i < _metrics.Length; i++)
			{
				_metrics[i].Cleanup();
			}
			for (int j = 0; j < _samplers.Length; j++)
			{
				_samplers[j].Cleanup();
			}
			base.Terminate();
		}

		protected abstract IBenchmarkProcessor CreateProcessor();
	}
}
