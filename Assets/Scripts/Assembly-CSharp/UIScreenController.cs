#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ABTest;
using AppRefresh;
using Authentication;
using Cloud;
using Cloud.Internal;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using LoadLevel;
using Missions;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Info;
using SYBO.Subway.Themes;
using TopRun;
using UI;
using UI.MessageCenter;
using UI.Popups;
using UI.Screens;
using UI.Screens.RewardManagement;
using UI.SlideIns;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using WordHunt;

public class UIScreenController : MonoBehaviour
{
	private const string SCREEN_RESOURCE_PATH = "Prefabs/Screens/";

	private const string POPUP_RESOURCE_PATH = "Prefabs/Popups/";

	private const float CLOUD_OVERLAY_TIMEOUT_SECONDS = 20f;

	private static UIScreenController _instance;

	[SerializeField]
	[FormerlySerializedAs("backgroundAnchor")]
	private GameObject _backgroundLayer;

	[SerializeField]
	[FormerlySerializedAs("screenAnchor")]
	private GameObject _screenLayer;

	[SerializeField]
	[FormerlySerializedAs("popupAnchor")]
	private GameObject _popupLayer;

	[SerializeField]
	private GameObject _topLayer;

	[SerializeField]
	[FormerlySerializedAs("superPopupAnchor")]
	private GameObject _overlayLayer;

	[SerializeField]
	private GameObject _adsSystemDisableIndicator;

	[SerializeField]
	private Transform _topBarContainer;

	[SerializeField]
	private TopBar _topBarPrefab;

	private TopBar _topBar;

	private bool _hasSetTopBarContainer;

	public Camera Camera3D;

	public Camera CameraOverlay2dClipping;

	public Camera CameraOverlay2d;

	public Camera CameraCelebration;

	public Camera Camera2DNormal;

	public Camera Camera3DOverlay;

	public Camera Camera3DClip;

	public UIRoot root;

	public GameObject MenuElements3D;

	public bool LoadMenuOnStart;

	public Font FloatingTextFont;

	private Camera mainCamera;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> OnChangedScreen__BackingField = null;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> OnNavigatedToScreen__BackingField = null;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> OnWillShowScreen__BackingField = null;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> OnWillNavigateToScreen__BackingField = null;

	[SerializeField]
	private Font lilitaRegularFont;

	[SerializeField]
	private Font titanRegularFont;

	private bool _isApplicationFocused = true;

	private static readonly List<string> PAYOUT_DISALLOWED_SCREENS = new List<string> { "IngameUI" };

	private static bool _facebookPayoutPopupQueued = false;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnAwake__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnApplicationPaused__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnApplicationResumed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> OnPopupShown__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnLastPopupClosed__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> OnPopupClosed__BackingField;

	private Dictionary<string, UIScreen> _cachedScreens = new Dictionary<string, UIScreen>();

	private List<string> _screenStack = new List<string>();

	private List<string> _popupQueue = new List<string>();

	private List<string> _delayedPopups = new List<string>();

	private bool _popupActive;

	private List<UISlideIn> _allSlideIns = new List<UISlideIn>();

	private List<string> _screenNamesWithoutBackground = new List<string> { "FrontScreen", "IngameUI" };

	public const string DEFAULT_CHARACTER_SET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

	public const int DEFAULT_SIZE = 16;

	public const FontStyle DEFAULT_STYLE = FontStyle.Normal;

	public const string DOUBLE_UP_2X_TEXT = "2x";

	private bool _isApplicationResuming;

	private bool _isApplicationPaused;

	public bool justPaidOutTopRun;

	private bool _hasShownFrontScreenBefore;

	public List<InAppItemModel> coinAndKeyOffers;

	private bool _enabledMainCameraInThisFrame;

	private List<string> _excludedScreensFromClear = new List<string> { "CloudSaveCelebrationPopup" };

	private List<string> _excludedScreensFromMemoryCleanup = new List<string> { "FrontScreen", "NotebookScreen" };

	public bool doSaveAndDelayRemainingPopups;

	public bool ignoreLastPopupClosed;

	private UISlideInMissionHelper _missionSlideIn;

	private UISlideInMissionSetHelper _missionSetSlideIn;

	private UISlideInLettersHelper _lettersSlideIn;

	private UISlideInUnlock _unlockSlideIn;

	private UISlideInErrorMessage _errorSlideIn;

	private UISlideInOfferPayoutHelper _offerWallSlideIn;

	private UISlideInItemHunt _weeklyHuntSlideIn;

	private UISlideInBrag _bragSlideIn;

	private UISlideInText _slideInText;

	private UISlideInVideoPayoutHelper _videoPayoutSlideIn;

	private UISlideInAwards _awardsSlideIn;

	private UISlideInFacebook _facebookSlideIn;

	private UISlideInTopRunTimer _topRunTimerSlideIn;

	private UISlideInCharacterEventHelper _characterEventSlideIn;

	private UISlideInCharacterEventRewardHelper _characterEventRewardSlideIn;

	private UISlideInAdsRemoved _adsRemovedSlideIn;

	private UISlideInGift _giftSlideIn;

	private UiSlideInFirstPurchaseBonus _firstPurchaseBonusSlideIn;

	private UISlideInCloudSave _cloudSaveSlideIn;

	private UISlideInCloudSaveMessage _cloudSaveMessageSlideIn;

	private List<SlideIn> _slideInQueue = new List<SlideIn>(15);

	private bool _slideInActive;

	private bool _slideHasPreloaded;

	private bool _allowShowingSlideIns = true;

	private List<SlideInData.SlideInType> _uniqueSlideInTypes = new List<SlideInData.SlideInType> { SlideInData.SlideInType.Facebook };

	public UIMessageHelper messageHelper;

	private Queue<string> _messageQueue = new Queue<string>();

	private bool _messageShowing;

	public GameObject inAppPurchaseOverlay;

	public GameObject cloudSaveOverlay;

	public GameObject adsOverlay;

	[SerializeField]
	private GameObject transitionAdsOverlay;

	private IEnumerator _transitionAdsOverlayCoroutine;

	public GameObject videoAdsOverlay;

	public GameObject videoAdsOverlayCloseButton;

	public UILabel videoAdsOverlayLabel;

	private bool _allowInputOnVideoAdsOverlay = true;

	private string _provider = string.Empty;

	private AdCacheSlotInfo _adCacheSlotInfo = AdCacheSlots.UNDETERMINED_SLOT;

	private AdLocationInfo _adLocationInfo = AdLocations.UNDETERMINED_LOCATION;

	private IEnumerator _videoAdsOverlayInputCoroutine;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnOverlayManuallyClosed__BackingField;

	[SerializeField]
	private GameObject generalDebugLabel;

	public static Vector3 popupDefaultPosition = new Vector3(-70f, -25f, 0f);

