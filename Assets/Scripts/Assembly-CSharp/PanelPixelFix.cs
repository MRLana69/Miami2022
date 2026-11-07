using UnityEngine;

public class PanelPixelFix : MonoBehaviour
{
	[SerializeField]
	private UIPanel panel;

	private void LateUpdate()
	{
		if (panel != null)
		{
			Vector3 localPosition = panel.cachedTransform.localPosition;
			localPosition = new Vector3(Mathf.Round(localPosition.x), Mathf.Round(localPosition.y), localPosition.z);
			panel.cachedTransform.localPosition = localPosition;
			Vector4 baseClipRegion = panel.baseClipRegion;
			baseClipRegion = new Vector4(Mathf.Round(baseClipRegion.x), Mathf.Round(baseClipRegion.y), Mathf.Round(baseClipRegion.z), Mathf.Round(baseClipRegion.w));
			panel.baseClipRegion = baseClipRegion;
		}
	}
}
