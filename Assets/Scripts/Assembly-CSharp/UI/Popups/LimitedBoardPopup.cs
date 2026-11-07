using System;
using System.Collections.Generic;
using System.Text;
using Data.Managers;
using Info;
using UI.Generic;
using UI.Popups.LimitedBoardPopupInternal;
using UnityEngine;

namespace UI.Popups
{
	public class LimitedBoardPopup : UIPopup
	{
		[SerializeField]
		private GameObject _modelAnchor;

		[SerializeField]
		private UILabel _nameLabel;

		[SerializeField]
		private UILabel _staticPowerupLabel;

		[SerializeField]
		private UILabel _descriptionLabel;

		[SerializeField]
		private UISprite _powerIcon;

		[SerializeField]
		private UILabel _themeLimitedLabel;

		[SerializeField]
		private GameObject _themePower;

		[SerializeField]
		private UILabel _themePowerLabel;

		[SerializeField]
		private UIHoverboardThemeShifter _hoverboardThemeShifter;

		[SerializeField]
		private BoardSelectButton _boardSelectButtonPrefab;

		[SerializeField]
		private UIWidget _boardSelectButtonParent;

		protected BoardSelectButton _boardSelectButton;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private UIWidget _timerParent;

		private TimerHelper _timer;

		private const bool ALLOW_SIGNATURE_ANIMATIONS = false;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			if (UIModelController.Instance.limitedBoardPopupAnchor == null)
			{
				UIModelController.Instance.limitedBoardPopupAnchor = _modelAnchor;
			}
			InitializeSelectButton();
			InitializeTimer();
			_hoverboardThemeShifter.Init(BoardManager.ScreenContext.LimitedBoardPopup);
		}

