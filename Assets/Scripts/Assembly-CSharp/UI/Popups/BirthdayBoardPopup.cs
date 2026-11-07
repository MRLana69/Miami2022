using System;
using Data.Managers;
using Info;
using Kiloo;
using UI.Generic;
using UI.Generic.Layout;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.Popups
{
	public class BirthdayBoardPopup : UIPopup
	{
		[SerializeField]
		private GameObject _boardAnchor;

		[Header("Top")]
		[SerializeField]
		private UILabel _popupTitle;

		[SerializeField]
		private UILabel _popupSubTitle;

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
		private UISprite _backgroundSprite;

		[SerializeField]
		private UISprite _flashSprite;

		[Header("Button")]
		[SerializeField]
		private HorizontalLayout _horizontalLayout;

		[SerializeField]
		private UIButtonOverlayOff _buttonOverlay;

		[SerializeField]
		private GameObject _tvIcon;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private GameObject _noConnectionIcon;

		private bool _buttonActive;

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
			_buttonActive = false;
			SetupColors();
			SetupText();
			InitializeTimer();
			if (UIModelController.Instance.birthdayBoardPopupAnchor == null)
			{
				UIModelController.Instance.birthdayBoardPopupAnchor = _boardAnchor;
			}
		}

		protected override void Show()
		{
			UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.BirthdayBoardPopup);
			UIModelController.Instance.ShowHoverboardMenuModel(Boards.BoardId.birthdayBoard01, UIModelController.ModelScreen.BirthdayBoardPopup, true, false, Hoverboard.BoardAbility.Vanity);
			UpdateButton();
		}

		protected override void Hide()
		{
			UIModelController.Instance.ClearScreenAnchor(UIModelController.ModelScreen.BirthdayBoardPopup);
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.BirthdayBoardPopup);
		}

		private void SetupColors()
		{
			_backgroundSprite.gradientTop = NGUIMath.HexToColor(1774985471u);
			_backgroundSprite.gradientBottom = NGUIMath.HexToColor(2950974975u);
			_popupTitle.color = NGUIMath.HexToColor(54227455u);
			_popupSubTitle.color = NGUIMath.HexToColor(460826879u);
		}

		private void SetupText()
		{
			_popupTitle.text = Strings.Get("HOVERBOARDS_BIRTHDAY_BOARD01");
			_popupSubTitle.text = Strings.Get("UPDATE_SPECIAL_TAG_ANNIVERSARY");
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerContainer);
			_timer.SetTimerColors(NGUIMath.HexToColor(4210855679u), NGUIMath.HexToColor(4047581951u));
			_timer.SetAlignment(HorizontalAlignment.Left);
			TimeSpan timeSpan = Boards.boardData[Boards.BoardId.birthdayBoard01].defaultExpirationTime - CDateTime.Now;
			_timer.StartTimer(timeSpan);
			_timerLayout.Align();
		}

		private void UpdateButton()
		{
			if (!_buttonActive)
			{
				if (AdSystemManager.IsCached(AdLocations.BIRTHDAY_BOARD_VIDEO))
				{
					_buttonActive = true;
					_buttonOverlay.GoToNormalState();
					_tvIcon.SetActive(true);
					_noConnectionIcon.SetActive(false);
					_buttonLabel.text = Strings.Get("GET_NOW");
					_horizontalLayout.Align();
				}
				else
				{
					_buttonActive = false;
					_buttonOverlay.GoToNotAvailableState();
					_tvIcon.SetActive(false);
					_noConnectionIcon.SetActive(true);
					_buttonLabel.text = Strings.Get("FREE_STUFF_NO_CONNECTION");
					_horizontalLayout.Align();
				}
			}
		}

		public void OnClick()
		{
			if (_buttonActive)
			{
				AdSystemManager.Show(AdLocations.BIRTHDAY_BOARD_VIDEO, () =>
				{
					UIScreenController.Instance.ClosePopup("BirthdayBoardPopup");
					BoardManager.UnlockBoard(Boards.BoardId.birthdayBoard01);
					CelebrationReward reward = new CelebrationReward
					{
						CelebrationRewardOrigin = CelebrationRewardOrigin.BoardUnlock,
						rewardType = CelebrationRewardType.specialBoard,
						boardId = Boards.BoardId.birthdayBoard01
					};
					UIModelController.Instance.SelectCurrentlyShownBoard(Boards.BoardId.birthdayBoard01);
					RewardManager.AddRewardToUnlock(reward);
					UIScreenController.Instance.PushCelebrationPopup();
				});
			}
		}
	}
}
