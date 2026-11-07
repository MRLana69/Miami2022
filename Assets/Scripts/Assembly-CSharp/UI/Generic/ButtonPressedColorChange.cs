using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Generic
{
	public class ButtonPressedColorChange : MonoBehaviour
	{
		[OnValueChanged("TakeNormalColorFromWidget")]
		[SerializeField]
		private UIWidget _widget;

		public Color normalColor { private get; set; }

		public Color pressedColor { private get; set; }

		[UsedImplicitly]
		private void OnPress(bool pressed)
		{
			_widget.color = ((!pressed) ? normalColor : pressedColor);
		}
	}
}
