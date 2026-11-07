using UnityEngine;

namespace UI.DebugPopup
{
	public class CharacterInvincibleButton : MonoBehaviour
	{
		[SerializeField]
		private UILabel _titleLabel;

		private bool _enabled;

		private void OnEnable()
		{
			SetupButton(_enabled);
		}

		public void Click()
		{
			_enabled = !_enabled;
			SetupButton(_enabled);
		}

		private void SetupButton(bool enabled)
		{
			if (enabled)
			{
				_titleLabel.text = "Invincible: ON";
			}
			else
			{
				_titleLabel.text = "Invincible: OFF";
			}
		}
	}
}
