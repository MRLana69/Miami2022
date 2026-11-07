using Kiloo.Common.Timer;
using UnityEngine;

namespace Kiloo
{
	public static class WidgetUtils
	{
		public static GameObject PlaceModelUnderAnchor(GameObject model, Transform anchor, int layer)
		{
			NGUITools.SetLayer(model, layer);
			model.transform.parent = anchor;
			model.transform.localPosition = Vector3.zero;
			model.transform.localEulerAngles = Vector3.zero;
			model.transform.localScale = Vector3.one;
			return model;
		}

		public static void PositionModelOnSprite(UIWidget tracker, GameObject objectToPosition, Vector3 positionOffset, Vector3 newRotation, Vector3 newScale)
		{
			Timer.ExecuteNextFrame(() =>
			{
				if (!(objectToPosition == null) && !(tracker == null))
				{
					Vector3 position = tracker.transform.position;
					Vector3 vector = UIScreenController.Instance.Camera2DNormal.WorldToScreenPoint(position);
					vector = new Vector3(vector.x, vector.y, 1f);
					Vector3 position2 = UIScreenController.Instance.Camera3D.ScreenToWorldPoint(vector);
					objectToPosition.transform.position = position2;
					objectToPosition.transform.localPosition = objectToPosition.transform.localPosition + positionOffset;
					objectToPosition.transform.localRotation = Quaternion.Euler(newRotation);
					objectToPosition.transform.localScale = newScale;
				}
			});
		}
	}
}
