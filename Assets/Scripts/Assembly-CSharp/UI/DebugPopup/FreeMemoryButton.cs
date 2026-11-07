using Debugging;
using UnityEngine;

namespace UI.DebugPopup
{
	public class FreeMemoryButton : MonoBehaviour
	{
		[SerializeField]
		private UILabel _titleLabel;

		private bool _showingCounter;

		private void OnEnable()
		{
			FreeMemoryStatus instance = FreeMemoryStatus.Instance;
			if (instance != null)
			{
				_showingCounter = instance.gameObject.activeInHierarchy;
			}
			SetupButton(_showingCounter);
		}

		public void Click()
		{
			FreeMemoryStatus freeMemoryStatus = FreeMemoryStatus.Instance;
			if (!_showingCounter)
			{
				if (freeMemoryStatus == null)
				{
					freeMemoryStatus = FreeMemoryStatus.Create(UIScreenController.Instance.popupLayer);
				}
				if (freeMemoryStatus != null)
				{
					freeMemoryStatus.ToggleVisible(true);
					_showingCounter = true;
				}
			}
			else if (freeMemoryStatus != null)
			{
				freeMemoryStatus.ToggleVisible(false);
				_showingCounter = false;
			}
			SetupButton(_showingCounter);
		}

		private void SetupButton(bool enabled)
		{
			if (enabled)
			{
				_titleLabel.text = "Free Memory Status: ON";
			}
			else
			{
				_titleLabel.text = "Free Memory Status: OFF";
			}
		}
	}
}
