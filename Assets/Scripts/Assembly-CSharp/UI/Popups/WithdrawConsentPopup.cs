using Info;
using JetBrains.Annotations;
using Kiloo.Analytics;
using NaughtyAttributes;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class WithdrawConsentPopup : UIPopup
	{
		[SerializeField]
		private UILabel _header;

		[SerializeField]
		private UILabel _bodyText;

		[SerializeField]
		private UILabel _requirement;

		[SerializeField]
		private UILabel _verification;

		[SerializeField]
		private UILabel _okLabel;

		[SerializeField]
		private UILabel _cancelLabel;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		[Button(null)]
		protected override void Init()
		{
			_header.text = Strings.GetLegal("WithdrawConsentTitle");
			_header.color = ColorData.instance.midnight;
			_bodyText.text = Strings.GetLegal("WithdrawConsentBody");
			_requirement.text = Strings.GetLegal("WithdrawConsentRequirement");
			_verification.text = Strings.GetLegal("WithdrawConsentVerification");
			_okLabel.text = Strings.GetLegal("WithdrawConsentYes");
			_cancelLabel.text = Strings.GetLegal("WithdrawConsentNo");
		}

		[UsedImplicitly]
		public void WithdrawClicked()
		{
			AgeRestriction.RegisterConsent(false);
			LogWithdraw("Yes");
			UIScreenController.Instance.ignoreLastPopupClosed = true;
			UIScreenController.Instance.ClosePopup();
			UIScreen screenFromCache = UIScreenController.Instance.GetScreenFromCache("SettingsPopup");
			if (screenFromCache != null)
			{
				UIScreenController.Instance.ignoreLastPopupClosed = true;
				UIScreenController.Instance.ClosePopup();
			}
			UIScreenController.Instance.PushPopup("ConsentPopup");
		}

		[UsedImplicitly]
		public void OnClose()
		{
			LogWithdraw("Cancel");
			ClosePopup();
		}

		[UsedImplicitly]
		public void OnNoClicked()
		{
			LogWithdraw("No");
			ClosePopup();
		}

		private void LogWithdraw(string result)
		{
			Flurry.LogEventWithAParameter("Consent withdrawn", "Result", result);
			Manager.LogEvent(Kiloo.Analytics.EventType.ConsentWithdrawn, false, result);
		}
	}
}
