using SYBO.Common.Automation.Steps;

namespace SYBO.Subway.Automation.Steps
{
	public class StartGameAndWaitStep : WaitForSecondsStep
	{
		public StartGameAndWaitStep(float durationSeconds)
			: base(durationSeconds)
		{
		}

		public override void Update(float unscaledDeltaTime)
		{
			if (!Game.Instance.IsInGame.Value)
			{
				Game.Instance.StartNewGame();
				UIScreenController.Instance.PushScreen("IngameUI");
			}
			base.Update(unscaledDeltaTime);
		}
	}
}
