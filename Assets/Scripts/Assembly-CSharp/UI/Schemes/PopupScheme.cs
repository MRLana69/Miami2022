using System;
using System.Collections.Generic;
using System.Linq;
using Kiloo.Common.Extensions;
using UI.Generic;
using UI.MessageCenter;
using UI.Popups;
using UnityEngine;

namespace UI.Schemes
{
	public class PopupScheme : MonoBehaviour
	{
		private enum Size
		{
			Medium = 0,
			Large = 1
		}

		private struct CustomSetup
		{
			public Color32? backgroundColor;

			public int? maxHeight;
		}

		[SerializeField]
		private Size _size;

		[SerializeField]
		private UISprite _bgFill;

		[SerializeField]
		private UISprite _bgOutline;

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UISprite _closeButton;

		private const int WIDTH_LARGE = 300;

		private const int WIDTH_MEDIUM = 280;

		private const int HEIGHT_MAX = 432;

		private readonly Vector4 _outlineAnchor = new Vector4(-2f, 7f, -7f, 2f);

		private Dictionary<string, CustomSetup> _customSetup = new Dictionary<string, CustomSetup>
		{
			{
				"ConsentPopup",
				new CustomSetup
				{
					backgroundColor = new Color32(0, 0, 0, byte.MaxValue),
					maxHeight = 460
				}
			},
			{
				"AgeRestrictionPopup",
				new CustomSetup
				{
					backgroundColor = new Color32(0, 0, 0, byte.MaxValue),
					maxHeight = 460
				}
			}
		};

		private const int ANCHOR_LEFT = 0;

		private const int ANCHOR_RIGHT = 1;

		private const int ANCHOR_BOTTOM = 0;

		private const int ANCHOR_TOP = 1;

		private bool TryFindChild(Transform trans, out Transform child, params string[] names)
		{
			foreach (Transform transChild in trans)
			{
				if (names.Any((string n) => transChild.name == n))
				{
					child = transChild;
					return true;
				}
				if (TryFindChild(transChild, out child, names))
				{
					return true;
				}
			}
			child = null;
			return false;
		}

		private void ResetReferences()
		{
			Transform child;
			if (TryFindChild(base.transform, out child, "Background", "0Background"))
			{
				_background = child.GetComponent<UISprite>();
			}
			if (TryFindChild(base.transform, out child, "BGFill", "0BGFill", "1BGFill"))
			{
				_bgFill = child.GetComponent<UISprite>();
			}
			if (TryFindChild(base.transform, out child, "BGOutline", "Outline", "0BGOutline", "1BGOutline", "2BGOutline"))
			{
				_bgOutline = child.GetComponent<UISprite>();
			}
			if (TryFindChild(base.transform, out child, "CloseButton", "1CloseButton"))
			{
				_closeButton = child.GetComponent<UISprite>();
			}
		}

		private void SetupBg()
		{
			_background.name = "Background";
			_background.transform.parent = base.transform;
			_background.transform.SetAsFirstSibling();
			_background.pivot = UIWidget.Pivot.Center;
			CustomSetup value;
			if (_customSetup.TryGetValue(base.name, out value) && value.backgroundColor.HasValue)
			{
				_background.color = value.backgroundColor.Value;
			}
			else
			{
				_background.color = new Color32(0, 0, 0, 204);
			}
			DestroyStretchAndAnchorIfExists(_background);
			SetAnchorsRegular(_background, base.transform, -1f, 1f, -1f, 1f);
			_background.updateAnchors = UIRect.AnchorUpdate.OnEnable;
			_background.ResetAndUpdateAnchors();
		}

		private void DestroyStretchAndAnchorIfExists(Component obj)
		{
			UIStretch component = obj.GetComponent<UIStretch>();
			if ((bool)component)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			UIAnchor component2 = obj.GetComponent<UIAnchor>();
			if ((bool)component2)
			{
				UnityEngine.Object.DestroyImmediate(component2);
			}
		}

		private void SetAnchorsRegular(UIWidget widget, Transform target, float left, float right, float bottom, float top)
		{
			widget.leftAnchor.Set(target, 0f, left);
			widget.rightAnchor.Set(target, 1f, right);
			widget.bottomAnchor.Set(target, 0f, bottom);
			widget.topAnchor.Set(target, 1f, top);
		}

		private void RemoveAnchors(UIWidget widget)
		{
			widget.leftAnchor.target = null;
			widget.rightAnchor.target = null;
			widget.bottomAnchor.target = null;
			widget.topAnchor.target = null;
		}

