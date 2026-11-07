using NaughtyAttributes;
using UnityEngine;

namespace UI.Generic
{
	public class ButtonText : MonoBehaviour
	{
		[SerializeField]
		private UILabel _label;

		[SerializeField]
		private Collider _collider;

		[SerializeField]
		private Color _activeColor;

		[SerializeField]
		private Color _activeEffectColor;

		[SerializeField]
		private Color _inactiveColor;

		[SerializeField]
		private Color _inactiveEffectColor;

		[Button(null)]
		private void TakeActiveColorFromWidget()
		{
			if (_label != null)
			{
				_activeColor = _label.color;
				_activeEffectColor = _label.effectColor;
			}
		}

		public void Refresh()
		{
			if (!(_label == null) && !(_collider == null))
			{
				bool flag = _collider.enabled;
				_label.color = ((!flag) ? _inactiveColor : _activeColor);
				_label.effectColor = ((!flag) ? _inactiveEffectColor : _activeEffectColor);
			}
		}
	}
}
