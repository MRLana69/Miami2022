using UnityEngine;

public class CenterLabelAndSpriteUnit : MonoBehaviour
{
	public void CenterUnit(float aditionalOffset = 0f)
	{
		Vector3 zero = Vector3.zero;
		base.transform.localPosition = zero;
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform);
		zero.x = zero.x - bounds.extents.x / 2f + aditionalOffset;
		base.transform.localPosition = zero;
	}
}
