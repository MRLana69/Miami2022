using System;
using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using SYBO.Subway.Promotion;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;
using UnityEngine.Serialization;
using WeeklyHunt;

namespace UI
{
	public class BoardSelectButton : MonoBehaviour
	{
		private enum ButtonStates
		{
			Buy = 0,
			Select = 1,
			Trial = 2,
			BuyWithInapp = 3,
			Unselect = 4,
			LockedTheme = 5,
			WatchVideo = 6,
			GetNow = 7,
			NoAvailableVideo = 8,
			View = 9
		}

		private enum StickerType
		{
			None = 0,
			Event = 1,
			Limited = 2,
			Gift = 3
		}

		private enum VideoState
		{
			Ready = 0,
			Failed = 1,
			Loading = 2
		}

		[Header("Buy")]
		[SerializeField]
		private GameObject _buy;

		[SerializeField]
		private UILabel _priceLabel;

		[SerializeField]
		private UISprite _keySprite;

		[SerializeField]
		private UISprite _coinSprite;

		[SerializeField]
		private HorizontalLayout _buyLayout;

		[Header("Select")]
		[SerializeField]
		private GameObject _select;

		[SerializeField]
		private UILabel _labelSelected;

		[Header("WatchVideo")]
		[SerializeField]
		private GameObject _watchVideo;

		[SerializeField]
		private GameObject _videoSprite;

		[SerializeField]
		private UISprite _loadingSprite;

		[SerializeField]
		private UISprite _spacingSprite;

		[SerializeField]
		private UILabel _watchVideoLabel;

		[SerializeField]
		private UILabel _watchVideoSubLabel;

		[SerializeField]
		private HorizontalLayout _watchVideoLayout;

		[SerializeField]
		private VerticalLayout _watchVideoLabelLayout;

		[Space]
		[FormerlySerializedAs("_lockedBoardGraphics")]
		[SerializeField]
		private GameObject _lockedBoardContent;

		[SerializeField]
		private UILabel _progressLabel;

		[SerializeField]
		private UISprite _tokenSprite;

		[SerializeField]
		private HorizontalLayout _lockedBoardLayout;

		[Header("Trial")]
		[SerializeField]
		private GameObject _trial;

		[SerializeField]
		private UILabel _labelTry;

		[Header("Buy With Inapp")]
		[SerializeField]
		private GameObject _buyWithInapp;

		[SerializeField]
		private UILabel _labelBuyWithInapp;

		[Header("Unselect")]
		[SerializeField]
		private GameObject _unselect;

		[SerializeField]
		private UILabel _labelUnselect;

		[Header("Locked theme")]
		[FormerlySerializedAs("_lockedThemeGraphics")]
		[SerializeField]
		private GameObject _lockedThemeContent;

		[SerializeField]
		private UISprite _lockedKeySprite;

		[SerializeField]
		private UISprite _lockedCoinSprite;

		[SerializeField]
		private UILabel _notAvailableThemeAmount;

		[SerializeField]
		private UILabel _notAvailableThemeFeedback;

		[SerializeField]
		private HorizontalLayout _themeToplayout;

		[SerializeField]
		private HorizontalLayout _themeBottomLayout;

		[Header("Get now")]
		[SerializeField]
		private GameObject _getNowContent;

		[Header("View")]
		[SerializeField]
		private GameObject _viewContent;

		[SerializeField]
		private UILabel _viewLabel;

		[Header("Sticker")]
		[SerializeField]
		private UISprite _backgroundSticker;

		[SerializeField]
		private UISprite _eventIcon;

		[SerializeField]
		private UISprite _giftIcon;

		[SerializeField]
		private UISprite _limitedIcon;

		public Action onPurchaseSuccessful;

		public Action onPurchaseFailed;

		private Boards.BoardId _boardId;

		private int _currentThemeIndex;

		private UIButtonOverlayOff _uiButtonOverlayOff;

		private bool _purchaseInProgress;

		private bool _hasInited;

		private bool _shouldShowEventIndicatorsForTryBoardsEvent;

		private bool _shouldShowEventIndicatorsForBuyLimitedBoardsEvent;

		private bool _shouldShowExpirationTimeForBoardBundleSale;

		private int _videoCacheRetry;

