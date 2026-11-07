using JetBrains.Annotations;
using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Popups
{
	public class TutorialFacebookPopup : CharacterPopup
	{
		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private UILabel _disclaimerLabel;

		[SerializeField]
		private UILabel _calloutLabel;

		[SerializeField]
		private GameObject _centerAnchor;

		[SerializeField]
		private GameObject _tutorialMultiLinesPrefab;

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
			Vector3? position = new Vector3(0f, -54f, 0f);
			_tutorialLinesHelper = TutorialMultilineLabelsHelper.AddToGameObject(_centerAnchor, _tutorialMultiLinesPrefab, position);
		}

		protected override void Show()
		{
			SetCharacter(Characters.CharacterId.tricky, new Vector2(38f, 3f), null, 19f, false, null, false);
			_calloutLabel.text = Strings.Get("POPUP_TUTORIAL_FACEBOOK_CALLOUT");
			_tutorialLinesHelper.SetText("POPUP_TUTORIAL_FACEBOOK_LINE1_BIG", "POPUP_TUTORIAL_FACEBOOK_LINE1_SMALL", "POPUP_TUTORIAL_FACEBOOK_LINE2_BIG", "POPUP_TUTORIAL_FACEBOOK_LINE2_SMALL", "POPUP_TUTORIAL_FACEBOOK_LINE3_BIG", "POPUP_TUTORIAL_FACEBOOK_LINE3_SMALL");
			_buttonLabel.text = Strings.Get("SOCIAL_BUTTON_JOIN_FRIENDS");
			_disclaimerLabel.text = Strings.Get("POPUP_TUTORIAL_FACEBOOK_DISCLAIMER");
		}

		protected override void Hide()
		{
			HideCharacter();
		}

		[UsedImplicitly]
		public void OkClicked()
		{
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.PushScreen("LeaderboardUI");
			SocialManager.instance.FacebookLogin(UIScreenController.Instance.FacebookLogIn);
			Flurry.LogOkPressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		[UsedImplicitly]
		public void CloseClicked()
		{
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.PushScreen("LeaderboardUI");
			Flurry.LogClosePressedOnPopup(UIScreens.GetFlurryPopUpWithOkAndCloseName(base.gameObject.name));
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}
}
