using Debugging;
using UnityEngine;

namespace UI.DebugPopup
{
	public class FPSCounterButton : MonoBehaviour
	{
		[SerializeField]
		private UILabel _titleLabel;

		private bool _showCounter;

		private void OnEnable()
		{
			FPSCounter instance = FPSCounter.Instance;
			if (instance != null)
			{
				_showCounter = instance.gameObject.activeInHierarchy;
			}
			SetupButton(_showCounter);
		}

		public void Click()
		{
			_showCounter = !_showCounter;
			FPSCounter fPSCounter = FPSCounter.Instance;
			if (_showCounter)
			{
				if (fPSCounter == null)
				{
					fPSCounter = FPSCounter.Create(UIScreenController.Instance.popupLayer);
				}
				fPSCounter.ToggleVisible(true);
			}
			else if (fPSCounter != null)
			{
				fPSCounter.ToggleVisible(false);
			}
			SetupButton(_showCounter);
		}

		private void SetupButton(bool enabled)
		{
			if (enabled)
			{
				_titleLabel.text = "FPS Counter: ON";
			}
			else
			{
				_titleLabel.text = "FPS Counter: OFF";
			}
		}
	}
}
