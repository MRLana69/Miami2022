using UnityEngine;

public class FacebookLoginButtonOffline : MonoBehaviour
{
	private void OnClick()
	{
		if (SocialManager.instance.FacebookLogin(null))
		{
			UIScreenController.Instance.FacebookLogIn(true);
		}
	}
}
