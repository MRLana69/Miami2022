using Data.Managers;
using Data.Save;
using InApp;
using Info;
using JetBrains.Annotations;
using SYBO.Subway.Info;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.Popups.LimitedCharacterPopupInternal;
using UI.Screens;
using UnityEngine;

namespace UI
{
	public class CharacterSelectButton : MonoBehaviour
	{
		private enum ButtonStates
		{
			Buy = 0,
			Select = 1,
			BuyWithInapp = 2,
			Facebook = 3,
			Collect = 4,
			TokenLocked = 5,
			View = 6
		}

		public enum ButtonContext
		{
			CharacterScreen = 0,
			CharacterNameEventPopup = 1,
			LimitedCharacterPopup = 2,
			CharacterPromotion = 3
		}

		public enum StickerType
		{
			None = 0,
			Event = 1,
			Limited = 2,
			Gift = 3
		}

		private UIButtonOverlayOff _uiButtonOverlayOff;

		[Header("Buy with Coins")]
		[SerializeField]
		private GameObject _buyContent;

		[SerializeField]
		private UILabel _priceLabel;

		[SerializeField]
		private UISprite _keySprite;

		[SerializeField]
		private UISprite _coinSprite;

		[SerializeField]
		private Layout _buyLayout;

		[Header("Select")]
		[SerializeField]
		private GameObject _selectContent;

		[SerializeField]
		private UILabel _labelSelected;

		[Space]
		[SerializeField]
		private GameObject _lockedThemeContent;

		[SerializeField]
		private Layout _themeTopLayout;

		[SerializeField]
		private Layout _themeBottomLayout;

		[SerializeField]
		private UISprite _notAvailableThemeCoin;

		[SerializeField]
		private UISprite _notAvailableThemeKey;

		[SerializeField]
		private UILabel _notAvailableThemeAmount;

		[SerializeField]
		private UILabel _notAvailableThemeFeedback;

		[Space]
		[SerializeField]
		private GameObject _lockedTokenContent;

		[SerializeField]
		private UILabel _progressLabel;

		[SerializeField]
		private UISprite _tokenSprite;

		[Space]
		[SerializeField]
		private GameObject _blockedTokenThemeContent;

		[SerializeField]
		private UILabel _unlockFirst;

		[Header("Buy with inapp")]
		[SerializeField]
		private GameObject _buyWithInappContent;

		[SerializeField]
		private UILabel _labelBuyWithInapp;

		[Header("Facebook login")]
		[SerializeField]
		private GameObject _facebookContent;

		[SerializeField]
		private GameObject _facebookOfflineContent;

		[SerializeField]
		private UILabel _facebookOfflineLabel;

		[SerializeField]
		private Layout _facebookOfflineLayout;

		[SerializeField]
		private GameObject _facebookLoadingContent;

		[SerializeField]
		private UILabel _facebookLoadingLabel;

		[SerializeField]
		private Layout _facebookLoadingLayout;

		[Header("Sticker")]
		[SerializeField]
		private GameObject _stickerContent;

		[SerializeField]
		private UISprite _backgroundSticker;

		[SerializeField]
		private UISprite _eventIcon;

		[SerializeField]
		private UISprite _giftIcon;

		[SerializeField]
		private UISprite _clockIcon;

		[Header("Collect")]
		[SerializeField]
		private GameObject _collectContent;

		[SerializeField]
		private UILabel _labelCollect;

		[SerializeField]
		private GameObject _smallFirstPurchaseBonusBannerGo;

		[Header("View Character")]
		[SerializeField]
		private GameObject _viewContent;

		[SerializeField]
		private Layout _viewLayout;

		[SerializeField]
		private UILabel _labelView;

		private Characters.CharacterId _currentlyShownModelId;

		private Characters.Model _modelData;

		private int _currentlyShownThemeIndex;

		private Outfit _modelOutfit;

		private PurchaseData.UnlockType _unlockType;

		private bool _isCharacterOwned;

		private bool _isThemeOwned;

		private int _unlockPrice;

		private const string TEXT_PROGRESS = "{0} / {1}";

		private bool _hasInited;

		private bool _buyCharacterWithIapEventIsRunning;

		private ButtonStates _activeState;

		private ButtonContext _buttonContext;

		private CharacterScreenManager _managerInstance;