		private bool _didWatchVideo;

		private bool _shouldLogWatchVideoData;

		private VideoState _currentVideoState;

		private BoardManager.ScreenContext _screenContext;

		private ButtonStates _activeState;

		public void SetCurrentThemeIndex(int index)
		{
			_currentThemeIndex = index;
		}

		public void UpdateCurrentThemeIndex()
		{
			_currentThemeIndex = BoardManager.GetCurrentlyShownHoverboardTheme(_screenContext);
		}

		private void SetSticker(StickerType type)
		{
			_backgroundSticker.gameObject.SetActive(type != StickerType.None);
			_eventIcon.gameObject.SetActive(type == StickerType.Event);
			_giftIcon.gameObject.SetActive(type == StickerType.Gift);
			_limitedIcon.gameObject.SetActive(type == StickerType.Limited);
			if (type == StickerType.Limited)
			{
				_eventIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
				_giftIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
				_limitedIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
				_backgroundSticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			}
			else
			{
				_eventIcon.color = GlobalColors.GIFT_LABEL_COLOR;
				_giftIcon.color = GlobalColors.GIFT_LABEL_COLOR;
				_limitedIcon.color = GlobalColors.GIFT_LABEL_COLOR;
				_backgroundSticker.color = GlobalColors.GIFT_COLOR;
			}
		}

		public void UpdateSticker(Boards.BoardId boardId, int themeIndex)
		{
			Boards.Board board = Boards.boardData[boardId];
			bool flag = BoardManager.IsBoardOwned(boardId);
			Boards.BoardId? eventBoardType = BoardManager.GetEventBoardType();
			bool flag2 = eventBoardType.HasValue && boardId == eventBoardType.Value;
			if (themeIndex == 0)
			{
				if (flag)
				{
					SetSticker(StickerType.None);
				}
				else if (flag2 && !SSEventsManagerController.instance.HasTheCurrentEventBeenTried())
				{
					SetSticker(StickerType.Event);
				}
				else if (IsBoardOrThemeFree(boardId))
				{
					SetSticker(StickerType.Gift);
				}
				else if (_shouldShowExpirationTimeForBoardBundleSale || ((board.hasOnlineTimeLimit || WeeklyHuntManager.IsBoardAwardedInCurrentPeriod(boardId)) && !flag2) || WelcomePackManager.IsWelcomeBoardActive(boardId))
				{
					SetSticker(StickerType.Limited);
					if (board == Boards.boardData[Boards.BoardId.birthdayBoard01])
					{
						_backgroundSticker.transform.localPosition = new Vector3(-69f, -30f, 0f);
						_backgroundSticker.transform.rotation = Quaternion.Euler(0f, 0f, 30f);
					}
					else
					{
						_backgroundSticker.transform.localPosition = new Vector3(68f, -30f, 0f);
						_backgroundSticker.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					}
				}
				else
				{
					SetSticker(StickerType.None);
				}
			}
			else if (IsBoardOrThemeFree(boardId, themeIndex - 1) && !BoardManager.IsThemeOwned(boardId, HoverboardThemes.GetThemeIdentifier(boardId, themeIndex - 1)))
			{
				SetSticker(StickerType.Gift);
			}
			else
			{
				SetSticker(StickerType.None);
			}
		}

		private bool IsBoardOrThemeFree(Boards.BoardId boardId, int themeIndex = -1)
		{
			if (themeIndex > -1)
			{
				HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[boardId][themeIndex];
				return (hoverboardTheme.unlockType == PurchaseData.UnlockType.Coins || hoverboardTheme.unlockType == PurchaseData.UnlockType.Keys) && hoverboardTheme.price == 0;
			}
			Boards.Board board = Boards.boardData[boardId];
			return (board.unlockType == PurchaseData.UnlockType.Coins || board.unlockType == PurchaseData.UnlockType.Keys) && board.price == 0;
		}

