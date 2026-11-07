using SYBO.Common;
using SYBO.Components.Utilities;
using SYBO.Subway.Level.TrackEvents.CarActivities;
using SYBO.Subway.Placeholders.Lifecycles;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class MovingCarActivity : Activity
	{
		[SerializeField]
		private PlaceholderLifecycle _carPlaceholderLifecycle;

		[SerializeField]
		private CollisionNotifier _brakingTrigger;

		private ITurnable _turnableBehaviour;

		private float _speed;

		private float _desiredSpeed;

		private float _brakeStartSpeed;

		private float _brakeStartDistance;

		private bool _braking;

		private bool _carAheadMoveOpposite;

		private MovingCarActivity _carAhead;

		private Transform _carAheadTransforn;

		private BoxCollider _collider;

		private RouteChunk _routeChunk;

		public float Speed
		{
			get
			{
				return _speed;
			}
		}

		public float Acceleration { get; set; }

		protected override void Initialize()
		{
			base.Initialize();
			Acceleration = 1f;
			_collider = GetComponent<Collider>() as BoxCollider;
			_brakingTrigger.TriggerEnter += BrakingTrigger_OnEnter;
			_brakingTrigger.TriggerExit += BrakingTrigger_OnExit;
			_turnableBehaviour = base.gameObject.GetComponent<ITurnable>();
			if (_turnableBehaviour == null)
			{
				_turnableBehaviour = base.gameObject.AddComponent<LerpedTurn>();
			}
			_turnableBehaviour.Init(this);
		}

		public override void Prepare(IActivityInitializer initializer)
		{
			CheckInitialize();
			IMoveObjectActivityInitializer moveObjectActivityInitializer = (IMoveObjectActivityInitializer)initializer;
			_transform.parent = moveObjectActivityInitializer.Transform;
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = Quaternion.identity;
			_desiredSpeed = moveObjectActivityInitializer.Speed;
			_speed = _desiredSpeed;
			_routeChunk = moveObjectActivityInitializer.RouteChunk;
			_routeChunk.OnDeactivate += RouteChunk_OnDeactivate;
			_running = false;
			_braking = false;
			_turnableBehaviour.Reset();
			_carPlaceholderLifecycle.Spawn();
		}

		protected override void Execute()
		{
			UpdateCurrentChunk();
			_turnableBehaviour.Turn();
			if (_braking)
			{
				Brake();
			}
			_transform.position += _speed * _transform.forward * Time.deltaTime * Acceleration;
		}

		private void UpdateCurrentChunk()
		{
			RouteChunk routeChunk = null;
			if (GamePositionRoot.Instance.GetGamePosition(_transform).z < _routeChunk.GamePositionStartZ)
			{
				routeChunk = _routeChunk.Previous;
			}
			else
			{
				if (!(_routeChunk.GamePositionEnd.z < GamePositionRoot.Instance.GetGamePosition(_transform).z))
				{
					return;
				}
				routeChunk = _routeChunk.Next;
			}
			if (routeChunk != null)
			{
				_routeChunk.OnDeactivate -= RouteChunk_OnDeactivate;
				_routeChunk = routeChunk;
				if (routeChunk.IsBuilt)
				{
					_transform.parent = _routeChunk.transform;
					_routeChunk.OnDeactivate += RouteChunk_OnDeactivate;
				}
				else
				{
					Stop();
				}
			}
		}

		private void OnTriggerEnter(Collider trigger)
		{
			if (trigger.CompareTag("TrackEventTrigger"))
			{
				_turnableBehaviour.StartTurning(trigger);
			}
		}

		public override void Stop()
		{
			if (!_initialized)
			{
				SDebug.LogError("Stop called on uninitialized MovingCarActivity");
				return;
			}
			base.Stop();
			_braking = false;
			_carAhead = null;
			_turnableBehaviour.Reset();
			_brakingTrigger.ClearFrame();
			_routeChunk.OnDeactivate -= RouteChunk_OnDeactivate;
			_carPlaceholderLifecycle.Despawn();
			PoolManager.Despawn(_transform);
		}

		private void Brake()
		{
			if (!_carAhead._running)
			{
				_braking = false;
			}
			Vector3 a = GapStartPoint();
			Vector3 b = GapEndPoint();
			float num = Vector3.Distance(a, b);
			_speed = Mathf.Lerp(_carAhead.Speed, _brakeStartSpeed, Mathf.Clamp01(num / _brakeStartDistance));
			if (_speed < 1f)
			{
				_speed = 0f;
				_braking = false;
			}
			else if (_speed - _carAhead.Speed < 1f || num < 1f)
			{
				_speed = _carAhead.Speed;
			}
		}

		private Vector3 GapStartPoint()
		{
			return _transform.position + new Vector3(0f, 0f, _collider.center.z) + _transform.forward * 0.5f * (_collider.size.z + _collider.size.x);
		}

		private Vector3 GapEndPoint()
		{
			float num = ((!_carAheadMoveOpposite) ? (-1f) : 1f);
			return _carAhead._transform.position + num * _carAhead._transform.forward * _collider.size.z * 0.5f;
		}

		private void RouteChunk_OnDeactivate()
		{
			if (_running)
			{
				Stop();
			}
		}

		private void BrakingTrigger_OnEnter(Collider otherCollider)
		{
			MovingCarActivity componentInParent = otherCollider.GetComponentInParent<MovingCarActivity>();
			if (componentInParent != null && !(componentInParent == this))
			{
				_carAhead = componentInParent;
				_carAheadTransforn = componentInParent.transform;
				_carAheadMoveOpposite = Vector3.Dot(_transform.forward, _carAheadTransforn.forward) < 0f;
				_braking = true;
				_brakeStartDistance = Vector3.Distance(GapStartPoint(), GapEndPoint());
				_brakeStartSpeed = _speed;
			}
		}

		private void BrakingTrigger_OnExit(Collider otherCollider)
		{
			MovingCarDirectionChange componentInParent = otherCollider.GetComponentInParent<MovingCarDirectionChange>();
			MovingCarActivity componentInParent2 = otherCollider.GetComponentInParent<MovingCarActivity>();
			if (componentInParent == null && componentInParent2 != this && componentInParent2 != null)
			{
				_braking = false;
			}
		}
	}
}
