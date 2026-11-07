using Info;
using JetBrains.Annotations;
using Kiloo.Analytics;
using Kiloo.Common.Extensions;
using Kiloo.Common.LocalizationManager;
using NaughtyAttributes;
using UI.Generic;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class ConsentPopup : UIScreen
	{
		[SerializeField]
		private UILabel _consentLongDetails;

		[SerializeField]
		private UILabel _consentAffirmation;

		[SerializeField]
		private UILabel _okLabel;

		[SerializeField]
		private UILabel _acceptLabel;

		[SerializeField]
		private UIButtonOverlayOff _okButton;

		[SerializeField]
		private UISprite _acceptButtonFill;

		[SerializeField]
		private UISprite _acceptButtonOverlay;

		private ButtonText _okButtonText;

		[SerializeField]
		private UISprite _checkMarkSprite;

		private bool _isAcceptChecked;

		private TweenAlpha _checkMarkTweenCoroutine;

		protected override void Init()
		{
			base.Init();
			RefreshLabels();
			_okButtonText = _okLabel.GetComponent<ButtonText>();
		}

		protected override void Show()
		{
			base.Show();
			_okButton.GoToNotAvailableState();
			_isAcceptChecked = false;
			_checkMarkSprite.alpha = 0f;
			_acceptLabel.text = Strings.GetLegal("ConsentAcceptUnchecked");
			_acceptButtonFill.color = GlobalColors.PRIMARY_ACTION_COLOR;
			_acceptButtonOverlay.gameObject.SetActive(true);
			_okButtonText.Refresh();
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		public override void BackButtonPressed()
		{
		}

		[Button(null)]
		private void RefreshLabels()
		{
			_consentAffirmation.text = Strings.GetLegal("ConsentAffirmation");
			_consentLongDetails.text = Strings.GetLegal("ConsentDetails").Populate("[u][c][2572A8][url=" + Globals.GetPrivacyPolicyUrl() + "]", "[/url][-][/c][/u]");
			_okLabel.text = Strings.GetLegal("ConsentOK");
			_acceptLabel.text = Strings.GetLegal("ConsentAcceptUnchecked");
		}

		[UsedImplicitly]
		public void CheckboxClicked()
		{
			_isAcceptChecked = !_isAcceptChecked;
			if (_isAcceptChecked)
			{
				_okButton.GoToNormalState();
				_acceptLabel.text = Strings.GetLegal("ConsentAcceptChecked");
				_acceptButtonFill.color = GlobalColors.PRIMARY_ACTION_COLOR_UNAVAILABLE;
			}
			else
			{
				_okButton.GoToNotAvailableState();
				_acceptLabel.text = Strings.GetLegal("ConsentAcceptUnchecked");
				_acceptButtonFill.color = GlobalColors.PRIMARY_ACTION_COLOR;
			}
			_checkMarkTweenCoroutine = TweenAlpha.Begin(_checkMarkSprite.gameObject, 0.15f, (!_isAcceptChecked) ? 0f : 1f);
			_acceptButtonOverlay.gameObject.SetActive(!_isAcceptChecked);
			_okButtonText.Refresh();
		}

		[UsedImplicitly]
		public void AllowClicked()
		{
			if (_checkMarkTweenCoroutine != null)
			{
				_checkMarkTweenCoroutine.Destroy();
			}
			AgeRestriction.RegisterConsent(true);
			Flurry.LogEvent("Consent given");
			Manager.LogEvent(Kiloo.Analytics.EventType.ConsentGiven);
			UIScreenController.Instance.ClosePopup();
		}

		[UsedImplicitly]
		public void PrivacyPolicyClicked(UILabel label)
		{
			string urlAtPosition = label.GetUrlAtPosition(UICamera.lastWorldPosition);
			if (urlAtPosition == Globals.GetPrivacyPolicyUrl())
			{
				Flurry.LogEvent("Consent Privacy Policy link opened");
				Manager.LogEvent(Kiloo.Analytics.EventType.ConsentPrivacyPolicyOpened);
				Application.OpenURL(urlAtPosition);
			}
		}

		[Button(null)]
		[UsedImplicitly]
		public void English()
		{
			SetLanguage("en-US");
		}

		[Button(null)]
		[UsedImplicitly]
		public void French()
		{
			SetLanguage("fr-FR");
		}

		[Button(null)]
		[UsedImplicitly]
		public void Italian()
		{
			SetLanguage("it-IT");
		}

		[Button(null)]
		[UsedImplicitly]
		public void German()
		{
			SetLanguage("de-DE");
		}

		[Button(null)]
		[UsedImplicitly]
		public void Spanish()
		{
			SetLanguage("es-ES");
		}

		[Button(null)]
		[UsedImplicitly]
		public void Portuguese()
		{
			SetLanguage("pt-BR");
		}

		private void SetLanguage(string locale)
		{
			LegalTextData.instance.SetLocale(locale);
			RefreshLabels();
		}
	}
}
