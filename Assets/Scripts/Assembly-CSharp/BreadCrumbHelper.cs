using Data.Managers;
using Data.Save;
using InApp;
using Kiloo;
using Kiloo.Common.Ads;
using Missions;
using TopRun;
using UI;
using UI.Screens;
using UI.VideoAdPrizes;
using UnityEngine;

public class BreadCrumbHelper : MonoBehaviour
{
	private enum BreadCrumbStates
	{
		inactive = 0,
		giftCharacter = 1,
		giftBoard = 2,
		boardEventActive = 3,
		expiredLimitedCharacter = 4,
		biggerJackpot = 5,
		earnKeys = 6,
		collectAward = 7,
		expiredLimitedBoard = 8,
		topRun = 9,
		characterDay = 10,
		facebookGift = 11,
		sale = 12,
		specialSale = 13,
		collectableCharacter = 14,
		dailyGift = 15
	}

	private enum BreadCrumbLocations
	{
		meButton = 0,
		shopButton = 1,
		hoverboardsButton = 2,
		charactersButton = 3,
		upgradesFooter = 4,
		upgradesScroll = 5,
		earnCoinsFooterButton = 6,
		earnCoinsVideoButton = 7,
		mysteryBoxPrizeList = 8,
		awardsButton = 9,
		friendsButton = 10,
		topRunButton = 11,
		topRunTab = 12,
		facebookGiftButton = 13,
		characterCollectButton = 14
	}

	[SerializeField]
	private UISprite _sticker;

	[SerializeField]
	private UISprite _stickerShadow;

	[SerializeField]
	private UISprite _giftIcon;

	[SerializeField]
	private UISprite _earnKeysIcon;

	[SerializeField]
	private UISprite _eventIcon;

	[SerializeField]
	private UISprite _awardIcon;

	[SerializeField]
	private UISprite _topRunIcon;

	[SerializeField]
	private UISprite _collectableCharacterIcon;

	[SerializeField]
	private GameObject _notification;

	[SerializeField]
	private GameObject _discount;

	private BreadCrumbStates _currentState;

	[SerializeField]
	private BreadCrumbLocations _currentLocation;

	private bool _isApplicationResuming;

	private bool _dailyGiftScrolled;

	private bool _biggerJackpotScrolled;

	private bool _saleScrolled;

	public bool IsAnyBreadCrumbActive
	{
		get
		{
			return _currentState != BreadCrumbStates.inactive;
		}
	}

	private void OnEnable()
	{
		SSEventsManagerController.OnSSEventStartedHandler += SetupBreadCrumbState;
		SSEventsManagerController.OnSSEventChangedHandler += SetupBreadCrumbState;
		FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += SetupBreadCrumbState;
		SSEventsManagerController.OnSSEventFinishedHandler += DisableBreadCrumbElements;
		UIScreenController.OnApplicationResumed += DisableEventBreadcrumb;
		if (_currentLocation == BreadCrumbLocations.earnCoinsFooterButton)
		{
			AdSystemManager.onCachedVideo += OnVideoStateChanged;
			AdSystemManager.onFailedToCacheVideo += OnVideoStateChanged;
		}
		if (_currentLocation == BreadCrumbLocations.shopButton)
		{
			InAppManager.instance.onProductRequestDone += SetupBreadCrumbState;
		}
		SetupBreadCrumbState();
	}

	private void OnDisable()
	{
		SSEventsManagerController.OnSSEventStartedHandler -= SetupBreadCrumbState;
		SSEventsManagerController.OnSSEventChangedHandler -= SetupBreadCrumbState;
		FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive -= SetupBreadCrumbState;
		SSEventsManagerController.OnSSEventFinishedHandler -= DisableBreadCrumbElements;
		UIScreenController.OnApplicationResumed -= DisableEventBreadcrumb;
		if (_currentLocation == BreadCrumbLocations.earnCoinsFooterButton)
		{
			AdSystemManager.onCachedVideo -= OnVideoStateChanged;
			AdSystemManager.onFailedToCacheVideo -= OnVideoStateChanged;
		}
		if (_currentLocation == BreadCrumbLocations.shopButton && InAppManager.instance != null)
		{
			InAppManager.instance.onProductRequestDone -= SetupBreadCrumbState;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		_PauseApplication(paused);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			_PauseApplication(!focus);
		}
	}

	private void _PauseApplication(bool paused)
	{
		if (!paused)
		{
			_isApplicationResuming = true;
		}
	}

	private void LateUpdate()
	{
		if (_isApplicationResuming)
		{
			_isApplicationResuming = false;
			SetupBreadCrumbState();
		}
	}

	private void OnVideoStateChanged(AdCacheSlotInfo adCacheSlotInfo)
	{
		OnVideoStateChanged(adCacheSlotInfo, false);
	}

