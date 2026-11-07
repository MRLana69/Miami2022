using System;

namespace UI.Popups.CloudSaveConfirmationPopupInternal
{
	public class CloudSaveConfirmationPopupPayload
	{
		public enum ConfirmationState
		{
			Disconnect = 0,
			KeepLocal = 1,
			KeepOnline = 2,
			LoadProgress = 3
		}

		public static readonly CloudSaveConfirmationPopupPayload DISCONNECT_PAYLOAD = new CloudSaveConfirmationPopupPayload
		{
			text = Strings.Get("CLOUDSAVE_CONFIRMATION_DESCRIPTION_DISCONNECT"),
			badButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_DISCONNECT"),
			badButtonClicked = () =>
			{
			},
			goodButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_CANCEL"),
			goodButtonClicked = () =>
			{
			},
			cornerIcon = "ic_cloud",
			cornerAccentIcon = "ic_x_red",
			flurryParameter = "Disconnect"
		};

		public static readonly CloudSaveConfirmationPopupPayload KEEP_LOCAL_PAYLOAD = new CloudSaveConfirmationPopupPayload
		{
			text = Strings.Get("CLOUDSAVE_CONFIRMATION_DESCRIPTION_LOCAL"),
			badButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_CANCEL"),
			badButtonClicked = () =>
			{
			},
			goodButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_UPLOAD"),
			goodButtonClicked = () =>
			{
			},
			cornerIcon = "ic_phone",
			cornerAccentIcon = "ic_upload",
			flurryParameter = "KeepLocal"
		};

		public static readonly CloudSaveConfirmationPopupPayload KEEP_ONLINE_PAYLOAD = new CloudSaveConfirmationPopupPayload
		{
			text = Strings.Get("CLOUDSAVE_CONFIRMATION_DESCRIPTION_CLOUD"),
			badButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_CANCEL"),
			badButtonClicked = () =>
			{
			},
			goodButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_DOWNLOAD"),
			goodButtonClicked = () =>
			{
			},
			cornerIcon = "ic_cloud",
			cornerAccentIcon = "ic_download",
			flurryParameter = "KeepCloud"
		};

		public static readonly CloudSaveConfirmationPopupPayload LOAD_PROGRESS_PAYLOAD = new CloudSaveConfirmationPopupPayload
		{
			text = Strings.Get("CLOUDSAVE_CONFIRMATION_DESCRIPTION_LOAD"),
			badButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_CANCEL"),
			badButtonClicked = () =>
			{
			},
			goodButtonText = Strings.Get("CLOUDSAVE_CONFIRMATION_BUTTON_LOAD"),
			goodButtonClicked = () =>
			{
			},
			cornerIcon = "ic_cloud",
			cornerAccentIcon = "ic_download",
			flurryParameter = "LoadProgress"
		};

		public Action goodButtonClicked;

		public Action badButtonClicked;

		public string text;

		public string badButtonText;

		public string goodButtonText;

		public string cornerIcon;

		public string cornerAccentIcon;

		public string flurryParameter;

		public static CloudSaveConfirmationPopupPayload activePayload { get; private set; }

		public static void PreparePayload(ConfirmationState state, Action goodButtonClicked, Action badButtonClicked)
		{
			switch (state)
			{
			case ConfirmationState.Disconnect:
				activePayload = DISCONNECT_PAYLOAD;
				break;
			case ConfirmationState.KeepLocal:
				activePayload = KEEP_LOCAL_PAYLOAD;
				break;
			case ConfirmationState.KeepOnline:
				activePayload = KEEP_ONLINE_PAYLOAD;
				break;
			case ConfirmationState.LoadProgress:
				activePayload = LOAD_PROGRESS_PAYLOAD;
				break;
			}
			activePayload.goodButtonClicked = goodButtonClicked;
			activePayload.badButtonClicked = badButtonClicked;
		}
	}
}
