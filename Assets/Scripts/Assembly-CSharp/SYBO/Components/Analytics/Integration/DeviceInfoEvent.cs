using System.Text.RegularExpressions;
using Kiloo.Common.Plugins.DeviceUtility;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class DeviceInfoEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("platform")]
		[SerializedSizeLimit(36, false)]
		public readonly string DeviceOS;

		[SerializeField]
		[SerializedSizeLimit(20, true)]
		[SerializedAs("platform_config")]
		public readonly string DevicePlatformConfig;

		[SerializeField]
		[SerializedAs("device")]
		[SerializedSizeLimit(72, false)]
		public readonly string Device;

		[SerializeField]
		[SerializedAs("device_id")]
		[SerializedSizeLimit(44, false)]
		public readonly string DeviceID;

		[SerializeField]
		[SerializedAs("os_language")]
		[SerializedSizeLimit(3, false)]
		public readonly string Language;

		[SerializeField]
		[SerializedAs("performance_level")]
		public readonly int PerformanceLevel;

		[SerializeField]
		[SerializedAs("screen_height")]
		public readonly int ScreenHeight;

		[SerializeField]
		[SerializedAs("screen_width")]
		public readonly int ScreenWidth;

		[SerializeField]
		[SerializedAs("screen_ppi")]
		public readonly int ScreenPPI;

		[SerializeField]
		[SerializedSizeLimit(40, false)]
		[SerializedAs("gpu")]
		public readonly string Gpu;

		[SerializeField]
		[SerializedAs("gpu_score")]
		public readonly int GpuScore;

		[SerializeField]
		[SerializedAs("cpu_threads")]
		public readonly int CpuTreads;

		[SerializeField]
		[SerializedAs("cpu_frequency")]
		public readonly int CpuFrequencey;

		[SerializeField]
		[SerializedAs("system_memory")]
		public readonly int SystemMemory;

		[SerializeField]
		[SerializedAs("online_settings_locale")]
		[SerializedSizeLimit(6, false)]
		public readonly string OnlineSettingsLocale;

		public override string EventName
		{
			get
			{
				return "device_info";
			}
		}

		public DeviceInfoEvent()
		{
			DeviceOS = BaseSessionEvent.GetOperatingSystem();
			DevicePlatformConfig = "android";
			Device = BaseSessionEvent.GetDeviceModel();
			DeviceID = BaseSessionEvent.GetDeviceID();
			Language = BaseSessionEvent.GetISOThreeLetterLanguageCode();
			PerformanceLevel = (int)DeviceInfo.Instance.performanceLevel;
			ScreenHeight = Screen.height;
			ScreenWidth = Screen.width;
			ScreenPPI = (int)Screen.dpi;
			Gpu = GetTrimmedGPUName();
			GpuScore = DeviceInfo.Instance.gpuScore;
			CpuTreads = SystemInfo.processorCount;
			CpuFrequencey = SystemInfo.processorFrequency;
			SystemMemory = SystemInfo.systemMemorySize;
			OnlineSettingsLocale = DeviceUtility.GetLocale();
		}

		public DeviceInfo.PerformanceLevel GetPerformanceLevel()
		{
			return (DeviceInfo.PerformanceLevel)PerformanceLevel;
		}

		public static string GetTrimmedGPUName()
		{
			string text = SystemInfo.graphicsDeviceName.ToLower();
			text = text.Replace("(tm)", string.Empty);
			Regex regex = new Regex("[^a-z0-9]");
			return regex.Replace(text, string.Empty);
		}
	}
}
