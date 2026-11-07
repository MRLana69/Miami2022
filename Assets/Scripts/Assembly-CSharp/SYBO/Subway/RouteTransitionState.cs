using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway
{
	public class RouteTransitionState : MonoBehaviour, IGameState
	{
		protected Route _sourceRoute;

		protected RouteType _targetRouteType;

		public bool IsCompleted { get; private set; }

		public bool IsReadyToEnter { get; private set; }

		public void Prepare(Route sourceRoute, RouteType targetRouteType)
		{
			_sourceRoute = sourceRoute;
			_targetRouteType = targetRouteType;
			IsReadyToEnter = true;
			IsCompleted = false;
		}

		public virtual void Initialize()
		{
		}

		public virtual void Enter()
		{
			IsReadyToEnter = false;
		}

		public virtual void UpdateState()
		{
		}

		public virtual void Exit()
		{
		}
	}
}
