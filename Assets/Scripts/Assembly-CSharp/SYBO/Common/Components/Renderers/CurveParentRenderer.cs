using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Common.Components.Renderers
{
	public class CurveParentRenderer : MonoBehaviour
	{
		[SerializeField]
		private CurveParent _curveParent;

		[SerializeField]
		private GameObject _curveNode;

		protected void Start()
		{
			if (_curveNode != null)
			{
				float maxTime = _curveParent.MaxTime;
				float minTime = _curveParent.MinTime;
				float num = 0.0125f;
				GameObject gameObject = new GameObject("CurveNodes");
				HierarchyUtility.Reparent(gameObject.transform, base.transform);
				for (float num2 = minTime; num2 <= maxTime; num2 += num)
				{
					Vector3 worldPositionForTime = _curveParent.GetWorldPositionForTime(num2);
					Quaternion rotationForTime = _curveParent.GetRotationForTime(num2, num);
					GameObject gameObject2 = Object.Instantiate(_curveNode, worldPositionForTime, rotationForTime);
					HierarchyUtility.Reparent(gameObject2.transform, gameObject.transform);
				}
			}
		}
	}
}