	private void OnVideoStateChanged(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
	{
		SetupBreadCrumbState();
	}

	private void SetupBreadCrumbStateGameOverTab(GameOverScreen.ActiveTabState tabState)
	{
		SetupBreadCrumbState();
	}

	public void SetupBreadCrumbState()
	{
		_currentState = BreadCrumbStates.inactive;
		switch (_currentLocation)
		{
		case BreadCrumbLocations.awardsButton:
			SetAwardsBreadcrumbState();
			break;
		case BreadCrumbLocations.charactersButton:
			SetCharacterBreadcrumbState();
			break;
		case BreadCrumbLocations.earnCoinsFooterButton:
			SetEarnCoinsBreadcrumbState();
			break;
		case BreadCrumbLocations.earnCoinsVideoButton:
			SetEarnCoinsVideoBreadcrumbState();
			break;
		case BreadCrumbLocations.facebookGiftButton:
			SetFacebookGiftBreadcrumbState();
			break;
		case BreadCrumbLocations.hoverboardsButton:
			SetHoverboardBreadcrumbState();
			break;
		case BreadCrumbLocations.meButton:
			SetMeBreadcrumbState();
			break;
		case BreadCrumbLocations.mysteryBoxPrizeList:
			SetUpgradesBreadcrumbState(true);
			break;
		case BreadCrumbLocations.shopButton:
			SetShopBreadcrumbState();
			break;
		case BreadCrumbLocations.friendsButton:
		case BreadCrumbLocations.topRunButton:
		case BreadCrumbLocations.topRunTab:
			SetTopRunBreadcrumbState();
			break;
		case BreadCrumbLocations.upgradesFooter:
			SetUpgradesBreadcrumbState(false);
			break;
		case BreadCrumbLocations.upgradesScroll:
			SetUpgradesBreadcrumbState(true);
			break;
		case BreadCrumbLocations.characterCollectButton:
			SetCharacterCollectButtonState();
			break;
		}
		SetupBreadCrumb();
	}

	private void SetMeBreadcrumbState()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		SSEventType sSEventType = SSEventType.None;
		if (activeEventManager != null)
		{
			sSEventType = activeEventManager.GetEventType();
		}
		if (AwardsManager.instance.GetAwardsForState(AwardState.Collectible).Count > 0)
		{
			_currentState = BreadCrumbStates.collectAward;
			return;
		}
		switch (sSEventType)
		{
		case SSEventType.SpecialPowerBoard:
			_currentState = BreadCrumbStates.boardEventActive;
			return;
		case SSEventType.ExpiredLimitedCharacter:
			_currentState = BreadCrumbStates.expiredLimitedCharacter;
			return;
		case SSEventType.CharacterDay:
			_currentState = BreadCrumbStates.characterDay;
			return;
		case SSEventType.ExpiredLimitedBoard:
			_currentState = BreadCrumbStates.expiredLimitedBoard;
			return;
		}
		if (ShouldShowBoardsGiftSticker())
		{
			_currentState = BreadCrumbStates.giftBoard;
		}
		else if (ShouldShowCharactersGiftSticker())
		{
			_currentState = BreadCrumbStates.giftCharacter;
		}
	}

