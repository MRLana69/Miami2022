using System;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.Breadcrumbs;
using UnityEngine;
using Utilities;

namespace UI.Footer
{
	public class FooterButton : MonoBehaviour
	{
		public class FooterPayload
		{
			public Action clicked;

			public string iconName;

			public ButtonColorScheme colorScheme;

			public int width;

			public string title;

			public Breadcrumb.BreadcrumbEvent[] breadcrumbEvents;

			public Vector3 iconOffset = Vector3.zero;
		}

		[SerializeField]
		private UIWidget _baseWidget;

		[SerializeField]
		private Collider _collider;

		[SerializeField]
		private UISprite _fill;

		[SerializeField]
		private UISprite _overlay;

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private Breadcrumb _breadcrumb;

		private FooterPayload _payload;

		public void Init(FooterPayload payload)
		{
			_payload = payload;
			_icon.spriteName = payload.iconName;
			_icon.MakePixelPerfect();
			_icon.transform.localPosition += payload.iconOffset;
			_title.text = _payload.title;
			ButtonColorScheme colorScheme = _payload.colorScheme;
			_fill.color = colorScheme.original.GetValueOrDefault();
			_overlay.color = colorScheme.light.GetValueOrDefault();
			_baseWidget.width = _payload.width;
			_baseWidget.gameObject.BroadcastMessage("UpdateAnchors");
			SetSelectedState(false);
			_breadcrumb.EnableListeners(_payload.breadcrumbEvents);
			if (SafeArea.IsRestricted)
			{
				base.transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
			}
		}

		public void SetSelectedState(bool selected)
		{
			_collider.enabled = !selected;
			_overlay.gameObject.SetActive(!selected);
			ButtonColorScheme colorScheme = _payload.colorScheme;
			Color32? selected2 = colorScheme.selected;
			if (selected2.HasValue)
			{
				Color32? original = colorScheme.original;
				if (original.HasValue)
				{
					_fill.color = ((!selected) ? colorScheme.original.Value : colorScheme.selected.Value);
				}
			}
		}

		[UsedImplicitly]
		private void OnClick()
		{
			_payload.clicked.SafeInvoke();
		}

		[UsedImplicitly]
		private void OnPress(bool pressed)
		{
			_overlay.gameObject.SetActive(!pressed);
			Color32 color = _payload.colorScheme.original.GetValueOrDefault();
			if (pressed)
			{
				color = GlobalColors.ApplyButtonPressColorOffset(color);
			}
			_fill.color = color;
		}
	}
}
