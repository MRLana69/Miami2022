using System.Collections;
using System.Collections.Generic;
using Cloud;
using Cloud.Internal;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using Kiloo.GameCenter;
using NaughtyAttributes;
using UI.Popups.CloudSaveConfirmationPopupInternal;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveStatusPopup : UIPopup
	{
		private enum Status
		{
			Ok = 0,
			Error = 1,
			Conflict = 2
		}

		private Status _screenStatus;

		[SerializeField]
		private UILabel _headline;

		[SerializeField]
		private UILabel _subline;

		[SerializeField]
		private UILabel _bannerTopLabel;

		[SerializeField]
		private UILabel _bannerBottomLabel;

		[SerializeField]
		private UILabel _disconnectLabel;

		[SerializeField]
		private UILabel _statusLabel;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private UISprite _banner;

		[SerializeField]
		private UIWidget _skyline;

		[SerializeField]
		private UISprite _statusSprite;

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private GameObject _rays;

		[SerializeField]
		private GameObject _button;

		[SerializeField]
		private UIButtonOverlayOff _buttonOverlay;

		[SerializeField]
		private UISprite _spinner;

		[SerializeField]
		private GameObject _closeButton;

		private bool _authRoutineRunning;

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
			SetupStatus();
			_subline.text = string.Format(Strings.Get("CLOUDSAVE_USERNAME"), CloudSave.userName);
		}

		public override void ClosePopup()
		{
			if (!_authRoutineRunning)
			{
				base.ClosePopup();
			}
		}

		public override void BackButtonPressed()
		{
			ClosePopup();
		}

		public override void GainFocus()
		{
			StartCoroutine(DelayedGainFocus());
		}

		private IEnumerator DelayedGainFocus()
		{
			yield return null;
			SetupStatus();
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_headline.color = instance.midnight;
			_subline.color = instance.peacock;
			_disconnectLabel.color = instance.tuna;
			_banner.color = instance.grass;
			_bannerTopLabel.color = instance.sun;
			_bannerBottomLabel.color = instance.glow;
			_buttonLabel.color = instance.white;
			_buttonLabel.effectColor = instance.shadow;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
			_headline.text = Strings.Get("CLOUDSAVE_STATUS_HEADER");
			_bannerTopLabel.text = Strings.Get("CLOUDSAVE_STATUS_SAVED_LABELTITLE");
			_disconnectLabel.text = "[u]" + Strings.Get("CLOUDSAVE_STATUS_DISCONNECT");
		}

		private void ResetPopup()
		{
			_buttonLabel.gameObject.SetActive(true);
			_spinner.gameObject.SetActive(false);
			if (_buttonOverlay.gameObject.activeInHierarchy)
			{
				_buttonOverlay.GoToNormalState();
			}
			_closeButton.SetActive(true);
			_disconnectLabel.gameObject.SetActive(true);
		}

		private void SetupStatus()
		{
			if (!(_buttonLabel == null))
			{
				ResetPopup();
				switch (CloudSave.state)
				{
				case CloudSaveState.Error:
					_screenStatus = Status.Error;
					break;
				case CloudSaveState.Conflict:
					_screenStatus = Status.Conflict;
					break;
				default:
					_screenStatus = Status.Ok;
					break;
				}
				switch (_screenStatus)
				{
				case Status.Ok:
					SetupSavedStatus();
					break;
				case Status.Error:
					SetupErrorStatus();
					break;
				case Status.Conflict:
					SetupConflictStatus();
					break;
				}
				_bannerBottomLabel.text = CloudSaveDecorator.FormatDateTime(UserSave.latestSuccessfulCloudSaveSyncTime);
			}
		}

		[Button(null)]
		private void SetupSavedStatus()
		{
			ColorData instance = ColorData.instance;
			_background.color = instance.spring;
			_skyline.color = instance.christi;
			_statusLabel.color = instance.verdun;
			_statusLabel.effectColor = instance.mint;
			_rays.SetActive(true);
			_banner.gameObject.SetActive(true);
			_statusSprite.spriteName = "ic_check";
			_statusSprite.MakePixelPerfect();
			_statusLabel.text = Strings.Get("CLOUDSAVE_STATUS_SAVED_INFO");
			_button.SetActive(false);
		}

		[Button(null)]
		private void SetupErrorStatus()
		{
			ColorData instance = ColorData.instance;
			_background.color = instance.sand;
			_skyline.color = instance.desert;
			_statusLabel.color = instance.mahogany;
			_statusLabel.effectColor = instance.glow;
			_rays.SetActive(false);
			_banner.gameObject.SetActive(false);
			_button.SetActive(true);
			_statusSprite.spriteName = "ic_indicator_error";
			_statusSprite.MakePixelPerfect();
			switch (CloudSave.errorState)
			{
			case CloudSaveErrorState.Offline:
				_buttonLabel.text = Strings.Get("CLOUDSAVE_STATUS_OFFLINE_BUTTON");
				_statusLabel.text = Strings.Get("CLOUDSAVE_STATUS_OFFLINE_INFO");
				break;
			case CloudSaveErrorState.Unauthenticated:
				_buttonLabel.text = Strings.Get("CLOUDSAVE_STATUS_AUTHENTICATE_BUTTON");
				_statusLabel.text = Strings.Get("CLOUDSAVE_STATUS_AUTHENTICATE_INFO");
				break;
			case CloudSaveErrorState.GameOutdated:
				_buttonLabel.text = Strings.Get("CLOUDSAVE_STATUS_OUTDATED_BUTTON");
				_statusLabel.text = Strings.Get("CLOUDSAVE_STATUS_OUTDATED_INFO");
				break;
			}
		}

		[Button(null)]
		private void SetupConflictStatus()
		{
			ColorData instance = ColorData.instance;
			_background.color = instance.peach;
			_skyline.color = instance.safran;
			_statusLabel.color = instance.mahogany;
			_statusLabel.effectColor = instance.pink;
			_rays.SetActive(false);
			_banner.gameObject.SetActive(false);
			_button.SetActive(true);
			_statusSprite.spriteName = "ic_indicator_conflict";
			_statusSprite.MakePixelPerfect();
			_buttonLabel.text = Strings.Get("CLOUDSAVE_STATUS_CONFLICT_BUTTON");
			_statusLabel.text = Strings.Get("CLOUDSAVE_STATUS_CONFLICT_INFO");
		}

		[UsedImplicitly]
		public void ButtonClicked()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = _screenStatus.ToString();
			if (_screenStatus == Status.Error)
			{
				value = CloudSave.errorState.ToString();
			}
			dictionary.Add("ButtonClicked", value);
			Flurry.LogEventWithSeveralParameters("CloudSave Status Popup", dictionary);
			switch (_screenStatus)
			{
			case Status.Error:
				switch (CloudSave.errorState)
				{
				case CloudSaveErrorState.Offline:
				case CloudSaveErrorState.Unauthenticated:
					CoroutineUtil.StartCoroutine(AuthRoutine());
					break;
				case CloudSaveErrorState.GameOutdated:
					Application.OpenURL(Globals.AppStoreURL);
					break;
				}
				break;
			case Status.Conflict:
				UIScreenController.Instance.PushPopup("CloudSaveComparisonPopup");
				break;
			}
		}

		[UsedImplicitly]
		public void DisconnectClicked()
		{
			CloudSaveConfirmationPopupPayload.PreparePayload(CloudSaveConfirmationPopupPayload.ConfirmationState.Disconnect, () =>
			{
				UIScreenController.Instance.ClosePopup();
			}, () =>
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "Source", "FromStatusPopup" } };
				string value = _screenStatus.ToString();
				if (_screenStatus == Status.Error)
				{
					value = CloudSave.errorState.ToString();
				}
				dictionary.Add("FromStatusPopup", value);
				CloudSave.Disable(true);
				Flurry.LogEventWithSeveralParameters("CloudSave Disconnected", dictionary);
				UIScreenController.Instance.CloseActivePopups("CloudSaveStatusPopup");
			});
			UIScreenController.Instance.PushPopup("CloudSaveConfirmationPopup");
		}

		private IEnumerator AuthRoutine()
		{
			if (_authRoutineRunning)
			{
				yield break;
			}
			_authRoutineRunning = true;
			UIScreenController.Instance.topBar.ShowTopBar(false);
			_buttonLabel.gameObject.SetActive(false);
			_spinner.gameObject.SetActive(true);
			_buttonOverlay.GoToNotAvailableState();
			_closeButton.SetActive(false);
			_disconnectLabel.gameObject.SetActive(false);
			bool shouldDisplayGameCenterPopup = false;
			if (GameCenterPlugin.isSupported && !GameCenterPlugin.GetLocalPlayer().authenticated && !GameCenterPlugin.hasLoginView)
			{
				shouldDisplayGameCenterPopup = true;
			}
			if (shouldDisplayGameCenterPopup)
			{
				UIScreenController.Instance.PushPopup("ConnectGameCenterPopup");
			}
			else
			{
				yield return CloudSave.LoginRoutine(() =>
				{
					if (CloudSave.state == CloudSaveState.Active || CloudSave.state == CloudSaveState.Conflict)
					{
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.CloudSave,
							cloudSaveState = CloudSave.state
						});
					}
				});
			}
			SetupStatus();
			_authRoutineRunning = false;
			UIScreenController.Instance.topBar.ShowTopBar(true);
		}
	}
}
