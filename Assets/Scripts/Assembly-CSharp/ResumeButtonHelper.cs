using System.Collections;
using UnityEngine;

public class ResumeButtonHelper : MonoBehaviour
{
	private const float MIN_WAIT_TIME = 1f;

	private const float MAX_WAIT_TIME = 5f;

	[SerializeField]
	private UILabel label;

	private UIButtonOverlayOff _cachedOverlayHelper;

	private bool _buttonEnabled = true;

	private UIButtonOverlayOff overlayHelper
	{
		get
		{
			return _cachedOverlayHelper ?? (_cachedOverlayHelper = base.gameObject.GetComponent<UIButtonOverlayOff>());
		}
	}

	public bool isButtonEnabled
	{
		get
		{
			return _buttonEnabled;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		DisableButton();
		if (!pause)
		{
			StartCoroutine(EnableButtonWhenReady());
		}
	}

	private IEnumerator EnableButtonWhenReady()
	{
		float startTime = Time.realtimeSinceStartup;
		float timeWaited = 0f;
		while (timeWaited < 1f)
		{
			timeWaited = Time.realtimeSinceStartup - startTime;
			yield return new WaitForEndOfFrame();
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
		{
			while (timeWaited < 5f && !SocialManager.instance.consolidatedFriendsCompleted)
			{
				timeWaited = Time.realtimeSinceStartup - startTime;
				yield return new WaitForEndOfFrame();
			}
		}
		EnableButton();
	}

	public void EnableButton()
	{
		if (!_buttonEnabled)
		{
			TweenColor.Begin(overlayHelper.fillWithPrimeColor, 0f, GlobalColors.FOOTER_BIG_PLAY_RESUME_COLOR);
			overlayHelper.GoToNormalState();
			_buttonEnabled = true;
			label.text = Strings.Get("PAUSE_UI_RESUME");
		}
	}

	public void DisableButton()
	{
		if (_buttonEnabled)
		{
			TweenColor.Begin(overlayHelper.fillWithPrimeColor, 0f, GlobalColors.FOOTER_BIG_WAIT_COLOR);
			overlayHelper.GoToNotAvailableState();
			_buttonEnabled = false;
			label.text = Strings.Get("RESUME_BUTTON_HELPER_WAIT");
		}
	}
}
