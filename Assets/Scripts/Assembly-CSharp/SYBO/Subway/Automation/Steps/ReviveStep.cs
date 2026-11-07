using Data.Save;
using SYBO.Common.Automation.Steps;

namespace SYBO.Subway.Automation.Steps
{
	public class ReviveStep : AutomationStep
	{
		private bool _waitFrame;

		public override void Cleanup()
		{
			_waitFrame = false;
			base.Cleanup();
		}

		public override void Update(float unscaledDeltaTime)
		{
			if (_waitFrame)
			{
				InventorySave.IncreaseKeys(SaveMeManager.GetNumberOfKeysToSaveMe(), "Revive");
				base.IsDone = true;
			}
			else if (UIScreenController.Instance.isShowingPopup && UIScreenController.Instance.GetCurrentPopupName().Equals("SaveMePopup"))
			{
				_waitFrame = true;
			}
		}
	}
}
