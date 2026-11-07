using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UI.Generic;
using UI.Screens.BoardScreenInternal;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using WeeklyHunt;

namespace UI.Screens
{
	public class BoardScreen : UIScreen
	{
		private class BoardState
		{
			public Quaternion defaultRotation;

			public GameObject boardRoot;

			public Transform boardTransform;
		}

		[FormerlySerializedAs("scrollAnchor")]
		[SerializeField]
		private GameObject _scrollAnchor;

		[FormerlySerializedAs("scrollGrid")]
		[SerializeField]
		private UIGrid _scrollGrid;

		[FormerlySerializedAs("overlayParent")]
		[SerializeField]
		private GameObject _overlayParent;

		[FormerlySerializedAs("scrollPanel")]
		[SerializeField]
		private UIPanel _scrollPanel;

		[FormerlySerializedAs("scroll2dElements")]
		[SerializeField]
		private GameObject _scroll2DElements;

		[SerializeField]
		private GameObject _centerAnchor;

		[FormerlySerializedAs("dummyObject")]
		[SerializeField]
		private GameObject _dummyObject;

		[FormerlySerializedAs("nameLabel")]
		[SerializeField]
		private UILabel _nameLabel;

		[FormerlySerializedAs("staticPowerupLabel")]
		[SerializeField]
		private UILabel _staticPowerupLabel;

		[FormerlySerializedAs("descriptionLabel")]
		[SerializeField]
		private UILabel _descriptionLabel;

		[FormerlySerializedAs("powerIcon")]
		[SerializeField]
		private UISprite _powerIcon;

		[FormerlySerializedAs("themeLimitedLabel")]
		[SerializeField]
		private UILabel _themeLimitedLabel;

		[SerializeField]
		private UIWidget _ribbonsContainer;

		[SerializeField]
		private RibbonHelper _eventRibbon;

		[SerializeField]
		private GameObject _scrollHorizontalParent;

		[SerializeField]
		private UIHoverboardThemeShifter _hoverboardThemeShifter;

		[SerializeField]
		private GameObject _hoverboardSelectButtonPrefab;

		[FormerlySerializedAs("selectSound")]
		[SerializeField]
		private AudioClip _selectSound;

		[FormerlySerializedAs("themePower")]
		[SerializeField]
		private GameObject _themePower;

		[FormerlySerializedAs("themePowerStaticLabel")]
		[SerializeField]
		private UILabel _themePowerStaticLabel;

		[FormerlySerializedAs("themePowerLabel")]
		[SerializeField]
		private UILabel _themePowerLabel;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerContainer;

		private TimerHelper _timer;

		private bool _hasInited;

		private CenterOnChild _centerer;

		private BoardSelectButton _boardSelectButton;

		private bool _refreshingThemeInScrollList;

		private SSEventType _cachedActiveEventType;

		private bool _hasSetupScroll;

		private float _cellWidth;

		private const float DEFAULT_SCALE_FACTOR = 7f;

		private const float FOCUSED_SCALE_FACTOR = 10f;

		private const int SCROLL_BOARD_VISIBILITY_COUNT = 4;

		private const int SCALE_MODEL_COUNT = 4;

		private const float SCALE_SCROLL_TOLERANCE = 1f;

		private List<KeyValuePair<Boards.BoardId, Boards.Board>> _boardList;

		private Quaternion[] _previousBoardQuaternions;

		private bool _hasEnabledScrollElements;

		private float _cachedScaleScrollPosition;

		private float _cachedScrollPosition;

		private bool _refreshOnLastPopupClosed;

		private List<BoardState> scrollBoards = new List<BoardState>();

		private List<HoverboardOverlayHelper> boardHelpers = new List<HoverboardOverlayHelper>();

		private List<OverlayIndex> boardIndices = new List<OverlayIndex>();

		private bool _oldHoverboardOwned = true;

		private bool _inappOverlayActive;

		private bool _popupActive;

		private bool _modelsEnabled;

		protected override void Init()
		{
			InitializeSelectButton();
			_cellWidth = _scrollGrid.cellWidth;
			_centerer = _scrollGrid.GetComponent<CenterOnChild>();
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			_cachedActiveEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
			_hoverboardThemeShifter.Init(BoardManager.ScreenContext.BoardScreen);
			_hoverboardThemeShifter.OnClickedTheme += ClickedHoverboardTheme;
			InitBoards();
			InitializeTimer();
			_hasInited = true;
			HandleSafeArea();
			VerifyBoardAmount();
		}

