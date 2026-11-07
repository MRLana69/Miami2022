using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace SYBO.Common.Benchmarking
{
	public class TemperatureMetric : BenchmarkMetric<float>
	{
		public class TemperatureStats
		{
			public float minTemperature;

			public float maxTemperature;

			public float totalSumOfAllTemperatures { get; private set; }

			public StringBuilder temperaturesArrayString { get; private set; }

			public TemperatureStats()
			{
				minTemperature = float.MaxValue;
				maxTemperature = float.MinValue;
				temperaturesArrayString = new StringBuilder();
			}

			public void AddTemperature(float temperature)
			{
				totalSumOfAllTemperatures += temperature;
				temperaturesArrayString.Append(temperature.ToString("0.00"));
			}

			public float CalculateAverageTemperature(int temperatureCount)
			{
				return totalSumOfAllTemperatures / (float)temperatureCount;
			}
		}

		private static readonly RuntimePlatform[] SUPPORTED_PLATFORMS = new RuntimePlatform[1] { RuntimePlatform.Android };

		public override void Update(float unscaledDeltaTime)
		{
		}

		public override void Sample()
		{
			if (isSupported())
			{
				_samples.Add(DeviceUtility.GetBatteryTemperature());
			}
		}

		public override List<string> Serialize()
		{
			List<string> list;
			if (!isSupported())
			{
				list = new List<string>();
				list.Add("not supported");
				return list;
			}
			TemperatureStats temperatureStats = GetTemperatureStats();
			StringBuilder temperaturesArrayString = temperatureStats.temperaturesArrayString;
			temperaturesArrayString.Append(Benchmark.SeparatorCharacter);
			temperaturesArrayString.Append("0");
			temperaturesArrayString.Append(Benchmark.SeparatorCharacter);
			temperaturesArrayString.Append("90");
			list = new List<string>();
			list.Add(temperaturesArrayString.ToString());
			list.Add(temperatureStats.CalculateAverageTemperature(_samples.Count).ToString("0.00"));
			list.Add(temperatureStats.minTemperature.ToString("0.00"));
			list.Add(temperatureStats.maxTemperature.ToString("0.00"));
			return list;
		}

		private bool isSupported()
		{
			return !Globals.DEBUG_TEMPERATURE_METRIC_DISABLED && SUPPORTED_PLATFORMS.Contains(Application.platform);
		}

		private TemperatureStats GetTemperatureStats()
		{
			TemperatureStats temperatureStats = new TemperatureStats();
			int count = _samples.Count;
			for (int i = 0; i < count; i++)
			{
				float num = _samples[i];
				temperatureStats.minTemperature = Mathf.Min(temperatureStats.minTemperature, num);
				temperatureStats.maxTemperature = Mathf.Max(temperatureStats.maxTemperature, num);
				temperatureStats.AddTemperature(num);
				if (i < count - 1)
				{
					temperatureStats.temperaturesArrayString.Append(Benchmark.SeparatorCharacter);
				}
			}
			return temperatureStats;
		}
	}
}
