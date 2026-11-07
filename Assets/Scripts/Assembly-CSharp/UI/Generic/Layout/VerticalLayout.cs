using System;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.Generic.Layout
{
	[ExecuteInEditMode]
	public class VerticalLayout : Layout
	{
		[UsedImplicitly]
		public VerticalAlignment alignment;

		[ContextMenu("Align")]
		protected override void Align_Internal()
		{
			if (this == null)
			{
				return;
			}
			if (_panel == null)
			{
				_panel = GetComponentInParent<UIPanel>();
			}
			if (_panel != null)
			{
				_panel.Invalidate(true);
			}
			Action action = null;
			_calculatedHeight = 0f;
			if (_snapToLabelContent)
			{
				UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>();
				foreach (UILabel l in componentsInChildren)
				{
					if (l.overflowMethod != UILabel.Overflow.ResizeHeight)
					{
						int maxHeight = l.height;
						float y = l.printedSize.y;
						if (y < (float)maxHeight)
						{
							l.height = y.CeilToInt();
						}
						action = (Action)Delegate.Combine(action, (Action)(() =>
						{
							l.height = maxHeight;
						}));
					}
				}
			}
			switch (alignment)
			{
			case VerticalAlignment.Center:
				_calculatedHeight = base.transform.CenterChildrenVertically(padding);
				break;
			case VerticalAlignment.Up:
				_calculatedHeight = base.transform.AlignChildrenVertically(padding, false);
				break;
			case VerticalAlignment.Down:
				_calculatedHeight = base.transform.AlignChildrenVertically(padding, true);
				break;
			}
			action.SafeInvoke();
			if (!autosizeWidget)
			{
				return;
			}
			UIWidget uIWidget = ((!targetWidget) ? GetComponent<UIWidget>() : targetWidget);
			if (!(uIWidget != null))
			{
				return;
			}
			_calculatedHeight = Mathf.Max(_calculatedHeight + autosizeWidgetMargin, autosizeWidgetMinWidth);
			uIWidget.height = Mathf.RoundToInt(_calculatedHeight);
			if (!adjustWidgetPosition)
			{
				return;
			}
			Vector3 localPosition = uIWidget.transform.localPosition;
			switch (alignment)
			{
			case VerticalAlignment.Up:
				localPosition.y = _calculatedHeight / 2f;
				if ((bool)targetWidget)
				{
				}
				break;
			case VerticalAlignment.Down:
				localPosition.y = _calculatedHeight / -2f;
				if ((bool)targetWidget)
				{
				}
				break;
			}
			uIWidget.transform.localPosition = localPosition;
		}
	}
}
