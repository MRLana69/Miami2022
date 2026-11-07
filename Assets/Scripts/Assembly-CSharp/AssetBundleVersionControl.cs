using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using SYBO.Components.Utilities;
using SYBO.Subway.Info;
using SYBO.Subway.Utilities;
using UnityEngine;

public class AssetBundleVersionControl
{
	private const string VERSION_INFO_SAVE_PATH = "AssetBundleVersion";

	private const string VERSION_INFO_FILE_NAME = "AssetBundleVersionInfo";

	private const string ASSET_BUNDLE_MANIFESTS_RESOURCES_PATH = "AssetBundleManifests/";

	public const string DOWNLOADABLE_BUNDLE_DATA_FILENAME = "DownloadableAssetBundlesManifest";

	public const string SHIPPED_BUNDLE_DATA_FILENAME = "ShippedAssetBundlesManifest";

	private const string DEBUG_TAG = "[AssetBundleVersionControl]: ";

	private const string ONLINE_SETTINGS_BUNDLES_VERSION_KEY = "asset_bundles_version";

	private const string ONLINE_SETTINGS_UPDATED_ASSET_BUNDLES = "updated_bundles_list";

	private const char UPDATED_BUNDLE_SEPARATOR = ';';

	private const char UPDATED_BUNDLE_VALUE_SEPARATOR = ':';

	private const string MANIFESTS_IOS_FOLDER = "ios";

	private const string MANIFESTS_ANDROID_FOLDER = "android";

	private const string MANIFESTS_EDITOR_FOLDER = "editor";

	private const string MANIFESTS_DEAULT_FOLDER = "editor";

	private static int DEFAULT_BUNDLES_VERSION = Globals.DEFAULT_BUNDLES_VERSION;

	private static readonly IJsonSerializer _jsonConverter = new KilooSerializer();

	private bool _initialized;

	private string _versionInfoFilePath = string.Empty;

	private Dictionary<string, int> _availableBundlesData;

	private Dictionary<string, int> _shippedBundlesData;

	private Dictionary<string, int> _downloadableBundlesData;

	private Dictionary<string, int> _neededBundles;

	public Action OnDownloadableAssetBundlesUpdated;

	private static AssetBundleVersionControl _instance;

	public static AssetBundleVersionControl Instance
	{
		get
		{
			return _instance ?? (_instance = new AssetBundleVersionControl());
		}
		private set
		{
		}
	}

