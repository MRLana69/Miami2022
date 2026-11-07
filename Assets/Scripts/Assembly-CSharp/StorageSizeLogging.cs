using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

public static class StorageSizeLogging
{
	private static long _currentSize = 0L;

	private const string DATA_MAIN = "Data";

	private const string CACHE_MAIN = "Cache";

	private const string ASSETBUNDLES_MAIN = "AssetBundles";

	private static Dictionary<string, string> _androidProviderPathsDictionary;

	private static Dictionary<string, string> _iOSProviderPathsDictionary;

	private static string _userDataPath = string.Empty;

	private static string _userExternalDataPath = string.Empty;

	private static string _userCachePath = string.Empty;

	private static string _userTemporaryPath = string.Empty;

	private static string _assetBundlesPath = string.Empty;

	private const string STORAGE_SIZE_LOGGING_ENABLED_KEY = "storage_logging_enabled";

	private const bool DEFAULT_STORAGE_SIZE_LOGGING_SETTING = false;

	private static int _dataSize = 0;

	private static int _cacheSize = 0;

	private static int _assetBundlesSize = 0;

	private static bool _hasFlurryLoggedInSession = false;

	private static List<int> _buckets = new List<int> { 0, 1, 10, 25, 50, 100, 300, 500, 1000, 1500 };

	public static int DataSize
	{
		get
		{
			return _dataSize;
		}
	}

	public static int CacheSize
	{
		get
		{
			return _cacheSize;
		}
	}

	public static int AssetBundlesSize
	{
		get
		{
			return _assetBundlesSize;
		}
	}

	private static bool _isStorageLoggingEnabled
	{
		get
		{
			return OnlineSettings.instance.GetValue("storage_logging_enabled", false);
		}
	}

	public static bool HasFlurryLoggedInSession()
	{
		return _hasFlurryLoggedInSession;
	}

	public static void RegisterStorageUsage()
	{
		if (_isStorageLoggingEnabled)
		{
			InitDevicePaths();
			if (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice)
			{
				_dataSize = LogOverallUsage("Data", new List<string> { _userDataPath, _userExternalDataPath });
				_cacheSize = LogOverallUsage("Cache", new List<string> { _userCachePath, _userTemporaryPath });
				_assetBundlesSize = LogOverallUsage("AssetBundles", new List<string> { _assetBundlesPath });
				LogProviderUsage(_androidProviderPathsDictionary);
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_dataSize = LogOverallUsage("Data", new List<string> { _userDataPath });
				_cacheSize = LogOverallUsage("Cache", new List<string> { _userCachePath, _userTemporaryPath });
				_assetBundlesSize = LogOverallUsage("AssetBundles", new List<string> { _assetBundlesPath });
				LogProviderUsage(_iOSProviderPathsDictionary);
			}
			_hasFlurryLoggedInSession = true;
		}
	}

	private static void InitDevicePaths()
	{
		_userDataPath = DeviceUtility.GetUserDataPath();
		_userCachePath = DeviceUtility.GetCachePath();
		_assetBundlesPath = _userDataPath + "/AssetBundles/";
		if (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice)
		{
			_userExternalDataPath = DeviceUtility.GetAndroidExternalSavePath();
			_userTemporaryPath = Application.temporaryCachePath;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("AdColony3.3.4", _userDataPath + "/adc3/");
			dictionary.Add("Chartboost7.0.1", _userCachePath + "/.chartboost/");
			dictionary.Add("Facebook5.2.1", _userTemporaryPath + "/video-cache/");
			dictionary.Add("Flurry6.5.0", _userTemporaryPath + "/.fcaches/");
			dictionary.Add("InMobi7.0.2", _userCachePath + "/im_cached_content/");
			dictionary.Add("MobVista8.12.0", _userExternalDataPath + "/Movies/.Mintegral_VC/");
			dictionary.Add("SuperSonic6.8.2", _userTemporaryPath + "/supersonicads/");
			dictionary.Add("TapJoy12.0.0", _userDataPath + "/Tapjoy/");
			dictionary.Add("Vungle5.3.2", _userExternalDataPath + "/.vungle/");
			_androidProviderPathsDictionary = dictionary;
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_userTemporaryPath = DeviceUtility.GetUserTempPathiOS();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("AdColony3.3.4", _userCachePath + "/adc3/");
			dictionary.Add("Chartboost7.0.4", _userCachePath + "/CBPrefetchCacheFolder/");
			dictionary.Add("Facebook5.0", _userCachePath + "/audience_network/");
			dictionary.Add("Flurry7.6.4", _userCachePath + "/FlurryFiles/");
			dictionary.Add("InMobi6.0.0", _userCachePath + "/.inmobi_6.0.0/");
			dictionary.Add("MobVista3.8.0", _userCachePath + "/MVVideoCache/");
			dictionary.Add("SuperSonic6.8.1", _userCachePath + "/SSACache_5.40/");
			dictionary.Add("TapJoy11.11.1", _userCachePath + "/TapjoyAssets/");
			dictionary.Add("Vungle6.2.0", _userCachePath + "/vungle/");
			_iOSProviderPathsDictionary = dictionary;
		}
	}

	private static int LogOverallUsage(string generalLocation, List<string> paths)
	{
		_currentSize = 0L;
		foreach (string path in paths)
		{
			VerifyPathAndAddFileSizes(path);
		}
		Flurry.LogEventWithAParameter("Main Storage Sizes", generalLocation, GetAppropiateBucketSize(_currentSize));
		return Convert.ToInt32(_currentSize) / 1000 / 1000;
	}

	private static void LogProviderUsage(Dictionary<string, string> providerDictionary)
	{
		foreach (KeyValuePair<string, string> item in providerDictionary)
		{
			_currentSize = 0L;
			VerifyPathAndAddFileSizes(item.Value);
			Flurry.LogEventWithAParameter("Provider Storage Sizes", item.Key, GetAppropiateBucketSize(_currentSize));
		}
	}

	private static string GetAppropiateBucketSize(long size)
	{
		double mbSize = (float)size / 1000f / 1000f;
		return _buckets.OrderBy((int item) => Math.Abs(mbSize - (double)item)).First().ToString();
	}

	private static void VerifyPathAndAddFileSizes(string directoryPath)
	{
		if (Directory.Exists(directoryPath))
		{
			WalkDirectoryTree(new DirectoryInfo(directoryPath));
		}
	}

	private static void WalkDirectoryTree(DirectoryInfo directoryInfo)
	{
		FileInfo[] array = null;
		DirectoryInfo[] array2 = null;
		try
		{
			array = directoryInfo.GetFiles("*.*");
		}
		catch (UnauthorizedAccessException)
		{
		}
		catch (DirectoryNotFoundException)
		{
		}
		if (array != null)
		{
			FileInfo[] array3 = array;
			foreach (FileInfo fileInfo in array3)
			{
				_currentSize += fileInfo.Length;
			}
			array2 = directoryInfo.GetDirectories();
			DirectoryInfo[] array4 = array2;
			foreach (DirectoryInfo directoryInfo2 in array4)
			{
				WalkDirectoryTree(directoryInfo2);
			}
		}
	}

	[Conditional("ENABLE_PROVIDER_STORAGE_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