		private void SetButtonState(ButtonStates state)
		{
			_activeState = state;
			_buyContent.SetActive(state == ButtonStates.Buy);
			_selectContent.SetActive(state == ButtonStates.Select || state == ButtonStates.TokenLocked);
			_buyWithInappContent.SetActive(state == ButtonStates.BuyWithInapp);
			_facebookContent.SetActive(state == ButtonStates.Facebook);
			_collectContent.SetActive(state == ButtonStates.Collect);
			_viewContent.SetActive(state == ButtonStates.View);
			switch (state)
			{
			case ButtonStates.Select:
			case ButtonStates.TokenLocked:
				UpdateUIForSelectState();
				break;
			case ButtonStates.BuyWithInapp:
				UpdateUIForBuyWithInappState();
				break;
			case ButtonStates.Facebook:
				UpdateUIForFacebookState();
				break;
			case ButtonStates.Buy:
				UpdateUIForBuyState();
				break;
			case ButtonStates.Collect:
				UpdateUIForCollectState();
				break;
			case ButtonStates.View:
				UpdateUIForViewState();
				break;
			}
		}

		public void InitButton(ButtonContext newContext)
		{
			_hasInited = true;
			_buttonContext = newContext;
			_managerInstance = CharacterScreenManager.Instance;
			_uiButtonOverlayOff = GetComponent<UIButtonOverlayOff>();
			ReloadButton();
		}

		public void SetSticker(StickerType type)
		{
			_stickerContent.SetActive(type != StickerType.None);
			_eventIcon.gameObject.SetActive(type == StickerType.Event);
			_giftIcon.gameObject.SetActive(type == StickerType.Gift);
			_clockIcon.gameObject.SetActive(type == StickerType.Limited);
			switch (type)
			{
			case StickerType.None:
				break;
			case StickerType.Event:
				_backgroundSticker.color = GlobalColors.GIFT_COLOR;
				_eventIcon.color = GlobalColors.GIFT_LABEL_COLOR;
				break;
			case StickerType.Limited:
				_backgroundSticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
				_clockIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
				break;
			case StickerType.Gift:
				_backgroundSticker.color = GlobalColors.GIFT_COLOR;
				_giftIcon.color = GlobalColors.GIFT_LABEL_COLOR;
				if (Characters.characterData.ContainsKey(_currentlyShownModelId) && Characters.characterData[_currentlyShownModelId].hasOnlineSettings)
				{
					_backgroundSticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
					_giftIcon.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
				}
				break;
			}
		}

		public bool CurrentlyShownCharacterMatchesEventItem()
		{
			return SSEventsManagerController.instance.DoesCharacterMatchCurrentEvent(UIModelController.Instance.currentlyShownModel);
		}

