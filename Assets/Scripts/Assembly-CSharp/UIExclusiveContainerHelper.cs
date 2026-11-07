using UnityEngine;

public class UIExclusiveContainerHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel messageLabel;

	[SerializeField]
	private UISprite messageBackground;

	public void SetupExclusiveMessage(string message, Color backgroundColor, Color textColor)
	{
		base.gameObject.SetActive(true);
		messageLabel.text = message;
		messageBackground.color = backgroundColor;
		messageLabel.color = textColor;
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
	}

	public void Enable()
	{
		base.gameObject.SetActive(true);
	}
}
