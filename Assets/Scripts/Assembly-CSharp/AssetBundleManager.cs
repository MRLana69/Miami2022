using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.HTTP;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Info;
using UI.Screens.CharacterScreenInternal;
using UnityEngine;

public class AssetBundleManager
{
	[CompilerGenerated]
	private sealed class _003CCacheNeededAssetBundles_003Ec__Iterator2 : IEnumerator, IDisposable, IEnumerator<float>
	{
		internal Dictionary<string, int> _003CneededBundles_003E__0;

		internal int _003CnShippedBundles_003E__0;

		internal int _003CnBundlesProcessed_003E__0;

		internal Dictionary<string, int>.KeyCollection.Enumerator _0024locvar0;

		internal string _003CbundleName_003E__1;

		internal int _003CshippedVersion_003E__2;

		internal float _003Cpercentage_003E__2;

		internal AssetBundleManager _0024this;

		internal float _0024current;

		internal bool _0024disposing;

		internal int _0024PC;

		float IEnumerator<float>.Current
		{
			[DebuggerHidden]
			get
			{
				return System_002ECollections_002EGeneric_002EIEnumerator_003Cfloat_003E_002Eget_Current();
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		[DebuggerHidden]
		public _003CCacheNeededAssetBundles_003Ec__Iterator2()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			bool flag = false;
			switch (num)
			{
			case 0u:
				_003CneededBundles_003E__0 = _0024this._versionControl.GetNeededBundles();
				_003CnShippedBundles_003E__0 = _003CneededBundles_003E__0.Count;
				_003CnBundlesProcessed_003E__0 = 0;
				_0024this.succesfullyPrecached = 0;
				_0024this.attemptingToPrecacheAmount = 0;
				AssetBundleCache.StartBulkCaching();
				_0024locvar0 = _003CneededBundles_003E__0.Keys.GetEnumerator();
				num = 4294967293u;
				goto case 1u;
			case 1u:
				try
				{
					switch (num)
					{
					default:
						if (!_0024locvar0.MoveNext())
						{
							break;
						}
						_003CbundleName_003E__1 = _0024locvar0.Current;
						_003CshippedVersion_003E__2 = _0024this._versionControl.GetAvailableShippedVersionForBundle(_003CbundleName_003E__1);
						if (_003CshippedVersion_003E__2 >= 0)
						{
							if (!AssetBundleCache.IsAssetBundleCached(_003CbundleName_003E__1, _003CneededBundles_003E__0[_003CbundleName_003E__1]))
							{
								_0024this.attemptingToPrecacheAmount++;
								if (_0024this.CacheAssetBundle(_003CbundleName_003E__1, _0024this._versionControl.GetAvailableShippedVersionForBundle(_003CbundleName_003E__1)))
								{
									_0024this.succesfullyPrecached++;
								}
							}
						}
						else if (!AssetBundleCache.IsAssetBundleCached(_003CbundleName_003E__1, AssetBundleVersionControl.Instance.GetLatestVersionForBundle(_003CbundleName_003E__1)))
						{
							_0024this.AddAssetBundleDownloadRequest(_003CbundleName_003E__1, _0024this._versionControl.GetLatestVersionForBundle(_003CbundleName_003E__1));
						}
						_003CnBundlesProcessed_003E__0++;
						_003Cpercentage_003E__2 = (float)_003CnBundlesProcessed_003E__0 / (float)_003CnShippedBundles_003E__0;
						_0024current = _003Cpercentage_003E__2;
						if (!_0024disposing)
						{
							_0024PC = 1;
						}
						flag = true;
						goto IL_020f;
					}
				}
				finally
				{
					if (!flag)
					{
						((IDisposable)_0024locvar0/*cast due to constrained. prefix*/).Dispose();
					}
				}
				AssetBundleCache.EndBulkCaching();
				_0024PC = -1;
				goto default;
			default:
				{
					return false;
				}
				IL_020f:
				return true;
			}
		}

		[DebuggerHidden]
		private float System_002ECollections_002EGeneric_002EIEnumerator_003Cfloat_003E_002Eget_Current()
		{
			return _0024current;
		}

		[DebuggerHidden]
		public void Dispose()
		{
			uint num = (uint)_0024PC;
			_0024disposing = true;
			_0024PC = -1;
			switch (num)
			{
			case 1u:
				try
				{
					break;
				}
				finally
				{
					((IDisposable)_0024locvar0/*cast due to constrained. prefix*/).Dispose();
				}
			case 0u:
				break;
			}
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	public const string COMPRESSION_SIGNATURE = "subway_bundles";

	private const string ASSET_BUNDLE_RESOURCES_PATH = "AssetBundles";

	private const string ASSET_BUNDLES_ROOT_PATH = "..\\..\\AssetBundles";

	private const string EDITOR_ASSET_BUNDLE_CACHE_PATH = "..\\EditorCache";

	private const string UNCOMPRESSED_ASSET_BUNDLE_ANDROID_FOLDER = "android";

	private const string UNCOMPRESSED_ASSET_BUNDLE_IOS_FOLDER = "ios";

	private const string UNCOMPRESSED_ASSET_BUNDLE_STANDALONE_FOLDER = "standalone";

	private const string TEMPORARY_EDITOR_ASSET_BUNDLE_FOLDER = "Temp";

	private const string DEBUG_TAG = "[AssetBundleManager]: ";

	private const char VALUE_SEPARATOR = '-';

	private const char ITEM_SEPARATOR = ',';

	private const string ASSET_BUNDLE_SERVER_URL_OS_KEY = "asset_bundle_server";

	private const string ASSET_BUNDLE_SERVER_IOS_FOLDER_OS_KEY = "asset_bundle_server_ios_folder";

	private const string ASSET_BUNDLE_SERVER_ANDROID_FOLDER_OS_KEY = "asset_bundle_server_android_folder";

	private const string ASSET_BUNDLE_REQUEST_TIMEOUT_OS_KEY = "asset_bundle_request_timeout";

	private const string ASSET_BUNDLE_REQUEST_MAX_BACKOFF_TIME_OS_KEY = "asset_bundle_request_max_backoff_time";

	private const string ASSET_BUNDLE_DOWNLOAD_REST_TIME_OS_KEY = "asset_bundle_download_rest_time";

	private const string ASSET_BUNDLE_IS_USING_EXTENSION_OS_KEY = "asset_bundle_using_extension";

	private const string ASSET_BUNDLE_EXTENSION_OS_KEY = "asset_bundle_extension";

	private const int SIMULTANEOUS_CHARACTER_BUNDLE_REQUESTS_LOW = 3;

	private const int SIMULTANEOUS_CHARACTER_BUNDLE_REQUESTS_HIGH = 6;

	private static string _assetBundleBaseServerUrl = "http://subway.azureedge.net/subway/";

	private static string _serverDirIos = "ios";

	private static string _serverDirAndroid = "android";

	private static float _requestDownloadTimeout = 30f;

	private static float _requestMaxBackoffTime = 300f;

	private static float _downloadAttemptRestTime = 10f;

	private static bool _usingBundleExtension = true;

	private static string _assetBundleExtension = ".bytes";

	private AssetBundleVersionControl _versionControl;

	private List<string> _assetBundlesLoading = new List<string>();

	private Dictionary<string, AssetBundle> _loadedAssetBundles = new Dictionary<string, AssetBundle>();

	private Dictionary<string, bool> _pendingUnloads = new Dictionary<string, bool>();

	private List<KeyValuePair<string, Action<AssetBundle>>> _pendingRequests = new List<KeyValuePair<string, Action<AssetBundle>>>();

	private List<CharacterModelRequest.CharacterBundleRequest> _pendingCharacterBundleRequests = new List<CharacterModelRequest.CharacterBundleRequest>();

	private List<CharacterModelRequest.CharacterBundleRequest> _activeCharacterBundleRequests = new List<CharacterModelRequest.CharacterBundleRequest>();

	private int _simultaneousCharacterBundleRequests = 3;

	private Dictionary<string, int> _pendingDownloads;

	private bool _isDownloading;

	private float _lastInitializedDownloads = 0f - _downloadAttemptRestTime;

	private bool _downloadableBundlesUpdated;

	private IEnumerator _currentDownloaderCoroutine;

	private static AssetBundleManager _instance;

	private bool _delayedAvailabilityCheck;

	private int attemptingToPrecacheAmount;

	private int succesfullyPrecached;

	private int attemptingToDownloadAmount;

	private int succesfullyDownloaded;

	public static AssetBundleManager Instance
	{
		get
		{
			return _instance ?? (_instance = new AssetBundleManager());
		}
		private set
		{
		}
	}

	private AssetBundleManager()
	{
		AssetBundleCache.Initialize();
		_versionControl = AssetBundleVersionControl.Instance;
		AssetBundleVersionControl versionControl = _versionControl;
		versionControl.OnDownloadableAssetBundlesUpdated = (Action)Delegate.Combine(versionControl.OnDownloadableAssetBundlesUpdated, new Action(OnDownloadableAssetBundlesUpdated));
		Game.OnGameStarted += StopAssetbundlesDownload;
		Game.OnMenuScreenShown += ResumeAssetBundlesDownload;
		SSEventsManagerController.OnSSEventChangedHandler += OnEventChanged;
		Game.OnMenuScreenShown += DelayedAvailabilityCheck;
		InitializeDownloadRequests();
		if (DeviceInfo.Instance.performanceLevel > DeviceInfo.PerformanceLevel.Low)
		{
			_simultaneousCharacterBundleRequests = 6;
		}
		CoroutineUtil.StartCoroutine(ProcessCharacterBundleRequests());
	}

	private string GetAssetBundlesServerURL(string bundleName)
	{
		OnlineSettings instance = OnlineSettings.instance;
		string text = DeviceUtility.GetBundleVersion() + "/v" + _versionControl.GetCurrentlyAvailableBundleVersion() + "/";
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			text += instance.GetValue("asset_bundle_server_android_folder", _serverDirAndroid);
			break;
		case RuntimePlatform.IPhonePlayer:
			text += instance.GetValue("asset_bundle_server_ios_folder", _serverDirIos);
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
			text += _serverDirIos;
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			LogError("No Asset Bundle directory found for platform: " + Application.platform);
		}
		string path = Path.Combine(instance.GetValue("asset_bundle_server", _assetBundleBaseServerUrl), text);
		return Path.Combine(path, bundleName.ToLower() + ((!instance.GetValue("asset_bundle_using_extension", _usingBundleExtension)) ? string.Empty : instance.GetValue("asset_bundle_extension", _assetBundleExtension)));
	}

	private string GetEditorBundlesPath(string bundleName)
	{
		string path = "Temp";
		string path2 = Path.Combine("..\\..\\AssetBundles".RelativeToAbsolutePath(), path);
		return Path.Combine(path2, bundleName);
	}

	public static string GetAssetBundlesCachePath()
	{
		string text = DeviceUtility.GetUserDataPath();
		if (Application.isEditor)
		{
			text = GetAbsolutePath("..\\EditorCache");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
		}
		return text;
	}

	private static string GetAbsolutePath(string path)
	{
		string text = Application.dataPath;
		List<string> list = path.Split('\\').ToList();
		for (int i = 0; i < list.Count((string s) => s == ".."); i++)
		{
			text = Directory.GetParent(text).FullName;
		}
		list.RemoveAll((string s) => s == "..");
		for (int num = 0; num < list.Count; num++)
		{
			text = Path.Combine(text, list[num]);
		}
		return text;
	}

	public void GetAssetBundle(string bundleName, Action<AssetBundle> requestResultHandler, bool asynchronous = false)
	{
		AssetBundle value = null;
		if (_loadedAssetBundles.TryGetValue(bundleName, out value))
		{
			requestResultHandler(value);
			return;
		}
		if (!_assetBundlesLoading.Contains(bundleName))
		{
			_assetBundlesLoading.Add(bundleName);
			try
			{
				RequestAssetBundle(bundleName, requestResultHandler, asynchronous);
				return;
			}
			catch (Exception ex)
			{
				LogError("Exception caught while trying to retrieve an AssetBundle: " + bundleName + "Error: " + ex);
				return;
			}
		}
		KeyValuePair<string, Action<AssetBundle>> item = new KeyValuePair<string, Action<AssetBundle>>(bundleName, requestResultHandler);
		_pendingRequests.Add(item);
	}

	private void RequestAssetBundle(string bundleName, Action<AssetBundle> requestResultHandler, bool asynchronous = false)
	{
		AssetBundle assetBundle = null;
		int latestVersionForBundle = _versionControl.GetLatestVersionForBundle(bundleName);
		int versionForCachedBundle = AssetBundleCache.GetVersionForCachedBundle(bundleName);
		if (latestVersionForBundle < 0)
		{
			LogError("No version information exists for bundle :" + bundleName);
		}
		else if (latestVersionForBundle > versionForCachedBundle && !_versionControl.IsBundleVersionShipped(bundleName, latestVersionForBundle))
		{
			AddAssetBundleDownloadRequest(bundleName, latestVersionForBundle);
		}
		if (versionForCachedBundle < 0)
		{
			int availableShippedVersionForBundle = _versionControl.GetAvailableShippedVersionForBundle(bundleName);
			if (availableShippedVersionForBundle >= 0)
			{
				CacheAssetBundle(bundleName, availableShippedVersionForBundle);
				versionForCachedBundle = AssetBundleCache.GetVersionForCachedBundle(bundleName);
			}
		}
		if (versionForCachedBundle >= 0)
		{
			if (asynchronous)
			{
				CoroutineUtil.StartCoroutine(LoadAssetBundleAsync(bundleName, versionForCachedBundle, requestResultHandler));
				return;
			}
			assetBundle = AssetBundleCache.LoadAssetBundle(bundleName, versionForCachedBundle);
			OnRequestFinished(bundleName, assetBundle, requestResultHandler);
		}
		else
		{
			LogError("No cached version available for bundle: " + bundleName);
			OnRequestFinished(bundleName, assetBundle, requestResultHandler);
		}
	}

	private IEnumerator LoadAssetBundleAsync(string bundleName, int cachedVersion, Action<AssetBundle> requestResultHandler)
	{
		if (Globals.DEBUG_ENABLE_LOADING_DELAY)
		{
			yield return new WaitForSeconds(0.5f);
		}
		AssetBundleCreateRequest request = AssetBundleCache.LoadAssetBundleAsync(bundleName, cachedVersion);
		yield return request;
		OnRequestFinished(bundleName, request.assetBundle, requestResultHandler);
	}

	// Os pacotes vieram do Unity 2018 (Android): no navegador troca o shader de cada material pelo do projeto
	// com o mesmo nome (os shaders do projeto têm o código GLSL original do build WebGL)
	private static void GjRemapShaders(AssetBundle bundle)
	{
		if (Application.platform != RuntimePlatform.WebGLPlayer)
		{
			return;
		}
		foreach (Material material in bundle.LoadAllAssets<Material>())
		{
			if (material == null || material.shader == null)
			{
				continue;
			}
			Shader shader = Shader.Find(material.shader.name);
			if (shader != null && shader != material.shader)
			{
				int queue = material.renderQueue;
				material.shader = shader;
				material.renderQueue = queue;
			}
		}
	}

	private void OnRequestFinished(string bundleName, AssetBundle assetBundle, Action<AssetBundle> requestResultHandler)
	{
		if (assetBundle != null)
		{
			GjRemapShaders(assetBundle);
			if (_loadedAssetBundles.ContainsKey(bundleName))
			{
				if (_loadedAssetBundles[bundleName] != null)
				{
				}
				_loadedAssetBundles[bundleName] = assetBundle;
			}
			else
			{
				_loadedAssetBundles.Add(bundleName, assetBundle);
			}
		}
		else
		{
			LogError("AssetBundle request finished unsuccessfully for bundle: " + bundleName);
		}
		if (_assetBundlesLoading.Contains(bundleName))
		{
			_assetBundlesLoading.Remove(bundleName);
		}
		requestResultHandler(assetBundle);
		List<KeyValuePair<string, Action<AssetBundle>>> list = new List<KeyValuePair<string, Action<AssetBundle>>>(_pendingRequests);
		_pendingRequests.RemoveAll((KeyValuePair<string, Action<AssetBundle>> entry) => entry.Key == bundleName);
		foreach (KeyValuePair<string, Action<AssetBundle>> item in list)
		{
			if (item.Key == bundleName)
			{
				item.Value(assetBundle);
			}
		}
	}

	private IEnumerator ProcessCharacterBundleRequests()
	{
		while (true)
		{
			if (_pendingCharacterBundleRequests.Count > 0)
			{
				_pendingCharacterBundleRequests.Sort((CharacterModelRequest.CharacterBundleRequest r1, CharacterModelRequest.CharacterBundleRequest r2) => r1.GetPriority().CompareTo(r2.GetPriority()));
				CharacterModelRequest.CharacterBundleRequest characterBundleRequest = _pendingCharacterBundleRequests.First();
				if (_activeCharacterBundleRequests.Count < _simultaneousCharacterBundleRequests)
				{
					_activeCharacterBundleRequests.Add(characterBundleRequest);
					_pendingCharacterBundleRequests.Remove(characterBundleRequest);
					string[] assetBundleNames = characterBundleRequest.AssetBundleNames;
					foreach (string bundleName in assetBundleNames)
					{
						GetAssetBundle(bundleName, characterBundleRequest.OnBundleLoaded, true);
					}
				}
			}
			yield return null;
		}
	}

	public void EnqueueAsyncCharacterBundleRequest(Characters.CharacterId characterId, int theme, Action onBundlesLoaded, CharacterModelRequest modelRequest)
	{
		foreach (CharacterModelRequest.CharacterBundleRequest pendingCharacterBundleRequest in _pendingCharacterBundleRequests)
		{
			if (pendingCharacterBundleRequest.characterId == characterId)
			{
				pendingCharacterBundleRequest.RegisterModelRequest(modelRequest);
				return;
			}
		}
		foreach (CharacterModelRequest.CharacterBundleRequest activeCharacterBundleRequest in _activeCharacterBundleRequests)
		{
			if (activeCharacterBundleRequest.characterId == characterId)
			{
				activeCharacterBundleRequest.RegisterModelRequest(modelRequest);
				return;
			}
		}
		CharacterModelRequest.CharacterBundleRequest characterBundleRequest = new CharacterModelRequest.CharacterBundleRequest(characterId, theme);
		characterBundleRequest.RegisterModelRequest(modelRequest);
		_pendingCharacterBundleRequests.Add(characterBundleRequest);
	}

	public void RemoveAsyncCharacterBundleRequest(CharacterModelRequest modelRequest)
	{
		CharacterModelRequest.CharacterBundleRequest characterBundleRequest = _pendingCharacterBundleRequests.Find((CharacterModelRequest.CharacterBundleRequest r) => r.characterId == modelRequest.characterId);
		if (characterBundleRequest != null && characterBundleRequest.RemoveModelRequest(modelRequest))
		{
			_pendingCharacterBundleRequests.Remove(characterBundleRequest);
			return;
		}
		characterBundleRequest = _activeCharacterBundleRequests.Find((CharacterModelRequest.CharacterBundleRequest r) => r.characterId == modelRequest.characterId);
		if (characterBundleRequest != null)
		{
			characterBundleRequest.RemoveModelRequest(modelRequest);
		}
	}

	public void CharacterBundleRequestCompleted(CharacterModelRequest.CharacterBundleRequest request)
	{
		_activeCharacterBundleRequests.Remove(request);
	}

	public void CancelCharacterBundleRequest(CharacterModelRequest.CharacterBundleRequest request)
	{
		if (_pendingCharacterBundleRequests.Contains(request))
		{
			_pendingCharacterBundleRequests.Remove(request);
		}
	}

	[DebuggerHidden]
	public IEnumerator<float> CacheNeededAssetBundles()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CCacheNeededAssetBundles_003Ec__Iterator2 obj = new _003CCacheNeededAssetBundles_003Ec__Iterator2();
		obj._0024this = this;
		return obj;
	}