		public void ReloadButton(bool overrideCharacter = false, Characters.CharacterId newCharacter = Characters.CharacterId.slick)
		{
			if (!_hasInited)
			{
				SDebug.LogError("CharacterSelectButton not initialized. Aborting reload.", base.gameObject);
				return;
			}
			switch (_buttonContext)
			{
			case ButtonContext.CharacterScreen:
			{
				CharacterScreenManager instance2 = CharacterScreenManager.Instance;
				_currentlyShownModelId = instance2.currentlyShownCharacter;
				_currentlyShownThemeIndex = instance2.currentlyShownThemeIndex;
				break;
			}
			case ButtonContext.LimitedCharacterPopup:
			{
				LimitedCharacterPopupManager instance = LimitedCharacterPopupManager.Instance;
				_currentlyShownModelId = instance.currentCharacter;
				_currentlyShownThemeIndex = instance.currentThemeIndex;
				break;
			}
			case ButtonContext.CharacterPromotion:
			{
				CharacterPromotion characterPromotion = PromotionManager.Instance.TryGetCurrentPromotion<CharacterPromotion>();
				if (characterPromotion == null)
				{
					Debug.LogError("Current promotion isn't a character promotion");
					return;
				}
				_currentlyShownModelId = characterPromotion.CharacterId;
				_currentlyShownThemeIndex = characterPromotion.ThemeIndex;
				break;
			}
			case ButtonContext.CharacterNameEventPopup:
				if (overrideCharacter)
				{
					_currentlyShownModelId = newCharacter;
					if (Outfits.GetOutfitForCharacter(_currentlyShownModelId, CharacterManager.GetOutfitIndex(_currentlyShownModelId)) != null)
					{
						_currentlyShownThemeIndex = CharacterManager.GetOutfitIndex(_currentlyShownModelId);
					}
					else
					{
						_currentlyShownThemeIndex = 0;
					}
				}
				_stickerContent.SetActive(false);
				break;
			}
			_modelData = Characters.characterData[_currentlyShownModelId];
			if (_buttonContext == ButtonContext.CharacterScreen || _buttonContext == ButtonContext.LimitedCharacterPopup)
			{
				_modelOutfit = Outfits.GetOutfitForCharacter(_currentlyShownModelId, _currentlyShownThemeIndex);
			}
			else
			{
				_modelOutfit = null;
			}
			_unlockType = _modelData.unlockType;
			_isCharacterOwned = CharacterManager.IsCharacterOwned(_currentlyShownModelId);
			_unlockPrice = _modelData.Price;
			if (_modelOutfit != null)
			{
				Outfit modelOutfit = _modelOutfit;
				_unlockType = modelOutfit.unlockType;
				_isThemeOwned = CharacterManager.IsOutfitUnlocked(_currentlyShownModelId, _currentlyShownThemeIndex);
				_unlockPrice = modelOutfit.price;
			}
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.ExpiredLimitedCharacter);
			_buyCharacterWithIapEventIsRunning = CurrentlyShownCharacterMatchesEventItem() && (_isCharacterOwned || _modelOutfit == null) && activeEventManager != null;
			if (_unlockType == PurchaseData.UnlockType.Facebook && !Globals.DEBUG_ALL_CHARS)
			{
				SetButtonState(_isCharacterOwned ? ButtonStates.Select : ButtonStates.Facebook);
			}
			else if (_modelData.isPartOfBundle && !UIScreenController.Instance.isShowingPopup && !_isCharacterOwned && _currentlyShownThemeIndex == 0)
			{
				SetButtonState(ButtonStates.View);
			}
			else if (_unlockType == PurchaseData.UnlockType.Tokens && ((!_isCharacterOwned && _modelOutfit == null) || (!_isThemeOwned && _modelOutfit != null)))
			{
				SetButtonState(ButtonStates.TokenLocked);
			}
			else if ((_unlockType == PurchaseData.UnlockType.AlwaysUnlocked || (_isCharacterOwned && _modelOutfit == null) || (_isCharacterOwned && _isThemeOwned) || (!_isCharacterOwned && _modelOutfit != null)) && !_buyCharacterWithIapEventIsRunning)
			{
				SetButtonState(ButtonStates.Select);
			}
			else if (_buyCharacterWithIapEventIsRunning || _unlockType == PurchaseData.UnlockType.InApp)
			{
				SetButtonState(ButtonStates.BuyWithInapp);
			}
			else if (_unlockType == PurchaseData.UnlockType.RewardForInApp)
			{
				SetButtonState(ButtonStates.Collect);
			}
			else
			{
				SetButtonState(ButtonStates.Buy);
			}
		}

		private void UpdateUIForFacebookState()
		{
			if (CharacterScreenManager.Instance.IsFacebookLoginRunning())
			{
				SetToFacebookLoadingState();
			}
			else
			{
				SetToFacebookOfflineState();
			}
		}

		private void SetToFacebookLoadingState()
		{
			_facebookLoadingContent.SetActive(true);
			_facebookOfflineContent.SetActive(false);
			_facebookLoadingLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
			_facebookLoadingLayout.Align();
			SetUIToNotTappable();
		}

		private void SetToFacebookOfflineState()
		{
			_facebookLoadingContent.SetActive(false);
			_facebookOfflineContent.SetActive(true);
			_facebookOfflineLabel.text = Strings.Get("SOCIAL_BUTTON_LOGIN");
			_facebookOfflineLayout.Align();
			SetUIToSelectGreenState();
		}

		private void UpdateUIForBuyWithInappState()
		{
			SetUIToSelectGreenState();
			_labelBuyWithInapp.text = Strings.Get("BUY_INAPP_NOPRICE_DEFAULT");
			string key = InAppData.ConvertCharacterTypeToDynamicPurchaseId(_currentlyShownModelId);
			_labelBuyWithInapp.text = ((!InAppData.inAppData.ContainsKey(key)) ? Strings.Get("BUY_INAPP_NOPRICE_DEFAULT") : InAppData.inAppData[key].localizedPrice);
		}