		private void HandleSafeArea()
		{
			if (SafeArea.IsRestricted)
			{
				NGUIMath.MoveWidget(_ribbonsContainer, 0f, -SafeArea.OFFSET_TOP);
				_scrollAnchor.transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
				_overlayParent.transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
				_scrollHorizontalParent.transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
			}
		}

		protected override void Show()
		{
			UIScreenController.OnApplicationResumed += RefreshIfPopupNotActive;
			SSEventsManagerController.OnSSEventStartedHandler += RefreshEventBoardAndUI;
			SSEventsManagerController.OnSSEventFinishedHandler += RefreshEventBoardAndUI;
			BoardBundlePopupManager.instance.onBoardBundleStartedHandler += RefreshEventBoardAndUI;
			BoardBundlePopupManager.instance.onBoardBundleExpiredHandler += RefreshEventBoardAndUI;
			BoardManager.boardSelected += UpdateUIElements;
			BoardManager.themeActiveStateChanged += OnThemeActiveStateChanged;
			UIScreenController.OnLastPopupClosed += OnLastPopupClosed;
			RefreshBoards();
			if (UIScreenController.Instance.isShowingPopup)
			{
				_refreshOnLastPopupClosed = true;
			}
			else
			{
				RefreshEventBoardAndUI();
			}
		}

		protected override void Hide()
		{
			base.Hide();
			UIScreenController.OnApplicationResumed -= RefreshIfPopupNotActive;
			SSEventsManagerController.OnSSEventStartedHandler -= RefreshEventBoardAndUI;
			SSEventsManagerController.OnSSEventFinishedHandler -= RefreshEventBoardAndUI;
			BoardBundlePopupManager.instance.onBoardBundleStartedHandler -= RefreshEventBoardAndUI;
			BoardBundlePopupManager.instance.onBoardBundleExpiredHandler -= RefreshEventBoardAndUI;
			BoardManager.boardSelected -= UpdateUIElements;
			BoardManager.themeActiveStateChanged -= OnThemeActiveStateChanged;
			UIScreenController.OnLastPopupClosed -= OnLastPopupClosed;
			HoverboardModelPreviewFactory.Instance.ResetTeleportFX();
			HoverboardModelPreviewFactory.Instance.ResetAnimator();
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.Boards);
		}

		public override void GainFocus()
		{
			base.GainFocus();
			if (BoardScreenManager.Instance.cameBackFromBundle)
			{
				BoardScreenManager.Instance.cameBackFromBundle = false;
				_boardSelectButton.SetCurrentThemeIndex(0);
			}
			BoardScreenManager.Instance.willResetBoardAnimation = true;
		}

		public override void LoseFocus()
		{
			base.LoseFocus();
			BoardScreenManager.Instance.willResetBoardAnimation = false;
		}

		private void OnLastPopupClosed()
		{
			if (_refreshOnLastPopupClosed)
			{
				RefreshEventBoardAndUI();
				_refreshOnLastPopupClosed = false;
			}
		}

		private void RefreshIfPopupNotActive()
		{
			if (_inappOverlayActive || _popupActive)
			{
				BoardScreenManager.Instance.refreshBoardsWhenNotShowingPopup = true;
				return;
			}
			RefreshBoards();
			CoroutineUtil.StartCoroutine(DelayedRefreshEventBoardAndUI());
		}

