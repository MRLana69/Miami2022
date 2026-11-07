using JetBrains.Annotations;
using SYBO.Subway.Info;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class CharacterPromotionPopup : UIPopup
	{
		[Header("Anchors")]
		[SerializeField]
		private GameObject _topAnchor;

		[Header("Top")]
		[SerializeField]
		private UILabel _popupCharacterTitle;

		[SerializeField]
		private UILabel _popupCharacterSubTitle;

		[SerializeField]
		private UITexture _popupStickerTexture;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private Transform _timerContainer;

		[SerializeField]
		private Layout _timerLayout;

		private TimerHelper _timer;

		[Header("Bottom")]
		[SerializeField]
		private UILabel _labelGotoShop;

		[Header("Background")]
		[SerializeField]
		private UISprite backgroundSprite;

		[SerializeField]
		private UISprite flashSprite;

		private CharacterPromotion characterPromotion;

		private const int CharacterSelectButtonWidth = 135;

		private const int CharacterSelectButtonHeight = 50;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			base.Init();
			characterPromotion = PromotionManager.Instance.TryGetCurrentPromotion<CharacterPromotion>();
			if (characterPromotion == null)
			{
				Debug.LogError("Current Promotion is not a character promotion.");
				return;
			}
			backgroundSprite.gradientTop = characterPromotion.BackgroundColorTop;
			backgroundSprite.gradientBottom = characterPromotion.BackgroundColorBottom;
			foreach (Transform item in backgroundSprite.transform)
			{
				UISprite component = item.GetComponent<UISprite>();
				if (component == flashSprite)
				{
					component.color = characterPromotion.FlashColor;
				}
				else
				{
					component.color = characterPromotion.BeamColor;
				}
			}
			InitializeTimer();
			SetupTop();
			SetupBottom();
		}

		private void OnCharacterPurchaseSuccess(Characters.CharacterId characterId, int themeId)
		{
			if (characterId == characterPromotion.CharacterId)
			{
				PromotionManager.Instance.OnPromotionPurchaseSuccess();
				CharacterScreenManager.Instance.SelectCharacter(characterPromotion.CharacterId, characterPromotion.ThemeIndex);
			}
		}

		protected override void Show()
		{
			base.Show();
			if (characterPromotion != null)
			{
				CharacterScreenManager.characterUnlocked += OnCharacterPurchaseSuccess;
				CharacterScreenManager.characterUnlockedFailure += PromotionManager.Instance.OnPromotionPurchaseFailed;
				UIModelController.Instance.ShowMenuModel(characterPromotion.CharacterId, 0, UIModelController.ModelScreen.CharacterPromotionPopup);
			}
		}

		public override void GainFocus()
		{
			base.GainFocus();
			if (characterPromotion != null)
			{
				UIModelController.Instance.ShowMenuModel(characterPromotion.CharacterId, 0, UIModelController.ModelScreen.CharacterPromotionPopup);
			}
		}

		public override void LoseFocus()
		{
			base.LoseFocus();
			if (characterPromotion != null)
			{
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.CharacterPromotionPopup);
			}
		}

		protected override void Hide()
		{
			base.Hide();
			if (characterPromotion != null)
			{
				CharacterScreenManager.characterUnlocked -= OnCharacterPurchaseSuccess;
				CharacterScreenManager.characterUnlockedFailure -= PromotionManager.Instance.OnPromotionPurchaseFailed;
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.CharacterPromotionPopup);
			}
		}

		private void InitializeTimer()
		{
			_timer = Object.Instantiate(_timerPrefab, _timerContainer);
			_timer.SetTimerColors(characterPromotion.TimerTextColor, characterPromotion.TimerBgColor);
			_timer.SetAlignment(HorizontalAlignment.Left);
		}

		private void SetupTop()
		{
			_popupCharacterTitle.text = Strings.Get(characterPromotion.TitleId);
			_popupCharacterTitle.color = characterPromotion.TitleTextColor;
			_popupCharacterSubTitle.text = Strings.Get(characterPromotion.SubtitleId);
			_popupCharacterSubTitle.color = characterPromotion.SubtitleTextColor;
			_popupStickerTexture.mainTexture = characterPromotion.StickerTexture;
			_popupStickerTexture.transform.localPosition = new Vector3(characterPromotion.StickerPosition.x, characterPromotion.StickerPosition.y, 0f);
			_popupStickerTexture.GetComponent<UIWidget>().width = (int)characterPromotion.StickerSize.x;
			_popupStickerTexture.GetComponent<UIWidget>().height = (int)characterPromotion.StickerSize.y;
			_timer.StartTimer(characterPromotion.TimeLeft);
			_timerLayout.Align();
		}

		private void SetupBottom()
		{
			_labelGotoShop.text = Strings.Get("POPUP_PROMOTION_GOTO_SHOP");
		}

		[UsedImplicitly]
		public void OnCloseClicked()
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		[UsedImplicitly]
		public void OnGoToShop()
		{
			StoreScreen.ScrollToInterestPointOnNextShow(StoreInterestPoint.LimitedDeals);
			UIScreenController.Instance.PushScreen("StoreScreen");
			ClosePopup();
		}
	}
}
