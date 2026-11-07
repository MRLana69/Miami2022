using UnityEngine;

public class ForceCoinScreenButton : MonoBehaviour
{
	private void OnClick()
	{
		string topScreenName = UIScreenController.Instance.GetTopScreenName();
		if (!string.IsNullOrEmpty(topScreenName))
		{
			Flurry.LogEventWithAParameter("More coins button clicked", "Screen Name", topScreenName);
		}
		else
		{
			Flurry.LogEvent("More coins button clicked");
		}
		UIScreenController.Instance.ClosePopup();
		UIScreenController.Instance.QueuePopup("StorePopup");
		UIScreenController.Instance.QueuePopup("ConsumablesPopup");
	}
}
