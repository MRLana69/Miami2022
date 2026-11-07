namespace SYBO.Common.Automation.Steps
{
	public class WaitUntilDoneStep : AutomationStep
	{
		public override void Update(float unscaledDeltaTime)
		{
		}

		public void MarkAsDone()
		{
			base.IsDone = true;
		}
	}
}
