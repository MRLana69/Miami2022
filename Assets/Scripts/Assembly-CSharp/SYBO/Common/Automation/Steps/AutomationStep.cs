namespace SYBO.Common.Automation.Steps
{
	public abstract class AutomationStep : IAutomationStep
	{
		public bool IsDone { get; protected set; }

		public virtual void Initialize()
		{
		}

		public virtual void Cleanup()
		{
			IsDone = false;
		}

		public abstract void Update(float unscaledDeltaTime);
	}
}
