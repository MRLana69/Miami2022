using System;
using System.Collections;
using System.Collections.Generic;
using Authentication;
using Cloud;
using Cloud.Internal;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Utility;
using Kiloo.GameCenter;
using NaughtyAttributes;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class CloudSaveIntroductionPopup : UIPopup
	{
		private enum HideAction
		{
			None = 0,
			Closed = 1,
			Saved = 2,
			Refreshed = 3
		}

		[SerializeField]
		private UILabel _header;

		[SerializeField]
		private UILabel _speechBubbleText;

		[SerializeField]
		private UILabel _bottomText;

		[SerializeField]
		private UIWidget _skyline;

		[SerializeField]
		private UIWidget _skylineFiller;

		[SerializeField]
		private UILabel _buttonText;

		[SerializeField]
		private UIButtonOverlayOff _buttonOverlay;

		[SerializeField]
		private UISprite _spinner;

		private bool _isLoggedInNative;

		private HideAction _hideSourceAction;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_isLoggedInNative = Auth.isLoggedIn;
			SetupBasic();
			Localize();
		}

		protected override void Show()
		{
			_hideSourceAction = HideAction.None;
		}

		protected override void Hide()
		{
			if (_hideSourceAction == HideAction.Saved || _hideSourceAction == HideAction.Refreshed)
			{
				return;
			}
			if (_isLoggedInNative)
			{
				Action onUpdateFinished = () =>
				{
					if (CloudSave.state == CloudSaveState.Active)
					{
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.CloudSave,
							cloudSaveState = CloudSave.state
						});
					}
				};
				CoroutineUtil.StartCoroutine(CloudSave.LoginRoutine(onUpdateFinished, true));
				FlurryLogCloudSaveEnabled();
			}
			LogLastShownTime();
			FlurryLogPopupShown("CloseClicked");
		}

		private void Localize()
		{
			_header.text = Strings.Get("CLOUDSAVE_INTRODUCTION_HEADER");
			if (_isLoggedInNative)
			{
				_speechBubbleText.text = Strings.Get("CLOUDSAVE_INTRODUCTION_JAKE_B");
				_buttonText.text = Strings.Get("CLOUDSAVE_INTRODUCTION_BUTTON_B");
				_bottomText.text = string.Format(Strings.Get("CLOUDSAVE_INTRODUCTION_DESCRIPTION_B"));
			}
			else
			{
				_speechBubbleText.text = Strings.Get("CLOUDSAVE_INTRODUCTION_JAKE");
				_buttonText.text = Strings.Get("CLOUDSAVE_INTRODUCTION_BUTTON");
				_bottomText.text = string.Format(Strings.Get("CLOUDSAVE_INTRODUCTION_DESCRIPTION"), OnlineSettings.instance.GetCurrentPlatformPretty());
			}
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_header.color = instance.midnight;
			_speechBubbleText.color = instance.peacock;
			_buttonText.color = instance.white;
			_buttonText.effectColor = instance.shadow;
			_bottomText.color = instance.pink;
			_bottomText.effectColor = instance.shadow;
			_skyline.color = instance.royale;
			_skylineFiller.color = instance.royale;
			if (!Application.isPlaying)
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
			_spinner.gameObject.SetActive(false);
		}

		[Button(null)]
		private void LocalizeLoggedIn()
		{
			_isLoggedInNative = true;
			Localize();
		}

		[Button(null)]
		private void LocalizeLoggedOut()
		{
			_isLoggedInNative = false;
			Localize();
		}

		[UsedImplicitly]
		public void CloseClicked()
		{
			_hideSourceAction = HideAction.Closed;
			ClosePopup();
		}

		[UsedImplicitly]
		public void SaveNowClicked()
		{
			_hideSourceAction = HideAction.Saved;
			if (_isLoggedInNative)
			{
				UIScreenController.Instance.ClosePopup();
				Action onUpdateFinished = () =>
				{
					if (CloudSave.state == CloudSaveState.Active)
					{
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.CloudSave,
							cloudSaveState = CloudSave.state
						});
					}
				};
				CoroutineUtil.StartCoroutine(CloudSave.LoginRoutine(onUpdateFinished, true));
			}
			else
			{
				CoroutineUtil.StartCoroutine(LoginRoutine());
			}
			LogLastShownTime();
			FlurryLogCloudSaveEnabled();
			FlurryLogPopupShown("ButtonClicked");
		}

		private IEnumerator LoginRoutine()
		{
			_buttonText.gameObject.SetActive(false);
			_spinner.gameObject.SetActive(true);
			_buttonOverlay.GoToNotAvailableState();
			bool shouldDisplayGameCenterPopup = false;
			if (GameCenterPlugin.isSupported && !GameCenterPlugin.GetLocalPlayer().authenticated && !GameCenterPlugin.hasLoginView)
			{
				shouldDisplayGameCenterPopup = true;
			}
			if (!shouldDisplayGameCenterPopup)
			{
				yield return CloudSave.LoginRoutine(() =>
				{
					if (CloudSave.state == CloudSaveState.Active)
					{
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.CloudSave,
							cloudSaveState = CloudSave.state
						});
					}
				});
			}
			if (_buttonText != null)
			{
				_buttonText.gameObject.SetActive(true);
				_spinner.gameObject.SetActive(false);
				_buttonOverlay.GoToNormalState();
			}
			while (UIScreenController.Instance.GetCurrentPopupName() == "CloudSaveCelebrationPopup")
			{
				yield return null;
			}
			_hideSourceAction = HideAction.Refreshed;
			UIScreenController.Instance.CloseActivePopups("CloudSaveIntroductionPopup");
			if (shouldDisplayGameCenterPopup)
			{
				UIScreenController.Instance.PushPopup("ConnectGameCenterPopup");
			}
		}

		private void LogLastShownTime()
		{
			UserSave.introductionPopupShownCount++;
			OnboardingSave.lastShownCloudSaveIntroduction = CDateTime.UtcNow;
		}

		private void FlurryLogCloudSaveEnabled()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("FromIntroductionPopup", (!_isLoggedInNative) ? "NativeLoggedOut" : "NativeLoggedIn");
			dictionary.Add("Source", "FromIntroductionPopup");
			Dictionary<string, string> dictionary2 = dictionary;
			if (_isLoggedInNative)
			{
				dictionary2.Add("KilooTag", Auth.kilooTag);
			}
			Flurry.LogEventWithSeveralParameters("CloudSave Enabled", dictionary2);
		}

		private void FlurryLogPopupShown(string clickType)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add(clickType, (!_isLoggedInNative) ? "NativeLoggedOut" : "NativeLoggedIn");
			dictionary.Add("ShownCount", UserSave.introductionPopupShownCount.ToString());
			Dictionary<string, string> parameters = dictionary;
			Flurry.LogEventWithSeveralParameters("CloudSave Introduction Popup", parameters);
		}
	}
}
