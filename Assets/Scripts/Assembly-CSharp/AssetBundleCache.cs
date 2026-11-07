using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class AssetBundleCache
{
	private enum CachingAction
	{
		Add = 0,
		Remove = 1,
		ClearCache = 2
	}

	private static readonly string ASSET_BUNDLE_CACHE_PATH = Path.Combine(AssetBundleManager.GetAssetBundlesCachePath(), "AssetBundles");

	private const string CACHED_VERSION_INFO_FILE_NAME = "CachedBundlesVersionInfo";

	private const string DEBUG_TAG = "[AssetBundleCache]: ";

	private static Dictionary<string, int> _cachedBundlesVersionInfo;

	private static bool _isPerformingBulkCaching = false;

	public static void Initialize()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(ASSET_BUNDLE_CACHE_PATH);
		if (!directoryInfo.Exists)
		{
			directoryInfo.Create();
		}
		LoadCachedBundlesVersionInfo();
	}

	public static bool CacheAssetBundle(string bundleName, int version, byte[] bytes)
	{
		try
		{
			if (_cachedBundlesVersionInfo.ContainsKey(bundleName))
			{
				int num = _cachedBundlesVersionInfo[bundleName];
				if (num > version)
				{
					return false;
				}
				RemoveFromCache(bundleName, num);
			}
			string path = Path.Combine(ASSET_BUNDLE_CACHE_PATH, bundleName);
			File.WriteAllBytes(path, bytes);
		}
		catch (Exception ex)
		{
			LogError("Could not write file to disk at: " + Path.Combine(ASSET_BUNDLE_CACHE_PATH, bundleName) + " Error writing file to disk: " + ex);
			Flurry.LogEventWithSeveralParameters("Asset Bundle Failed to be Cached", "Asset Bundle Name;Asset Bundle Version", bundleName + ";" + version);
			return false;
		}
		RecordCachingInfo(bundleName, version, CachingAction.Add);
		return true;
	}

	public static bool CacheEditorBundle(string bundleName, int version, string bundlePath)
	{
		return true;
	}

	public static void StartBulkCaching()
	{
		_isPerformingBulkCaching = true;
	}

	public static void EndBulkCaching()
	{
		_isPerformingBulkCaching = false;
		SaveCachedBundlesVersionInfo();
	}

	private static void RecordCachingInfo(string bundleName, int version, CachingAction action)
	{
		switch (action)
		{
		case CachingAction.Add:
			if (!_cachedBundlesVersionInfo.ContainsKey(bundleName))
			{
				_cachedBundlesVersionInfo.Add(bundleName, version);
				break;
			}
			LogError("Recording caching info for a bundle that is already cached. Overwriting info. This might cause verison mismatch.");
			_cachedBundlesVersionInfo[bundleName] = version;
			break;
		case CachingAction.Remove:
			if (!_cachedBundlesVersionInfo.ContainsKey(bundleName))
			{
				LogError("Attempting to record caching info for removing a bundle that is not cached. Aborting.");
			}
			else
			{
				_cachedBundlesVersionInfo.Remove(bundleName);
			}
			break;
		case CachingAction.ClearCache:
			if (_cachedBundlesVersionInfo != null)
			{
				_cachedBundlesVersionInfo.Clear();
			}
			break;
		}
		if (!_isPerformingBulkCaching)
		{
			SaveCachedBundlesVersionInfo();
		}
	}

	public static AssetBundle LoadAssetBundle(string bundleName, int version)
	{
		if (IsAssetBundleCached(bundleName, version))
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(ASSET_BUNDLE_CACHE_PATH, bundleName));
			if (assetBundle == null)
			{
				Flurry.LogEventWithSeveralParameters("Asset Bundle Failed to be loaded from Cache", "Asset Bundle Name;Asset Bundle Version", bundleName + ";" + version);
			}
			return assetBundle;
		}
		LogError("Attemping to load bundle " + bundleName + " but it's not cached. Aborting.");
		return null;
	}

	public static AssetBundleCreateRequest LoadAssetBundleAsync(string bundleName, int version)
	{
		if (IsAssetBundleCached(bundleName, version))
		{
			return AssetBundle.LoadFromFileAsync(Path.Combine(ASSET_BUNDLE_CACHE_PATH, bundleName));
		}
		LogError("Attemping to load bundle " + bundleName + " but it's not cached. Aborting.");
		return null;
	}

	public static int GetVersionForCachedBundle(string bundleName)
	{
		return (!_cachedBundlesVersionInfo.ContainsKey(bundleName)) ? (-1) : _cachedBundlesVersionInfo[bundleName];
	}

	public static bool IsAssetBundleCached(string bundleName, int version)
	{
		if (!_cachedBundlesVersionInfo.ContainsKey(bundleName))
		{
			return false;
		}
		if (!File.Exists(Path.Combine(ASSET_BUNDLE_CACHE_PATH, bundleName)))
		{
			LogError("Asset Bundle file was not found in the cache. Correcting cache info.");
			RecordCachingInfo(bundleName, GetVersionForCachedBundle(bundleName), CachingAction.Remove);
			return false;
		}
		return _cachedBundlesVersionInfo[bundleName] == version;
	}

	public static bool RemoveFromCache(string bundleName, int version)
	{
		if (IsAssetBundleCached(bundleName, version))
		{
			File.Delete(Path.Combine(ASSET_BUNDLE_CACHE_PATH, bundleName));
			RecordCachingInfo(bundleName, version, CachingAction.Remove);
			return true;
		}
		LogError("Attempting to remove bundle " + bundleName + " but it's not cached.");
		return false;
	}

	public static void ClearCache()
	{
		if (Directory.Exists(ASSET_BUNDLE_CACHE_PATH))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(ASSET_BUNDLE_CACHE_PATH);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.Delete();
			}
			RecordCachingInfo(string.Empty, 0, CachingAction.ClearCache);
		}
	}

	private static void LoadCachedBundlesVersionInfo()
	{
		string path = Path.Combine(ASSET_BUNDLE_CACHE_PATH, "CachedBundlesVersionInfo");
		if (!File.Exists(path))
		{
			_cachedBundlesVersionInfo = new Dictionary<string, int>();
			return;
		}
		StreamReader streamReader = File.OpenText(path);
		string serializedData = streamReader.ReadToEnd();
		_cachedBundlesVersionInfo = AssetBundleManager.ParseAssetBundleVersionData(serializedData);
		streamReader.Close();
	}

	private static void SaveCachedBundlesVersionInfo()
	{
		string path = Path.Combine(ASSET_BUNDLE_CACHE_PATH, "CachedBundlesVersionInfo");
		File.WriteAllText(path, AssetBundleManager.SerializeAssetBundleVersionData(_cachedBundlesVersionInfo));
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private static void Log(string msg)
	{
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private static void LogWarning(string msg)
	{
	}

	private static void LogError(string msg)
	{
		SDebug.LogError("[AssetBundleCache]: " + msg);
	}
}