		private void UpdateUIForSelectState()
		{
			bool flag = PlayerSave.currentCharacterId == _currentlyShownModelId;
			bool flag2 = _modelOutfit != null;
			if (flag && (_buttonContext == ButtonContext.CharacterScreen || _buttonContext == ButtonContext.LimitedCharacterPopup))
			{
				flag = PlayerSave.currentOutfitIndex == _currentlyShownThemeIndex;
			}
			if (flag)
			{
				SetUIToSelectedBlueState();
				return;
			}
			if (!_isThemeOwned && flag2 && _unlockType == PurchaseData.UnlockType.Tokens)
			{
				bool flag3 = false;
				string empty = string.Empty;
				if (_modelOutfit.uiPriority == 0)
				{
					flag3 = _isCharacterOwned;
					empty = Strings.Get(Characters.characterData[_currentlyShownModelId].name);
				}
				else
				{
					int outfitRequirementIndex = Outfits.GetOutfitRequirementIndex(_currentlyShownModelId, _currentlyShownThemeIndex);
					if (outfitRequirementIndex == -1)
					{
						flag3 = _isCharacterOwned;
						empty = Strings.Get(Characters.characterData[_currentlyShownModelId].name);
					}
					else
					{
						flag3 = CharacterManager.IsOutfitUnlocked(_currentlyShownModelId, outfitRequirementIndex + 1);
						empty = Strings.Get(Outfits.GetOutfitForCharacter(_currentlyShownModelId, outfitRequirementIndex + 1).title);
					}
				}
				if (flag3)
				{
					ShowLockedTokenForOutfitContent();
					_notAvailableThemeAmount.text = _modelOutfit.price.ToString();
					_notAvailableThemeCoin.enabled = false;
					_notAvailableThemeKey.enabled = false;
					if (!string.IsNullOrEmpty(empty))
					{
						_notAvailableThemeFeedback.text = empty.ToUpper();
					}
					_themeTopLayout.Align();
					_themeBottomLayout.Align();
				}
				else
				{
					SetUIToNotTappable();
					_lockedTokenContent.SetActive(false);
					_blockedTokenThemeContent.SetActive(true);
					_labelSelected.gameObject.SetActive(false);
					_unlockFirst.text = string.Format(Strings.Get("TOKEN_UNLOCK_OUTFIT_FIRST"), string.Empty, empty);
					_themeTopLayout.Align();
					_themeBottomLayout.Align();
				}
				return;
			}
			if ((_isCharacterOwned && !flag2) || (_isCharacterOwned && _isThemeOwned))
			{
				SetUIToSelectGreenState();
				return;
			}
			bool flag4 = _unlockType == PurchaseData.UnlockType.Tokens;
			if (_isCharacterOwned || (!flag4 && !flag2))
			{
				return;
			}
			SetUIToNotTappable();
			if (flag2)
			{
				_lockedTokenContent.SetActive(false);
				_blockedTokenThemeContent.SetActive(false);
			}
			else
			{
				ShowLockedTokenForCharacterContent();
			}
			_lockedThemeContent.SetActive(flag2);
			if (flag2)
			{
				Outfit modelOutfit = _modelOutfit;
				_notAvailableThemeAmount.text = modelOutfit.price.ToString();
				_notAvailableThemeCoin.enabled = _unlockType == PurchaseData.UnlockType.Coins;
				_notAvailableThemeKey.enabled = _unlockType == PurchaseData.UnlockType.Keys;
				if (_unlockPrice == 0)
				{
					_notAvailableThemeAmount.text = Strings.Get("FREE_WITH_EXCLAMATION");
					_notAvailableThemeCoin.enabled = false;
					_notAvailableThemeKey.enabled = false;
				}
				string text = Strings.Get(Characters.characterData[_currentlyShownModelId].name);
				if (!string.IsNullOrEmpty(text))
				{
					_notAvailableThemeFeedback.text = text.ToUpper();
				}
				_themeTopLayout.Align();
				_themeBottomLayout.Align();
			}
		}

