using System;
using System.Collections.Generic;
using Data.Managers;
using InApp;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Generic.Layout;
using UI.Popups.LimitedBoardPopupInternal;
using UI.Popups.LimitedCharacterPopupInternal;
using UI.RenderTexture;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	public class StoreEventContainerSmall : MonoBehaviour
	{
		public enum EventCellButtonAction
		{
			RedirectToScreen = 0,
			OpenPopup = 1,
			Buy = 2
		}

		[SerializeField]
		private bool _wideCell;

		[Header("Background")]
		[SerializeField]
		private UISprite _fill;

		[Header("Content")]
		[SerializeField]
		private VerticalLayout _rightLayout;

		[SerializeField]
		private UILabel _titleText;

		[SerializeField]
		private UILabel _descriptionText;

		[SerializeField]
		private UITexture _texture;

		[SerializeField]
		private GameObject _themeParent;

		[SerializeField]
		private VerticalLayout _themeLayout;

		[SerializeField]
		private UILabel _boardTheme1Titel;

		[SerializeField]
		private UISprite _boardTheme1Background;

		[SerializeField]
		private UISprite _boardTheme1Icon;

		[SerializeField]
		private GameObject _boardTheme1CheckmarkIcon;

		[SerializeField]
		private FitSpriteToWidget _boardTheme1IconFitter;

		[SerializeField]
		private UILabel _boardTheme2Titel;

		[SerializeField]
		private UISprite _boardTheme2Background;

		[SerializeField]
		private UISprite _boardTheme2Icon;

		[SerializeField]
		private GameObject _boardTheme2CheckmarkIcon;

		[SerializeField]
		private FitSpriteToWidget _boardTheme2IconFitter;

		[Header("SubContent")]
		[SerializeField]
		private RenderTextureObject _starObject;

		[Header("BuyButton")]
		[SerializeField]
		private UIButtonOverlayOff _buyButton;

		[SerializeField]
		private Layout _buyButtonLayout;

		[SerializeField]
		private UILabel _buyButtonLabel;

		[SerializeField]
		private UISprite _buyButtonPriceIcon;

		[SerializeField]
		private FitSpriteToWidget _buyButtonPriceIconFitter;

		[SerializeField]
		private UISprite _buyButtonCheckedSprite;

		[Header("Timer")]
		[SerializeField]
		private UILabel _timerTextLabel;

		[SerializeField]
		private AlignTimer _alignTimer;

		private StoreEventContainerPayload _payload;

		private PurchaseData.UnlockType _priceType = PurchaseData.UnlockType.Redirect;

		private string _purchaseID = string.Empty;

		private string _boardBundle1PurchaseID = string.Empty;

		private string _boardBundle2PurchaseID = string.Empty;

		private bool _subscribed;

		private void OnEnable()
		{
			if (_payload != null)
			{
				UpdateButton();
			}
			if (!_subscribed)
			{
				InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
				CharacterManager.characterUnlocked += OnCharacterUnlocked;
				BoardManager.boardUnlocked += OnBoardUnlocked;
				_subscribed = true;
			}
		}

		private void OnDisable()
		{
			if (_subscribed)
			{
				if (InAppManager.instance != null)
				{
					InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
				}
				CharacterManager.characterUnlocked -= OnCharacterUnlocked;
				BoardManager.boardUnlocked -= OnBoardUnlocked;
				_subscribed = false;
			}
		}

		public void Setup(StoreEventContainerPayload payload)
		{
			_payload = payload;
			_fill.gradientTop = payload.cellAppearancePayload.fillTopColor;
			_fill.gradientBottom = payload.cellAppearancePayload.fillBottomColor;
			_titleText.color = payload.cellAppearancePayload.titleColor;
			_titleText.effectColor = payload.cellAppearancePayload.titleEffectColor;
			if (_descriptionText != null)
			{
				_descriptionText.color = payload.cellAppearancePayload.subtitleColor;
				_descriptionText.effectColor = payload.cellAppearancePayload.subtitleEffectColor;
			}
			if (_boardTheme1Titel != null && _boardTheme2Titel != null)
			{
				_boardTheme1Titel.color = payload.cellAppearancePayload.boardThemeColor;
				_boardTheme1Titel.effectColor = payload.cellAppearancePayload.boardThemeEffectColor;
				_boardTheme2Titel.color = payload.cellAppearancePayload.boardThemeColor;
				_boardTheme2Titel.effectColor = payload.cellAppearancePayload.boardThemeEffectColor;
			}
			if (_timerTextLabel != null)
			{
				_timerTextLabel.text = Strings.Get("TIMER_ENDS_IN");
			}
			_alignTimer.SetTimerColor(ColorData.instance.mahogany);
			if (payload.useCharacter)
			{
				_titleText.text = Strings.Get(Characters.characterData[payload.character.charID].name);
			}
			else
			{
				_titleText.text = Strings.Get(Boards.boardData[payload.board.boardID].name);
			}
			if (_wideCell)
			{
				if (payload.useCharacter)
				{
					_descriptionText.text = Strings.Get(Characters.characterData[payload.character.charID].characterLimitedDescription);
				}
				else
				{
					_descriptionText.text = Strings.Get(Boards.boardData[payload.board.boardID].limitedDescription);
				}
				if (payload.isBoardBundle)
				{
					_boardTheme1Icon.spriteName = HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][0].iconName;
					_boardTheme1Icon.color = HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][0].buttonIconColor;
					_boardTheme1Background.color = HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][0].buttonBgColor;
					_boardTheme1Titel.text = Strings.Get(HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][0].title);
					_boardTheme2Icon.spriteName = HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][1].iconName;
					_boardTheme2Icon.color = HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][1].buttonIconColor;
					_boardTheme2Background.color = HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][1].buttonBgColor;
					_boardTheme2Titel.text = Strings.Get(HoverboardThemes.hoverboardCustomThemes[payload.board.boardID][1].title);
					_boardTheme1CheckmarkIcon.SetActive(BoardManager.IsThemeOwned(_payload.board.boardID, HoverboardThemes.hoverboardCustomThemes[_payload.board.boardID][0].identifier));
					_boardTheme2CheckmarkIcon.SetActive(BoardManager.IsThemeOwned(_payload.board.boardID, HoverboardThemes.hoverboardCustomThemes[_payload.board.boardID][1].identifier));
					Timer.ExecuteWithDelay(0.1f, () =>
					{
						if (_boardTheme1IconFitter != null)
						{
							_boardTheme1IconFitter.FitNow();
						}
					});
					Timer.ExecuteWithDelay(0.1f, () =>
					{
						if (_boardTheme2IconFitter != null)
						{
							_boardTheme2IconFitter.FitNow();
						}
					});
					_themeLayout.Align(true);
				}
				_themeParent.SetActive(payload.isBoardBundle);
				_rightLayout.Align();
			}
			UpdateButton();
			SetupModel();
		}

		private string GetPriceTypeIconName(PurchaseData.UnlockType type)
		{
			switch (type)
			{
			case PurchaseData.UnlockType.Coins:
				return "icon_coin_large";
			case PurchaseData.UnlockType.Keys:
				return "icon_key";
			default:
				return string.Empty;
			}
		}

		private bool IsHoverboardThemesOwned()
		{
			List<HoverboardThemes.HoverboardTheme> list = HoverboardThemes.hoverboardCustomThemes[_payload.board.boardID];
			foreach (HoverboardThemes.HoverboardTheme item in list)
			{
				if (!BoardManager.IsThemeOwned(_payload.board.boardID, item.identifier))
				{
					return false;
				}
			}
			return true;
		}

		private void UpdateButton()
		{
			_buyButtonPriceIconFitter.gameObject.SetActive(false);
			if (_payload.useCharacter ? (!CharacterManager.IsCharacterOwned(_payload.character.charID)) : ((!_payload.isBoardBundle) ? (!BoardManager.IsBoardOwned(_payload.board.boardID)) : (!IsHoverboardThemesOwned())))
			{
				_buyButtonCheckedSprite.gameObject.SetActive(false);
				_buyButton.GoToNormalState();
				switch (_payload.cellAppearancePayload.buttonAction)
				{
				case EventCellButtonAction.RedirectToScreen:
				case EventCellButtonAction.OpenPopup:
					if (_payload.isBoardBundle && BoardManager.IsBoardOwned(_payload.board.boardID) && !IsHoverboardThemesOwned())
					{
						_buyButtonLabel.text = Strings.Get("STORE_CELL_UPGRADE");
						break;
					}
					if (_payload.isBoardBundle)
					{
						_purchaseID = InAppData.ConvertBoardTypeToDynamicPurchaseId(_payload.board.boardID);
						string[] inAppBoardBundleTiers = InAppData.GetInAppBoardBundleTiers();
						string[] array = inAppBoardBundleTiers;
						foreach (string text in array)
						{
							InAppItemModel.BundleItem bundleItem = InAppData.inAppData[text].bundleContents.Find((InAppItemModel.BundleItem x) => x.boardId == _payload.board.boardID);
							if (bundleItem != null)
							{
								if (_boardBundle1PurchaseID == string.Empty)
								{
									_boardBundle1PurchaseID = text;
								}
								else if (_boardBundle2PurchaseID == string.Empty)
								{
									_boardBundle2PurchaseID = text;
									break;
								}
							}
						}
					}
					_buyButtonLabel.text = Strings.Get("STORE_EVENT_CELL_BUTTON_VIEW");
					break;
				case EventCellButtonAction.Buy:
					if (_payload.useCharacter)
					{
						_buyButtonLabel.text = Characters.characterData[_payload.character.charID].Price + string.Empty;
						_priceType = Characters.characterData[_payload.character.charID].unlockType;
						_purchaseID = InAppData.ConvertCharacterTypeToDynamicPurchaseId(_payload.character.charID);
					}
					else
					{
						_buyButtonLabel.text = Boards.boardData[_payload.board.boardID].price + string.Empty;
						_priceType = Boards.boardData[_payload.board.boardID].unlockType;
						_purchaseID = InAppData.ConvertBoardTypeToDynamicPurchaseId(_payload.board.boardID);
					}
					if (_priceType == PurchaseData.UnlockType.Coins || _priceType == PurchaseData.UnlockType.Keys)
					{
						_buyButtonPriceIcon.spriteName = GetPriceTypeIconName(_priceType);
						_buyButtonPriceIconFitter.gameObject.SetActive(true);
						_buyButtonPriceIconFitter.FitNow();
					}
					else if (_priceType == PurchaseData.UnlockType.InApp)
					{
						_buyButtonLabel.text = Strings.Get("BUY_INAPP_NOPRICE_DEFAULT");
						_buyButtonLabel.text = ((!InAppData.inAppData.ContainsKey(_purchaseID)) ? Strings.Get("BUY_INAPP_NOPRICE_DEFAULT") : InAppData.inAppData[_purchaseID].localizedPrice);
					}
					break;
				}
			}
			else
			{
				_buyButtonCheckedSprite.gameObject.SetActive(true);
				if (_payload.board != null && _payload.board.boardID == Boards.BoardId.birthdayBoard01)
				{
					_buyButtonLabel.text = Strings.Get("STORE_CELL_UNLOCKED");
				}
				else
				{
					_buyButtonLabel.text = Strings.Get("STORE_CELL_PURCHASED");
				}
				_buyButton.GoToNotAvailableState();
			}
			_buyButtonLayout.Align(true);
		}

		private void SetupModel()
		{
			if (_payload.useCharacter)
			{
				CharacterRenderTextureSetup characterRenderTextureSetup = new CharacterRenderTextureSetup();
				characterRenderTextureSetup.Setup(_payload.character.charID, _payload.character.charOutfitIndex, _texture, new List<RenderTextureObject>
				{
					_payload.character.GetRenderTextureObject(_wideCell),
					_starObject
				}, new CameraSettings
				{
					backgroundColor = new Color(1f, 1f, 1f, 0f)
				});
			}
			else
			{
				BoardRenderTextureSetup boardRenderTextureSetup = new BoardRenderTextureSetup();
				RenderTextureObject renderTextureObject = _payload.board.GetRenderTextureObject(_wideCell);
				float animationDelay = UnityEngine.Random.Range(0f, 1f);
				renderTextureObject.animationDelay = animationDelay;
				boardRenderTextureSetup.Setup(_payload.isBoardBundle, _texture, new List<RenderTextureObject> { renderTextureObject, _starObject }, new CameraSettings
				{
					backgroundColor = new Color(1f, 1f, 1f, 0f)
				});
			}
		}

		public void SetTimeRemaining(TimeSpan ts)
		{
			if (ts.TotalSeconds <= 0.0)
			{
				StoreScreen.RefreshOnNextShow();
			}
			_alignTimer.SetTimeRemaining(ts);
		}

		private void Update()
		{
			_payload.onContainerUpdate.SafeInvoke(this);
		}

		[UsedImplicitly]
		public void OnButtonClicked()
		{
			switch (_payload.cellAppearancePayload.buttonAction)
			{
			case EventCellButtonAction.RedirectToScreen:
				if (_payload.useCharacter)
				{
					CharacterScreen.externalSelectedCharacter = _payload.character.charID;
					CharacterScreen.externalForceCenterOnCharacter = true;
					UIScreenController.Instance.PushScreen("CharacterScreen");
				}
				else
				{
					BoardScreenManager.Instance.forceSpecificBoard = _payload.board.boardID;
					BoardScreenManager.Instance.forceCenterOnSpecificBoard = true;
					UIScreenController.Instance.PushScreen("BoardScreen");
				}
				break;
			case EventCellButtonAction.OpenPopup:
				if (_payload.cellAppearancePayload.popupID.IsNullOrEmpty())
				{
					break;
				}
				if (_payload.isBoardBundle && BoardManager.IsBoardOwned(_payload.board.boardID))
				{
					LimitedBoardPopupManager.instance.OverrideNextShowingWithBoard(_payload.board.boardID);
					UIScreenController.Instance.PushPopup("LimitedBoardPopup");
					break;
				}
				if (_payload.cellAppearancePayload.popupID == "LimitedCharacterPopup")
				{
					LimitedCharacterPopupManager.Instance.currentCharacter = _payload.character.charID;
					LimitedCharacterPopupManager.Instance.currentThemeIndex = _payload.character.charOutfitIndex;
				}
				else if (_payload.cellAppearancePayload.popupID == "LimitedBoardPopup")
				{
					LimitedBoardPopupManager.instance.OverrideNextShowingWithBoard(_payload.board.boardID);
				}
				UIScreenController.Instance.PushPopup(_payload.cellAppearancePayload.popupID);
				break;
			case EventCellButtonAction.Buy:
				if (_priceType == PurchaseData.UnlockType.InApp)
				{
					InAppManager.instance.StartPurchase(_purchaseID);
					break;
				}
				if (_payload.useCharacter)
				{
					PurchaseHandler.Instance.PurchaseCharacter(_payload.character.charID, _payload.character.charOutfitIndex);
					break;
				}
				PurchaseHandler.Instance.PurchaseHoverboard(_payload.board.boardID, () =>
				{
					UIScreenController.Instance.ShowUnlockAnimationForHoverboard(_payload.board.boardID);
					Setup(_payload);
				}, () =>
				{
				});
				break;
			}
		}

		private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			if (productId == _purchaseID || productId == _boardBundle1PurchaseID || productId == _boardBundle2PurchaseID)
			{
				Setup(_payload);
			}
		}

		private void OnCharacterUnlocked(Characters.CharacterId characterId, bool b)
		{
			if (_payload.character != null && characterId == _payload.character.charID)
			{
				Setup(_payload);
			}
		}

		private void OnBoardUnlocked(Boards.BoardId boardIdUnlocked)
		{
			if (_payload.board != null && boardIdUnlocked == _payload.board.boardID)
			{
				Setup(_payload);
			}
		}
	}
}