		private void SetButtonState(ButtonStates state)
		{
			_activeState = state;
			_buy.SetActive(state == ButtonStates.Buy);
			_select.SetActive(state == ButtonStates.Select);
			_watchVideo.SetActive(state == ButtonStates.WatchVideo);
			_trial.SetActive(state == ButtonStates.Trial);
			_buyWithInapp.SetActive(state == ButtonStates.BuyWithInapp);
			_unselect.SetActive(state == ButtonStates.Unselect);
			_lockedThemeContent.SetActive(state == ButtonStates.LockedTheme);
			_viewContent.SetActive(state == ButtonStates.View);
			if (_getNowContent != null)
			{
				_getNowContent.SetActive(state == ButtonStates.GetNow);
			}
			if (_getNowContent != null)
			{
				_getNowContent.SetActive(state == ButtonStates.GetNow);
			}
			switch (state)
			{
			case ButtonStates.Buy:
				if (_currentThemeIndex == 0)
				{
					SetBuyHoverboardUI();
				}
				else
				{
					SetBuyBoardThemeUI();
				}
				break;
			case ButtonStates.Select:
				if (_currentThemeIndex == 0)
				{
					SetSelectHoverboardUI();
				}
				else
				{
					SetSelectBoardThemeUI();
				}
				break;
			case ButtonStates.Trial:
				SetTryHoverboardUI();
				break;
			case ButtonStates.BuyWithInapp:
				BuyHoverboardWithInapp();
				break;
			case ButtonStates.Unselect:
				SetUnselectBoardThemeUI();
				break;
			case ButtonStates.LockedTheme:
				SetSelectLockedTheme();
				break;
			case ButtonStates.WatchVideo:
				SetWatchVideo();
				break;
			case ButtonStates.GetNow:
				SetToSelectGreenState();
				break;
			case ButtonStates.View:
				SetToViewState();
				break;
			case ButtonStates.NoAvailableVideo:
				break;
			}
		}

		private void OnEnable()
		{
			SSEventsManagerController.OnSSEventStartedHandler += OnChangedCurrentlyShownBoardModel;
			SSEventsManagerController.OnSSEventFinishedHandler += OnChangedCurrentlyShownBoardModel;
			BoardBundlePopupManager.instance.onBoardBundleStartedHandler += OnChangedCurrentlyShownBoardModel;
			BoardBundlePopupManager.instance.onBoardBundleExpiredHandler += OnChangedCurrentlyShownBoardModel;
			UIScreenController.OnApplicationResumed += OnChangedCurrentlyShownBoardModel;
			UIModelController.OnChangedCurrentlyShownHoverboard += OnChangedCurrentlyShownBoardModel;
			AdSystemManager.onCachedVideo += OnCachedVideo;
			AdSystemManager.onFailedToCacheVideo += OnFailedToCacheVideo;
		}

		private void OnDisable()
		{
			if (_shouldLogWatchVideoData)
			{
				SendFlurryLogForBirthdayBoard();
			}
			SSEventsManagerController.OnSSEventStartedHandler -= OnChangedCurrentlyShownBoardModel;
			SSEventsManagerController.OnSSEventFinishedHandler -= OnChangedCurrentlyShownBoardModel;
			BoardBundlePopupManager.instance.onBoardBundleStartedHandler -= OnChangedCurrentlyShownBoardModel;
			BoardBundlePopupManager.instance.onBoardBundleExpiredHandler -= OnChangedCurrentlyShownBoardModel;
			UIScreenController.OnApplicationResumed -= OnChangedCurrentlyShownBoardModel;
			UIModelController.OnChangedCurrentlyShownHoverboard -= OnChangedCurrentlyShownBoardModel;
			AdSystemManager.onCachedVideo -= OnCachedVideo;
			AdSystemManager.onFailedToCacheVideo -= OnFailedToCacheVideo;
		}

		public void SendFlurryLogForBirthdayBoard()
		{
			_shouldLogWatchVideoData = false;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Number of retries", _videoCacheRetry.ToString());
			dictionary.Add("Did watch ad", (!_didWatchVideo) ? "no" : "yes");
			string text = string.Empty;
			if (_screenContext == BoardManager.ScreenContext.BoardScreen)
			{
				text = "Birthday Board showed on Board screen";
			}
			else if (_screenContext == BoardManager.ScreenContext.BoardPromotionPopup)
			{
				text = "Birthday Board showed in popup";
			}
			if (!string.IsNullOrEmpty(text))
			{
				Flurry.LogEventWithSeveralParameters(text, dictionary);
			}
			_didWatchVideo = false;
			_videoCacheRetry = 0;
		}

