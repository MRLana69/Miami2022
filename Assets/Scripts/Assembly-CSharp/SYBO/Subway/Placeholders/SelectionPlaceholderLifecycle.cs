using System.Collections.Generic;
using SYBO.Subway.Level;
using SYBO.Subway.Placeholders.Lifecycles;
using SYBO.Subway.Placeholders.PrefabProviders;
using UnityEngine;

namespace SYBO.Subway.Placeholders
{
	[RequireComponent(typeof(TrackObject))]
	[RequireComponent(typeof(Selector))]
	[RequireComponent(typeof(Placeholder))]
	public class SelectionPlaceholderLifecycle : PlaceholderLifecycle
	{
		[Tooltip("Only for placeholders in scene not handled by level generation")]
		[SerializeField]
		protected bool _spawnOnceOnStart;

		[SerializeField]
		[Range(0f, 1f)]
		private float _spawnProbability = 1f;

		[Tooltip("For placeholders in Merge scene Intro")]
		[SerializeField]
		private bool _locatedInIntro;

		[Tooltip("Allows avoiding populating placeholders when their parents are used in Intro")]
		[SerializeField]
		private bool _dontSpawnInIntro;

		private Selector _selector;

		private TrackObject _trackObject;

		public bool SpawnOnceOnStart
		{
			get
			{
				return _spawnOnceOnStart;
			}
		}

		public float SpawnProbability
		{
			get
			{
				return _spawnProbability;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			_selector = GetComponent<Selector>();
			_selector.PerformingSelection += Selector_OnPerformingSelection;
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			_trackObject.OnDeactivate += TrackObject_OnDeactivate;
		}

		protected void Start()
		{
			if (_spawnOnceOnStart && Application.isPlaying)
			{
				SpawnInstances(true, true, _locatedInIntro);
			}
		}

		private void OnDestroy()
		{
			if (_selector != null)
			{
				_selector.PerformingSelection -= Selector_OnPerformingSelection;
			}
			if (_trackObject != null)
			{
				_trackObject.OnActivate -= TrackObject_OnActivate;
				_trackObject.OnDeactivate -= TrackObject_OnDeactivate;
			}
		}

		private void SpawnInstances(bool spawnInChildrenLifecycles = false, bool spawningOnStart = false, bool locatedInIntro = false)
		{
			if ((!locatedInIntro || !_dontSpawnInIntro) && !(_spawnProbability < Random.value))
			{
				_prefabProvider.Evaluate();
				SpawnInstances(_prefabProvider, spawnInChildrenLifecycles, spawningOnStart, locatedInIntro);
			}
		}

		protected virtual void SpawnInstances(PlaceholderPrefabProviderBase prefabProvider, bool spawnInChildrenLifecycles = false, bool spawningOnStart = false, bool locatedInIntro = false)
		{
			_placeholder.SpawnInstances(prefabProvider);
			if (!spawnInChildrenLifecycles)
			{
				return;
			}
			SelectionPlaceholderLifecycle[] componentsInChildren = GetComponentsInChildren<SelectionPlaceholderLifecycle>(true);
			SelectionPlaceholderLifecycle[] array = componentsInChildren;
			foreach (SelectionPlaceholderLifecycle selectionPlaceholderLifecycle in array)
			{
				if (!(selectionPlaceholderLifecycle == this))
				{
					selectionPlaceholderLifecycle.SpawnInstances(spawnInChildrenLifecycles, spawningOnStart, locatedInIntro);
				}
			}
		}

		protected virtual void GetSpawnedObjects(List<Selector> selectors)
		{
			_placeholder.GetSpawnedObjects(selectors);
		}

		protected virtual void TrackObject_OnActivate()
		{
			Activate();
		}

		protected virtual void TrackObject_OnDeactivate()
		{
			Deactivate();
			if (!_spawnOnceOnStart)
			{
				Despawn();
			}
		}

		private void Selector_OnPerformingSelection(List<Selector> selectors)
		{
			if (!_spawnOnceOnStart)
			{
				SpawnInstances();
				GetSpawnedObjects(selectors);
			}
		}
	}
}
