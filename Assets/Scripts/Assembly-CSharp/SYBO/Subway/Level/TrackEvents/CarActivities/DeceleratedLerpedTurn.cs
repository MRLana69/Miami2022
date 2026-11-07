using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents.CarActivities
{
	public class DeceleratedLerpedTurn : LerpedTurn
	{
		public override void Turn()
		{
			base.Turn();
			if (_turning)
			{
				_movingCarActivity.Acceleration = 1f - Mathf.PingPong(_colliderPassProgress, 0.5f);
			}
		}
	}
}