	private void OnEventChanged()
	{
		if (!Game.IsInstantiated || Game.Instance.IsInGame.Value)
		{
			_delayedAvailabilityCheck = true;
		}
		else
		{
			PerformAvailabilityCheck();
		}
	}

	private void DelayedAvailabilityCheck()
	{
		if (_delayedAvailabilityCheck)
		{
			_delayedAvailabilityCheck = false;
			PerformAvailabilityCheck();
		}
	}

	public void PerformAvailabilityCheck()
	{
		Dictionary<string, int> neededBundles = _versionControl.GetNeededBundles();
		bool flag = true;
		AssetBundleCache.StartBulkCaching();
		foreach (KeyValuePair<string, int> item in neededBundles)
		{
			if (!flag)
			{
				break;
			}
			int versionForCachedBundle = AssetBundleCache.GetVersionForCachedBundle(item.Key);
			if (versionForCachedBundle != item.Value)
			{
				flag = CacheAssetBundle(item.Key, item.Value);
			}
		}
		AssetBundleCache.EndBulkCaching();
		if (!flag)
		{
			LogError("We do not have all the necessary bundles cached. Please free some disk space and restart the App.");
			Flurry.LogEvent("Asset Bundles Availability Check Failed");
		}
	}

	private bool CacheAssetBundle(string bundleName, int version, byte[] bundleBytes = null)
	{
		if (bundleBytes != null)
		{
			return AssetBundleCache.CacheAssetBundle(bundleName, version, bundleBytes);
		}
		if (Application.isEditor)
		{
			string editorBundlesPath = GetEditorBundlesPath(bundleName);
			return AssetBundleCache.CacheEditorBundle(bundleName, version, editorBundlesPath);
		}
		string path = Path.Combine("AssetBundles", bundleName);
		TextAsset textAsset = Resources.Load<TextAsset>(path);
		if ((bool)textAsset)
		{
			byte[] bytes = textAsset.bytes.Decompress("subway_bundles");
			return AssetBundleCache.CacheAssetBundle(bundleName, version, bytes);
		}
		LogError("Cannot load bundle from resources: " + Path.Combine("AssetBundles", bundleName));
		return false;
	}

