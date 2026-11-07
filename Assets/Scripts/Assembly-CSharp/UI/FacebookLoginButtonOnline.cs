using System.Collections;
using JetBrains.Annotations;
using UI.Generic.Layout;
using UI.SlideIns;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
	public class FacebookLoginButtonOnline : MonoBehaviour
	{
		private enum LoginButtonState
		{
			Simple = 0,
			CouldNotConnectTryAgain = 1,
			Connecting = 2
		}

		[SerializeField]
		private GameObject _simpleContent;

		[SerializeField]
		private UILabel _simpleLabel;

		[SerializeField]
		private Layout _simpleLayout;

		[SerializeField]
		private GameObject _connectingToGameObject;

		[SerializeField]
		private Layout _connectingLayout;

		[FormerlySerializedAs("_connectingTo")]
		[SerializeField]
		private UILabel _connectingLabel;

		[SerializeField]
		private GameObject _couldNotConnectTryAgainGameObject;

		[SerializeField]
		private Layout _tryAgainLayout;

		[SerializeField]
		private UILabel _tryAgain1;

		[SerializeField]
		private UILabel _tryAgain2;

		[SerializeField]
		private UIButtonOverlayOff _btnOverlayOffDefault;

		[SerializeField]
		private UIButtonOverlayOff _btnOverlayOffTryAgain;

		private bool _isGameOverScreen;

		private bool _shouldDisplayAwardsSpecific;

		private bool _shouldDisplayTopRunSpecific;

		private LoginButtonState _loginButtonState;

		private bool _enabled = true;

		public void Init(bool isLoggedIntoFacebook, bool isAwardsSpecific = false, bool isLoggedIntoGC = false, bool isTopRunSpecific = false, bool noGCFriends = false, bool isGameOverScreen = false)
		{
			_isGameOverScreen = isGameOverScreen;
			_shouldDisplayAwardsSpecific = isAwardsSpecific && !isLoggedIntoGC;
			_shouldDisplayTopRunSpecific = isTopRunSpecific;
			SocialManager.instance.AddFacebookStateChangedHandler(FacebookStateChanged);
			SocialManager.instance.friendsConsolidatedHandler += FriendsConsolidated;
			SetState(LoginButtonState.Simple);
			EnableTouch(true);
			string text = Strings.Get("SOCIAL_BUTTON_LOGIN");
			if (_shouldDisplayAwardsSpecific)
			{
				text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_LOGIN_2_AWARDS");
			}
			else if (_shouldDisplayTopRunSpecific)
			{
				text = ((!_isGameOverScreen) ? Strings.Get("FACEBOOK_BUTTON_CONNECT") : Strings.Get("FACEBOO_PROMOTION_TOPRUN_TITEL"));
			}
			_simpleLabel.text = text;
		}

		private void SetState(LoginButtonState newState)
		{
			_loginButtonState = newState;
			_connectingToGameObject.SetActive(_loginButtonState == LoginButtonState.Connecting);
			_couldNotConnectTryAgainGameObject.SetActive(_loginButtonState == LoginButtonState.CouldNotConnectTryAgain);
			_simpleContent.SetActive(_loginButtonState == LoginButtonState.Simple);
			_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.SocialGrey);
			switch (_loginButtonState)
			{
			case LoginButtonState.Simple:
				_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.Secondary);
				if (_shouldDisplayTopRunSpecific && !_isGameOverScreen)
				{
					_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.Primary);
				}
				_simpleLayout.Align(true);
				break;
			case LoginButtonState.CouldNotConnectTryAgain:
				if (_isGameOverScreen)
				{
					_tryAgain1.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_TRY_AGAIN_1");
					_tryAgain2.text = Strings.Get("BUTTON_TRY_AGAIN");
					_tryAgainLayout.Align(true);
				}
				else if (_shouldDisplayTopRunSpecific)
				{
					_tryAgain2.text = Strings.Get("FACEBOOK_BUTTON_RECONNECT");
					_btnOverlayOffDefault.SetButtonType(UIButtonOverlayOff.ButtonType.Primary);
				}
				break;
			case LoginButtonState.Connecting:
				_connectingLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
				_connectingLayout.Align(true);
				break;
			}
		}

		private void OnDestroy()
		{
			SocialManager.instance.RemoveFacebookStateChangedHandler(FacebookStateChanged);
			SocialManager.instance.friendsConsolidatedHandler -= FriendsConsolidated;
		}

		[UsedImplicitly]
		private void OnPress(bool pressed)
		{
			if (_enabled)
			{
				if (_btnOverlayOffDefault != null)
				{
					_btnOverlayOffDefault.ButtonPressed(pressed);
				}
				if (_btnOverlayOffTryAgain != null && _couldNotConnectTryAgainGameObject != null && _couldNotConnectTryAgainGameObject.activeInHierarchy)
				{
					_btnOverlayOffTryAgain.ButtonPressed(pressed);
				}
			}
		}

		[UsedImplicitly]
		private void OnClick()
		{
			if (_enabled)
			{
				SetState(LoginButtonState.Connecting);
				EnableTouch(false);
				SocialManager.instance.FacebookLogin(FacebookLoggedIn);
			}
		}

		private void FacebookLoggedIn(bool success)
		{
			if ((bool)this && base.gameObject.activeInHierarchy)
			{
				if (!success)
				{
					StartCoroutine(UserFeedback());
				}
				else
				{
					UIScreenController.Instance.FacebookLogIn(true);
					EnableTouch(false);
				}
			}
			QueueSlideIn(success);
		}

		private IEnumerator UserFeedback()
		{
			yield return new WaitForSeconds(2f);
			SetState(LoginButtonState.CouldNotConnectTryAgain);
			EnableTouch(true);
		}

		private void QueueSlideIn(bool success)
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.Facebook,
				facebookState = (success ? UISlideInFacebook.FacebookSlideInState.ConnectSucceeded : UISlideInFacebook.FacebookSlideInState.ConnectFailed)
			});
		}

		private void FacebookStateChanged()
		{
			if ((bool)this)
			{
				switch (SocialManager.instance.facebookState)
				{
				case SocialManager.FacebookState.LoggingIn:
					SetState(LoginButtonState.Connecting);
					EnableTouch(false);
					break;
				case SocialManager.FacebookState.Error:
					SetState(LoginButtonState.CouldNotConnectTryAgain);
					EnableTouch(true);
					break;
				case SocialManager.FacebookState.LoggedIn:
					break;
				case SocialManager.FacebookState.None:
					break;
				}
			}
		}

		private void FriendsConsolidated(bool success)
		{
			if (SocialManager.instance.facebookIsLoggedIn && !SocialManager.instance.AreFacebookFriendsConsolidated())
			{
				_simpleLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_COULD_NOT_CONNECT_1");
				SetState(LoginButtonState.Simple);
				EnableTouch(false);
			}
		}

		private void EnableTouch(bool enable)
		{
			_enabled = enable;
			if (_btnOverlayOffDefault != null)
			{
				if (_enabled)
				{
					_btnOverlayOffDefault.GoToNormalState();
				}
				else
				{
					_btnOverlayOffDefault.GoToNotAvailableState();
				}
			}
		}
	}
}
