using System.Collections.Generic;
using System.Diagnostics;
using Adverty;
using Adverty.AdUnit;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using SYBO.Subway;
using UnityEngine;

public static class BillboardAdsManager
{
	private const string ONLINE_SETTINGS_ENABLED_KEY = "billboard_ads_enabled";

	private const bool ONLINE_SETTINGS_ENABLED_DEFAULT_VALUE = false;

	private const int MIN_SUPPORTED_ANDROID_API_VERSION = 21;

	private const int MIN_SUPPORTED_IOS_VERSION = 9;

	private const string ADVERTY_EDITOR_API_KEY = "MGJkODhlMTUtNWUxNC00NzdhLTkyN2ItNTJmM2Q3NzQ0NzgzJGh0dHBzOi8vcHJvZC5hZHNlcnZlci5rOHMuYWR2ZXJ0eS5jb20=";

	private static readonly Dictionary<RuntimePlatform, string> RESTRICTED_ADVERTY_PLATFORM_API_KEYS = new Dictionary<RuntimePlatform, string>
	{
		{
			RuntimePlatform.Android,
			"MGJkODhlMTUtNWUxNC00NzdhLTkyN2ItNTJmM2Q3NzQ0NzgzJGh0dHBzOi8vcHJvZC5hZHNlcnZlci5rOHMuYWR2ZXJ0eS5jb20="
		},
		{
			RuntimePlatform.IPhonePlayer,
			"NzYyY2I4MWQtZGRkNC00MjExLWI1NTgtMTA5ZDYyMzlhMDNhJGh0dHBzOi8vcHJvZC5hZHNlcnZlci5rOHMuYWR2ZXJ0eS5jb20="
		}
	};

	private static Dictionary<RuntimePlatform, string> NON_RESTRICTED_ADVERTY_PLATFORM_API_KEYS = new Dictionary<RuntimePlatform, string>
	{
		{
			RuntimePlatform.Android,
			"NzY4NWY0MTYtMGU4Mi00YWY1LTkzMmItMjRhNzNlNzY0YjllJGh0dHBzOi8vcHJvZC5hZHNlcnZlci5rOHMuYWR2ZXJ0eS5jb20="
		},
		{
			RuntimePlatform.IPhonePlayer,
			"NWNiYzU0NTgtNzQyYi00NGE3LWIxOWMtZWFmZWRlZmQ0ODA2JGh0dHBzOi8vcHJvZC5hZHNlcnZlci5rOHMuYWR2ZXJ0eS5jb20="
		}
	};

	private const AdvertySettings.Mode ADVERTY_EDITOR_PLATFORM_MODE = AdvertySettings.Mode.VR;

	private static bool _hasInitializedAdvertySdk;

	private static int _numberOfBillboardAdUnitsSpawnedInCurrentRun;

	private static int _numberOfBillboardAdsWithFillInCurrentRun;

	private static int _numberOfBillboardAdsShownInCurrentRun;

	public static bool IsBillboardAdsEnabled
	{
		get
		{
			return IsBillboardAdsEnabledInOnlineSettings() && IsPlatformSupported() && SystemInfo.supportsComputeShaders;
		}
	}

	public static void Initialize()
	{
		Log();
		BillboardAdPlacement.OnBillboardAdUnitSpawned += OnBillboardAdUnitSpawned;
		BillboardAdPlacement.OnBillboardAdUnitActivated += OnBillboardAdUnitActivated;
		Game.OnGameStarted += OnGameStarted;
		Game.OnGameEnded += OnGameEnded;
		AdvertyEvents.UnitActivatedEvent += OnUnitActivatedEvent;
		AgeRestriction.restrictionChanged += OnAgeRestrictionChanged;
	}

	private static void OnAgeRestrictionChanged()
	{
		InitializeAdvertySdk();
	}

	public static void InitializeAdvertySdk()
	{
		if (!_hasInitializedAdvertySdk && IsBillboardAdsEnabled)
		{
			SetAdvertySdkPlatform();
			AdvertySettings.APIKey = GetAdvertyApiKey();
			_hasInitializedAdvertySdk = true;
		}
	}

	private static string GetAdvertyApiKey()
	{
		RuntimePlatform platform = Application.platform;
		if (Application.isEditor)
		{
			return "MGJkODhlMTUtNWUxNC00NzdhLTkyN2ItNTJmM2Q3NzQ0NzgzJGh0dHBzOi8vcHJvZC5hZHNlcnZlci5rOHMuYWR2ZXJ0eS5jb20=";
		}
		if (AgeRestriction.isRestricted)
		{
			return RESTRICTED_ADVERTY_PLATFORM_API_KEYS[platform];
		}
		return NON_RESTRICTED_ADVERTY_PLATFORM_API_KEYS[platform];
	}

	private static void SetAdvertySdkPlatform()
	{
		if (Application.isEditor)
		{
			AdvertySettings.Platform = AdvertySettings.Mode.VR;
		}
		else
		{
			AdvertySettings.Platform = AdvertySettings.Mode.Mobile;
		}
	}

	private static void OnBillboardAdUnitSpawned()
	{
		_numberOfBillboardAdUnitsSpawnedInCurrentRun++;
	}

	private static void OnBillboardAdUnitActivated()
	{
		_numberOfBillboardAdsShownInCurrentRun++;
	}

	private static void OnGameStarted()
	{
		_numberOfBillboardAdUnitsSpawnedInCurrentRun = 0;
		_numberOfBillboardAdsShownInCurrentRun = 0;
		_numberOfBillboardAdsWithFillInCurrentRun = 0;
	}

	private static void OnGameEnded()
	{
		if (IsBillboardAdsEnabled)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Number of ad requests", _numberOfBillboardAdUnitsSpawnedInCurrentRun.ToString());
			dictionary.Add("Number of request success", _numberOfBillboardAdsWithFillInCurrentRun.ToString());
			dictionary.Add("Run length", GameStats.Instance.MetersInCurrentGame.ToString());
			dictionary.Add("Number of ads shown", _numberOfBillboardAdsShownInCurrentRun.ToString());
			dictionary.Add("Run duration", Game.Instance.ElapsedGameDuration.ToString());
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("Billboard fill rate", parameters);
		}
	}

	private static void OnUnitActivatedEvent(Unit unit, AdConfiguration adConfiguration)
	{
		_numberOfBillboardAdsWithFillInCurrentRun++;
	}

	private static void Log()
	{
		if (!IsBillboardAdsEnabled)
		{
			LogReasonForBillboardAdsBeingDisabled();
		}
	}

	private static void LogReasonForBillboardAdsBeingDisabled()
	{
		if (IsBillboardAdsEnabledInOnlineSettings() && IsPlatformSupported() && SystemInfo.supportsComputeShaders)
		{
		}
	}

	private static bool IsBillboardAdsEnabledInOnlineSettings()
	{
		return OnlineSettings.instance.GetValue("billboard_ads_enabled", false);
	}

	private static bool IsPlatformSupported()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
		{
			bool result = true;
			string oSVersion = DeviceUtility.GetOSVersion();
			int result2;
			if (int.TryParse(oSVersion.Split('.')[0], out result2) && result2 < 9)
			{
				result = false;
			}
			return result;
		}
		case RuntimePlatform.Android:
			return DeviceUtility.GetAndroidApiLevel() >= 21;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.LinuxEditor:
			return true;
		default:
			return false;
		}
	}

	[Conditional("ENABLE_BILLBOARD_AD_LOGS")]
	public static void Log(object message)
	{
	}
}
