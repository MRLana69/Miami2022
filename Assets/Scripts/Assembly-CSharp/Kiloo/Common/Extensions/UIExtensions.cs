using System;
using System.Collections.Generic;
using System.Linq;
using UI.Generic.Layout;
using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class UIExtensions
	{
		public static void DrawBounds(this Bounds b, Color col)
		{
			Vector3 center = b.center;
			Vector3 vector = b.center - b.extents;
			Vector3 vector2 = b.center + b.extents;
			Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector2.x, vector.y, center.z), col);
			Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector.x, vector2.y, center.z), col);
			Debug.DrawLine(new Vector3(vector2.x, vector.y, center.z), new Vector3(vector2.x, vector2.y, center.z), col);
			Debug.DrawLine(new Vector3(vector.x, vector2.y, center.z), new Vector3(vector2.x, vector2.y, center.z), col);
		}

		public static Vector4 GetEndPositions(this Transform transform, Renderer[] renderers = null)
		{
			renderers = renderers ?? transform.gameObject.GetComponentsInChildren<Renderer>();
			float num = float.MinValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MaxValue;
			Renderer[] array = renderers;
			foreach (Renderer renderer in array)
			{
				Bounds bounds = renderer.bounds;
				float num5 = bounds.center.x + bounds.extents.x;
				float num6 = bounds.center.x - bounds.extents.x;
				float num7 = bounds.center.y + bounds.extents.y;
				float num8 = bounds.center.y - bounds.extents.y;
				if (num5 > num)
				{
					num = num5;
				}
				if (num6 < num2)
				{
					num2 = num6;
				}
				if (num7 > num3)
				{
					num3 = num7;
				}
				if (num8 < num4)
				{
					num4 = num8;
				}
			}
			return new Vector4(num2, num4, num, num3);
		}

		public static void PositionOnWidget(this Transform t, UIWidget w, Camera modelCamera = null, Camera widgetCamera = null, UIWidget.Pivot pivot = UIWidget.Pivot.Center)
		{
			Vector3 position = Vector3.zero;
			Vector3[] worldCorners = w.worldCorners;
			Vector3[] sides = w.GetSides(null);
			switch (pivot)
			{
			case UIWidget.Pivot.TopLeft:
				position = worldCorners[1];
				break;
			case UIWidget.Pivot.Top:
				position = sides[1];
				break;
			case UIWidget.Pivot.TopRight:
				position = worldCorners[2];
				break;
			case UIWidget.Pivot.Left:
				position = sides[0];
				break;
			case UIWidget.Pivot.Center:
				position = w.worldCenter;
				break;
			case UIWidget.Pivot.Right:
				position = sides[2];
				break;
			case UIWidget.Pivot.BottomLeft:
				position = worldCorners[1];
				break;
			case UIWidget.Pivot.Bottom:
				position = sides[3];
				break;
			case UIWidget.Pivot.BottomRight:
				position = worldCorners[3];
				break;
			}
			if (widgetCamera == null)
			{
				widgetCamera = NGUITools.FindCameraForLayer(w.gameObject.layer);
			}
			if (modelCamera == null)
			{
				modelCamera = NGUITools.FindCameraForLayer(t.gameObject.layer);
			}
			position = widgetCamera.WorldToScreenPoint(position);
			position.z = Mathf.Abs(widgetCamera.transform.position.z - modelCamera.transform.position.z);
			position.z += t.position.z;
			position = modelCamera.ScreenToWorldPoint(position);
			t.position = position;
		}

		public static void SimpleFitToWidget(this Transform transform, UIWidget widget, bool fitHorizontal = true)
		{
			MeshFilter[] componentsInChildren = transform.gameObject.GetComponentsInChildren<MeshFilter>();
			float num = float.MaxValue;
			float num2 = float.MinValue;
			Vector3 position = widget.transform.position;
			Vector3 position2 = transform.position;
			widget.cachedTransform.position = Vector3.zero;
			transform.position = new Vector3(0f, 0f, transform.position.z);
			MeshFilter[] array = componentsInChildren;
			foreach (MeshFilter meshFilter in array)
			{
				Mesh mesh = meshFilter.mesh;
				Vector3[] vertices = mesh.vertices;
				foreach (Vector3 position3 in vertices)
				{
					Vector3 vector = meshFilter.transform.TransformPoint(position3);
					float num3 = ((!fitHorizontal) ? vector.y : vector.x);
					if (num3 < num)
					{
						num = num3;
					}
					else if (num3 > num2)
					{
						num2 = num3;
					}
				}
			}
			float num4;
			float num5;
			if (fitHorizontal)
			{
				num4 = widget.worldCorners[0].x;
				num5 = widget.worldCorners[2].x;
			}
			else
			{
				num4 = widget.worldCorners[0].y;
				num5 = widget.worldCorners[1].y;
			}
			Camera camera = NGUITools.FindCameraForLayer(widget.cachedGameObject.layer);
			Camera camera2 = NGUITools.FindCameraForLayer(transform.gameObject.layer);
			float z = Mathf.Abs(transform.position.z - camera2.transform.position.z);
			float num6;
			float num7;
			float num8;
			float num9;
			if (fitHorizontal)
			{
				num6 = camera.WorldToScreenPoint(new Vector3(num4, widget.cachedTransform.position.y)).x;
				num7 = camera.WorldToScreenPoint(new Vector3(num5, widget.cachedTransform.position.y)).x;
				num8 = camera2.WorldToScreenPoint(new Vector3(num, transform.position.y, z)).x;
				num9 = camera2.WorldToScreenPoint(new Vector3(num2, transform.position.y, z)).x;
			}
			else
			{
				num6 = camera.WorldToScreenPoint(new Vector3(widget.cachedTransform.position.x, num4)).y;
				num7 = camera.WorldToScreenPoint(new Vector3(widget.cachedTransform.position.x, num5)).y;
				num8 = camera2.WorldToScreenPoint(new Vector3(transform.position.x, num, z)).y;
				num9 = camera2.WorldToScreenPoint(new Vector3(transform.position.x, num2, z)).y;
			}
			float num10 = Mathf.Abs(num7 - num6);
			float num11 = Mathf.Abs(num8 - num9);
			transform.localScale *= num10 / num11 / 2f;
			transform.position = position2;
			widget.cachedTransform.position = position;
		}

		public static Bounds CalculateMeshBounds(this Transform t)
		{
			MeshFilter[] componentsInChildren = t.gameObject.GetComponentsInChildren<MeshFilter>(true);
			Quaternion rotation = t.rotation;
			t.rotation = Quaternion.Euler(0f, 0f, 0f);
			Bounds bounds = default(Bounds);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				MeshFilter meshFilter = componentsInChildren[i];
				Mesh mesh = meshFilter.mesh;
				Vector3 position = meshFilter.gameObject.transform.position;
				Vector3 lossyScale = meshFilter.gameObject.transform.lossyScale;
				Quaternion rotation2 = meshFilter.gameObject.transform.rotation;
				int vertexCount = mesh.vertexCount;
				Vector3[] vertices = mesh.vertices;
				Vector3 vector = Vector3.zero;
				Vector3 point = Vector3.zero;
				for (int j = 0; j < vertexCount; j++)
				{
					if (i == 0 && j == 0)
					{
						vector = position + rotation2 * Vector3.Scale(lossyScale, vertices[j]);
						point = vector;
						bounds = new Bounds(vector, Vector3.zero);
						continue;
					}
					Vector3 vector2 = position + rotation2 * Vector3.Scale(lossyScale, vertices[j]);
					vector.x = ((!(vector.x > vector2.x)) ? vector.x : vector2.x);
					vector.y = ((!(vector.y > vector2.y)) ? vector.y : vector2.y);
					vector.z = ((!(vector.z > vector2.z)) ? vector.z : vector2.z);
					point.x = ((!(point.x < vector2.x)) ? point.x : vector2.x);
					point.y = ((!(point.y < vector2.y)) ? point.y : vector2.y);
					point.z = ((!(point.z < vector2.z)) ? point.z : vector2.z);
				}
				bounds.Encapsulate(vector);
				bounds.Encapsulate(point);
			}
			t.rotation = rotation;
			Vector3 vector3 = t.position + rotation * (bounds.center - t.position);
			Vector3 vector4 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(1f, 1f, 1f));
			Vector3 vector5 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(-1f, 1f, 1f));
			Vector3 vector6 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(-1f, 1f, -1f));
			Vector3 vector7 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(1f, 1f, -1f));
			Vector3 vector8 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(1f, -1f, 1f));
			Vector3 vector9 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(-1f, -1f, 1f));
			Vector3 vector10 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(-1f, -1f, -1f));
			Vector3 vector11 = vector3 + rotation * Vector3.Scale(bounds.extents, new Vector3(1f, -1f, -1f));
			Vector3[] array = new Vector3[8] { vector4, vector5, vector6, vector7, vector8, vector9, vector10, vector11 };
			bounds = new Bounds(vector4, Vector3.zero);
			Vector3[] array2 = array;
			foreach (Vector3 point2 in array2)
			{
				bounds.Encapsulate(point2);
			}
			return bounds;
		}

		public static Bounds CalculateRendererBounds(this Transform t)
		{
			Bounds result = new Bounds(t.position, Vector3.zero);
			Renderer[] componentsInChildren = t.gameObject.GetComponentsInChildren<Renderer>(true);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				result.Encapsulate(renderer.bounds);
			}
			return result;
		}

		public static void CenterAlignOnZero(this UIGrid grid)
		{
			int num = 0;
			for (int i = 0; i < grid.transform.childCount; i++)
			{
				if (grid.transform.GetChild(i).gameObject.activeSelf || !grid.hideInactive)
				{
					num++;
				}
			}
			float num2 = (float)(num - 1) * grid.cellWidth;
			Vector3 localPosition = grid.transform.localPosition;
			localPosition.x = num2 / -2f;
			grid.transform.localPosition = localPosition;
		}

		public static void CenterAlignOnZero(this UITable table)
		{
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < table.transform.childCount; i++)
			{
				Transform child = table.transform.GetChild(i);
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(child, !table.hideInactive);
				if (child.gameObject.activeSelf || !table.hideInactive)
				{
					num2 += bounds.extents.x * 2f;
					num3 = Mathf.Max(num3, bounds.extents.y * 2f);
					num++;
				}
			}
			num2 += (float)(num - 1) * table.padding.x * 2f;
			Vector3 localPosition = table.transform.localPosition;
			localPosition.x = num2 / -2f - table.padding.x;
			localPosition.y = num3 / 2f;
			table.transform.localPosition = localPosition;
		}

		public static float CenterChildrenHorizontally(this Transform t, float padding)
		{
			Transform[] array = new Transform[t.childCount];
			for (int i = 0; i < t.childCount; i++)
			{
				array[i] = t.GetChild(i);
			}
			return CenterChildrenHorizontally(array, padding, t);
		}

		public static float CenterChildrenHorizontally(Transform[] children, float padding, Transform parent)
		{
			float num = 0f;
			Bounds[] array = new Bounds[children.Length];
			List<int> list = new List<int>(children.Length);
			for (int i = 0; i < children.Length; i++)
			{
				Transform transform = children[i];
				if (transform.gameObject.activeSelf && transform.GetComponent<IgnoreLayout>() == null)
				{
					Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(parent, transform, false);
					array[i] = bounds;
					num += bounds.extents.x * 2f;
					list.Add(i);
				}
			}
			num += (float)(list.Count - 1) * padding;
			float num2 = num / -2f;
			for (int j = 0; j < list.Count; j++)
			{
				int num3 = list[j];
				Transform transform2 = children[num3];
				Bounds bounds2 = array[num3];
				Vector3 localPosition = transform2.localPosition;
				localPosition.x = num2 + (localPosition.x - bounds2.center.x) + bounds2.extents.x;
				transform2.localPosition = localPosition;
				num2 += bounds2.extents.x * 2f;
				num2 += padding;
			}
			return num;
		}

		public static float CenterTransformsHorizontalFixedWidth(Transform[] transforms, float width, float padding)
		{
			float num = 0f;
			List<int> list = new List<int>(transforms.Length);
			for (int i = 0; i < transforms.Length; i++)
			{
				Transform transform = transforms[i];
				if (transform.gameObject.activeSelf)
				{
					num += width;
					list.Add(i);
				}
			}
			num += (float)(list.Count - 1) * padding;
			float num2 = num / -2f + width / 2f;
			for (int j = 0; j < list.Count; j++)
			{
				int num3 = list[j];
				Transform transform2 = transforms[num3];
				Vector3 localPosition = transform2.localPosition;
				localPosition.x = num2;
				transform2.localPosition = localPosition;
				num2 += width;
				num2 += padding;
			}
			return num;
		}

		public static float AlignChildrenHorizontally(this Transform t, float padding, bool leftAligned)
		{
			Transform[] array = new Transform[t.childCount];
			for (int i = 0; i < t.childCount; i++)
			{
				array[i] = t.GetChild(i);
			}
			return AlignChildrenHorizontally(array, padding, t, leftAligned);
		}

		private static float AlignChildrenHorizontally(Transform[] children, float padding, Transform parent, bool leftAligned)
		{
			float num = 0f;
			if (leftAligned)
			{
				foreach (Transform transform in children)
				{
					if (transform.gameObject.activeSelf)
					{
						Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(parent, transform, false);
						Vector3 localPosition = transform.localPosition;
						localPosition.x = num + (localPosition.x - bounds.center.x) + bounds.extents.x;
						transform.localPosition = localPosition;
						num += bounds.extents.x * 2f;
						num += padding;
					}
				}
			}
			else
			{
				for (int num2 = children.Length - 1; num2 >= 0; num2--)
				{
					Transform transform2 = children[num2];
					if (transform2.gameObject.activeSelf)
					{
						Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(parent, transform2, false);
						Vector3 localPosition2 = transform2.localPosition;
						localPosition2.x = num - (bounds2.extents.x + bounds2.center.x) + localPosition2.x;
						transform2.localPosition = localPosition2;
						num -= bounds2.extents.x * 2f;
						num -= padding;
					}
				}
			}
			num += ((!leftAligned) ? padding : (0f - padding));
			return Mathf.Abs(num);
		}

		public static float CenterChildrenVertically(this Transform t, float padding)
		{
			Transform[] array = new Transform[t.childCount];
			for (int i = 0; i < t.childCount; i++)
			{
				array[i] = t.GetChild(i);
			}
			return CenterChildrenVertically(array, padding, t);
		}

		private static float CenterChildrenVertically(Transform[] children, float padding, Transform parent)
		{
			float num = 0f;
			Bounds[] array = new Bounds[children.Length];
			List<int> list = new List<int>(children.Length);
			for (int i = 0; i < children.Length; i++)
			{
				Transform transform = children[i];
				if (transform.gameObject.activeSelf)
				{
					Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(parent, transform, false);
					array[i] = bounds;
					num += bounds.extents.y * 2f;
					list.Add(i);
				}
			}
			num += (float)(list.Count - 1) * padding;
			float num2 = num / 2f;
			for (int j = 0; j < list.Count; j++)
			{
				int num3 = list[j];
				Transform transform2 = children[num3];
				Bounds bounds2 = array[num3];
				Vector3 localPosition = transform2.localPosition;
				localPosition.y = num2 - (bounds2.center.y - localPosition.y) - bounds2.extents.y;
				transform2.localPosition = localPosition;
				num2 -= bounds2.extents.y * 2f;
				num2 -= padding;
			}
			return num;
		}

		public static float AlignChildrenVertically(this Transform t, float padding, bool downAlign)
		{
			Transform[] array = new Transform[t.childCount];
			for (int i = 0; i < t.childCount; i++)
			{
				array[i] = t.GetChild(i);
			}
			return AlignChildrenVertically(array, padding, t, downAlign);
		}

		private static float AlignChildrenVertically(Transform[] children, float padding, Transform parent, bool downAlign)
		{
			float num = 0f;
			float num2 = ((!downAlign) ? (-1f) : 1f);
			float num3 = 0f;
			int num4 = 0;
			if (!downAlign)
			{
				children = children.Reverse().ToArray();
			}
			foreach (Transform transform in children)
			{
				if (transform.gameObject.activeSelf)
				{
					Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(parent, transform, false);
					if (num4 == 0 && !downAlign)
					{
						num3 = bounds.extents.y * 2f * num2;
						num -= num3;
					}
					Vector3 localPosition = transform.localPosition;
					localPosition.y = num - (bounds.center.y - localPosition.y) - bounds.extents.y;
					transform.localPosition = localPosition;
					num -= bounds.extents.y * 2f * num2;
					num -= padding * num2;
					num4++;
				}
			}
			if (!downAlign)
			{
				num -= num3;
			}
			num -= padding * num2;
			return Mathf.Abs(num);
		}

		public static void SizeWidgetToChildren(this UIWidget w, bool recenter = false)
		{
			UIWidget[] array = (from wid in w.GetComponentsInChildren<UIWidget>()
				where wid != w && wid.GetComponentInParent<IgnoreLayout>() == null
				select wid).ToArray();
			if (array.Length <= 0)
			{
				return;
			}
			Matrix4x4 worldToLocalMatrix = w.transform.worldToLocalMatrix;
			Bounds bound = default(Bounds);
			for (int num = 0; num < array.Length; num++)
			{
				Vector3[] worldCorners = array[num].worldCorners;
				for (int num2 = 0; num2 < 4; num2++)
				{
					worldCorners[num2] = worldToLocalMatrix.MultiplyPoint3x4(worldCorners[num2]);
					if (num == 0 && num2 == 0)
					{
						bound = new Bounds(worldCorners[num2], Vector3.zero);
					}
					else
					{
						bound.Encapsulate(worldCorners[num2]);
					}
				}
			}
			w.width = bound.GetWidth();
			w.height = bound.GetHeight();
			if (!recenter)
			{
				return;
			}
			NGUIMath.MoveWidget(w, bound.center.x, bound.center.y);
			foreach (Transform item in w.transform)
			{
				UIWidget component = item.GetComponent<UIWidget>();
				if (component != null)
				{
					NGUIMath.MoveWidget(component, 0f - bound.center.x, 0f - bound.center.y);
					continue;
				}
				Vector3 localPosition = item.localPosition;
				localPosition -= bound.center;
				item.localPosition = localPosition;
			}
		}

		private static void EncapsulateViewportVertices(Vector3[] vertices, Transform transform, ref Bounds bounds, ref bool init, Camera modelCamera)
		{
			foreach (Vector3 position in vertices)
			{
				Vector3 position2 = transform.TransformPoint(position);
				Vector3 vector = modelCamera.WorldToViewportPoint(position2);
				vector.z = 0f;
				if (init)
				{
					bounds.Encapsulate(vector);
					continue;
				}
				init = true;
				bounds.SetMinMax(vector, vector);
			}
		}

		public static Bounds CalculateViewportBounds(this Transform t, Camera modelCamera)
		{
			Bounds bounds = default(Bounds);
			bool init = false;
			MeshFilter[] componentsInChildren = t.GetComponentsInChildren<MeshFilter>();
			MeshFilter[] array = componentsInChildren;
			foreach (MeshFilter meshFilter in array)
			{
				if (meshFilter.gameObject.activeSelf)
				{
					EncapsulateViewportVertices(meshFilter.sharedMesh.vertices, meshFilter.transform, ref bounds, ref init, modelCamera);
				}
			}
			SkinnedMeshRenderer[] componentsInChildren2 = t.GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array2 = componentsInChildren2;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
			{
				if (skinnedMeshRenderer.gameObject.activeSelf)
				{
					EncapsulateViewportVertices(skinnedMeshRenderer.sharedMesh.vertices, skinnedMeshRenderer.transform, ref bounds, ref init, modelCamera);
				}
			}
			return bounds;
		}

		public static void CenterInViewport(this Transform t, Camera modelCamera, Vector2 offset)
		{
			Vector3 position = modelCamera.ViewportToWorldPoint(new Vector3(0.5f + offset.x, 0.5f + offset.y, modelCamera.nearClipPlane + (modelCamera.farClipPlane - modelCamera.nearClipPlane) / 2f));
			t.position = position;
		}

		public static void CenterInViewport(this Transform t, Camera modelCamera)
		{
			t.CenterInViewport(modelCamera, Vector2.zero);
		}

		private static void EncapsulateScreenVertices(Vector3[] vertices, Transform transform, ref Bounds bounds, ref bool init, Camera modelCamera)
		{
			foreach (Vector3 position in vertices)
			{
				Vector3 position2 = transform.TransformPoint(position);
				Vector3 vector = modelCamera.WorldToScreenPoint(position2);
				vector.z = 0f;
				if (init)
				{
					bounds.Encapsulate(vector);
					continue;
				}
				init = true;
				bounds.SetMinMax(vector, vector);
			}
		}

		public static Bounds CalculateScreenBounds(this Transform t, Camera modelCamera)
		{
			Bounds bounds = default(Bounds);
			bool init = false;
			MeshFilter[] componentsInChildren = t.GetComponentsInChildren<MeshFilter>();
			MeshFilter[] array = componentsInChildren;
			foreach (MeshFilter meshFilter in array)
			{
				if (meshFilter.gameObject.activeSelf)
				{
					EncapsulateScreenVertices(meshFilter.sharedMesh.vertices, meshFilter.transform, ref bounds, ref init, modelCamera);
				}
			}
			SkinnedMeshRenderer[] componentsInChildren2 = t.GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array2 = componentsInChildren2;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
			{
				if (skinnedMeshRenderer.gameObject.activeSelf)
				{
					EncapsulateScreenVertices(skinnedMeshRenderer.sharedMesh.vertices, skinnedMeshRenderer.transform, ref bounds, ref init, modelCamera);
				}
			}
			return bounds;
		}

		public static void FitToWidget(this Transform t, UIWidget w, Camera modelCamera = null)
		{
			t.FitToWidget(w, t.CalculateMeshBounds(), modelCamera);
		}

		public static void FitToWidget(this Transform t, UIWidget w, Bounds b, Camera modelCamera = null)
		{
			if (modelCamera == null)
			{
				modelCamera = NGUITools.FindCameraForLayer(t.gameObject.layer);
			}
			Vector3 vector = modelCamera.WorldToScreenPoint(new Vector3(b.min.x, b.min.y, b.center.z));
			Vector3 vector2 = modelCamera.WorldToScreenPoint(new Vector3(b.max.x, b.max.y, b.center.z));
			float num = vector2.x - vector.x;
			float num2 = vector2.y - vector.y;
			float a = (float)w.width / num;
			float b2 = (float)w.height / num2;
			float num3 = Mathf.Min(a, b2);
			num3 /= UIRoot.GetPixelSizeAdjustment(w.cachedGameObject);
			if (Mathf.Abs(num3) > Mathf.Epsilon && Math.Abs(num3 - 1f) > 0.01f)
			{
				Vector3 localScale = t.localScale * num3;
				t.localScale = localScale;
			}
		}

		public static void FitToWidget(this Transform t, Camera modelCamera, UIWidget widget, float zoom = 1f, int passes = 2)
		{
			t.localScale = new Vector3(5000f, 5000f, 5000f);
			for (int i = 0; i < passes; i++)
			{
				Bounds bounds = t.CalculateScreenBounds(modelCamera);
				float a = (float)widget.width / UIRoot.GetPixelSizeAdjustment(widget.gameObject) / (bounds.extents.x * 2f);
				float b = (float)widget.height / UIRoot.GetPixelSizeAdjustment(widget.gameObject) / (bounds.extents.y * 2f);
				float num = Mathf.Min(a, b) * 0.95f;
				if (Mathf.Abs(num) > Mathf.Epsilon && Mathf.Abs(num - 1f) > 0.01f)
				{
					Vector3 localScale = t.localScale * Mathf.Clamp(num, 0f, 2.1474836E+09f);
					t.localScale = localScale;
				}
			}
			t.localScale *= zoom;
		}

		public static void FitHeightToWidget(this Transform t, UIWidget widget, Camera modelCamera = null, float zoom = 1f, int passes = 2)
		{
			if (modelCamera == null)
			{
				modelCamera = NGUITools.FindCameraForLayer(t.gameObject.layer);
			}
			t.localScale = new Vector3(5000f, 5000f, 5000f);
			for (int i = 0; i < passes; i++)
			{
				Bounds bounds = t.CalculateScreenBounds(modelCamera);
				float num = (float)widget.height / UIRoot.GetPixelSizeAdjustment(widget.gameObject) / (bounds.extents.y * 2f);
				float num2 = num * 0.95f;
				if (Mathf.Abs(num2) > Mathf.Epsilon && Mathf.Abs(num2 - 1f) > 0.01f)
				{
					Vector3 localScale = t.localScale * Mathf.Clamp(num2, 0f, 2.1474836E+09f);
					t.localScale = localScale;
				}
			}
			t.localScale *= zoom;
		}

		public static void OffsetToCenter(this Transform t)
		{
			Vector3 center = t.CalculateRendererBounds().center;
			t.position += t.position - center;
		}

		public static void FitWidthToWidget(this Transform t, UIWidget widget, Camera modelCamera = null, float zoom = 1f, int passes = 2)
		{
			if (modelCamera == null)
			{
				modelCamera = NGUITools.FindCameraForLayer(t.gameObject.layer);
			}
			t.localScale = new Vector3(5000f, 5000f, 5000f);
			for (int i = 0; i < passes; i++)
			{
				Bounds bounds = t.CalculateScreenBounds(modelCamera);
				float num = (float)widget.width / UIRoot.GetPixelSizeAdjustment(widget.gameObject) / (bounds.extents.x * 2f);
				float num2 = num * 0.95f;
				if (Mathf.Abs(num2) > Mathf.Epsilon && Mathf.Abs(num2 - 1f) > 0.01f)
				{
					Vector3 localScale = t.localScale * Mathf.Clamp(num2, 0f, 2.1474836E+09f);
					t.localScale = localScale;
				}
			}
			t.localScale *= zoom;
		}

		public static void CenterOnWidget(this Transform t, UIWidget widget, Camera modelCamera, Vector2 offset)
		{
			Vector3 position = widget.transform.localPosition + new Vector3(offset.x, offset.y, 0f);
			Vector3 vector = NGUITools.FindCameraForLayer(widget.gameObject.layer).WorldToScreenPoint(widget.transform.TransformPoint(position));
			Vector3 position2 = modelCamera.ScreenToWorldPoint(new Vector3(vector.x, vector.y, modelCamera.nearClipPlane + (modelCamera.farClipPlane - modelCamera.nearClipPlane) / 2f));
			t.position = position2;
		}

		public static void CenterOnWidget(this Transform t, UIWidget widget, Camera modelCamera)
		{
			t.CenterOnWidget(widget, modelCamera, Vector2.zero);
		}

		public static void CenterOnWidget(this Transform t, UIWidget widget)
		{
			t.CenterOnWidget(widget, NGUITools.FindCameraForLayer(t.gameObject.layer));
		}
	}
}
