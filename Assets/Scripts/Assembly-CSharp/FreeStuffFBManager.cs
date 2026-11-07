using System;
using Info;
using Kiloo.Common.Extensions;
using UI.Screens;
using UI.SlideIns;

public class FreeStuffFBManager
{
	private static bool _facebookLoginRunning;

	private static FreeStuffFBManager _instance;

	public Action setButtonToActiveState;

	public Action setButtonToLoadingState;

	public static FreeStuffFBManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FreeStuffFBManager();
			}
			return _instance;
		}
	}

	public void FacebookLoginClicked()
	{
		setButtonToLoadingState.SafeInvoke();
		HouseKeeper.SetFacebookPayoutAction(FacebookStatusChanged);
		_facebookLoginRunning = true;
		if (SocialManager.instance.FacebookLogin(OnFacebookLoginResult))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
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
			FreeStuffScreen.updateScreen = true;
		}
		else
		{
			if (_facebookLoginRunning)
			{
				OnFacebookLoginResult(false);
			}
			setButtonToActiveState.SafeInvoke();
		}
		HouseKeeper.SetFacebookPayoutActionToDefault();
		_facebookLoginRunning = false;
	}

	private void OnFacebookLoginResult(bool loggedIn)
	{
		if (!loggedIn && _facebookLoginRunning)
		{
			_facebookLoginRunning = false;
			QueueFacebookFailSlideIn(OnFailSlideInFinish);
		}
	}

	private static void QueueFacebookFailSlideIn(Action onFinishCallback)
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
}