		public void InitButton(BoardManager.ScreenContext newContext)
		{
			if (!_hasInited)
			{
				_screenContext = newContext;
				_uiButtonOverlayOff = GetComponent<UIButtonOverlayOff>();
				OnChangedCurrentlyShownBoardModel();
				_hasInited = true;
			}
		}

		public bool DoesCurrentBoardMatchEvent()
		{
			return CurrentlyShownBoardMatchesLimitedExpiredBoardEvent() || CurrentlyShownBoardMatchesTrySpecialPowerBoard();
		}

		private bool CurrentlyShownBoardMatchesTrySpecialPowerBoard()
		{
			return BoardManager.IsBoardTrySpecialPowerBoard(_boardId);
		}

		private bool CurrentlyShownBoardMatchesLimitedExpiredBoardEvent()
		{
			return BoardManager.IsBoardEventLimitedExpiredBoard(BoardManager.GetCurrentlyShownBoard(_screenContext));
		}

		private bool CurrentlyShownBoardMatchesBoardBundleEvent()
		{
			bool result = false;
			BoardBundlePromotion boardBundlePromotion = PromotionManager.Instance.TryGetCurrentPromotion() as BoardBundlePromotion;
			if (boardBundlePromotion != null && boardBundlePromotion.IsActive)
			{
				Boards.BoardId boardId = boardBundlePromotion.BoardId;
				if (boardId == BoardManager.GetCurrentlyShownBoard(_screenContext))
				{
					result = true;
				}
			}
			return result;
		}

		private void OnChangedCurrentlyShownBoardModel()
		{
			Boards.BoardId currentlyShownBoard = BoardManager.GetCurrentlyShownBoard(_screenContext);
			if (_boardId != currentlyShownBoard)
			{
				_currentThemeIndex = 0;
				BoardScreenManager.Instance.shouldResetThemeSelection = true;
			}
			_boardId = currentlyShownBoard;
			Boards.Board board = Boards.boardData[_boardId];
			if (_shouldLogWatchVideoData && board.unlockType != PurchaseData.UnlockType.WatchVideo)
			{
				SendFlurryLogForBirthdayBoard();
			}
			_shouldShowEventIndicatorsForTryBoardsEvent = CurrentlyShownBoardMatchesTrySpecialPowerBoard();
			_shouldShowEventIndicatorsForBuyLimitedBoardsEvent = CurrentlyShownBoardMatchesLimitedExpiredBoardEvent();
			_shouldShowExpirationTimeForBoardBundleSale = CurrentlyShownBoardMatchesBoardBundleEvent();
			if (_currentThemeIndex == 0)
			{
				if (board.unlockType == PurchaseData.UnlockType.AlwaysUnlocked || BoardManager.IsBoardOwned(_boardId) || board.unlockType == PurchaseData.UnlockType.WeeklyHunt)
				{
					SetButtonState(ButtonStates.Select);
				}
				else if (board.isPartOfBundle && !UIScreenController.Instance.isShowingPopup && !BoardManager.IsBoardOwned(_boardId))
				{
					SetButtonState(ButtonStates.View);
				}
				else if (_shouldShowEventIndicatorsForTryBoardsEvent)
				{
					SetButtonState(ButtonStates.Trial);
				}
				else if (_shouldShowEventIndicatorsForBuyLimitedBoardsEvent || board.unlockType == PurchaseData.UnlockType.InApp || _shouldShowExpirationTimeForBoardBundleSale)
				{
					SetButtonState(ButtonStates.BuyWithInapp);
				}
				else if (board.unlockType == PurchaseData.UnlockType.WatchVideo)
				{
					SetButtonState(ButtonStates.WatchVideo);
					_shouldLogWatchVideoData = true;
				}
				else if (board.unlockType == PurchaseData.UnlockType.Redirect)
				{
					SetButtonState(ButtonStates.GetNow);
				}
				else
				{
					SetButtonState(ButtonStates.Buy);
				}
				return;
			}
			HoverboardThemes.ThemeIdentifier themeIdentifier = HoverboardThemes.GetThemeIdentifier(_boardId, _currentThemeIndex - 1);
			if (BoardManager.IsBoardOwned(_boardId))
			{
				if (BoardManager.IsThemeOwned(_boardId, themeIdentifier))
				{
					if (BoardManager.IsThemeActive(_boardId, themeIdentifier))
					{
						SetButtonState(ButtonStates.Unselect);
					}
					else
					{
						SetButtonState(ButtonStates.Select);
					}
				}
				else
				{
					SetButtonState(ButtonStates.Buy);
				}
			}
			else if (Boards.boardData[_boardId].isPartOfBundle)
			{
				SetButtonState(ButtonStates.View);
			}
			else
			{
				SetButtonState(ButtonStates.LockedTheme);
			}
		}

