using System.Text.RegularExpressions;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics
{
	public class DeviceInfoEvent : BaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("platform")]
		[SerializedSizeLimit(36, false)]
		public readonly string DeviceOS;

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
		[SerializedAs("cpu_threads")]
		public readonly int CpuTreads;

		[SerializeField]
		[SerializedAs("cpu_frequency")]
		public readonly int CpuFrequencey;

		[SerializeField]
		[SerializedAs("system_memory")]
		public readonly int SystemMemory;

		public override string EventName
		{
			get
			{
				return "device_info";
			}
		}

		public DeviceInfoEvent()
			: this(true)
		{
		}

		protected DeviceInfoEvent(bool initializeParentValues)
			: base(initializeParentValues)
		{
			DeviceOS = BaseSessionEvent.GetOperatingSystem();
			Device = BaseSessionEvent.GetDeviceModel();
			DeviceID = BaseSessionEvent.GetDeviceID();
			Language = BaseSessionEvent.GetISOThreeLetterLanguageCode();
			ScreenHeight = Screen.height;
			ScreenWidth = Screen.width;
			ScreenPPI = (int)Screen.dpi;
			Gpu = GetTrimmedGPUName();
			CpuTreads = SystemInfo.processorCount;
			CpuFrequencey = SystemInfo.processorFrequency;
			SystemMemory = SystemInfo.systemMemorySize;
		}

		public static string GetDeviceInfoJson()
		{
			DeviceInfoEvent obj = new DeviceInfoEvent(false);
			return JsonApi.Serialize(obj);
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