		private void SetupBgOutline()
		{
			_bgOutline.name = "BGOutline";
			if (IsSamePanel(_bgOutline))
			{
				_bgOutline.transform.parent = _bgFill.transform;
				_bgOutline.transform.SetAsFirstSibling();
			}
			_bgOutline.spriteName = "popup_outline";
			_bgOutline.color = ColorData.instance.stroking;
			_bgOutline.pivot = UIWidget.Pivot.Center;
			DestroyStretchAndAnchorIfExists(_bgOutline);
			UISprite bgOutline = _bgOutline;
			Transform target = _bgFill.transform;
			Vector4 outlineAnchor = _outlineAnchor;
			float x = outlineAnchor.x;
			Vector4 outlineAnchor2 = _outlineAnchor;
			float y = outlineAnchor2.y;
			Vector4 outlineAnchor3 = _outlineAnchor;
			float z = outlineAnchor3.z;
			Vector4 outlineAnchor4 = _outlineAnchor;
			SetAnchorsRegular(bgOutline, target, x, y, z, outlineAnchor4.w);
			_bgOutline.updateAnchors = UIRect.AnchorUpdate.OnEnable;
			_bgOutline.ResetAndUpdateAnchors();
		}

		private void SetupBgFill()
		{
			_bgFill.name = "BGFill";
			DestroyStretchAndAnchorIfExists(_bgFill);
			RemoveAnchors(_bgFill);
			_bgFill.transform.parent = base.transform;
			_bgFill.pivot = UIWidget.Pivot.Center;
			_bgFill.transform.localPosition = Vector3.zero;
			CustomSetup value;
			int b = ((!_customSetup.TryGetValue(base.name, out value) || !value.maxHeight.HasValue) ? 432 : value.maxHeight.Value);
			_bgFill.height = Mathf.Min(_bgFill.height, b);
			int width;
			switch (_size)
			{
			case Size.Medium:
				width = 280;
				break;
			case Size.Large:
				width = 300;
				break;
			default:
				width = 280;
				break;
			}
			_bgFill.width = width;
			_bgFill.ResetAndUpdateAnchors();
			NGUITools.AddWidgetCollider(_bgFill.gameObject);
		}

		private void SetupCloseButton()
		{
			_closeButton.name = "CloseButton";
			UIButtonColor component = _closeButton.GetComponent<UIButtonColor>();
			if ((bool)component)
			{
				UnityEngine.Object.DestroyImmediate(component);
				ButtonColor buttonColor = _closeButton.gameObject.AddComponent<ButtonColor>();
				buttonColor.Setup(_closeButton, Color.white, Color.gray);
			}
			Transform transform = _bgFill.transform;
			if (IsSamePanel(_closeButton))
			{
				_closeButton.transform.parent = transform;
				_closeButton.transform.SetSiblingIndex(1);
			}
			_closeButton.spriteName = "btn_close";
			_closeButton.pivot = UIWidget.Pivot.Center;
			_closeButton.leftAnchor.Set(transform, 1f, -25f);
			_closeButton.rightAnchor.Set(transform, 1f, 7f);
			_closeButton.bottomAnchor.Set(transform, 1f, -25f);
			_closeButton.topAnchor.Set(transform, 1f, 7f);
			_closeButton.updateAnchors = UIRect.AnchorUpdate.OnEnable;
			_closeButton.autoResizeBoxCollider = true;
			_closeButton.ResizeCollider();
			_closeButton.ResetAndUpdateAnchors();
			UIButtonChangeScreen component2 = _closeButton.GetComponent<UIButtonChangeScreen>();
			if (component2 != null && component2.screenChangeType == UIButtonChangeScreen.ScreenChangeType.ClosePopup)
			{
				UIPopup component3 = GetComponent<UIPopup>();
				if (component3 == null)
				{
					Debug.LogError("A UIPopup was not found on " + base.gameObject.name, base.gameObject);
					throw new Exception("Missing UIPopup");
				}
				UIEventTrigger component4 = _closeButton.GetComponent<UIEventTrigger>();
				if (component4 != null)
				{
					Debug.LogError("Both a ButtonChangeScreen and a UIEventTrigger is on the Close Button. Please verify manually and consider revising.");
					throw new Exception("Both ButtonChangeScreen and UIEventTrigger found.");
				}
				component2.Destroy();
				component4 = _closeButton.GetOrAdd<UIEventTrigger>();
				component4.onClick.Add(new EventDelegate(GetComponent<UIPopup>().ClosePopup));
			}
			if ((bool)_closeButton && (bool)_background)
			{
				UIEventTrigger orAdd = _background.GetOrAdd<UIEventTrigger>();
				orAdd.onClick = _closeButton.GetComponent<UIEventTrigger>().onClick;
				_background.GetOrAdd<BoxCollider>();
			}
		}

		private bool IsSamePanel(UIWidget widget)
		{
			return GetComponent<UIPanel>() == widget.panel;
		}
	}
}