		private void UpdateUIForCollectState()
		{
			SetUIToSelectGreenState();
			if (FirstPurchaseBonusManager.isCollectingFirstPurchaseBonus)
			{
				_smallFirstPurchaseBonusBannerGo.SetActive(false);
				_labelCollect.text = Strings.Get("COLLECT_FPB_TITLE");
			}
			else if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
			{
				_smallFirstPurchaseBonusBannerGo.SetActive(true);
				_labelCollect.text = Strings.Get("FIRST_PURCHASE_POPUP_BUTTON");
			}
			else
			{
				_smallFirstPurchaseBonusBannerGo.SetActive(false);
			}
		}

		private void UpdateUIForBuyState()
		{
			if (_modelOutfit != null)
			{
				Outfit modelOutfit = _modelOutfit;
				_unlockType = modelOutfit.unlockType;
				_isThemeOwned = CharacterManager.IsOutfitUnlocked(_currentlyShownModelId, _currentlyShownThemeIndex);
				_unlockPrice = modelOutfit.price;
			}
			if (_unlockType == PurchaseData.UnlockType.Tokens)
			{
				SetUIToNotTappable();
			}
			else
			{
				SetUIToSelectGreenState();
			}
			if (_unlockPrice > 0)
			{
				_priceLabel.text = _unlockPrice.ToString();
				_coinSprite.gameObject.SetActive(_unlockType == PurchaseData.UnlockType.Coins);
				_keySprite.gameObject.SetActive(_unlockType == PurchaseData.UnlockType.Keys);
			}
			else
			{
				_priceLabel.text = Strings.Get("FREE_WITH_EXCLAMATION");
				_coinSprite.gameObject.SetActive(false);
				_keySprite.gameObject.SetActive(false);
			}
			_buyLayout.Align();
		}

		private void UpdateUIForViewState()
		{
			if (_modelOutfit != null)
			{
				Outfit modelOutfit = _modelOutfit;
				_unlockType = modelOutfit.unlockType;
				_isThemeOwned = CharacterManager.IsOutfitUnlocked(_currentlyShownModelId, _currentlyShownThemeIndex);
				_unlockPrice = modelOutfit.price;
			}
			SetUIToSelectGreenState();
			_labelView.text = Strings.Get("STORE_EVENT_CELL_BUTTON_VIEW");
			_viewLayout.Align();
		}