	public static bool IsInstanced
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController;
			}
			return _instance != null;
		}
	}

	public static UIScreenController Instance
	{
		get
		{
			return _instance ? _instance : (_instance = UnityEngine.Object.FindObjectOfType(typeof(UIScreenController)) as UIScreenController);
		}
	}

	public GameObject popupLayer
	{
		get
		{
			return _popupLayer;
		}
	}

	public TopBar topBar
	{
		get
		{
			if (_topBar == null)
			{
				LoadTopBar();
			}
			return _topBar;
		}
	}

	public bool IsApplicationFocused
	{
		get
		{
			return _isApplicationFocused;
		}
	}

	public bool IsApplicationPaused
	{
		get
		{
			return _isApplicationPaused;
		}
	}

	public bool isShowingPopup
	{
		get
		{
			return _popupQueue != null && _popupQueue.Count > 0;
		}
	}

	public static Vector3 screenDefaultPosition
	{
		get
		{
			Vector3 result = new Vector3(90f, -60f, 0f);
			if (SafeArea.IsRestricted)
			{
				result.y += -SafeArea.OFFSET_TOP;
			}
			return result;
		}
	}

	public static event Action<string> OnChangedScreen
	{
		add
		{
			Action<string> action = OnChangedScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnChangedScreen__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = OnChangedScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnChangedScreen__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> OnNavigatedToScreen
	{
		add
		{
			Action<string> action = OnNavigatedToScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnNavigatedToScreen__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = OnNavigatedToScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnNavigatedToScreen__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> OnWillShowScreen
	{
		add
		{
			Action<string> action = OnWillShowScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWillShowScreen__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = OnWillShowScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWillShowScreen__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> OnWillNavigateToScreen
	{
		add
		{
			Action<string> action = OnWillNavigateToScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWillNavigateToScreen__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = OnWillNavigateToScreen__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnWillNavigateToScreen__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnAwake
	{
		add
		{
			Action action = OnAwake__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnAwake__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnAwake__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnAwake__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnApplicationPaused
	{
		add
		{
			Action action = OnApplicationPaused__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnApplicationPaused__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnApplicationPaused__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnApplicationPaused__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnApplicationResumed
	{
		add
		{
			Action action = OnApplicationResumed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnApplicationResumed__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnApplicationResumed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnApplicationResumed__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> OnPopupShown
	{
		add
		{
			Action<string> action = OnPopupShown__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPopupShown__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = OnPopupShown__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPopupShown__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnLastPopupClosed
	{
		add
		{
			Action action = OnLastPopupClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLastPopupClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnLastPopupClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnLastPopupClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<string> OnPopupClosed
	{
		add
		{
			Action<string> action = OnPopupClosed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPopupClosed__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = OnPopupClosed__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnPopupClosed__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnOverlayManuallyClosed
	{
		add
		{
			Action action = OnOverlayManuallyClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnOverlayManuallyClosed__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnOverlayManuallyClosed__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnOverlayManuallyClosed__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	[UsedImplicitly]
	private void Awake()
	{
		foreach (Transform item in _backgroundLayer.transform)
		{
			item.gameObject.SetActive(false);
		}
		foreach (Transform item2 in _screenLayer.transform)
		{
			item2.gameObject.SetActive(false);
		}
		foreach (Transform item3 in _popupLayer.transform)
		{
			item3.gameObject.SetActive(false);
		}
		foreach (Transform item4 in _topBarContainer.transform)
		{
			item4.gameObject.SetActive(false);
		}
		MissionsManager.onMissionComplete += OnMissionCompleted;
		MissionsManager.onMissionSetComplete += OnMissionSetCompleted;
		WordHuntManager.OnPickedUpLetter += OnLetterPickedUp;
		CharacterManager.characterUnlocked += OnCharacterUnlocked;
		CharacterManager.outfitUnlocked += OnOutfitUnlocked;
		OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(() =>
		{
			coinAndKeyOffers = InAppManager.GetActiveInAppOffers().Values.ToList();
		});
		OnWillNavigateToScreen += OnWillNavigateToScreenHandler;
		AppRefreshHandler.willRefresh += OnWillRefresh;
		AppRefreshHandler.hasRefreshed += OnHasRefreshed;
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
		if (lilitaRegularFont != null)
		{
			lilitaRegularFont.RequestCharactersInTexture("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 16, FontStyle.Normal);
			RequestCharactersForDoubleUpButton();
		}
		if (titanRegularFont != null)
		{
			titanRegularFont.RequestCharactersInTexture("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 16, FontStyle.Normal);
		}
		if (_instance != null)
		{
			_instance = this;
		}
		OnAwake__BackingField.SafeInvoke();
		coinAndKeyOffers = InAppManager.GetActiveInAppOffers().Values.ToList();
		InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
		CloudSave.stateChanged += () =>
		{
			CloudSaveState state = CloudSave.state;
			if (state != CloudSaveState.Active && state != CloudSaveState.OptedOut && state != CloudSaveState.Conflict)
			{
				QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.CloudSave,
					cloudSaveState = state
				});
			}
		};
		if (Application.isEditor && Auth.isInitialized && !CloudSave.initializedAtStartup)
		{
			StartCoroutine(CloudSave.StartUpRoutine());
		}
	}

	public void RequestCharactersForDoubleUpButton()
	{
		lilitaRegularFont.RequestCharactersInTexture("CELEBRATION_POPUP_DOUBLE_UP", 40, FontStyle.Normal);
		lilitaRegularFont.RequestCharactersInTexture("CELEBRATION_POPUP_STICKER_BONUS", 32, FontStyle.Normal);
		lilitaRegularFont.RequestCharactersInTexture("2x", 78, FontStyle.Normal);
	}

	private void OnWillRefresh(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.UI))
		{
			if (mainCamera != null && !mainCamera.enabled)
			{
				mainCamera.enabled = true;
			}
			ignoreLastPopupClosed = true;
			ClearPopupsAndScreens();
			ClearSlideIns();
			_allowShowingSlideIns = false;
		}
		if (refreshType.HasFlag(RefreshType.MemoryWarning))
		{
			ignoreLastPopupClosed = true;
			MemoryWarningClearUnusedPopupsAndScreens();
			ClearSlideIns();
			_allowShowingSlideIns = false;
		}
		if (refreshType.HasFlag(RefreshType.Data) && (bool)_topBar)
		{
			UnityEngine.Object.Destroy(_topBar.gameObject);
		}
	}

	private void OnHasRefreshed(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.UI))
		{
			PreloadAllSlideIn();
			if (_hasShownFrontScreenBefore)
			{
				_ActivateScreen("FrontScreen");
			}
			Kiloo.Common.Timer.Timer.ExecuteNextFrame(PrepareSlideIns);
		}
		if (refreshType.HasFlag(RefreshType.MemoryWarning))
		{
			PreloadAllSlideIn();
			_ActivateScreen(MemoryWarningHandler.instance.activeScreenDuringCleanup);
			Kiloo.Common.Timer.Timer.ExecuteNextFrame(PrepareSlideIns);
		}
		if (refreshType.HasFlag(RefreshType.Data))
		{
			LoadTopBar();
			CharacterScreenManager.Instance.SelectCharacter(PlayerSave.currentCharacterId, PlayerSave.currentOutfitIndex);
		}
	}

	private void PrepareSlideIns()
	{
		_allowShowingSlideIns = true;
		ReadyForNextSlide();
	}

	private void OnDestroy()
	{
		MissionsManager.onMissionComplete -= OnMissionCompleted;
		MissionsManager.onMissionSetComplete -= OnMissionSetCompleted;
		WordHuntManager.OnPickedUpLetter -= OnLetterPickedUp;
		CharacterManager.characterUnlocked -= OnCharacterUnlocked;
		CharacterManager.outfitUnlocked -= OnOutfitUnlocked;
		OnWillNavigateToScreen -= OnWillNavigateToScreenHandler;
		AppRefreshHandler.willRefresh -= OnWillRefresh;
		AppRefreshHandler.hasRefreshed -= OnHasRefreshed;
	}

	private void Start()
	{
		HideInAppPurchaseOverlay();
		if (LoadMenuOnStart)
		{
			ShowMainMenu();
		}
		PlayerSave.bragScoreThreshold = PlayerSave.highScore;
		MissionInfo[] missionInfo = MissionsManager.Instance.GetMissionInfo();
		if (missionInfo[0].complete && missionInfo[1].complete && missionInfo[2].complete)
		{
			PlayerSave.missionSetCompletedCount++;
			MissionsManager.Instance.currentMissionSet++;
			LogWarning("you completed all missions but was not sent to next mission set, this should never happen, but this fixes it");
		}
		LoadTopBar();
		PreloadAllSlideIn();
		UpdateNguiTouchThresholds();
		EnableGeneralDebugLabelIfNeeded();
		WordHuntManager.instance.DownloadDailyIfExpired();
	}

	private void LoadTopBar()
	{
		if (_topBar == null)
		{
			UIWidget component = _topBarContainer.GetComponent<UIWidget>();
			if (!_hasSetTopBarContainer)
			{
				component.bottomAnchor.absolute += ((!SafeArea.IsRestricted) ? (-1) : (-SafeArea.OFFSET_TOP - 1));
				component.ResetAndUpdateAnchors();
				_hasSetTopBarContainer = true;
			}
			_topBar = UnityEngine.Object.Instantiate(_topBarPrefab, _topBarContainer);
			_topBar.transform.Reset();
			_topBar.Init(new TopBar.TopBarPayload
			{
				width = component.width,
				height = component.height
			});
		}
	}

	private void _SetApplicationPause(bool isApplicationPaused)
	{
		if (isApplicationPaused)
		{
			_isApplicationPaused = true;
			UpdateAppPopup.AllowAgainThisSession();
			LogFlurry();
			OnApplicationPaused__BackingField.SafeInvoke();
		}
		else
		{
			_isApplicationPaused = false;
		}
		_isApplicationResuming = !_isApplicationPaused;
	}

	private void _SetApplicationFocus(bool _isApplicationFocus)
	{
		_isApplicationFocused = _isApplicationFocus;
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			_SetApplicationPause(!_isApplicationFocus);
			if (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice && !_isApplicationFocus && Peek(_screenStack) == "IngameUI" && !Game.Instance.isDead)
			{
				SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = 0f;
			}
		}
	}

	public void DebugSetApplicationPause(bool isApplicationPaused)
	{
		_SetApplicationPause(isApplicationPaused);
	}

	public void DebugSetApplicationFocus(bool isApplicationFocused)
	{
		_SetApplicationFocus(isApplicationFocused);
	}

	private void LateUpdate()
	{
		if (_isApplicationResuming)
		{
			_isApplicationResuming = false;
			if (_screenStack.Count > 0 && Peek(_screenStack) == "IngameUI" && (Game.Instance == null || !Game.Instance.isDead) && (!_popupActive || !(Peek(_popupQueue) == "ConsumablesPopup")))
			{
				PushScreen(null, "PauseUI");
			}
			if (OnApplicationResumed__BackingField != null)
			{
				OnApplicationResumed__BackingField();
			}
		}
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape) || IsInAppPurchaseOverlayVisible())
		{
			return;
		}
		if (IsAdsOverlayVisible())
		{
			HideAdsOverlay();
			return;
		}
		string text = string.Empty;
		if (!isShowingPopup)
		{
			string text2 = Peek(_screenStack);
			if (!string.IsNullOrEmpty(text2) && _cachedScreens.ContainsKey(text2))
			{
				_cachedScreens[text2].BackButtonPressed();
				text = text2;
			}
		}
		else
		{
			string currentPopupName = GetCurrentPopupName();
			if (!string.IsNullOrEmpty(currentPopupName) && _cachedScreens.ContainsKey(currentPopupName))
			{
				_cachedScreens[currentPopupName].BackButtonPressed();
				text = currentPopupName;
			}
		}
		Flurry.LogEventWithAParameter("Back button pressed", "Screen Name", (!string.IsNullOrEmpty(text)) ? text : "Null");
	}

	private void OnApplicationPause(bool isApplicationPaused)
	{
		_SetApplicationPause(isApplicationPaused);
	}

	private void OnApplicationFocus(bool isFocused)
	{
		_SetApplicationFocus(isFocused);
	}

	private void UpdateNguiTouchThresholds()
	{
		float touchClickThreshold = ((!(Screen.dpi > 0f)) ? 30f : (0.1f * Screen.dpi));
		UICamera.first.touchDragThreshold = 1f;
		UICamera.first.touchClickThreshold = touchClickThreshold;
	}

	private void LogFlurry()
	{
		string currentPopupName = GetCurrentPopupName();
		string topScreenName = GetTopScreenName();
		string eventName = ((Application.platform == RuntimePlatform.Android) ? "Home button pressed" : ((Application.platform != RuntimePlatform.IPhonePlayer) ? "Editor back btn pressed" : "Back button pressed"));
		if (!string.IsNullOrEmpty(currentPopupName))
		{
			if (!string.IsNullOrEmpty(currentPopupName) && !string.IsNullOrEmpty(topScreenName))
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", topScreenName + "_" + currentPopupName);
			}
			else if (!string.IsNullOrEmpty(currentPopupName))
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", currentPopupName);
			}
			else if (!string.IsNullOrEmpty(topScreenName))
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", topScreenName);
			}
			else
			{
				Flurry.LogEventWithAParameter(eventName, "Screen Name", "NO SCREEN");
			}
		}
	}

	public void FacebookLogIn(bool loggedIn)
	{
		if (loggedIn)
		{
			MissionsManager.Instance.PlayerDidThis(MissionTarget.FacebookLoggedIn);
		}
	}

	public void ShowMainMenu()
	{
		StartCoroutine(ShowMainMenuCoroutine());
	}

	private IEnumerator ShowMainMenuCoroutine()
	{
		if (Application.isEditor)
		{
			while (PoolManager.Working)
			{
				yield return null;
			}
		}
		else
		{
			while (!LoadLevelCtrl.startupFinished)
			{
				yield return null;
			}
		}
		ThemeManager.Instance.ForceRefresh();
		yield return null;
		_hasShownFrontScreenBefore = true;
		_ActivateScreen("FrontScreen");
	}

	public void GameOverTriggered()
	{
		if (!GetTopScreenName().Equals("GameOverScreen"))
		{
			GameStats.Instance.RunCompleted();
			MissionsManager.Instance.inRun = false;
			_ActivateScreen("GameOverScreen");
		}
	}

	public void QueueMessage(string message)
	{
		_QueueMessage(message);
	}

	public void GoToMainMenuFromGame(GameObject sender)
	{
		if (Game.Instance != null)
		{
			MissionsManager.Instance.inRun = false;
			Game.Instance.StartTopMenu();
			Game.Instance.TriggerPause(false);
		}
		PayWeeklyHuntIfNeeded();
		_ActivateScreen("FrontScreen");
	}

	private void PayWeeklyHuntIfNeeded()
	{
		List<CelebrationReward> weeklyHuntRewardsToUnlock = RewardManager.GetWeeklyHuntRewardsToUnlock();
		foreach (CelebrationReward item in weeklyHuntRewardsToUnlock)
		{
			PayoutCelebrationReward(item);
		}
	}

	public void PayoutCelebrationReward(CelebrationReward reward)
	{
		if (reward.rewardType == CelebrationRewardType.topRun)
		{
			TopRunRewardHelper.TrackRewardStats(reward);
			TopRunEventsHandler.instance.TriggerOnTopRunChanged(this, new TournamentChangedEventArgs());
		}
		else if (reward.rewardType == CelebrationRewardType.powerup)
		{
			InventorySave.IncreaseAmount(reward.upgradeType, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.coins)
		{
			InventorySave.IncreaseCoins(reward.amount, reward.CelebrationRewardOrigin.ToString());
		}
		else if (reward.rewardType == CelebrationRewardType.token)
		{
			CharacterManager.CollectToken(reward.characterId, reward.amount);
		}
		else if (reward.rewardType == CelebrationRewardType.trophy)
		{
			PlayerSave.UnlockTrophy(reward.trophyType);
		}
		else if (reward.rewardType == CelebrationRewardType.keys)
		{
			InventorySave.IncreaseKeys(reward.amount, reward.CelebrationRewardOrigin.ToString());
		}
		RewardManager.RewardPaidOut(reward);
	}

	public bool IsCelebrationPopupBlocked()
	{
		string currentPopupName = GetCurrentPopupName();
		if (!currentPopupName.IsNullOrEmpty())
		{
			return GetScreenFromCache(currentPopupName).BlocksCelebrationPopup();
		}
		string topScreenName = GetTopScreenName();
		if (!topScreenName.IsNullOrEmpty())
		{
			return GetScreenFromCache(topScreenName).BlocksCelebrationPopup();
		}
		return false;
	}

	public string GetTopScreenName()
	{
		if (_screenStack != null && _screenStack.Count > 0)
		{
			return Peek(_screenStack);
		}
		return null;
	}

	public string GetTopScreenFlurryName()
	{
		return GetScreenFromCache(GetTopScreenName()).GetFlurryName();
	}

	public UIScreen GetScreenFromCache(string screenName)
	{
		if (_cachedScreens.ContainsKey(screenName))
		{
			return _cachedScreens[screenName];
		}
		return null;
	}

	public void PushScreen(string screenName)
	{
		ABTestManager.CountPages(screenName);
		PushScreen(null, screenName);
	}

	public void PushScreen(GameObject sender, string screenOverride)
	{
		string text = string.Empty;
		if (screenOverride != string.Empty)
		{
			text = screenOverride;
		}
		else
		{
			UIButtonChangeScreen component = sender.GetComponent<UIButtonChangeScreen>();
			if (component != null)
			{
				text = component.GetScreenOrPopupName();
			}
		}
		if (text == "FrontScreen")
		{
			string topScreenName = GetTopScreenName();
			if (!string.IsNullOrEmpty(topScreenName))
			{
				switch (topScreenName)
				{
				case "FriendsUI":
					UIScreens.friendsMenu_lastScreen = topScreenName;
					break;
				case "LeaderboardUI":
					UIScreens.friendsMenu_lastScreen = topScreenName;
					break;
				case "TopRunUI":
					UIScreens.friendsMenu_lastScreen = "TopRunUI";
					break;
				case "CharacterScreen":
				case "BoardScreen":
				case "AwardsScreen":
					UIScreens.meMenu_lastScreen = topScreenName;
					break;
				case "UpgradeScreen":
				case "FreeStuffScreen":
				case "StoreScreen":
					UIScreens.shopMenu_lastScreen = topScreenName;
					break;
				}
			}
		}
		if (text.Equals("FrontScreen") && !string.IsNullOrEmpty(GetTopScreenName()) && GetTopScreenName().Equals("GameOverScreen"))
		{
			GoToMainMenuFromGame(sender);
		}
		else if ((!(text == "PauseUI") || (Game.Instance != null && !Game.Instance.isDead)) && (!(Peek(_screenStack) == "PauseUI") || !(text == "IngameUI") || !_cachedScreens["PauseUI"].GetComponent<PauseScreen>().IsWaiting()))
		{
			_ActivateScreen(text);
		}
	}

	public void BackToPrevious()
	{
		BackToPreviousScreen();
	}

	private UIScreen _ActivateScreen(string screenName)
	{
		bool flag = true;
		UIScreen uIScreen;
		if (_cachedScreens.ContainsKey(screenName))
		{
			if (_screenStack.Count > 0)
			{
				int num = _screenStack.LastIndexOf(screenName);
				if (num >= 0)
				{
					flag = false;
					if (num < _screenStack.Count - 1)
					{
						int num2 = num + 1;
						int num3 = _screenStack.Count - num2;
						for (int i = num2; i < num2 + num3; i++)
						{
							_cachedScreens[_screenStack[i]].HideIfActive();
						}
						_screenStack.RemoveRange(num2, num3);
					}
				}
			}
			uIScreen = _cachedScreens[screenName];
		}
		else
		{
			uIScreen = _LoadScreenToCache(screenName, _screenLayer);
			if (uIScreen == null)
			{
				return null;
			}
		}
		if (flag)
		{
			if (_screenStack.Count > 0)
			{
				UIScreen uIScreen2 = _cachedScreens[_screenStack[_screenStack.Count - 1]];
				uIScreen2.HideIfActive();
			}
			_screenStack.Add(screenName);
		}
		_ShowScreen(screenName, uIScreen);
		return uIScreen;
	}

	private void _ShowScreen(string screenName, UIScreen screen)
	{
		_SetBackground(!_screenNamesWithoutBackground.Contains(screenName));
		OnWillShowScreen__BackingField.SafeInvoke(screenName);
		OnWillNavigateToScreen__BackingField.SafeInvoke(screenName);
		topBar.ShowTopBar(screen.ShouldShowTopBar());
		screen.ShowIfInactive();
		if (screenName == "GameOverScreen")
		{
			_cachedScreens[screenName].GetComponent<GameOverScreen>().SetupBeforeMysteryBox();
			if (RewardManager.rewardsToUnlockCount > 0)
			{
				QueuePopup("CelebrationPopup");
			}
			else
			{
				_cachedScreens[screenName].GetComponent<GameOverScreen>().SetupAfterMysteryBox();
			}
		}
		OnChangedScreen__BackingField.SafeInvoke(screenName);
		OnNavigatedToScreen__BackingField.SafeInvoke(screenName);
		ScreenDidChange(screenName);
	}

	public void SwitchScreen(string screenName)
	{
		string text = Pop(_screenStack);
		if (!string.IsNullOrEmpty(text))
		{
			_cachedScreens[text].HideIfActive();
			_ActivateScreen(screenName);
		}
		else
		{
			SDebug.LogError("UIScreenController: Tried to open null screen from a button!");
		}
	}

	public void BackToPreviousScreen()
	{
		if (_screenStack.Count > 1)
		{
			string key = Pop(_screenStack);
			_cachedScreens[key].HideIfActive();
			key = Peek(_screenStack);
			_cachedScreens[key].ShowIfInactive();
			_SetBackground(!_screenNamesWithoutBackground.Contains(key));
			ScreenDidChange(key);
		}
		else
		{
			LogError("Tried to remove the only screen in the stack. You dun goofed.", this);
		}
	}

	private void ScreenDidChange(string newScreenName)
	{
		if (newScreenName != "IngameUI")
		{
			messageHelper.DisableShowLabel();
		}
		TryQueuePayoutPopups();
		if (newScreenName == "FrontScreen" || newScreenName == "GameOverScreen")
		{
			HouseKeeper.RefreshOnlineSettingsAndInappsIfNeeded();
		}
	}

	private void _SetBackground(bool active)
	{
		if (active)
		{
			if (!_cachedScreens.ContainsKey("NotebookScreen"))
			{
				_LoadScreenToCache("NotebookScreen", _backgroundLayer);
			}
			_cachedScreens["NotebookScreen"].ShowIfInactive();
			CoroutineUtil.StartCoroutine(DelayHideMainCamera());
			return;
		}
		if (_cachedScreens.ContainsKey("NotebookScreen"))
		{
			_cachedScreens["NotebookScreen"].HideIfActive();
		}
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
		mainCamera.enabled = true;
		_enabledMainCameraInThisFrame = true;
		Kiloo.Common.Timer.Timer.ExecuteNextFrame(() =>
		{
			_enabledMainCameraInThisFrame = false;
		});
	}

	private IEnumerator DelayHideMainCamera()
	{
		yield return new WaitForEndOfFrame();
		if (!_enabledMainCameraInThisFrame)
		{
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			mainCamera.enabled = false;
		}
	}

	private UIScreen _LoadScreenToCache(string screenName, GameObject destinationParent)
	{
		string text = ((!(destinationParent == _popupLayer)) ? "Prefabs/Screens/" : "Prefabs/Popups/");
		UIScreen uIScreen = Resources.Load<UIScreen>(text + screenName);
		if (uIScreen == null)
		{
			SDebug.LogError("Attempted to load invalid screenName: " + screenName);
			return null;
		}
		UIScreen uIScreen2 = UnityEngine.Object.Instantiate(uIScreen, destinationParent.transform);
		uIScreen2.transform.Reset();
		uIScreen2.name = uIScreen2.name.Replace("(Clone)", string.Empty);
		uIScreen2.screenName = screenName;
		_cachedScreens.Add(screenName, uIScreen2);
		uIScreen2.InitializeScreen();
		return uIScreen2;
	}

	public static bool ShouldShowOnlineScreen()
	{
		if (Globals.DEBUG_USE_DEBUG_LOGIN_STATE)
		{
			return true;
		}
		return SocialManager.instance.facebookIsLoggedIn || SocialManager.instance.facebookState == SocialManager.FacebookState.LoggingIn || SocialManagerIOS.instance.gameCenterState != SocialManagerIOS.GCState.LoggedOut;
	}

	private string Peek(List<string> list)
	{
		if (list.Count > 0)
		{
			return list[list.Count - 1];
		}
		return string.Empty;
	}

	private string Pop(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
		}
		return result;
	}

	private string QueuePeek(List<string> list)
	{
		string result = string.Empty;
		if (list != null && list.Count > 0)
		{
			result = list[0];
		}
		return result;
	}

	private string Dequeue(List<string> list)
	{
		string result = string.Empty;
		if (list.Count > 0)
		{
			result = list[0];
			list.RemoveAt(0);
		}
		return result;
	}

	public void ShowUnlockAnimationForCharacter(Characters.CharacterId ctype, int themeIndex = 0)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
		celebrationReward.rewardType = CelebrationRewardType.character;
		celebrationReward.characterId = ctype;
		celebrationReward.characterOutfitIndex = themeIndex;
		RewardManager.AddRewardToUnlock(celebrationReward);
		PushCelebrationPopup();
	}

	public void ShowUnlockAnimationForHoverboard(Boards.BoardId boardId, Hoverboard.BoardAbility boardPower = Hoverboard.BoardAbility.None)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.BoardUnlock;
		celebrationReward.rewardType = CelebrationRewardType.specialBoard;
		celebrationReward.boardId = boardId;
		celebrationReward.boardPower = boardPower;
		RewardManager.AddRewardToUnlock(celebrationReward);
		PushCelebrationPopup();
	}

	public void ShowRewardAnimationForBundlePurchase(List<InAppItemModel.BundleItem> bundleContent, string kilooAnalyticsSourceId)
	{
		foreach (InAppItemModel.BundleItem item in bundleContent)
		{
			CelebrationReward celebrationReward = new CelebrationReward();
			celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.BundleUnlock;
			celebrationReward.kilooAnalyticsSourceId = kilooAnalyticsSourceId;
			bool flag = true;
			switch (item.type)
			{
			case InAppItemModel.ItemType.Board:
				if (item.boardId == Boards.BoardId.starterPackBoard01)
				{
					celebrationReward.boardId = item.boardId;
					celebrationReward.rewardType = CelebrationRewardType.welcomePackBoard;
				}
				break;
			case InAppItemModel.ItemType.Coins:
				celebrationReward.rewardType = CelebrationRewardType.coins;
				celebrationReward.amount = item.amount;
				break;
			case InAppItemModel.ItemType.Keys:
				celebrationReward.rewardType = CelebrationRewardType.keys;
				celebrationReward.amount = item.amount;
				break;
			case InAppItemModel.ItemType.Hoverboards:
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.upgradeType = UpgradeType.hoverboard;
				celebrationReward.amount = item.amount;
				break;
			case InAppItemModel.ItemType.Headstarts:
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.upgradeType = UpgradeType.headstart2000;
				celebrationReward.amount = item.amount;
				break;
			case InAppItemModel.ItemType.Scoreboosters:
				celebrationReward.rewardType = CelebrationRewardType.powerup;
				celebrationReward.upgradeType = UpgradeType.scorebooster;
				celebrationReward.amount = item.amount;
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				RewardManager.AddRewardToUnlock(celebrationReward);
			}
		}
		RewardManager.canShowMultipleQueuedCelebrations = true;
		PushPopup("CelebrationPopup");
	}

	public void ShowUnlockAnimationForHoverboardBundle(Boards.BoardId boardId, List<HoverboardThemes.ThemeIdentifier> bundledThemes)
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.BoardBundleUnlock;
		celebrationReward.rewardType = CelebrationRewardType.boardBundle;
		celebrationReward.boardId = boardId;
		celebrationReward.bundledThemes = bundledThemes;
		RewardManager.AddRewardToUnlock(celebrationReward);
		PushCelebrationPopup();
	}

	private void ClearPopupsAndScreens()
	{
		for (int num = _popupQueue.Count - 1; num >= 0; num--)
		{
			if (!_excludedScreensFromClear.Contains(_popupQueue[num]))
			{
				RemovePopup(_popupQueue[num]);
			}
		}
		Dictionary<string, UIScreen> dictionary = new Dictionary<string, UIScreen>();
		foreach (string key in _cachedScreens.Keys)
		{
			UIScreen uIScreen = _cachedScreens[key];
			if (!_excludedScreensFromClear.Contains(key))
			{
				uIScreen.HideIfActive();
				UnityEngine.Object.Destroy(uIScreen.gameObject);
			}
			else
			{
				dictionary.Add(key, uIScreen);
			}
		}
		_cachedScreens = dictionary;
		_screenStack.Clear();
		_delayedPopups.Clear();
	}

	private void MemoryWarningClearUnusedPopupsAndScreens()
	{
		Dictionary<string, UIScreen> dictionary = new Dictionary<string, UIScreen>();
		foreach (string key in _cachedScreens.Keys)
		{
			UIScreen uIScreen = _cachedScreens[key];
			if (!_excludedScreensFromMemoryCleanup.Contains(key) && !uIScreen.isActive)
			{
				uIScreen.HideIfActive();
				UnityEngine.Object.Destroy(uIScreen.gameObject);
			}
			else
			{
				dictionary.Add(key, uIScreen);
			}
		}
		_cachedScreens = dictionary;
		_screenStack.Clear();
		_delayedPopups.Clear();
	}

	private void ClearSlideIns()
	{
		_slideInQueue.Clear();
		_slideInActive = false;
		foreach (UISlideIn allSlideIn in _allSlideIns)
		{
			allSlideIn.gameObject.Kill();
		}
		_slideHasPreloaded = false;
		_allSlideIns.Clear();
	}

	public string GetCurrentPopupName()
	{
		return QueuePeek(_popupQueue);
	}

	public bool IsPopupQueueEmpty()
	{
		return _popupQueue.Count <= 0;
	}

	public bool IsPopupInQueue(string popupName)
	{
		return _popupQueue.Any((string pName) => pName == popupName);
	}

	public bool IsPopupInQueue(params string[] popupNames)
	{
		return _popupQueue.Any((string pName) => popupNames.ToList().Any((string n) => n == pName));
	}

	public void PushPopup(GameObject sender)
	{
		string screenOrPopupName = sender.GetComponent<UIButtonChangeScreen>().GetScreenOrPopupName();
		PushPopup(screenOrPopupName);
	}

	public void PushPopup(string popupName)
	{
		int num = -1;
		string text = QueuePeek(_popupQueue);
		if (!string.IsNullOrEmpty(text) && _cachedScreens.ContainsKey(text))
		{
			UIPanel[] componentsInChildren = _cachedScreens[text].GetComponentsInChildren<UIPanel>(true);
			UIPanel[] array = componentsInChildren;
			foreach (UIPanel uIPanel in array)
			{
				if (uIPanel.depth > num)
				{
					num = uIPanel.depth;
				}
			}
		}
		_popupQueue.Insert(0, popupName);
		_ActivateNextPopup(text);
		if (!_cachedScreens.ContainsKey(popupName))
		{
			return;
		}
		UIPanel[] componentsInChildren2 = _cachedScreens[popupName].GetComponentsInChildren<UIPanel>(true);
		int num2 = -1;
		if (componentsInChildren2 == null || componentsInChildren2.Length <= 0)
		{
			return;
		}
		UIPanel componentInChildren = _cachedScreens[popupName].GetComponentInChildren<UIPanel>();
		if (componentInChildren != null && num > -1)
		{
			num2 = num + 1 - componentInChildren.depth;
		}
		if (num2 > -1)
		{
			UIPanel[] array2 = componentsInChildren2;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].depth += num2;
			}
		}
	}

	public void QueuePopup(string popupName)
	{
		messageHelper.DisableShowLabel();
		_popupQueue.Add(popupName);
		if (!_popupActive)
		{
			_ActivateNextPopup(string.Empty);
		}
	}

	private void _ActivateNextPopup(string oldPopupName = "")
	{
		string empty = string.Empty;
		if (_popupQueue.Count > 0)
		{
			string text = QueuePeek(_popupQueue);
			bool flag = false;
			foreach (string item in _popupQueue)
			{
				if (!_cachedScreens.ContainsKey(item))
				{
					continue;
				}
				UIScreen uIScreen = _cachedScreens[item];
				if (uIScreen.isActive)
				{
					flag = true;
					if (item != text)
					{
						uIScreen.LoseFocus();
						continue;
					}
					uIScreen.GainFocus();
					topBar.ShowTopBar(uIScreen.ShouldShowTopBar());
				}
			}
			ActivateModelAnchors(false);
			string topScreenName = GetTopScreenName();
			if (!flag && !string.IsNullOrEmpty(topScreenName) && _cachedScreens.ContainsKey(topScreenName))
			{
				UIScreen uIScreen2 = _cachedScreens[topScreenName];
				if (uIScreen2.isActive)
				{
					uIScreen2.LoseFocus();
				}
			}
			if (!_cachedScreens.ContainsKey(text))
			{
				UIScreen uIScreen3 = _LoadScreenToCache(text, _popupLayer);
				if (uIScreen3 == null)
				{
					return;
				}
			}
			if (text == "CelebrationPopup" && _screenStack.Count > 0 && _screenStack[_screenStack.Count - 1] == "GameOverScreen")
			{
				RewardManager.canShowMultipleQueuedCelebrations = true;
			}
			UIScreen uIScreen4 = _cachedScreens[text];
			if (uIScreen4 != null && !uIScreen4.isActive)
			{
				if (oldPopupName != string.Empty)
				{
					uIScreen4.parentScreen = oldPopupName;
				}
				topBar.ShowTopBar(uIScreen4.ShouldShowTopBar());
				uIScreen4.ShowIfInactive();
			}
			_popupActive = true;
			OnChangedScreen__BackingField.SafeInvoke(text);
			empty = text;
		}
		else
		{
			ActivateModelAnchors(true);
			empty = string.Empty;
		}
		OnPopupShown__BackingField.SafeInvoke(empty);
	}

	private void ActivateModelAnchors(bool active)
	{
		NGUITools.SetActive(UIModelController.Instance.CharacterAnchor, active, false);
		NGUITools.SetActive(UIModelController.Instance.GameOverAnchor, active, false);
		NGUITools.SetActive(UIModelController.Instance.CelebrationPopupAnchor, active, false);
		NGUITools.SetActive(UIModelController.Instance.PauseScreenAnchor, active, false);
		_PauseAnimations(!active, MenuElements3D.transform);
	}

	public static void QueueFacebookPayoutPopup()
	{
		_facebookPayoutPopupQueued = true;
		if (IsInstanced)
		{
			Instance.TryQueuePayoutPopups();
		}
	}

	private void TryQueuePayoutPopups()
	{
		if (_facebookPayoutPopupQueued && _screenStack != null && _screenStack.Count > 0)
		{
			string item = Peek(_screenStack);
			if (!PAYOUT_DISALLOWED_SCREENS.Contains(item))
			{
				FacebookRewardsInfo.PayoutRewardIfPossible();
				_facebookPayoutPopupQueued = false;
			}
		}
	}

	public void QueuePopupRightAfterCurrentActivePopup(string popupName)
	{
		string value = QueuePeek(_popupQueue);
		if (!string.IsNullOrEmpty(value))
		{
			_popupQueue.Insert(1, popupName);
		}
		else
		{
			QueuePopup(popupName);
		}
	}

	public void QueuePopup(GameObject sender)
	{
		string screenOrPopupName = sender.GetComponent<UIButtonChangeScreen>().GetScreenOrPopupName();
		QueuePopup(screenOrPopupName);
	}

	public void PushCelebrationPopup()
	{
		CelebrationReward[] rewardsToUnlockForCelebration = RewardManager.GetRewardsToUnlockForCelebration();
		if (rewardsToUnlockForCelebration.Length == 0)
		{
			return;
		}
		string empty = string.Empty;
		if (_popupQueue.Count > 0)
		{
			empty = QueuePeek(_popupQueue);
			if (empty == "CelebrationPopup")
			{
				return;
			}
		}
		if (_popupQueue.Count > 0)
		{
			PushPopup("CelebrationPopup");
		}
		else
		{
			QueuePopup("CelebrationPopup");
		}
	}

	public void CloseAllPopups()
	{
		List<string> list = new List<string>();
		foreach (string item in _popupQueue)
		{
			list.Add(item);
		}
		list.ForEach(RemovePopup);
	}

	public void CloseActivePopups(string popupToStopAt)
	{
		List<string> list = new List<string>();
		if (_popupQueue.Contains(popupToStopAt))
		{
			foreach (string item in _popupQueue)
			{
				if (!_cachedScreens.ContainsKey(item))
				{
					continue;
				}
				UIScreen uIScreen = _cachedScreens[item];
				if (uIScreen.isActive)
				{
					list.Add(item);
					if (item == popupToStopAt)
					{
						break;
					}
				}
			}
		}
		foreach (string item2 in list)
		{
			RemovePopup(item2);
		}
	}

	public void ClosePopup()
	{
		RemovePopup(string.Empty);
	}

	[Obsolete("Use ClosePopup() without GameObject parameter.")]
	public void ClosePopup(GameObject go)
	{
		RemovePopup(string.Empty);
	}

	public void ClosePopup(string screenName)
	{
		RemovePopup(screenName);
	}

	public void DebugCloseAllPopups()
	{
		for (int num = _popupQueue.Count - 1; num >= 0; num--)
		{
			RemovePopup(_popupQueue[num]);
		}
	}

	private void _PauseAnimations(bool pause, Transform t)
	{
		foreach (Transform item in t)
		{
			_PauseAnimations(pause, item);
		}
		if (t.GetComponent<CharacterModel>() != null)
		{
			if (pause)
			{
				t.GetComponent<CharacterModel>().StopIdleAnimations();
			}
			else if (Peek(_screenStack) == "BoardScreen")
			{
				HoverboardModelPreviewFactory.Instance.ResetTeleportFX();
			}
			else if (t.gameObject.activeInHierarchy)
			{
				t.GetComponent<CharacterModel>().StartIdleAnimations(UIModelController.ModelScreen.Character);
			}
		}
	}

	public void SaveAndDelayPopups()
	{
		doSaveAndDelayRemainingPopups = false;
		foreach (string item in _popupQueue)
		{
			_delayedPopups.Add(item);
		}
		_popupQueue.Clear();
	}

	public void RequeueDelayedPopups()
	{
		foreach (string delayedPopup in _delayedPopups)
		{
			QueuePopup(delayedPopup);
		}
		_delayedPopups.Clear();
	}

	private void RemovePopup(string popupName = "")
	{
		if (_popupQueue.Count < 1)
		{
			return;
		}
		if (popupName == string.Empty)
		{
			popupName = Dequeue(_popupQueue);
		}
		else
		{
			_popupQueue.Remove(popupName);
		}
		UIScreen value;
		if (_cachedScreens.TryGetValue(popupName, out value))
		{
			value.HideIfActive();
			value.parentScreen = string.Empty;
		}
		OnPopupClosed__BackingField.SafeInvoke(popupName);
		_popupActive = false;
		if (_popupQueue.Count == 0)
		{
			string topScreenName = GetTopScreenName();
			OnChangedScreen__BackingField.SafeInvoke(topScreenName);
			if (!string.IsNullOrEmpty(topScreenName) && _cachedScreens.ContainsKey(topScreenName))
			{
				UIScreen uIScreen = _cachedScreens[topScreenName];
				uIScreen.GainFocus();
				topBar.ShowTopBar(uIScreen.ShouldShowTopBar());
			}
		}
		if (doSaveAndDelayRemainingPopups)
		{
			SaveAndDelayPopups();
		}
		_ActivateNextPopup(string.Empty);
		if (popupName == "CelebrationPopup")
		{
			if (_screenStack.Count > 0 && GetTopScreenName() != "GameOverScreen")
			{
				_cachedScreens[_screenStack[_screenStack.Count - 1]].ShowIfInactive();
				string currentPopupName = GetCurrentPopupName();
				if (isShowingPopup && (currentPopupName == "StorePopup" || currentPopupName == "NotEnoughCurrencyPopup"))
				{
					ActivateModelAnchors(false);
				}
			}
			if (GetTopScreenName() == "GameOverScreen")
			{
				_cachedScreens["GameOverScreen"].GetComponent<GameOverScreen>().SetupAfterMysteryBox();
			}
		}
		if (_popupQueue.Count == 0 && OnLastPopupClosed__BackingField != null)
		{
			if (!ignoreLastPopupClosed)
			{
				OnLastPopupClosed__BackingField();
			}
			else
			{
				ignoreLastPopupClosed = false;
			}
		}
	}

	public int GetAmountOfCachedScreens()
	{
		int result = 0;
		if (_cachedScreens != null)
		{
			result = _cachedScreens.Count;
		}
		return result;
	}

	public void SpawnCollectText(Vector3 startPosition, string text)
	{
		UILabel uILabel = _overlayLayer.AddWidget<UILabel>();
		Utility.SetLayerRecursively(uILabel.gameObject.transform, _overlayLayer.layer);
		uILabel.text = text;
		uILabel.transform.position = new Vector3(startPosition.x, startPosition.y, uILabel.cachedTransform.position.z);
		uILabel.trueTypeFont = FloatingTextFont;
		uILabel.fontSize = 20;
		uILabel.supportEncoding = false;
		uILabel.multiLine = false;
		uILabel.keepCrispWhenShrunk = UILabel.Crispness.Always;
		uILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
		uILabel.MakePixelPerfect();
		uILabel.color = new Color(250f / 255f, 198f / 255f, 60f / 255f, 0f);
		uILabel.gameObject.AddComponent<UIPanel>().depth = 11;
		StartCoroutine(AnimateCollectText(uILabel));
	}

	private IEnumerator AnimateCollectText(UILabel collectText)
	{
		Vector3 fromLocalPosition = collectText.transform.localPosition;
		Vector3 toLocalPosition = new Vector3(fromLocalPosition.x, fromLocalPosition.y + 50f, fromLocalPosition.z);
		yield return StartCoroutine(AnimateAlpha(collectText, 0.1f, 1f));
		StartCoroutine(MoveTransform(collectText.cachedTransform, 1f, toLocalPosition));
		yield return new WaitForSeconds(0.8f);
		StartCoroutine(AnimateAlpha(collectText, 0.2f, 0f));
		yield return new WaitForSeconds(0.25f);
		UnityEngine.Object.Destroy(collectText.gameObject);
	}

	private IEnumerator AnimateAlpha(UILabel label, float duration, float toAlpha)
	{
		float fromAlpha = label.alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			label.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
	}

	private IEnumerator MoveTransform(Transform trans, float duration, Vector3 toPos)
	{
		Vector3 fromPos = trans.localPosition;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			trans.localPosition = Vector3.Lerp(fromPos, toPos, factor);
			yield return null;
		}
	}

	public void SetCoinRewardAmount(int coins)
	{
		_missionSetSlideIn.NewCoinAmount(coins);
	}

	private void PreloadAllSlideIn()
	{
		if (_slideHasPreloaded)
		{
			return;
		}
		_slideHasPreloaded = true;
		_missionSlideIn = InstantiateSlideIn<UISlideInMissionHelper>();
		_missionSetSlideIn = InstantiateSlideIn<UISlideInMissionSetHelper>();
		_lettersSlideIn = InstantiateSlideIn<UISlideInLettersHelper>();
		_unlockSlideIn = InstantiateSlideIn<UISlideInUnlock>();
		_errorSlideIn = InstantiateSlideIn<UISlideInErrorMessage>();
		_weeklyHuntSlideIn = InstantiateSlideIn<UISlideInItemHunt>();
		_bragSlideIn = InstantiateSlideIn<UISlideInBrag>();
		_slideInText = InstantiateSlideIn<UISlideInText>();
		_videoPayoutSlideIn = InstantiateSlideIn<UISlideInVideoPayoutHelper>();
		_offerWallSlideIn = InstantiateSlideIn<UISlideInOfferPayoutHelper>();
		_awardsSlideIn = InstantiateSlideIn<UISlideInAwards>();
		_facebookSlideIn = InstantiateSlideIn<UISlideInFacebook>();
		_topRunTimerSlideIn = InstantiateSlideIn<UISlideInTopRunTimer>();
		_characterEventSlideIn = InstantiateSlideIn<UISlideInCharacterEventHelper>();
		_characterEventRewardSlideIn = InstantiateSlideIn<UISlideInCharacterEventRewardHelper>();
		_adsRemovedSlideIn = InstantiateSlideIn<UISlideInAdsRemoved>();
		_giftSlideIn = InstantiateSlideIn<UISlideInGift>();
		_firstPurchaseBonusSlideIn = InstantiateSlideIn<UiSlideInFirstPurchaseBonus>();
		_cloudSaveSlideIn = InstantiateSlideIn<UISlideInCloudSave>();
		_cloudSaveMessageSlideIn = InstantiateSlideIn<UISlideInCloudSaveMessage>();
		foreach (UISlideIn allSlideIn in _allSlideIns)
		{
			allSlideIn.StartCoroutine(allSlideIn.PreloadSlideIn());
		}
	}

	private T InstantiateSlideIn<T>() where T : UISlideIn
	{
		T val = SlideInData.instance.InstantiateSlideIn<T>(_overlayLayer.transform);
		_allSlideIns.Add(val);
		return val;
	}

	private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
	{
		InAppItemModel inAppItemModel = coinAndKeyOffers.Find((InAppItemModel x) => x.itemId == productId);
		if (inAppItemModel != null)
		{
			if (productId.Contains("coins"))
			{
				_instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.OfferWall,
					stringValue = inAppItemModel.payoutAmount.ToString(),
					purchase = true
				});
			}
			if (productId.Contains("key"))
			{
				_instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.OfferWall,
					stringValue = inAppItemModel.payoutAmount.ToString(),
					iconName = "icon_key_cloud",
					purchase = true
				});
			}
		}
	}

	private void OnMissionCompleted(string message, int missionIndex)
	{
		if (GetTopScreenName() != "ChallengesScreen" && GetTopScreenName() != "PauseUI" && GetCurrentPopupName() != "DebugSettingsPopup")
		{
			QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.Mission,
				stringValue = message
			});
		}
	}

	private void OnMissionSetCompleted()
	{
		MissionsManager.Instance.PlayerDidThis(MissionTarget.ReachMissionSet);
		if (GetTopScreenName() != "ChallengesScreen" && GetTopScreenName() != "PauseUI" && GetCurrentPopupName() != "DebugSettingsPopup")
		{
			QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.MissionSet
			});
		}
	}

	private void OnLetterPickedUp()
	{
		QueueSlideIn(new SlideIn
		{
			type = SlideInData.SlideInType.Letters
		});
	}

	private void OnCharacterUnlocked(Characters.CharacterId id, bool unlockedByRestore)
	{
		if (!unlockedByRestore)
		{
			if (Characters.characterData.ContainsKey(id) && Characters.characterData[id].unlockType == PurchaseData.UnlockType.Tokens)
			{
				RewardManager.canShowMultipleQueuedCelebrations = true;
			}
			else
			{
				RewardManager.canShowMultipleQueuedCelebrations = false;
			}
			ShowUnlockAnimationForCharacter(id);
		}
	}

	private void OnOutfitUnlocked(Characters.CharacterId id, int outfitIndex)
	{
		Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(id, outfitIndex);
		if (outfitForCharacter != null && outfitForCharacter.unlockType == PurchaseData.UnlockType.Tokens)
		{
			RewardManager.canShowMultipleQueuedCelebrations = true;
		}
		else
		{
			RewardManager.canShowMultipleQueuedCelebrations = false;
		}
		ShowUnlockAnimationForCharacter(id, outfitIndex);
	}

	private void OnWillNavigateToScreenHandler(string screen)
	{
		bool flag = screen == "IngameUI";
		SetMenuCamerasEnabled(!flag);
	}

	private void SetMenuCamerasEnabled(bool state)
	{
		Camera3D.enabled = state;
		Camera3DClip.enabled = state;
		Camera3DOverlay.enabled = state;
		CameraOverlay2dClipping.enabled = state;
	}

	public void QueueErrorMessageSlidein(string message)
	{
		QueueSlideIn(new SlideIn
		{
			type = SlideInData.SlideInType.ErrorMessage,
			stringValue = message
		});
	}

	public void QueueSlideIn(SlideIn slideIn)
	{
		if (!AgeRestriction.waitForAgeRestriction && !UniqueAlreadyInQueue(slideIn.type))
		{
			_slideInQueue.Add(slideIn);
			if (!_slideInActive && _allowShowingSlideIns)
			{
				ShowSlideIn();
			}
		}
	}

	private bool UniqueAlreadyInQueue(SlideInData.SlideInType type)
	{
		return _uniqueSlideInTypes.Contains(type) && _slideInQueue.Any((SlideIn s) => s.type == type);
	}

	public void ReadyForNextSlide()
	{
		_slideInActive = false;
		ShowSlideIn();
	}

	private void ShowSlideIn()
	{
		if (_slideInQueue.Count > 0)
		{
			SlideIn slideIn = _slideInQueue[0];
			_slideInQueue.RemoveAt(0);
			GetCachedSlideIn(slideIn.type).SetupSlideIn(slideIn);
			So.Instance.playSound(SlideInData.instance.GetSlideInSound(slideIn));
			_slideInActive = true;
		}
	}

	private UISlideIn GetCachedSlideIn(SlideInData.SlideInType slideType)
	{
		switch (slideType)
		{
		case SlideInData.SlideInType.CharacterEventWordReward:
			return _characterEventRewardSlideIn;
		case SlideInData.SlideInType.CharacterEventWord:
			return _characterEventSlideIn;
		case SlideInData.SlideInType.TopRunTimer:
			return _topRunTimerSlideIn;
		case SlideInData.SlideInType.Mission:
			return _missionSlideIn;
		case SlideInData.SlideInType.MissionSet:
		case SlideInData.SlideInType.LettersCompleteMysteryBox:
		case SlideInData.SlideInType.LettersCompleteCoins:
			return _missionSetSlideIn;
		case SlideInData.SlideInType.Letters:
			return _lettersSlideIn;
		case SlideInData.SlideInType.Unlock:
			return _unlockSlideIn;
		case SlideInData.SlideInType.ErrorMessage:
			return _errorSlideIn;
		case SlideInData.SlideInType.OfferWall:
			return _offerWallSlideIn;
		case SlideInData.SlideInType.WeeklyHunt:
			return _weeklyHuntSlideIn;
		case SlideInData.SlideInType.Brag:
			return _bragSlideIn;
		case SlideInData.SlideInType.Text:
			return _slideInText;
		case SlideInData.SlideInType.VideoPayout:
			return _videoPayoutSlideIn;
		case SlideInData.SlideInType.Awards:
			return _awardsSlideIn;
		case SlideInData.SlideInType.Facebook:
			return _facebookSlideIn;
		case SlideInData.SlideInType.RemoveAds:
			return _adsRemovedSlideIn;
		case SlideInData.SlideInType.Gift:
			return _giftSlideIn;
		case SlideInData.SlideInType.FirstPurchaseBonus:
			return _firstPurchaseBonusSlideIn;
		case SlideInData.SlideInType.CloudSave:
			return _cloudSaveSlideIn;
		case SlideInData.SlideInType.CloudSaveMessage:
			return _cloudSaveMessageSlideIn;
		default:
			return null;
		}
	}

	public void ReadyForNextMessage()
	{
		_messageShowing = false;
		_ShowNextMessage();
	}

	private void _QueueMessage(string message)
	{
		_messageQueue.Enqueue(message);
		if (!_messageShowing)
		{
			_ShowNextMessage();
		}
		if (!_slideInActive)
		{
			ShowSlideIn();
		}
	}

	private void _ShowNextMessage()
	{
		if (_messageQueue.Count > 0)
		{
			string message = _messageQueue.Dequeue();
			messageHelper.ShowMessage(message);
			_messageShowing = true;
		}
	}

	public void ShowInAppPurchaseOverlay(string overlayText)
	{
		if (inAppPurchaseOverlay != null)
		{
			InAppPurchaseOverlay component = inAppPurchaseOverlay.GetComponent<InAppPurchaseOverlay>();
			if (component != null)
			{
				component.UpdateLabel(overlayText);
			}
			inAppPurchaseOverlay.SetActive(true);
			if (!Game.Instance.IsInGame.Value)
			{
				Camera3D.enabled = false;
			}
		}
	}

	public void HideInAppPurchaseOverlay()
	{
		inAppPurchaseOverlay.SetActive(false);
		if (!Game.Instance.IsInGame.Value)
		{
			Camera3D.enabled = true;
		}
	}

	public void DelayedHideInAppPurchaseOverlay()
	{
		if (inAppPurchaseOverlay.activeSelf)
		{
			CoroutineUtil.StartCoroutine(HideInAppPurchaceOverlayDelayedCoroutine());
		}
	}

	private IEnumerator HideInAppPurchaceOverlayDelayedCoroutine()
	{
		yield return new WaitForSecondsRealtime(2f);
		HideInAppPurchaseOverlay();
	}

	public bool IsInAppPurchaseOverlayVisible()
	{
		return inAppPurchaseOverlay.activeInHierarchy;
	}

	public void ShowCloudSaveOverlay(string overlayText)
	{
		if (!(cloudSaveOverlay == null))
		{
			CloudSaveOverlay component = cloudSaveOverlay.GetComponent<CloudSaveOverlay>();
			if (component != null)
			{
				component.UpdateLabel(overlayText);
			}
			cloudSaveOverlay.SetActive(true);
			Kiloo.Common.Timer.Timer.ExecuteWithDelay(20f, () =>
			{
				cloudSaveOverlay.SetActive(false);
			});
		}
	}

	public void HideCloudSaveOverlay()
	{
		if (!(cloudSaveOverlay == null))
		{
			cloudSaveOverlay.SetActive(false);
		}
	}

	public bool IsCloudSaveOverlayVisible()
	{
		if (cloudSaveOverlay == null)
		{
			return false;
		}
		return cloudSaveOverlay.activeInHierarchy;
	}

	public void ShowAdsOverlay()
	{
		adsOverlay.SetActive(true);
	}

	public void HideAdsOverlay()
	{
		StartCoroutine(HideAdsOverlayWithDelay(0.3f));
		StartCoroutine(HideAdsOverlayWithDelay(0.3f));
	}

	private IEnumerator HideAdsOverlayWithDelay(float delay)
	{
		yield return CoroutineUtil.WaitForRealSeconds(delay);
		adsOverlay.SetActive(false);
	}

	public bool IsAdsOverlayVisible()
	{
		return adsOverlay.activeInHierarchy;
	}

	public void ShowTransitionAdsOverlay()
	{
		transitionAdsOverlay.SetActive(true);
		CoroutineUtil.StopCoroutine(_transitionAdsOverlayCoroutine);
		_transitionAdsOverlayCoroutine = HideTransitionAdsOverlayWithDelay(0.5f);
		StartCoroutine(_transitionAdsOverlayCoroutine);
	}

	private IEnumerator HideTransitionAdsOverlayWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		transitionAdsOverlay.SetActive(false);
	}

	public void ShowDiskSpaceErrorOverlay()
	{
		videoAdsOverlay.SetActive(true);
		_allowInputOnVideoAdsOverlay = false;
	}

	public void ShowVideoAdsOverlay(AdCacheSlotInfo adCacheSlotInfo)
	{
		_provider = AdSystemManager.GetLastShowedProviderForCacheSlot(_adCacheSlotInfo);
		SetupVideoAdsOverlay();
	}

	public void ShowVideoAdsTransitionOverlay(AdLocationInfo adLocationInfo)
	{
		_adLocationInfo = adLocationInfo;
		SetupVideoAdsOverlay();
	}

	public void SetupVideoAdsOverlay()
	{
		videoAdsOverlay.SetActive(true);
		_allowInputOnVideoAdsOverlay = false;
		videoAdsOverlayCloseButton.SetActive(false);
		videoAdsOverlayLabel.text = Strings.Get("AD_OVERLAY_LOADING");
		CoroutineUtil.StopCoroutine(_videoAdsOverlayInputCoroutine);
		_videoAdsOverlayInputCoroutine = DelayInputOnOverlay();
		CoroutineUtil.StartCoroutine(_videoAdsOverlayInputCoroutine);
	}

	public void HideVideoAdsOverlay(bool forceHide = false)
	{
		if (_allowInputOnVideoAdsOverlay || Globals.DEBUG_SKIP_VIDEO_ADS || forceHide)
		{
			videoAdsOverlayCloseButton.SetActive(false);
			videoAdsOverlay.SetActive(false);
		}
		if (_allowInputOnVideoAdsOverlay && !forceHide)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Provider", _provider);
			dictionary.Add("Location", _adLocationInfo.id);
			Flurry.LogEventWithSeveralParameters("Manually closed ad overlay", dictionary);
			HouseKeeper.TryRestoreSoundOptionAfterLostFocusIfNeeded();
			if (OnOverlayManuallyClosed__BackingField != null)
			{
				OnOverlayManuallyClosed__BackingField();
			}
		}
	}

	private IEnumerator DelayInputOnOverlay()
	{
		yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(4f));
		_allowInputOnVideoAdsOverlay = true;
		videoAdsOverlayCloseButton.SetActive(true);
		videoAdsOverlayLabel.text = Strings.Get("AD_OVERLAY_CANCEL");
	}

	public bool IsVideoAdsOverlayVisible()
	{
		return videoAdsOverlay.activeInHierarchy;
	}

	public bool IsAnyOverlayActive()
	{
		if (transitionAdsOverlay != null && transitionAdsOverlay.activeInHierarchy)
		{
			return true;
		}
		if (videoAdsOverlay != null && videoAdsOverlay.activeInHierarchy)
		{
			return true;
		}
		if (adsOverlay != null && adsOverlay.activeInHierarchy)
		{
			return true;
		}
		if (inAppPurchaseOverlay != null && inAppPurchaseOverlay.activeInHierarchy)
		{
			return true;
		}
		if (IsCloudSaveOverlayVisible())
		{
			return true;
		}
		if (OfferWallManager.instance != null && OfferWallManager.isOfferwallActive)
		{
			return true;
		}
		if (SocialManager.instance != null && SocialManager.instance.facebookState == SocialManager.FacebookState.LoggingIn)
		{
			return true;
		}
		return false;
	}

	private void EnableGeneralDebugLabelIfNeeded()
	{
		generalDebugLabel.SetActive((Globals.DEBUG ? true : false) && Globals.DEBUG_ENABLE_WATERMARK);
	}

	public void ToggleDebugLabelIfNeeded()
	{
		generalDebugLabel.SetActive(!generalDebugLabel.activeSelf && Globals.DEBUG && Globals.DEBUG_ENABLE_WATERMARK);
	}

	public void EnableGeneralAdsSystemDisableIndicatorIfNeeded()
	{
		_adsSystemDisableIndicator.SetActive(Globals.DEBUG && DebugSave.disableAdSystem && Globals.DEBUG_ENABLE_WATERMARK);
	}

	public void SetViewportRectXForCamera(Camera camera, float x)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.x = x;
			camera.rect = rect;
		}
	}

	public void SetViewportRectYForCamera(Camera camera, float y)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.y = y;
			camera.rect = rect;
		}
	}

	public void SetViewportRectWidthForCamera(Camera camera, float width)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.width = width;
			camera.rect = rect;
		}
	}

	public void SetViewportRectHeightForCamera(Camera camera, float height)
	{
		if (camera != null)
		{
			Rect rect = camera.rect;
			rect.height = height;
			camera.rect = rect;
		}
	}

	public void RecalculateViewportRectForClipCamera(Camera camera, bool popupSizedClip)
	{
		if (camera != null)
		{
			UICameraScreenClipping component = camera.GetComponent<UICameraScreenClipping>();
			if (component != null)
			{
				component.CalculateClipping(popupSizedClip);
			}
		}
	}

	public void SetCameraToOrtographic(Camera camera, bool isOrtographic)
	{
		if (camera != null)
		{
			camera.orthographic = isOrtographic;
			if (isOrtographic)
			{
				camera.nearClipPlane = -5f;
			}
			else
			{
				camera.nearClipPlane = 0.1f;
			}
		}
	}

	public void CalculateCameraFieldOfView(Camera camera, float designedScreenWidth, float distanceToUI)
	{
		if (camera != null && !camera.orthographic)
		{
			float num = Mathf.Atan(designedScreenWidth / 2f / distanceToUI);
			float num2 = (float)camera.pixelWidth / (float)camera.pixelHeight;
			float fieldOfView = Mathf.Atan(Mathf.Tan(num / 2f) / num2) * 2f * 57.29578f;
			camera.fieldOfView = fieldOfView;
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_SCREENCONTROLLER_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
