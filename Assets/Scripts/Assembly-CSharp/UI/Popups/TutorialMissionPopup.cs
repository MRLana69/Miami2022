using JetBrains.Annotations;
using SYBO.Subway.Info;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class TutorialMissionPopup : CharacterPopup
	{
		[SerializeField]
		private GameObject _centerAnchor;

		[SerializeField]
		private GameObject _tutorialMultiLinesPrefab;

		[SerializeField]
		private UILabel _calloutLabel;

		[SerializeField]
		private UILabel _buttonText;

		private TutorialMultilineLabelsHelper _tutorialLinesHelper;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_tutorialLinesHelper = TutorialMultilineLabelsHelper.AddToGameObject(_centerAnchor, _tutorialMultiLinesPrefab);
		}

		protected override void Show()
		{
			SetCharacter(Characters.CharacterId.king, new Vector2(42f, -3f), null, 19f, false, null, false);
			_calloutLabel.text = Strings.Get("POPUP_TUTORIAL_MISSIONS_TITLE_1");
			_tutorialLinesHelper.SetText("POPUP_TUTORIAL_MISSIONS_LINE1BIG", "POPUP_TUTORIAL_MISSIONS_LINE1SMALL", "POPUP_TUTORIAL_MISSIONS_LINE2BIG", "POPUP_TUTORIAL_MISSIONS_LINE2SMALL", "POPUP_TUTORIAL_MISSIONS_LINE3BIG", "POPUP_TUTORIAL_MISSIONS_LINE3SMALL");
			_buttonText.text = Strings.Get("POPUP_TUTORIAL_MISSIONS_BUTTON");
		}

		protected override void Hide()
		{
			HideCharacter();
		}

		[UsedImplicitly]
		public void OkClicked()
		{
			ChallengesScreen.SetPageIndex(1);
			UIScreenController.Instance.PushScreen("ChallengesScreen");
			Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		[UsedImplicitly]
		public void CloseClicked()
		{
			Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}
}
