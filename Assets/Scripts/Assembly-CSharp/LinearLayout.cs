using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class LinearLayout : MonoBehaviour
{
	public enum LayoutDirection
	{
		Horizontal = 0
	}

	public enum LayoutAlignment
	{
		Center = 0
	}

	[SerializeField]
	private bool _updateManually = true;

	[SerializeField]
	private int _horizontalSpacing;

	[SerializeField]
	private bool _useHorizontalWeights;

	[SerializeField]
	private WeightList _layoutWeights = new WeightList();

	public WeightList LayoutWeights
	{
		get
		{
			return _layoutWeights;
		}
	}

	private void Awake()
	{
		_layoutWeights.SyncList(base.transform);
	}

	private void Update()
	{
		if (!_updateManually)
		{
			UpdateChildren();
		}
	}

	public void UpdateChildren()
	{
		if (base.transform.childCount == 0)
		{
			return;
		}
		UIWidget component = GetComponent<UIWidget>();
		float num = component.width;
		float num2 = component.height;
		float num3 = 0f;
		if (_useHorizontalWeights)
		{
			int num4 = (int)num - _horizontalSpacing * (base.transform.childCount - 1);
			List<Vector2> list = _layoutWeights.CalculateProportionalWeights();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					UIWidget component2 = child.GetComponent<UIWidget>();
					Vector2 vector = list[i];
					int num5 = (component2.width = ((!_useHorizontalWeights) ? component2.width : ((int)((float)num4 * vector.x))));
					BoxCollider component3 = child.GetComponent<BoxCollider>();
					if (component3 != null)
					{
						component3.size = new Vector3(num5, component3.size.y, component3.size.z);
					}
					UpdateWidgetAnchors(component2);
				}
			}
		}
		int num7 = -1;
		for (int j = 0; j < base.transform.childCount; j++)
		{
			Transform child2 = base.transform.GetChild(j);
			if (child2.gameObject.activeSelf)
			{
				Transform transform = ((num7 < 0) ? null : base.transform.GetChild(num7));
				UIWidget component4 = child2.GetComponent<UIWidget>();
				UIWidget uIWidget = ((!(transform != null)) ? null : transform.GetComponent<UIWidget>());
				int num8 = ((uIWidget != null) ? uIWidget.width : 0);
				int num9 = ((uIWidget != null) ? ((int)transform.localPosition.x) : 0);
				if (component4 == null)
				{
					Debug.LogError("All children of linear layout must have ui widget.");
					return;
				}
				int num10 = ((j != 0) ? _horizontalSpacing : 0);
				int num11 = num9 + num8 + num10;
				child2.transform.localPosition = new Vector3(num11, child2.transform.localPosition.y, child2.transform.localPosition.z);
				num3 += (float)(component4.width + num10);
				num7 = j;
			}
		}
		UIWidget component5 = base.transform.GetChild(0).GetComponent<UIWidget>();
		float num12 = num3 / 2f - (float)(component5.width / 2);
		for (int k = 0; k < base.transform.childCount; k++)
		{
			Transform child3 = base.transform.GetChild(k);
			child3.transform.localPosition = new Vector3(child3.transform.localPosition.x - num12, child3.transform.localPosition.y, child3.transform.localPosition.z);
		}
	}

	private void UpdateWidgetAnchors(UIWidget widget)
	{
		widget.UpdateAnchors();
		if (widget.transform.childCount != 0)
		{
			for (int i = 0; i < widget.transform.childCount; i++)
			{
				UIWidget component = widget.transform.GetChild(i).GetComponent<UIWidget>();
				UpdateWidgetAnchors(component);
			}
		}
	}
}
