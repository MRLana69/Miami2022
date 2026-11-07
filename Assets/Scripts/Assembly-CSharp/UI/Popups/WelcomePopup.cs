using Data.Save;
using Info;
using JetBrains.Annotations;
using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Popups
{
	public class WelcomePopup : CharacterPopup
	{
		[SerializeField]
		private UILabel _calloutLabel;

		[SerializeField]
		private UILabel _buttonText;

		[SerializeField]
		private UISprite _calloutBackgroundSprite;

		[SerializeField]
		private UISprite _calloutBackgroundTriangleSprite;

		[SerializeField]
		private GameObject _centerAnchor;

		[SerializeField]
		private GameObject _tutorialMultiLinesPrefab;

		private TutorialMultilineLabelsHelper _tutorialLinesHelper;

		protected override float screenOffset
		{
			get
			{
				return 0f;
			}
		}

		protected override void Init()
		{
			_tutorialLinesHelper = TutorialMultilineLabelsHelper.AddToGameObject(_centerAnchor, _tutorialMultiLinesPrefab);
		}

		protected override void Show()
		{
			SetCharacter(Characters.CharacterId.nick, new Vector2(43f, 6.43f), null, 19f, false, null, false);
			_calloutLabel.text = Strings.Get("POPUP_WELCOME_TITLE_LINE_1");
			_buttonText.text = Strings.Get("OK");
			_tutorialLinesHelper.SetText("POPUP_WELCOME_LINE1BIG", "POPUP_WELCOME_LINE1SMALL", "POPUP_WELCOME_LINE2BIG", "POPUP_WELCOME_LINE2SMALL", "POPUP_WELCOME_LINE3BIG", "POPUP_WELCOME_LINE3SMALL");
			UpdateColors();
		}

		protected override void Hide()
		{
			if (!UserSave.hasUserRecievedFreeTokenMysteryBox && MysteryBox.AreTokensAvailable())
			{
				UIScreenController.Instance.ignoreLastPopupClosed = true;
				UIScreenController.Instance.QueuePopup("TokenMysteryBoxPopup");
			}
			HideCharacter();
		}

		public override void GainFocus()
		{
			ShowCharacter();
		}

		public override void LoseFocus()
		{
			HideCharacter();
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		[UsedImplicitly]
		public void OkClicked()
		{
			ClosePopup();
		}

		public void UpdateColors()
		{
			_calloutBackgroundSprite.color = WelcomePopupTheme.instance.colors[0];
			_calloutBackgroundTriangleSprite.color = WelcomePopupTheme.instance.colors[0];
			_calloutLabel.color = WelcomePopupTheme.instance.colors[1];
		}
	}
}
