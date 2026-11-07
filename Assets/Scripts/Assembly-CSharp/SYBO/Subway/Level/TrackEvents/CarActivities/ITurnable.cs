using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents.CarActivities
{
	public interface ITurnable
	{
		void Init(MovingCarActivity movingCarActivity);

		void Turn();

		void StartTurning(Collider trigger);

		void Reset();
	}
}
