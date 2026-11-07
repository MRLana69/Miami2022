using SYBO.Common.Automation.Steps;

namespace SYBO.Subway.Automation.Steps
{
	public class WaitForHomeScreenStep : AutomationStep
	{
		public override void Update(float unscaledDeltaTime)
		{
			if (UIScreenController.Instance.GetTopScreenName().Equals("FrontScreen"))
			{
				base.IsDone = true;
			}
		}
	}
}
