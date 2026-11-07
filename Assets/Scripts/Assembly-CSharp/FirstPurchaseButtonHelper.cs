using JetBrains.Annotations;
using UnityEngine;

public class FirstPurchaseButtonHelper : MonoBehaviour
{
	[UsedImplicitly]
	public void FirstPurchaseButtonClicked()
	{
		UIScreenController.Instance.QueuePopup("FirstPurchaseBonusPopup");
	}
}
