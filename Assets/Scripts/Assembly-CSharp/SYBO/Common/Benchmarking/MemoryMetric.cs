using System.Collections.Generic;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace SYBO.Common.Benchmarking
{
	public class MemoryMetric : BenchmarkMetric<int>
	{
		public override void Sample()
		{
			_samples.Add(DeviceUtility.GetCurrentMemoryUsage());
		}

		public override void Update(float unscaledDeltaTime)
		{
		}

		public override List<string> Serialize()
		{
			List<string> list = base.Serialize();
			int a = int.MaxValue;
			int a2 = 0;
			for (int i = 0; i < _samples.Count; i++)
			{
				a = Mathf.Min(a, _samples[i]);
				a2 = Mathf.Max(a2, _samples[i]);
			}
			list.Add(a.ToString());
			list.Add(a2.ToString());
			return list;
		}
	}
}
