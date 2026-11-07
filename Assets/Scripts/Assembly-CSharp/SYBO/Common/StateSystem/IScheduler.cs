using System;

namespace SYBO.Common.StateSystem
{
	public interface IScheduler
	{
		void RegisterUpdateHandler(Action handler);

		void RegisterLateUpdateHandler(Action handler);
	}
}
