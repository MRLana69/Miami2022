using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Exceptions;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups;
using SYBO.Subway.Routes.Builders;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	[PrefabPath("Prefabs/Managers/RouteManager")]
	public class RouteManager : SingletonMonoBehaviour<RouteManager>
	{
		public enum ValueResetEvent
		{
			AppStart = 0,
			GameStart = 1,
			RunRevive = 2
		}

		public class CharacterChunks
		{
			public string PreviousChunkReportedName = string.Empty;

			public RouteChunk Current;

			public RouteChunk Next;

			public void Reset()
			{
				PreviousChunkReportedName = string.Empty;
				Current = (Next = null);
			}

			public bool Contains(RouteChunk chunk)
			{
				return Current == chunk || Next == chunk;
			}
		}

		[Serializable]
		public class AlternateRouteConfig
		{
			public AlternateRoute Route;

			public bool Enabled = true;

			public int Weight = 1;
		}

		private class WeightedAlternateRouteConfig : WeightedObject<AlternateRouteConfig>
		{
			public WeightedAlternateRouteConfig(AlternateRouteConfig routeConfig, int weight)
				: base(routeConfig, weight)
			{
			}
		}

		public class RouteCreatedEventArgs : EventArgs
		{
			public readonly Route Route;

			public RouteCreatedEventArgs(Route route)
			{
				Route = route;
			}
		}

		public class RouteTransitionEventArgs : EventArgs
		{
			public readonly Route CurrentRoute;

			public readonly Route TransitionRoute;

			public readonly RouteType TransitionRouteType;

			public bool ExistsTransitionRoute
			{
				get
				{
					return TransitionRoute != null;
				}
			}

			public RouteTransitionEventArgs(Route currentRoute, RouteType transitionRouteType, Route transitionRoute)
			{
				CurrentRoute = currentRoute;
				TransitionRouteType = transitionRouteType;
				TransitionRoute = transitionRoute;
			}

			public override string ToString()
			{
				return string.Format("[RouteTransitionSkippedEventArgs (Current={0}, TransutionType={1}, TransutionRoute={2})]", CurrentRoute, TransitionRoute, TransitionRoute);
			}
		}

		public class CharacterRouteChangedEventArgs : EventArgs
		{
			private Route _fromRoute;

			private Route _toRoute;

			public Route FromRoute
			{
				get
				{
					return _fromRoute;
				}
			}

			public bool HasFromRoute
			{
				get
				{
					return _fromRoute != null;
				}
			}

			public Route ToRoute
			{
				get
				{
					return _toRoute;
				}
			}

			public CharacterRouteChangedEventArgs(Route fromRoute, Route toRoute)
			{
				_fromRoute = fromRoute;
				_toRoute = toRoute;
			}

			public override string ToString()
			{
				return string.Format("[CharacterRouteTypeChangedEventArgs (FromRouteType={0}, ToRouteType={1})]", FromRoute, ToRoute);
			}
		}

		private const string RoutePoolName = "Routes";

		public const int RouteBuilderHeight = 90;

		public const int FirstRouteStartPosZ = 0;

		public static string PrefabPath = "Assets/Resources/Prefabs/Managers/RouteManager.prefab";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<RouteCreatedEventArgs> OnRouteCreated__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<CharacterRouteChangedEventArgs> OnCharacterRouteChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<RouteTransitionEventArgs> OnRouteTransitionStarted__BackingField;

		[SerializeField]
		private Route _defaultRoute;

		[SerializeField]
		[Range(0f, 600f)]
		private int _alternateRouteFirstOfferTime;

		[SerializeField]
		[Range(10f, 600f)]
		private int _alternateRouteRepeatedOfferDelay;

		[Range(0f, 60f)]
		[SerializeField]
		private int _alternateRouteRepeatedOfferDelayIncrement;

		[SerializeField]
		private bool _limitAlternateOfferToHeadstart;

		[SerializeField]
		private ValueResetEvent _alternateRouteOfferDelayResetEvent = ValueResetEvent.GameStart;

		[SerializeField]
		private List<AlternateRouteConfig> _alternateRouteConfigs;

		[SerializeField]
		private Route _benchmarkRoute;

		[SerializeField]
		private RouteManagerConfigurationSO _routeManagerConfiguration;

		private Game _game;

		private Character _character;

		private Route _mainRoute;

		private Route _characterRoute;

		private CharacterChunks _characterChunksNear = new CharacterChunks();

		private RouteCollection _routeCollection;

		private Vector3 _originGamePosition;

		private WeightRandomizedOrderedContainter<WeightedAlternateRouteConfig, AlternateRouteConfig> _alternateRouteConfigsEnabled = new WeightRandomizedOrderedContainter<WeightedAlternateRouteConfig, AlternateRouteConfig>();

		private float _timeInDefaultRoute;

		private float _alternateRouteRepeatedOfferDelayIncremented;

		private float _alternateRouteOfferDefaultRouteTimeNext;

		private bool _isWaitingForRouteChoice;

		private float _requiredOfferGamePositionZBeforeRouteOffer;

		private List<PickupSpawnPoint> _spawnPoints = new List<PickupSpawnPoint>();

		public Route DefaultRoute
		{
			get
			{
				return _defaultRoute;
			}
		}

		public static bool UseBenchmarkRoute { private get; set; }

		private bool IsPaused
		{
			get
			{
				return _game == null || (!_game.IsInGame.Value && _game.IsPaused);
			}
		}

		public ICollection<Route> Routes
		{
			get
			{
				return _routeCollection.Routes;
			}
		}

		public Route MainRoute
		{
			get
			{
				return _mainRoute;
			}
			private set
			{
				_mainRoute = value;
			}
		}

		public Route CharacterRoute
		{
			get
			{
				return _characterRoute;
			}
		}

		public CharacterChunks CharacterChunksNear
		{
			get
			{
				return _characterChunksNear;
			}
		}

		public bool IsCharacterInAnyRoute
		{
			get
			{
				return _characterRoute != null;
			}
		}

		public float TimeInDefaultRoute
		{
			get
			{
				return _timeInDefaultRoute;
			}
		}

		private float AlternateRouteOfferDefaultRouteTimeNext
		{
			get
			{
				return _alternateRouteOfferDefaultRouteTimeNext;
			}
			set
			{
				_alternateRouteOfferDefaultRouteTimeNext = value;
			}
		}

		public event EventHandler<RouteCreatedEventArgs> OnRouteCreated
		{
			add
			{
				EventHandler<RouteCreatedEventArgs> eventHandler = OnRouteCreated__BackingField;
				EventHandler<RouteCreatedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnRouteCreated__BackingField, (EventHandler<RouteCreatedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<RouteCreatedEventArgs> eventHandler = OnRouteCreated__BackingField;
				EventHandler<RouteCreatedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnRouteCreated__BackingField, (EventHandler<RouteCreatedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<CharacterRouteChangedEventArgs> OnCharacterRouteChanged
		{
			add
			{
				EventHandler<CharacterRouteChangedEventArgs> eventHandler = OnCharacterRouteChanged__BackingField;
				EventHandler<CharacterRouteChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterRouteChanged__BackingField, (EventHandler<CharacterRouteChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CharacterRouteChangedEventArgs> eventHandler = OnCharacterRouteChanged__BackingField;
				EventHandler<CharacterRouteChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterRouteChanged__BackingField, (EventHandler<CharacterRouteChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<RouteTransitionEventArgs> OnRouteTransitionStarted
		{
			add
			{
				EventHandler<RouteTransitionEventArgs> eventHandler = OnRouteTransitionStarted__BackingField;
				EventHandler<RouteTransitionEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnRouteTransitionStarted__BackingField, (EventHandler<RouteTransitionEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<RouteTransitionEventArgs> eventHandler = OnRouteTransitionStarted__BackingField;
				EventHandler<RouteTransitionEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnRouteTransitionStarted__BackingField, (EventHandler<RouteTransitionEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		protected override void SubclassInitialize()
		{
			_game = Game.Instance;
			Game.OnGameRevived += Game_OnGameRevived;
			_character = _game.Character;
			_alternateRouteOfferDefaultRouteTimeNext = _alternateRouteFirstOfferTime;
			_alternateRouteRepeatedOfferDelayIncremented = _alternateRouteRepeatedOfferDelay - _alternateRouteRepeatedOfferDelayIncrement;
			FilterAlternateRouteConfigs();
			_routeCollection = new RouteCollection(this);
			if (SingletonMonoBehaviour<PickupManager>.IsInstantiated())
			{
				SingletonMonoBehaviour<PickupManager>.Instance.OnPickupSpawnPointRegistered += PickupManager_OnPickupSpawnPointRegistered;
				SingletonMonoBehaviour<PickupManager>.Instance.OnPickupSpawnPointDeregistered += PickupManager_OnPickupSpawnPointDeregistered;
				SingletonMonoBehaviour<PickupManager>.Instance.OnPickupSpawned += PickupManager_OnPickupSpawned;
			}
			ThemeManager.Instance.Theme.InitializeEnvironmentTransitions();
			ThemeManager.Instance.Theme.InitializeEnvironmentTransitions();
			UseBenchmarkRoute = false;
		}

		private void FilterAlternateRouteConfigs(IList<RouteType> allowedRouteTypes = null)
		{
			_alternateRouteConfigsEnabled.Clear();
			foreach (AlternateRouteConfig alternateRouteConfig in _alternateRouteConfigs)
			{
				if (alternateRouteConfig.Enabled && (allowedRouteTypes == null || allowedRouteTypes.Contains(alternateRouteConfig.Route.Type)))
				{
					_alternateRouteConfigsEnabled.Add(new WeightedAlternateRouteConfig(alternateRouteConfig, alternateRouteConfig.Weight));
				}
			}
		}

		public void Reset()
		{
			_routeCollection.RemoveAll();
			_characterRoute = null;
			_mainRoute = null;
			_originGamePosition = Vector3.forward * 0f;
			_timeInDefaultRoute = 0f;
			_isWaitingForRouteChoice = false;
			_requiredOfferGamePositionZBeforeRouteOffer = ((!_limitAlternateOfferToHeadstart) ? 0f : 6500f);
			_spawnPoints.Clear();
			if (_alternateRouteOfferDelayResetEvent == ValueResetEvent.GameStart)
			{
				_alternateRouteOfferDefaultRouteTimeNext = _alternateRouteFirstOfferTime;
				_alternateRouteRepeatedOfferDelayIncremented = _alternateRouteRepeatedOfferDelay - _alternateRouteRepeatedOfferDelayIncrement;
			}
			_mainRoute = CreateRoute(RouteType.Default, SectionSpawnMode.All, ChunkBuildMode.Normal, _originGamePosition);
			_characterChunksNear.Reset();
			ChangeCharacterRoute(_mainRoute);
			UpdateRoutes(0f);
		}

		private void Update()
		{
			if (IsPaused)
			{
				return;
			}
			UpdateCharacterRoute();
			for (int i = 0; i < _routeCollection.Routes.Count; i++)
			{
				Route route = _routeCollection.Routes[i];
				if (!route.CharacterEntered && !route.CharacterSkippedEntry && GamePositionRoot.Instance.GetGamePosition(route.transform).z < _character.GamePosition.z)
				{
					route.NotifyCharacterEntrySkipped();
				}
			}
			UpdateRoutes(_character.GamePosition.z);
		}

		public bool IsModifierAllowedInCurrentRoute(CharacterModifier characterModifier)
		{
			AlternateRoute alternateRoute = SingletonMonoBehaviour<RouteManager>.Instance.CharacterRoute as AlternateRoute;
			if (alternateRoute != null)
			{
				return characterModifier.IsAllowedInAlternateRoutes;
			}
			return true;
		}

		public float GetRoutePositionOffsetY(RouteHeight routeHeight)
		{
			float result = 0f;
			switch (routeHeight)
			{
			case RouteHeight.Neg1:
				result = -90f;
				break;
			case RouteHeight.Pos0:
				result = 0f;
				break;
			case RouteHeight.Pos1:
				result = 90f;
				break;
			default:
				SwitchDefaultException.Throw(routeHeight.ToString());
				break;
			}
			return result;
		}

		private void UpdateRoutes(float characterGamePositionZ)
		{
			RouteType routeType = EvaluateAlternateRouteOffer(_mainRoute.Builder.SpawnedLeadingEdgeGamePositionZ);
			if (routeType != RouteType.None)
			{
				CreateRouteTransition(routeType);
			}
			_routeCollection.Update(characterGamePositionZ, _game.RandomGenerator);
		}

		private void UpdateCharacterRoute()
		{
			Vector3 gamePosition = _character.GamePosition;
			Route routeContainingPoint = _routeCollection.GetRouteContainingPoint(gamePosition);
			if (routeContainingPoint != _characterRoute)
			{
				ChangeCharacterRoute(routeContainingPoint);
			}
			UpdateCharacterNearChunks(_characterChunksNear, gamePosition, _characterRoute);
			if (_characterRoute is DefaultRoute)
			{
				_timeInDefaultRoute += Time.deltaTime;
			}
		}

		private void UpdateCharacterNearChunks(CharacterChunks charChunks, Vector3 characterGamePosition, Route charRoute)
		{
			if (charRoute == null)
			{
				SDebug.LogError("Character is outside bounds for any route!");
				return;
			}
			RouteChunk chunkContainingPoint = charRoute.Builder.GetChunkContainingPoint(characterGamePosition);
			if (chunkContainingPoint == null)
			{
				return;
			}
			if (charChunks.Current != chunkContainingPoint)
			{
				if (_characterChunksNear.Current != null)
				{
					charChunks.PreviousChunkReportedName = _characterChunksNear.Current.ReportedName;
				}
				charChunks.Current = chunkContainingPoint;
				charChunks.Next = chunkContainingPoint.Next;
			}
			if (charChunks.Next != charChunks.Current.Next)
			{
				charChunks.Next = charChunks.Current.Next;
			}
		}

		private void ChangeCharacterRoute(Route route)
		{
			Route route2 = route;
			if (route2 == null)
			{
				route2 = MainRoute;
			}
			if (_characterRoute != route2)
			{
				Route characterRoute = _characterRoute;
				_characterRoute = route2;
				if (characterRoute != null)
				{
					ProcessCharacterRouteExited(characterRoute, _characterRoute);
				}
				if (_characterRoute != null)
				{
					ProcessCharacterRouteEnter(_characterRoute, characterRoute);
				}
				RaiseOnCharacterRouteChanged(this, new CharacterRouteChangedEventArgs(characterRoute, _characterRoute));
			}
		}

		private void ProcessCharacterRouteEnter(Route route, Route previousRoute)
		{
			AlternateRoute alternateRoute = route as AlternateRoute;
			if (alternateRoute != null)
			{
				_isWaitingForRouteChoice = false;
				_alternateRouteRepeatedOfferDelayIncremented += _alternateRouteRepeatedOfferDelayIncrement;
				if (alternateRoute.Builder.IsDone)
				{
					SetNextAlternateRouteOfferTimeFromGamePosition(alternateRoute.Builder.SpawnedLeadingEdgeGamePositionZ);
				}
			}
			route.NotifyCharacterEntered(_character, previousRoute);
		}

		private void ProcessCharacterRouteExited(Route route, Route nextRoute)
		{
			route.NotifyCharacterExited(nextRoute);
		}

		public Route GetRoutePrefab(RouteType routeType)
		{
			if (UseBenchmarkRoute)
			{
				return _benchmarkRoute;
			}
			Route route = _defaultRoute;
			if (_defaultRoute.Type == routeType)
			{
				route = _defaultRoute;
			}
			else if (routeType != RouteType.Default)
			{
				route = FindAlternateRoute((Route r) => r.Type == routeType);
			}
			if (route == null)
			{
				return null;
			}
			return route;
		}

		private Route FindAlternateRoute(Predicate<Route> predicate)
		{
			WeightedAlternateRouteConfig weightedAlternateRouteConfig = _alternateRouteConfigsEnabled.Find((WeightedAlternateRouteConfig rc) => predicate(rc.Object.Route));
			return (weightedAlternateRouteConfig == null) ? null : weightedAlternateRouteConfig.Object.Route;
		}

		public T GetRoutePrefab<T>() where T : Route
		{
			T val = (T)null;
			if (_defaultRoute.GetType() == typeof(T))
			{
				return (T)_defaultRoute;
			}
			return (T)FindAlternateRoute((Route r) => r.GetType() == typeof(T));
		}

		private Route CreateRoute(Route routePrefab, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, Vector3 originGamePosition, RouteType previousRouteType)
		{
			Route route = UnityEngine.Object.Instantiate(routePrefab);
			route.Initialize(this, originGamePosition, sectionSpawnMode, chunkBuildMode, previousRouteType, _game.RandomGenerator);
			route.OnDestructing += Route_OnDestructing;
			AlternateRoute alternateRoute = route as AlternateRoute;
			if (alternateRoute != null)
			{
				alternateRoute.OnCharacterEntrySkipped += AlternateRoute_CharacterEntrySkipped;
				alternateRoute.Builder.OnCompletedLayingSections += AlternateRouteBuilder_OnCompletedLayingSections;
			}
			_routeCollection.Add(route);
			if (OnRouteCreated__BackingField != null)
			{
				OnRouteCreated__BackingField(this, new RouteCreatedEventArgs(route));
			}
			return route;
		}

		public Route CreateRoute(RouteType routeType, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, Vector3 originGamePosition)
		{
			Route routePrefab = GetRoutePrefab(routeType);
			return CreateRoute(routePrefab, sectionSpawnMode, chunkBuildMode, originGamePosition, RouteType.None);
		}

		public Route CreateRouteAndConnect(RouteType routeType, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, Route routeConnectTo)
		{
			Route routePrefab = GetRoutePrefab(routeType);
			Vector3 originGamePosition = Vector3.forward * routeConnectTo.Builder.SpawnedLeadingEdgeGamePositionZ;
			originGamePosition.y = GetRoutePositionOffsetY(routePrefab.Builder.RouteHeight);
			Route route = CreateRoute(routePrefab, sectionSpawnMode, chunkBuildMode, originGamePosition, routeConnectTo.Type);
			routeConnectTo.Builder.ConnectBuilder(route.Builder);
			return route;
		}

		private void CreateRouteTransition(RouteType transitionRouteType)
		{
			RouteSection routeSection;
			if (_mainRoute.Builder.AddTransitionSectionTo(transitionRouteType, out routeSection))
			{
				IRouteTransitionTrigger transitionTrigger = routeSection.GetTransitionTrigger();
				if (transitionTrigger != null)
				{
					transitionTrigger.TargetRoute = CreateRouteAndConnect(transitionRouteType, SectionSpawnMode.OnlyEntry, ChunkBuildMode.Normal, _mainRoute);
				}
			}
		}

		public bool CanCreateRoute(RouteType routeType)
		{
			return _defaultRoute.Type == routeType || FindAlternateRoute((Route r) => r.Type == routeType) != null;
		}

		public bool CanCreateRoute<T>()
		{
			return _defaultRoute.GetType() == typeof(T) || FindAlternateRoute((Route r) => r.GetType() == typeof(T)) != null;
		}

		public void StartRouteTransition(Route targetRoute)
		{
			targetRoute.CharacterStartEntryTransition(_characterRoute, targetRoute);
			if (OnRouteTransitionStarted__BackingField != null)
			{
				RouteTransitionEventArgs e = new RouteTransitionEventArgs(CharacterRoute, targetRoute.Type, targetRoute);
				OnRouteTransitionStarted__BackingField(this, e);
			}
		}

		private float ComputeDefaultRouteTimeUntilPosition(float gamePositionZ)
		{
			float num = _game.OffsetGameStartTime;
			float num2 = gamePositionZ - _character.GamePosition.z;
			AlternateRoute alternateRoute = CharacterRoute as AlternateRoute;
			if (alternateRoute != null && alternateRoute.Builder.IsDoneLayingSections)
			{
				float num3 = alternateRoute.Builder.SpawnedLeadingEdgeGamePositionZ;
				float num4 = Math.Max(0f, num3 - _character.GamePosition.z);
				float num5 = _game.EvaluatePlayerDistanceTravelTime(num4);
				num += num5;
				num2 -= num4;
			}
			if (num2 < 0f)
			{
				return 0f;
			}
			float speedAtTime = _game.GetSpeedAtTime(num);
			return num2 / speedAtTime;
		}

		private void SetNextAlternateRouteOfferTimeFromGamePosition(float gamePositionZ)
		{
			float num = ComputeDefaultRouteTimeUntilPosition(gamePositionZ);
			AlternateRouteOfferDefaultRouteTimeNext = _timeInDefaultRoute + num + _alternateRouteRepeatedOfferDelayIncremented;
		}

		public RouteType EvaluateAlternateRouteOffer(float offerGamePositionZ)
		{
			if (_alternateRouteConfigsEnabled.Count == 0)
			{
				return RouteType.None;
			}
			if ((object)this != null)
			{
				return RouteType.None;
			}
			FlyingState flyingState = GameStateMachine.Instance.TryGetCurrentState<FlyingState>();
			if (flyingState != null && (float)flyingState.TargetGamePositionZ > offerGamePositionZ)
			{
				return RouteType.None;
			}
			if (offerGamePositionZ < _requiredOfferGamePositionZBeforeRouteOffer)
			{
				return RouteType.None;
			}
			if (_characterRoute is AlternateRoute && !_characterRoute.Builder.IsDoneLayingSections)
			{
				return RouteType.None;
			}
			DefaultRouteBuilder defaultRouteBuilder = _mainRoute.Builder as DefaultRouteBuilder;
			if (defaultRouteBuilder == null || !defaultRouteBuilder.ReadyToAddSection)
			{
				return RouteType.None;
			}
			if (_spawnPoints.Count > 0 && GamePositionRoot.Instance.GetGamePosition(_spawnPoints[0].transform).z < offerGamePositionZ)
			{
				return RouteType.None;
			}
			if (defaultRouteBuilder.GetConstrainingBuilderAt(offerGamePositionZ) != null)
			{
				return RouteType.None;
			}
			float num = ComputeDefaultRouteTimeUntilPosition(offerGamePositionZ);
			float num2 = _timeInDefaultRoute + num;
			if (_isWaitingForRouteChoice || !(_alternateRouteOfferDefaultRouteTimeNext < num2))
			{
				return RouteType.None;
			}
			FilterAlternateRouteConfigs();
			RouteType result = RouteType.None;
			AlternateRouteConfig alternateRouteConfig = _alternateRouteConfigsEnabled.PickSemiRandomlyOrdered(_game.RandomGenerator);
			if (alternateRouteConfig != null)
			{
				result = alternateRouteConfig.Route.Type;
				SetNextAlternateRouteOfferTimeFromGamePosition(offerGamePositionZ);
				_isWaitingForRouteChoice = true;
			}
			return result;
		}

		private void RaiseOnCharacterRouteChanged(object sender, CharacterRouteChangedEventArgs args)
		{
			if (OnCharacterRouteChanged__BackingField != null)
			{
				OnCharacterRouteChanged__BackingField(this, args);
			}
		}

		private void Route_OnDestructing(object sender, EventArgs args)
		{
			Route route = (Route)sender;
			route.OnDestructing -= Route_OnDestructing;
			route.OnCharacterEntrySkipped -= AlternateRoute_CharacterEntrySkipped;
			route.Builder.OnCompletedLayingSections -= AlternateRouteBuilder_OnCompletedLayingSections;
		}

		private void Game_OnGameRevived()
		{
			if (_alternateRouteOfferDelayResetEvent == ValueResetEvent.RunRevive)
			{
				_alternateRouteRepeatedOfferDelayIncremented = _alternateRouteRepeatedOfferDelay;
			}
		}

		private void AlternateRouteBuilder_OnCompletedLayingSections(object sender, EventArgs args)
		{
			RouteBuilder routeBuilder = (RouteBuilder)sender;
			if (!_isWaitingForRouteChoice)
			{
				SetNextAlternateRouteOfferTimeFromGamePosition(routeBuilder.SpawnedLeadingEdgeGamePositionZ);
			}
		}

		private void AlternateRoute_CharacterEntrySkipped(object sender, Route.CharacterEntrySkippedEventArgs args)
		{
			AlternateRoute alternateRoute = (AlternateRoute)sender;
			_isWaitingForRouteChoice = false;
			SetNextAlternateRouteOfferTimeFromGamePosition(alternateRoute.OriginGamePosition.z);
		}

		private void PickupManager_OnPickupSpawnPointRegistered(PickupSpawnPoint spawnPoint)
		{
			if (spawnPoint.SpawnPointMode != SpawnPointMode.WillForcePickupType && spawnPoint.SpawnPointDifficulty == SpawnPointDifficulty.Easy && spawnPoint.ContainingRoute is DefaultRoute)
			{
				_spawnPoints.Add(spawnPoint);
				_spawnPoints.Sort((PickupSpawnPoint x, PickupSpawnPoint y) => x.transform.position.z.CompareTo(y.transform.position.z));
			}
		}

		private void PickupManager_OnPickupSpawnPointDeregistered(PickupSpawnPoint spawnPoint)
		{
			if (_spawnPoints.Contains(spawnPoint))
			{
				_spawnPoints.Remove(spawnPoint);
			}
		}

		private void PickupManager_OnPickupSpawned(object sender, PickupManager.PickupEventArgs args)
		{
			if (_spawnPoints.Contains(args.SpawnPoint))
			{
				_spawnPoints.Remove(args.SpawnPoint);
			}
			switch (args.SpawnPoint.PickupData.PickupType)
			{
			case PickupType.PogoStick:
			{
				float num = ComputeDefaultRouteTimeUntilPosition(args.SpawnPoint.transform.position.z);
				float num2 = _timeInDefaultRoute + num;
				if (AlternateRouteOfferDefaultRouteTimeNext < num2)
				{
					AlternateRouteOfferDefaultRouteTimeNext = num2;
				}
				break;
			}
			case PickupType.JetPack:
				_requiredOfferGamePositionZBeforeRouteOffer = GamePositionRoot.Instance.GetGamePosition(args.SpawnPoint.transform).z + 3240f;
				break;
			}
		}
	}
}
