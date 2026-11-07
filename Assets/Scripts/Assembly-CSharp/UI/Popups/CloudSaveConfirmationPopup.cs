using System.Collections.Generic;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UI.Generic;
using UI.Popups.CloudSaveConfirmationPopupInternal;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveConfirmationPopup : UIPopup
	{
		[SerializeField]
		private UILabel _text;

		[SerializeField]
		private UILabel _badButton;

		[SerializeField]
		private UILabel _goodButton;

		[SerializeField]
		private UISprite _cornerIcon;

		[SerializeField]
		private UISprite _cornerAccentIcon;

		[SerializeField]
		private FitSpriteToWidget _fitCornerIcon;

		[SerializeField]
		private FitSpriteToWidget _fitCornerAccentIcon;

		private CloudSaveConfirmationPopupPayload _payload;

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
			SetupBasic();
			SetupConfirmation(CloudSaveConfirmationPopupPayload.activePayload);
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_text.color = instance.peacock;
			_goodButton.effectColor = instance.midnight;
			_badButton.effectColor = instance.midnight;
			base.gameObject.Blink();
			_badButton.text = "Bad";
			_goodButton.text = "Good";
		}

		private void SetupConfirmation(CloudSaveConfirmationPopupPayload payload)
		{
			_payload = payload;
			_text.text = _payload.text;
			_badButton.text = _payload.badButtonText;
			_goodButton.text = _payload.goodButtonText;
			_cornerIcon.spriteName = _payload.cornerIcon;
			_cornerAccentIcon.spriteName = _payload.cornerAccentIcon;
			_fitCornerIcon.FitNow();
			_fitCornerAccentIcon.FitNow();
		}

		[UsedImplicitly]
		public void BadButtonClicked()
		{
			_payload.badButtonClicked.SafeInvoke();
			FlurryLogConfirmationPopup("Bad");
		}

		[UsedImplicitly]
		public void GoodButtonClicked()
		{
			_payload.goodButtonClicked.SafeInvoke();
			FlurryLogConfirmationPopup("Good");
		}

		[Button(null)]
		[UsedImplicitly]
		private void SetupDisconnect()
		{
			SetupConfirmation(CloudSaveConfirmationPopupPayload.DISCONNECT_PAYLOAD);
		}

		[Button(null)]
		[UsedImplicitly]
		private void SetupKeepLocal()
		{
			SetupConfirmation(CloudSaveConfirmationPopupPayload.KEEP_LOCAL_PAYLOAD);
		}

		[Button(null)]
		[UsedImplicitly]
		private void SetupKeepOnline()
		{
			SetupConfirmation(CloudSaveConfirmationPopupPayload.KEEP_ONLINE_PAYLOAD);
		}

		[Button(null)]
		[UsedImplicitly]
		private void SetupLoadProgress()
		{
			SetupConfirmation(CloudSaveConfirmationPopupPayload.LOAD_PROGRESS_PAYLOAD);
		}

		private void FlurryLogConfirmationPopup(string parameterValue)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add(_payload.flurryParameter, parameterValue);
			dictionary.Add("state", _payload.flurryParameter);
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("CloudSave Confirmation Popup", parameters);
		}
	}
}
