namespace SYBO.Subway
{
	public interface IGameState
	{
		bool IsReadyToEnter { get; }

		bool IsCompleted { get; }

		void UpdateState();

		void Initialize();

		void Enter();

		void Exit();
	}
}
