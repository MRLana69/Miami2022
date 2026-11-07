using System;
using System.Collections.Generic;
using Data.Managers;
using InApp;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.Popups.LimitedBoardPopupInternal;
using UI.Popups.LimitedCharacterPopupInternal;
using UI.RenderTexture;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	public class StoreBundleContainer : MonoBehaviour
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

		[Header("Banner")]
		[SerializeField]
		private UISprite _bannerSprite;

		[SerializeField]
		private UILabel _bannerLabel;

		[SerializeField]
		private HorizontalLayout _bannerLayout;

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

		private bool _isBundleOwned;

		private bool _subscribed;

		private DuoPromotion _duoPromotion;

		public Action<StoreBundleContainer> onContainerUpdate;

		private void OnEnable()
		{
			if (_payload != null)
			{
				UpdateButton();
			}
			if (!_subscribed && InAppManager.instance != null)
			{
				InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
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
			if (_timerTextLabel != null)
			{
				_timerTextLabel.text = Strings.Get("TIMER_ENDS_IN");
			}
			_alignTimer.SetTimerColor(ColorData.instance.mahogany);
			_duoPromotion = PromotionManager.Instance.TryGetCurrentPromotion<DuoPromotion>();
			_titleText.text = Strings.Get(_duoPromotion.TitleId);
			_descriptionText.text = Strings.Get(_duoPromotion.SubtitleId);
			if (!string.IsNullOrEmpty(Strings.Get("STORE_BUNDLE_BANNER")))
			{
				_bannerSprite.gameObject.SetActive(true);
				_bannerSprite.gradientTop = payload.cellAppearancePayload.bannerColorTop;
				_bannerSprite.gradientBottom = payload.cellAppearancePayload.bannerColorBottom;
				_bannerLabel.color = payload.cellAppearancePayload.bannerTextColor;
				_bannerLabel.effectColor = ColorData.instance.shadow;
				_bannerLabel.text = Strings.Get("STORE_BUNDLE_BANNER");
				_bannerLayout.Align();
			}
			else
			{
				_bannerSprite.gameObject.SetActive(false);
			}
			_rightLayout.Align();
			_isBundleOwned = IsBundleContentOwned();
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

		private bool IsBundleContentOwned()
		{
			return CharacterManager.IsCharacterOwned(_duoPromotion.Entry1.CharacterId) && CharacterManager.IsCharacterOwned(_duoPromotion.Entry2.CharacterId) && BoardManager.IsBoardOwned(_duoPromotion.Entry1.BoardId) && BoardManager.IsBoardOwned(_duoPromotion.Entry2.BoardId);
		}

		private void UpdateButton()
		{
			_buyButtonPriceIconFitter.gameObject.SetActive(false);
			if (!IsBundleContentOwned())
			{
				_buyButtonCheckedSprite.gameObject.SetActive(false);
				_buyButton.GoToNormalState();
				switch (_payload.cellAppearancePayload.buttonAction)
				{
				case StoreEventContainerSmall.EventCellButtonAction.RedirectToScreen:
				case StoreEventContainerSmall.EventCellButtonAction.OpenPopup:
					_buyButtonLabel.text = Strings.Get("STORE_EVENT_CELL_BUTTON_VIEW");
					break;
				case StoreEventContainerSmall.EventCellButtonAction.Buy:
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
				_buyButtonLabel.text = Strings.Get("STORE_CELL_PURCHASED");
				_buyButton.GoToNotAvailableState();
			}
			_buyButtonLayout.Align(true);
		}

		private void SetupModel()
		{
			CharacterBoardBundleRenderTextureSetup characterBoardBundleRenderTextureSetup = new CharacterBoardBundleRenderTextureSetup();
			List<RenderTextureObject> list = new List<RenderTextureObject>();
			List<Characters.CharacterId> list2 = new List<Characters.CharacterId>();
			List<DuoPromotionEntry> list3 = new List<DuoPromotionEntry>();
			list3.Add(_duoPromotion.Entry1);
			list3.Add(_duoPromotion.Entry2);
			List<DuoPromotionEntry> list4 = list3;
			foreach (DuoPromotionEntry item in list4)
			{
				list2.Add(item.CharacterId);
				list.Add(new RenderTextureObject
				{
					animationClip = item.CharacterAnimation,
					position = item.StoreCharacterPosition,
					rotation = item.StoreCharacterRotation,
					scale = item.StoreCharacterScale,
					prefab = UIModelController.Instance.ModelPrefab
				});
				list.Add(new RenderTextureObject
				{
					position = item.StoreBoardPosition,
					rotation = item.StoreBoardRotation,
					scale = item.StoreBoardScale,
					prefab = (Resources.Load("Prefabs/HoverboardAssets/" + Boards.boardData[item.BoardId].boardModelName) as GameObject),
					animationDelay = UnityEngine.Random.Range(0f, 1f),
					animationClip = item.BoardAnimation
				});
			}
			list.Add(_starObject);
			characterBoardBundleRenderTextureSetup.Setup(list2, _texture, list, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
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
			if (!_isBundleOwned && IsBundleContentOwned())
			{
				UpdateButton();
				_isBundleOwned = true;
			}
			onContainerUpdate.SafeInvoke(this);
		}

		[UsedImplicitly]
		public void OnButtonClicked()
		{
			switch (_payload.cellAppearancePayload.buttonAction)
			{
			case StoreEventContainerSmall.EventCellButtonAction.RedirectToScreen:
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
			case StoreEventContainerSmall.EventCellButtonAction.OpenPopup:
				if (!_payload.cellAppearancePayload.popupID.IsNullOrEmpty())
				{
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
				}
				break;
			case StoreEventContainerSmall.EventCellButtonAction.Buy:
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
			if (productId == _purchaseID)
			{
				Setup(_payload);
			}
		}
	}
}
