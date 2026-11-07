using UnityEngine;

public class BundleContentSlot : MonoBehaviour
{
	[SerializeField]
	private UILabel amountField;

	public void UpdateAmount(string amount)
	{
		amountField.text = amount;
	}
}
