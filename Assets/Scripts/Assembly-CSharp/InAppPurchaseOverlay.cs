using UnityEngine;

public class InAppPurchaseOverlay : MonoBehaviour
{
	[SerializeField]
	private UILabel titleLabel;

	public void UpdateLabel(string text)
	{
		if (titleLabel != null)
		{
			titleLabel.text = text;
		}
	}
}
