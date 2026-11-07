using System.Collections;
using System.Collections.Generic;
using Authentication;
using Cloud;
using Cloud.Internal;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using UI.Generic.Layout;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveDisabledStatusPopup : UIPopup
	{
		[SerializeField]
		private UILabel _headline;

		[SerializeField]
		private UILabel _message1;

		[SerializeField]
		private UILabel _message2;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UIWidget _skyline;

		[SerializeField]
		private Layout _horizontalLayout;

		[SerializeField]
		private Layout _verticalLayout;

		[SerializeField]
		private UIButtonOverlayOff _buttonOverlay;

		[SerializeField]
		private UISprite _spinner;

		[SerializeField]
		private UISprite _glow;

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
			ResetPopup();
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

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_headline.color = instance.midnight;
			_message1.color = instance.peacock;
			_message2.color = instance.peacock;
			_background.color = instance.sky.WithAlpha(0.7f);
			_skyline.color = instance.push.WithAlpha(0.5f);
			_buttonLabel.color = instance.white;
			_buttonLabel.effectColor = instance.shadow;
			_glow.color = instance.sun;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
			_headline.text = Strings.Get("CLOUDSAVE_STATUS_DISABLED_TITLE");
			_message1.text = "+ " + Strings.Get("CLOUDSAVE_STATUS_DISABLED_INFO1");
			_message2.text = "+ " + string.Format(Strings.Get("CLOUDSAVE_STATUS_DISABLED_INFO2"), OnlineSettings.instance.GetCurrentPlatformPretty());
			_buttonLabel.text = Strings.Get("CLOUDSAVE_STATUS_DISABLED_BUTTON");
			_verticalLayout.Align();
			_horizontalLayout.Align();
			_spinner.gameObject.SetActive(false);
		}

		private void ResetPopup()
		{
			if (!(_buttonLabel == null))
			{
				_buttonLabel.gameObject.SetActive(true);
				_spinner.gameObject.SetActive(false);
				_buttonOverlay.GoToNormalState();
				_closeButton.SetActive(true);
			}
		}

		[UsedImplicitly]
		public void ButtonClicked()
		{
			PreferencesSave.cloudSaveOptedOut = false;
			SaveManager.Save();
			CoroutineUtil.StartCoroutine(ButtonClicked_Routine());
		}

		private IEnumerator ButtonClicked_Routine()
		{
			if (_authRoutineRunning)
			{
				yield break;
			}
			_authRoutineRunning = true;
			UIScreenController.Instance.topBar.ShowTopBar(false);
			_closeButton.SetActive(false);
			_buttonLabel.gameObject.SetActive(false);
			_spinner.gameObject.SetActive(true);
			_buttonOverlay.GoToNotAvailableState();
			bool loggedIntoNative = Auth.isLoggedIn;
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
			while (UIScreenController.Instance.GetCurrentPopupName() == "CloudSaveCelebrationPopup")
			{
				yield return null;
			}
			ResetPopup();
			UIScreenController.Instance.CloseActivePopups("CloudSaveDisabledStatusPopup");
			Dictionary<string, string> dict = new Dictionary<string, string>
			{
				{
					"FromDisabledPopup",
					(!loggedIntoNative) ? "NativeLoggedOut" : "NativeLoggedIn"
				},
				{ "Source", "FromDisabledPopup" }
			};
			if (loggedIntoNative)
			{
				dict.Add("KilooTag", Auth.kilooTag);
			}
			Flurry.LogEventWithSeveralParameters("CloudSave Enabled", dict);
			_authRoutineRunning = false;
			UIScreenController.Instance.topBar.ShowTopBar(true);
		}
	}
}
