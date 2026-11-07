namespace SYBO.Common.Automation.Steps
{
	public class CompoundAutomationStep : IAutomationStep
	{
		private IAutomationStep[] _steps;

		private int _currentStepIndex;

		public bool IsDone
		{
			get
			{
				return _steps.Length == 0 || _steps[_steps.Length - 1].IsDone;
			}
		}

		public CompoundAutomationStep(IAutomationStep[] steps)
		{
			_steps = steps;
		}

		public virtual void Initialize()
		{
			for (int i = 0; i < _steps.Length; i++)
			{
				_steps[i].Initialize();
			}
		}

		public virtual void Cleanup()
		{
			for (int i = 0; i < _steps.Length; i++)
			{
				_steps[i].Cleanup();
			}
		}

		public void Update(float unscaledDeltaTime)
		{
			if (!IsDone)
			{
				if (_steps[_currentStepIndex].IsDone)
				{
					_currentStepIndex++;
				}
				_steps[_currentStepIndex].Update(unscaledDeltaTime);
			}
		}
	}
}
