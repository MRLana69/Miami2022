using System.Collections;
using JetBrains.Annotations;
using Kiloo.GameCenter;
using UnityEngine;

public class GameOverOfflineHelper : MonoBehaviour
{
	public GameObject FacebookLoginButton;

	public GameObject GameCenterLoginButton;

	public UISprite FacebookIcon;

	public UISprite GameCenterIcon;

	[UsedImplicitly]
	private void Start()
	{
		if (!GameCenterPlugin.isSupported && GameCenterLoginButton != null && FacebookLoginButton != null)
		{
			GameCenterLoginButton.SetActive(false);
			Object.Destroy(GameCenterLoginButton);
			Vector3 localPosition = FacebookLoginButton.transform.localPosition;
			localPosition.x = 0f;
			FacebookLoginButton.transform.localPosition = localPosition;
		}
	}

	public void EnableButtons()
	{
	}

	public void DisableButtons()
	{
	}

	private IEnumerator AnimateAlpha(UISprite sprite1, UISprite sprite2, float duration, float toAlpha)
	{
		float fromAlpha = sprite1.alpha;
		float factor = 0f;
		while (factor < 1f)
		{
			factor += Time.deltaTime / duration;
			factor = Mathf.Clamp01(factor);
			sprite1.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			sprite2.alpha = Mathf.Lerp(fromAlpha, toAlpha, factor);
			yield return null;
		}
		sprite1.alpha = toAlpha;
		sprite2.alpha = toAlpha;
		HandleColliders(true);
	}

	private void HandleColliders(bool AddNow)
	{
		if (AddNow)
		{
			NGUITools.AddWidgetCollider(FacebookLoginButton);
		}
		else if (FacebookLoginButton.GetComponent<Collider>() != null)
		{
			Object.Destroy(FacebookLoginButton.GetComponent<Collider>());
		}
	}
}
