using UnityEngine;
using UnityEngine.Events;

namespace SYBO.Components.Logging
{
	public class GestureUnlock : MonoBehaviour
	{
		public Vector2[] Gesture;

		public float InchWidth;

		public int Position = -1;

		public Behaviour Target;

		public UnityEvent OnUnlock;

		private void Update()
		{
			Vector2 mouseScreenPosition = GetMouseScreenPosition();
			if (Position == -1)
			{
				if (Input.GetMouseButtonDown(0) && AtPosition(mouseScreenPosition, Gesture[0]))
				{
					Position = 0;
				}
			}
			else if (AtPosition(mouseScreenPosition, Gesture[Position + 1]))
			{
				Position++;
				if (Position == Gesture.Length - 1)
				{
					OnUnlock.Invoke();
					if (Target != null)
					{
						Target.enabled = true;
					}
					Position = -1;
					base.enabled = false;
				}
			}
			else if (!DistanceGreen(mouseScreenPosition, Gesture[Position], Gesture[Position + 1]))
			{
				Position = -1;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				Position = -1;
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(new Vector3(0f, 0f, 0f), new Vector3(Screen.width, 0f, 0f));
			Gizmos.DrawLine(new Vector3(0f, 0f, 0f), new Vector3(0f, Screen.height, 0f));
			Gizmos.DrawLine(new Vector3(Screen.width, Screen.height, 0f), new Vector3(Screen.width, 0f, 0f));
			Gizmos.DrawLine(new Vector3(Screen.width, Screen.height, 0f), new Vector3(0f, Screen.height, 0f));
			for (int i = 0; i < Gesture.Length - 1; i++)
			{
				Gizmos.DrawLine(new Vector3(Gesture[i].x * (float)Screen.width, Gesture[i].y * (float)Screen.height), new Vector3(Gesture[i + 1].x * (float)Screen.width, Gesture[i + 1].y * (float)Screen.height));
			}
			for (int j = 0; j < Gesture.Length; j++)
			{
				Gizmos.DrawWireSphere(new Vector3(Gesture[j].x * (float)Screen.width, Gesture[j].y * (float)Screen.height), InchWidth * Screen.dpi);
			}
		}

		private Vector2 GetMouseScreenPosition()
		{
			Vector3 mousePosition = Input.mousePosition;
			return new Vector2(mousePosition.x, mousePosition.y);
		}

		private bool AtPosition(Vector2 input, Vector2 point)
		{
			point.Scale(new Vector2(Screen.width, Screen.height));
			return (input - point).magnitude / Screen.dpi < InchWidth;
		}

		private bool DistanceGreen(Vector2 input, Vector2 point1, Vector2 point2)
		{
			point1.Scale(new Vector2(Screen.width, Screen.height));
			point2.Scale(new Vector2(Screen.width, Screen.height));
			float num = DistancePointToLineSegment(input, point1, point2);
			return num / Screen.dpi < InchWidth;
		}

		private static float DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
		{
			float sqrMagnitude = (b - a).sqrMagnitude;
			if (sqrMagnitude == 0f)
			{
				return (point - a).magnitude;
			}
			float num = Vector2.Dot(point - a, b - a) / sqrMagnitude;
			if (num < 0f)
			{
				return (point - a).magnitude;
			}
			if (num > 1f)
			{
				return (point - b).magnitude;
			}
			Vector2 vector = a + num * (b - a);
			return (point - vector).magnitude;
		}
	}
}
