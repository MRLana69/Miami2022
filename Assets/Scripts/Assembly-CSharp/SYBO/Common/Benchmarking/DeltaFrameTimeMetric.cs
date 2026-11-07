using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SYBO.Common.Benchmarking
{
	public class DeltaFrameTimeMetric : BenchmarkMetric<float>
	{
		private float _minFrameTime = float.MaxValue;

		private float _maxFrameTime;

		public override void Cleanup()
		{
			base.Cleanup();
			_minFrameTime = float.MaxValue;
			_maxFrameTime = 0f;
		}

		public override void Update(float unscaledDeltaTime)
		{
			_minFrameTime = Mathf.Min(_minFrameTime, unscaledDeltaTime);
			_maxFrameTime = Mathf.Max(_maxFrameTime, unscaledDeltaTime);
		}

		public override void Sample()
		{
			_samples.Add(_maxFrameTime - _minFrameTime);
			_minFrameTime = float.MaxValue;
			_maxFrameTime = 0f;
		}

		public override List<string> Serialize()
		{
			List<string> list = new List<string>();
			float num = float.MaxValue;
			float num2 = 0f;
			StringBuilder stringBuilder = new StringBuilder();
			int count = _samples.Count;
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append((_samples[i] * 1000f).ToString("0.0"));
				if (i < count - 1)
				{
					stringBuilder.Append(Benchmark.SeparatorCharacter);
				}
				num = Mathf.Min(num, _samples[i]);
				num2 = Mathf.Max(num2, _samples[i]);
			}
			stringBuilder.Append(Benchmark.SeparatorCharacter);
			stringBuilder.Append('0');
			list.Add(stringBuilder.ToString());
			list.Add((num * 1000f).ToString("0.0"));
			list.Add((num2 * 1000f).ToString("0.0"));
			return list;
		}
	}
}
