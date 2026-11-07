using UnityEngine;

namespace UI.Generic.Layout
{
	[ExecuteInEditMode]
	public class TableLayout : Layout
	{
		[SerializeField]
		private UIWidget _contentArea;

		[SerializeField]
		private float _horizontalPadding;

		[SerializeField]
		private float _verticalPadding;

		[SerializeField]
		private bool _fillVertically;

		protected override void Align_Internal()
		{
			if (this == null)
			{
				return;
			}
			if (_contentArea == null && GetComponent<UIWidget>() != null)
			{
				_contentArea = GetComponent<UIWidget>();
			}
			Transform[] array = new Transform[base.transform.childCount];
			for (int i = 0; i < base.transform.childCount; i++)
			{
				array[i] = base.transform.GetChild(i);
			}
			Vector2 vector = new Vector2(0f, 0f);
			if (!_fillVertically)
			{
				int width = _contentArea.width;
				float num = 0f;
				foreach (Transform transform in array)
				{
					if (transform.gameObject.activeSelf)
					{
						Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform, transform, false);
						if (bounds.extents.x * 2f + vector.x > (float)width)
						{
							vector.x = 0f;
							vector.y -= num + _verticalPadding;
							num = 0f;
						}
						num = Mathf.Max(num, bounds.extents.y * 2f);
						Vector3 localPosition = transform.localPosition;
						localPosition.x = vector.x + (localPosition.x - bounds.center.x) + bounds.extents.x;
						localPosition.y = vector.y - (bounds.center.y - localPosition.y) - bounds.extents.y;
						transform.localPosition = localPosition;
						vector.x += bounds.extents.x * 2f;
						vector.x += _horizontalPadding;
					}
				}
				return;
			}
			int height = _contentArea.height;
			float num2 = 0f;
			foreach (Transform transform2 in array)
			{
				if (transform2.gameObject.activeSelf)
				{
					Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(base.transform, transform2, false);
					if (0f - bounds2.size.y + vector.y < (float)(-height))
					{
						vector.y = 0f;
						vector.x += num2 + _horizontalPadding;
						num2 = 0f;
					}
					num2 = Mathf.Max(num2, bounds2.size.x);
					Vector3 localPosition2 = transform2.localPosition;
					localPosition2.x = vector.x + (localPosition2.x - bounds2.center.x) + bounds2.extents.x;
					localPosition2.y = vector.y - (localPosition2.y - bounds2.center.y) - bounds2.extents.y;
					transform2.localPosition = localPosition2;
					vector.y -= bounds2.size.y;
					vector.y -= _verticalPadding;
				}
			}
		}
	}
}
