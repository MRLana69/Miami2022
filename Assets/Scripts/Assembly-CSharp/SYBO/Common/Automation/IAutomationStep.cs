namespace SYBO.Common.Automation
{
	public interface IAutomationStep
	{
		bool IsDone { get; }

		void Initialize();

		void Cleanup();

		void Update(float unscaledDeltaTime);
	}
}
