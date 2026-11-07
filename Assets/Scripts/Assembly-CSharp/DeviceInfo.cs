using System.Diagnostics;
using Data.Save;
using Kiloo.Common.Performance;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

public class DeviceInfo
{
	public enum FormFactor
	{
		iPhone = 0,
		iPad = 1,
		small = 2,
		medium = 3,
		large = 4,
		iPhone5 = 5,
		iPhone6 = 6,
		iPhone6Plus = 7,
		iPhoneUnknown = 8
	}

	public enum PerformanceLevel
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public enum MobileDeviceType
	{
		Tablet = 0,
		Phone = 1
	}

	private const int DEFAULT_ANDROID_PERFORMANCE_LEVEL_THRESHOLD_VALUE = 1000;

	private const int DEFAULT_KINDLE_PERFORMANCE_LEVEL_THRESHOLD_VALUE = 900;

	private static readonly string ShaderBackgroundBlendSrcPropName = "_BackgroundBlendSrcMode";

	private static readonly string ShaderBackgroundBlendDstPropName = "_BackgroundBlendDstMode";

	public PerformanceLevel _performanceLevel;

	public string deviceModel;

	private static DeviceInfo _instance;

	public PerformanceLevel performanceLevel
	{
		get
		{
			return _performanceLevel;
		}
	}

	public float dpi { get; private set; }

	public FormFactor formFactor { get; private set; }

	public bool isHighres { get; private set; }

	public int gpuScore { get; private set; }

	public GPUCategory gpuCategory { get; private set; }

	public static DeviceInfo Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DeviceInfo();
			}
			return _instance;
		}
	}

	private DeviceInfo()
	{
		Shader.SetGlobalInt(ShaderBackgroundBlendSrcPropName, 5);
		Shader.SetGlobalInt(ShaderBackgroundBlendDstPropName, 10);
		deviceModel = SystemInfo.deviceModel;
		isHighres = (float)Screen.height > 480f;
		CategoryThresholds thresholds = default(CategoryThresholds);
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.Android)
		{
			if (DeviceUtility.isKindleDevice)
			{
				thresholds.high = 900f;
			}
			else
			{
				thresholds.high = 1000f;
			}
		}
		else
		{
			thresholds.high = 2.1474836E+09f;
		}
		AndroidGPUCategorizer.Initialize(thresholds, LogUnknownGPU);
		gpuScore = AndroidGPUCategorizer.gpuScore;
		gpuCategory = AndroidGPUCategorizer.gpuCategory;
		if (Screen.height < 500)
		{
			formFactor = FormFactor.small;
		}
		else if (Screen.height < 900)
		{
			formFactor = FormFactor.medium;
		}
		else
		{
			formFactor = FormFactor.large;
		}
		if (isTabletFormFactor())
		{
			formFactor = FormFactor.iPad;
		}
		if (Screen.height >= 500 && Screen.width > 320)
		{
			isHighres = true;
		}
		else
		{
			isHighres = false;
		}
		dpi = Screen.dpi;
		if (dpi <= 0f)
		{
			dpi = 300f;
		}
		GPUCategory gPUCategory = gpuCategory;
		if (gPUCategory == GPUCategory.High)
		{
			_performanceLevel = PerformanceLevel.High;
		}
		else
		{
			_performanceLevel = PerformanceLevel.Low;
		}
		if (DeviceUtility.isKindleDevice && deviceModel == "Amazon Kindle Fire")
		{
			Shader.SetGlobalInt(ShaderBackgroundBlendSrcPropName, 1);
			Shader.SetGlobalInt(ShaderBackgroundBlendDstPropName, 0);
		}
		if (Globals.DEBUG && -1 < DebugSave.overridePerformanceLevel)
		{
			_performanceLevel = (PerformanceLevel)DebugSave.overridePerformanceLevel;
		}
		// No navegador quem escolhe é a opção "Qualidade Gráfica" das Configurações
		if (platform == RuntimePlatform.WebGLPlayer)
		{
			_performanceLevel = TrainingOptions.GraphicsHigh ? PerformanceLevel.High : PerformanceLevel.Low;
			TrainingOptions.ApplyGraphics();
		}
	}

	private bool isTabletFormFactor()
	{
		float f = ((!(Screen.dpi > 0f)) ? ((float)Screen.width) : ((float)Screen.width / Screen.dpi));
		float f2 = ((!(Screen.dpi > 0f)) ? ((float)Screen.height) : ((float)Screen.height / Screen.dpi));
		double num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
		return num >= 6.0;
	}

	public bool IsTablet()
	{
		return DeviceUtility.IsTablet();
	}

	public MobileDeviceType GetMobileDeviceType()
	{
		if (IsTablet())
		{
			return MobileDeviceType.Tablet;
		}
		return MobileDeviceType.Phone;
	}

	private void SetDeviceInfoForUnknownIphone()
	{
		if ((float)Screen.height > 960f)
		{
			_performanceLevel = PerformanceLevel.High;
		}
		else
		{
			_performanceLevel = PerformanceLevel.Low;
		}
		formFactor = FormFactor.iPhoneUnknown;
		isHighres = (float)Screen.height > 480f;
		dpi = 300f;
	}

	private void SetDeviceInfoForUnknownIpad()
	{
		if ((float)Screen.height > 1024f)
		{
			_performanceLevel = PerformanceLevel.High;
		}
		else
		{
			_performanceLevel = PerformanceLevel.Low;
		}
		formFactor = FormFactor.iPad;
		isHighres = (float)Screen.height > 480f;
		dpi = 264f;
	}

	private void LogUnknownGPU(string gpuName)
	{
		if (AndroidGPUCategorizer.IsSupportedOnPlatform())
		{
			Flurry.LogEventWithAParameter("Unknown GPU", "GPU name", gpuName);
		}
	}

	public bool IsDeviceCapablePerformance(PerformanceLevel perfLevel)
	{
		return perfLevel <= _performanceLevel;
	}

	private static void LogError(string msg, Object context = null)
	{
		SDebug.LogError("DeviceInfo: " + msg, context);
	}

	private static void LogWarning(string msg, Object context = null)
	{
	}

	[Conditional("DEVICE_INFO_DEBUG_LOGS")]
	private static void Log(string msg, Object context = null)
	{
	}
}