	private void SetHoverboardBreadcrumbState()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		string text = UIScreenController.Instance.GetTopScreenName() ?? string.Empty;
		if (!text.Equals("BoardScreen") && !SSEventsManagerController.instance.HasTheCurrentEventBeenTried())
		{
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.SpecialPowerBoard)
			{
				_currentState = BreadCrumbStates.boardEventActive;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.ExpiredLimitedBoard)
			{
				_currentState = BreadCrumbStates.expiredLimitedBoard;
			}
			else if (ShouldShowBoardsGiftSticker())
			{
				_currentState = BreadCrumbStates.giftBoard;
			}
		}
	}

	private void SetCharacterBreadcrumbState()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		string text = UIScreenController.Instance.GetTopScreenName() ?? string.Empty;
		if (!text.Equals("CharacterScreen"))
		{
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.ExpiredLimitedCharacter)
			{
				_currentState = BreadCrumbStates.expiredLimitedCharacter;
			}
			else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterDay)
			{
				_currentState = BreadCrumbStates.characterDay;
			}
			else if (ShouldShowCharactersGiftSticker())
			{
				_currentState = BreadCrumbStates.giftCharacter;
			}
		}
	}

	private void SetAwardsBreadcrumbState()
	{
		string text = UIScreenController.Instance.GetTopScreenName() ?? string.Empty;
		if (!text.Equals("AwardsScreen") && AwardsManager.instance.GetAwardsForState(AwardState.Collectible).Count > 0)
		{
			_currentState = BreadCrumbStates.collectAward;
		}
	}

	private void SetUpgradesBreadcrumbState(bool isPopup)
	{
		string text = UIScreenController.Instance.GetTopScreenName() ?? string.Empty;
		if (text.Equals("UpgradeScreen") && !isPopup)
		{
		}
	}

	private void SetShopBreadcrumbState()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (UserSave.dailyGiftLastGiven.Date < CDateTime.Now.Date)
		{
			_currentState = BreadCrumbStates.dailyGift;
			if (!_dailyGiftScrolled)
			{
				_dailyGiftScrolled = true;
				StoreScreen.ScrollToInterestPointOnNextShow(StoreInterestPoint.MysteryBox);
			}
		}
		else if ((bool)_discount && InAppManager.IsAnyDiscountAvailable && InAppManager.IsSpecialSaleActive)
		{
			_currentState = BreadCrumbStates.specialSale;
		}
		else if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
		{
			_currentState = BreadCrumbStates.biggerJackpot;
			if (!_biggerJackpotScrolled)
			{
				_biggerJackpotScrolled = true;
				StoreScreen.ScrollToInterestPointOnNextShow(StoreInterestPoint.MysteryBox);
			}
		}
		else if ((bool)_discount && (InAppManager.IsDoubleCoinsDiscountActive || InAppManager.IsAnyDiscountAvailable))
		{
			_currentState = BreadCrumbStates.sale;
			if (!_saleScrolled)
			{
				_saleScrolled = true;
				StoreScreen.ScrollToInterestPointOnNextShow(StoreInterestPoint.Sale);
			}
		}
	}

	private void SetEarnCoinsBreadcrumbState()
	{
		string text = UIScreenController.Instance.GetTopScreenName() ?? string.Empty;
		if (!text.Equals("FreeStuffScreen"))
		{
			if (_giftIcon != null && FacebookWeeklyGiftManager.IsGiftAvailable())
			{
				_currentState = BreadCrumbStates.facebookGift;
			}
			else if (VideoAdPrizeManager.instance.IsVideoForKeysAvailable() && AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS))
			{
				_currentState = BreadCrumbStates.earnKeys;
			}
		}
	}

	private void SetEarnCoinsVideoBreadcrumbState()
	{
		if (VideoAdPrizeManager.instance.IsVideoForKeysAvailable() && AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS))
		{
			_currentState = BreadCrumbStates.earnKeys;
		}
	}

	private void SetFacebookGiftBreadcrumbState()
	{
		if (_giftIcon != null && FacebookWeeklyGiftManager.IsGiftAvailable())
		{
			_currentState = BreadCrumbStates.facebookGift;
		}
	}

	private void SetTopRunBreadcrumbState()
	{
		string text = UIScreenController.Instance.GetTopScreenName() ?? string.Empty;
		IManager manager = TopRunRoot.manager;
		if (manager == null || manager.tournament == null || !TopRunRewardHelper.ShouldShowResults())
		{
			return;
		}
		if (text.Equals("GameOverScreen"))
		{
			GameOverScreen gameOverScreen = UIScreenController.Instance.GetScreenFromCache(text) as GameOverScreen;
			if (gameOverScreen != null)
			{
				GameOverScreen.ActiveTabState activeTab = gameOverScreen.activeTab;
				gameOverScreen.onTabChanged += SetupBreadCrumbStateGameOverTab;
				if (activeTab == GameOverScreen.ActiveTabState.LeaderBoard)
				{
					_currentState = BreadCrumbStates.topRun;
				}
			}
		}
		else if (!text.Equals("TopRunUI"))
		{
			_currentState = BreadCrumbStates.topRun;
		}
	}

	private void SetCharacterCollectButtonState()
	{
		if (ShouldShowCollectableCharacterSticker())
		{
			_currentState = BreadCrumbStates.collectableCharacter;
		}
	}

	private bool ShouldShowBoardsGiftSticker()
	{
		return HoverboardStickerManager.ShouldShowHoverboardStickerFlow() && HoverboardStickerManager.IsAnyBoardOrAbilityFree();
	}

	private bool ShouldShowCharactersGiftSticker()
	{
		return CharacterStickerManager.ShouldShowCharacterThemesStickerFlow() && CharacterStickerManager.IsAnyCharacterWithFreeTheme();
	}

	private bool ShouldShowCollectableCharacterSticker()
	{
		return FirstPurchaseBonusManager.isCollectingFirstPurchaseBonus;
	}

	private void SetupBreadCrumb()
	{
		if (_currentState == BreadCrumbStates.inactive)
		{
			DisableBreadCrumbElements();
		}
		else
		{
			EnableBreadCrumbElements();
		}
	}

	private void TrySetColor(UIWidget widget, Color color)
	{
		if ((bool)widget)
		{
			widget.color = color;
		}
	}

	private bool TrySetActive(GameObject go, bool state)
	{
		if ((bool)go)
		{
			go.SetActive(state);
			return true;
		}
		return false;
	}

	private bool TrySetActive(UIWidget widget, bool state)
	{
		if ((bool)widget)
		{
			widget.enabled = state;
			return true;
		}
		return false;
	}

	private void EnableBreadCrumbElements()
	{
		if (_currentState == BreadCrumbStates.sale || _currentState == BreadCrumbStates.specialSale)
		{
			DisableBreadCrumbElements();
			TrySetActive(_discount, true);
			return;
		}
		if (TrySetActive(_sticker, true))
		{
			_sticker.color = GlobalColors.GIFT_COLOR;
		}
		TrySetActive(_stickerShadow, true);
		TrySetActive(_notification, false);
		TrySetActive(_awardIcon, _currentState == BreadCrumbStates.collectAward);
		TrySetActive(_topRunIcon, _currentState == BreadCrumbStates.topRun);
		TrySetActive(_eventIcon, _currentState == BreadCrumbStates.boardEventActive || _currentState == BreadCrumbStates.expiredLimitedCharacter || _currentState == BreadCrumbStates.characterDay || _currentState == BreadCrumbStates.biggerJackpot || _currentState == BreadCrumbStates.expiredLimitedBoard);
		TrySetActive(_giftIcon, _currentState == BreadCrumbStates.giftBoard || _currentState == BreadCrumbStates.giftCharacter || _currentState == BreadCrumbStates.facebookGift || _currentState == BreadCrumbStates.dailyGift);
		TrySetActive(_earnKeysIcon, _currentState == BreadCrumbStates.earnKeys);
		TrySetActive(_collectableCharacterIcon, _currentState == BreadCrumbStates.collectableCharacter);
		TrySetActive(_discount, false);
		Color32 color;
		switch (_currentState)
		{
		case BreadCrumbStates.giftCharacter:
		case BreadCrumbStates.giftBoard:
			color = GlobalColors.GIFT_COLOR;
			TrySetColor(_giftIcon, GlobalColors.GIFT_LABEL_COLOR);
			break;
		case BreadCrumbStates.earnKeys:
			color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			TrySetColor(_earnKeysIcon, GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR);
			break;
		case BreadCrumbStates.collectAward:
			color = GlobalColors.AWARDS_COLLECTIBLE_STICKER_COLOR;
			break;
		case BreadCrumbStates.topRun:
			color = GlobalColors.TOP_RUN_STICKER_COLOR;
			break;
		case BreadCrumbStates.facebookGift:
			color = GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG;
			TrySetColor(_giftIcon, GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT);
			break;
		case BreadCrumbStates.dailyGift:
			color = ColorData.instance.sun;
			TrySetColor(_giftIcon, ColorData.instance.mahogany);
			break;
		case BreadCrumbStates.collectableCharacter:
			color = GlobalColors.FIRST_PURCHASE_BONUS_BREADCRUMP_BUBBLE_COLOR;
			break;
		default:
			color = GlobalColors.GIFT_COLOR;
			break;
		}
		TrySetColor(_sticker, color);
		if (_currentLocation == BreadCrumbLocations.meButton)
		{
			UserSave.IncrementNumberOfBreadCrumbsShownOnFrontPage();
		}
	}

	private void DisableBreadCrumbElements()
	{
		TrySetActive(_sticker, false);
		TrySetActive(_eventIcon, false);
		TrySetActive(_stickerShadow, false);
		TrySetActive(_awardIcon, false);
		TrySetActive(_topRunIcon, false);
		TrySetActive(_notification, true);
		TrySetActive(_discount, false);
		TrySetActive(_giftIcon, false);
		TrySetActive(_collectableCharacterIcon, false);
		TrySetActive(_earnKeysIcon, false);
	}

	private void DisableEventBreadcrumb()
	{
		IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		bool flag = activeEventManager != null;
		if (base.gameObject.activeInHierarchy && !flag && (_currentState == BreadCrumbStates.boardEventActive || _currentState == BreadCrumbStates.characterDay || _currentState == BreadCrumbStates.expiredLimitedCharacter || _currentState == BreadCrumbStates.biggerJackpot))
		{
			DisableBreadCrumbElements();
		}
	}

	private void Update()
	{
		if (_currentState == BreadCrumbStates.collectAward && _stickerShadow != null && _sticker != null)
		{
			float zAngle = Time.deltaTime * 60f;
			_sticker.transform.Rotate(0f, 0f, zAngle);
			_stickerShadow.transform.Rotate(0f, 0f, zAngle);
		}
	}
}
