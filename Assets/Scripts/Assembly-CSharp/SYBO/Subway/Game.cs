using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Adverty;
using AppRefresh;
using Data.Save;
using Kiloo;
using Kiloo.Common.Extensions;
using SYBO.Common.Managers;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using SYBO.Subway.Info;
using SYBO.Subway.Inputs;
using SYBO.Subway.Level;
using SYBO.Subway.Managers;
using SYBO.Subway.Pickups;
using SYBO.Subway.Routes;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using SYBO.Subway.Utilities;
using UnityEngine;

namespace SYBO.Subway
{
	public class Game : MonoBehaviour
	{
		public enum SpeedBoostType
		{
			Normal = 0,
			High = 1
		}

		[Serializable]
		public class SwipeInfo
		{
			public float distanceMin = 0.1f;

			public float doubleTapDuration = 0.3f;
		}

		[Serializable]
		public class SpeedInfo
		{
			public float min = 110f;

			public float max = 220f;

			public float rampUpDuration = 200f;
		}

		public const float Z_OFFSET_BACKGROUND_TO_CHARACTER_CAMERA = 900f;

		private static CharacterController _characterController;

		public const float DISTANCE_PER_METER = 8f;

		public static float GAME_DEFAULT_GAME_SPEED = 110f;

		public static bool HasLoaded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnGameAwake__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnGameStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool, bool, bool, bool> OnRunStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnGameRevived__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> OnPlayerDied__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> OnRunEnded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnGameReviveOffer__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnGameEnded__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnMenuScreenShown__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnGameDestroyed__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static EventHandler<CharacterModifierCollectionChangedEventArgs> OnCharacterModifierCollectionChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<int> OnTurboHeadstartInput__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnEndStumbleDeathSequence__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<bool> OnPauseChange__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnStageMenuSequence__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnIntroRun__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnCharacterModelRecreated__BackingField;

		[SerializeField]
		private float highSpeedFOVIncrease = 10f;

		[SerializeField]
		private SpeedInfo hotrodSpeedInfo;

		[SerializeField]
		private GameObject characterModelPreviewFactoryPrefab;

		[SerializeField]
		private GameObject hoverboardFactoryPrefab;

		[SerializeField]
		private SpeedInfo highSpeedInfo;

		[SerializeField]
		private float _speedChangeTransitionTime = 0.5f;

		[SerializeField]
		private float speedFOVIncrease = 5f;

		[HideInInspector]
		public bool isDead;

		public bool ingameTouchDetection = true;

		public bool canRevive;

		private float __currentSpeed;

		private float __currentLevelSpeed = 30f;

		private bool _isLockedGameSpeed;

		public float SpeedAtLastObstacleHit;

		public SwipeInfo swipe;

		public SpeedInfo speed;

		private float _elapsedGameDuration;

		public Variable<bool> IsInGame;

		public Variable<bool> IsInTopMenu;

		public AudioClipInfo DieSound;

		public bool awakeDone;

		public DateTime gameStartTime;

		[HideInInspector]
		public string startCharacterName;

		[HideInInspector]
		public int startCharacterTheme;

		public int pogoLevel;

		public int pogoJumpsLeft;

		[OptionalField]
		private Character _character;

		[HideInInspector]
		private CharacterModifierCollection _modifiers;

		private SpeedInfo _selectedBoardSpeedInfo;

		private SpeedInfo _currentSpeedInfo = new SpeedInfo();

		private SpeedInfo _originalSpeed;

		private IEnumerator _stumbleDeathSequenceRoutine;

		private Swipe _currentSwipe;

		private CharacterRendering _characterRendering;

		private CharacterCamera _characterCamera;

		private FollowingGuard _followingGuard;

		private GameObject[] _menuFactories = new GameObject[2];

		private GameStats _gameStats;

		private Camera _mainCamera;

		private bool _hasStartedFirstRun;

		private float _elapsedRunDuration;

		private float _offsetGameStartTime;

		private List<Action<float>> _speedChangeHandlers = new List<Action<float>>();

		private bool _isPaused;

		private PickupManagerController _pickupManagerController;

		private SyboRandom _randomGenerator = new SyboRandom();

		private static Game instance;

		private bool _isPausedPowerups;

		private IEnumerator _debugRepeatedlySwipeLeftRightCoroutine;

		private bool _isInStumbleDeathSequence;

		public static CharacterController Charactercontroller
		{
			get
			{
				return _characterController ?? (_characterController = UnityEngine.Object.FindObjectOfType(typeof(CharacterController)) as CharacterController);
			}
		}

		public float HighSpeedFOVIncrease
		{
			get
			{
				return highSpeedFOVIncrease;
			}
		}

		public float SpeedFOVIncrease
		{
			get
			{
				return speedFOVIncrease;
			}
		}

		public bool IsRunningOnTutorialTrack { get; set; }

		public bool IsFirstRun { get; set; }

		public float CurrentSpeed
		{
			get
			{
				return __currentSpeed;
			}
		}

		public float CurrentLevelSpeed
		{
			get
			{
				return __currentLevelSpeed;
			}
		}

