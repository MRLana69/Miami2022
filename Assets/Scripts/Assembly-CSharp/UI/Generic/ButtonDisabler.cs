using JetBrains.Annotations;
using UnityEngine;

namespace UI.Generic
{
	public class ButtonDisabler : MonoBehaviour
	{
		[SerializeField]
		private UIWidget _widget;

		[UsedImplicitly]
		private void OnPress(bool pressed)
		{
			_widget.gameObject.SetActive(!pressed);
		}
	}
}
