using JetBrains.Annotations;
using UnityEngine;

public class BoardPromotionButtonHelper : MonoBehaviour
{
	[UsedImplicitly]
	public void BoardPromotionButtonClicked()
	{
		UIScreenController.Instance.QueuePopup("BoardPromotionPopup");
	}
}
