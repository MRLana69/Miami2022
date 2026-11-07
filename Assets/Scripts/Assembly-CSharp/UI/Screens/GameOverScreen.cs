using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using InApp;
using Info;
using Info.VideoRewardCooldowns;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Ads;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using Missions;
using NaughtyAttributes;
using SYBO.Subway;
using SYBO.Subway.Info;
using TopRun;
using UI.DoubleCoinsVideo;
using UI.Generic.Layout;
using UI.Screens.RewardManagement;
using UnityEngine;
using Utilities;

namespace UI.Screens
{
	public class GameOverScreen : RunStartScreen
	{
		public enum ActiveTabState
		{
			GameOver = 0,
			LeaderBoard = 1,
			TopRun = 2
		}

		[Header("General")]
		[SerializeField]
		private UIWidget _topContainer;

		[SerializeField]
		private GameObject _tabAreaBackground;

		[Header("GameOver Tab")]
		[SerializeField]
		private UISprite _gameOverTabActive;

		[SerializeField]
		private UISprite _gameOverTabInactive;

		[SerializeField]
		private GameObject _gameOverContent;

		[SerializeField]
		private UIWidget _middleAnchor;

		[SerializeField]
		private UILabel _topScoreLabel;

		[SerializeField]
		private UILabel _scoreLabel;

		[SerializeField]
		private HorizontalLayout _coinsHorizontalLayout;

		[SerializeField]
		private UILabel _coinsLabel;

		[SerializeField]
		private UILabel _collectedCoinLabel;

		[SerializeField]
		private GameObject _characterDayEffects;

		[SerializeField]
		private UI.DoubleCoinsVideo.DoubleCoinsVideo _doubleUpPanel;

		[SerializeField]
		private GameObject _doubleCoinsPanel;

		[SerializeField]
		private UILabel _doubleCoinsButtonLabel;

		[SerializeField]
		private GameObject _doubleCoinsActive;

		[SerializeField]
		private GameObject _doubleCoinsActiveStar;

		private Animation[] _doubleCoinActiveStarElements;

		[SerializeField]
		private UILabel _doubleCoinsActiveLabel;

		[SerializeField]
		private UILabel _doubleCoinsBannerTitle;

		[SerializeField]
		private UIWidget _friendsListContainer;

		[SerializeField]
		private Transform _doubleCoinsBanner;

		[SerializeField]
		private Transform _doubleUpBanner;

		[Header("Leaderboard Tab")]
		[SerializeField]
		private UISprite _leaderboardTabActive;

		[SerializeField]
		private UISprite _leaderboardTabInactive;

		[SerializeField]
		private GameObject _leaderboardNotification;

		[SerializeField]
		private GameObject _leaderboardContent;

		[SerializeField]
		private UIWidget _leaderboardContentPrefab;

		[Header("TopRun Tab")]
		[SerializeField]
		private UISprite _topRunTabActive;

		[SerializeField]
		private UISprite _topRunTabInactive;

		[SerializeField]
		private GameObject _topRunNotification;

		[SerializeField]
		private GameObject _topRunContent;

		[SerializeField]
		private UIWidget _topRunContentPrefab;

		[Header("Footer")]
		[SerializeField]
		private UISprite _playButtonFill;

		[SerializeField]
		private UIButtonOverlayOff _playButtonOverlayOff;

		[SerializeField]
		private UILabel _homeButtonLabel;

		[SerializeField]
		private UILabel _boostsButtonLabel;

		[SerializeField]
		private UILabel _playButtonLabel;

		[SerializeField]
		private BreadCrumbHelper _breadcrumb;

		private const string GAMEOVER_SCREEN_DEBUG_TAG = "[GameOver Screen]: ";

		private const string SCREEN_FLURRY_NAME = "GameoverUI";

		private const uint DEFAULT_COUNTER_EXPIRE_TIME = 72000u;

		private const uint DEFAULT_TOTAL_SHOW_SLOTS = 5u;

		private const uint DEFAULT_ACTIVE_SHOW_SLOT = 1u;

		private const string INTERSTITIAL_GAMEOVER_COUNTER_EXPIRE_TIME = "interstitial_game_over_counter_expire_time";

		private const string INTERSTITIAL_GAMEOVER_TOTAL_SHOW_SLOTS = "interstitial_game_over_total_show_slots";

		private const string INTERSTITIAL_GAMEOVER_ACTIVE_SHOW_SLOT = "interstitial_game_over_active_show_slot";

		private const string RATING_POPUP_ENABLED_ONLINESETTINGS_KEY = "rating_popup_enabled";

		private const bool RATING_POPUP_ENABLED_DEFAULT_VALUE = true;

		private const string RATING_POPUP_RUN_LIMIT_ONLINESETTINGS_KEY = "rating_popup_run_limit";

		private const int RATING_POPUP_RUN_LIMIT_DEFAULT_VALUE = 10;

		private const int STACKED_FRIEND_LIST_Y_POS = 94;

		private const int DEFAULT_FRIEND_LIST_Y_POS = -9;

		private const int GAMEOVER_DOUBLE_UP_VIDEO_MIN_PAYOUT = 500;

		private const float DOUBLE_COIN_STAR_ANIMATION_PAUSE = 4f;

