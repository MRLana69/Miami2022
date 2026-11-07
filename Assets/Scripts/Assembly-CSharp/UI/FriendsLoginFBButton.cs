using System;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UnityEngine;

namespace UI
{
	public class FriendsLoginFBButton : MonoBehaviour
	{
		[SerializeField]
		private GameObject _connectingContent;

		[SerializeField]
		private GameObject _normalContentLeaderboard;

		[SerializeField]
		private GameObject _overlay;

		[SerializeField]
		private UILabel _leaderboardLabel1;

		[SerializeField]
		private Collider _leaderboardCollider;

		[SerializeField]
		private UISprite _connectingFill;

		[SerializeField]
		private UILabel _connectingLabel;

		[SerializeField]
		private Layout _connectingLayout;

		[SerializeField]
		private Layout _loginLayout;

		private Action _clickedAction;

		private Action _activateReconnect;

		private bool _leaderboardScreen;

		private bool _noFriends;

		private bool _isGameOverScreen;

		public void Init(Action clickedAction, bool leaderboardScreen = false, bool noFriends = false, bool isGameOverScreen = false, Action activateReconnect = null)
		{
			_clickedAction = clickedAction;
			_connectingFill.color = GlobalColors.SOCIAL_GREY;
			_connectingLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
			_activateReconnect = activateReconnect;
			_leaderboardScreen = leaderboardScreen;
			_noFriends = noFriends;
			_isGameOverScreen = isGameOverScreen;
			SetToLogInState(leaderboardScreen, noFriends, isGameOverScreen);
		}

		[UsedImplicitly]
		public void ConnectClicked()
		{
			_clickedAction.SafeInvoke();
			SetToConnectingState();
		}

		private void SetToLogInState(bool isLeaderboardScreen, bool noFriends, bool isGameOverScreen)
		{
			_overlay.SetActive(true);
			_connectingContent.SetActive(false);
			_normalContentLeaderboard.SetActive(isLeaderboardScreen);
			if (noFriends)
			{
				if (isGameOverScreen)
				{
					_leaderboardLabel1.text = Strings.Get("FACEBOO_PROMOTION_TOPRUN_TITEL");
				}
				else if (_activateReconnect == null)
				{
					_leaderboardLabel1.text = Strings.Get("FACEBOOK_BUTTON_RECONNECT");
				}
				else
				{
					_leaderboardLabel1.text = Strings.Get("FACEBOOK_BUTTON_CONNECT");
				}
			}
			else
			{
				_leaderboardLabel1.text = Strings.Get("FACEBOOK_BUTTON_LEADERBOARD_TEXT");
			}
			_loginLayout.Align();
			_leaderboardCollider.enabled = true;
		}

		private void SetToConnectingState()
		{
			_overlay.SetActive(false);
			_normalContentLeaderboard.SetActive(false);
			_connectingContent.SetActive(true);
			_connectingLayout.Align();
			_leaderboardCollider.enabled = false;
		}

		private void FBStateChangedHandler()
		{
			SocialManager.FacebookState facebookState = SocialManager.instance.facebookState;
			if (facebookState == SocialManager.FacebookState.Error)
			{
				if (_activateReconnect != null)
				{
					_activateReconnect.SafeInvoke();
				}
				else
				{
					SetToLogInState(_leaderboardScreen, _noFriends, _isGameOverScreen);
				}
			}
		}

		private void OnEnable()
		{
			SocialManager.instance.AddFacebookStateChangedHandler(FBStateChangedHandler);
		}

		private void OnDisable()
		{
			SocialManager.instance.RemoveFacebookStateChangedHandler(FBStateChangedHandler);
		}
	}
}