	public void UnloadAssetBundle(string bundleName, bool unloadAllLoadedObjects)
	{
		_pendingUnloads[bundleName] = unloadAllLoadedObjects;
		CoroutineUtil.StartCoroutine(UnloadBundlesCoroutine());
	}

	public static void ClearCache()
	{
		AssetBundleCache.ClearCache();
	}

	private IEnumerator UnloadBundlesCoroutine()
	{
		yield return new WaitForEndOfFrame();
		if (_pendingUnloads.Count <= 0)
		{
			yield break;
		}
		List<string> successfullyUnloadedBundles = new List<string>();
		foreach (KeyValuePair<string, bool> pendingUnload in _pendingUnloads)
		{
			string key = pendingUnload.Key;
			bool value = pendingUnload.Value;
			if (!_assetBundlesLoading.Contains(key))
			{
				if (_loadedAssetBundles.ContainsKey(key) && _loadedAssetBundles[key] != null)
				{
					_loadedAssetBundles[key].Unload(value);
					_loadedAssetBundles.Remove(key);
				}
				successfullyUnloadedBundles.Add(key);
			}
		}
		foreach (string item in successfullyUnloadedBundles)
		{
			_pendingUnloads.Remove(item);
		}
	}

	private void InitializeDownloadRequests()
	{
		_lastInitializedDownloads = Time.timeSinceLevelLoad;
		_pendingDownloads = new Dictionary<string, int>();
		List<KeyValuePair<Characters.CharacterId, Characters.Model>> availableCharacters = CharacterManager.GetAvailableCharacters();
		foreach (KeyValuePair<Characters.CharacterId, Characters.Model> item in availableCharacters)
		{
			string[] referencedAssetBundleNames = SingletonMonoBehaviour<Factories>.Instance.GetReferencedAssetBundleNames(item.Key);
			string[] array = referencedAssetBundleNames;
			foreach (string bundleName in array)
			{
				int latestVersionForBundle = _versionControl.GetLatestVersionForBundle(bundleName);
				if (!AssetBundleCache.IsAssetBundleCached(bundleName, latestVersionForBundle) && !_versionControl.IsBundleVersionShipped(bundleName, latestVersionForBundle))
				{
					AddAssetBundleDownloadRequest(bundleName, latestVersionForBundle);
				}
			}
		}
		UserSave.allAssetBundlesDownloaded = !_pendingDownloads.Any();
		attemptingToDownloadAmount = ((_pendingDownloads.Count > 0) ? _pendingDownloads.Count : 0);
		succesfullyDownloaded = 0;
	}

