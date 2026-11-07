using JetBrains.Annotations;
using UnityEngine;

namespace UI.Popups.SaveMePopupInternal
{
	public class SaveMeCancelButton : MonoBehaviour
	{
		[SerializeField]
		private SaveMePopup _saveMePopup;

		[UsedImplicitly]
		private void OnClick()
		{
			if (_saveMePopup != null)
			{
				if (SaveMeManager.videoIsPlaying)
				{
					SaveMeManager.videoIsPlaying = false;
					_saveMePopup.StartCountdown();
				}
				else if (_saveMePopup.animTimeLeft < SaveMeManager.GetTimeOutAnimationDuration() - 1f)
				{
					CloseSaveMe();
				}
			}
		}

		public static void CloseSaveMe()
		{
			UIScreenController.Instance.CloseActivePopups("SaveMePopup");
			Revive.Instance.SendSkipRevive();
		}
	}
}
