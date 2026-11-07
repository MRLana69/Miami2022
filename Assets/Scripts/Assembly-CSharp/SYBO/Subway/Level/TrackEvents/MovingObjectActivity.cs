using SYBO.Components.Utilities;
using SYBO.Subway.Placeholders.Lifecycles;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	public class MovingObjectActivity : Activity
	{
		[SerializeField]
		private PlaceholderLifecycle _movingObject;

		private float _speed;

		private float _desiredSpeed;

		private Transform _initialParent;

		private RouteChunk _routeChunk;

		public float Speed
		{
			get
			{
				return _speed;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public override void Prepare(IActivityInitializer initializer)
		{
			CheckInitialize();
			IMoveObjectActivityInitializer moveObjectActivityInitializer = (IMoveObjectActivityInitializer)initializer;
			_initialParent = moveObjectActivityInitializer.Transform;
			_transform.parent = moveObjectActivityInitializer.Transform;
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = Quaternion.identity;
			_desiredSpeed = moveObjectActivityInitializer.Speed;
			_speed = _desiredSpeed;
			_routeChunk = moveObjectActivityInitializer.RouteChunk;
			_routeChunk.OnDeactivate += RouteChunk_OnDeactivate;
			_movingObject.Spawn();
		}

		protected override void Execute()
		{
			UpdateCurrentChunk();
			_transform.localPosition += _speed * Vector3.forward * Time.deltaTime;
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

		public override void Stop()
		{
			if (!_initialized)
			{
				SDebug.LogError("Stop called on uninitialized MovingCarActivity");
				return;
			}
			base.Stop();
			_routeChunk.OnDeactivate -= RouteChunk_OnDeactivate;
			_movingObject.Despawn();
			PoolManager.Despawn(_transform);
		}

		private void RouteChunk_OnDeactivate()
		{
			if (_running)
			{
				Stop();
			}
		}
	}
}