	private void OnDownloadableAssetBundlesUpdated()
	{
		_downloadableBundlesUpdated = true;
	}

	private void AddAssetBundleDownloadRequest(string bundleName, int version)
	{
		if (_pendingDownloads.ContainsKey(bundleName))
		{
			if (_pendingDownloads[bundleName] > version)
			{
				_pendingDownloads[bundleName] = version;
			}
		}
		else
		{
			_pendingDownloads.Add(bundleName, version);
		}
	}

	public void ResumeAssetBundlesDownload()
	{
		// Sem servidor de conteúdo da Kiloo: no navegador não baixa pacotes extras
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			return;
		}
		if (_pendingDownloads == null || !_pendingDownloads.Any() || _downloadableBundlesUpdated)
		{
			if (AllowedToInitializeDownloads())
			{
				InitializeDownloadRequests();
				if (_pendingDownloads.Any())
				{
					ResumeAssetBundlesDownload();
				}
			}
		}
		else if (!_isDownloading && !Game.Instance.IsInGame.Value)
		{
			_currentDownloaderCoroutine = DownloadAllAssetBundles();
			CoroutineUtil.StartCoroutine(_currentDownloaderCoroutine);
		}
	}

	private bool AllowedToInitializeDownloads()
	{
		if (_downloadableBundlesUpdated)
		{
			_downloadableBundlesUpdated = false;
			return true;
		}
		return _lastInitializedDownloads + OnlineSettings.instance.GetValue("asset_bundle_download_rest_time", _downloadAttemptRestTime) < Time.timeSinceLevelLoad;
	}

