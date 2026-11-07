using UnityEngine;
using Utilities;

public class VideoAdsOverlayCloseButton : MonoBehaviour
{
	private void Start()
	{
		if (SafeArea.IsRestricted)
		{
			UIWidget component = base.gameObject.GetComponent<UIWidget>();
			if (component != null)
			{
				component.topAnchor.absolute -= SafeArea.OFFSET_TOP;
				component.bottomAnchor.absolute -= SafeArea.OFFSET_TOP;
			}
			else
			{
				SDebug.LogError("Error getting close button on VideoAdsOverlayCloseButton!");
			}
		}
	}
}
