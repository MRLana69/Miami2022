using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SYBO.Common.Benchmarking
{
	public class FPSMetric : BenchmarkMetric<FPSReportEntry>
	{
		private int _currentFrames;

		private float _totalFrameTime;

		private float _minFrameTime = float.MaxValue;

		private float _maxFrameTime;

		public override void Cleanup()
		{
			base.Cleanup();
			_currentFrames = 0;
			_totalFrameTime = 0f;
			_minFrameTime = float.MaxValue;
			_maxFrameTime = 0f;
		}

		public override void Update(float unscaledDeltaTime)
		{
			_totalFrameTime += unscaledDeltaTime;
			_minFrameTime = Mathf.Min(_minFrameTime, unscaledDeltaTime);
			_maxFrameTime = Mathf.Max(_maxFrameTime, unscaledDeltaTime);
			if (unscaledDeltaTime > _maxFrameTime)
			{
				_maxFrameTime = unscaledDeltaTime;
			}
			_currentFrames++;
		}

		public override void Sample()
		{
			_samples.Add(new FPSReportEntry(1f / (_totalFrameTime / (float)_currentFrames), _minFrameTime, _maxFrameTime));
			_currentFrames = 0;
			_totalFrameTime = 0f;
			_minFrameTime = float.MaxValue;
			_maxFrameTime = 0f;
		}

		public override List<string> Serialize()
		{
			float num = 0f;
			float a = float.MaxValue;
			float a2 = 0f;
			StringBuilder stringBuilder = new StringBuilder();
			int count = _samples.Count;
			for (int i = 0; i < count; i++)
			{
				FPSReportEntry fPSReportEntry = _samples[i];
				num += fPSReportEntry.AvgFPS;
				a = Mathf.Min(a, fPSReportEntry.AvgFPS);
				a2 = Mathf.Max(a2, fPSReportEntry.AvgFPS);
				stringBuilder.Append(fPSReportEntry.AvgFPS.ToString("0.00"));
				if (i < count - 1)
				{
					stringBuilder.Append(Benchmark.SeparatorCharacter);
				}
			}
			stringBuilder.Append(Benchmark.SeparatorCharacter);
			stringBuilder.Append("0");
			stringBuilder.Append(Benchmark.SeparatorCharacter);
			stringBuilder.Append("60");
			List<string> list = new List<string>();
			list.Add(stringBuilder.ToString());
			list.Add((num / (float)_samples.Count).ToString("0.00"));
			list.Add(a.ToString("0.00"));
			list.Add(a2.ToString("0.00"));
			return list;
		}
	}
}
