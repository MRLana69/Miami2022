using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.Helpers
{
	public class WelcomePackButtonHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _timerLabel;

		public void OnWelcomePackButtonClicked()
		{
			UIScreenController.Instance.QueuePopup("WelcomePackPopup");
		}

		private void Update()
		{
			_timerLabel.text = WelcomePackManager.welcomePackTimeLeft.NicifyTimeString();
		}
	}
}
