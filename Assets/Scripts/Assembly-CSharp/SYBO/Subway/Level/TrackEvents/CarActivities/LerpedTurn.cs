using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents.CarActivities
{
	public class LerpedTurn : MonoBehaviour, ITurnable
	{
		protected MovingCarActivity _movingCarActivity;

		protected bool _turning;

		protected float _colliderPassProgress;

		private Transform _transform;

		private Collider _turnTrigger;

		private Transform _turnTarget;

		private Quaternion _turnStartRotation;

		private float _distanceStartToTarget;

		private Vector3 _vectStartToTarget;

		private Vector3 _dirToTarget;

		private Vector3 _turnStartPoint;

		private Vector3 _turnTargetDirectionForward;

		private Quaternion _turnTargetRotation;

		private Vector3 _turnHalfWayDirectionForward;

		private Vector3 _turnHalfWayDirectionUp;

		private Quaternion _turnHalfWayRotation;

		public void Init(MovingCarActivity movingCarActivity)
		{
			_transform = movingCarActivity.transform;
			_movingCarActivity = movingCarActivity;
		}

		public void Reset()
		{
			_turnTrigger = null;
			_turning = false;
		}

		public void StartTurning(Collider trigger)
		{
			MovingCarDirectionChange component = trigger.GetComponent<MovingCarDirectionChange>();
			if (component != null && !(_turnTrigger == trigger) && !_turning)
			{
				_turning = true;
				_turnTrigger = trigger;
				_turnTarget = component.TargetDirection;
				_turnStartPoint = _transform.position;
				_turnStartRotation = _transform.rotation;
				_turnTargetDirectionForward = _turnTarget.forward;
				_turnTargetRotation = _turnTarget.rotation;
				_vectStartToTarget = _turnTarget.position - _transform.position;
				_dirToTarget = _vectStartToTarget.normalized;
				_distanceStartToTarget = _vectStartToTarget.magnitude;
				_turnHalfWayDirectionUp = (_transform.up + _turnTarget.up).normalized;
				float num = Quaternion.Angle(_transform.rotation, _turnTarget.rotation);
				bool flag = 90f < num;
				bool flag2 = Vector3.Dot(_transform.forward, _vectStartToTarget.normalized) < 0f;
				if (flag && flag2)
				{
					_turnHalfWayDirectionForward = _dirToTarget;
					_turnHalfWayRotation = Quaternion.LookRotation(_turnHalfWayDirectionForward, _turnHalfWayDirectionUp);
				}
				else
				{
					_turnHalfWayDirectionForward = (_dirToTarget - 0.2f * _turnTargetDirectionForward).normalized;
					_turnHalfWayRotation = Quaternion.LookRotation(_turnHalfWayDirectionForward, _turnHalfWayDirectionUp);
				}
			}
		}

		public virtual void Turn()
		{
			if (_turning)
			{
				Vector3 lhs = _transform.position - _turnStartPoint;
				_colliderPassProgress = Vector3.Dot(lhs, _dirToTarget) / _distanceStartToTarget;
				if (0.99f < _colliderPassProgress)
				{
					_colliderPassProgress = 1f;
				}
				if (_colliderPassProgress < 0.5f)
				{
					float t = _colliderPassProgress * 2f;
					_transform.rotation = Quaternion.Lerp(_turnStartRotation, _turnHalfWayRotation, t);
				}
				else
				{
					float t2 = (_colliderPassProgress - 0.5f) * 2f;
					_transform.rotation = Quaternion.Lerp(_turnHalfWayRotation, _turnTargetRotation, t2);
				}
				if (1f <= _colliderPassProgress)
				{
					_turning = false;
					_turnTrigger = null;
				}
			}
		}
	}
}
