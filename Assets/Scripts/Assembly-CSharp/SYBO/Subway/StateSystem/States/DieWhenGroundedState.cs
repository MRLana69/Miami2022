using SYBO.Common.Managers;
using SYBO.Common.StateSystem;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;

namespace SYBO.Subway.StateSystem.States
{
	public class DieWhenGroundedState : State
	{
		private Character _character;

		private int _frameCount;

		public DieWhenGroundedState(int id)
			: base(id)
		{
			_character = Character.Instance;
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			_frameCount = 0;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (_character.CharacterController.isGrounded)
			{
				GameStateMachine.Instance.ActivateTrigger(108);
				return;
			}
			float y = _character.GamePosition.y;
			_character.MoveWithGravity();
			float y2 = _character.GamePosition.y;
			float num = y2 - y;
			if (num == 0f && SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale != 0f)
			{
				_frameCount++;
				if (_frameCount > 3)
				{
					GameStateMachine.Instance.ActivateTrigger(108);
				}
			}
			else
			{
				_frameCount = 0;
			}
		}
	}
}
