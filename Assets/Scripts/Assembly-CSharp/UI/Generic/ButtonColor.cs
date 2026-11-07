using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Generic
{
	public class ButtonColor : MonoBehaviour
	{
		[OnValueChanged("TakeNormalColorFromWidget")]
		[SerializeField]
		private UIWidget _widget;

		[SerializeField]
		private Color _normalColor;

		[SerializeField]
		private Color _pressedColor;

		public Color SetNormalColor
		{
			set
			{
				_normalColor = value;
			}
		}

		public Color SetPressedColor
		{
			set
			{
				_pressedColor = value;
			}
		}

		public void Setup(UIWidget widget, Color normal, Color pressed)
		{
			_widget = widget;
			_normalColor = normal;
			_pressedColor = pressed;
		}

		[UsedImplicitly]
		private void OnPress(bool pressed)
		{
			_widget.color = ((!pressed) ? _normalColor : _pressedColor);
		}

		[Button(null)]
		private void TakeNormalColorFromWidget()
		{
			if (_widget != null)
			{
				_normalColor = _widget.color;
			}
		}

		[Button(null)]
		private void MakePressedFromNormalColor()
		{
			float num = 30f / 255f;
			float r = Mathf.Clamp(_normalColor.r - num, 0f, 1f);
			float g = Mathf.Clamp(_normalColor.g - num, 0f, 1f);
			float b = Mathf.Clamp(_normalColor.b - num, 0f, 1f);
			float a = _normalColor.a;
			_pressedColor = new Color(r, g, b, a);
		}

		public void TransferFromUIButtonColor(UIWidget w, Color normalColor, Color pressedColor)
		{
			_normalColor = normalColor;
			_pressedColor = pressedColor;
			_widget = w;
		}
	}
}
