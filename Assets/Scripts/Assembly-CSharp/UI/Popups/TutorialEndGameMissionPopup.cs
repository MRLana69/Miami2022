using JetBrains.Annotations;
using SYBO.Subway.Info;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class TutorialEndGameMissionPopup : CharacterPopup
	{
		[SerializeField]
		private UILabel _calloutLabel;

		[SerializeField]
		private UILabel _calloutSubtitle;

		[SerializeField]
		private Layout _labelLayout;

		[SerializeField]
		private GameObject _centerAnchor;

		[SerializeField]
		private GameObject _tutorialMultiLinesPrefab;

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
			SetCharacter(Characters.CharacterId.lucy, new Vector2(38f, 7f), null, 19f, false, null, false);
			_calloutLabel.text = Strings.Get("POPUP_TUTORIAL_END_GAME_MISSION_CALLOUT");
			_calloutSubtitle.text = Strings.Get("POPUP_TUTORIAL_END_GAME_MISSION_SUBTITLE");
			_labelLayout.Align();
			_tutorialLinesHelper.SetText("POPUP_TUTORIAL_END_GAME_MISSION_LINE1_BIG", "POPUP_TUTORIAL_END_GAME_MISSION_LINE1_SMALL", "POPUP_TUTORIAL_END_GAME_MISSION_LINE2_BIG", "POPUP_TUTORIAL_END_GAME_MISSION_LINE2_SMALL", "POPUP_TUTORIAL_END_GAME_MISSION_LINE3_BIG", "POPUP_TUTORIAL_END_GAME_MISSION_LINE3_SMALL");
			_buttonText.text = Strings.Get("POPUP_TUTORIAL_END_GAME_MISSION_BUTTON");
		}

		public override void GainFocus()
		{
			ShowCharacter();
		}

		public override void LoseFocus()
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