		private void SetTryHoverboardUI()
		{
			if (!SSEventsManagerController.instance.HasTheCurrentEventBeenTried() || (_shouldShowEventIndicatorsForTryBoardsEvent && BoardManager.GetCurrentlyShownBoard(_screenContext) != PlayerSave.currentBoardId))
			{
				SetToSelectGreenState();
			}
			else
			{
				SetToSelectedBlueState();
			}
			_coinSprite.gameObject.SetActive(false);
			_keySprite.gameObject.SetActive(false);
		}

		private void SetSelectHoverboardUI()
		{
			if (PlayerSave.currentBoardId == _boardId)
			{
				SetToSelectedBlueState();
			}
			else if (BoardManager.IsBoardOwned(_boardId) || Boards.boardData[_boardId].unlockType == PurchaseData.UnlockType.AlwaysUnlocked)
			{
				SetToSelectGreenState();
			}
			else
			{
				SetToLockedBoard();
			}
		}

		private void SetBuyHoverboardUI()
		{
			SetToSelectGreenState();
			Boards.Board board = Boards.boardData[_boardId];
			if (board.price == 0)
			{
				_priceLabel.text = Strings.Get("FREE_WITH_EXCLAMATION");
				_coinSprite.gameObject.SetActive(false);
				_keySprite.gameObject.SetActive(false);
			}
			else
			{
				_priceLabel.text = board.price.ToString();
				_coinSprite.gameObject.SetActive(board.unlockType == PurchaseData.UnlockType.Coins);
				_keySprite.gameObject.SetActive(board.unlockType == PurchaseData.UnlockType.Keys);
			}
			_buyLayout.Align();
		}

		private void SetBuyBoardThemeUI()
		{
			SetToSelectGreenState();
			if (_currentThemeIndex == 0)
			{
				SDebug.LogError("Trying to set up 'buy theme' button while theme index is 0. Aborting.");
				return;
			}
			HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[_boardId][_currentThemeIndex - 1];
			if (hoverboardTheme.price == 0)
			{
				_priceLabel.text = Strings.Get("FREE_WITH_EXCLAMATION");
				_coinSprite.gameObject.SetActive(false);
				_keySprite.gameObject.SetActive(false);
			}
			else
			{
				_coinSprite.gameObject.SetActive(hoverboardTheme.unlockType == PurchaseData.UnlockType.Coins);
				_keySprite.gameObject.SetActive(hoverboardTheme.unlockType == PurchaseData.UnlockType.Keys);
				_priceLabel.text = hoverboardTheme.price.ToString();
			}
			_buyLayout.Align();
		}

		private void SetSelectBoardThemeUI()
		{
			SetToSelectGreenState();
		}

		private void SetUnselectBoardThemeUI()
		{
			SetToUnselectBlueState();
		}

		private void SetSelectLockedTheme()
		{
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
			_uiButtonOverlayOff.GoToNotAvailableState();
			_lockedBoardContent.SetActive(false);
			_labelSelected.gameObject.SetActive(false);
			HoverboardThemes.HoverboardTheme hoverboardTheme = HoverboardThemes.hoverboardCustomThemes[_boardId][_currentThemeIndex - 1];
			_notAvailableThemeAmount.text = hoverboardTheme.price.ToString();
			_lockedCoinSprite.gameObject.SetActive(hoverboardTheme.unlockType == PurchaseData.UnlockType.Coins);
			_lockedKeySprite.gameObject.SetActive(hoverboardTheme.unlockType == PurchaseData.UnlockType.Keys);
			_notAvailableThemeFeedback.text = Strings.Get(Boards.boardData[_boardId].name).ToUpper();
			if (hoverboardTheme.price == 0)
			{
				_notAvailableThemeAmount.text = Strings.Get("FREE_WITH_EXCLAMATION");
				_lockedCoinSprite.gameObject.SetActive(false);
				_lockedKeySprite.gameObject.SetActive(false);
			}
			_themeToplayout.Align();
			_themeBottomLayout.Align();
		}

