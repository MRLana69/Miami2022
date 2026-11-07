using System.Collections.Generic;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(TrackObject))]
	[RequireComponent(typeof(BoxCollider))]
	public class MovingCarTrackEvent : CharacterTriggeredTrackEvent, IMoveObjectActivityInitializer, IActivityInitializer
	{
		[SerializeField]
		private MovingCarActivity _activityPrefab;

		private Transform _activityPrefabTransform;

		[Range(0f, 1000f)]
		[SerializeField]
		private float _speed = 200f;

		[SerializeField]
		private bool _randomize;

		[Tooltip("Probability of spawning car(y) depending on a player speed(x)")]
		[SerializeField]
		private AnimationCurve spawnProbability;

		[Tooltip("Used as the max speed value when randomizing speed")]
		[Range(0f, 1000f)]
		[SerializeField]
		private float _speedMax = 400f;

		private List<Collider> _collidersInSpawnArea = new List<Collider>();

		Transform IMoveObjectActivityInitializer.Transform
		{
			get
			{
				return _transform;
			}
		}

		public RouteChunk RouteChunk { get; private set; }

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

		private void OnTriggerEnter(Collider otherCollider)
		{
			_collidersInSpawnArea.Add(otherCollider);
		}

		private void OnTriggerExit(Collider otherCollider)
		{
			_collidersInSpawnArea.Remove(otherCollider);
		}

		protected override void Launch()
		{
			RouteChunk = GetComponentInParent<RouteChunk>();
			if (!(0f < (float)_collidersInSpawnArea.Count) && !(RouteChunk == null) && ShouldSpawn())
			{
				Transform transform = PoolManager.Spawn(_activityPrefabTransform);
				MovingCarActivity component = transform.GetComponent<MovingCarActivity>();
				component.Prepare(this);
				component.Run();
			}
		}

		protected override void TrackObject_OnActivate()
		{
			_collidersInSpawnArea.Clear();
			Character instance = Character.Instance;
			if (instance != null && _trigger.EndPosition < instance.TransformPosition.z && !_launched)
			{
				Launch();
			}
		}

		private bool ShouldSpawn()
		{
			float value = Random.value;
			float num = spawnProbability.Evaluate((Game.Instance.CurrentSpeed - Game.Instance.speed.min) / (Game.Instance.speed.max - Game.Instance.speed.min));
			return num >= value;
		}
	}
}
