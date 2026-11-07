using System;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.KScrollView
{
	public sealed class Container
	{
		public object controlComponent;

		public GameObject prefabInstance;

		public Action onRecycleContainer;

		public Vector2 coordinates = Vector2.zero;

		public Bounds bounds;

		public int positionIndex;

		public int modelType;

		public float height;

		public float width;

		public bool isMeasured;

		private bool _parentSet;

		public float top;

		public float bottom;

		public float right;

		public float left;

		public Transform modelTransform
		{
			get
			{
				return prefabInstance.transform;
			}
		}

		public Container(GameObject prefab)
		{
			prefabInstance = UnityEngine.Object.Instantiate(prefab);
		}

		public void Measure(bool includeInactive = true)
		{
			if (!isMeasured)
			{
				bounds = NGUIMath.CalculateRelativeWidgetBounds(prefabInstance.transform, includeInactive);
				width = bounds.size.x;
				height = bounds.size.y;
				isMeasured = true;
			}
		}

		public void SetPosition()
		{
			modelTransform.localPosition = coordinates;
		}

		public void SetParent(Transform parent)
		{
			if (!_parentSet)
			{
				prefabInstance.transform.parent = parent;
				prefabInstance.transform.Reset();
				_parentSet = true;
			}
		}

		public void SetModelType(int mType)
		{
			modelType = mType;
		}

		public override string ToString()
		{
			return string.Format(string.Concat("[Container - #Top: ", top, " #Bottom: ", bottom, "Left: ", left, " Right:", right, " #height: ", height, " #width:", width, " #dataIndex: ", positionIndex, "# modelType:", modelType, "# Bounds:", bounds, "# 2DCoordinates:", coordinates));
		}
	}
}