	private void StopAssetbundlesDownload()
	{
		if (_isDownloading)
		{
			CoroutineUtil.StopCoroutine(_currentDownloaderCoroutine);
			_isDownloading = false;
		}
	}

	private IEnumerator DownloadAllAssetBundles()
	{
		_isDownloading = true;
		while (_pendingDownloads.Any())
		{
			KeyValuePair<string, int> nextBundle = _pendingDownloads.First();
			yield return CoroutineUtil.StartCoroutine(DownloadAssetBundle(nextBundle.Key, nextBundle.Value));
		}
		_isDownloading = false;
	}

	private IEnumerator DownloadAssetBundle(string bundleName, int version)
	{
		OnlineSettings onlineSettings = OnlineSettings.instance;
		bool downloadSuccessful = false;
		string serverUrl = GetAssetBundlesServerURL(bundleName);
		float timeout = onlineSettings.GetValue("asset_bundle_request_timeout", _requestDownloadTimeout);
		IRequest request = RequestExtensions.WithMaxBackOffTime(maxBackOffTime: onlineSettings.GetValue("asset_bundle_request_max_backoff_time", _requestMaxBackoffTime), request: WebClient.CreateGet(new Uri(serverUrl)).WithTimeout(timeout));
		yield return request.SendAsync();
		if (request.isFinished && request.isSuccessful)
		{
			byte[] bundleBytes = request.response.bytes.Decompress("subway_bundles");
			if (CacheAssetBundle(bundleName, version, bundleBytes))
			{
				downloadSuccessful = true;
				succesfullyDownloaded++;
				ResolveDownloadRequest(bundleName, version);
			}
			else
			{
				LogError("Attempt to cache downloaded bundle from the server failed.");
			}
		}
		else
		{
			if (request.isFinished)
			{
				string contryCodeFromLocale = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
				Flurry.LogEventWithSeveralParameters("Asset Bundle Failed to Download", "Asset Bundle Name;Asset Bundle Version;Asset Bundle Download Error;Country", bundleName + ";" + version + ";" + ((request.response == null) ? (-1) : request.response.status) + ";" + contryCodeFromLocale);
			}
			LogError("Attempt to download from the server failed. Error message: " + request.exception);
		}
		if (!downloadSuccessful)
		{
			DownloadFailedFor(bundleName, version);
		}
	}