		private void BuyHoverboardWithInapp()
		{
			SetToSelectGreenState();
			_coinSprite.gameObject.SetActive(false);
			_keySprite.gameObject.SetActive(false);
			_labelBuyWithInapp.text = Strings.Get("BUY_INAPP_NOPRICE_DEFAULT");
			string key = InAppData.ConvertBoardTypeToDynamicPurchaseId(_boardId);
			if (InAppData.inAppData.ContainsKey(key))
			{
				_labelBuyWithInapp.text = InAppData.inAppData[key].localizedPrice;
			}
		}

		private void SetToSelectGreenState()
		{
			_lockedBoardContent.SetActive(false);
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Primary);
			_uiButtonOverlayOff.GoToNormalState();
			if (_currentThemeIndex == 0)
			{
				if (_shouldShowEventIndicatorsForTryBoardsEvent)
				{
					_labelTry.text = Strings.Get("SELECT_BUTTON_TRY_FOR_FREE");
					return;
				}
				_labelSelected.text = Strings.Get("SELECT_BUTTON_SELECT");
				_labelSelected.gameObject.SetActive(true);
			}
			else
			{
				_labelSelected.text = Strings.Get("HOVERBOARD_THEME_SELECT_BUTTON_TEXT");
				_labelSelected.gameObject.SetActive(true);
			}
		}

		private void SetToViewState()
		{
			SetToSelectGreenState();
			_viewLabel.text = Strings.Get("STORE_EVENT_CELL_BUTTON_VIEW");
		}

		private void SetToSelectedBlueState()
		{
			_lockedBoardContent.SetActive(false);
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
			_uiButtonOverlayOff.GoToSelectedState();
			if (_shouldShowEventIndicatorsForTryBoardsEvent)
			{
				_labelTry.text = Strings.Get("SELECT_BUTTON_SELECTED");
				return;
			}
			_labelSelected.text = Strings.Get("SELECT_BUTTON_SELECTED");
			_labelSelected.gameObject.SetActive(true);
		}

		private void SetToUnselectBlueState()
		{
			_lockedBoardContent.SetActive(false);
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
			_uiButtonOverlayOff.GoToNormalState();
			_labelUnselect.text = Strings.Get("HOVERBOARD_THEME_BUTTON_UNSELECT");
		}

		private void SetToLockedBoard()
		{
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
			_uiButtonOverlayOff.GoToNotAvailableState();
			_labelSelected.gameObject.SetActive(false);
			Boards.Board board = Boards.boardData[_boardId];
			if (board.unlockType == PurchaseData.UnlockType.WeeklyHunt)
			{
				_progressLabel.text = string.Format("{0} / {1}", WeeklyHuntManager.ItemsCollectedForBoardType(_boardId), WeeklyHuntManager.ItemsToCollectForBoardType(_boardId));
			}
			_lockedBoardContent.SetActive(true);
			_tokenSprite.spriteName = board.tokenSprite2dName;
			_tokenSprite.MakePixelPerfect();
			_lockedBoardLayout.Align();
		}