		public bool IsLockedGameSpeed
		{
			get
			{
				return _isLockedGameSpeed;
			}
			set
			{
				_isLockedGameSpeed = value;
			}
		}

		public float ElapsedGameDuration
		{
			get
			{
				return _elapsedGameDuration;
			}
		}

		public bool isReadyForSlideinPowerups { get; set; }

		public Character Character
		{
			get
			{
				return _character;
			}
		}

		public CharacterModifierCollection Modifiers
		{
			get
			{
				return _modifiers;
			}
		}

		public bool IsInJetpackMode
		{
			get
			{
				return GameStateMachine.Instance.IsCurrentState<FlyingState>();
			}
		}

		public bool IsInPogostickMode
		{
			get
			{
				return GameStateMachine.Instance.IsCurrentState<PogostickState>();
			}
		}

		public bool HasSuperSneakers
		{
			get
			{
				return _modifiers.SuperSneakes.isActive;
			}
		}

		public float CurrentSpeedNormalized
		{
			get
			{
				return CurrentSpeed / speed.min;
			}
		}

		public float ElapsedRunDuration
		{
			get
			{
				return _elapsedRunDuration;
			}
		}

		public SyboRandom RandomGenerator
		{
			get
			{
				return _randomGenerator;
			}
		}

		public float OffsetGameStartTime
		{
			get
			{
				return Time.time - _offsetGameStartTime;
			}
		}

		public bool IsPaused
		{
			get
			{
				return _isPaused;
			}
		}

		public static Game Instance
		{
			get
			{
				return instance ?? (instance = Utils.FindObject<Game>(false));
			}
		}

		public static bool IsInstantiated
		{
			get
			{
				return instance != null;
			}
		}

		public bool IsPausedPowerups
		{
			get
			{
				return _isPausedPowerups;
			}
			set
			{
				_isPausedPowerups = value;
				if (_isPausedPowerups)
				{
					GameStats.Instance.IsPausedPowerups = true;
					Modifiers.Pause();
				}
				else
				{
					GameStats.Instance.IsPausedPowerups = false;
					Modifiers.Resume();
				}
			}
		}

		public bool isInStumbleDeathSequence
		{
			get
			{
				return _isInStumbleDeathSequence;
			}
		}

