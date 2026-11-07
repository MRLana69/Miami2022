using SYBO.Common.Automation;
using SYBO.Common.Benchmarking;
using SYBO.Subway.Automation.Samplers;
using SYBO.Subway.Automation.Steps;
using UnityEngine;

namespace SYBO.Subway.Benchmarking
{
	public class SubwayRunningBenchmark : SubwayBenchmark
	{
		private const float DefaultDurationSeconds = 600f;

		private const float DefaultSampleInterval = 0.5f;

		private float _durationSeconds;

		private float _sampleInterval;

		public override bool RequiresConfirmation
		{
			get
			{
				return _durationSeconds > 600f;
			}
		}

		public override string ConfirmationMessage
		{
			get
			{
				return "This benchmark will run for " + Mathf.RoundToInt(_durationSeconds / 60f) + " minutes. Please ensure that the device is connected to a charger during this test.";
			}
		}

		public SubwayRunningBenchmark(float durationSeconds = 600f, float sampleInterval = 0.5f)
			: base(GenerateSteps(durationSeconds), GenerateMetrics(), new IMetricSampler[1]
			{
				new InGameIntervalSampler(sampleInterval)
			})
		{
			_durationSeconds = durationSeconds;
			_sampleInterval = sampleInterval;
			_initialSampleCount = Mathf.RoundToInt(_durationSeconds / _sampleInterval);
		}

		private static IAutomationStep[] GenerateSteps(float runLength)
		{
			return new IAutomationStep[1]
			{
				new StartGameAndWaitStep(runLength)
			};
		}

		private static IBenchmarkMetric[] GenerateMetrics()
		{
			return new IBenchmarkMetric[9]
			{
				new FPSMetric(),
				new DeltaFrameTimeMetric(),
				new FrameTimeAboveMetric(1f / 30f),
				new FrameTimeAboveMetric(1f / 15f),
				new FrameTimeAboveMetric(2f / 15f),
				new FrameTimeAboveMetric(4f / 15f),
				new GCCallsMetric(),
				new InstantiatedObjectsMetric(true),
				new TemperatureMetric()
			};
		}
	}
}
