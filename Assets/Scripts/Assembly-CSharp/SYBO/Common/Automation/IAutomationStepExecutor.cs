namespace SYBO.Common.Automation
{
	public interface IAutomationStepExecutor
	{
		bool IsDone { get; }

		float ElapsedDuration { get; }

		void Initialize();

		void StartExecution();

		void Update(float unscaledDeltaTime);

		void Terminate();
	}
}
