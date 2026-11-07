using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInFacebook : UISlideIn
	{
		public enum FacebookSlideInState
		{
			ConnectFailed = 0,
			ConnectSucceeded = 1,
			LoggedOut = 2
		}

		[SerializeField]
		private UILabel _statusLabel;

		[SerializeField]
		private UISprite _onlineIcon;

		[SerializeField]
		private UISprite _offlineIcon;

		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			switch (slideIn.facebookState)
			{
			case FacebookSlideInState.ConnectSucceeded:
				_onlineIcon.enabled = true;
				_offlineIcon.enabled = false;
				_statusLabel.text = Strings.Get("FACEBOOK_ONLINE_SLIDEIN");
				break;
			case FacebookSlideInState.ConnectFailed:
				_onlineIcon.enabled = false;
				_offlineIcon.enabled = true;
				_statusLabel.text = Strings.Get("FACEBOOK_OFFLINE_SLIDEIN");
				break;
			case FacebookSlideInState.LoggedOut:
				_onlineIcon.enabled = true;
				_offlineIcon.enabled = false;
				_statusLabel.text = Strings.Get("FACEBOOK_LOGOUT_SLIDEIN");
				break;
			}
			_layout.Align();
			SlideIn(slideIn.onDidShowCallback);
		}
	}
}
