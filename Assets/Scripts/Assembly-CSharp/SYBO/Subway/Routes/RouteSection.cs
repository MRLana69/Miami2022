using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SYBO.Common.Exceptions;
using SYBO.Common.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	public class RouteSection : MonoBehaviour
	{
		[Serializable]
		public class RepeatConfiguration
		{
			[SerializeField]
			private int _sectionsBetweenRepeats = 1;

			private int _remainingSectionsBetweenRepeats;

			[SerializeField]
			private int _distanceBetweenRepeats;

			private int _lastSpawnZPosition;

			[SerializeField]
			private List<RouteSection> _doNotSpawnAfter = new List<RouteSection>();

			public int RemainingSectionsBetweenRepeat
			{
				get
				{
					return _remainingSectionsBetweenRepeats;
				}
			}

			public void Reset()
			{
				_remainingSectionsBetweenRepeats = 0;
				_lastSpawnZPosition = 0;
			}

			public bool CanSpawnAtPositionZ(int posZ, int routeSpecificDistanceBetweenRepeats)
			{
				return _lastSpawnZPosition == 0 || posZ >= _lastSpawnZPosition + Mathf.Max(_distanceBetweenRepeats, routeSpecificDistanceBetweenRepeats);
			}

			public bool CanSpawnAfter(RouteSection previousSection)
			{
				return !_doNotSpawnAfter.Contains(previousSection);
			}

			public void NotifySectionSpawn(int posZ, int maxLength)
			{
				_remainingSectionsBetweenRepeats = _sectionsBetweenRepeats;
				_lastSpawnZPosition = posZ + maxLength;
			}

			public void UpdateRepeatConfiguration()
			{
				if (_remainingSectionsBetweenRepeats > 0)
				{
					_remainingSectionsBetweenRepeats--;
				}
			}

			public void Validate(RouteSection mySection)
			{
				_sectionsBetweenRepeats = Mathf.Max(_sectionsBetweenRepeats, 0);
				_distanceBetweenRepeats = Mathf.Max(_distanceBetweenRepeats, 0);
				foreach (RouteSection item in _doNotSpawnAfter)
				{
					if (item == null)
					{
						SDebug.LogError("Null section in repeat configuration of " + Utils.GetLongName(mySection.transform));
					}
				}
			}
		}

		[Header("Spawn Conditions")]
		[SerializeField]
		private int _zMinimum;

		[SerializeField]
		private bool _zMaximumActive;

		[SerializeField]
		private int _zMaximum;

		[SerializeField]
		private int _minSpawnOrder;

		[SerializeField]
		private RepeatConfiguration _repeatConfiguration;

		[Header("Appearance")]
		[SerializeField]
		private bool _middleChunkCountLimited = true;

		[SerializeField]
		[Range(1f, 50f)]
		private int _middleChunkCountMin = 1;

		[SerializeField]
		[Range(1f, 50f)]
		private int _middleChunkCountMax = 1;

		[SerializeField]
		protected WeightRandomizedRouteChunkContainter _startChunks = new WeightRandomizedRouteChunkContainter();

		[SerializeField]
		protected WeightRandomizedRouteChunkContainter _midChunks = new WeightRandomizedRouteChunkContainter();

		[SerializeField]
		protected WeightRandomizedRouteChunkContainter _endChunks = new WeightRandomizedRouteChunkContainter();

		[Header("Data Extracted From Chunks")]
		[SerializeField]
		private List<EnvironmentKind> _limitedAllowedEnvironmentKinds = new List<EnvironmentKind>();

		[SerializeField]
		private List<EnvironmentKind> _forcedEnvironmentKinds = new List<EnvironmentKind>();

		[SerializeField]
		private int _lengthMin;

		[SerializeField]
		private int _lengthMax;

		private int _lengthMinPrev;

		private int _lengthMaxPrev;

		private int _middleChunkCountMinPrev;

		private int _middleChunkCountMaxPrev;

		private RouteBuildProgress _progressState;

		private WeightRandomizedRouteChunkContainter _filteredChunks = new WeightRandomizedRouteChunkContainter();

		private int _midChunksCountUsed;

		private int _midChunksCountTarget;

		private RouteBuilder _builder;

		private List<RouteChunk> _pickedChunks = new List<RouteChunk>();

		private ReadOnlyCollection<RouteChunk> _pickedChunksReadOnly;

		protected List<RouteChunk> _spawnedChunks = new List<RouteChunk>();

		public List<EnvironmentKind> ForcedEnvironmentKinds
		{
			get
			{
				return _forcedEnvironmentKinds;
			}
		}

		public bool IsDone
		{
			get
			{
				return _progressState == RouteBuildProgress.Done;
			}
		}

		public int ZMinimum
		{
			get
			{
				return _zMinimum;
			}
		}

		public int ZMaximum
		{
			get
			{
				return _zMaximum;
			}
		}

		public bool ZMaximumActive
		{
			get
			{
				return _zMaximumActive;
			}
		}

		public int MinSpawnOrder
		{
			get
			{
				return _minSpawnOrder;
			}
		}

		public int LengthMin
		{
			get
			{
				return _lengthMin;
			}
		}

		public int LengthMax
		{
			get
			{
				return _lengthMax;
			}
		}

		public ReadOnlyCollection<RouteChunk> PickedChunks
		{
			get
			{
				if (_pickedChunksReadOnly == null)
				{
					_pickedChunksReadOnly = _pickedChunks.AsReadOnly();
				}
				return _pickedChunksReadOnly;
			}
		}

		public List<RouteChunk> SpawnedChunks
		{
			get
			{
				return _spawnedChunks;
			}
		}

		public WeightRandomizedRouteChunkContainter StartChunks
		{
			get
			{
				return _startChunks;
			}
		}

		public WeightRandomizedRouteChunkContainter MidChunks
		{
			get
			{
				return _midChunks;
			}
		}

		public WeightRandomizedRouteChunkContainter EndChunks
		{
			get
			{
				return _endChunks;
			}
		}

		public int SpawnedChunkCount
		{
			get
			{
				return _spawnedChunks.Count;
			}
		}

		public void Reset(RouteBuilder builder)
		{
			_builder = builder;
			_startChunks.SetPickEvaluator(CanSpawnChunk);
			_midChunks.SetPickEvaluator(CanSpawnChunk);
			_endChunks.SetPickEvaluator(CanSpawnChunk);
			_filteredChunks.SetPickEvaluator(CanSpawnChunk);
			_filteredChunks.Clear();
			_pickedChunks.Clear();
			_spawnedChunks.Clear();
			_midChunksCountUsed = 0;
			_midChunksCountTarget = UnityEngine.Random.Range(_middleChunkCountMin, _middleChunkCountMax + 1);
			_progressState = RouteBuildProgress.Start;
			WeightRandomizedRouteChunkContainter progressStateChunks = GetProgressStateChunks(_progressState);
			if (progressStateChunks == null || progressStateChunks.Count == 0)
			{
				IncrementProgressState();
			}
		}

		public bool SupportsEnvironment(EnvironmentKind envKind)
		{
			return _limitedAllowedEnvironmentKinds.Count == 0 || _limitedAllowedEnvironmentKinds.Contains(envKind);
		}

		public RepeatConfiguration GetRepeatConfiguration()
		{
			return _repeatConfiguration;
		}

		public RouteChunk GetRandomMidChunk(SyboRandom random)
		{
			RouteChunk routeChunk = _midChunks.PickSemiRandomlyOrderedPrefab(random);
			_pickedChunks.Add(routeChunk);
			return routeChunk;
		}

		public RouteChunk GetNextChunk(RouteBuilder builder, SyboRandom random)
		{
			WeightRandomizedRouteChunkContainter progressStateChunks = GetProgressStateChunks(_progressState);
			RouteChunk routeChunk = progressStateChunks.PickSemiRandomlyOrderedPrefab(random);
			if (routeChunk == null)
			{
				routeChunk = builder.FallbackRouteChunk;
				SDebug.LogError("RouteSection " + base.name + ": No compatible chunks at " + builder.SpawnedLeadingEdgeGamePositionZ + ". Chunk repeatability disabled? Use generic fallback " + routeChunk, this);
			}
			if (_progressState == RouteBuildProgress.Mid)
			{
				_midChunksCountUsed++;
			}
			if (routeChunk != null)
			{
				IncrementProgressState();
			}
			_pickedChunks.Add(routeChunk);
			return routeChunk;
		}

		public void AddSpawnedChunk(RouteChunk chunk)
		{
			_spawnedChunks.Add(chunk);
		}

		public void RemoveSpawnedChunk(RouteChunk chunk)
		{
			_spawnedChunks.Remove(chunk);
		}

		public RouteChunk GetSpawnedChunk(int index)
		{
			return (index >= _spawnedChunks.Count) ? null : _spawnedChunks[index];
		}

		public bool HasChunk(RouteChunk chunk)
		{
			return _startChunks.Contains(chunk) || _midChunks.Contains(chunk) || _endChunks.Contains(chunk);
		}

		public IRouteTransitionTrigger GetTransitionTrigger()
		{
			IRouteTransitionTrigger routeTransitionTrigger = null;
			foreach (RouteChunk spawnedChunk in _spawnedChunks)
			{
				if (routeTransitionTrigger == null)
				{
					routeTransitionTrigger = spawnedChunk.RouteTransitionTrigger;
				}
				else if (spawnedChunk.RouteTransitionTrigger != null)
				{
					SDebug.LogErrorFormat(this, "{0}: Found more than one RouteTransitionTrigger. Returning first.", base.name);
				}
			}
			if (routeTransitionTrigger == null)
			{
				SDebug.LogErrorFormat(this, "{0}: Couldn't find a RouteTransitionTrigger", base.name);
			}
			return routeTransitionTrigger;
		}

		public override string ToString()
		{
			return base.name;
		}

		private bool CanSpawnChunk(RouteChunk chunk)
		{
			if (!chunk.IsRepeatableInSections && _pickedChunks.Contains(chunk))
			{
				return false;
			}
			return _builder.CanSpawnChunk(chunk);
		}

		private void IncrementProgressState()
		{
			while (_progressState != RouteBuildProgress.Done)
			{
				WeightRandomizedRouteChunkContainter progressStateChunks = GetProgressStateChunks(_progressState);
				bool flag = progressStateChunks.Count != 0;
				if (_progressState == RouteBuildProgress.Mid && flag && (!_middleChunkCountLimited || _midChunksCountUsed < _midChunksCountTarget))
				{
					break;
				}
				_progressState++;
				if (_progressState == RouteBuildProgress.Done)
				{
					break;
				}
				WeightRandomizedRouteChunkContainter progressStateChunks2 = GetProgressStateChunks(_progressState);
				if (progressStateChunks2.Count != 0)
				{
					break;
				}
			}
		}

		private WeightRandomizedRouteChunkContainter GetProgressStateChunks(RouteBuildProgress progressState)
		{
			switch (progressState)
			{
			case RouteBuildProgress.Start:
				return _startChunks;
			case RouteBuildProgress.Mid:
				return _midChunks;
			case RouteBuildProgress.End:
				return _endChunks;
			default:
				throw new SwitchDefaultException(progressState.ToString());
			}
		}
	}
}