	private void DownloadFailedFor(string bundleName, int version)
	{
		if (_pendingDownloads.ContainsKey(bundleName))
		{
			_pendingDownloads.Remove(bundleName);
			return;
		}
		LogError("No pending download found for failed download: " + bundleName + " version " + version);
	}

	private void ResolveDownloadRequest(string bundleName, int version)
	{
		if (_pendingDownloads.ContainsKey(bundleName) && _pendingDownloads[bundleName] <= version)
		{
			_pendingDownloads.Remove(bundleName);
		}
	}

	public static string SerializeAssetBundleVersionData(Dictionary<string, int> versionData)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (versionData != null)
		{
			foreach (KeyValuePair<string, int> versionDatum in versionData)
			{
				stringBuilder.Append(versionDatum.Key);
				stringBuilder.Append('-');
				stringBuilder.Append(versionDatum.Value);
				stringBuilder.Append(',');
			}
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<string, int> ParseAssetBundleVersionData(string serializedData)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array = serializedData.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split('-');
			if (array3.Length == 2)
			{
				dictionary.Add(array3[0], int.Parse(array3[1]));
			}
		}
		return dictionary;
	}

	public string GetPreCachedBundles()
	{
		if (attemptingToPrecacheAmount == succesfullyPrecached)
		{
			return "Complete(" + attemptingToPrecacheAmount + ")";
		}
		return string.Empty + succesfullyPrecached + "/" + attemptingToPrecacheAmount;
	}

	public string GetDownloadedBundles()
	{
		if (attemptingToDownloadAmount == succesfullyDownloaded)
		{
			return "Complete(" + attemptingToDownloadAmount + ")";
		}
		return string.Empty + succesfullyDownloaded + "/" + attemptingToDownloadAmount;
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
		SDebug.LogError("[AssetBundleManager]: " + msg);
	}

	[Conditional("ASSET_BUNDLES_REQUESTS_DEBUG_LOGS")]
	private void DebugRequestStatus(string bundleName)
	{
		string text = "REQUESTED: " + bundleName + "\n";
		text += "LOADED:\n";
		foreach (string key in _loadedAssetBundles.Keys)
		{
			text = text + "     " + key + "\n";
		}
		text += "LOADING:\n";
		foreach (string item in _assetBundlesLoading)
		{
			text = text + "     " + item + "\n";
		}
		text += "PENDING:\n";
		foreach (KeyValuePair<string, Action<AssetBundle>> pendingRequest in _pendingRequests)
		{
			text = text + "     " + pendingRequest.Key + "\n";
		}
		text += "UNLOADING:\n";
		foreach (string key2 in _pendingUnloads.Keys)
		{
			text = text + "     " + key2 + "\n";
		}
	}
}
