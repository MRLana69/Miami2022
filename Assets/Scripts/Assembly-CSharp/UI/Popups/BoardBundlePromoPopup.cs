using Data.Managers;
using Info;
using JetBrains.Annotations;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Popups
{
	public class BoardBundlePromoPopup : UIPopup
	{
		private const int SCREEN_REFERENCE_HEIGHT = 480;

		[Header("Anchors")]
		[SerializeField]
		private GameObject _topAnchor;

		[Header("Top")]
		[SerializeField]
		private UILabel _popupBoardTitle;

		[SerializeField]
		private UILabel _popupBoardSubTitle;

		[SerializeField]
		private UITexture _popupStickerTexture;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private Transform _timerContainer;

		[SerializeField]
		private Layout _timerLayout;

		private TimerHelper _timer;

		[Header("Board")]
		[SerializeField]
		private Transform _boardAnchor;

		[Header("Bottom")]
		[SerializeField]
		private UILabel _labelGotoShop;

		[FormerlySerializedAs("backgroundSprite")]
		[Header("Background")]
		[SerializeField]
		private UISprite _backgroundSprite;

		[FormerlySerializedAs("flashSprite")]
		[SerializeField]
		private UISprite _flashSprite;

		private BoardBundlePromotion _bundlePromotion;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_bundlePromotion = PromotionManager.Instance.TryGetCurrentPromotion<BoardBundlePromotion>();
			if (_bundlePromotion == null)
			{
				Debug.LogError("Current Promotion is not a board promotion.");
				return;
			}
			_backgroundSprite.gradientTop = _bundlePromotion.BackgroundColorTop;
			_backgroundSprite.gradientBottom = _bundlePromotion.BackgroundColorBottom;
			foreach (Transform item in _backgroundSprite.transform)
			{
				UISprite component = item.GetComponent<UISprite>();
				if (component == _flashSprite)
				{
					component.color = _bundlePromotion.FlashColor;
				}
				else
				{
					component.color = _bundlePromotion.BeamColor;
				}
			}
			InitBoardAnchor();
			InitializeTimer();
			SetupTop();
			SetupBottom();
			if (UIModelController.Instance.BoardPromotionPopupAnchor == null)
			{
				UIModelController.Instance.BoardPromotionPopupAnchor = _boardAnchor.gameObject;
			}
		}

		protected override void Show()
		{
			if (_bundlePromotion != null)
			{
				ShowHoverboard();
			}
		}

		protected override void Hide()
		{
			if (_bundlePromotion != null)
			{
				HideHoverboard();
			}
		}

		public override void GainFocus()
		{
			if (_bundlePromotion != null)
			{
				ShowHoverboard();
			}
		}

		public override void LoseFocus()
		{
			if (_bundlePromotion != null)
			{
				HideHoverboard();
			}
		}

		private void HideHoverboard()
		{
			_boardAnchor.gameObject.SetActive(false);
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.BoardPromotionPopup);
		}

		private void ShowHoverboard()
		{
			UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.BoardPromotionPopup);
			if (_bundlePromotion.BoardAbility != Hoverboard.BoardAbility.None)
			{
				int themeIndexForPower = HoverboardThemes.GetThemeIndexForPower(_bundlePromotion.BoardId, _bundlePromotion.BoardAbility);
				BoardManager.DisplayCustomSet(themeIndexForPower, true);
			}
			if (_bundlePromotion != null)
			{
				UIModelController.Instance.ShowHoverboardMenuModel(_bundlePromotion.BoardId, UIModelController.ModelScreen.BoardPromotionPopup, true, false, Hoverboard.BoardAbility.None);
				if (_bundlePromotion.BoardAbility != Hoverboard.BoardAbility.None)
				{
					UIModelController.Instance.ShowHoverboardMenuModel(_bundlePromotion.BoardId, UIModelController.ModelScreen.BoardPromotionPopup, true, false, _bundlePromotion.BoardAbility);
				}
			}
		}

		private void InitBoardAnchor()
		{
			_boardAnchor.localPosition = _bundlePromotion.ModelPosition;
			_boardAnchor.localRotation = Quaternion.Euler(_bundlePromotion.ModelRotation);
			if (UIScreenController.Instance.root != null)
			{
				float num = 480f / (float)UIScreenController.Instance.root.manualHeight;
				_boardAnchor.localScale *= num;
				float num2 = _boardAnchor.localPosition.y * (1f - 1f / num);
				_boardAnchor.localPosition = new Vector3(_boardAnchor.localPosition.x, _boardAnchor.localPosition.y + num2, _boardAnchor.localPosition.z);
			}
		}

		private void InitializeTimer()
		{
			_timer = Object.Instantiate(_timerPrefab, _timerContainer);
			_timer.SetTimerColors(GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT, GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG);
			_timer.SetAlignment(HorizontalAlignment.Left);
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

		private void SetupTop()
		{
			Boards.Board board = Boards.boardData[_bundlePromotion.BoardId];
			_popupBoardTitle.text = Strings.Get(board.name);
			_popupBoardTitle.color = _bundlePromotion.TitleTextColor;
			_popupBoardSubTitle.text = Strings.Get(board.limitedDescription);
			_popupBoardSubTitle.color = _bundlePromotion.SubtitleTextColor;
			_popupStickerTexture.mainTexture = _bundlePromotion.StickerTexture;
			_popupStickerTexture.transform.localPosition = _bundlePromotion.StickerPosition;
			_popupStickerTexture.GetComponent<UIWidget>().width = (int)_bundlePromotion.StickerSize.x;
			_popupStickerTexture.GetComponent<UIWidget>().height = (int)_bundlePromotion.StickerSize.x;
			_timer.StartTimer(_bundlePromotion.TimeLeft);
			_timerLayout.Align();
		}

		private void SetupBottom()
		{
			_labelGotoShop.text = Strings.Get("POPUP_PROMOTION_GOTO_SHOP");
		}

		private void OnDestroy()
		{
			if (_bundlePromotion != null)
			{
				UIModelController.Instance.ClearScreenAnchor(UIModelController.ModelScreen.BoardPromotionPopup);
			}
		}
	}
}
