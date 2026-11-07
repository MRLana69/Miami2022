using System.Collections.Generic;
using Cloud;
using Cloud.Internal;
using Data.Save;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UI.Popups.CloudSaveConfirmationPopupInternal;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveReminderPopup : UIPopup
	{
		[SerializeField]
		private UILabel _headline;

		[SerializeField]
		private UILabel _subline;

		[SerializeField]
		private UILabel _messageLabel;

		[SerializeField]
		private UILabel _disableLabel;

		[SerializeField]
		private UILabel _fixLabel;

		[SerializeField]
		private UIWidget _skyline;

		[SerializeField]
		private UISprite _background;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			SetupBasic();
		}

		protected override void Show()
		{
			_messageLabel.text = string.Format(Strings.Get("CLOUDSAVE_REMINDER_LASTSYNCED"), CloudSaveDecorator.FormatDateTime(UserSave.latestSuccessfulCloudSaveSyncTime));
			UserSave.lastCloudSaveReminderShownTime = CDateTime.UtcNow;
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_headline.color = instance.midnight;
			_subline.color = instance.peacock;
			_messageLabel.color = instance.mahogany;
			_messageLabel.effectColor = instance.glow;
			_disableLabel.color = instance.white;
			_disableLabel.effectColor = instance.shadow;
			_fixLabel.color = instance.white;
			_fixLabel.effectColor = instance.shadow;
			_background.color = instance.sand;
			_skyline.color = instance.desert;
			base.gameObject.Blink();
			_headline.text = Strings.Get("CLOUDSAVE_REMINDER_HEADER");
			_subline.text = Strings.Get("CLOUDSAVE_REMINDER_SUBTITLE");
			_disableLabel.text = Strings.Get("CLOUDSAVE_REMINDER_BUTTON_DISABLE");
			_fixLabel.text = Strings.Get("CLOUDSAVE_REMINDER_BUTTON_FIX");
		}

		[UsedImplicitly]
		public void DisableClicked()
		{
			CloudSaveConfirmationPopupPayload.PreparePayload(CloudSaveConfirmationPopupPayload.ConfirmationState.Disconnect, () =>
			{
				UIScreenController.Instance.ClosePopup();
			}, () =>
			{
				CloudSave.Disable(true);
				UIScreenController.Instance.CloseActivePopups("CloudSaveReminderPopup");
				Dictionary<string, string> parameters = new Dictionary<string, string>
				{
					{ "Source", "FromReminderPopup" },
					{
						"FromReminderPopup",
						string.Empty
					}
				};
				Flurry.LogEventWithSeveralParameters("CloudSave Disconnected", parameters);
			});
			UIScreenController.Instance.PushPopup("CloudSaveConfirmationPopup");
			FlurryLogOnButtonClicked("Disconnect");
		}

		[UsedImplicitly]
		public void FixClicked()
		{
			switch (CloudSave.state)
			{
			case CloudSaveState.Inactive:
			case CloudSaveState.Active:
			case CloudSaveState.Error:
			{
				UIScreenController.Instance.QueuePopup("CloudSaveStatusPopup");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string value = CloudSave.state.ToString();
				if (CloudSave.state == CloudSaveState.Error)
				{
					value = CloudSave.errorState.ToString();
				}
				dictionary.Add("Source", "FromReminderPopup");
				dictionary.Add("FromReminderPopup", value);
				Flurry.LogEventWithSeveralParameters("CloudSave Status Popup", dictionary);
				break;
			}
			case CloudSaveState.Conflict:
				UIScreenController.Instance.QueuePopup((CloudSave.conflictResolver.recommendation != CloudSaveConflictResolver.Recommendation.Theirs) ? "CloudSaveComparisonPopup" : "CloudSavePromotionPopup");
				break;
			}
			UIScreenController.Instance.ClosePopup();
			FlurryLogOnButtonClicked("Fix");
		}

		[UsedImplicitly]
		public void CloseClicked()
		{
			ClosePopup();
			FlurryLogOnButtonClicked("Close");
		}

		private void FlurryLogOnButtonClicked(string buttonClicked)
		{
			string value = CloudSave.state.ToString();
			if (CloudSave.state == CloudSaveState.Error)
			{
				value = CloudSave.errorState.ToString();
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ButtonClicked", buttonClicked);
			dictionary.Add("State", value);
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("CloudSave Reminder Popup", parameters);
		}
	}
}
