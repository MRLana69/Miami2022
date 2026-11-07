namespace SYBO.Common.StateSystem
{
	public class State
	{
		public readonly int Id;

		public State(int id)
		{
			Id = id;
		}

		public virtual void Enter(State fromState)
		{
		}

		public virtual void Leave(State toState)
		{
		}

		public virtual void OnUpdate()
		{
		}

		public virtual void OnLateUpdate()
		{
		}
	}
}
