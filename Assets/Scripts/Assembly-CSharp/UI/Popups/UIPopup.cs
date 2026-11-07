using UI.Screens;

namespace UI.Popups
{
	public abstract class UIPopup : UIScreen
	{
		public virtual void ClosePopup()
		{
			UIScreenController.Instance.ClosePopup();
		}

		public override void BackButtonPressed()
		{
			UIScreenController.Instance.ClosePopup();
		}
	}
}
