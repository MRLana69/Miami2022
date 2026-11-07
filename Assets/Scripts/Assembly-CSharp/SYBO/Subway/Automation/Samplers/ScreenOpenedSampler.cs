using SYBO.Common.Benchmarking;

namespace SYBO.Subway.Automation.Samplers
{
	public class ScreenOpenedSampler : IMetricSampler
	{
		private bool _shouldSample;

		public void Initialize()
		{
			UIScreenController.OnNavigatedToScreen += UIScreenController_OnNavigateToScreen;
			UIScreenController.OnPopupShown += UIScreenController_OnPopupShown;
		}

		public void Cleanup()
		{
			UIScreenController.OnNavigatedToScreen -= UIScreenController_OnNavigateToScreen;
			UIScreenController.OnPopupShown -= UIScreenController_OnPopupShown;
		}

		public bool Update(float unscaledDeltaTime)
		{
			if (_shouldSample)
			{
				_shouldSample = false;
				return true;
			}
			return false;
		}

		private void UIScreenController_OnNavigateToScreen(string screenId)
		{
			if (!screenId.Equals("IngameUI"))
			{
				_shouldSample = true;
			}
		}

		private void UIScreenController_OnPopupShown(string popupId)
		{
			if (!string.IsNullOrEmpty(popupId))
			{
				_shouldSample = true;
			}
		}
	}
}