	private AssetBundleVersionControl()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (_initialized)
		{
			return;
		}
		_versionInfoFilePath = GetVersionInfoFilePath();
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(AssetBundleManager.GetAssetBundlesCachePath(), "AssetBundleVersion"));
		if (!directoryInfo.Exists)
		{
			directoryInfo.Create();
		}
		_shippedBundlesData = ReadAssetBundleManifest(false);
		_downloadableBundlesData = ReadAssetBundleManifest(true);
		if (HasAppBeenUpdated(UserSave.lastSeenBundleVersion) || IsFreshInstall(UserSave.lastSeenBundleVersion))
		{
			UserSave.latestAvailableAssetBundlesVersion = OnlineSettings.instance.GetValue("asset_bundles_version", DEFAULT_BUNDLES_VERSION);
			SaveManager.Save();
			if (_shippedBundlesData == null || _downloadableBundlesData == null)
			{
				LogError("Unable to read manifests. Aborting initialization.");
				return;
			}
			CreateAvailableBundlesVersionInfo(_shippedBundlesData, _downloadableBundlesData);
		}
		else
		{
			LoadAvailableBundlesData();
		}
		InitializeNeededBundles();
		OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsDownloaded);
		_initialized = true;
	}

	private static string GetVersionInfoFilePath()
	{
		return Path.Combine(Path.Combine(AssetBundleManager.GetAssetBundlesCachePath(), "AssetBundleVersion"), "AssetBundleVersionInfo");
	}

	private bool IsFreshInstall(string lastSeenVersion)
	{
		return string.IsNullOrEmpty(lastSeenVersion);
	}

	private bool HasAppBeenUpdated(string lastSeenVersion)
	{
		string bundleVersion = DeviceUtility.GetBundleVersion();
		return !lastSeenVersion.Equals(bundleVersion);
	}

	private void OnOnlineSettingsDownloaded()
	{
		OnlineSettings instance = OnlineSettings.instance;
		if (!instance.HasValue("asset_bundles_version"))
		{
			return;
		}
		int value = instance.GetValue("asset_bundles_version", UserSave.latestAvailableAssetBundlesVersion);
		if (value > UserSave.latestAvailableAssetBundlesVersion)
		{
			UserSave.latestAvailableAssetBundlesVersion = value;
		}
		if (!instance.HasValue("updated_bundles_list"))
		{
			return;
		}
		Dictionary<string, int> dictionary = ParseUpdatedBundles(instance.GetValue("updated_bundles_list", string.Empty));
		if (dictionary == null || !dictionary.Any())
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			if (_availableBundlesData.ContainsKey(item.Key) && item.Value > _availableBundlesData[item.Key])
			{
				_availableBundlesData[item.Key] = item.Value;
			}
		}
		SaveAvailableBundlesData();
		if (OnDownloadableAssetBundlesUpdated != null)
		{
			OnDownloadableAssetBundlesUpdated();
		}
	}

	private Dictionary<string, int> ParseUpdatedBundles(string serializedUpdatedBundles)
	{
		if (string.IsNullOrEmpty(serializedUpdatedBundles))
		{
			return null;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array = serializedUpdatedBundles.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			if (array3.Length != 2)
			{
				LogError("Updated bundle data formatting is wrong. Aborting bundle data update.");
			}
			else
			{
				dictionary.Add(array3[0], int.Parse(array3[1]));
			}
		}
		return dictionary;
	}

	private void CreateAvailableBundlesVersionInfo(Dictionary<string, int> shippedData, Dictionary<string, int> downloadableData)
	{
		DeleteAvailableBundlesData();
		_availableBundlesData = new Dictionary<string, int>(shippedData);
		foreach (KeyValuePair<string, int> downloadableDatum in downloadableData)
		{
			if (_availableBundlesData.ContainsKey(downloadableDatum.Key))
			{
				LogError("Repeated key " + downloadableDatum.Key);
				if (downloadableDatum.Value > _availableBundlesData[downloadableDatum.Key])
				{
					_availableBundlesData[downloadableDatum.Key] = downloadableDatum.Value;
				}
			}
			else
			{
				_availableBundlesData.Add(downloadableDatum.Key, downloadableDatum.Value);
			}
		}
		SaveAvailableBundlesData();
	}

	private void InitializeNeededBundles()
	{
		if (_neededBundles == null)
		{
			_neededBundles = new Dictionary<string, int>();
		}
		_neededBundles.Clear();
		List<KeyValuePair<Characters.CharacterId, Characters.Model>> availableCharacters = CharacterManager.GetAvailableCharacters();
		if (_neededBundles != null)
		{
			_neededBundles.Clear();
		}
		else
		{
			_neededBundles = new Dictionary<string, int>();
		}
		foreach (KeyValuePair<Characters.CharacterId, Characters.Model> item in availableCharacters)
		{
			AddNeededBundles(_neededBundles, item.Key);
		}
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (activeEventManager == null)
		{
			return;
		}
		switch (activeEventManager.GetEventType())
		{
		case SSEventType.CharacterDay:
		{
			CharacterDayEventManager typeOfActiveEventManager2 = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterDayEventManager>();
			if (typeOfActiveEventManager2 != null)
			{
				bool found = false;
				Characters.CharacterId eventValue = typeOfActiveEventManager2.GetEventValue<Characters.CharacterId>(out found);
				if (found)
				{
					AddNeededBundles(_neededBundles, eventValue);
				}
			}
			break;
		}
		case SSEventType.ExpiredLimitedCharacter:
		{
			ExpiredLimitedCharacterEventManager typeOfActiveEventManager3 = SSEventsManagerController.instance.GetTypeOfActiveEventManager<ExpiredLimitedCharacterEventManager>();
			if (typeOfActiveEventManager3 != null)
			{
				bool found2 = false;
				Characters.CharacterId eventValue2 = typeOfActiveEventManager3.GetEventValue<Characters.CharacterId>(out found2);
				if (found2)
				{
					AddNeededBundles(_neededBundles, eventValue2);
				}
			}
			break;
		}
		case SSEventType.CharacterName:
		{
			CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
			if (typeOfActiveEventManager == null)
			{
				break;
			}
			List<Characters.CharacterId> characterList = typeOfActiveEventManager.GetCharacterList();
			{
				foreach (Characters.CharacterId item2 in characterList)
				{
					AddNeededBundles(_neededBundles, item2);
				}
				break;
			}
		}
		}
	}

	private void AddNeededBundles(Dictionary<string, int> neededBundles, Characters.CharacterId character)
	{
		string[] referencedAssetBundleNames = SingletonMonoBehaviour<Factories>.Instance.GetReferencedAssetBundleNames(character);
		string[] array = referencedAssetBundleNames;
		foreach (string text in array)
		{
			int availableShippedVersionForBundle = GetAvailableShippedVersionForBundle(text);
			if (availableShippedVersionForBundle != -1 && !neededBundles.ContainsKey(text))
			{
				neededBundles.Add(text, availableShippedVersionForBundle);
			}
		}
	}

	public Dictionary<string, int> GetNeededBundles()
	{
		InitializeNeededBundles();
		return _neededBundles;
	}

	public int GetLatestVersionForBundle(string bundleName)
	{
		return (!_availableBundlesData.ContainsKey(bundleName)) ? (-1) : _availableBundlesData[bundleName];
	}

	public int GetAvailableShippedVersionForBundle(string bundleName)
	{
		return (!_shippedBundlesData.ContainsKey(bundleName)) ? (-1) : _shippedBundlesData[bundleName];
	}

	public bool IsBundleVersionShipped(string bundleName, int version)
	{
		return _shippedBundlesData.ContainsKey(bundleName) && _shippedBundlesData[bundleName] == version;
	}

	public int GetCurrentlyAvailableBundleVersion()
	{
		return UserSave.latestAvailableAssetBundlesVersion;
	}

	private Dictionary<string, int> ReadAssetBundleManifest(bool downloadables)
	{
		string filename = ((!downloadables) ? "ShippedAssetBundlesManifest" : "DownloadableAssetBundlesManifest");
		string manifestPath = GetManifestPath(filename);
		if (manifestPath == string.Empty)
		{
			return new Dictionary<string, int>();
		}
		try
		{
			TextAsset textAsset = Resources.Load<TextAsset>(manifestPath);
			if ((bool)textAsset)
			{
				Dictionary<string, object> dictionary = _jsonConverter.FromJson<Dictionary<string, object>>(textAsset.text);
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
				if (dictionary == null)
				{
					LogError("Deserialized JSon is null. Returning empty manifest.");
					return dictionary2;
				}
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					dictionary2.Add(item.Key, Convert.ToInt32(item.Value));
				}
				return dictionary2;
			}
			LogError("Attempt to load " + manifestPath + " from resources failed.");
		}
		catch (Exception ex)
		{
			LogError("Exception caught attempting to load " + manifestPath + ":" + ex);
		}
		LogError("Unable to read " + manifestPath);
		return null;
	}

	private string GetManifestPath(string filename)
	{
		string empty = string.Empty;
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			empty = "android";
			break;
		case RuntimePlatform.IPhonePlayer:
			empty = "ios";
			break;
		case RuntimePlatform.WebGLPlayer:
			empty = "android";
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
			if (DebugSettings.IsAllowed && DebugSettings.UseManifestsForAssetBundlesInEditor)
			{
				empty = "android";
				break;
			}
			return string.Empty;
		default:
			LogError("No manifest folder found for platform: " + Application.platform);
			return string.Empty;
		}
		string path = Path.Combine("AssetBundleManifests/", empty);
		return Path.Combine(path, filename);
	}

	private void LoadAvailableBundlesData()
	{
		if (!File.Exists(_versionInfoFilePath))
		{
			CreateAvailableBundlesVersionInfo(_shippedBundlesData, _downloadableBundlesData);
			return;
		}
		StreamReader streamReader = File.OpenText(_versionInfoFilePath);
		string serializedData = streamReader.ReadToEnd();
		_availableBundlesData = AssetBundleManager.ParseAssetBundleVersionData(serializedData);
		streamReader.Close();
	}

	private void SaveAvailableBundlesData()
	{
		File.WriteAllText(_versionInfoFilePath, AssetBundleManager.SerializeAssetBundleVersionData(_availableBundlesData));
	}

	private void DeleteAvailableBundlesData()
	{
		if (File.Exists(_versionInfoFilePath))
		{
			File.Delete(_versionInfoFilePath);
		}
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private void Log(string msg)
	{
	}

	[Conditional("ASSET_BUNDLES_DEBUG_LOGS")]
	private void LogWarning(string msg)
	{
	}

	private void LogError(string msg)
	{
		SDebug.LogError("[AssetBundleVersionControl]: " + msg);
	}
}