		private void SetUIToSelectGreenState()
		{
			_lockedThemeContent.SetActive(false);
			_lockedTokenContent.SetActive(false);
			_blockedTokenThemeContent.SetActive(false);
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Primary);
			_uiButtonOverlayOff.GoToNormalState();
			_labelSelected.gameObject.SetActive(true);
			_labelSelected.text = Strings.Get("SELECT_BUTTON_SELECT");
		}

		private void SetUIToSelectedBlueState()
		{
			_lockedThemeContent.SetActive(false);
			_lockedTokenContent.SetActive(false);
			_blockedTokenThemeContent.SetActive(false);
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
			_uiButtonOverlayOff.GoToSelectedState();
			_labelSelected.gameObject.SetActive(true);
			_labelSelected.text = Strings.Get("SELECT_BUTTON_SELECTED");
		}

		private void SetUIToNotTappable()
		{
			_uiButtonOverlayOff.GoToNotAvailableState();
			_labelSelected.gameObject.SetActive(false);
		}

		private void ShowLockedTokenForCharacterContent()
		{
			_lockedTokenContent.SetActive(true);
			_blockedTokenThemeContent.SetActive(false);
			_uiButtonOverlayOff.GoToNormalState();
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
			_labelSelected.gameObject.SetActive(false);
			_progressLabel.text = string.Format("{0} / {1}", CharacterManager.GetCollectedTokens(_currentlyShownModelId), _modelData.Price);
			_tokenSprite.spriteName = Characters.characterData[_currentlyShownModelId].tokenSprite2dName;
			_tokenSprite.GetComponent<FitSpriteToWidget>().FitNow();
		}

		private void ShowLockedTokenForOutfitContent()
		{
			_lockedTokenContent.SetActive(true);
			_blockedTokenThemeContent.SetActive(false);
			_uiButtonOverlayOff.GoToNormalState();
			_uiButtonOverlayOff.SetButtonType(UIButtonOverlayOff.ButtonType.Tertiary);
			_labelSelected.gameObject.SetActive(false);
			_progressLabel.text = string.Format("{0} / {1}", CharacterManager.GetCollectedTokens(_currentlyShownModelId), _modelOutfit.price);
			_tokenSprite.spriteName = Characters.characterData[_currentlyShownModelId].tokenSprite2dName;
			_tokenSprite.GetComponent<FitSpriteToWidget>().FitNow();
		}

		[UsedImplicitly]
		public void OnClick()
		{
			if (_activeState == ButtonStates.Buy)
			{
				PurchaseCharacter(false);
			}
			else if (_activeState == ButtonStates.BuyWithInapp)
			{
				PurchaseCharacter(true);
			}
			else if (_activeState == ButtonStates.Facebook)
			{
				SetToFacebookLoadingState();
				_managerInstance.StartLoginToFacebook();
			}
			else if (_activeState == ButtonStates.Select)
			{
				_managerInstance.SelectCharacter(_currentlyShownModelId, _currentlyShownThemeIndex);
			}
			else if (_activeState == ButtonStates.TokenLocked)
			{
				UIScreenController.Instance.PushPopup("TokenMysteryBoxPopup");
			}
			else if (_activeState == ButtonStates.Collect)
			{
				if (UserSave.hasPurchasedOrRestored)
				{
					CharacterManager.CollectToken(_currentlyShownModelId, _unlockPrice);
					CharacterScreenManager.Instance.OnFirstPurchaseCharacterUnlocked();
				}
				else
				{
					UIScreenController.Instance.QueuePopup("StorePopup");
				}
			}
			else if (_activeState == ButtonStates.View)
			{
				LimitedCharacterPopupManager.Instance.currentCharacter = _currentlyShownModelId;
				LimitedCharacterPopupManager.Instance.currentThemeIndex = _currentlyShownThemeIndex;
				UIScreenController.Instance.PushPopup("LimitedCharacterBundlePopup");
			}
			else
			{
				SDebug.LogError("CharacterSelect was clicked with an unknown state: " + _activeState);
			}
		}

		private void PurchaseCharacter(bool withInAppPurchase)
		{
			switch (_buttonContext)
			{
			case ButtonContext.CharacterScreen:
				if (withInAppPurchase)
				{
					_managerInstance.PurchaseCharacterWithInapp(CharacterScreenManager.CharacterPurchaseInitiator.characterScreen, _currentlyShownModelId);
				}
				else
				{
					_managerInstance.PurchaseCharacter(CharacterScreenManager.CharacterPurchaseInitiator.characterScreen, _currentlyShownModelId, _currentlyShownThemeIndex, _unlockPrice);
				}
				break;
			case ButtonContext.LimitedCharacterPopup:
				if (Characters.characterData[_currentlyShownModelId].Price <= InventorySave.coins)
				{
					UIScreenController.Instance.CloseActivePopups("LimitedCharacterBundlePopup");
				}
				if (withInAppPurchase)
				{
					_managerInstance.PurchaseCharacterWithInapp(CharacterScreenManager.CharacterPurchaseInitiator.limitedCharacterPopup, _currentlyShownModelId);
				}
				else
				{
					_managerInstance.PurchaseCharacter(CharacterScreenManager.CharacterPurchaseInitiator.limitedCharacterPopup, _currentlyShownModelId, _currentlyShownThemeIndex);
				}
				CharacterScreen.forceCenterOnCurrentlySelectedCharacter = true;
				CharacterScreen.forceRefreshOnCharacterScreen = true;
				break;
			case ButtonContext.CharacterNameEventPopup:
				if (withInAppPurchase)
				{
					_managerInstance.PurchaseCharacterWithInapp(CharacterScreenManager.CharacterPurchaseInitiator.characterNameEvent, _currentlyShownModelId);
				}
				else
				{
					_managerInstance.PurchaseCharacter(CharacterScreenManager.CharacterPurchaseInitiator.characterNameEvent, _currentlyShownModelId, 0);
				}
				CharacterScreen.forceCenterOnCurrentlySelectedCharacter = true;
				CharacterScreen.forceRefreshOnCharacterScreen = true;
				break;
			case ButtonContext.CharacterPromotion:
				if (withInAppPurchase)
				{
					_managerInstance.PurchaseCharacterWithInapp(CharacterScreenManager.CharacterPurchaseInitiator.characterScreen, _currentlyShownModelId);
				}
				else
				{
					_managerInstance.PurchaseCharacter(CharacterScreenManager.CharacterPurchaseInitiator.characterScreen, _currentlyShownModelId, _currentlyShownThemeIndex, _unlockPrice);
				}
				break;
			}
		}
	}
}
