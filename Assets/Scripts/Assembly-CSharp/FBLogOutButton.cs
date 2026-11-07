using JetBrains.Annotations;
using TopRun;
using UI.Popups;
using UI.SlideIns;
using UnityEngine;

public class FBLogOutButton : MonoBehaviour
{
	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private GameObject _overlay;

	[SerializeField]
	private UISprite _fill;

	[SerializeField]
	private SettingsPopup settingsPopup;

	[UsedImplicitly]
	private void OnClick()
	{
		SocialManager.instance.FacebookLogout();
		UIScreenController.Instance.QueueSlideIn(new SlideIn
		{
			type = SlideInData.SlideInType.Facebook,
			facebookState = UISlideInFacebook.FacebookSlideInState.LoggedOut
		});
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			manager.RefreshTournament();
		}
		settingsPopup.LoggedOutOfFacebook();
	}

	private void Enable()
	{
		_description.text = Strings.Get("SETTINGS_POPUP_FB_LOG_OUT_DESC");
		_title.text = Strings.Get("SETTINGS_POPUP_FB_TITLE");
		_overlay.SetActive(true);
		_fill.color = GlobalColors.TERTIARY_ACTION_COLOR;
		GetComponent<Collider>().enabled = true;
	}

	private void OnEnable()
	{
		Enable();
	}
}
