using SYBO.Common.Automation.Steps;

namespace SYBO.Subway.Automation.Steps
{
	public class OpenCloseUIElementStep : AutomationStep
	{
		public enum UIElementType
		{
			Popup = 0,
			Screen = 1
		}

		private enum StepState
		{
			None = 0,
			WaitingBeforeOpen = 1,
			Open = 2,
			Closed = 3
		}

		private UIElementType _type;

		private string _name = string.Empty;

		private const float DelayBefore = 0.2f;

		private const float OpenTime = 0.2f;

		private const float DelayAfter = 0.2f;

		private float _currentDuration;

		private StepState _currentState;

		private bool _skipFrame;

		public OpenCloseUIElementStep(UIElementType uiElementType, string name)
		{
			_type = uiElementType;
			_name = name;
		}

		public override void Initialize()
		{
			base.Initialize();
			_currentState = StepState.WaitingBeforeOpen;
		}

		public override void Cleanup()
		{
			_currentDuration = 0f;
			_currentState = StepState.None;
			_skipFrame = false;
			base.Cleanup();
		}

		public override void Update(float unscaledDeltaTime)
		{
			if (Game.Instance.IsInGame.Value)
			{
				UIScreenController.Instance.GoToMainMenuFromGame(null);
				UIScreenController.Instance.ClosePopup();
			}
			if (_skipFrame)
			{
				_skipFrame = false;
				return;
			}
			_currentDuration += unscaledDeltaTime;
			if (_currentState == StepState.WaitingBeforeOpen && _currentDuration >= 0.2f)
			{
				if (_type == UIElementType.Popup)
				{
					UIScreenController.Instance.PushPopup(_name);
				}
				else if (_type == UIElementType.Screen)
				{
					UIScreenController.Instance.PushScreen(_name);
				}
				_currentState = StepState.Open;
				_skipFrame = true;
			}
			else if (_currentState == StepState.Open && _currentDuration >= 0.4f)
			{
				if (_type == UIElementType.Popup)
				{
					UIScreenController.Instance.ClosePopup();
				}
				else if (_type == UIElementType.Screen)
				{
					UIScreenController.Instance.BackToPreviousScreen();
				}
				_currentState = StepState.Closed;
			}
			else if (_currentState == StepState.Closed && _currentDuration >= 0.6f)
			{
				UIScreenController.Instance.ClosePopup();
				base.IsDone = true;
			}
		}
	}
}
