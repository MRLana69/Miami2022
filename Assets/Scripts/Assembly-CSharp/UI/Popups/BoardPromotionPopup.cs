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
	public class BoardPromotionPopup : UIPopup
	{
		private readonly Vector3 MODEL_POSITION = new Vector3(-2f, -65f, 920f);

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

		[SerializeField]
		private BoardSelectButton _boardSelectButtonPrefab;

		[SerializeField]
		private Transform _boardSelectButtonParent;

		private BoardSelectButton _boardSelectButton;

		private BoardPromotion _boardPromotion;

		private const int BOARD_SELECT_BUTTON_WIDTH = 135;

		private const int BOARD_SELECT_BUTTON_HEIGHT = 50;

		[FormerlySerializedAs("backgroundSprite")]
		[Header("Background")]
		[SerializeField]
		private UISprite _backgroundSprite;

		[FormerlySerializedAs("flashSprite")]
		[SerializeField]
		private UISprite _flashSprite;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_boardPromotion = PromotionManager.Instance.TryGetCurrentPromotion<BoardPromotion>();
			if (_boardPromotion == null)
			{
				Debug.LogError("Current Promotion is not a board promotion.");
				return;
			}
			_backgroundSprite.color = _boardPromotion.BackgroundColor;
			foreach (Transform item in _backgroundSprite.transform)
			{
				UISprite component = item.GetComponent<UISprite>();
				if (component == _flashSprite)
				{
					component.color = _boardPromotion.FlashColor;
				}
				else
				{
					component.color = _boardPromotion.BeamColor;
				}
			}
			Boards.BoardId boardId = _boardPromotion.BoardId;
			InitializeSelectButton();
			InitBoardAnchor();
			InitializeTimer();
			SetupTop(boardId);
			_boardSelectButton.onPurchaseFailed = PromotionManager.Instance.OnPromotionPurchaseFailed;
			_boardSelectButton.onPurchaseSuccessful = PromotionManager.Instance.OnPromotionPurchaseSuccess;
			if (UIModelController.Instance.BoardPromotionPopupAnchor == null)
			{
				UIModelController.Instance.BoardPromotionPopupAnchor = _boardAnchor.gameObject;
			}
		}

		protected override void Show()
		{
			if (_boardPromotion != null)
			{
				UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.BoardPromotionPopup);
				UIModelController.Instance.ShowHoverboardMenuModel(_boardPromotion.BoardId, UIModelController.ModelScreen.BoardPromotionPopup, true, false, Hoverboard.BoardAbility.None);
			}
		}

		protected override void Hide()
		{
			if (_boardPromotion != null)
			{
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.BoardPromotionPopup);
			}
		}

		public override void GainFocus()
		{
			if (_boardPromotion != null)
			{
				_boardAnchor.gameObject.SetActive(true);
				UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.BoardPromotionPopup);
				UIModelController.Instance.ShowHoverboardMenuModel(_boardPromotion.BoardId, UIModelController.ModelScreen.BoardPromotionPopup, true, false, Hoverboard.BoardAbility.None);
			}
		}

		public override void LoseFocus()
		{
			if (_boardPromotion != null)
			{
				_boardAnchor.gameObject.SetActive(false);
				UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.BoardPromotionPopup);
			}
		}

		private void InitBoardAnchor()
		{
			_boardAnchor.localPosition = MODEL_POSITION;
			if (UIScreenController.Instance.root != null)
			{
				float num = 480f / (float)UIScreenController.Instance.root.manualHeight;
				_boardAnchor.localScale *= num;
				float num2 = _boardAnchor.localPosition.y * (1f - 1f / num);
				_boardAnchor.localPosition = new Vector3(_boardAnchor.localPosition.x, _boardAnchor.localPosition.y + num2, _boardAnchor.localPosition.z);
			}
		}

		private void InitializeSelectButton()
		{
			_boardSelectButton = Object.Instantiate(_boardSelectButtonPrefab, _boardSelectButtonParent);
			_boardSelectButton.transform.localPosition = Vector3.zero;
			_boardSelectButton.InitButton(BoardManager.ScreenContext.BoardPromotionPopup);
			_boardSelectButton.GetComponent<UIWidget>().width = 135;
			_boardSelectButton.GetComponent<UIWidget>().height = 50;
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

		private void SetupTop(Boards.BoardId boardId)
		{
			Boards.Board board = Boards.boardData[boardId];
			BoardPromotion boardPromotion = PromotionManager.Instance.TryGetCurrentPromotion<BoardPromotion>();
			if (boardPromotion == null)
			{
				Debug.LogError("Current promotion is of the wrong type.");
				return;
			}
			_popupBoardTitle.text = Strings.Get(board.name);
			_popupBoardTitle.color = ColorData.instance.midnight;
			_popupBoardSubTitle.text = Strings.Get(board.limitedDescription);
			_popupBoardSubTitle.color = ColorData.instance.peacock;
			_popupStickerTexture.mainTexture = boardPromotion.StickerTexture;
			_timer.StartTimer(boardPromotion.TimeLeft);
			_timerLayout.Align();
		}

		private void OnDestroy()
		{
			if (_boardPromotion != null)
			{
				UIModelController.Instance.ClearScreenAnchor(UIModelController.ModelScreen.BoardPromotionPopup);
			}
		}
	}
}
