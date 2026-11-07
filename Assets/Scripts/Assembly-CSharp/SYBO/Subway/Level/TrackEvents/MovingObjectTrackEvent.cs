using SYBO.Components.Utilities;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	public class MovingObjectTrackEvent : TrackEvent, IMoveObjectActivityInitializer, IActivityInitializer
	{
		[SerializeField]
		private MovingObjectActivity _activityPrefab;

		private Transform _activityPrefabTransform;

		[Range(0f, 1000f)]
		[SerializeField]
		private float _speed = 200f;

		[SerializeField]
		private bool _randomize;

		[Tooltip("Used as the max speed value when randomizing speed")]
		[Range(0f, 1000f)]
		[SerializeField]
		private float _speedMax = 400f;

		private RouteChunk _routeChunk;

		Transform IMoveObjectActivityInitializer.Transform
		{
			get
			{
				return _transform;
			}
		}

		public RouteChunk RouteChunk
		{
			get
			{
				return _routeChunk;
			}
		}

		public float Speed
		{
			get
			{
				return (!_randomize) ? _speed : Random.Range(_speed, _speedMax);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			_activityPrefabTransform = _activityPrefab.transform;
		}

		protected override void Launch()
		{
			Transform transform = PoolManager.Spawn(_activityPrefabTransform);
			MovingObjectActivity component = transform.GetComponent<MovingObjectActivity>();
			_routeChunk = GetComponentInParent<RouteChunk>();
			component.Prepare(this);
			component.Run();
		}
	}
}
