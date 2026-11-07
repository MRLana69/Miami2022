using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SYBO.Common.Exceptions;
using SYBO.Common.Math;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups;
using SYBO.Subway.Routes.Builders.Worker;
using SYBO.Subway.Themes;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace SYBO.Subway.Routes.Builders
{
	[Serializable]
	public abstract class RouteBuilder
	{
		private struct BuildDirective
		{
			public readonly int GamePositionZ;

			public readonly bool IsApplicablePositionStrict;

			public readonly RouteSection Section;

			public readonly SectionSpawnMode SectionSpawnMode;

			public readonly ChunkBuildMode ChunkBuildMode;

			public bool HasSection
			{
				get
				{
					return Section != null;
				}
			}

			public BuildDirective(int gamePositionZ, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, bool isApplicablePositionStrict = true, RouteSection section = null)
			{
				GamePositionZ = gamePositionZ;
				Section = section;
				SectionSpawnMode = sectionSpawnMode;
				ChunkBuildMode = chunkBuildMode;
				IsApplicablePositionStrict = isApplicablePositionStrict;
			}

			public override string ToString()
			{
				return string.Format("BuildDirective at: {0} isStrict: {1} spawn: {2} build: {3} section: {4}", GamePositionZ, IsApplicablePositionStrict, SectionSpawnMode, ChunkBuildMode, Section);
			}
		}

		[Serializable]
		public class EnvironmentKindRepeatConfig
		{
			public EnvironmentKind EnvironmentKind = EnvironmentKind.None;

			[Range(1080f, 3600f)]
			public int RepeatDistance = 2160;

			public void Validate()
			{
				if (EnvironmentKind.IsRepeatable)
				{
					if (!EnvironmentKind.IsValidDensity(EnvironmentKind))
					{
						EnvironmentKind = EnvironmentKind.GetValidated();
					}
				}
				else
				{
					EnvironmentKind = EnvironmentKind.None;
				}
				RepeatDistance = RepeatDistance / 180 * 180;
			}
		}

		[Serializable]
		public class TransitionSectionInfo
		{
			public RouteType TransitionRouteType;

			public RouteSection Section;
		}

		private class EnvironmentRangeWrapper
		{
			public EnvironmentKind EnvironmentKind = EnvironmentKind.None;

			public float StartGamePositionZ;

			public float EndGamePositionZ;

			public EnvironmentRangeWrapper(EnvironmentKind kind, float startGamePositionZ, float endGamePositionZ)
			{
				EnvironmentKind = kind;
				StartGamePositionZ = startGamePositionZ;
				EndGamePositionZ = endGamePositionZ;
			}
		}

		private const int RouteChunksToBuildForEntry = 1;

		public const int CameraToBackgroundGameDistance = 1000;

		public const int DistanceToChunksSpawned = 2000;

		public const int DistanceToChunksBuildStart = 1200;

		public const int DistanceToChunksReady = 1100;

		public const int DistanceStartCleanupAfterCharacter = 170;

		public const int DistanceDoneCleanupAfterCharacter = 2000;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler OnCompletedLayingSections__BackingField;

		public Action<Route, RouteSection, float> OnAddedSection;

		public Action<Route, RouteChunk, float> OnRouteChunkSpawned;

		public static Action<Route, RouteSection, float> OnGlobalAddedSection;

		[SerializeField]
		internal RouteHeight _routeHeight;

		[SerializeField]
		private bool _midSectionsCountLimited;

		[SerializeField]
		private int _midSectionsCountTarget = 1;

		[SerializeField]
		protected List<TransitionSectionInfo> _startSectionPrefabs = new List<TransitionSectionInfo>();

		[FormerlySerializedAs("_midSectionPrefabsNew")]
		[SerializeField]
		protected WeightRandomizedRouteSectionContainter _midSectionPrefabs = new WeightRandomizedRouteSectionContainter();

		[SerializeField]
		private List<RouteSectionGroup> _midSectionGroupPrefabs = new List<RouteSectionGroup>();

		[SerializeField]
		protected List<TransitionSectionInfo> _endSectionPrefabs = new List<TransitionSectionInfo>();

		[SerializeField]
		private int _minDistanceBetweenSectionRepeats;

		[SerializeField]
		[Tooltip("A short section with single chunk supporting all environment to be used when need add generic filler")]
		internal RouteSection _fallbackSection;

		protected int _length;

		protected SectionSpawnMode _secionSpawnMode;

		protected ChunkBuildMode _chunkBuildMode;

		protected Route _route;

		protected Bounds _bounds;

		private List<RouteBuilder> _buildersAbove = new List<RouteBuilder>();

		private List<RouteBuilder> _buildersBelow = new List<RouteBuilder>();

		private RouteBuilder _builderForward;

		private List<RouteBuilder> _buildersOnSides = new List<RouteBuilder>();

		private PickupSpawnPoint _buildLimitingSpawnPoint;

		private List<RouteChunk> _chunks = new List<RouteChunk>();

		private int _chunksBuiltCount;

		protected int _spawnedChunkCount;

		private RouteSection _lastSpawnedSection;

		private RouteBuildProgress _sectionLayingProgress;

		private int _midSectionCountUsed;

		protected RouteManager _routeManager;

		protected bool _stopped;

		private bool _completedSectionLayingThisUpdate;

		protected List<RouteChunk> _chunksSpawnedThisUpdate = new List<RouteChunk>();

		protected List<RouteChunk> _chunksSpawnedDelayedBuild = new List<RouteChunk>();

		protected List<IConstrainingRouteBuilder> _constrainingBuilders = new List<IConstrainingRouteBuilder>();

		protected LogicAssetsManager _logicAssetManager;

		protected ThemeManager _themeManager;

		protected Vector3 _originGamePosition;

		protected RouteSection _currentSection;

		private List<RouteSection> _sections = new List<RouteSection>();

		private List<BuildDirective> _buildDirectives = new List<BuildDirective>();

		private RouteBuilderWorker _worker = new RouteBuilderWorker();

		private RouteChunk _lastBuiltChunk;

		private HashSet<Selector> _processedSelectors = new HashSet<Selector>();

		private LinkedList<EnvironmentRangeWrapper> _environmentWrapperQueue = new LinkedList<EnvironmentRangeWrapper>();

		private EnvironmentKind _lastSpawnedEnvironmentKind = EnvironmentKind.None;

		protected SyboRandom RandomGenerator { get; private set; }

		public RouteHeight RouteHeight
		{
			get
			{
				return _routeHeight;
			}
		}

		public int SpawnedLeadingEdgeGamePositionZ
		{
			get
			{
				return (int)(_originGamePosition.z + (float)_length);
			}
		}

		public int SpawnedLeadingEdgeWorldPositionZ
		{
			get
			{
				return (int)GamePositionRoot.Instance.GetWorldPositionZ(SpawnedLeadingEdgeGamePositionZ);
			}
		}

		public int BuiltLeadingEdgeGamePositionZ
		{
			get
			{
				return (!(_lastBuiltChunk == null)) ? ((int)_lastBuiltChunk.GamePositionEnd.z) : SpawnedLeadingEdgeGamePositionZ;
			}
		}

		public virtual int DistanceVisibleFromEntry
		{
			get
			{
				return 1080;
			}
		}

		public virtual int LastPositionVisibleFromEntry
		{
			get
			{
				return (int)(_originGamePosition.z + (float)DistanceVisibleFromEntry);
			}
		}

		public SectionSpawnMode SectionSpawnMode
		{
			get
			{
				return _secionSpawnMode;
			}
		}

		public ChunkBuildMode ChunkBuildMode
		{
			get
			{
				return _chunkBuildMode;
			}
		}

		public Route Route
		{
			get
			{
				return _route;
			}
		}

		public Bounds Bounds
		{
			get
			{
				return _bounds;
			}
		}

		public bool IsDone
		{
			get
			{
				return _stopped || (_sectionLayingProgress == RouteBuildProgress.Done && _chunksSpawnedDelayedBuild.Count == 0);
			}
		}

		public bool IsDoneLayingSections
		{
			get
			{
				return _stopped || _sectionLayingProgress == RouteBuildProgress.Done;
			}
		}

		public RouteChunk FallbackRouteChunk
		{
			get
			{
				return _fallbackSection.GetRandomMidChunk(RandomGenerator);
			}
		}

		public RouteChunk LastChunk
		{
			get
			{
				if (_chunks != null && 0 < _chunks.Count)
				{
					return _chunks[_chunks.Count - 1];
				}
				return null;
			}
		}

		private RouteSection LastSection
		{
			get
			{
				if (_sections != null && 0 < _sections.Count)
				{
					return _sections[_sections.Count - 1];
				}
				return null;
			}
		}

		public List<RouteBuilder> BuildersAbove
		{
			get
			{
				return _buildersAbove;
			}
		}

		public List<RouteBuilder> BuildersBelow
		{
			get
			{
				return _buildersBelow;
			}
		}

		public RouteBuilder BuilderForward
		{
			get
			{
				return _builderForward;
			}
		}

		public bool IsCleanedUp
		{
			get
			{
				return _chunks.Count == 0;
			}
		}

		public int SpawnedChunkCount
		{
			get
			{
				return _spawnedChunkCount;
			}
		}

		protected RouteBuildProgress SectionLayingProgress
		{
			get
			{
				return _sectionLayingProgress;
			}
		}

		public int MidSectionCountUsed
		{
			get
			{
				return _midSectionCountUsed;
			}
		}

		public int SectionCountUsed { get; private set; }

		public bool ReadyToAddSection
		{
			get
			{
				return _currentSection == null || _currentSection.IsDone;
			}
		}

		public virtual EnvironmentKind DesiredEnvironmentKind { get; protected set; }

		private EnvironmentKind PreviousDesiredEnvironmentKind { get; set; }

		private List<RouteBuilder> BuildersOnSides
		{
			get
			{
				_buildersOnSides.Clear();
				_buildersOnSides.AddRange(_buildersAbove);
				_buildersOnSides.AddRange(_buildersBelow);
				return _buildersOnSides;
			}
		}

		public List<IConstrainingRouteBuilder> ConstrainingBuilders
		{
			get
			{
				return _constrainingBuilders;
			}
		}

		protected int DistanceBuilt { get; private set; }

		public event EventHandler OnCompletedLayingSections
		{
			add
			{
				EventHandler eventHandler = OnCompletedLayingSections__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCompletedLayingSections__BackingField, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = OnCompletedLayingSections__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCompletedLayingSections__BackingField, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public RouteBuilder()
		{
			DesiredEnvironmentKind = EnvironmentKind.None;
			PreviousDesiredEnvironmentKind = EnvironmentKind.None;
		}

		public virtual void Initialize(RouteManager routeManager, Route route, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, RouteType previousRouteType, SyboRandom randomGenerator)
		{
			_route = route;
			RandomGenerator = randomGenerator;
			_routeManager = routeManager;
			_logicAssetManager = SingletonMonoBehaviour<LogicAssetsManager>.Instance;
			_themeManager = ThemeManager.Instance;
			InitializeSectionGroups();
			_midSectionPrefabs.RemoveEmptyEntries();
			_midSectionCountUsed = 0;
			SectionCountUsed = 0;
			_originGamePosition = route.OriginGamePosition;
			SetBuildMode(sectionSpawnMode, chunkBuildMode);
			CalculateBounds();
			_midSectionPrefabs.SetPickEvaluator(CanSpawnSection);
			foreach (WeightedRouteSection midSectionPrefab in _midSectionPrefabs)
			{
				midSectionPrefab.Prefab.GetRepeatConfiguration().Reset();
			}
			_lastSpawnedSection = null;
			_sectionLayingProgress = RouteBuildProgress.Start;
			RouteSection sectionPrefab = GetSectionPrefab(_sectionLayingProgress, previousRouteType);
			AddSection(sectionPrefab);
			RouteChunk.OnEnvironmentSegmentSpawned = (Action<EnvironmentKind, int, Route>)Delegate.Combine(RouteChunk.OnEnvironmentSegmentSpawned, new Action<EnvironmentKind, int, Route>(RouteChunk_OnEnvironmentSegmentSpawned));
		}

		private void InitializeSectionGroups()
		{
			foreach (RouteSectionGroup midSectionGroupPrefab in _midSectionGroupPrefabs)
			{
				foreach (WeightedRouteSection routeSection in midSectionGroupPrefab.RouteSections)
				{
					_midSectionPrefabs.Add(routeSection);
				}
			}
		}

		public virtual void Validate(Route route)
		{
			int num = 0;
			foreach (WeightedRouteSection midSectionPrefab in _midSectionPrefabs)
			{
				num += midSectionPrefab.Weight;
			}
			foreach (RouteSectionGroup midSectionGroupPrefab in _midSectionGroupPrefabs)
			{
				foreach (WeightedRouteSection routeSection in midSectionGroupPrefab.RouteSections)
				{
					num += routeSection.Weight;
				}
			}
			if (num <= 0)
			{
				SDebug.LogErrorFormat(Route, "{0}: Weight sum of all mid section is 0! At least one should have a weight!", route.name);
			}
		}

		public void SetBuildMode(SectionSpawnMode spawnMode, ChunkBuildMode chunkBuildMode)
		{
			_secionSpawnMode = spawnMode;
			_chunkBuildMode = chunkBuildMode;
		}

		public void StopBuilding()
		{
			_sectionLayingProgress = RouteBuildProgress.Done;
			_chunksSpawnedDelayedBuild.Clear();
			_stopped = true;
		}

		public void EnqueueStartSection(RouteType fromRouteType, int positionZ)
		{
			RouteSection sectionPrefab = GetSectionPrefab(RouteBuildProgress.Start, fromRouteType);
			EnqueueBuildDirective(positionZ, SectionSpawnMode.All, ChunkBuildMode.Normal, true, sectionPrefab);
		}

		internal void EnqueueBuildDirective(int gamePositionZ, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, bool isApplicablePositionStict = true, RouteSection section = null)
		{
			BuildDirective buildDirective = new BuildDirective(gamePositionZ, sectionSpawnMode, chunkBuildMode, isApplicablePositionStict, section);
			if (_buildDirectives.Count != 0)
			{
				BuildDirective buildDirective2 = _buildDirectives[_buildDirectives.Count - 1];
				if (gamePositionZ < buildDirective2.GamePositionZ)
				{
					SDebug.LogError(string.Format("{0} enqueuing a build directive before already existing one. Existing: [{0}] New: [{1}]", Route.name, buildDirective2, buildDirective), Route);
				}
			}
			if (isApplicablePositionStict && gamePositionZ % 180 != 0)
			{
				SDebug.LogError(string.Format("{0} enqueuing a build directive with position not multiple of MIN_CHUNK_LENGTH ({1}) - [{2}] ", Route.name, 180, buildDirective), Route);
			}
			bool flag = false;
			if ((float)gamePositionZ < GetSectionMinEndGamePosition(LastSection))
			{
				if (isApplicablePositionStict)
				{
					while (!LastSection.GetSpawnedChunk(0).IsBuilt && (float)gamePositionZ < GetSectionMinEndGamePosition(LastSection))
					{
						RouteSection lastSection = LastSection;
						do
						{
							RouteChunk routeChunk = lastSection.SpawnedChunks[lastSection.SpawnedChunks.Count - 1];
							_length -= routeChunk.Length;
							DespawnRouteChunk(routeChunk, true);
						}
						while (lastSection.SpawnedChunks.Count > 0);
						_sections.Remove(lastSection);
						PoolManager.Despawn(lastSection.transform);
					}
					_currentSection = _sections[_sections.Count - 1];
					float sectionMinEndGamePosition = GetSectionMinEndGamePosition(LastSection);
					if ((float)gamePositionZ < sectionMinEndGamePosition)
					{
						SDebug.LogError(string.Format("[{0}] Cancel enqueuing a strict build directive before end ({1}) of section {2} - [{3}] ", Route.name, sectionMinEndGamePosition, LastSection.name, buildDirective), Route);
						flag = true;
					}
				}
				else if (gamePositionZ < SpawnedLeadingEdgeGamePositionZ)
				{
					SDebug.LogError(string.Format("[{0}] enqueuing a build directive before end ({1}) of section {2} - [{3}]", Route.name, GetSectionMinEndGamePosition(LastSection), LastSection.name, buildDirective), Route);
				}
			}
			if (!flag)
			{
				_buildDirectives.Add(buildDirective);
				Vector3 vector = Vector3.forward * gamePositionZ;
				Vector3 end = vector + Vector3.up * 300f;
				Debug.DrawLine(vector, end, Color.red, 3000f);
			}
		}

		private float GetSectionMinEndGamePosition(RouteSection section)
		{
			return GamePositionRoot.Instance.GetGamePosition(section.transform).z + (float)section.LengthMin;
		}

		public float GetLastSectionStartGamePosition()
		{
			return GamePositionRoot.Instance.GetGamePosition(LastSection.transform).z;
		}

		public float GetLastSectionMinEndGamePosition()
		{
			return GetSectionMinEndGamePosition(LastSection);
		}

		public virtual void ConnectBuilder(RouteBuilder builderToConnect)
		{
			if (_routeHeight < builderToConnect.RouteHeight)
			{
				_buildersAbove.Add(builderToConnect);
				builderToConnect._buildersBelow.Add(this);
			}
			else if (builderToConnect.RouteHeight < _routeHeight)
			{
				_buildersBelow.Add(builderToConnect);
				builderToConnect._buildersAbove.Add(this);
			}
			else if (_builderForward == null)
			{
				_builderForward = builderToConnect;
			}
			else
			{
				SDebug.LogError(string.Concat("RouteBuilder: - attempting to connect forward builder ", builderToConnect, " but already have connected ", _builderForward), Route);
			}
			CollectConstrainingBuilders();
			builderToConnect.CollectConstrainingBuilders();
		}

		public virtual void DisconnectBuilder(RouteBuilder builderToDisconnect)
		{
			if (_buildersAbove.Remove(builderToDisconnect))
			{
				builderToDisconnect._buildersBelow.Remove(this);
			}
			else if (_buildersBelow.Remove(builderToDisconnect))
			{
				builderToDisconnect._buildersAbove.Remove(this);
			}
			else if (_builderForward == builderToDisconnect)
			{
				_builderForward = null;
			}
			CollectConstrainingBuilders();
			builderToDisconnect.CollectConstrainingBuilders();
		}

		private void DisconnectAllBuilders()
		{
			while (0 < _buildersAbove.Count)
			{
				DisconnectBuilder(_buildersAbove[0]);
			}
			while (0 < _buildersBelow.Count)
			{
				DisconnectBuilder(_buildersBelow[0]);
			}
			if (_builderForward != null)
			{
				DisconnectBuilder(_builderForward);
			}
		}

		public IConstrainingRouteBuilder GetConstrainingBuilderAt(float gamePositionZ)
		{
			foreach (IConstrainingRouteBuilder constrainingBuilder in ConstrainingBuilders)
			{
				if ((float)constrainingBuilder.ConstraintStartZ <= gamePositionZ && gamePositionZ <= (float)constrainingBuilder.ConstraintEndZ)
				{
					return constrainingBuilder;
				}
			}
			return null;
		}

		public virtual void UpdateBuilder(float characterGamePositionZ)
		{
			_completedSectionLayingThisUpdate = false;
			_chunksSpawnedThisUpdate.Clear();
			int spawnedLeadingEdgeGamePositionZ = SpawnedLeadingEdgeGamePositionZ;
			ChunkBuildMode chunkBuildMode = _chunkBuildMode;
			ProcessBuildDirectives(spawnedLeadingEdgeGamePositionZ);
			float num = characterGamePositionZ + 2000f;
			float num2 = characterGamePositionZ + 1200f;
			float reqPositionReadyGameZ = characterGamePositionZ + 1100f;
			bool flag = (float)spawnedLeadingEdgeGamePositionZ < num;
			bool flag2 = this is IConstrainingRouteBuilder;
			bool flag3 = _secionSpawnMode == SectionSpawnMode.OnlyEntry && _sectionLayingProgress == RouteBuildProgress.Start && !_currentSection.IsDone;
			bool flag4 = (_secionSpawnMode == SectionSpawnMode.All || flag3) && !IsConstraintBlockingSpawningAt(spawnedLeadingEdgeGamePositionZ) && !IsDone && (flag || flag2);
			bool flag5 = LastChunk != null && LastChunk == null && (LastChunk.Previous == null || LastChunk.Previous.IsBuilt);
			if (flag4 || flag5)
			{
				if (ReadyToAddSection)
				{
					AddNextSection();
				}
				SpawnNextChunk(_currentSection);
			}
			bool flag6 = chunkBuildMode != ChunkBuildMode.None;
			bool flag7 = _chunkBuildMode != ChunkBuildMode.None;
			float num3 = ((!(_lastBuiltChunk == null)) ? _lastBuiltChunk.GamePositionEnd.z : _originGamePosition.z);
			bool flag8 = num3 < num2;
			bool flag9 = flag3;
			if (flag9 || (flag8 && (flag6 || flag7)))
			{
				if (flag9)
				{
					BuildChunks(2.1474836E+09f);
				}
				else
				{
					BuildChunks(num2, chunkBuildMode, _chunkBuildMode);
				}
			}
			DespawnPassedChunks(characterGamePositionZ);
			if (_stopped)
			{
				DespawnUnbuiltChunks();
			}
			RemoveEmptySections();
			if (_environmentWrapperQueue.Count > 0 && _environmentWrapperQueue.First.Value.EndGamePositionZ < characterGamePositionZ)
			{
				_environmentWrapperQueue.RemoveFirst();
			}
			if (_completedSectionLayingThisUpdate && OnCompletedLayingSections__BackingField != null)
			{
				OnCompletedLayingSections__BackingField(this, EventArgs.Empty);
			}
			float reqPositionCleanedUpGameZ = characterGamePositionZ - 2000f;
			_worker.UpdateWorker(reqPositionReadyGameZ, reqPositionCleanedUpGameZ);
		}

		private bool IsConstraintBlockingSpawningAt(int spawnedLeadingEdgeGamePositionZ)
		{
			for (int i = 0; i < ConstrainingBuilders.Count; i++)
			{
				ConstrainingRouteBuilder constrainingRouteBuilder = ConstrainingBuilders[i] as ConstrainingRouteBuilder;
				if (!constrainingRouteBuilder.IsDone && constrainingRouteBuilder.ConstraintEndZ == spawnedLeadingEdgeGamePositionZ)
				{
					return true;
				}
				if (!constrainingRouteBuilder.IsDone && !constrainingRouteBuilder.HasReachedLengthVisibleFromEntry)
				{
					return true;
				}
			}
			return false;
		}

		private bool ProcessBuildDirectives(int spawnedLeadingEdgeGamePositionZ)
		{
			if (_buildDirectives.Count == 0)
			{
				return false;
			}
			BuildDirective buildDirective = _buildDirectives[0];
			if (spawnedLeadingEdgeGamePositionZ < buildDirective.GamePositionZ)
			{
				return false;
			}
			if ((buildDirective.GamePositionZ % 180 == 0 && buildDirective.GamePositionZ == spawnedLeadingEdgeGamePositionZ) || (buildDirective.GamePositionZ < spawnedLeadingEdgeGamePositionZ && !buildDirective.IsApplicablePositionStrict))
			{
				_buildDirectives.RemoveAt(0);
				SetBuildMode(buildDirective.SectionSpawnMode, buildDirective.ChunkBuildMode);
				if (buildDirective.HasSection)
				{
					AddSection(buildDirective.Section);
					return true;
				}
			}
			else
			{
				SDebug.LogErrorFormat(Route, "{0} Failed to process build directive at {1} [{2}]. Discarding...", Route.name, spawnedLeadingEdgeGamePositionZ, buildDirective);
				_buildDirectives.RemoveAt(0);
			}
			return false;
		}

		private void BuildChunks(float buildUntilGamePositionZ, ChunkBuildMode previouslySpawnedBuildMode = ChunkBuildMode.Normal, ChunkBuildMode thisUpdateSpawnedBuildMode = ChunkBuildMode.Normal)
		{
			if (_chunks.Count == 0)
			{
				return;
			}
			RouteChunk routeChunk = ((!(_lastBuiltChunk == null)) ? _lastBuiltChunk.Next : _chunks[0]);
			while (routeChunk != null && !routeChunk.IsBuilt && routeChunk.GamePositionStart.z < buildUntilGamePositionZ)
			{
				bool flag = _chunksSpawnedThisUpdate.Contains(routeChunk);
				if (flag && thisUpdateSpawnedBuildMode == ChunkBuildMode.None)
				{
					break;
				}
				ChunkBuildMode buildMode = ((!flag) ? previouslySpawnedBuildMode : thisUpdateSpawnedBuildMode);
				BuildChunk(routeChunk, buildMode);
				routeChunk = routeChunk.Next;
			}
		}

		private void BuildChunk(RouteChunk chunk, ChunkBuildMode buildMode)
		{
			DesiredEnvironmentKind = EvalNextChunkDesiredEnvironment(chunk);
			chunk.Build(DesiredEnvironmentKind, PreviousDesiredEnvironmentKind);
			_lastBuiltChunk = chunk;
			PreviousDesiredEnvironmentKind = DesiredEnvironmentKind;
			DistanceBuilt += chunk.Length;
			_chunksBuiltCount++;
			if (buildMode == ChunkBuildMode.Normal)
			{
				_worker.EnqueueSpawnWork(chunk, PerformSelectionAndActivationASync(chunk, buildMode));
			}
		}

		private IEnumerator PerformSelectionAndActivationASync(RouteChunk chunk, ChunkBuildMode buildMode)
		{
			yield return PerformSelectionASync(chunk);
			yield return chunk.ActivateTrackObjectsASync();
		}

		protected virtual EnvironmentKind EvalNextChunkDesiredEnvironment(RouteChunk spawnedChunk)
		{
			return DesiredEnvironmentKind;
		}

		public virtual bool WillChangeEnvironment(int addedDistance)
		{
			return false;
		}

		private bool IsPositionWithinConnectedBuilder(Vector3 position)
		{
			foreach (RouteBuilder buildersOnSide in BuildersOnSides)
			{
				if (buildersOnSide.Bounds.Contains(position))
				{
					return true;
				}
			}
			return false;
		}

		public void RemoveChunk(RouteChunk chunk)
		{
			_chunks.Remove(chunk);
		}

		public virtual void Destruct()
		{
			DisconnectAllBuilders();
			DespawnAllRouteChunks();
			RouteChunk.OnEnvironmentSegmentSpawned = (Action<EnvironmentKind, int, Route>)Delegate.Remove(RouteChunk.OnEnvironmentSegmentSpawned, new Action<EnvironmentKind, int, Route>(RouteChunk_OnEnvironmentSegmentSpawned));
		}

		private void CollectConstrainingBuilders()
		{
			_constrainingBuilders.Clear();
			CollectConstrainingBuilders(_buildersAbove);
			CollectConstrainingBuilders(_buildersBelow);
			CollectIfConstrainingBuilder(_builderForward);
		}

		private void CollectConstrainingBuilders(ICollection<RouteBuilder> builders)
		{
			foreach (RouteBuilder builder in builders)
			{
				CollectIfConstrainingBuilder(builder);
			}
		}

		private void CollectIfConstrainingBuilder(RouteBuilder builder)
		{
			IConstrainingRouteBuilder constrainingRouteBuilder = builder as IConstrainingRouteBuilder;
			if (constrainingRouteBuilder != null && constrainingRouteBuilder.ConstrainedRouteTypes.Contains(Route.Type))
			{
				_constrainingBuilders.Add(constrainingRouteBuilder);
			}
		}

		private void CalculateBounds()
		{
			Vector3 size = new Vector3(60f, 90f, 0f);
			Vector3 center = _originGamePosition + new Vector3(0f, 90f, 0f) * 0.5f;
			_bounds = new Bounds(center, size);
			for (int i = 0; i < _chunks.Count; i++)
			{
				_bounds.Encapsulate(_chunks[i].TrackBounds);
			}
		}

		protected virtual bool CanSpawnSection(RouteSection section)
		{
			int spawnedLeadingEdgeGamePositionZ = SpawnedLeadingEdgeGamePositionZ;
			int num = spawnedLeadingEdgeGamePositionZ + section.LengthMin;
			RouteSection.RepeatConfiguration repeatConfiguration = section.GetRepeatConfiguration();
			if (repeatConfiguration.RemainingSectionsBetweenRepeat > 0)
			{
				return false;
			}
			if (!repeatConfiguration.CanSpawnAtPositionZ(spawnedLeadingEdgeGamePositionZ, _minDistanceBetweenSectionRepeats))
			{
				return false;
			}
			if (_lastSpawnedSection != null && !repeatConfiguration.CanSpawnAfter(_lastSpawnedSection))
			{
				return false;
			}
			for (int i = 0; i < _buildDirectives.Count && _buildDirectives[i].GamePositionZ < num; i++)
			{
				if (_buildDirectives[i].IsApplicablePositionStrict)
				{
					return false;
				}
			}
			if (spawnedLeadingEdgeGamePositionZ < section.ZMinimum || (section.ZMaximum < spawnedLeadingEdgeGamePositionZ && section.ZMaximumActive))
			{
				return false;
			}
			if (section.MinSpawnOrder != 0 && SectionCountUsed < section.MinSpawnOrder + 1)
			{
				return false;
			}
			return true;
		}

		public bool CanSpawnChunk(RouteChunk chunk)
		{
			if (_constrainingBuilders.Count == 0)
			{
				return true;
			}
			int spawnedLeadingEdgeGamePositionZ = SpawnedLeadingEdgeGamePositionZ;
			int num = spawnedLeadingEdgeGamePositionZ + chunk.Length;
			for (int i = 0; i < _buildDirectives.Count && _buildDirectives[i].GamePositionZ < num; i++)
			{
				if (_buildDirectives[i].IsApplicablePositionStrict)
				{
					return false;
				}
			}
			bool flag = true;
			foreach (IConstrainingRouteBuilder constrainingBuilder in _constrainingBuilders)
			{
				ConstrainingRouteBuilder constrainingRouteBuilder = constrainingBuilder as ConstrainingRouteBuilder;
				EnvironmentKind[] allowedEnvironmentKinds = constrainingRouteBuilder.GetConstraint(Route.Type).AllowedEnvironmentKinds;
				flag = chunk.SupportsAnyEnvironmentOf(allowedEnvironmentKinds);
				bool flag2 = constrainingRouteBuilder.IsDone && constrainingRouteBuilder.ConstraintEndZ <= spawnedLeadingEdgeGamePositionZ;
				bool flag3 = constrainingRouteBuilder.IsConstrainingRange(spawnedLeadingEdgeGamePositionZ, num);
				flag = flag2 || (flag3 && flag);
				if (!flag)
				{
					return flag;
				}
			}
			return flag;
		}

		public void HideLogic(float fromGamePositionZ, float distance)
		{
			float num = fromGamePositionZ + distance;
			foreach (RouteChunk chunk in _chunks)
			{
				float z = chunk.GamePositionStart.z;
				float z2 = chunk.GamePositionEnd.z;
				if (MathUtil.AreRangesOverlapping(z, z2, fromGamePositionZ, num))
				{
					chunk.HideLogic(num);
				}
			}
		}

		protected bool IsStartPositionConstrained(int startPositionZ)
		{
			foreach (IConstrainingRouteBuilder constrainingBuilder in _constrainingBuilders)
			{
				if (constrainingBuilder.IsConstrainingStartAt(startPositionZ))
				{
					return true;
				}
			}
			return false;
		}

		protected IConstrainingRouteBuilder GetBuilderConstrainingStartAt(int startPositionZ)
		{
			return _constrainingBuilders.Find((IConstrainingRouteBuilder cb) => cb.IsConstrainingStartAt(startPositionZ));
		}

		protected void SpawnNextChunk(RouteSection section)
		{
			RouteChunk nextChunk = section.GetNextChunk(this, RandomGenerator);
			if (nextChunk != null)
			{
				SpawnRouteChunk(nextChunk);
			}
			else
			{
				bool flag = false;
				ConstrainingRouteBuilder constrainingRouteBuilder = this as ConstrainingRouteBuilder;
				if (constrainingRouteBuilder != null)
				{
					foreach (RouteBuilder constrainedBuilder in constrainingRouteBuilder.ConstrainedBuilders)
					{
						flag = flag || _constrainingBuilders.Contains(constrainedBuilder as ConstrainingRouteBuilder);
					}
				}
				if (flag)
				{
					SDebug.LogError(string.Concat("MaybeSpawnNextRouteChunk(): ", Route.Type, " No compatible chunks! Has cross-builder constrains? ", flag), Route.transform);
				}
			}
			if (section.IsDone)
			{
				IncrementSectionProgressState();
			}
		}

		protected virtual RouteSection GetSectionPrefab(RouteBuildProgress progressState, RouteType transitionRouteType = RouteType.None)
		{
			switch (progressState)
			{
			case RouteBuildProgress.Start:
				return GetStartSectionPrefab(transitionRouteType);
			case RouteBuildProgress.Mid:
				return GetMidSectionPrefab();
			case RouteBuildProgress.End:
				return GetEndSectionPrefab(transitionRouteType);
			default:
				throw new SwitchDefaultException(progressState.ToString());
			}
		}

		protected virtual RouteSection GetTransitionSectionPrefab(List<TransitionSectionInfo> transitionSections, RouteType transitionRouteType)
		{
			foreach (TransitionSectionInfo transitionSection in transitionSections)
			{
				if (transitionSection.TransitionRouteType == transitionRouteType)
				{
					return transitionSection.Section;
				}
			}
			return null;
		}

		protected RouteSection GetStartSectionPrefab(RouteType previousRouteType)
		{
			return GetTransitionSectionPrefab(_startSectionPrefabs, previousRouteType);
		}

		protected RouteSection GetEndSectionPrefab(RouteType nextRouteType)
		{
			return GetTransitionSectionPrefab(_endSectionPrefabs, nextRouteType);
		}

		protected RouteSection GetMidSectionPrefab()
		{
			RouteSection routeSection = TrainingOptions.PickMidSection(_midSectionPrefabs) ?? _midSectionPrefabs.PickSemiRandomlyOrderedPrefab(RandomGenerator);
			if (routeSection == null)
			{
				routeSection = _fallbackSection;
			}
			_lastSpawnedSection = routeSection;
			foreach (WeightedRouteSection midSectionPrefab in _midSectionPrefabs)
			{
				midSectionPrefab.Prefab.GetRepeatConfiguration().UpdateRepeatConfiguration();
			}
			routeSection.GetRepeatConfiguration().NotifySectionSpawn(SpawnedLeadingEdgeGamePositionZ, routeSection.LengthMax);
			return routeSection;
		}

		protected virtual RouteSection GetNextSectionPrefab()
		{
			return GetSectionPrefab(_sectionLayingProgress);
		}

		protected virtual void AddNextSection()
		{
			if (_sectionLayingProgress == RouteBuildProgress.Mid)
			{
				_midSectionCountUsed++;
			}
			RouteSection nextSectionPrefab = GetNextSectionPrefab();
			AddSection(nextSectionPrefab);
		}

		protected RouteSection AddSection(RouteSection sectionPrefab)
		{
			SectionCountUsed++;
			Transform transform = PoolManager.Spawn(sectionPrefab.transform);
			transform.gameObject.SetActive(true);
			transform.parent = Route.transform;
			transform.position = SpawnedLeadingEdgeWorldPositionZ * Vector3.forward;
			GamePositionRoot.Instance.AddOffsetTransform(transform);
			RouteSection component = transform.GetComponent<RouteSection>();
			component.Reset(this);
			_sections.Add(component);
			_currentSection = component;
			if (OnAddedSection != null)
			{
				OnAddedSection(_route, sectionPrefab, SpawnedLeadingEdgeGamePositionZ);
			}
			if (OnGlobalAddedSection != null)
			{
				OnGlobalAddedSection(_route, sectionPrefab, SpawnedLeadingEdgeGamePositionZ);
			}
			return component;
		}

		public bool AddTransitionSectionTo(RouteType routeType, out RouteSection routeSection)
		{
			routeSection = null;
			if (!ReadyToAddSection)
			{
				SDebug.LogErrorFormat(Route, "Attempted to add transition section to {0} even though, current section {1} is not Done", routeType, _currentSection.name);
			}
			if (routeType != RouteType.None)
			{
				TransitionSectionInfo transitionSectionInfo = _endSectionPrefabs.Find((TransitionSectionInfo s) => s.TransitionRouteType == routeType);
				if (transitionSectionInfo != null && transitionSectionInfo.Section != null)
				{
					routeSection = AddSection(transitionSectionInfo.Section);
					while (!routeSection.IsDone)
					{
						SpawnNextChunk(routeSection);
					}
				}
				else
				{
					SDebug.LogErrorFormat(Route, "Couldn't not find transition section to route: {0}", routeType);
				}
			}
			else
			{
				SDebug.LogErrorFormat(Route, "Invalid transition route specified: {0}", routeType);
			}
			return routeSection != null;
		}

		private void IncrementSectionProgressState()
		{
			while (_sectionLayingProgress != RouteBuildProgress.Done)
			{
				int progressStateSectionCount = GetProgressStateSectionCount(_sectionLayingProgress);
				if (_sectionLayingProgress == RouteBuildProgress.Mid && 0 < progressStateSectionCount && (!_midSectionsCountLimited || _midSectionCountUsed < _midSectionsCountTarget))
				{
					break;
				}
				_sectionLayingProgress++;
				if (_sectionLayingProgress == RouteBuildProgress.Done)
				{
					_completedSectionLayingThisUpdate = true;
					break;
				}
				if (GetProgressStateSectionCount(_sectionLayingProgress) != 0)
				{
					break;
				}
			}
		}

		private int GetProgressStateSectionCount(RouteBuildProgress progressState)
		{
			switch (progressState)
			{
			case RouteBuildProgress.Start:
				return _startSectionPrefabs.Count;
			case RouteBuildProgress.Mid:
				return _midSectionPrefabs.Count;
			case RouteBuildProgress.End:
				return _endSectionPrefabs.Count;
			default:
				throw new SwitchDefaultException(progressState.ToString());
			}
		}

		protected RouteChunk SpawnRouteChunk(RouteChunk chunkPrefab)
		{
			Transform transform = PoolManager.Spawn(chunkPrefab.transform);
			transform.gameObject.SetActive(true);
			transform.parent = _currentSection.transform;
			transform.SetAsLastSibling();
			transform.position = GamePositionRoot.Instance.GetWorldPosition(_originGamePosition + Vector3.forward * _length);
			RouteChunk component = transform.GetComponent<RouteChunk>();
			component.Initialize(this, _currentSection, chunkPrefab);
			_currentSection.AddSpawnedChunk(component);
			_spawnedChunkCount++;
			_bounds.Encapsulate(component.TrackBounds);
			RouteChunk routeChunk = ((_chunks.Count != 0) ? _chunks[_chunks.Count - 1] : null);
			if (routeChunk != null)
			{
				SetChunkCrossReferences(routeChunk, component);
			}
			_chunksSpawnedThisUpdate.Add(component);
			_chunks.Add(component);
			_length += component.Length;
			if (OnRouteChunkSpawned != null)
			{
				OnRouteChunkSpawned(Route, component, GamePositionRoot.Instance.GetGamePosition(transform).z);
			}
			return component;
		}

		private void SetChunkCrossReferences(RouteChunk previous, RouteChunk next)
		{
			if (previous != null)
			{
				previous.Next = next;
			}
			if (next != null)
			{
				next.Previous = previous;
			}
		}

		public RouteChunk GetChunkContainingPoint(Vector3 gamePosition)
		{
			RouteChunk result = null;
			for (int i = 0; i < _chunks.Count; i++)
			{
				RouteChunk routeChunk = _chunks[i];
				Bounds trackBounds = routeChunk.TrackBounds;
				if (trackBounds.min.z < gamePosition.z && gamePosition.z < trackBounds.max.z)
				{
					result = routeChunk;
					break;
				}
			}
			return result;
		}

		protected void DespawnRouteChunk(RouteChunk chunk, bool instantDespawn)
		{
			_bounds.center += 0.5f * Vector3.forward * chunk.Length;
			_bounds.size = new Vector3(_bounds.size.x, _bounds.size.y, _bounds.size.z - (float)chunk.Length);
			_chunks.Remove(chunk);
			_worker.EnqueueDespawnWork(chunk, DespawnRouteChunkASync(chunk), instantDespawn);
		}

		private IEnumerator DespawnRouteChunkASync(RouteChunk chunk)
		{
			yield return chunk.DeactivateTrackObjects();
			yield return chunk.Destruct();
			if (_routeHeight == RouteHeight.Pos1)
			{
				UnityEngine.Object.Destroy(chunk.gameObject);
			}
			else
			{
				PoolManager.Despawn(chunk.transform);
			}
			chunk.Section.RemoveSpawnedChunk(chunk);
		}

		protected void DespawnAllRouteChunks()
		{
			int num = _chunks.Count - 1;
			while (0 <= num)
			{
				DespawnRouteChunk(_chunks[num], true);
				num--;
			}
			int num2 = _sections.Count - 1;
			while (0 <= num2)
			{
				PoolManager.Despawn(_sections[num2].transform);
				num2--;
			}
			_sections.Clear();
		}

		protected void DespawnPassedChunks(float characterGamePositionZ)
		{
			while (0 < _chunks.Count && CanDespawnPassedChunk(_chunks[0], characterGamePositionZ))
			{
				DespawnRouteChunk(_chunks[0], false);
			}
		}

		private bool CanDespawnPassedChunk(RouteChunk chunk, float characterGamePositionZ)
		{
			return !_routeManager.CharacterChunksNear.Contains(chunk) && chunk.GamePositionEnd.z + 170f < characterGamePositionZ;
		}

		protected void DespawnUnbuiltChunks()
		{
			int num = _chunks.Count - 1;
			while (0 <= num)
			{
				RouteChunk routeChunk = _chunks[num];
				if (!routeChunk.IsBuilt)
				{
					DespawnRouteChunk(routeChunk, false);
				}
				num--;
			}
		}

		protected void RemoveEmptySections()
		{
			while (0 < _sections.Count && _sections[0].SpawnedChunkCount == 0)
			{
				RouteSection routeSection = _sections[0];
				_sections.Remove(routeSection);
				GamePositionRoot.Instance.RemoveOffsetTransform(routeSection.transform);
				PoolManager.Despawn(routeSection.transform);
			}
		}

		private IEnumerator PerformSelectionASync(RouteChunk chunk)
		{
			_processedSelectors.Clear();
			List<Selector> spawnPoints = new List<Selector>();
			List<GameObject> objectsToVisit = new List<GameObject>();
			yield return RecursiveSelectionASync(chunk.Selectors, objectsToVisit, spawnPoints);
			objectsToVisit.Clear();
			yield return RecursiveSelectionASync(chunk.EnvironmentSelectors, objectsToVisit, spawnPoints);
			objectsToVisit.Clear();
			foreach (Selector spawnPoint in spawnPoints)
			{
				spawnPoint.PerformSelection(objectsToVisit);
				yield return null;
			}
			foreach (GameObject go in objectsToVisit)
			{
				go.SetActive(true);
				yield return null;
			}
		}

		private IEnumerator RecursiveSelectionASync(List<Selector> selectors, List<GameObject> objectsToVisit, List<Selector> spawnPoints)
		{
			objectsToVisit.Clear();
			foreach (Selector selector in selectors)
			{
				if (_processedSelectors.Contains(selector))
				{
					continue;
				}
				_processedSelectors.Add(selector);
				if (!(selector != null) || !selector.gameObject.activeInHierarchy)
				{
					continue;
				}
				List<Selector> subSelectors = new List<Selector>();
				selector.PerformSelection(objectsToVisit, subSelectors);
				foreach (GameObject go in objectsToVisit)
				{
					go.SetActive(true);
					yield return null;
				}
				yield return RecursiveSelectionASync(subSelectors, objectsToVisit, spawnPoints);
			}
		}

		private void RouteBuilder_OnCompletedLayingSections(object sender, EventArgs args)
		{
		}

		private void RouteChunk_OnEnvironmentSegmentSpawned(EnvironmentKind spawnedKind, int gamePositionZ, Route route)
		{
			if (route is DefaultRoute)
			{
				if (_environmentWrapperQueue.Count > 0)
				{
					_environmentWrapperQueue.Last().EndGamePositionZ = gamePositionZ;
				}
				if (spawnedKind != _lastSpawnedEnvironmentKind)
				{
					EnvironmentRangeWrapper value = new EnvironmentRangeWrapper(spawnedKind, gamePositionZ, gamePositionZ);
					_environmentWrapperQueue.AddLast(value);
					_lastSpawnedEnvironmentKind = spawnedKind;
				}
			}
		}

		public EnvironmentKind GetEnvironmentKindAt(float gamePositionZ)
		{
			if (_environmentWrapperQueue.Count > 0)
			{
				foreach (EnvironmentRangeWrapper item in _environmentWrapperQueue)
				{
					if (item.StartGamePositionZ <= gamePositionZ && item.EndGamePositionZ > gamePositionZ)
					{
						return item.EnvironmentKind;
					}
				}
				return _environmentWrapperQueue.Last().EnvironmentKind;
			}
			return EnvironmentKind.None;
		}
	}
}