		public static event Action OnGameAwake
		{
			add
			{
				Action action = OnGameAwake__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameAwake__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnGameAwake__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameAwake__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnGameStarted
		{
			add
			{
				Action action = OnGameStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnGameStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<bool, bool, bool, bool> OnRunStarted
		{
			add
			{
				Action<bool, bool, bool, bool> action = OnRunStarted__BackingField;
				Action<bool, bool, bool, bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRunStarted__BackingField, (Action<bool, bool, bool, bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool, bool, bool, bool> action = OnRunStarted__BackingField;
				Action<bool, bool, bool, bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRunStarted__BackingField, (Action<bool, bool, bool, bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnGameRevived
		{
			add
			{
				Action action = OnGameRevived__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameRevived__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnGameRevived__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameRevived__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<bool> OnPlayerDied
		{
			add
			{
				Action<bool> action = OnPlayerDied__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPlayerDied__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = OnPlayerDied__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPlayerDied__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<bool> OnRunEnded
		{
			add
			{
				Action<bool> action = OnRunEnded__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRunEnded__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = OnRunEnded__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRunEnded__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnGameReviveOffer
		{
			add
			{
				Action action = OnGameReviveOffer__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameReviveOffer__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnGameReviveOffer__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameReviveOffer__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnGameEnded
		{
			add
			{
				Action action = OnGameEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameEnded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnGameEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameEnded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnMenuScreenShown
		{
			add
			{
				Action action = OnMenuScreenShown__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnMenuScreenShown__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnMenuScreenShown__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnMenuScreenShown__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnGameDestroyed
		{
			add
			{
				Action action = OnGameDestroyed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameDestroyed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnGameDestroyed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnGameDestroyed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event EventHandler<CharacterModifierCollectionChangedEventArgs> OnCharacterModifierCollectionChanged
		{
			add
			{
				EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler = OnCharacterModifierCollectionChanged__BackingField;
				EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterModifierCollectionChanged__BackingField, (EventHandler<CharacterModifierCollectionChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler = OnCharacterModifierCollectionChanged__BackingField;
				EventHandler<CharacterModifierCollectionChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterModifierCollectionChanged__BackingField, (EventHandler<CharacterModifierCollectionChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event Action<int> OnTurboHeadstartInput
		{
			add
			{
				Action<int> action = OnTurboHeadstartInput__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnTurboHeadstartInput__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<int> action = OnTurboHeadstartInput__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnTurboHeadstartInput__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action OnEndStumbleDeathSequence
		{
			add
			{
				Action action = OnEndStumbleDeathSequence__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnEndStumbleDeathSequence__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnEndStumbleDeathSequence__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnEndStumbleDeathSequence__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<bool> OnPauseChange
		{
			add
			{
				Action<bool> action = OnPauseChange__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPauseChange__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = OnPauseChange__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPauseChange__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action OnStageMenuSequence
		{
			add
			{
				Action action = OnStageMenuSequence__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnStageMenuSequence__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnStageMenuSequence__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnStageMenuSequence__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action OnIntroRun
		{
			add
			{
				Action action = OnIntroRun__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnIntroRun__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnIntroRun__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnIntroRun__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action OnCharacterModelRecreated
		{
			add
			{
				Action action = OnCharacterModelRecreated__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnCharacterModelRecreated__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnCharacterModelRecreated__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnCharacterModelRecreated__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public Game()
		{
			IsInGame = new Variable<bool>(false);
			IsInTopMenu = new Variable<bool>(false);
		}

		public static float ConvertUnityUnitsToGameMeters(float unityUnits)
		{
			return unityUnits / 8f;
		}

		public static float ConvertGameMetersToUnityUnits(float meters)
		{
			return meters * 8f;
		}

		public void SetCurrentSpeed(float speed)
		{
			__currentSpeed = speed;
		}

		public void SetInitialSpeed()
		{
			speed.min = _originalSpeed.min;
			speed.max = _originalSpeed.max;
			speed.rampUpDuration = _originalSpeed.rampUpDuration;
			SetCurrentLevelSpeed(Speed(0f, _currentSpeedInfo));
			_offsetGameStartTime = Time.time;
		}

		public void SetCurrentLevelSpeed(float levelSpeed)
		{
			__currentLevelSpeed = levelSpeed;
		}

		protected void Awake()
		{
			_character = Character.Instance;
			SingletonMonoBehaviour<TimeScaleManager>.Instantiate();
			SingletonMonoBehaviour<CoinManager>.Instantiate();
			_originalSpeed = speed;
			LoadFactories();
			HasLoaded = true;
			_hasStartedFirstRun = false;
			IsFirstRun = true;
			_mainCamera = Camera.main;
			GameStateMachine.GenerateStateMachine();
			Character.Initialize();
			_characterRendering = CharacterRendering.Instance;
			_characterCamera = CharacterCamera.Instance;
			_followingGuard = FollowingGuard.Instance;
			_followingGuard.Initialize();
			_modifiers = new CharacterModifierCollection();
			_modifiers.OnChanged += CharacterModifierCollectionChanged;
			Character.OnStumble += OnStumble;
			Character.OnCriticalHit += OnCriticalHit;
			SetGameSpeed(Speed(0f, _currentSpeedInfo));
			_gameStats = GameStats.Instance;
			awakeDone = true;
			Variable<bool> isInTopMenu = IsInTopMenu;
			isInTopMenu.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInTopMenu.OnChange, (Variable<bool>.OnChangeDelegate)((bool value) =>
			{
				if (value)
				{
					_characterRendering.CharacterModel.StartEyeAnimations(UIModelController.ModelScreen.Character);
				}
				else
				{
					_characterRendering.CharacterModel.StartEyeAnimations(UIModelController.ModelScreen.Character);
				}
			}));
			SingletonMonoBehaviour<PickupManager>.Instantiate();
			OnRunStarted += SingletonMonoBehaviour<PickupManager>.Instance.Game_OnRunStarted;
			OnGameStarted += SingletonMonoBehaviour<PickupManager>.Instance.Game_OnGameStarted;
			_pickupManagerController = new PickupManagerController(SingletonMonoBehaviour<PickupManager>.Instance);
			SingletonMonoBehaviour<HoverboardModelFactory>.Instantiate();
			SingletonMonoBehaviour<InputManager>.Instantiate();
			SingletonMonoBehaviour<InputManager>.Instance.OnInputEvent += InputManager_OnInputEvent;
			SingletonMonoBehaviour<RouteManager>.Instantiate();
			SingletonMonoBehaviour<SceneViewWindowManager>.Instantiate();
			if (DebugSettings.IsAllowed && !DebugSettings.UnlockedSceneCamera)
			{
				SingletonMonoBehaviour<SceneViewWindowManager>.Instance.IsEnabled = false;
				if (SingletonMonoBehaviour<SceneViewWindowManager>.Instance.IsEnabled)
				{
					SingletonMonoBehaviour<SceneViewWindowManager>.Instance.Character = _character;
					SingletonMonoBehaviour<SceneViewWindowManager>.Instance.SceneViewCameraTargetType = SceneViewCameraTargetType.Character;
					SingletonMonoBehaviour<SceneViewWindowManager>.Instance.SceneViewCameraZoomType = SceneViewCameraZoomType.Default;
				}
			}
			AppRefreshHandler.hasRefreshed += (RefreshType type) =>
			{
				if (type.HasFlag(RefreshType.Data))
				{
					SingletonMonoBehaviour<RouteManager>.Instance.Reset();
				}
			};
			if (OnGameAwake__BackingField != null)
			{
				OnGameAwake__BackingField();
			}
		}

		protected void Start()
		{
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.ShowMainMenu();
			}
			GameStateMachine.Initialize();
		}

		protected void OnDestroy()
		{
			if (_pickupManagerController != null)
			{
				_pickupManagerController.Destroy();
			}
			if (SingletonMonoBehaviour<InputManager>.IsInstantiated() && !SingletonMonoBehaviour.IsShuttingDown)
			{
				SingletonMonoBehaviour<InputManager>.Instance.OnInputEvent -= InputManager_OnInputEvent;
			}
			if (SingletonMonoBehaviour<CoinManager>.IsInstantiated() && !SingletonMonoBehaviour.IsShuttingDown)
			{
				SingletonMonoBehaviour<CoinManager>.Instance.OnCoinCollected -= CoinManager_OnCoinCollected;
			}
			if (_characterCamera != null && _modifiers != null)
			{
				_modifiers.OnChanged -= CharacterModifierCollectionChanged;
			}
			if (OnGameDestroyed__BackingField != null)
			{
				OnGameDestroyed__BackingField();
			}
			IsFirstRun = false;
		}

		public static void InvokeReviveOffer()
		{
			OnGameReviveOffer__BackingField.SafeInvoke();
		}

		public static void InvokeOnGameEnded()
		{
			instance.IsFirstRun = false;
			OnGameEnded__BackingField.SafeInvoke();
		}

		public static void InvokeOnMenuScreenShown()
		{
			OnMenuScreenShown__BackingField.SafeInvoke();
		}

		public void InvokeOnStageMenuSequence()
		{
			OnStageMenuSequence__BackingField.SafeInvoke();
		}

		public float EvaluatePlayerDistanceTravelTime(float distanceFromCharacter)
		{
			return distanceFromCharacter / CurrentSpeed;
		}

		public void RecreateCharacterModel()
		{
			Character.RecreateCharacterModel();
			if (OnCharacterModelRecreated__BackingField != null)
			{
				OnCharacterModelRecreated__BackingField();
			}
		}

		public void ChangeCurrentSpeed(float timeOffset)
		{
			_offsetGameStartTime -= timeOffset;
		}

		public void StartNewGame()
		{
			gameStartTime = CDateTime.UtcNow;
			_elapsedGameDuration = 0f;
			_elapsedRunDuration = 0f;
			Character.KillSwitchEnabled = true;
			_isInStumbleDeathSequence = false;
			IsRunningOnTutorialTrack = !OnboardingSave.isTutorialCompleted;
			isReadyForSlideinPowerups = false;
			if (IsRunningOnTutorialTrack)
			{
				_character.SetLastCheckpointGamePositionZ(0f);
			}
			if (!_hasStartedFirstRun && OnboardingSave.isTutorialCompleted)
			{
				_hasStartedFirstRun = true;
				IsFirstRun = true;
			}
			startCharacterName = SYBO.Subway.Info.Characters.characterData[PlayerSave.currentCharacterId].modelName;
			startCharacterTheme = PlayerSave.currentOutfitIndex;
			SaveMeManager.isPurchaseRunningIngame = false;
			IsInTopMenu.Value = false;
			IsInGame.Value = true;
			Hoverboard.Instance.HardReset();
			GameStateMachine.Instance.ActivateTrigger(101);
			OnIntroRun__BackingField.SafeInvoke();
			OnGameStarted__BackingField.SafeInvoke();
			OnRunStarted__BackingField.SafeInvoke(false, false, false, false);
			GameStats.Instance.CharacterDayEventActivated = false;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.CharacterDay);
			if (OnboardingSave.isTutorialCompleted && activeEventManager != null)
			{
				SYBO.Subway.Info.Characters.CharacterId currentCharacterId = PlayerSave.currentCharacterId;
				bool found = false;
				SYBO.Subway.Info.Characters.CharacterId eventValue = activeEventManager.GetEventValue<SYBO.Subway.Info.Characters.CharacterId>(out found);
				if (found && currentCharacterId == eventValue)
				{
					StartCoroutine(EnableCharacterDayEventAnimation());
				}
			}
			UpdateFirstNonTutorialRunSession();
			AdvertySettings.SetMainCamera(_mainCamera);
		}

		private void UpdateFirstNonTutorialRunSession()
		{
			if (OnboardingSave.isTutorialCompleted && OnboardingSave.firstNonTutorialRunSession <= 0)
			{
				OnboardingSave.firstNonTutorialRunSession = OnboardingSave.currentSessionCount;
			}
		}

		public void Update()
		{
			if (IsInGame.Value)
			{
				_elapsedGameDuration += Time.deltaTime;
				_elapsedRunDuration += Time.deltaTime;
			}
			if (!SaveMeManager.isPurchaseRunningIngame && !IsInTopMenu.Value && !IsRunningOnTutorialTrack && !IsLockedGameSpeed)
			{
				SetCurrentLevelSpeed(Speed(OffsetGameStartTime, _currentSpeedInfo));
			}
			else
			{
				_offsetGameStartTime += Time.deltaTime;
			}
			if (GameStateMachine.Instance.IsInGame())
			{
				_modifiers.Update();
			}
			GameStats.Instance.UpdatePowerupTimes(Time.deltaTime);
			if (_pickupManagerController != null)
			{
				_pickupManagerController.Update();
			}
			GjKeyboardInput();
		}

		// Teclado no navegador: WASD e setas fazem o mesmo que deslizar o dedo, espaço = toque duplo (skate)
		private void GjKeyboardInput()
		{
			if (IsPaused || !GameStateMachine.Instance.IsInGame())
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
			{
				HandleProductionInput(InputType.SwipeLeft);
			}
			if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				HandleProductionInput(InputType.SwipeRight);
			}
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
			{
				HandleProductionInput(InputType.SwipeUp);
			}
			if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
			{
				HandleProductionInput(InputType.SwipeDown);
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				HandleProductionInput(InputType.FingerDoubleTap);
			}
			// B = vantagem na hora (enquanto o botão de vantagem está na tela)
			if (Input.GetKeyDown(KeyCode.B) && SlideinPowerupHelper.GjInstance != null)
			{
				SlideinPowerupHelper.GjInstance.GjBoostHotkey();
			}
		}

		public void UpdateMeters()
		{
			_gameStats.MetersInCurrentGame = Mathf.RoundToInt(Character.GamePosition.z / 8f);
		}

		public void ActivateHighSpeed(SpeedBoostType speedBoostType)
		{
			float num = 0f;
			switch (speedBoostType)
			{
			case SpeedBoostType.High:
				_selectedBoardSpeedInfo = highSpeedInfo;
				num = highSpeedFOVIncrease;
				break;
			case SpeedBoostType.Normal:
				_selectedBoardSpeedInfo = hotrodSpeedInfo;
				num = speedFOVIncrease;
				break;
			}
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = 68f + num;
			StartCoroutine(pTween.To(_speedChangeTransitionTime, (float t) =>
			{
				_currentSpeedInfo.min = Mathf.Lerp(speed.min, _selectedBoardSpeedInfo.min, t);
				_currentSpeedInfo.max = Mathf.Lerp(speed.max, _selectedBoardSpeedInfo.max, t);
				RegisterSpeedChange(Speed(OffsetGameStartTime, _currentSpeedInfo));
				_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
			}));
		}

		public void DeactivateHighSpeed()
		{
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = 68f;
			StartCoroutine(pTween.To(_speedChangeTransitionTime, (float t) =>
			{
				if (GameStateMachine.Instance.IsInGame())
				{
					_currentSpeedInfo.min = Mathf.Lerp(_selectedBoardSpeedInfo.min, speed.min, t);
					_currentSpeedInfo.max = Mathf.Lerp(_selectedBoardSpeedInfo.max, speed.max, t);
					RegisterSpeedChange(Speed(OffsetGameStartTime, _currentSpeedInfo));
					_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
				}
				else
				{
					_currentSpeedInfo.min = speed.min;
					_currentSpeedInfo.max = speed.max;
					RegisterSpeedChange(Speed(0f, _currentSpeedInfo));
					_characterCamera.Camera.fieldOfView = 68f;
				}
			}));
		}

		public void RegisterSpeedChangeHandler(Action<float> handler)
		{
			_speedChangeHandlers.Add(handler);
		}

		private void RegisterSpeedChange(float newSpeed)
		{
			foreach (Action<float> speedChangeHandler in _speedChangeHandlers)
			{
				speedChangeHandler(newSpeed);
			}
		}

		public void PickupJetpack()
		{
			StartJetpack();
			GameStats.Instance.JetpackPickupsCollectedIncrement();
		}

		public void StartJetpack()
		{
			GameStateMachine.Instance.ActivateTrigger(104);
		}

		public void PickupPogostick()
		{
			pogoJumpsLeft = pogoLevel;
			StartPogostick();
			GameStats.Instance.PogoStickPickupsCollectedIncrement();
		}

		public void StartPogostick()
		{
			GameStateMachine.Instance.ActivateTrigger(105);
		}

		public void StartTopMenu()
		{
			GameStateMachine.Instance.ActivateTrigger(100);
		}

		public void Megaheadstart()
		{
			HeadstartState state = GameStateMachine.Instance.GetState<HeadstartState>(3);
			if (!GameStateMachine.Instance.IsCurrentState<HeadstartState>())
			{
				if (isDead)
				{
					return;
				}
				GameStateMachine.Instance.ActivateTrigger(103);
			}
			else if (state.TurboHeadstart < 2)
			{
				state.ActivateLongHeadstart();
			}
			if (OnTurboHeadstartInput__BackingField != null)
			{
				OnTurboHeadstartInput__BackingField(state.TurboHeadstart);
			}
		}

		public void Die()
		{
			if (ShouldSurviveHit())
			{
				SurviveHit();
				return;
			}
			bool obj = false;
			if (_modifiers.IsActive(_modifiers.Hoverboard))
			{
				if (_modifiers.Hoverboard.IsTeleporter)
				{
					CharacterRendering.Instance.ResetTeleportFx();
				}
				_followingGuard.Restart(true);
				_modifiers.Hoverboard.Stop = CharacterModifier.StopSignal.STOP;
				_followingGuard.MuteProximityLoop();
				_followingGuard.ResetCatchUp();
				if (_character.IsStumbling)
				{
					_character.StopStumble();
				}
				GameStats.Instance.RemoveHoverBoardPowerup();
				if (_stumbleDeathSequenceRoutine != null)
				{
					StopCoroutine(_stumbleDeathSequenceRoutine);
					_stumbleDeathSequenceRoutine = null;
					_isInStumbleDeathSequence = false;
				}
				obj = true;
			}
			else if (IsRunningOnTutorialTrack)
			{
				GameStateMachine.Instance.ActivateTrigger(106);
			}
			else
			{
				canRevive = true;
				GameStats.Instance.IsPausedPowerups = true;
				isDead = true;
				MovingTrain.ActivateAutoPilot();
				if (_followingGuard.isShowing)
				{
					if (_characterRendering.CharacterAnimation["death_movingTrain"].enabled)
					{
						_followingGuard.HitByTrainSequence();
					}
					else
					{
						_followingGuard.CatchPlayer(Character.x - Character.GetTrackX());
					}
				}
				_followingGuard.enabled = false;
				StopAllCoroutines();
				GameStateMachine.Instance.ActivateTrigger(107);
			}
			if (OnPlayerDied__BackingField != null)
			{
				OnPlayerDied__BackingField(obj);
			}
			if (OnRunEnded__BackingField != null)
			{
				OnRunEnded__BackingField(obj);
			}
		}

		public void Revive(bool revivedWithAds = false, bool adsWasAvailable = true, bool adsFailed = false)
		{
			_elapsedRunDuration = 0f;
			ResetEnemy();
			Character.Revive();
			GameStats.Instance.ResetRun();
			IsInGame.Value = true;
			StopAllCoroutines();
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInGameDefault;
			GameStateMachine.Instance.ActivateTrigger(102);
			canRevive = false;
			isDead = false;
			_isInStumbleDeathSequence = false;
			isReadyForSlideinPowerups = IngameScreen.allowPowerupsOnRevive;
			if (OnGameRevived__BackingField != null)
			{
				OnGameRevived__BackingField();
			}
			if (OnRunStarted__BackingField != null)
			{
				OnRunStarted__BackingField(true, revivedWithAds, adsWasAvailable, adsFailed);
			}
		}

		public void ResetEnemy()
		{
			if (_followingGuard != null)
			{
				_followingGuard.enabled = true;
				_followingGuard.MuteProximityLoop();
				_followingGuard.ResetCatchUp();
				_followingGuard.ResetModelRootPosition();
				_followingGuard.ShowEnemies(false);
			}
		}

		public void ResetEnemyForEnterPlay()
		{
			_followingGuard.enabled = true;
			_followingGuard.MuteProximityLoop();
			_followingGuard.Restart(true);
			_followingGuard.ShowEnemies(true);
			_followingGuard.PlayIntro();
		}

		public void DisableEnemy()
		{
			_followingGuard.StopAllCoroutines();
			_followingGuard.ResetCatchUp();
			_followingGuard.MuteProximityLoop();
			_followingGuard.enabled = false;
			_followingGuard.ShowEnemies(false);
		}

		public void EnableEnemy()
		{
			ResetEnemy();
		}

		public void LockGameSpeedTo(float gameSpeed = -1f)
		{
			Instance.IsLockedGameSpeed = true;
			SetGameSpeed(gameSpeed);
		}

		public void UnlockGameSpeed()
		{
			Instance.IsLockedGameSpeed = false;
		}

		public void SetGameSpeed(float gameSpeed)
		{
			SetCurrentSpeed(gameSpeed);
			SetCurrentLevelSpeed(gameSpeed);
			RegisterSpeedChange(gameSpeed);
		}

		public void ResetGameSpeed()
		{
			SetGameSpeed(Speed(0f, _currentSpeedInfo));
		}

		public IEnumerator SkipRevive()
		{
			ingameTouchDetection = false;
			canRevive = false;
			yield return new WaitForSeconds(0.1f);
			UIScreenController.Instance.GameOverTriggered();
			GameStateMachine.Instance.ActivateTrigger(100);
		}

		private void LoadFactories()
		{
			if (_menuFactories[0] == null)
			{
				_menuFactories[0] = UnityEngine.Object.Instantiate(characterModelPreviewFactoryPrefab);
				_menuFactories[1] = UnityEngine.Object.Instantiate(hoverboardFactoryPrefab);
				HierarchyUtility.ReparentByHierarchyPathType(_menuFactories[0].transform, HierarchyPathType.DynamicManagers);
				HierarchyUtility.ReparentByHierarchyPathType(_menuFactories[1].transform, HierarchyPathType.DynamicManagers);
			}
		}

		public void TriggerPause(bool pauseGame)
		{
			_isPaused = pauseGame;
			if (pauseGame)
			{
				ingameTouchDetection = false;
				SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = 0f;
			}
			else
			{
				ingameTouchDetection = true;
				SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInGameDefault;
			}
			if (OnPauseChange__BackingField != null)
			{
				OnPauseChange__BackingField(_isPaused);
			}
		}

		public void RewardCoin()
		{
			_gameStats.CoinsCollectedIncrement(_gameStats.CoinMultiplier);
			if (_character.IsAboveGroundOfCharacterRoute)
			{
				_gameStats.CoinsInAirIncrement();
			}
			if (IsInJetpackMode)
			{
				_gameStats.CoinsWithJetpackIncrement();
			}
			if (IsInPogostickMode)
			{
				_gameStats.CoinsWithPogoStickIncrement();
			}
			if (_character.TrackIndexCurrent == 0)
			{
				_gameStats.CoinsCollectedOnLeftTrackIncrement();
			}
			else if (_character.TrackIndexCurrent == 1)
			{
				_gameStats.CoinsCollectedOnCenterTrackIncrement();
			}
			else if (_character.TrackIndexCurrent == 2)
			{
				_gameStats.CoinsCollectedOnRightTrackIncrement();
			}
		}

		private IEnumerator EnableCharacterDayEventAnimation()
		{
			yield return new WaitForEndOfFrame();
			GameStats.Instance.CharacterDayEventActivated = true;
		}

		private IEnumerator DebugRepeatedlySwipeLeftRightCoroutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.05f);
				GameStateMachine.Instance.HandleSwipe(SwipeDir.Left);
				yield return new WaitForSeconds(0.05f);
				GameStateMachine.Instance.HandleSwipe(SwipeDir.Right);
			}
		}

		private float Speed(float time, SpeedInfo speedInfo)
		{
			if (time < speed.rampUpDuration)
			{
				float num = time / speed.rampUpDuration;
				return speedInfo.min + (speedInfo.max - speedInfo.min) * num;
			}
			return speedInfo.max;
		}

		public float GetSpeedAtTime(float t)
		{
			return Speed(t, _currentSpeedInfo);
		}

		private IEnumerator StumbleDeathSequence()
		{
			_isInStumbleDeathSequence = true;
			SetCurrentSpeed(speed.min);
			yield return new WaitForSeconds(0.2f);
			if (IsInGame.Value && !IsInPogostickMode && !IsInJetpackMode)
			{
				Character.LastDeathCause = Character.DeathCause.RepeatedStumble;
				if (OnEndStumbleDeathSequence__BackingField != null)
				{
					OnEndStumbleDeathSequence__BackingField();
				}
				GameStateMachine.Instance.HandleCriticalHit();
			}
			_isInStumbleDeathSequence = false;
			_stumbleDeathSequenceRoutine = null;
		}

		private void HandleProductionInput(InputType inputType)
		{
			switch (inputType)
			{
			case InputType.SwipeUp:
				GameStateMachine.Instance.HandleSwipe(SwipeDir.Up);
				break;
			case InputType.SwipeDown:
				GameStateMachine.Instance.HandleSwipe(SwipeDir.Down);
				break;
			case InputType.SwipeLeft:
				GameStateMachine.Instance.HandleSwipe(SwipeDir.Left);
				break;
			case InputType.SwipeRight:
				GameStateMachine.Instance.HandleSwipe(SwipeDir.Right);
				break;
			case InputType.FingerDoubleTap:
				GameStateMachine.Instance.HandleDoubleTap();
				break;
			case InputType.FingerTap:
				break;
			}
		}

		private void ToggleSwipeLeftRight()
		{
			if (_debugRepeatedlySwipeLeftRightCoroutine != null)
			{
				StopCoroutine(_debugRepeatedlySwipeLeftRightCoroutine);
				_debugRepeatedlySwipeLeftRightCoroutine = null;
			}
			else
			{
				_debugRepeatedlySwipeLeftRightCoroutine = DebugRepeatedlySwipeLeftRightCoroutine();
				StartCoroutine(_debugRepeatedlySwipeLeftRightCoroutine);
			}
		}

		private void SimulateOnApplicationPause()
		{
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.DebugSetApplicationPause(!UIScreenController.Instance.IsApplicationPaused);
			}
		}

		private void SimulateOnApplicationFocus()
		{
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.DebugSetApplicationFocus(!UIScreenController.Instance.IsApplicationFocused);
			}
		}

		public void ShowNextSceneViewCameraTargetType()
		{
			if (SingletonMonoBehaviour<SceneViewWindowManager>.Instance.IsEnabled)
			{
				SingletonMonoBehaviour<SceneViewWindowManager>.Instance.ShowNextSceneViewCameraTargetType();
			}
		}

		public void ShowNextSceneViewZoomType()
		{
			if (SingletonMonoBehaviour<SceneViewWindowManager>.Instance.IsEnabled)
			{
				SingletonMonoBehaviour<SceneViewWindowManager>.Instance.ShowNextSceneViewZoomType();
			}
		}

		private void HandleDebugInput(KeyCode keyCode)
		{
			if (!GameStateMachine.Instance.IsInGame())
			{
				return;
			}
			switch (keyCode)
			{
			case KeyCode.C:
				RewardCoin();
				break;
			case KeyCode.N:
				if (!GameStateMachine.Instance.IsCurrentState<JetpackState>())
				{
					Megaheadstart();
				}
				break;
			case KeyCode.J:
				if (!IsInJetpackMode)
				{
					StartJetpack();
				}
				break;
			case KeyCode.Q:
				if (pogoJumpsLeft == 0 && Character.IsGrounded.Value)
				{
					PickupPogostick();
				}
				break;
			case KeyCode.H:
				AddModifierSafe(Modifiers.Hoverboard);
				break;
			case KeyCode.A:
				AddModifierSafe(MysteryPowerup.GetRandomModifier());
				break;
			case KeyCode.E:
				AddModifierSafe(Modifiers.DoubleScoreMultiplier);
				break;
			case KeyCode.M:
				AddModifierSafe(Modifiers.CoinMagnet);
				break;
			case KeyCode.X:
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					_modifiers.ExpireSoon();
				}
				else
				{
					_modifiers.Stop();
				}
				break;
			case KeyCode.D:
				AddModifierSafe(Modifiers.DoubleJump);
				break;
			case KeyCode.S:
				AddModifierSafe(Modifiers.SuperSneakes);
				break;
			case KeyCode.O:
				ToggleSwipeLeftRight();
				break;
			case KeyCode.P:
				SimulateOnApplicationPause();
				break;
			case KeyCode.F:
				SimulateOnApplicationFocus();
				break;
			case KeyCode.W:
				Character.CharacterCamera.Shake();
				break;
			case KeyCode.Comma:
			case KeyCode.Less:
				ShowNextSceneViewCameraTargetType();
				break;
			case KeyCode.Period:
			case KeyCode.Greater:
				ShowNextSceneViewZoomType();
				break;
			}
		}

		private void AddModifierSafe(CharacterModifier characterModifier)
		{
			bool flag = false;
			if (!SingletonMonoBehaviour<RouteManager>.Instance.IsModifierAllowedInCurrentRoute(characterModifier))
			{
				return;
			}
			if (characterModifier == Modifiers.Hoverboard && !Globals.DEBUG_DISABLE_HOVERBOARD)
			{
				if (!Modifiers.Hoverboard.isActive)
				{
					Modifiers.Add(characterModifier);
					flag = true;
				}
			}
			else if (characterModifier == Modifiers.SuperSneakes)
			{
				if (!IsInJetpackMode && !IsInPogostickMode)
				{
					GameStats.Instance.SuperSneakerPickupsCollectedIncrement();
					_modifiers.Add(characterModifier);
					flag = true;
				}
			}
			else if (characterModifier == Modifiers.DoubleScoreMultiplier)
			{
				GameStats.Instance.DoubleMultiplierPickupsCollectedIncrement();
				Modifiers.Add(characterModifier);
				flag = true;
			}
			else if (characterModifier == Modifiers.DoubleCoins)
			{
				Modifiers.Add(characterModifier);
				flag = true;
			}
			else if (characterModifier == Modifiers.PlusMultiplier)
			{
				Modifiers.Add(characterModifier);
				flag = true;
			}
			else if (characterModifier == Modifiers.CoinMagnet)
			{
				GameStats.Instance.CoinMagnetPickupsCollectedIncrement();
				_modifiers.Add(characterModifier);
				flag = true;
			}
			else if (characterModifier == Modifiers.DoubleJump)
			{
				_modifiers.Add(characterModifier);
				flag = true;
			}
			else
			{
				SDebug.LogError("Add an 'if-else' for : " + characterModifier);
			}
		}

		private void InputManager_OnInputEvent(object sender, InputEventArgs inputEventArgs)
		{
			if (!IsPaused)
			{
				switch (inputEventArgs.InputType)
				{
				case InputType.SwipeUp:
				case InputType.SwipeDown:
				case InputType.SwipeLeft:
				case InputType.SwipeRight:
				case InputType.FingerTap:
				case InputType.FingerDoubleTap:
					HandleProductionInput(inputEventArgs.InputType);
					break;
				}
			}
		}

		private void CoinManager_OnCoinCollected(object sender, EventArgs args)
		{
			RewardCoin();
		}

		private void CharacterModifierCollectionChanged(object sender, CharacterModifierCollectionChangedEventArgs e)
		{
			if (OnCharacterModifierCollectionChanged__BackingField != null)
			{
				OnCharacterModifierCollectionChanged__BackingField(sender, e);
			}
		}

		public void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName, bool safeStumble)
		{
			// "Não Morrer": tropeçar de novo com o guarda colado não mata
			if (Character.IsStumbling && !safeStumble && !ShouldSurviveHit())
			{
				_stumbleDeathSequenceRoutine = StumbleDeathSequence();
				StartCoroutine(_stumbleDeathSequenceRoutine);
			}
		}

		public void OnCriticalHit(Character.CriticalHitType type)
		{
			if (!ShouldSurviveHit())
			{
				So.Instance.playSound(DieSound);
			}
			GameStateMachine.Instance.HandleCriticalHit();
		}

		// "Não Morrer" (Configurações): as batidas que matariam são tratadas no Character.cs (tropeça e atravessa).
		// Se ainda assim chegar aqui, continua correndo: sem sumir com obstáculos e sem pulo.
		private bool ShouldSurviveHit()
		{
			return TrainingOptions.NoDeath && IsInGame.Value && !IsRunningOnTutorialTrack && !_modifiers.IsActive(_modifiers.Hoverboard);
		}

		private void SurviveHit()
		{
			if (_stumbleDeathSequenceRoutine != null)
			{
				StopCoroutine(_stumbleDeathSequenceRoutine);
				_stumbleDeathSequenceRoutine = null;
			}
			_isInStumbleDeathSequence = false;
			if (_character.IsStumbling)
			{
				_character.StopStumble();
			}
		}
	}
}
