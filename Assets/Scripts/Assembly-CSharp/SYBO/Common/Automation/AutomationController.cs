using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Common.Automation
{
	public class AutomationController : SingletonMonoBehaviour<AutomationController>
	{
		private IAutomationStepExecutor _executor;

		private bool _skipInitialFrame;

		public bool IsRunning
		{
			get
			{
				return _executor != null;
			}
		}

		public void StartAutomation(IAutomationStepExecutor executor)
		{
			if (_executor != null)
			{
				SDebug.LogError("Can only run one automation at a time. Cancelling new automation.");
				return;
			}
			_executor = executor;
			_executor.Initialize();
			_skipInitialFrame = true;
		}

		protected void Update()
		{
			if (_executor == null)
			{
				return;
			}
			if (_skipInitialFrame)
			{
				_skipInitialFrame = false;
				_executor.StartExecution();
				return;
			}
			if (!_executor.IsDone)
			{
				_executor.Update(Time.unscaledDeltaTime);
			}
			if (_executor.IsDone)
			{
				_executor.Terminate();
				_executor = null;
			}
		}
	}
}
