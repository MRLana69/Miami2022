namespace SYBO.Common.Automation.Steps
{
	public class WaitForSecondsStep : AutomationStep
	{
		private float _durationSeconds;

		private float _elapsedDurationSeconds;

		public WaitForSecondsStep(float durationSeconds)
		{
			_durationSeconds = durationSeconds;
		}

		public override void Cleanup()
		{
			_elapsedDurationSeconds = 0f;
			base.Cleanup();
		}

		public override void Update(float unscaledDeltaTime)
		{
			_elapsedDurationSeconds += unscaledDeltaTime;
			if (_elapsedDurationSeconds >= _durationSeconds)
			{
				base.IsDone = true;
			}
		}
	}
}