		private void SetWatchVideo()
		{
			switch (_currentVideoState)
			{
			case VideoState.Ready:
				SetToSelectGreenState();
				_videoSprite.SetActive(true);
				_loadingSprite.enabled = false;
				if ((bool)_spacingSprite)
				{
					_spacingSprite.enabled = false;
				}
				_watchVideoSubLabel.enabled = false;
				_watchVideoLabel.text = Strings.Get("GET_NOW");
				break;
			case VideoState.Failed:
				SetToUnselectBlueState();
				_videoSprite.SetActive(false);
				_loadingSprite.enabled = false;
				if ((bool)_spacingSprite)
				{
					_spacingSprite.enabled = true;
				}
				_watchVideoSubLabel.enabled = true;
				_watchVideoLabel.text = Strings.Get("BUTTON_TRY_AGAIN");
				_watchVideoSubLabel.text = Strings.Get("CONNECTION_PROBLEM");
				break;
			case VideoState.Loading:
				SetToLockedBoard();
				_videoSprite.SetActive(false);
				_loadingSprite.enabled = true;
				if ((bool)_spacingSprite)
				{
					_spacingSprite.enabled = false;
				}
				_watchVideoSubLabel.enabled = false;
				_watchVideoLabel.text = Strings.Get("AD_OVERLAY_LOADING");
				break;
			}
			_watchVideoLayout.Align();
			_watchVideoLabelLayout.Align();
		}

