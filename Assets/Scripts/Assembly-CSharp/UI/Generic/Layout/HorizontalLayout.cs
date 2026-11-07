using System;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.Generic.Layout
{
	[ExecuteInEditMode]
	public class HorizontalLayout : Layout
	{
		[UsedImplicitly]
		public HorizontalAlignment alignment;

		[ContextMenu("Align")]
		protected override void Align_Internal()
		{
			try
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
				_calculatedWidth = 0f;
				Action action = null;
				if (_snapToLabelContent)
				{
					UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>();
					foreach (UILabel l in componentsInChildren)
					{
						if (l.overflowMethod != UILabel.Overflow.ResizeFreely)
						{
							int maxWidth = l.width;
							float x = l.printedSize.x;
							if (x < (float)maxWidth)
							{
								l.width = x.CeilToInt();
							}
							action = (Action)Delegate.Combine(action, (Action)(() =>
							{
								l.width = maxWidth;
							}));
						}
					}
				}
				switch (alignment)
				{
				case HorizontalAlignment.Center:
					_calculatedWidth = base.transform.CenterChildrenHorizontally(padding);
					break;
				case HorizontalAlignment.Left:
					_calculatedWidth = base.transform.AlignChildrenHorizontally(padding, true);
					break;
				case HorizontalAlignment.Right:
					_calculatedWidth = base.transform.AlignChildrenHorizontally(padding, false);
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
				_calculatedWidth = Mathf.Max(_calculatedWidth + autosizeWidgetMargin, autosizeWidgetMinWidth);
				uIWidget.width = Mathf.RoundToInt(_calculatedWidth);
				if (!adjustWidgetPosition)
				{
					return;
				}
				UIWidget.Pivot pivot = uIWidget.pivot;
				int num = (int)pivot;
				pivot = ((num <= 2) ? UIWidget.Pivot.Top : ((num > 5) ? UIWidget.Pivot.Bottom : UIWidget.Pivot.Center));
				uIWidget.pivot = pivot;
				Vector3 localPosition = uIWidget.transform.localPosition;
				switch (alignment)
				{
				case HorizontalAlignment.Center:
					localPosition.x = 0f;
					base.transform.SetLocalPosX(0f);
					break;
				case HorizontalAlignment.Left:
					localPosition.x = _calculatedWidth / 2f;
					if ((bool)targetWidget)
					{
					}
					break;
				case HorizontalAlignment.Right:
					localPosition.x = 0f - _calculatedWidth / 2f;
					if ((bool)targetWidget)
					{
					}
					break;
				}
				uIWidget.transform.localPosition = localPosition;
			}
			catch (Exception ex)
			{
				SDebug.LogError("Exception caught in HorizontalLayout.Align_Internal(): {0}\nStacktrace:\n{1}".Populate(ex.Message, ex.StackTrace));
			}
		}
	}
}