		private const float TALLEST_ASPECT_RATIO = 0.46f;

		private const float WIDEST_ASPECT_RATIO = 0.75f;

		private const float TALLEST_ASPECT_OFFSET = 50f;

		private const float WIDEST_ASPECT_OFFSET = 10f;

		private const string EVENT_ARG_HOME_BUTTON_CLICKED_VALUE = "Home";

		private const string EVENT_ARG_BOOSTS_BUTTON_CLICKED_VALUE = "Boosts";

		private const string EVENT_ARG_PLAY_BUTTON_CLICKED_VALUE = "Play";

		private int _scoreFrom;

		private int _scoreTo;

		private int _coinboxFrom;

		private int _coinboxTo;

		private int _collectedCoinsFrom;

		private int _collectedCoinsTo;

		private int _counter;

		private int _facebookShowCounter = 2;

		private bool _isNewHighscore;

		private bool _hasBeenSetupAfterAGame;

		private bool _countingUpCoins;

		private bool _hasDoubledCoinsThisRun;

		private bool _hasSeenLeaderboardTab;

		private bool _hasSeenTopRunTab;

		private bool _firstBannerShowAttempt;

		private bool _hasAttemptedToShowAdOnTopOfPopup;

		private int _coinsConvertedToScore;

		private DateTime _lastCountUpTimeStamp = DateTime.MinValue;

		private ActiveTabState _initialTab;

		private ActiveTabState _currentActiveTab;

		private ScoreCounterSoundPlayer _scoreCounterSoundPlayer;

		private IEnumerator _countUpCoroutine;

		private IEnumerator _reloadFriendsCoroutine;

		private Dictionary<string, string> _doubleCoinsVideoAttemptParams = new Dictionary<string, string>();

		private Dictionary<string, string> _doubleCoinsVideoResultParams = new Dictionary<string, string>();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onGameOverScreenShown__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onAnchorsUpdated__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<float> onBottomElementsSetup__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<ActiveTabState> onTabChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onCountUpStarted__BackingField;

		private GameOverFriendsListHandler _gameOverFriendsListHandler;

		private IEnumerator _doubleCoinStarCoroutine;

		public ActiveTabState activeTab
		{
			get
			{
				return _currentActiveTab;
			}
		}