		public void OnClick()
		{
			switch (_activeState)
			{
			case ButtonStates.Buy:
				if (!_purchaseInProgress)
				{
					_purchaseInProgress = true;
					if (_currentThemeIndex == 0)
					{
						PurchaseHandler.Instance.PurchaseHoverboard(_boardId, this);
					}
					else
					{
						PurchaseHandler.Instance.PurchaseHoverboardTheme(_boardId, _currentThemeIndex - 1, this);
					}
				}
				break;
			case ButtonStates.Select:
				if (_currentThemeIndex != 0)
				{
					ActivateTheme(_boardId, _currentThemeIndex - 1, true);
					OnChangedCurrentlyShownBoardModel();
				}
				BoardManager.SelectCurrentlyShownBoard(_screenContext);
				break;
			case ButtonStates.Trial:
			{
				IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SpecialPowerBoard);
				if (activeEventManager == null)
				{
					break;
				}
				PlayerSave.previousBoardId = PlayerSave.currentBoardId;
				EventsSave.shouldRestoreItemAfterEvent = true;
				EventsSave.lastEventID = activeEventManager.GetEventID();
				bool found;
				Boards.BoardId eventValue = activeEventManager.GetEventValue<Boards.BoardId>(out found);
				if (found)
				{
					EventsSave.lastEventItemID = eventValue.ToString();
				}
				OnChangedCurrentlyShownBoardModel();
				BoardManager.SelectCurrentlyShownBoard(_screenContext);
				if (!SSEventsManagerController.instance.HasTheCurrentEventBeenTried())
				{
					UIScreenController.Instance.ShowUnlockAnimationForHoverboard(PlayerSave.currentBoardId);
					SSEventsManagerController.instance.CurrentEventHasBeenTriedNow();
					if (found)
					{
						Flurry.LogEventWithAParameter("Event Popup was shown", "Id", eventValue.ToString().ToUpper());
					}
				}
				break;
			}
			case ButtonStates.BuyWithInapp:
				if (!_purchaseInProgress)
				{
					_purchaseInProgress = true;
					PurchaseHandler.Instance.StartPurchaseBoardWithInapp(BoardManager.GetCurrentlyShownBoard(_screenContext), this);
				}
				break;
			case ButtonStates.Unselect:
				DeactivateTheme(_boardId, _currentThemeIndex - 1);
				break;
			case ButtonStates.WatchVideo:
				if (_currentVideoState == VideoState.Ready)
				{
					AdSystemManager.Show(AdLocations.BOARD_BUTTON_VIDEOS, OnFinishedWatchingVideo);
				}
				else if (_currentVideoState == VideoState.Failed)
				{
					_currentVideoState = VideoState.Loading;
					SetWatchVideo();
					_videoCacheRetry++;
				}
				break;
			case ButtonStates.GetNow:
				UIScreenController.Instance.QueuePopup("WelcomePackPopup");
				break;
			case ButtonStates.View:
				UIScreenController.Instance.PushPopup("BoardBundlePopup");
				break;
			default:
				SDebug.LogError("State " + _activeState.ToString() + " is not handled");
				break;
			}
		}

		private void ActivateTheme(Boards.BoardId boardId, int themeIndex, bool allowSignatureAnimation)
		{
			BoardManager.SetThemeActive(boardId, HoverboardThemes.GetThemeIdentifier(boardId, themeIndex), true);
			Flurry.LogEventWithAParameter("Hoverboard Theme activated", "Id", HoverboardThemes.GetThemeIdentifier(boardId, themeIndex).ToString());
			OnChangedCurrentlyShownBoardModel();
			Hoverboard.BoardAbility boardAbility = (allowSignatureAnimation ? HoverboardThemes.hoverboardCustomThemes[boardId][themeIndex].power : Hoverboard.BoardAbility.None);
			BoardManager.ChangeThemeActiveState(_screenContext, boardAbility);
		}

		private void DeactivateTheme(Boards.BoardId boardId, int themeIndex)
		{
			BoardManager.SetThemeActive(boardId, HoverboardThemes.GetThemeIdentifier(boardId, themeIndex), false);
			Flurry.LogEventWithAParameter("Hoverboard Theme deactivated", "Id", HoverboardThemes.GetThemeIdentifier(boardId, themeIndex).ToString());
			OnChangedCurrentlyShownBoardModel();
			BoardManager.ChangeThemeActiveState(_screenContext);
		}

		private void OnFailedToCacheVideo(AdCacheSlotInfo adCacheSlotInfo)
		{
			Boards.Board board = Boards.boardData[_boardId];
			if (board.unlockType == PurchaseData.UnlockType.WatchVideo)
			{
				_currentVideoState = VideoState.Failed;
				SetWatchVideo();
			}
		}

		private void OnCachedVideo(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
		{
			Boards.Board board = Boards.boardData[_boardId];
			if (board.unlockType == PurchaseData.UnlockType.WatchVideo)
			{
				_currentVideoState = VideoState.Ready;
				SetWatchVideo();
			}
		}

		private void OnFinishedWatchingVideo()
		{
			Boards.Board board = Boards.boardData[_boardId];
			if (board.unlockType == PurchaseData.UnlockType.WatchVideo)
			{
				Timer.ExecuteNextFrame(() =>
				{
					UIScreenController.Instance.ShowUnlockAnimationForHoverboard(_boardId);
				});
				BoardManager.UnlockBoard(_boardId);
				BoardManager.SelectCurrentlyShownBoard(_screenContext);
				_didWatchVideo = true;
			}
		}

		public void PurchaseSuccessful(bool isBoardTheme = false, int themeIndex = 0)
		{
			_purchaseInProgress = false;
			if (isBoardTheme)
			{
				BoardThemePurchaseSuccessful(themeIndex);
				StoreScreen.RefreshOnNextShow();
			}
			else
			{
				BoardPurchaseSuccessful();
			}
			onPurchaseSuccessful.SafeInvoke();
		}

		private void BoardPurchaseSuccessful()
		{
			BoardManager.SelectCurrentlyShownBoard(_screenContext);
			Boards.Board board = Boards.boardData[_boardId];
			UIScreenController.Instance.ShowUnlockAnimationForHoverboard(_boardId);
			Flurry.LogEventWithAParameter("Hoverboard bought", "Id", char.ToUpper(Strings.Get(board.name)[0]) + Strings.Get(board.name).Substring(1));
			if (IsBoardOrThemeFree(_boardId))
			{
				Flurry.LogEventWithAParameter("Bread crumbs before purchasing the free hoverboard", "Total", UserSave.numberOfBreadCrumbsShownOnFrontPage + string.Empty);
			}
		}

		private void BoardThemePurchaseSuccessful(int themeIndex)
		{
			BoardManager.SelectCurrentlyShownBoard(_screenContext);
			if (BoardManager.IsThemeOwned(_boardId, HoverboardThemes.GetThemeIdentifier(_boardId, themeIndex)))
			{
				ActivateTheme(_boardId, themeIndex, false);
				UIScreenController.Instance.ShowUnlockAnimationForHoverboard(PlayerSave.currentBoardId, HoverboardThemes.hoverboardCustomThemes[_boardId][themeIndex].power);
				Flurry.LogEventWithAParameter("Hoverboard Theme bought", "Id", HoverboardThemes.GetThemeIdentifier(_boardId, themeIndex).ToString());
			}
			else
			{
				SDebug.LogError("Purchased theme and current theme differ.");
			}
		}

		public void PurchaseFailure()
		{
			_purchaseInProgress = false;
			onPurchaseFailed.SafeInvoke();
		}
	}
}
