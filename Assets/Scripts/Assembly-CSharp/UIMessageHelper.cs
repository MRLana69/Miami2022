using System.Collections;
using UnityEngine;

public class UIMessageHelper : MonoBehaviour
{
	public Color shownColor = Color.white;

	private UILabel _label;

	private bool _wasPermanentlyHidden;

	private void Awake()
	{
		_label = GetComponent<UILabel>();
		_label.alpha = 0f;
		base.gameObject.SetActive(false);
	}

	public void ShowMessage(string message, bool bigText = false)
	{
		_wasPermanentlyHidden = false;
		base.gameObject.SetActive(true);
		_label.enabled = true;
		_label.text = message;
		_label.color = shownColor;
		if (bigText)
		{
			_label.fontSize = 30;
		}
		else
		{
			_label.fontSize = 20;
		}
		StartCoroutine("FadeOut");
	}

	private IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(2f);
		float duration = 0.2f;
		float fadeTime = 0f;
		Vector3 scaleFrom = _label.transform.localScale;
		Vector3 scaleTo = new Vector3(scaleFrom.x, 0f, scaleFrom.z);
		while (fadeTime < 1f)
		{
			fadeTime += Time.deltaTime / duration;
			_label.transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, fadeTime);
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		_label.text = string.Empty;
		_label.transform.localScale = scaleFrom;
		base.gameObject.SetActive(false);
		UIScreenController.Instance.ReadyForNextMessage();
	}

	public void SetTemporaryHidden(bool hidden)
	{
		_label.enabled = !hidden && !_wasPermanentlyHidden;
	}

	public void DisableShowLabel()
	{
		_wasPermanentlyHidden = _label.text != string.Empty;
		_label.enabled = false;
	}
}
