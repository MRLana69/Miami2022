using SYBO.Common.StateSystem;
using SYBO.Subway.Inputs;
using SYBO.Subway.StateSystem.States;
using UnityEngine;

namespace SYBO.Subway.StateSystem
{
	public class GameStateMachine : StateMachine
	{
		private static GameStateMachine _instance;

		public const int MenuStateId = 0;

		public const int EnterPlayStateId = 1;

		public const int RunningStateId = 2;

		public const int HeadstartStateId = 3;

		public const int JetpackStateId = 4;

		public const int PogostickStateId = 5;

		public const int ReturnToCheckpointStateId = 6;

		public const int DieWhenGroundedStateId = 7;

		public const int DeathStateId = 8;

		public const int MenuTriggerId = 100;

		public const int EnterPlayTriggerId = 101;

		public const int RunningTriggerId = 102;

		public const int HeadstartTriggerId = 103;

		public const int JetpackTriggerId = 104;

		public const int PogostickTriggerId = 105;

		public const int ReturnToCheckpointTriggerId = 106;

		public const int DieWhenGroundedTriggerId = 107;

		public const int DeathTriggerId = 108;

		public static GameStateMachine Instance
		{
			get
			{
				if (_instance == null)
				{
					GenerateStateMachine();
				}
				return _instance;
			}
		}

		private GameStateMachine(IScheduler scheduler)
			: base(scheduler)
		{
		}

		public static void Initialize()
		{
			if (_instance == null)
			{
				GenerateStateMachine();
			}
			_instance.EnterInitialState(0);
		}

		public static void GenerateStateMachine()
		{
			if (_instance == null)
			{
				MonobehaviourScheduler scheduler = new GameObject("StateMachineScheduler").AddComponent<MonobehaviourScheduler>();
				_instance = new GameStateMachine(scheduler);
				_instance.AddState(new MenuState(0));
				_instance.AddState(new EnterPlayState(1));
				_instance.AddState(new RunningState(2));
				_instance.AddState(new HeadstartState(3));
				_instance.AddState(new JetpackState(4));
				_instance.AddState(new PogostickState(5));
				_instance.AddState(new ReturnToCheckpointState(6));
				_instance.AddState(new DieWhenGroundedState(7));
				_instance.AddState(new DeathState(8));
				_instance.AddTransition(0, 1, 101);
				_instance.AddTransition(1, 0, 100);
				_instance.AddTransition(1, 2, 102);
				_instance.AddTransition(2, 3, 103);
				_instance.AddTransition(3, 2, 102);
				_instance.AddTransition(3, 0, 100);
				_instance.AddTransition(2, 4, 104);
				_instance.AddTransition(4, 2, 102);
				_instance.AddTransition(4, 0, 100);
				_instance.AddTransition(2, 5, 105);
				_instance.AddTransition(5, 2, 102);
				_instance.AddTransition(5, 0, 100);
				_instance.AddTransition(2, 6, 106);
				_instance.AddTransition(6, 2, 102);
				_instance.AddTransition(6, 0, 100);
				_instance.AddTransition(2, 0, 100);
				_instance.AddTransition(2, 7, 107);
				_instance.AddTransition(7, 8, 108);
				_instance.AddTransition(8, 2, 102);
				_instance.AddTransition(8, 0, 100);
			}
		}

		public bool IsInGame()
		{
			return IsCurrentState<InGameState>();
		}

		public void HandleSwipe(SwipeDir swipeDir)
		{
			InGameState inGameState = TryGetCurrentState<InGameState>();
			if (inGameState != null)
			{
				inGameState.HandleSwipe(swipeDir);
			}
		}

		public void HandleDoubleTap()
		{
			InGameState inGameState = TryGetCurrentState<InGameState>();
			if (inGameState != null)
			{
				inGameState.HandleDoubleTap();
			}
		}

		public void HandleCriticalHit()
		{
			InGameState inGameState = TryGetCurrentState<InGameState>();
			if (inGameState != null)
			{
				inGameState.HandleCriticalHit();
			}
		}
	}
}
