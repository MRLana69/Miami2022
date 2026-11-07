using NaughtyAttributes;
using UnityEngine;

namespace UI.Generic
{
	[RequireComponent(typeof(UIWidget))]
	public class WidgetSizer : MonoBehaviour
	{
		[Button(null)]
		private void ResizeNow()
		{
			Transform transform = base.transform;
			UIWidget component = GetComponent<UIWidget>();
			UIWidget[] componentsInChildren = transform.GetComponentsInChildren<UIWidget>();
			if (componentsInChildren.Length <= 0)
			{
				return;
			}
			Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
			Bounds bounds = default(Bounds);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] == component)
				{
					continue;
				}
				Vector3[] worldCorners = componentsInChildren[i].worldCorners;
				for (int j = 0; j < 4; j++)
				{
					worldCorners[j] = worldToLocalMatrix.MultiplyPoint3x4(worldCorners[j]);
					if (i == 0 && j == 0)
					{
						bounds = new Bounds(worldCorners[j], Vector3.zero);
					}
					else
					{
						bounds.Encapsulate(worldCorners[j]);
					}
				}
			}
			component.SetDimensions((int)bounds.size.x, (int)bounds.size.y);
		}
	}
}