		public static event Action onGameOverScreenShown
		{
			add
			{
				Action action = onGameOverScreenShown__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGameOverScreenShown__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onGameOverScreenShown__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGameOverScreenShown__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onAnchorsUpdated
		{
			add
			{
				Action action = onAnchorsUpdated__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAnchorsUpdated__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onAnchorsUpdated__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onAnchorsUpdated__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<float> onBottomElementsSetup
		{
			add
			{
				Action<float> action = onBottomElementsSetup__BackingField;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onBottomElementsSetup__BackingField, (Action<float>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<float> action = onBottomElementsSetup__BackingField;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onBottomElementsSetup__BackingField, (Action<float>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<ActiveTabState> onTabChanged
		{
			add
			{
				Action<ActiveTabState> action = onTabChanged__BackingField;
				Action<ActiveTabState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onTabChanged__BackingField, (Action<ActiveTabState>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<ActiveTabState> action = onTabChanged__BackingField;
				Action<ActiveTabState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onTabChanged__BackingField, (Action<ActiveTabState>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onCountUpStarted
		{
			add
			{
				Action action = onCountUpStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCountUpStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onCountUpStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCountUpStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private void OnEnable()
		{
			_hasAttemptedToShowAdOnTopOfPopup = false;
		}

		protected override void Init()
		{
			_doubleCoinsVideoAttemptParams.Add("HasFill", "N/A");
			_doubleCoinsVideoAttemptParams.Add("IsCharged", "N/A");
			_doubleCoinsVideoAttemptParams.Add("WasBannerShown", "N/A");
			_doubleCoinsVideoResultParams.Add("ElementShown", "N/A");
			_doubleCoinsVideoResultParams.Add("VideoPayoutAmount", "N/A");
			_doubleCoinsVideoResultParams.Add("IsDoubleCoinsVideoClicked", "N/A");
			onTabChanged += OnTabChanged;
			InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
			AdSystemManager.onDismissedVideo += OnVideoClosed;
			_currentActiveTab = _initialTab;
			if (_leaderboardContentPrefab != null)
			{
				UIWidget uIWidget = UnityEngine.Object.Instantiate(_leaderboardContentPrefab);
				uIWidget.transform.parent = _leaderboardContent.transform;
				uIWidget.transform.Reset();
				uIWidget.SetAnchor(_tabAreaBackground);
			}
			if (_topRunContentPrefab != null)
			{
				UIWidget uIWidget2 = UnityEngine.Object.Instantiate(_topRunContentPrefab);
				uIWidget2.transform.parent = _topRunContent.transform;
				uIWidget2.transform.Reset();
				uIWidget2.SetAnchor(_tabAreaBackground);
			}
			_scoreCounterSoundPlayer = GetComponent<ScoreCounterSoundPlayer>();
			_playButtonFill.color = GlobalColors.FOOTER_BIG_PLAY_RESUME_COLOR;
			_collectedCoinLabel.effectColor = ColorData.instance.shadow;
			_homeButtonLabel.text = Strings.Get("FOOTER_HOME_BUTTON_TITLE");
			_boostsButtonLabel.text = Strings.Get("FOOTER_BOOSTS_BUTTON_TITLE");
			_playButtonLabel.text = Strings.Get("GAMEOVER_UI_PLAY");
			_topScoreLabel.text = Strings.Get("GAMEOVER_UI_SCORE");
			_topScoreLabel.color = ColorData.instance.midnight;
			_coinsLabel.text = Strings.Get("GAMEOVER_UI_COINS");
			_coinsLabel.color = ColorData.instance.midnight;
			_doubleCoinsActiveLabel.text = Strings.Get("DOUBLE_COINS_TITLE");
			_doubleCoinsBannerTitle.text = Strings.Get("GAMEOVER_DOUBLE_COINS_BANNER_TITLE");
			_doubleCoinsButtonLabel.text = InAppData.inAppData["com.kiloo.subways.doublecoins"].localizedPrice;
			_doubleCoinActiveStarElements = _doubleCoinsActiveStar.GetComponentsInChildren<Animation>();
			AnimateBannerStars();
			HandleSafeArea();
		}

		private void OnDestroy()
		{
			onTabChanged -= OnTabChanged;
			if (InAppManager.instance != null)
			{
				InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
			}
			AdSystemManager.onDismissedVideo -= OnVideoClosed;
		}

		protected override void Show()
		{
			_gameOverFriendsListHandler = _friendsListContainer.GetComponent<GameOverFriendsListHandler>();
			_gameOverFriendsListHandler.Reset();
			_isNewHighscore = false;
			_firstBannerShowAttempt = true;
			_hasDoubledCoinsThisRun = false;
			UIScreenController.OnPopupShown += OnPopupShown;
			SocialManager.friendsChangedHandler += OnFriendsChanged;
			SSEventsManagerController.OnSSEventStartedHandler += SetUpEventUIElements;
			SSEventsManagerController.OnSSEventFinishedHandler += SetUpEventUIElements;
			UIScreenController.OnApplicationResumed += SetUpEventUIElements;
			UIScreenController.OnLastPopupClosed += OnLastPopupClosed;
			UIScreenController.OnPopupClosed += OnPopupClosed;
			ChangeTab(_initialTab, true);
			if (activeTab == ActiveTabState.GameOver)
			{
				UIModelController.Instance.ActivateGameOverModel();
			}
			UpdateDoubleCoinsActive();
			onGameOverScreenShown__BackingField.SafeInvoke();
			int score = GameStats.Instance.Score;
			_coinsConvertedToScore = GameStats.CoinToScoreConversion(GameStats.Instance.CoinsCollected);
			int num = score + _coinsConvertedToScore;
			_isNewHighscore = num > PlayerSave.highScore;
			ChangePlayButtonState();
			if (_isNewHighscore)
			{
				PlayerSave.SetHighScore(num);
				PushNewHighScoreCelebrationScreen();
				MissionsManager.Instance.PlayerDidThis(MissionTarget.BeatOwnHighscore);
			}
			if (SocialManager.instance != null)
			{
				SocialManager.instance.ReportScore(num, _isNewHighscore);
			}
			IManager manager = TopRunRoot.manager;
			if (manager != null)
			{
				manager.SubmitScore(num, Game.Instance.gameStartTime);
			}
			_breadcrumb.SetupBreadCrumbState();
			HideEventStarsIfNecessary();
			SetUpEventUIElements();
			SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
			UpdateNotificationStickers();
			SaveManager.Save();
		}

		protected override void Hide()
		{
			UIScreenController.OnPopupShown -= OnPopupShown;
			SocialManager.friendsChangedHandler -= OnFriendsChanged;
			SSEventsManagerController.OnSSEventFinishedHandler -= SetUpEventUIElements;
			SSEventsManagerController.OnSSEventStartedHandler -= SetUpEventUIElements;
			UIScreenController.OnApplicationResumed -= SetUpEventUIElements;
			UIScreenController.OnLastPopupClosed -= OnLastPopupClosed;
			UIScreenController.OnPopupClosed -= OnPopupClosed;
			_hasSeenLeaderboardTab = false;
			_hasSeenTopRunTab = false;
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.GameOver);
			Flurry.LogEventWithSeveralParameters("DoubleCoinsVideoResult", _doubleCoinsVideoResultParams);
			UIScreenController.Instance.topBar.SetCoinsLock(false);
			StopDoubleCoinsStarAnimation();
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause && !_countingUpCoins)
			{
				ReloadFriends();
			}
		}

		private void ChangeTab(ActiveTabState newTab, bool overrideChange = false)
		{
			if (_currentActiveTab != newTab || overrideChange)
			{
				if (_currentActiveTab != newTab && !overrideChange)
				{
					Flurry.LogEventWithAParameter("Game Over Screen Tabs", "TabClicked", newTab.ToString());
				}
				_currentActiveTab = newTab;
				onTabChanged__BackingField.SafeInvoke(_currentActiveTab);
			}
		}

		[UsedImplicitly]
		public void GameOverTabTapped()
		{
			ChangeTab(ActiveTabState.GameOver);
		}

		[UsedImplicitly]
		public void LeaderboardTabTapped()
		{
			ChangeTab(ActiveTabState.LeaderBoard);
		}

		[UsedImplicitly]
		public void TopRunTabTapped()
		{
			ChangeTab(ActiveTabState.TopRun);
		}

		private void OnTabChanged(ActiveTabState newTab)
		{
			bool flag = newTab == ActiveTabState.GameOver;
			bool flag2 = newTab == ActiveTabState.LeaderBoard;
			bool flag3 = newTab == ActiveTabState.TopRun;
			_hasSeenLeaderboardTab = _hasSeenLeaderboardTab || flag2;
			_hasSeenTopRunTab = _hasSeenTopRunTab || flag3;
			_gameOverTabActive.gameObject.SetActive(flag);
			_gameOverTabInactive.gameObject.SetActive(!flag);
			_leaderboardTabActive.gameObject.SetActive(flag2);
			_leaderboardTabInactive.gameObject.SetActive(!flag2);
			_topRunTabActive.gameObject.SetActive(flag3);
			_topRunTabInactive.gameObject.SetActive(!flag3);
			_gameOverContent.SetActive(flag);
			_leaderboardContent.SetActive(flag2);
			_topRunContent.SetActive(flag3);
			if (flag)
			{
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterDay);
				if (activeEventManager != null)
				{
					_characterDayEffects.SetActive(ShouldEnableCharacterDayEventElements(activeEventManager));
				}
			}
			else
			{
				_characterDayEffects.SetActive(false);
			}
			if (flag)
			{
				UpdateDoubleCoinsActive();
			}
			else
			{
				StopDoubleCoinsStarAnimation();
			}
			UpdateNotificationStickers();
			if (newTab == ActiveTabState.GameOver)
			{
				UIModelController.Instance.ActivateGameOverModel();
				SetupBottomAndMiddleElements();
			}
			else
			{
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.GameOver);
			}
			if (_friendsListContainer.gameObject.activeSelf)
			{
				ReloadFriends();
			}
		}

		private void UpdateNotificationStickers()
		{
			bool active = activeTab != ActiveTabState.LeaderBoard && (_isNewHighscore || HasBeatenFriend()) && !_hasSeenLeaderboardTab;
			bool active2 = activeTab != ActiveTabState.TopRun && TopRunRewardHelper.ShouldShowResults() && !_hasSeenTopRunTab;
			_leaderboardNotification.SetActive(active);
			_topRunNotification.SetActive(active2);
		}

		private bool HasBeatenFriend()
		{
			Friend[] array = ((!Application.isEditor) ? SocialManager.instance.FriendsSortedByScore() : Globals.GetDebugFriends(50));
			int bragScoreThreshold = PlayerSave.bragScoreThreshold;
			int highScore = PlayerSave.highScore;
			Friend[] array2 = array;
			foreach (Friend friend in array2)
			{
				if (friend.score >= bragScoreThreshold && friend.score < highScore)
				{
					return true;
				}
			}
			return false;
		}

		[UsedImplicitly]
		public void OnDoubleCoinsClicked()
		{
			Flurry.LogEvent("Double Coin button pressed");
			InAppManager.instance.StartPurchase("com.kiloo.subways.doublecoins");
		}

		private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			if (productId == "com.kiloo.subways.doublecoins")
			{
				SetupBottomAndMiddleElements();
			}
		}

		private void OnVideoClosed(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			if (!UIScreenController.Instance.isShowingPopup)
			{
				SetupBottomAndMiddleElements();
			}
		}

		private void OnLastPopupClosed()
		{
			if (activeTab == ActiveTabState.GameOver)
			{
				UIModelController.Instance.ActivateGameOverModel();
			}
			if (_hasAttemptedToShowAdOnTopOfPopup && IsAllowedToShowInterstitial() && AdSystemManager.IsCached(AdLocations.GAME_OVER_INTERSTITIALS))
			{
				AdSystemManager.Show(AdLocations.GAME_OVER_INTERSTITIALS);
			}
		}

		private void OnPopupClosed(string popupName)
		{
			if (UIScreenController.Instance.IsPopupQueueEmpty() && activeTab == ActiveTabState.GameOver)
			{
				if (popupName == "CharacterNameEventPopup")
				{
					UIModelController.Instance.ActivateGameOverModel();
				}
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterDay);
				if (activeEventManager != null)
				{
					_characterDayEffects.SetActive(ShouldEnableCharacterDayEventElements(activeEventManager));
				}
			}
		}

		private void OnFriendsChanged()
		{
			ReloadFriends();
		}

		private void InAppManager_OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			UpdateDoubleCoinsActive();
		}

		[UsedImplicitly]
		public void LogOnHomeClicked()
		{
			Flurry.LogEventWithAParameter("Game Over Screen Buttons", "ButtonClicked", "Home");
		}

		[UsedImplicitly]
		public void LogOnBoostsClicked()
		{
			Flurry.LogEventWithAParameter("Game Over Screen Buttons", "ButtonClicked", "Boosts");
		}

		[UsedImplicitly]
		public void LogOnPlayClicked()
		{
			Flurry.LogEventWithAParameter("Game Over Screen Buttons", "ButtonClicked", "Play");
		}

		private void AnimateBannerStars()
		{
			DoubleCoinsVideoBanner component = _doubleCoinsPanel.GetComponent<DoubleCoinsVideoBanner>();
			if (component != null)
			{
				if (_doubleCoinsPanel.activeInHierarchy)
				{
					component.AnimateStars();
				}
			}
			else
			{
				LogError("DoubleCoins banner script not found. Stars won't be animated.");
			}
		}

		private void UpdateDoubleCoinsActive()
		{
			if (UserSave.hasDoubleCoinsUpgrade)
			{
				_doubleCoinsActive.SetActive(true);
				if (_doubleCoinStarCoroutine != null)
				{
					CoroutineUtil.StopCoroutine(_doubleCoinStarCoroutine);
				}
				_doubleCoinStarCoroutine = PlayDoubleCoinStarAnimations();
				CoroutineUtil.StartCoroutine(_doubleCoinStarCoroutine);
			}
			else
			{
				_doubleCoinsActive.SetActive(false);
			}
		}

		private void StopDoubleCoinsStarAnimation()
		{
			if (UserSave.hasDoubleCoinsUpgrade)
			{
				Animation[] doubleCoinActiveStarElements = _doubleCoinActiveStarElements;
				foreach (Animation animation in doubleCoinActiveStarElements)
				{
					animation.Stop();
				}
				_doubleCoinsActiveStar.SetActive(false);
				if (_doubleCoinStarCoroutine != null)
				{
					CoroutineUtil.StopCoroutine(_doubleCoinStarCoroutine);
				}
			}
		}

		private IEnumerator PlayDoubleCoinStarAnimations()
		{
			yield return new WaitForSeconds(1f);
			_doubleCoinsActiveStar.SetActive(true);
			Animation[] doubleCoinActiveStarElements = _doubleCoinActiveStarElements;
			foreach (Animation animation in doubleCoinActiveStarElements)
			{
				animation.Play();
			}
			yield return new WaitForSeconds(4f);
			_doubleCoinStarCoroutine = PlayDoubleCoinStarAnimations();
			CoroutineUtil.StartCoroutine(_doubleCoinStarCoroutine);
		}

		private void HandleSafeArea()
		{
			if (SafeArea.IsRestricted)
			{
				NGUIMath.MoveWidget(_topContainer, 0f, -SafeArea.OFFSET_TOP);
				_topContainer.ResetAndUpdateAnchors();
				UIWidget component = _tabAreaBackground.GetComponent<UIWidget>();
				component.topAnchor.absolute -= SafeArea.OFFSET_TOP;
				component.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
				_tabAreaBackground.GetComponent<UIWidget>().ResetAndUpdateAnchors();
				onAnchorsUpdated__BackingField.SafeInvoke();
			}
		}

		private bool IsTallAspect()
		{
			float screenAspect = GetScreenAspect();
			return screenAspect < 0.57f;
		}

		private float GetScreenAspect()
		{
			int num = 0;
			if (SafeArea.IsRestricted && UIScreenController.IsInstanced && UIScreenController.Instance != null && UIScreenController.Instance.root != null)
			{
				int manualHeight = UIScreenController.Instance.root.manualHeight;
				float num2 = (float)SafeArea.OFFSET_TOP / (float)manualHeight;
				float num3 = (float)SafeArea.OFFSET_BOTTOM / (float)manualHeight;
				int num4 = ((float)Screen.height * num2).RoundToInt();
				int num5 = ((float)Screen.height * num3).RoundToInt();
				num = num4 + num5;
			}
			return (float)Screen.width / (float)(Screen.height - num);
		}

		private float CalculateModelOffset()
		{
			float num = (GetScreenAspect() - 0.46f) / 0.29f;
			return Mathf.Lerp(10f, 50f, 1f - num);
		}

		private void HideEventStarsIfNecessary()
		{
			if (UIScreenController.Instance.isShowingPopup && _characterDayEffects.activeSelf)
			{
				_characterDayEffects.SetActive(false);
			}
		}

		private void SetupBottomElements()
		{
			bool flag = false;
			if (SetupDoubleUpBanner(GameStats.Instance.CoinsCollected))
			{
				flag = true;
			}
			else if (!UserSave.hasDoubleCoinsUpgrade)
			{
				SetupDoubleCoinsBanner();
				flag = true;
			}
			SetupFriendListIfNecessary(flag);
			float argument = ((!flag) ? CalculateModelOffset() : 0f);
			onBottomElementsSetup__BackingField.SafeInvoke(argument);
		}

		private void SetupMiddleElements()
		{
			Transform target = ((!_friendsListContainer.gameObject.activeInHierarchy) ? ((!_doubleCoinsPanel.gameObject.activeInHierarchy) ? _doubleUpBanner : _doubleCoinsBanner) : _friendsListContainer.transform);
			_middleAnchor.bottomAnchor.target = target;
			_middleAnchor.bottomAnchor.absolute = 0;
			_middleAnchor.ResetAndUpdateAnchors();
		}

		private bool ShouldEnableCharacterDayEventElements(IEventManager activeEventManager)
		{
			bool result = false;
			Characters.CharacterId currentCharacterId = PlayerSave.currentCharacterId;
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterDay)
			{
				bool found;
				Characters.CharacterId eventValue = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
				result = found && currentCharacterId == eventValue;
			}
			return result;
		}

		private void SetUpEventUIElements()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			bool flag = ShouldEnableCharacterDayEventElements(activeEventManager);
			_characterDayEffects.SetActive(flag && !UIScreenController.Instance.isShowingPopup && activeTab == ActiveTabState.GameOver);
		}

		private void ChangePlayButtonState()
		{
			_playButtonLabel.text = Strings.Get("GAMEOVER_UI_PLAY");
			_playButtonOverlayOff.GoToNormalState();
		}

		public void SetupBeforeMysteryBox()
		{
			if (Globals.DEBUG_USE_CELEBRATION_TESTER)
			{
				CelebrationTesterPopup.RewardDebugCelebrationReward();
			}
			_hasBeenSetupAfterAGame = true;
			_scoreLabel.text = string.Empty + GameStats.Instance.Score;
			_scoreFrom = GameStats.Instance.Score;
		}

		public void SetupAfterMysteryBox()
		{
			if (!_hasBeenSetupAfterAGame)
			{
				return;
			}
			int coinsCollected = GameStats.Instance.CoinsCollected;
			_collectedCoinsFrom = 0;
			_collectedCoinsTo = coinsCollected;
			_scoreTo = _scoreFrom + _coinsConvertedToScore;
			_coinboxFrom = InventorySave.coins;
			_coinboxTo = _coinboxFrom + coinsCollected;
			UIScreenController.Instance.topBar.SetCoinsLock(true);
			InventorySave.unrewardedCoins = 0;
			InventorySave.IncreaseCoins(_coinboxTo - _coinboxFrom, "InRun");
			MissionsManager.Instance.PlayerDidThis(MissionTarget.GetExactlyAmountOfCoins, coinsCollected);
			_countUpCoroutine = CountUpCoins();
			StartCoroutine(_countUpCoroutine);
			_hasBeenSetupAfterAGame = false;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterName);
			if (activeEventManager != null)
			{
				CharacterNameEventManager typeOfActiveEventManager = SSEventsManagerController.instance.GetTypeOfActiveEventManager<CharacterNameEventManager>();
				if (typeOfActiveEventManager != null && typeOfActiveEventManager.HasCompletedCollectionForSelectedCharacter())
				{
					UIScreenController.Instance.QueuePopup("CharacterNameEventPopup");
				}
			}
			if (FacebookRewardsInfo.CanPayoutReward())
			{
				FacebookRewardsInfo.PayoutRewardIfPossible();
			}
			else if (FacebookRewardsInfo.ShouldShowConnectPopupOnGameOver(_facebookShowCounter))
			{
				UIScreenController.Instance.QueuePopup("FBPromotionPopup");
				FacebookRewardsInfo.connectPopupLastShownOnGameOver = CDateTime.Now;
				_facebookShowCounter = 1;
			}
			else
			{
				_facebookShowCounter++;
			}
			HideEventStarsIfNecessary();
		}

		private void PushNewHighScoreCelebrationScreen()
		{
			CelebrationReward celebrationReward = new CelebrationReward();
			celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.NewHighScore;
			celebrationReward.rewardType = CelebrationRewardType.highscore;
			celebrationReward.characterId = PlayerSave.currentCharacterId;
			celebrationReward.characterOutfitIndex = PlayerSave.currentOutfitIndex;
			CelebrationReward reward = celebrationReward;
			RewardManager.AddRewardToUnlock(reward);
		}

		private void ReloadFriends()
		{
			if (_reloadFriendsCoroutine != null)
			{
				StopCoroutine(_reloadFriendsCoroutine);
			}
			_reloadFriendsCoroutine = CorouReloadFriends(false);
			StartCoroutine(_reloadFriendsCoroutine);
		}

		private IEnumerator CorouReloadFriends(bool framedelay)
		{
			if (UIScreenController.ShouldShowOnlineScreen() && framedelay)
			{
				float framestamp = Time.frameCount;
				while ((float)Time.frameCount < framestamp + 2f)
				{
					yield return null;
				}
			}
			if (activeTab == ActiveTabState.GameOver)
			{
				_gameOverFriendsListHandler.ReloadFriends();
			}
			yield return null;
		}

		private void CountUpCompleted()
		{
			if (_doubleUpPanel != null && _doubleUpPanel.gameObject.activeInHierarchy)
			{
				int coinVisualValue = Mathf.Clamp(_collectedCoinsTo, 500, UI.DoubleCoinsVideo.DoubleCoinsVideo.payoutCap);
				_doubleUpPanel.SetCoinVisualValue(coinVisualValue);
			}
			_scoreLabel.text = string.Empty + _scoreTo;
			_collectedCoinLabel.text = string.Empty + _collectedCoinsTo;
			int value = OnlineSettings.instance.GetValue("rating_popup_run_limit", 10);
			if (!PreferencesSave.neverAskForRating && PlayerSave.completedRunCount > 0 && value > 0 && PlayerSave.completedRunCount % value == 0 && UserSave.lastVersionRated != DeviceUtility.GetBundleVersion() && !string.IsNullOrEmpty(Globals.RatingURL) && OnlineSettings.instance.GetValue("rating_popup_enabled", true))
			{
				UIScreenController.Instance.QueuePopup("RatingPopup");
			}
			if (IsAllowedToShowInterstitial() && AdSystemManager.IsCached(AdLocations.GAME_OVER_INTERSTITIALS))
			{
				AdSystemManager.Show(AdLocations.GAME_OVER_INTERSTITIALS);
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.Score, _scoreTo - _scoreFrom);
		}

		private bool IsAllowedToShowInterstitial()
		{
			bool result = false;
			if (!UIScreenController.Instance.isShowingPopup)
			{
				if (!AdSystemManager.IsCached(AdLocations.GAME_OVER_INTERSTITIALS))
				{
					return false;
				}
				uint value = OnlineSettings.instance.GetValue("interstitial_game_over_counter_expire_time", 72000u);
				DateTime t = _lastCountUpTimeStamp + TimeSpan.FromSeconds(value);
				if (DateTime.Compare(CDateTime.Now, t) > 0)
				{
					_counter = 0;
				}
				Flurry.LogEventWithAParameter("Interstitial Game Over Counter", "Value", _counter.ToString());
				uint value2 = OnlineSettings.instance.GetValue("interstitial_game_over_total_show_slots", 5u);
				uint value3 = OnlineSettings.instance.GetValue("interstitial_game_over_active_show_slot", 1u);
				uint num = 1u;
				if (value3 < value2)
				{
					num = value3;
				}
				result = _counter % value2 == num;
				_lastCountUpTimeStamp = CDateTime.Now;
				_counter++;
			}
			else
			{
				_hasAttemptedToShowAdOnTopOfPopup = true;
			}
			return result;
		}

		private void OnPopupShown(string popupName)
		{
			HideEventStarsIfNecessary();
		}

		public void StopCoinsCountUp()
		{
			if (_countingUpCoins)
			{
				if (_countUpCoroutine != null)
				{
					StopCoroutine(_countUpCoroutine);
				}
				_scoreCounterSoundPlayer.StopScoreSound();
				UIScreenController.Instance.topBar.SetCoinsLock(false);
				_countingUpCoins = false;
				CountUpCompleted();
			}
		}

		private IEnumerator CountUpCoins()
		{
			UIScreenController.Instance.topBar.BeforeCountUp(_coinboxTo);
			float countTime = Mathf.Lerp(0.3f, 2f, (float)_collectedCoinsTo / 100f);
			_countingUpCoins = true;
			_collectedCoinLabel.text = _collectedCoinsFrom.ToString();
			onCountUpStarted__BackingField.SafeInvoke();
			yield return new WaitForSeconds(1.2f);
			float countFactor = 0f;
			while (countFactor < 1f)
			{
				_scoreCounterSoundPlayer.PlayCoinSound(countFactor);
				countFactor += Time.deltaTime / countTime;
				_scoreLabel.text = Mathf.RoundToInt(Mathf.SmoothStep(_scoreFrom, _scoreTo, countFactor)).ToString();
				UIScreenController.Instance.topBar.SetCoins(Mathf.RoundToInt(Mathf.SmoothStep(_coinboxFrom, _coinboxTo, countFactor)), false);
				UIScreenController.Instance.topBar.UpdateCoinBoxLayout();
				_collectedCoinLabel.text = Mathf.RoundToInt(Mathf.SmoothStep(_collectedCoinsFrom, _collectedCoinsTo, countFactor)).ToString();
				if (_doubleUpPanel != null && _doubleUpPanel.gameObject.activeInHierarchy)
				{
					if (_collectedCoinsTo > 500)
					{
						int a = Mathf.SmoothStep(_collectedCoinsFrom, _collectedCoinsTo, countFactor).RoundToInt();
						_doubleUpPanel.SetCoinVisualValue(Mathf.Min(a, UI.DoubleCoinsVideo.DoubleCoinsVideo.payoutCap));
					}
					else
					{
						_doubleUpPanel.SetCoinVisualValue(500);
					}
				}
				_coinsHorizontalLayout.Align();
				yield return null;
			}
			_scoreCounterSoundPlayer.StopScoreSound();
			_scoreLabel.text = _scoreTo.ToString();
			_collectedCoinLabel.text = _collectedCoinsTo.ToString();
			UIScreenController.Instance.topBar.SetCoinsLock(false);
			_countingUpCoins = false;
			CountUpCompleted();
		}

		private IEnumerator CountUpDoubleCoinsVideo(int coinsToPayout)
		{
			int from = _collectedCoinsTo;
			int to = from + coinsToPayout;
			float countTime = Mathf.Lerp(0.3f, 2f, (float)coinsToPayout / 100f);
			_collectedCoinLabel.text = from.ToString();
			int totalTo = InventorySave.coins;
			int totalFrom = totalTo - coinsToPayout;
			UIScreenController.Instance.topBar.BeforeCountUp(totalTo);
			yield return new WaitForSeconds(0.5f);
			float countFactor = 0f;
			while (countFactor < 1f)
			{
				_scoreCounterSoundPlayer.PlayCoinSound(countFactor);
				countFactor += Time.deltaTime / countTime;
				UIScreenController.Instance.topBar.SetCoins(Mathf.SmoothStep(totalFrom, totalTo, countFactor).RoundToInt(), false);
				UIScreenController.Instance.topBar.UpdateCoinBoxLayout();
				_collectedCoinLabel.text = "{0}{1}".Populate(ColorData.instance.sun.ToUIHex(), Mathf.SmoothStep(from, to, countFactor).RoundToInt());
				_coinsHorizontalLayout.Align();
				yield return null;
			}
			_scoreCounterSoundPlayer.StopScoreSound();
			UIScreenController.Instance.topBar.SetCoinsLock(false);
		}

		private void SetupBottomAndMiddleElements()
		{
			ClearBottomContent();
			SetupBottomElements();
			SetupMiddleElements();
		}

		private void SetupFriendListIfNecessary(bool bannerActive)
		{
			bool flag = !bannerActive || IsTallAspect();
			_friendsListContainer.gameObject.SetActive(flag);
			if (flag)
			{
				if (bannerActive)
				{
					_friendsListContainer.transform.SetLocalPosY(94f);
				}
				else
				{
					_friendsListContainer.transform.SetLocalPosY(-9f);
				}
			}
		}

		[Button(null)]
		private void ClearBottomContent()
		{
			_doubleUpPanel.gameObject.SetActive(false);
			_doubleCoinsPanel.SetActive(false);
			_friendsListContainer.gameObject.SetActive(false);
		}

		[Button(null)]
		private void ToggleDoubleUpVideoBanner()
		{
			_doubleUpPanel.gameObject.SetActive(!_doubleUpPanel.gameObject.activeSelf);
		}

		[Button(null)]
		private void ToggleDoubleCoinsBanner()
		{
			_doubleCoinsPanel.SetActive(!_doubleCoinsPanel.activeSelf);
		}

		[Button(null)]
		private void ToggleFriendsList()
		{
			_friendsListContainer.gameObject.SetActive(!_friendsListContainer.gameObject.activeSelf);
		}

		private void SetupDoubleCoinsBanner()
		{
			_doubleCoinsPanel.SetActive(true);
		}

		private bool SetupDoubleUpBanner(int coinsCollectedTotal, bool forceShow = false)
		{
			int payoutCoins = Mathf.Clamp(coinsCollectedTotal, 500, UI.DoubleCoinsVideo.DoubleCoinsVideo.payoutCap);
			bool flag = HasAdFill();
			bool flag2 = CooldownManager.HasCharge("doublecoinsvideo");
			bool flag3 = (flag2 && coinsCollectedTotal >= 1 && flag && !_hasDoubledCoinsThisRun) || forceShow;
			if (_firstBannerShowAttempt)
			{
				_doubleCoinsVideoAttemptParams["HasFill"] = flag.ToString();
				_doubleCoinsVideoAttemptParams["IsCharged"] = flag2.ToString();
				_doubleCoinsVideoAttemptParams["WasBannerShown"] = flag3.ToString();
				_doubleCoinsVideoResultParams["ElementShown"] = flag3.ToString();
				_doubleCoinsVideoResultParams["VideoPayoutAmount"] = payoutCoins.ToString();
				_doubleCoinsVideoResultParams["IsDoubleCoinsVideoClicked"] = "false";
				Flurry.LogEventWithSeveralParameters("DoubleCoinsVideoAttempt", _doubleCoinsVideoAttemptParams);
				_firstBannerShowAttempt = false;
			}
			if (flag3)
			{
				DoubleCoinsVideoPayload doubleCoinsVideoPayload = new DoubleCoinsVideoPayload();
				doubleCoinsVideoPayload.coinAmount = payoutCoins;
				doubleCoinsVideoPayload.onWatchVideo = () =>
				{
					UIScreenController.Instance.topBar.SetCoinsLock(true);
					InventorySave.IncreaseCoins(payoutCoins, "DoubleCoinsVideo");
					_hasDoubledCoinsThisRun = true;
					_doubleUpPanel.gameObject.SetActive(false);
					_doubleCoinsVideoResultParams["IsDoubleCoinsVideoClicked"] = "true";
					StartCoroutine(CountUpDoubleCoinsVideo(payoutCoins));
				};
				DoubleCoinsVideoPayload doubleCoinsVideoPayload2 = doubleCoinsVideoPayload;
				_doubleUpPanel.gameObject.SetActive(true);
				_doubleUpPanel.Setup(doubleCoinsVideoPayload2);
				if (coinsCollectedTotal < 500)
				{
					_doubleUpPanel.SetCoinVisualValue(500);
				}
				if (_hasSeenLeaderboardTab || _hasSeenTopRunTab)
				{
					_doubleUpPanel.SetCoinVisualValue(doubleCoinsVideoPayload2.coinAmount);
				}
			}
			else
			{
				_doubleUpPanel.gameObject.SetActive(false);
			}
			if (flag2 && payoutCoins >= 1 && !_hasDoubledCoinsThisRun)
			{
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS);
			}
			return flag3;
		}

		private bool HasAdFill()
		{
			return AdSystemManager.IsCached(AdLocations.GAMEOVER_DOUBLE_COINS_VIDEOS);
		}

		public override string GetFlurryName()
		{
			return "GameoverUI" + _initialTab;
		}

		private void LogError(string msg)
		{
			SDebug.LogError("[GameOver Screen]: " + msg);
		}

		[Conditional("ENABLE_GAMEOVER_SCREEN_LOGS")]
		private void Log(string msg)
		{
		}
	}
}
