using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Utilities;

public class ScrollViewHeightResizer : MonoBehaviour
{
	[SerializeField]
	private GameObject _scrollBar;

	[SerializeField]
	private GameObject _scrollCollider;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private GameObject _grid;

	[SerializeField]
	private float _staticObjectsHeight;

	[SerializeField]
	private float _centerOffset;

	[SerializeField]
	private float _topStaticObjectsHeight;

	[SerializeField]
	private bool _addTopCollider;

	[SerializeField]
	private bool _addBottomcollider;

	[SerializeField]
	private float _zDepth;

	private bool isCalculated;

	private UITable _table;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action onRearrangedScrollView__BackingField;

	[SerializeField]
	private bool rearrange;

	public float staticObjectsHeight
	{
		get
		{
			return _staticObjectsHeight;
		}
		set
		{
			_staticObjectsHeight = value;
		}
	}

	public float topStaticObjectsHeight
	{
		get
		{
			return _topStaticObjectsHeight;
		}
		set
		{
			_topStaticObjectsHeight = value;
		}
	}

	public Vector4 clipping
	{
		get
		{
			if (!isCalculated)
			{
				RearrangeWidgets();
			}
			return _scrollPanel.finalClipRegion;
		}
	}

	public float Height
	{
		get
		{
			return _staticObjectsHeight;
		}
		set
		{
			_staticObjectsHeight = value;
		}
	}

	public Vector3 scrollPanelPosition
	{
		get
		{
			if (!isCalculated)
			{
				RearrangeWidgets();
			}
			return _scrollPanel.transform.localPosition;
		}
	}

	public event Action onRearrangedScrollView
	{
		add
		{
			Action action = onRearrangedScrollView__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRearrangedScrollView__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onRearrangedScrollView__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRearrangedScrollView__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Update()
	{
		if (rearrange)
		{
			RearrangeWidgets();
			rearrange = false;
		}
	}

	private void Start()
	{
		rearrange = false;
		_table = _grid.GetComponent<UITable>();
		if (_scrollBar == null)
		{
			SDebug.LogError("ScrollBar not set in ScrollViewHeightResizer");
		}
		if (_scrollCollider == null)
		{
			SDebug.LogError("ScrollCollider not set in ScrollViewHeightResizer");
		}
		if (_scrollPanel == null)
		{
			SDebug.LogError("ScrollPanel not set in ScrollViewHeightResizer");
		}
		if (_grid == null)
		{
			SDebug.LogError("Grid not set in ScrollViewHeightResizer");
		}
		RearrangeWidgets();
	}

	public void RearrangeWidgets()
	{
		UIRoot uIRoot = null;
		if (UIScreenController.Instance != null)
		{
			uIRoot = UIScreenController.Instance.root;
		}
		if (uIRoot == null)
		{
		}
		UIScrollBar component = _scrollBar.GetComponent<UIScrollBar>();
		float y = (float)(uIRoot.manualHeight / 2) + _centerOffset;
		float num = (float)uIRoot.manualHeight - _staticObjectsHeight;
		if (SafeArea.IsRestricted)
		{
			num -= (float)(SafeArea.OFFSET_BOTTOM + SafeArea.OFFSET_TOP);
		}
		Vector4 baseClipRegion = _scrollPanel.baseClipRegion;
		baseClipRegion.y = y;
		baseClipRegion.w = num;
		_scrollPanel.baseClipRegion = baseClipRegion;
		Vector3 localPosition = _scrollPanel.transform.localPosition;
		localPosition.y = _scrollPanel.clipOffset.y * -1f;
		_scrollPanel.transform.localPosition = localPosition;
		Vector3 localPosition2 = _grid.transform.localPosition;
		localPosition2.y = 0f;
		_grid.transform.localPosition = localPosition2;
		float y2 = (float)(uIRoot.manualHeight / 2) + _centerOffset;
		float y3 = (float)uIRoot.manualHeight - _staticObjectsHeight;
		Vector3 localPosition3 = _scrollCollider.transform.localPosition;
		localPosition3.y = y2;
		_scrollCollider.transform.localPosition = localPosition3;
		Vector3 localScale = _scrollCollider.transform.localScale;
		localScale.y = y3;
		_scrollCollider.transform.localScale = localScale;
		UIWidget backgroundWidget = component.backgroundWidget;
		UIWidget foregroundWidget = component.foregroundWidget;
		int height = (backgroundWidget.height = Mathf.RoundToInt(num));
		foregroundWidget.height = height;
		Vector3 localPosition4 = _scrollBar.transform.localPosition;
		localPosition4.y = (float)uIRoot.manualHeight - _topStaticObjectsHeight;
		if (SafeArea.IsRestricted)
		{
			localPosition4.y -= SafeArea.OFFSET_TOP;
		}
		_scrollBar.transform.localPosition = localPosition4;
		_table.Reposition();
		AddColliders();
		isCalculated = true;
		if (onRearrangedScrollView__BackingField != null)
		{
			onRearrangedScrollView__BackingField();
		}
	}

	private void AddColliders()
	{
		Vector3 zero = Vector3.zero;
		zero.z = _zDepth;
		if (!isCalculated)
		{
			if (_addTopCollider)
			{
				GameObject gameObject = base.gameObject.transform.gameObject.AddChild();
				gameObject.AddComponent<BoxCollider>().center = zero;
				gameObject.name = "TopScrollBlocker";
				float num = (float)Screen.height - (_scrollCollider.transform.localPosition.y + _scrollCollider.transform.localScale.y / 2f);
				float y = (float)Screen.height - num / 2f;
				float x = Screen.width;
				float x2 = 0f;
				float z = base.gameObject.transform.localPosition.z;
				gameObject.transform.localPosition = new Vector3(x2, y, z);
				gameObject.transform.localScale = new Vector3(x, num, 1f);
			}
			if (_addBottomcollider)
			{
				GameObject gameObject2 = base.gameObject.transform.gameObject.AddChild();
				gameObject2.AddComponent<BoxCollider>().center = zero;
				gameObject2.name = "BottomScrollBlocker";
				float num2 = _scrollCollider.transform.localPosition.y - _scrollCollider.transform.localScale.y / 2f;
				float y2 = num2 / 2f;
				float x3 = Screen.width;
				float x4 = 0f;
				float z2 = base.gameObject.transform.localPosition.z;
				gameObject2.transform.localPosition = new Vector3(x4, y2, z2);
				gameObject2.transform.localScale = new Vector3(x3, num2, 1f);
			}
		}
	}
}
