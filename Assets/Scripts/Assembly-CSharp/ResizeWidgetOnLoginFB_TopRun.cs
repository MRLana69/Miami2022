using UnityEngine;

public class ResizeWidgetOnLoginFB_TopRun : MonoBehaviour
{
	[SerializeField]
	public UIWidget emptyWidget;

	public void SetEmptyWidgetSize(int width, int height)
	{
		emptyWidget.width = width;
		emptyWidget.height = height;
	}
}
