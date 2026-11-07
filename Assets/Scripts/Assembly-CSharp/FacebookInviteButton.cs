using UnityEngine;

public class FacebookInviteButton : MonoBehaviour
{
	[SerializeField]
	private UILabel _recommendLabel;

	private void OnEnable()
	{
		_recommendLabel.text = Strings.Get("FACEBOOK_RECOMMEND_BUTTON_TEXT");
	}

	private void OnClick()
	{
		if (SocialManager.instance.facebookIsLoggedIn)
		{
			SocialManager.instance.RecommendAppFacebook();
		}
		else
		{
			SocialManager.instance.FacebookLogin(UIScreenController.Instance.FacebookLogIn);
		}
	}
}