		protected override void Show()
		{
			LimitedBoardPopupManager.instance.currentlyShownThemeIndex = 0;
			_boardSelectButton.UpdateCurrentThemeIndex();
			_hoverboardThemeShifter.OnClickedTheme += ThemeButtonClicked;
			SetupThemeShifter();
			BoardManager.themeActiveStateChanged += OnThemeActiveStateChanged;
			UIScreenController.OnApplicationResumed += HidePopupIfNeeded;
			BoardManager.boardUnlocked += BoardUnlocked;
			if (!BoardManager.HasBoardBeenSeen(LimitedBoardPopupManager.instance.currentBoardId))
			{
				BoardManager.MarkHoverboardAsSeen(LimitedBoardPopupManager.instance.currentBoardId);
			}
			UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.LimitedBoardPopup);
			ShowHoverboardInMenu(true, false);
			UpdateUIElements();
		}

		protected override void Hide()
		{
			_hoverboardThemeShifter.OnClickedTheme -= ThemeButtonClicked;
			BoardManager.themeActiveStateChanged -= OnThemeActiveStateChanged;
			UIScreenController.OnApplicationResumed -= HidePopupIfNeeded;
			BoardManager.boardUnlocked -= BoardUnlocked;
			UIModelController.Instance.ClearScreenAnchor(UIModelController.ModelScreen.LimitedBoardPopup);
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.LimitedBoardPopup);
			LogFlurryEvent();
		}

		public override void GainFocus()
		{
			UIModelController.Instance.ActivateHoverboardModel(UIModelController.ModelScreen.LimitedBoardPopup);
			ShowHoverboardInMenu(true, false);
		}

		public override void LoseFocus()
		{
			UIModelController.Instance.DeactivateModelScreen(UIModelController.ModelScreen.LimitedBoardPopup);
		}

		private void InitializeSelectButton()
		{
			_boardSelectButton = UnityEngine.Object.Instantiate(_boardSelectButtonPrefab, _boardSelectButtonParent.transform);
			_boardSelectButton.transform.localPosition = Vector3.zero;
			_boardSelectButton.GetComponent<UIWidget>().width = _boardSelectButtonParent.width;
			_boardSelectButton.InitButton(BoardManager.ScreenContext.LimitedBoardPopup);
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerParent.transform);
			_timer.SetTimerColors(GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR, GlobalColorsGenerated.DISCOUNT_STICKER_COLOR);
		}

		private void UpdateUIElements()
		{
			Boards.BoardId currentBoardId = LimitedBoardPopupManager.instance.currentBoardId;
			int currentlyShownThemeIndex = LimitedBoardPopupManager.instance.currentlyShownThemeIndex;
			_nameLabel.text = Strings.Get(Boards.boardData[currentBoardId].name);
			_nameLabel.color = ColorData.instance.midnight;
			_descriptionLabel.text = Strings.Get(Boards.boardData[currentBoardId].description);
			_descriptionLabel.color = ColorData.instance.peacock;
			_themeLimitedLabel.text = Strings.Get(Boards.boardData[currentBoardId].limitedDescription);
			_staticPowerupLabel.text = Strings.Get("BOARD_SCREEN_SPECIAL_POWER");
			bool flag = !string.IsNullOrEmpty(Strings.Get(Boards.boardData[currentBoardId].limitedDescription));
			bool flag2 = !string.IsNullOrEmpty(Strings.Get(Boards.boardData[currentBoardId].description));
			_powerIcon.enabled = flag2;
			_staticPowerupLabel.enabled = flag2;
			_themeLimitedLabel.enabled = flag;
			if (!flag || !flag2 || Application.isEditor)
			{
			}
			if (currentlyShownThemeIndex == 0)
			{
				_themePower.SetActive(false);
				_nameLabel.gameObject.SetActive(true);
			}
			else
			{
				_themePower.SetActive(true);
				_nameLabel.gameObject.SetActive(false);
				_themePowerLabel.text = Strings.Get(HoverboardThemes.hoverboardCustomThemes[currentBoardId][currentlyShownThemeIndex - 1].title);
				_themeLimitedLabel.text = string.Empty;
			}
			bool flag3 = BoardManager.IsBoardOwned(currentBoardId);
			if (flag && !flag3)
			{
				DateTime endTime = BoardManager.BoardExpirationDate(currentBoardId);
				_timer.gameObject.SetActive(true);
				_timer.StartTimer(endTime);
			}
			else
			{
				_timer.StopTimer();
				_timer.gameObject.SetActive(false);
			}
			UpdateThemeShifter();
			_boardSelectButton.UpdateSticker(currentBoardId, currentlyShownThemeIndex);
		}

		private void SetupThemeShifter()
		{
			_hoverboardThemeShifter.ResetThemeSelection();
			UpdateThemeShifter();
		}

		private void UpdateThemeShifter()
		{
			if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(LimitedBoardPopupManager.instance.currentBoardId))
			{
				_hoverboardThemeShifter.MakeCustomThemesAvailable(HoverboardThemes.hoverboardCustomThemes[LimitedBoardPopupManager.instance.currentBoardId]);
			}
			else
			{
				_hoverboardThemeShifter.DisableThemeShifter();
			}
		}

		private void BoardUnlocked(Boards.BoardId boardId)
		{
			BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
		}

		private void OnThemeActiveStateChanged(Hoverboard.BoardAbility boardAbility)
		{
			ShowHoverboardInMenu(true, true, boardAbility);
		}

		private void ThemeButtonClicked(int index)
		{
			Boards.BoardId currentBoardId = LimitedBoardPopupManager.instance.currentBoardId;
			bool flag = BoardManager.IsThemeOwned(currentBoardId, HoverboardThemes.GetThemeIdentifier(currentBoardId, index - 1));
			if (LimitedBoardPopupManager.instance.currentlyShownThemeIndex == index)
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
			LimitedBoardPopupManager.instance.currentlyShownThemeIndex = index;
			_boardSelectButton.UpdateCurrentThemeIndex();
			UpdateDisplayedModelWithTheme(currentBoardId, index);
			if (flag)
			{
				return;
			}
			CharacterModel activeCharacterModel = UIModelController.Instance.GetActiveCharacterModel(UIModelController.ModelScreen.LimitedBoardPopup);
			if (activeCharacterModel != null)
			{
				GameObject hoverboardGameObject = activeCharacterModel.GetHoverboardGameObject();
				Animation characterAnimation = activeCharacterModel.CharacterAnimation;
				if (hoverboardGameObject != null && characterAnimation != null)
				{
					Hoverboard.BoardAbility boardAbility = ((index > 0) ? HoverboardThemes.hoverboardCustomThemes[currentBoardId][index - 1].power : Hoverboard.BoardAbility.None);
					HoverboardModelPreviewFactory.Instance.ChangeHoverboard(currentBoardId, hoverboardGameObject, characterAnimation, true, true, boardAbility);
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
						BoardManager.DisplayCustomSet(i, BoardManager.IsThemeActive(boardId, HoverboardThemes.GetThemeIdentifier(boardId, i)));
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

		private void ShowHoverboardInMenu(bool willUpdateAnimation, bool willAllowSignatureAnimation, Hoverboard.BoardAbility ability = Hoverboard.BoardAbility.None)
		{
			UIModelController.Instance.ShowHoverboardMenuModel(LimitedBoardPopupManager.instance.currentBoardId, UIModelController.ModelScreen.LimitedBoardPopup, willUpdateAnimation, willAllowSignatureAnimation, ability);
			UpdateUIElements();
		}

		private void LogFlurryEvent()
		{
			Boards.BoardId currentBoardId = LimitedBoardPopupManager.instance.currentBoardId;
			string arg = Strings.Get(Boards.boardData[currentBoardId].name).ToLower();
			bool state = BoardManager.IsBoardOwned(currentBoardId);
			StringBuilder stringBuilder = new StringBuilder(string.Format("Board {0} bought", arg));
			stringBuilder.Append(";");
			StringBuilder stringBuilder2 = new StringBuilder(GetStateString(state));
			stringBuilder2.Append(";");
			List<HoverboardThemes.HoverboardTheme> list = HoverboardThemes.TryGetCustomThemesForBoard(currentBoardId);
			if (list != null)
			{
				foreach (HoverboardThemes.HoverboardTheme item in list)
				{
					HoverboardThemes.ThemeIdentifier identifier = item.identifier;
					bool state2 = BoardManager.IsThemeOwned(currentBoardId, identifier);
					stringBuilder.AppendFormat("{0}_{1} bought", arg, identifier.ToString());
					stringBuilder.Append(";");
					stringBuilder2.AppendFormat("{0};", GetStateString(state2));
				}
			}
			Flurry.LogEventWithSeveralParameters("Limited board popup closed", stringBuilder.ToString(), stringBuilder2.ToString());
		}

		private string GetStateString(bool state)
		{
			return (!state) ? "no" : "yes";
		}

		private void HidePopupIfNeeded()
		{
			if (!LimitedBoardPopupManager.instance.IsBoardAvailable())
			{
				UIScreenController.Instance.CloseActivePopups("LimitedBoardPopup");
			}
		}
	}
}