		private void InitializeSelectButton()
		{
			GameObject gameObject = _centerAnchor.AddChild(_hoverboardSelectButtonPrefab);
			_boardSelectButton = gameObject.GetComponent<BoardSelectButton>();
			gameObject.transform.localPosition = new Vector3(62f, 0f, 0f);
			_boardSelectButton.InitButton(BoardManager.ScreenContext.BoardScreen);
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerContainer.transform);
		}

		private IEnumerator DelayedRefreshEventBoardAndUI()
		{
			yield return new WaitForEndOfFrame();
			RefreshEventBoardAndUI();
		}

		private void RefreshEventBoardAndUI()
		{
			SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
			ActivateEventRibbon(_boardSelectButton.DoesCurrentBoardMatchEvent());
			UIModelController.Instance.ActivateHoverboardModel();
			ShowHoverboardInMenu(true, false);
		}

		private void OnThemeActiveStateChanged(Hoverboard.BoardAbility boardAbility)
		{
			ShowHoverboardInMenu(true, true, boardAbility);
			ShowHoverboardInScrollList();
		}

		public void ShowHoverboardInMenu(bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility ability = Hoverboard.BoardAbility.None)
		{
			UIModelController.Instance.ShowHoverboardMenuModel(BoardScreenManager.Instance.currentlyShownBoardId, UIModelController.ModelScreen.Boards, willUpdateAnimation, willAllowSignatureAnimation, ability);
			UpdateUIElements();
		}

		public void ShowHoverboardInScrollList()
		{
			_refreshingThemeInScrollList = true;
			RefreshBoards();
			_refreshingThemeInScrollList = false;
		}

		private void UpdateUIElements()
		{
			Boards.BoardId currentlyShownBoardId = BoardScreenManager.Instance.currentlyShownBoardId;
			Boards.Board board = Boards.boardData[currentlyShownBoardId];
			if (BoardScreenManager.Instance.shouldResetThemeSelection)
			{
				BoardScreenManager.Instance.shouldResetThemeSelection = false;
				BoardScreenManager.Instance.currentlyShownThemeIndex = 0;
				_hoverboardThemeShifter.ResetThemeSelection();
			}
			if (BoardScreenManager.Instance.currentlyShownThemeIndex != 0)
			{
				List<HoverboardThemes.HoverboardTheme> list = HoverboardThemes.TryGetCustomThemesForBoard(currentlyShownBoardId);
				if (list == null || BoardScreenManager.Instance.currentlyShownThemeIndex > list.Count)
				{
					BoardScreenManager.Instance.currentlyShownThemeIndex = 0;
					_hoverboardThemeShifter.ResetThemeSelection();
				}
			}
			_nameLabel.text = Strings.Get(board.name);
			_descriptionLabel.text = Strings.Get(board.description);
			_themeLimitedLabel.text = Strings.Get(board.limitedDescription);
			_staticPowerupLabel.text = Strings.Get("BOARD_SCREEN_SPECIAL_POWER");
			_nameLabel.color = ColorData.instance.midnight;
			_themeLimitedLabel.color = ColorData.instance.peacock;
			bool flag = !string.IsNullOrEmpty(Strings.Get(board.limitedDescription));
			bool flag2 = !string.IsNullOrEmpty(Strings.Get(board.description));
			_powerIcon.enabled = flag2;
			_staticPowerupLabel.enabled = flag2;
			_themeLimitedLabel.enabled = flag;
			if (!flag || flag2)
			{
			}
			if (BoardScreenManager.Instance.currentlyShownThemeIndex == 0)
			{
				_themePower.SetActive(false);
				_nameLabel.gameObject.SetActive(true);
			}
			else
			{
				_themePower.SetActive(true);
				_nameLabel.gameObject.SetActive(false);
				_themePowerStaticLabel.text = Strings.Get("BOARD_SCREEN_SPECIAL_POWER");
				_themePowerLabel.text = Strings.Get(HoverboardThemes.hoverboardCustomThemes[BoardScreenManager.Instance.currentlyShownBoardId][BoardScreenManager.Instance.currentlyShownThemeIndex - 1].title);
				_themeLimitedLabel.text = string.Empty;
			}
			Boards.BoardId? eventBoardType = BoardManager.GetEventBoardType();
			bool flag3 = eventBoardType.HasValue && currentlyShownBoardId == eventBoardType.Value;
			bool flag4 = BoardManager.IsBoardTrySpecialPowerBoard(currentlyShownBoardId);
			bool flag5 = BoardManager.IsBoardEventLimitedExpiredBoard(currentlyShownBoardId);
			TimeSpan timeSpan = TimeSpan.MinValue;
			if (!BoardManager.IsBoardOwned(currentlyShownBoardId))
			{
				if (flag4 || flag5)
				{
					timeSpan = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
					_timer.SetTimerColors(GlobalColors.GIFT_LABEL_COLOR, GlobalColors.GIFT_COLOR);
				}
				else if ((board.hasOnlineTimeLimit || WeeklyHuntManager.IsBoardAwardedInCurrentPeriod(currentlyShownBoardId)) && !flag3)
				{
					timeSpan = BoardManager.BoardExpirationTimespan(currentlyShownBoardId);
					if (WeeklyHuntManager.IsBoardAwardedInCurrentPeriod(currentlyShownBoardId))
					{
						timeSpan = WeeklyHuntManager.GetTimeLeftForActivePeriod();
					}
					_timer.SetTimerColors(GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR, GlobalColorsGenerated.DISCOUNT_STICKER_COLOR);
				}
				else if (WelcomePackManager.IsWelcomeBoardActive(currentlyShownBoardId))
				{
					timeSpan = WelcomePackManager.welcomePackTimeLeft;
					_timer.SetTimerColors(GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT, GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG);
				}
			}
			ActivateEventRibbon(_boardSelectButton.DoesCurrentBoardMatchEvent());
			ShowTimer(timeSpan != TimeSpan.MinValue, timeSpan);
			_boardSelectButton.UpdateSticker(BoardScreenManager.Instance.currentlyShownBoardId, BoardScreenManager.Instance.currentlyShownThemeIndex);
			SetupThemes();
		}

		private void ActivateEventRibbon(bool boardMatchesEvent)
		{
			string firstLine = string.Empty;
			string secondLine = string.Empty;
			string thirdLine = string.Empty;
			bool shouldShow = boardMatchesEvent;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType eventType = activeEventManager.GetEventType();
				if (eventType == SSEventType.ExpiredLimitedBoard || eventType == SSEventType.SpecialPowerBoard)
				{
					activeEventManager.GetRibbonDescription(out firstLine, out secondLine, out thirdLine);
				}
				else
				{
					shouldShow = false;
				}
			}
			_eventRibbon.Setup(new RibbonHelper.RibbonPayload
			{
				line1 = firstLine,
				line2 = secondLine,
				line3 = thirdLine,
				shouldShow = shouldShow
			});
		}

		private void InitBoards()
		{
			_boardList = new List<KeyValuePair<Boards.BoardId, Boards.Board>>();
			Boards.BoardId? eventBoardType = BoardManager.GetEventBoardType();
			foreach (Boards.BoardId item in Boards.boardOrder)
			{
				Boards.Board value = Boards.boardData[item];
				if (BoardManager.IsBoardOwned(item) || (BoardManager.IsBoardActive(item) && item != Boards.BoardId.starterPackBoard01) || (eventBoardType.HasValue && eventBoardType.Value == item) || WelcomePackManager.IsWelcomeBoardActive(item))
				{
					_boardList.Add(new KeyValuePair<Boards.BoardId, Boards.Board>(item, value));
				}
			}
			if (eventBoardType.HasValue)
			{
				BoardScreenManager.Instance.forceCenterOnSpecificBoard = true;
				BoardScreenManager.Instance.forceSpecificBoard = eventBoardType.Value;
			}
			int num = 0;
			foreach (KeyValuePair<Boards.BoardId, Boards.Board> board in _boardList)
			{
				BoardState boardState = new BoardState();
				boardState.boardRoot = HoverboardModelPreviewFactory.Instance.GetHoverboardModelPreview(board.Value.boardModelName, board.Key);
				if (!(boardState.boardRoot == null))
				{
					boardState.boardRoot.name = string.Format("{0:000}{1}", num, board.Value.name);
					boardState.boardTransform = boardState.boardRoot.transform.GetChild(0);
					boardState.defaultRotation = HoverboardModelPreviewFactory.Instance.GetHoverboardDefaultRotation(board.Value.boardModelName);
					scrollBoards.Add(boardState);
					Transform transform = boardState.boardRoot.transform;
					transform.parent = _scrollAnchor.transform;
					transform.localPosition = new Vector3((float)num * _cellWidth, 0f, 50f);
					transform.localScale = Vector3.one * 7f;
					transform.localEulerAngles = new Vector3(53f, 183f, 360f);
					GameObject gameObject = _overlayParent.AddChild(_scroll2DElements);
					gameObject.name = string.Format("{0:000}{1}", num, board.Value.name);
					boardHelpers.Add(gameObject.GetComponent<HoverboardOverlayHelper>());
					boardHelpers[num].Init(num, board.Key, transform);
					GameObject gameObject2 = _scrollGrid.gameObject.AddChild(_dummyObject);
					boardIndices.Add(gameObject2.AddComponent<OverlayIndex>());
					boardIndices[num].index = num;
					gameObject2.name = string.Format("{0:000}{1}", num, board.Key.ToString());
					num++;
				}
			}
			Utility.SetLayerRecursively(_scrollAnchor.transform, 29);
			Utility.SetLayerRecursively(_overlayParent.transform, 26);
			_scrollGrid.SendMessage("Start");
			if (!_refreshingThemeInScrollList)
			{
				_hasEnabledScrollElements = false;
				StartCoroutine(DelayedInitCenterBoard());
				UIModelController.Instance.ActivateHoverboardModel();
				Boards.BoardId currentlyShownBoardId = BoardTypeToCenterOn();
				BoardScreenManager.Instance.currentlyShownBoardId = currentlyShownBoardId;
				ToggleScrollElements(false);
				return;
			}
			Boards.BoardId currentlyShownBoardId2 = BoardScreenManager.Instance.currentlyShownBoardId;
			OverlayIndex overlayIndex = ElementToCenterOn(boardIndices, currentlyShownBoardId2);
			_centerer.CenterOnTransform(overlayIndex.transform, true);
			BoardScreenManager.Instance.ChangedDisplayedHoverboard(currentlyShownBoardId2);
			for (int i = 0; i < scrollBoards.Count; i++)
			{
				scrollBoards[i].boardTransform.rotation = _previousBoardQuaternions[i];
			}
			ToggleScrollElements(true);
			ScaleScrollModels(true);
		}

		private IEnumerator DelayedInitCenterBoard()
		{
			yield return new WaitForEndOfFrame();
			Boards.BoardId relevantBoard = BoardTypeToCenterOn();
			OverlayIndex elementOverlayIndex = ElementToCenterOn(boardIndices, relevantBoard);
			if (elementOverlayIndex != null)
			{
				_centerer.CenterOnTransform(elementOverlayIndex.transform, true);
			}
			BoardScreenManager.Instance.ChangedDisplayedHoverboard(relevantBoard);
		}

		private void ToggleScrollElements(bool enable)
		{
			if (_inappOverlayActive || _popupActive)
			{
				BoardScreenManager.Instance.refreshBoardsWhenNotShowingPopup = true;
				return;
			}
			_scrollAnchor.SetActive(enable);
			_overlayParent.SetActive(enable);
		}

		private void ClickedHoverboardTheme(int index)
		{
			bool flag = BoardManager.IsThemeOwned(BoardScreenManager.Instance.currentlyShownBoardId, HoverboardThemes.GetThemeIdentifier(BoardScreenManager.Instance.currentlyShownBoardId, index - 1));
			if (BoardScreenManager.Instance.currentlyShownThemeIndex == index)
			{
				if (flag)
				{
					_boardSelectButton.OnClick();
				}
				else if (index != 0)
				{
					return;
				}
			}
			BoardScreenManager.Instance.currentlyShownThemeIndex = index;
			_boardSelectButton.UpdateCurrentThemeIndex();
			bool flag2 = !_oldHoverboardOwned;
			_oldHoverboardOwned = flag;
			UpdateDisplayedModelWithTheme(BoardScreenManager.Instance.currentlyShownBoardId, BoardScreenManager.Instance.currentlyShownThemeIndex);
			if (flag && !flag2)
			{
				return;
			}
			CharacterModel activeCharacterModel = UIModelController.Instance.GetActiveCharacterModel(UIModelController.ModelScreen.Boards);
			if (!(activeCharacterModel != null))
			{
				return;
			}
			GameObject hoverboardGameObject = activeCharacterModel.GetHoverboardGameObject();
			Animation characterAnimation = activeCharacterModel.CharacterAnimation;
			if (hoverboardGameObject != null && characterAnimation != null)
			{
				if (BoardScreenManager.Instance.currentlyShownThemeIndex > 0)
				{
					HoverboardModelPreviewFactory.Instance.ChangeHoverboard(BoardScreenManager.Instance.currentlyShownBoardId, hoverboardGameObject, characterAnimation, true, true, HoverboardThemes.hoverboardCustomThemes[BoardScreenManager.Instance.currentlyShownBoardId][BoardScreenManager.Instance.currentlyShownThemeIndex - 1].power);
				}
				else
				{
					HoverboardModelPreviewFactory.Instance.ChangeHoverboard(BoardScreenManager.Instance.currentlyShownBoardId, hoverboardGameObject, characterAnimation, true, true, Hoverboard.BoardAbility.None);
				}
			}
		}

		private void UpdateDisplayedModelWithTheme(Boards.BoardId boardId, int themeIndex)
		{
			if (themeIndex != 0)
			{
				bool flag = BoardManager.IsThemeOwned(boardId, HoverboardThemes.GetThemeIdentifier(boardId, themeIndex - 1)) && !BoardManager.IsThemeActive(boardId, HoverboardThemes.GetThemeIdentifier(boardId, themeIndex - 1));
				BoardManager.DisplayCustomSet(themeIndex - 1, !flag);
				for (int i = 0; i < 2; i++)
				{
					if (i != themeIndex - 1)
					{
						BoardManager.DisplayCustomSet(i, BoardManager.IsThemeOwned(boardId, HoverboardThemes.GetThemeIdentifier(boardId, i)) && BoardManager.IsThemeActive(boardId, HoverboardThemes.GetThemeIdentifier(boardId, i)));
					}
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					BoardManager.DisplayCustomSet(j, BoardManager.IsThemeActive(boardId, HoverboardThemes.GetThemeIdentifier(boardId, j)));
				}
			}
			ShowHoverboardInMenu(false, true);
		}

		private void RefreshBoards()
		{
			_boardList = new List<KeyValuePair<Boards.BoardId, Boards.Board>>();
			bool flag = false;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			SSEventType sSEventType = ((activeEventManager != null) ? activeEventManager.GetEventType() : SSEventType.None);
			if (_cachedActiveEventType != sSEventType)
			{
				_cachedActiveEventType = sSEventType;
				flag = true;
			}
			if (BoardScreenManager.Instance.forceRefreshOnBoardScreen)
			{
				BoardScreenManager.Instance.forceRefreshOnBoardScreen = false;
				flag = true;
			}
			if (_refreshingThemeInScrollList)
			{
				flag = true;
			}
			Boards.BoardId? eventBoardType = BoardManager.GetEventBoardType();
			foreach (Boards.BoardId item in Boards.boardOrder)
			{
				Boards.Board value = Boards.boardData[item];
				if (BoardManager.IsBoardOwned(item) || (BoardManager.IsBoardActive(item) && item != Boards.BoardId.starterPackBoard01) || (eventBoardType.HasValue && eventBoardType.Value == item) || WelcomePackManager.IsWelcomeBoardActive(item))
				{
					_boardList.Add(new KeyValuePair<Boards.BoardId, Boards.Board>(item, value));
				}
			}
			if (!flag)
			{
				if (boardHelpers.Count != _boardList.Count)
				{
					flag = true;
				}
				else
				{
					for (int i = 0; i < boardHelpers.Count; i++)
					{
						if (_boardList[i].Key != boardHelpers[i].GetBoardType())
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				_previousBoardQuaternions = new Quaternion[scrollBoards.Count];
				for (int j = 0; j < scrollBoards.Count; j++)
				{
					_previousBoardQuaternions[j] = scrollBoards[j].boardTransform.rotation;
				}
				foreach (BoardState scrollBoard in scrollBoards)
				{
					UnityEngine.Object.Destroy(scrollBoard.boardRoot);
				}
				scrollBoards.Clear();
				foreach (HoverboardOverlayHelper boardHelper in boardHelpers)
				{
					UnityEngine.Object.Destroy(boardHelper.gameObject);
				}
				boardHelpers.Clear();
				foreach (OverlayIndex boardIndex in boardIndices)
				{
					UnityEngine.Object.Destroy(boardIndex.gameObject);
				}
				boardIndices.Clear();
				foreach (Transform item2 in _scrollGrid.transform)
				{
					item2.gameObject.SetActive(false);
					UnityEngine.Object.Destroy(item2);
				}
				_scrollAnchor.transform.localPosition = new Vector3(0f, _scrollAnchor.transform.localPosition.y, _scrollAnchor.transform.localPosition.z);
				_scrollPanel.cachedTransform.localPosition = Vector3.zero;
				_scrollPanel.clipOffset = new Vector2(0f, 0f);
				_centerer.ClearCenterObject();
				SpringPanel component = _scrollPanel.GetComponent<SpringPanel>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				InitBoards();
			}
			else if (BoardScreenManager.Instance.forceCenterOnSpecificBoard)
			{
				Boards.BoardId boardId = BoardTypeToCenterOn();
				OverlayIndex overlayIndex = ElementToCenterOn(boardIndices, boardId);
				_centerer.CenterOnTransform(overlayIndex.transform);
				UIModelController.Instance.ActivateHoverboardModel();
				BoardScreenManager.Instance.ChangedDisplayedHoverboard(boardId);
				BoardScreenManager.Instance.forceCenterOnSpecificBoard = false;
			}
		}

		private void ShowTimer(bool showTimer, TimeSpan timeLeft)
		{
			_timer.gameObject.SetActive(showTimer);
			if (showTimer)
			{
				_timer.StartTimer(timeLeft);
			}
			else
			{
				_timer.StopTimer();
			}
		}

		private void SetupThemes()
		{
			if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(BoardScreenManager.Instance.currentlyShownBoardId))
			{
				_hoverboardThemeShifter.MakeCustomThemesAvailable(HoverboardThemes.hoverboardCustomThemes[BoardScreenManager.Instance.currentlyShownBoardId]);
			}
			else
			{
				_hoverboardThemeShifter.DisableThemeShifter();
			}
		}

		public void ScrollClicked(Vector2 pos)
		{
			if (_centerer.CenterOnClosestChildAtPosition(pos) && BoardManager.IsBoardOwned(BoardScreenManager.Instance.currentlyShownBoardId))
			{
				BoardManager.SelectCurrentlyShownBoard(BoardManager.ScreenContext.BoardScreen);
				NGUITools.PlaySound(_selectSound);
			}
		}

		private Boards.BoardId BoardTypeToCenterOn()
		{
			Boards.BoardId result;
			if (BoardScreenManager.Instance.forceCenterOnSpecificBoard)
			{
				result = BoardScreenManager.Instance.forceSpecificBoard;
			}
			else
			{
				SSEventsManagerController.instance.RestoreBoardAfterEventIfNeeded();
				result = PlayerSave.currentBoardId;
				Boards.BoardId currentlyShownBoardId = BoardScreenManager.Instance.currentlyShownBoardId;
				if (_hasSetupScroll && (BoardManager.IsBoardOwned(currentlyShownBoardId) || (BoardManager.IsBoardActive(currentlyShownBoardId) && currentlyShownBoardId != Boards.BoardId.starterPackBoard01) || WelcomePackManager.IsWelcomeBoardActive(currentlyShownBoardId)))
				{
					result = BoardScreenManager.Instance.currentlyShownBoardId;
				}
			}
			return result;
		}

		private T ElementToCenterOn<T>(List<T> elements, Boards.BoardId board)
		{
			T result = default(T);
			int num = 0;
			using (List<KeyValuePair<Boards.BoardId, Boards.Board>>.Enumerator enumerator = _boardList.GetEnumerator())
			{
				while (enumerator.MoveNext() && board != enumerator.Current.Key)
				{
					num++;
				}
			}
			if (num < elements.Count)
			{
				result = elements[num];
			}
			return result;
		}

		private void ScaleScrollModels(bool forceUpdate = false)
		{
			float num = Mathf.Abs(_scrollPanel.transform.localPosition.x);
			bool flag = Mathf.Abs(num - _cachedScrollPosition) > 0f || forceUpdate;
			bool flag2 = Mathf.Abs(num - _cachedScaleScrollPosition) > 1f || forceUpdate;
			if (!flag)
			{
				return;
			}
			int num2 = Mathf.RoundToInt(num / _cellWidth);
			int num3 = Mathf.Max(0, num2 - 4);
			int num4 = Mathf.Min(scrollBoards.Count, num2 + 4);
			for (int i = num3; i < num4; i++)
			{
				float num5 = Mathf.Abs(num - (float)i * _cellWidth);
				float num6 = 1.5f * _cellWidth;
				if (flag2)
				{
					float num7 = Mathf.SmoothStep(10f, 7f, num5 / num6);
					scrollBoards[i].boardRoot.transform.localScale = Vector3.one * num7;
					_cachedScaleScrollPosition = num;
				}
				boardHelpers[i].currentDistFromCenter = num5 / num6;
				_cachedScrollPosition = num;
			}
		}

		private void Update()
		{
			if (!_hasInited || _boardList == null)
			{
				return;
			}
			if (BoardScreenManager.Instance.refreshBoardsWhenNotShowingPopup && !_inappOverlayActive && !_popupActive)
			{
				RefreshIfPopupNotActive();
				BoardScreenManager.Instance.refreshBoardsWhenNotShowingPopup = false;
			}
			UpdateBoardEnabledState();
			if (_centerer.selectedObject != null)
			{
				int index = _centerer.selectedObject.GetComponent<OverlayIndex>().index;
				Transform boardTransform = scrollBoards[index].boardTransform;
				boardTransform.RotateAround(boardTransform.right, 0f - Time.deltaTime);
				Boards.BoardId boardType = boardHelpers[index].GetBoardType();
				if (!_hasSetupScroll)
				{
					BoardScreenManager.Instance.currentlyShownBoardId = boardType;
					_hasSetupScroll = true;
				}
				else if (BoardScreenManager.Instance.currentlyShownBoardId != boardType || BoardScreenManager.Instance.willResetBoardAnimation)
				{
					if (!BoardScreenManager.Instance.willResetBoardAnimation)
					{
						BoardScreenManager.Instance.currentlyShownThemeIndex = 0;
						_boardSelectButton.UpdateCurrentThemeIndex();
					}
					if (BoardScreenManager.Instance.currentlyShownBoardId != boardType)
					{
						BoardScreenManager.Instance.ChangedDisplayedHoverboard(boardType);
					}
					ShowHoverboardInMenu(true, true);
					BoardScreenManager.Instance.willResetBoardAnimation = false;
					boardHelpers[index].SelectedInMenu();
					foreach (HoverboardOverlayHelper boardHelper in boardHelpers)
					{
						if (boardHelper.GetIndex() != index)
						{
							boardHelper.RefreshSelected(false);
						}
					}
				}
				index = ((!(_centerer.selectedObject != null)) ? (-1) : _centerer.selectedObject.GetComponent<OverlayIndex>().index);
				for (int i = 0; i < scrollBoards.Count; i++)
				{
					if (i != index)
					{
						BoardState boardState = scrollBoards[i];
						Quaternion b = boardState.boardTransform.parent.rotation * boardState.defaultRotation;
						boardState.boardTransform.rotation = Quaternion.Lerp(boardState.boardTransform.rotation, b, Time.deltaTime);
					}
				}
			}
			ScaleScrollModels();
			if (!_hasEnabledScrollElements)
			{
				ToggleScrollElements(true);
				_hasEnabledScrollElements = true;
			}
			_popupActive = UIScreenController.Instance.isShowingPopup;
			_inappOverlayActive = UIScreenController.Instance.inAppPurchaseOverlay.activeInHierarchy;
			if (_inappOverlayActive || _popupActive)
			{
				if (_modelsEnabled)
				{
					_scrollAnchor.SetActive(false);
					_overlayParent.SetActive(false);
					_modelsEnabled = false;
				}
			}
			else if (!_modelsEnabled)
			{
				_scrollAnchor.SetActive(true);
				_overlayParent.SetActive(true);
				_modelsEnabled = true;
			}
		}

		private void UpdateBoardEnabledState()
		{
			int num = Mathf.RoundToInt(Mathf.Abs(_scrollPanel.transform.localPosition.x) / _cellWidth);
			for (int i = 0; i < scrollBoards.Count; i++)
			{
				bool active = Mathf.Abs(i - num) <= 4;
				scrollBoards[i].boardRoot.SetActive(active);
				boardHelpers[i].gameObject.SetActive(active);
			}
		}

		private void VerifyBoardAmount()
		{
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError(msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
