using UnityEngine;

namespace UI.DebugPopup
{
	public class CharacterIgnoreObstaclesButton : MonoBehaviour
	{
		[SerializeField]
		private UILabel _titleLabel;

		private int _characterLayer;

		private int _hitLayer;

		private void OnEnable()
		{
			_characterLayer = LayerMask.NameToLayer("Character");
			_hitLayer = LayerMask.NameToLayer("Hit");
			SetupButton(Physics.GetIgnoreLayerCollision(_hitLayer, _characterLayer));
		}

		public void Click()
		{
			bool ignoreLayerCollision = Physics.GetIgnoreLayerCollision(_hitLayer, _characterLayer);
			Physics.IgnoreLayerCollision(_hitLayer, _characterLayer, !ignoreLayerCollision);
			SetupButton(!ignoreLayerCollision);
		}

		private void SetupButton(bool enabled)
		{
			if (enabled)
			{
				_titleLabel.text = "Ignore Obstacles: ON";
			}
			else
			{
				_titleLabel.text = "Ignore Obstacles: OFF";
			}
		}
	}
}
