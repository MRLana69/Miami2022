using System;
using SYBO.Common.StateSystem;
using SYBO.Common.Utility;

namespace SYBO.Subway.StateSystem
{
	public class MonobehaviourScheduler : SubscribableMonobehaviour, IScheduler
	{
		public void RegisterUpdateHandler(Action handler)
		{
			OnUpdate = (Action)Delegate.Combine(OnUpdate, handler);
		}

		public void RegisterLateUpdateHandler(Action handler)
		{
			OnLateUpdate = (Action)Delegate.Combine(OnLateUpdate, handler);
		}
	}
}
