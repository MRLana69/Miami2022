using System;
using Info;
using JetBrains.Annotations;
using UI.Generic.Layout;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups.SettingsPopupInternal
{
	public class SettingsLogInFacebookButton : MonoBehaviour
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private Layout _layout;

		[SerializeField]
		private UISprite _fbIcon;

		[SerializeField]
		private UISprite _spinner;

		[SerializeField]
		private GameObject _overlay;

		[SerializeField]
		private UISprite _fill;

		private SettingsPopup _settingsPopup;

		private bool _facebookLoginRunning;

		public void InitButton(SettingsPopup settingsPopup)
		{
			_settingsPopup = settingsPopup;
		}

		[UsedImplicitly]
		private void OnClick()
		{
			HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
			_facebookLoginRunning = true;
			if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
			{
				UIScreenController.Instance.FacebookLogIn(true);
			}
			SetToConnectingState();
		}

		private void OnFacebookLoginResult(bool loggedIn)
		{
			if (!loggedIn && _facebookLoginRunning)
			{
				_facebookLoginRunning = false;
				QueueFacebookFailSlideIn(OnFailSlideInFinish);
			}
		}

		private void QueueFacebookFailSlideIn(Action onFinishCallback)
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.Facebook,
				facebookState = UISlideInFacebook.FacebookSlideInState.ConnectFailed,
				onDidShowCallback = onFinishCallback
			});
		}

		private void OnFailSlideInFinish()
		{
			FacebookStatusChanged(false);
		}

		private void FacebookStatusChanged(bool success)
		{
			if (success)
			{
				if (!FacebookRewardsInfo.PayoutRewardIfPossible())
				{
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.Facebook,
						facebookState = UISlideInFacebook.FacebookSlideInState.ConnectSucceeded
					});
				}
				_settingsPopup.ConnectedToFacebook();
			}
			else
			{
				if (_facebookLoginRunning)
				{
					OnFacebookLoginResult(false);
				}
				SetToLogInState();
			}
			HouseKeeper.SetFacebookPayoutActionToDefault();
			_facebookLoginRunning = false;
		}

		private void OnEnable()
		{
			SetToLogInState();
		}

		private void OnDisable()
		{
			if (_facebookLoginRunning)
			{
				HouseKeeper.SetFacebookPayoutActionToDefault();
				_facebookLoginRunning = false;
			}
		}

		private void SetToLogInState()
		{
			_overlay.SetActive(true);
			_fill.color = GlobalColors.TERTIARY_ACTION_COLOR;
			_description.gameObject.SetActive(true);
			_title.text = Strings.Get("SETTINGS_POPUP_FB_TITLE");
			_description.text = Strings.Get("SETTINGS_POPUP_FB_LOG_IN_DESC");
			ColorData instance = ColorData.instance;
			_title.color = instance.royale;
			_description.color = instance.push;
			_layout.Align();
			_fbIcon.gameObject.SetActive(true);
			_spinner.gameObject.SetActive(false);
			GetComponent<Collider>().enabled = true;
		}

		private void SetToConnectingState()
		{
			_overlay.SetActive(false);
			_fill.color = GlobalColors.SOCIAL_GREY;
			_description.gameObject.SetActive(false);
			_title.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
			_layout.Align();
			_fbIcon.gameObject.SetActive(false);
			_spinner.gameObject.SetActive(true);
			GetComponent<Collider>().enabled = false;
		}
	}
}
