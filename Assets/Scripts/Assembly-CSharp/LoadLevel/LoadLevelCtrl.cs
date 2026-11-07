using System.Collections;
using AppRefresh;
using Cloud;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using NaughtyAttributes;
using SYBO.Components.Utilities;
using SYBO.Subway.UI;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace LoadLevel
{
	public class LoadLevelCtrl : MonoBehaviour
	{
		private AsyncOperation _asyncMerge;

		[SerializeField]
		private UILabel _titleTipLabel;

		[SerializeField]
		private UILabel _tipLabel;

		[SerializeField]
		private UILabel _userNameLabel;

		[SerializeField]
		private UILabel _loadingLabel;

		[SerializeField]
		private UISlider _slider;

		[SerializeField]
		private GameObject _splashObject;

		[SerializeField]
		private UIWidget _background;

		[SerializeField]
		private UIRoot _uiRoot;

		private float _sliderSteps;

		private bool _sliderStepsIsSet;

		private const float CACHING_TIME_BUDGET = 0.1f;

		private readonly string[] _standardTipArray = new string[35]
		{
			"LOADING_TIP_01", "LOADING_TIP_02", "LOADING_TIP_03", "LOADING_TIP_04", "LOADING_TIP_05", "LOADING_TIP_06", "LOADING_TIP_07", "LOADING_TIP_08", "LOADING_TIP_10", "LOADING_TIP_11",
			"LOADING_TIP_12", "LOADING_TIP_13", "LOADING_TIP_14", "LOADING_TIP_15", "LOADING_TIP_16", "LOADING_TIP_17", "LOADING_TIP_18", "LOADING_TIP_19", "LOADING_TIP_20", "LOADING_TIP_21",
			"LOADING_TIP_22", "LOADING_TIP_23", "LOADING_TIP_25", "LOADING_TIP_26", "LOADING_TIP_27", "LOADING_TIP_28", "LOADING_TIP_29", "LOADING_TIP_30", "LOADING_TIP_31", "LOADING_TIP_32",
			"LOADING_TIP_33", "LOADING_TIP_34", "LOADING_TIP_35", "LOADING_TIP_36", "LOADING_TIP_37"
		};

		private float _markedProgress;

		private float _currentProgress;

		private float _loadSteps = 3f;

		public static bool startupFinished { get; private set; }

		[UsedImplicitly]
		private void Awake()
		{
			Object.DontDestroyOnLoad(base.transform.gameObject);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.IPhonePlayer:
				num = IOSViewController.GetBottomInsetInPoints();
				num3 = IOSViewController.GetDeviceHeightInPoints();
				break;
			case RuntimePlatform.Android:
				if (!DeviceUtility.isKindleDevice)
				{
					num = DeviceUtility.GetAndroidBottomCutout();
					num2 = DeviceUtility.GetAndroidTopCutout();
					num3 = Screen.height;
					if (num > 0 || num2 > 0)
					{
						Screen.autorotateToLandscapeLeft = false;
						Screen.autorotateToLandscapeRight = false;
						Screen.autorotateToPortrait = true;
						Screen.autorotateToPortraitUpsideDown = false;
						Screen.orientation = ScreenOrientation.AutoRotation;
						SafeArea.PortraitUpsideDownIsAllowed = false;
					}
				}
				break;
			}
			if (num > 0)
			{
				float num4 = (float)num / (float)num3;
				float num5 = 0f;
				if (_uiRoot != null && _background != null)
				{
					num5 = num4 * (float)_uiRoot.manualHeight;
					_background.topAnchor.absolute += num5.RoundToInt();
				}
			}
		}

		[UsedImplicitly]
		private void Update()
		{
			LoadBar();
		}

		private void SetLabels()
		{
			_loadingLabel.text = Strings.Get("LOADING_PROGRESS_TEXT");
			SetColors();
			UserTip[] userTips = OnlineSettingsManager.localizedData.userTips;
			string loadScreenTitle = OnlineSettingsManager.localizedData.loadScreenTitle;
			if (OnlineSettingsManager.data.allowLoadScreenUserTips && userTips != null && userTips.Length > 0 && !loadScreenTitle.IsNullOrEmpty())
			{
				SetUserTip(loadScreenTitle, userTips);
			}
			else
			{
				SetDefaultTip();
			}
		}

		[Button(null)]
		private void SetColors()
		{
			ColorData instance = ColorData.instance;
			_tipLabel.color = instance.midnight;
			_userNameLabel.color = instance.peacock;
			_slider.foregroundWidget.color = instance.sun;
			_slider.backgroundWidget.color = instance.sky;
			_slider.gameObject.Blink();
		}

		private void SetUserTip(string loadScreenTitle, UserTip[] userTips)
		{
			_userNameLabel.gameObject.SetActive(true);
			_titleTipLabel.text = loadScreenTitle;
			UserTip randomElement = userTips.GetRandomElement();
			if (randomElement == null)
			{
				SetDefaultTip();
				return;
			}
			if (randomElement.userName.IsNullOrEmpty() || randomElement.city.IsNullOrEmpty() || randomElement.country.IsNullOrEmpty() || randomElement.tip.IsNullOrEmpty())
			{
				SetDefaultTip();
				return;
			}
			string text = randomElement.userName + ", " + randomElement.city + ", " + randomElement.country;
			_userNameLabel.text = "- " + text + " -";
			_tipLabel.text = "\"" + randomElement.tip + "\"";
		}

		private void SetDefaultTip()
		{
			_userNameLabel.enabled = false;
			_titleTipLabel.text = Strings.Get("LOAD_LEVEL_CTRL_TIP_MESSAGE");
			_tipLabel.text = Strings.Get(_standardTipArray.GetRandomElement());
		}

		[UsedImplicitly]
		private IEnumerator Start()
		{
			startupFinished = false;
			UnityEngine.Debug.Log("GJLOAD 1 start");
			SetLabels();
			UnityEngine.Debug.Log("GJLOAD 2 labels");
			if (_tipLabel.panel != null)
			{
				_tipLabel.panel.Refresh();
			}
			float cacheTimestamp = Time.realtimeSinceStartup;
			AssetBundleManager assetBundleManager = AssetBundleManager.Instance;
			IEnumerator cacheCoroutine = assetBundleManager.CacheNeededAssetBundles();
			while (cacheCoroutine.MoveNext())
			{
				if (Time.realtimeSinceStartup - cacheTimestamp >= 0.1f)
				{
					SetCurrentProgress((float)cacheCoroutine.Current, 0.1f / _loadSteps);
					yield return null;
					cacheTimestamp = Time.realtimeSinceStartup;
				}
			}
			UnityEngine.Debug.Log("GJLOAD 3 bundles");
			MarkProgress();
			while (Application.platform != RuntimePlatform.WebGLPlayer && (!CloudSave.initializedAtStartup || AppRefreshHandler.refreshInProgress))
			{
				yield return null;
			}
			UnityEngine.Debug.Log("GJLOAD 4 cloud ok");
			_asyncMerge = SceneManager.LoadSceneAsync(MergeLevelStartHelp.MERGE_SCENE_NAME_FOR_BUILDS, LoadSceneMode.Additive);
			StartCoroutine(Progress());
		}

		private IEnumerator Progress()
		{
			while (!_asyncMerge.isDone)
			{
				if (_asyncMerge != null)
				{
					SetCurrentProgress(_asyncMerge.progress);
				}
				yield return null;
			}
			UnityEngine.Debug.Log("GJLOAD 5 merge loaded");
			MarkProgress();
			PoolManager.BalancingBudgetMS = 50;
			PoolManager.BeginBalancing(PoolManager_onProgress);
			while (PoolManager.Working)
			{
				yield return null;
			}
			UnityEngine.Debug.Log("GJLOAD 6 pool done");
			PoolManager.BalancingBudgetMS = 5;
			_currentProgress = 1f;
			yield return null;
			UnloadAssets();
			Object.Destroy(base.gameObject);
			startupFinished = true;
		}

		private void PoolManager_onProgress(float balancingProgress)
		{
			SetCurrentProgress(balancingProgress);
		}

		private void LoadBar()
		{
			if (!_sliderStepsIsSet)
			{
				_sliderSteps = _slider.backgroundWidget.width;
				_sliderStepsIsSet = true;
			}
			_slider.value = GetProgress() * _sliderSteps / _sliderSteps;
		}

		private float GetProgress()
		{
			return _markedProgress + _currentProgress * (1f - _markedProgress) / _loadSteps;
		}

		private void SetCurrentProgress(float progress, float maxDeltaProgress = 1f)
		{
			if (_loadSteps > 1f)
			{
				_currentProgress = Mathf.Clamp(progress, _currentProgress, _currentProgress + maxDeltaProgress);
			}
			else
			{
				_currentProgress = progress;
			}
		}

		private void MarkProgress()
		{
			_markedProgress = GetProgress();
			_currentProgress = 0f;
			_loadSteps--;
		}

		private void UnloadAssets()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				Texture mainTexture = _splashObject.GetComponent<Renderer>().material.mainTexture;
				if (mainTexture != null)
				{
					Resources.UnloadAsset(mainTexture);
				}
			}
			Object.Destroy(_splashObject);
		}
	}
}
