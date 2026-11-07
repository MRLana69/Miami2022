using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups
{
	public class RatingPopup : CharacterPopup
	{
		[SerializeField]
		private UILabel bigLabel;

		[SerializeField]
		private UILabel pleaseLabel;

		[SerializeField]
		private UILabel rateButtonLabel;

		[SerializeField]
		private UILabel cancelButtonLabel;

		[SerializeField]
		private UILabel notAgainLabel;

		[SerializeField]
		private UIToggle notAgainCheckBox;

		[SerializeField]
		private Layout _checkMarkLayout;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Show()
		{
			base.Show();
			SetCharacter(Characters.CharacterId.tricky, new Vector2(32f, 0f), null, 19f, false, null, false);
			bigLabel.text = Strings.Get("POPUP_RATING_BIG_LABEL");
			pleaseLabel.text = Strings.Get("POPUP_RATING_PLEASE_LABEL");
			rateButtonLabel.text = Strings.Get("POPUP_RATING_RATE_BUTTON_LABEL");
			cancelButtonLabel.text = Strings.Get("POPUP_RATING_CANCEL_BUTTON_LABEL");
			notAgainLabel.text = Strings.Get("POPUP_RATING_NOTAGAIN_LABEL");
			_checkMarkLayout.Align();
		}

		[UsedImplicitly]
		public void OkClicked()
		{
			if (UIScreenController.Instance.GetCurrentPopupName() == "RatingPopup")
			{
				UIScreenController.Instance.CloseActivePopups(screenName);
				PreferencesSave.neverAskForRating = true;
				UserSave.lastVersionRated = DeviceUtility.GetBundleVersion();
				Flurry.LogEventWithSeveralParameters("Rate the app popup", "Result;Should ask again", "Ok;Do not ask again");
			}
			Application.OpenURL(Globals.RatingURL);
		}

		[UsedImplicitly]
		public void CancelClicked()
		{
			if (UIScreenController.Instance.GetCurrentPopupName() == "RatingPopup")
			{
				UIScreenController.Instance.CloseActivePopups(screenName);
				PreferencesSave.neverAskForRating = notAgainCheckBox.value;
				UserSave.lastVersionRated = DeviceUtility.GetBundleVersion();
				if (notAgainCheckBox.value)
				{
					Flurry.LogEventWithSeveralParameters("Rate the app popup", "Result;Should ask again", "Cancel;Do not ask again");
				}
				else
				{
					Flurry.LogEventWithSeveralParameters("Rate the app popup", "Result;Should ask again", "Cancel;Ask again");
				}
			}
		}
	}
}
