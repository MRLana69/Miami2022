using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Missions;
using SYBO.Common;
using SYBO.Common.Managers;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Coins;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups;
using SYBO.Subway.Rendering;
using SYBO.Subway.Routes;
using SYBO.Subway.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SYBO.Subway.Characters
{
	public class Character : MonoBehaviour
	{
		public enum StumbleState
		{
			Standard = 0,
			Safe = 1,
			None = 2
		}

		public enum RunPositions
		{
			ground = 0,
			station = 1,
			train = 2,
			movingTrain = 3,
			air = 4
		}

		public enum StumbleHorizontalHit
		{
			Left = 0,
			LeftCorner = 1,
			Center = 2,
			RightCorner = 3,
			Right = 4
		}

		public enum StumbleVerticalHit
		{
			Upper = 0,
			Middle = 1,
			Lower = 2
		}

		public enum StumbleType
		{
			Normal = 0,
			Bush = 1,
			Side = 2
		}

		public delegate void OnStumbleDelegate(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName, bool safeStumble);

		public enum DeathCause
		{
			None = 0,
			RepeatedStumble = 1,
			CriticalHit = 2,
			KillSwitch = 3
		}

		public enum ObstacleType
		{
			None = 0,
			Sides = 1,
			MinorObstacle = 2,
			Train = 3,
			MovingTrain = 4,
			TrainRamp = 5,
			BarrierStandard = 6,
			BarrierJump = 7,
			BarrierJumpHigh = 8,
			BarrierRoll = 9,
			Pillar = 10,
			Gates = 11,
			StationPlatform = 12,
			Blocker = 13,
			Environment = 14
		}

		public enum ObstaclePassType
		{
			None = 0,
			Roll = 1,
			Jump = 2,
			JumpHigh = 3
		}

		public enum CriticalHitType
		{
			Train = 0,
			Barrier = 1,
			MovingTrain = 2,
			None = 3
		}

		public delegate void OnCriticalHitDelegate(CriticalHitType type);

		public delegate void OnReviveDelegate();

		public delegate void OnJumpDelegate(float jumpHeight, bool willPlaySound);

		public delegate void OnDoubleJumpDelegate();

		public delegate void OnJumpIfHitByTrainDelegate();

		public delegate void OnHangtimeDelegate();

		public delegate void OnRollDelegate();

		public delegate void OnRollGuardDelegate();

		public delegate void OnHitByTrainDelegate();

		public enum OnChangeTrackDirection
		{
			Left = 0,
			Right = 1
		}

		public delegate void OnChangeTrackDelegate(OnChangeTrackDirection direction);

		public delegate void OnTutorialMoveBackToCheckPointDelegate(float duration);

		public delegate void OnTutorialStartFromCheckPointDelegate();

		public delegate void OnPassedObstacleDelegate(ObstaclePassType type);

		public delegate void OnJumpOverTrainDelegate();

		public delegate void OnTeleboardStartDelegate(float duration, Vector3 offset, float gameSpeed);

		public delegate void OnGliderEndDelegate();

		public delegate void OnResetTeleportFXDelegate();

		private struct SuperSneakersJump
		{
			public float StartGamePositionZ;

			public float Length;

			public float EndGamePositionZ;

			public float StartGamePositionY;
		}

		private enum ImpactX
		{
			Left = 0,
			Middle = 1,
			Right = 2
		}

		private enum ImpactY
		{
			Upper = 0,
			Middle = 1,
			Lower = 2
		}

		private enum ImpactZ
		{
			Before = 0,
			Middle = 1,
			After = 2
		}

		private static Character _instance;

		private const float _HEIGHT_CONSIDERED_IS_ABOVE_ROUTES_GROUND = 20f;

		public const int InitialTrackIndex = 1;

		[SerializeField]
		private float _glideTime = 1f;

		[SerializeField]
		private AnimationCurve _glideVerticalSpeed = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		[HideInInspector]
		public CapsuleCollider CharacterTrigger;

		public CollisionNotifier CharacterTriggerNotifier;

		public CollisionNotifier CoinMagnetCollider;

		public CollisionNotifier CoinMagnetLongCollider;

		public ParticleSystem HoverboardCrashParticleSystem;

		public ParticleSystem MysteryPowerupParticleSystem;

		public ParticleSystem TurboHeadstartParticleSystem;

		public float ColliderTrackWidth = 17f;

		[OptionalField]
		public CharacterPickupParticles CharacterPickupParticleSystem;

		[HideInInspector]
		public bool ImmuneToCriticalHit;

		[HideInInspector]
		public float LastGroundedY;

		[HideInInspector]
		public float SubwayMaxY;

		[HideInInspector]
		public bool StopColliding;

		[HideInInspector]
		public float JumpHeight;

		[HideInInspector]
		public float x;

		[SerializeField]
		private ReviveFX _reviveFX;

		private bool _killSwitchEnabled;

		public AnimationCurve superSneakersJumpCurve;

		public float hitPoistionDistance;

		public float superSneakersJumpApexRatio = 0.5f;

		public float gliderGravity = 0.01f;

		public float gravity = 200f;

		public float jumpHeightNormal = 20f;

		public float jumpHeightSuperSneakers = 40f;

		public float verticalFallSpeedLimit = -1f;

		public float stumbleCornerTolerance = 15f;

		public float stumbleDecayTime = 5f;

		public bool inAirJump;

		public Variable<bool> IsGrounded = new Variable<bool>(false);

		public float verticalSpeed;

		public float SetPieceForgivingJumpTime = -1f;

		public float SetPieceForgivingJumpHeight = -1f;

		public bool SetPieceForcedForgivingJump;

		public Action OnSetPieceForgivingJump;

		public const float TRACK_SWITCH_SPEED_CONSTANT_DEFAULT = 30f;

		public const float TRACK_SWITCH_SPEED_CONSTANT_TELEBORD = 20f;

		private const float RampingDistance = 50f;

		private bool _isStumbling;

		private bool _isFalling;

		private bool __isJumping;

		private bool _isRolling;

		private bool _isTucked;

		private bool _isInsideSubway;

		[HideInInspector]
		private int __trackIndex;

		private float _offsetFromFloorY;

		private int __trackIndexPrevious;

		private bool _isJumpingHigher;

		private bool _isTeleboard;

		private bool _isDoubleJumping;

		private bool _isGliderboard;

		private bool _isVTol;

		private CharacterController _characterController;

		private CharacterModel _characterModel;

		private CharacterCamera _characterCamera;

		private CharacterRendering _characterRendering;

		private Hoverboard _hoverboard;

		private LongMagnet _longMagnet;

		private SuperSneakers _superSneakers;

		private Transform _characterRoot;

		private VariableBool _squeezeCollider = new VariableBool();

		private float _characterAngle = 45f;

		private Queue<Collider> GrindedTrains = new Queue<Collider>();

		private int GrindedTrainsBufferSize = 5;

		private Vector3 _gamePosition = Vector3.zero;

		private Vector3 _transformPosition = Vector3.zero;

		private Vector3 __previousFixedUpdateTransformPosition;

		private Vector3 __previousPreviousFixedUpdateTransformPosition;

		private StumbleState _stumbleState;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnStumbleDelegate OnStumble__BackingField;

		public DeathCause LastDeathCause;

		private ObstacleType _lastObstacleType;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnCriticalHitDelegate OnCriticalHit__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnReviveDelegate OnRevive__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnJumpDelegate OnJump__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnDoubleJumpDelegate OnDoubleJump__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnJumpIfHitByTrainDelegate OnJumpIfHitByTrain__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<Character> OnLanding__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnHangtimeDelegate OnHangtime__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnRollDelegate OnRoll__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnRollGuardDelegate OnRollGuard__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnHitByTrainDelegate OnHitByTrain__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnChangeTrackDelegate OnChangeTrack__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnTutorialMoveBackToCheckPointDelegate OnTutorialMoveBackToCheckPoint__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnTutorialStartFromCheckPointDelegate OnTutorialStartFromCheckPoint__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnPassedObstacleDelegate OnPassedObstacle__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnJumpOverTrainDelegate OnJumpOverTrain__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnTeleboardStartDelegate OnTeleboardStart__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnGliderEndDelegate OnVTolEnd__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private OnResetTeleportFXDelegate OnResetTeleportFX__BackingField;

		public Action<bool> OnRouteDone;

		public Action<bool> OnToggleDust;

		public Action OnEnterRoute;

		public Action OnEnterRouteEnd;

		public Action OnExitRoute;

		private bool __isInLipFlight;

		private float _lastGamePositionZ;

		private int _killSwitchUpdates = 5;

		private Sound _currentlyPlayingGrindSound;

		private Vector3 _characterControllerCenter;

		private float _characterControllerHeight;

		private float _characterControllerSkinWidth;

		private Vector3 _characterColliderCenter;

		private float _characterColliderHeight;

		private int _trackMovement;

		private int _trackMovementNext;

		private int _ignoredMovement;

		private float _characterRotation;

		private int _trackIndexTarget;

		private float _trackIndexPosition;

		private bool _willSecondJump;

		private Game _game;

		private FollowingGuard _followingGuard;

		private RouteManager _routeManager;

		private float _originaljumpHeightSuperSneakers;

		private float _glideTimeLeft;

		private bool _vTolEnded;

		private bool _jumpUpperCollision;

		private float _jumpUpperCollisionColliderMinY;

		private bool _jumpUpperCollisionEvaded = true;

		private bool _movedSinceLastFixedUpdate = true;

		private Vector3 _posFixedUpdateLast = Vector3.zero;

		private Vector3 _posFixedUpdatePrevious = Vector3.zero;

		private HashSet<Collider> subwayColliders = new HashSet<Collider>();

		private SuperSneakersJump? _superSneakersJump;

		private string _lastHitTag;

		private bool _startedJumpFromGround;

		private float _trainJumpSampleGamePositionZ;

		private float _trainJumpSampleLength = 10f;

		private bool _trainJump;

		private float _verticalSpeed_jumpTolerance = -30f;

		private Layers _layers;

		private ObstaclePassType _lastObstaclePassType;

		private int _lastObstaclePassTrackIndex;

		private float _lastCheckpointGamePositionZ;

		public static Character Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.Object.FindObjectOfType(typeof(Character)) as Character;
				}
				return _instance;
			}
		}

		public ReviveFX ReviveFX
		{
			get
			{
				return _reviveFX;
			}
		}

		public bool KillSwitchEnabled
		{
			get
			{
				return _killSwitchEnabled;
			}
			set
			{
				_killSwitchEnabled = value;
			}
		}

		public bool IsStumbling
		{
			get
			{
				return _isStumbling;
			}
			set
			{
				_isStumbling = value;
			}
		}

		public bool IsChangingTracks
		{
			get
			{
				return _trackMovement != 0;
			}
		}

		public int TrackMovement
		{
			get
			{
				return _trackMovement;
			}
			set
			{
				_trackMovement = value;
			}
		}

		public int TrackMovementNext
		{
			set
			{
				_trackMovementNext = value;
			}
		}

		public bool IsFalling
		{
			get
			{
				return _isFalling;
			}
			set
			{
				_isFalling = value;
			}
		}

		public bool IsJumping
		{
			get
			{
				return __isJumping;
			}
			set
			{
				__isJumping = value;
			}
		}

		public bool TransitioningFromHeight { get; set; }

		public bool IsRolling
		{
			get
			{
				return _isRolling;
			}
		}

		public bool IsTucked
		{
			get
			{
				return _isTucked;
			}
		}

		public bool IsInsideSubway
		{
			get
			{
				return _isInsideSubway;
			}
		}

		public int TrackIndexCurrent
		{
			get
			{
				return __trackIndex;
			}
			set
			{
				if (__trackIndex != value)
				{
					TrackIndexPrevious = __trackIndex;
					__trackIndex = value;
				}
			}
		}

		public float OffsetFromFloorY
		{
			get
			{
				return _offsetFromFloorY;
			}
			private set
			{
				_offsetFromFloorY = value;
			}
		}

		public int TrackIndexPrevious
		{
			get
			{
				return __trackIndexPrevious;
			}
			private set
			{
				__trackIndexPrevious = value;
			}
		}

		public bool IsAboveGroundOfCharacterRoute
		{
			get
			{
				float y = TransformPosition.y;
				if (_routeManager.IsCharacterInAnyRoute)
				{
					y = _routeManager.CharacterRoute.GetRelativePosition(TransformPosition).y;
				}
				return y > 20f;
			}
		}

		public bool IsJumpingHigher
		{
			get
			{
				return _isJumpingHigher || (Hoverboard.isActive && Hoverboard.Instance.IsJumper);
			}
			set
			{
				_isJumpingHigher = value;
			}
		}

		public bool IsTeleboard
		{
			get
			{
				return _isTeleboard || (Hoverboard.isActive && Hoverboard.Instance.IsTeleporter);
			}
			set
			{
				_isTeleboard = value;
			}
		}

		public bool IsDoubleJumping
		{
			get
			{
				return _isDoubleJumping || DoubleJump.doubleJumpEnabled || Hoverboard.IsDoubleJump;
			}
			set
			{
				_isDoubleJumping = value;
			}
		}

		public bool IsGliderboard
		{
			get
			{
				return _isGliderboard || (Hoverboard.isActive && Hoverboard.Instance.IsGlider);
			}
			set
			{
				_isGliderboard = value;
			}
		}

		public bool IsVTol
		{
			get
			{
				return _isVTol || (Hoverboard.isActive && Hoverboard.Instance.IsVTol);
			}
			set
			{
				_isVTol = value;
			}
		}

		public CharacterController CharacterController
		{
			get
			{
				return _characterController;
			}
		}

		public CharacterModel CharacterModel
		{
			get
			{
				return _characterModel;
			}
		}

		public CharacterCamera CharacterCamera
		{
			get
			{
				return _characterCamera;
			}
		}

		public CharacterRendering CharacterRendering
		{
			get
			{
				return _characterRendering;
			}
		}

		public Hoverboard Hoverboard
		{
			get
			{
				return _hoverboard;
			}
		}

		public LongMagnet LongMagnet
		{
			get
			{
				return _longMagnet;
			}
		}

		public SuperSneakers SuperSneakers
		{
			get
			{
				return _superSneakers;
			}
		}

		public Transform CharacterRoot
		{
			get
			{
				return _characterRoot;
			}
		}

		public VariableBool SqueezeCollider
		{
			get
			{
				return _squeezeCollider;
			}
		}

		public float CharacterAngle
		{
			get
			{
				return _characterAngle;
			}
		}

		public Bounds Bounds
		{
			get
			{
				return CharacterTrigger.bounds;
			}
		}

		public StumbleState CurrentStumbleState
		{
			get
			{
				return _stumbleState;
			}
			set
			{
				_stumbleState = value;
			}
		}

		public RunPositions CurrentRunPosition { get; private set; }

		public Vector3 GamePosition
		{
			get
			{
				return _gamePosition;
			}
		}

		public Vector3 TransformPosition
		{
			get
			{
				return _transformPosition;
			}
		}

		public Vector3 PreviousFixedUpdateTransformPosition
		{
			get
			{
				return __previousFixedUpdateTransformPosition;
			}
			set
			{
				if (__previousFixedUpdateTransformPosition != value)
				{
					PreviousPreviousFixedUpdateTransformPosition = __previousFixedUpdateTransformPosition;
					__previousFixedUpdateTransformPosition = value;
				}
			}
		}

		public Vector3 PreviousPreviousFixedUpdateTransformPosition
		{
			get
			{
				return __previousPreviousFixedUpdateTransformPosition;
			}
			private set
			{
				__previousPreviousFixedUpdateTransformPosition = value;
			}
		}

		public ObstacleType LastObstacleType
		{
			get
			{
				return _lastObstacleType;
			}
		}

		public int IgnoredMovement
		{
			set
			{
				_ignoredMovement = value;
			}
		}

		public int TrackIndexTarget
		{
			set
			{
				_trackIndexTarget = value;
			}
		}

		public float TrackIndexPosition
		{
			get
			{
				return _trackIndexPosition;
			}
			set
			{
				_trackIndexPosition = value;
			}
		}

		public float CharacterColliderLeadingZ
		{
			get
			{
				return CharacterTrigger.bounds.max.z;
			}
		}

		public bool IsInLipFlight
		{
			get
			{
				return __isInLipFlight;
			}
			set
			{
				__isInLipFlight = value;
			}
		}

		public bool IsVisibleShadow
		{
			get
			{
				return CharacterModel.IsVisibleShadow;
			}
			set
			{
				CharacterModel.IsVisibleShadow = value;
			}
		}

		public float LastCheckpointGamePositionZ
		{
			get
			{
				return _lastCheckpointGamePositionZ;
			}
		}

		public event OnStumbleDelegate OnStumble
		{
			add
			{
				OnStumbleDelegate onStumbleDelegate = OnStumble__BackingField;
				OnStumbleDelegate onStumbleDelegate2;
				do
				{
					onStumbleDelegate2 = onStumbleDelegate;
					onStumbleDelegate = Interlocked.CompareExchange(ref OnStumble__BackingField, (OnStumbleDelegate)Delegate.Combine(onStumbleDelegate2, value), onStumbleDelegate);
				}
				while ((object)onStumbleDelegate != onStumbleDelegate2);
			}
			remove
			{
				OnStumbleDelegate onStumbleDelegate = OnStumble__BackingField;
				OnStumbleDelegate onStumbleDelegate2;
				do
				{
					onStumbleDelegate2 = onStumbleDelegate;
					onStumbleDelegate = Interlocked.CompareExchange(ref OnStumble__BackingField, (OnStumbleDelegate)Delegate.Remove(onStumbleDelegate2, value), onStumbleDelegate);
				}
				while ((object)onStumbleDelegate != onStumbleDelegate2);
			}
		}

		public event OnCriticalHitDelegate OnCriticalHit
		{
			add
			{
				OnCriticalHitDelegate onCriticalHitDelegate = OnCriticalHit__BackingField;
				OnCriticalHitDelegate onCriticalHitDelegate2;
				do
				{
					onCriticalHitDelegate2 = onCriticalHitDelegate;
					onCriticalHitDelegate = Interlocked.CompareExchange(ref OnCriticalHit__BackingField, (OnCriticalHitDelegate)Delegate.Combine(onCriticalHitDelegate2, value), onCriticalHitDelegate);
				}
				while ((object)onCriticalHitDelegate != onCriticalHitDelegate2);
			}
			remove
			{
				OnCriticalHitDelegate onCriticalHitDelegate = OnCriticalHit__BackingField;
				OnCriticalHitDelegate onCriticalHitDelegate2;
				do
				{
					onCriticalHitDelegate2 = onCriticalHitDelegate;
					onCriticalHitDelegate = Interlocked.CompareExchange(ref OnCriticalHit__BackingField, (OnCriticalHitDelegate)Delegate.Remove(onCriticalHitDelegate2, value), onCriticalHitDelegate);
				}
				while ((object)onCriticalHitDelegate != onCriticalHitDelegate2);
			}
		}

		public event OnReviveDelegate OnRevive
		{
			add
			{
				OnReviveDelegate onReviveDelegate = OnRevive__BackingField;
				OnReviveDelegate onReviveDelegate2;
				do
				{
					onReviveDelegate2 = onReviveDelegate;
					onReviveDelegate = Interlocked.CompareExchange(ref OnRevive__BackingField, (OnReviveDelegate)Delegate.Combine(onReviveDelegate2, value), onReviveDelegate);
				}
				while ((object)onReviveDelegate != onReviveDelegate2);
			}
			remove
			{
				OnReviveDelegate onReviveDelegate = OnRevive__BackingField;
				OnReviveDelegate onReviveDelegate2;
				do
				{
					onReviveDelegate2 = onReviveDelegate;
					onReviveDelegate = Interlocked.CompareExchange(ref OnRevive__BackingField, (OnReviveDelegate)Delegate.Remove(onReviveDelegate2, value), onReviveDelegate);
				}
				while ((object)onReviveDelegate != onReviveDelegate2);
			}
		}

		public event OnJumpDelegate OnJump
		{
			add
			{
				OnJumpDelegate onJumpDelegate = OnJump__BackingField;
				OnJumpDelegate onJumpDelegate2;
				do
				{
					onJumpDelegate2 = onJumpDelegate;
					onJumpDelegate = Interlocked.CompareExchange(ref OnJump__BackingField, (OnJumpDelegate)Delegate.Combine(onJumpDelegate2, value), onJumpDelegate);
				}
				while ((object)onJumpDelegate != onJumpDelegate2);
			}
			remove
			{
				OnJumpDelegate onJumpDelegate = OnJump__BackingField;
				OnJumpDelegate onJumpDelegate2;
				do
				{
					onJumpDelegate2 = onJumpDelegate;
					onJumpDelegate = Interlocked.CompareExchange(ref OnJump__BackingField, (OnJumpDelegate)Delegate.Remove(onJumpDelegate2, value), onJumpDelegate);
				}
				while ((object)onJumpDelegate != onJumpDelegate2);
			}
		}

		public event OnDoubleJumpDelegate OnDoubleJump
		{
			add
			{
				OnDoubleJumpDelegate onDoubleJumpDelegate = OnDoubleJump__BackingField;
				OnDoubleJumpDelegate onDoubleJumpDelegate2;
				do
				{
					onDoubleJumpDelegate2 = onDoubleJumpDelegate;
					onDoubleJumpDelegate = Interlocked.CompareExchange(ref OnDoubleJump__BackingField, (OnDoubleJumpDelegate)Delegate.Combine(onDoubleJumpDelegate2, value), onDoubleJumpDelegate);
				}
				while ((object)onDoubleJumpDelegate != onDoubleJumpDelegate2);
			}
			remove
			{
				OnDoubleJumpDelegate onDoubleJumpDelegate = OnDoubleJump__BackingField;
				OnDoubleJumpDelegate onDoubleJumpDelegate2;
				do
				{
					onDoubleJumpDelegate2 = onDoubleJumpDelegate;
					onDoubleJumpDelegate = Interlocked.CompareExchange(ref OnDoubleJump__BackingField, (OnDoubleJumpDelegate)Delegate.Remove(onDoubleJumpDelegate2, value), onDoubleJumpDelegate);
				}
				while ((object)onDoubleJumpDelegate != onDoubleJumpDelegate2);
			}
		}

		public event OnJumpIfHitByTrainDelegate OnJumpIfHitByTrain
		{
			add
			{
				OnJumpIfHitByTrainDelegate onJumpIfHitByTrainDelegate = OnJumpIfHitByTrain__BackingField;
				OnJumpIfHitByTrainDelegate onJumpIfHitByTrainDelegate2;
				do
				{
					onJumpIfHitByTrainDelegate2 = onJumpIfHitByTrainDelegate;
					onJumpIfHitByTrainDelegate = Interlocked.CompareExchange(ref OnJumpIfHitByTrain__BackingField, (OnJumpIfHitByTrainDelegate)Delegate.Combine(onJumpIfHitByTrainDelegate2, value), onJumpIfHitByTrainDelegate);
				}
				while ((object)onJumpIfHitByTrainDelegate != onJumpIfHitByTrainDelegate2);
			}
			remove
			{
				OnJumpIfHitByTrainDelegate onJumpIfHitByTrainDelegate = OnJumpIfHitByTrain__BackingField;
				OnJumpIfHitByTrainDelegate onJumpIfHitByTrainDelegate2;
				do
				{
					onJumpIfHitByTrainDelegate2 = onJumpIfHitByTrainDelegate;
					onJumpIfHitByTrainDelegate = Interlocked.CompareExchange(ref OnJumpIfHitByTrain__BackingField, (OnJumpIfHitByTrainDelegate)Delegate.Remove(onJumpIfHitByTrainDelegate2, value), onJumpIfHitByTrainDelegate);
				}
				while ((object)onJumpIfHitByTrainDelegate != onJumpIfHitByTrainDelegate2);
			}
		}

		public event Action<Character> OnLanding
		{
			add
			{
				Action<Character> action = OnLanding__BackingField;
				Action<Character> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLanding__BackingField, (Action<Character>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Character> action = OnLanding__BackingField;
				Action<Character> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLanding__BackingField, (Action<Character>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event OnHangtimeDelegate OnHangtime
		{
			add
			{
				OnHangtimeDelegate onHangtimeDelegate = OnHangtime__BackingField;
				OnHangtimeDelegate onHangtimeDelegate2;
				do
				{
					onHangtimeDelegate2 = onHangtimeDelegate;
					onHangtimeDelegate = Interlocked.CompareExchange(ref OnHangtime__BackingField, (OnHangtimeDelegate)Delegate.Combine(onHangtimeDelegate2, value), onHangtimeDelegate);
				}
				while ((object)onHangtimeDelegate != onHangtimeDelegate2);
			}
			remove
			{
				OnHangtimeDelegate onHangtimeDelegate = OnHangtime__BackingField;
				OnHangtimeDelegate onHangtimeDelegate2;
				do
				{
					onHangtimeDelegate2 = onHangtimeDelegate;
					onHangtimeDelegate = Interlocked.CompareExchange(ref OnHangtime__BackingField, (OnHangtimeDelegate)Delegate.Remove(onHangtimeDelegate2, value), onHangtimeDelegate);
				}
				while ((object)onHangtimeDelegate != onHangtimeDelegate2);
			}
		}

		public event OnRollDelegate OnRoll
		{
			add
			{
				OnRollDelegate onRollDelegate = OnRoll__BackingField;
				OnRollDelegate onRollDelegate2;
				do
				{
					onRollDelegate2 = onRollDelegate;
					onRollDelegate = Interlocked.CompareExchange(ref OnRoll__BackingField, (OnRollDelegate)Delegate.Combine(onRollDelegate2, value), onRollDelegate);
				}
				while ((object)onRollDelegate != onRollDelegate2);
			}
			remove
			{
				OnRollDelegate onRollDelegate = OnRoll__BackingField;
				OnRollDelegate onRollDelegate2;
				do
				{
					onRollDelegate2 = onRollDelegate;
					onRollDelegate = Interlocked.CompareExchange(ref OnRoll__BackingField, (OnRollDelegate)Delegate.Remove(onRollDelegate2, value), onRollDelegate);
				}
				while ((object)onRollDelegate != onRollDelegate2);
			}
		}

		public event OnRollGuardDelegate OnRollGuard
		{
			add
			{
				OnRollGuardDelegate onRollGuardDelegate = OnRollGuard__BackingField;
				OnRollGuardDelegate onRollGuardDelegate2;
				do
				{
					onRollGuardDelegate2 = onRollGuardDelegate;
					onRollGuardDelegate = Interlocked.CompareExchange(ref OnRollGuard__BackingField, (OnRollGuardDelegate)Delegate.Combine(onRollGuardDelegate2, value), onRollGuardDelegate);
				}
				while ((object)onRollGuardDelegate != onRollGuardDelegate2);
			}
			remove
			{
				OnRollGuardDelegate onRollGuardDelegate = OnRollGuard__BackingField;
				OnRollGuardDelegate onRollGuardDelegate2;
				do
				{
					onRollGuardDelegate2 = onRollGuardDelegate;
					onRollGuardDelegate = Interlocked.CompareExchange(ref OnRollGuard__BackingField, (OnRollGuardDelegate)Delegate.Remove(onRollGuardDelegate2, value), onRollGuardDelegate);
				}
				while ((object)onRollGuardDelegate != onRollGuardDelegate2);
			}
		}

		public event OnHitByTrainDelegate OnHitByTrain
		{
			add
			{
				OnHitByTrainDelegate onHitByTrainDelegate = OnHitByTrain__BackingField;
				OnHitByTrainDelegate onHitByTrainDelegate2;
				do
				{
					onHitByTrainDelegate2 = onHitByTrainDelegate;
					onHitByTrainDelegate = Interlocked.CompareExchange(ref OnHitByTrain__BackingField, (OnHitByTrainDelegate)Delegate.Combine(onHitByTrainDelegate2, value), onHitByTrainDelegate);
				}
				while ((object)onHitByTrainDelegate != onHitByTrainDelegate2);
			}
			remove
			{
				OnHitByTrainDelegate onHitByTrainDelegate = OnHitByTrain__BackingField;
				OnHitByTrainDelegate onHitByTrainDelegate2;
				do
				{
					onHitByTrainDelegate2 = onHitByTrainDelegate;
					onHitByTrainDelegate = Interlocked.CompareExchange(ref OnHitByTrain__BackingField, (OnHitByTrainDelegate)Delegate.Remove(onHitByTrainDelegate2, value), onHitByTrainDelegate);
				}
				while ((object)onHitByTrainDelegate != onHitByTrainDelegate2);
			}
		}

		public event OnChangeTrackDelegate OnChangeTrack
		{
			add
			{
				OnChangeTrackDelegate onChangeTrackDelegate = OnChangeTrack__BackingField;
				OnChangeTrackDelegate onChangeTrackDelegate2;
				do
				{
					onChangeTrackDelegate2 = onChangeTrackDelegate;
					onChangeTrackDelegate = Interlocked.CompareExchange(ref OnChangeTrack__BackingField, (OnChangeTrackDelegate)Delegate.Combine(onChangeTrackDelegate2, value), onChangeTrackDelegate);
				}
				while ((object)onChangeTrackDelegate != onChangeTrackDelegate2);
			}
			remove
			{
				OnChangeTrackDelegate onChangeTrackDelegate = OnChangeTrack__BackingField;
				OnChangeTrackDelegate onChangeTrackDelegate2;
				do
				{
					onChangeTrackDelegate2 = onChangeTrackDelegate;
					onChangeTrackDelegate = Interlocked.CompareExchange(ref OnChangeTrack__BackingField, (OnChangeTrackDelegate)Delegate.Remove(onChangeTrackDelegate2, value), onChangeTrackDelegate);
				}
				while ((object)onChangeTrackDelegate != onChangeTrackDelegate2);
			}
		}

		public event OnTutorialMoveBackToCheckPointDelegate OnTutorialMoveBackToCheckPoint
		{
			add
			{
				OnTutorialMoveBackToCheckPointDelegate onTutorialMoveBackToCheckPointDelegate = OnTutorialMoveBackToCheckPoint__BackingField;
				OnTutorialMoveBackToCheckPointDelegate onTutorialMoveBackToCheckPointDelegate2;
				do
				{
					onTutorialMoveBackToCheckPointDelegate2 = onTutorialMoveBackToCheckPointDelegate;
					onTutorialMoveBackToCheckPointDelegate = Interlocked.CompareExchange(ref OnTutorialMoveBackToCheckPoint__BackingField, (OnTutorialMoveBackToCheckPointDelegate)Delegate.Combine(onTutorialMoveBackToCheckPointDelegate2, value), onTutorialMoveBackToCheckPointDelegate);
				}
				while ((object)onTutorialMoveBackToCheckPointDelegate != onTutorialMoveBackToCheckPointDelegate2);
			}
			remove
			{
				OnTutorialMoveBackToCheckPointDelegate onTutorialMoveBackToCheckPointDelegate = OnTutorialMoveBackToCheckPoint__BackingField;
				OnTutorialMoveBackToCheckPointDelegate onTutorialMoveBackToCheckPointDelegate2;
				do
				{
					onTutorialMoveBackToCheckPointDelegate2 = onTutorialMoveBackToCheckPointDelegate;
					onTutorialMoveBackToCheckPointDelegate = Interlocked.CompareExchange(ref OnTutorialMoveBackToCheckPoint__BackingField, (OnTutorialMoveBackToCheckPointDelegate)Delegate.Remove(onTutorialMoveBackToCheckPointDelegate2, value), onTutorialMoveBackToCheckPointDelegate);
				}
				while ((object)onTutorialMoveBackToCheckPointDelegate != onTutorialMoveBackToCheckPointDelegate2);
			}
		}

		public event OnTutorialStartFromCheckPointDelegate OnTutorialStartFromCheckPoint
		{
			add
			{
				OnTutorialStartFromCheckPointDelegate onTutorialStartFromCheckPointDelegate = OnTutorialStartFromCheckPoint__BackingField;
				OnTutorialStartFromCheckPointDelegate onTutorialStartFromCheckPointDelegate2;
				do
				{
					onTutorialStartFromCheckPointDelegate2 = onTutorialStartFromCheckPointDelegate;
					onTutorialStartFromCheckPointDelegate = Interlocked.CompareExchange(ref OnTutorialStartFromCheckPoint__BackingField, (OnTutorialStartFromCheckPointDelegate)Delegate.Combine(onTutorialStartFromCheckPointDelegate2, value), onTutorialStartFromCheckPointDelegate);
				}
				while ((object)onTutorialStartFromCheckPointDelegate != onTutorialStartFromCheckPointDelegate2);
			}
			remove
			{
				OnTutorialStartFromCheckPointDelegate onTutorialStartFromCheckPointDelegate = OnTutorialStartFromCheckPoint__BackingField;
				OnTutorialStartFromCheckPointDelegate onTutorialStartFromCheckPointDelegate2;
				do
				{
					onTutorialStartFromCheckPointDelegate2 = onTutorialStartFromCheckPointDelegate;
					onTutorialStartFromCheckPointDelegate = Interlocked.CompareExchange(ref OnTutorialStartFromCheckPoint__BackingField, (OnTutorialStartFromCheckPointDelegate)Delegate.Remove(onTutorialStartFromCheckPointDelegate2, value), onTutorialStartFromCheckPointDelegate);
				}
				while ((object)onTutorialStartFromCheckPointDelegate != onTutorialStartFromCheckPointDelegate2);
			}
		}

		public event OnPassedObstacleDelegate OnPassedObstacle
		{
			add
			{
				OnPassedObstacleDelegate onPassedObstacleDelegate = OnPassedObstacle__BackingField;
				OnPassedObstacleDelegate onPassedObstacleDelegate2;
				do
				{
					onPassedObstacleDelegate2 = onPassedObstacleDelegate;
					onPassedObstacleDelegate = Interlocked.CompareExchange(ref OnPassedObstacle__BackingField, (OnPassedObstacleDelegate)Delegate.Combine(onPassedObstacleDelegate2, value), onPassedObstacleDelegate);
				}
				while ((object)onPassedObstacleDelegate != onPassedObstacleDelegate2);
			}
			remove
			{
				OnPassedObstacleDelegate onPassedObstacleDelegate = OnPassedObstacle__BackingField;
				OnPassedObstacleDelegate onPassedObstacleDelegate2;
				do
				{
					onPassedObstacleDelegate2 = onPassedObstacleDelegate;
					onPassedObstacleDelegate = Interlocked.CompareExchange(ref OnPassedObstacle__BackingField, (OnPassedObstacleDelegate)Delegate.Remove(onPassedObstacleDelegate2, value), onPassedObstacleDelegate);
				}
				while ((object)onPassedObstacleDelegate != onPassedObstacleDelegate2);
			}
		}

		public event OnJumpOverTrainDelegate OnJumpOverTrain
		{
			add
			{
				OnJumpOverTrainDelegate onJumpOverTrainDelegate = OnJumpOverTrain__BackingField;
				OnJumpOverTrainDelegate onJumpOverTrainDelegate2;
				do
				{
					onJumpOverTrainDelegate2 = onJumpOverTrainDelegate;
					onJumpOverTrainDelegate = Interlocked.CompareExchange(ref OnJumpOverTrain__BackingField, (OnJumpOverTrainDelegate)Delegate.Combine(onJumpOverTrainDelegate2, value), onJumpOverTrainDelegate);
				}
				while ((object)onJumpOverTrainDelegate != onJumpOverTrainDelegate2);
			}
			remove
			{
				OnJumpOverTrainDelegate onJumpOverTrainDelegate = OnJumpOverTrain__BackingField;
				OnJumpOverTrainDelegate onJumpOverTrainDelegate2;
				do
				{
					onJumpOverTrainDelegate2 = onJumpOverTrainDelegate;
					onJumpOverTrainDelegate = Interlocked.CompareExchange(ref OnJumpOverTrain__BackingField, (OnJumpOverTrainDelegate)Delegate.Remove(onJumpOverTrainDelegate2, value), onJumpOverTrainDelegate);
				}
				while ((object)onJumpOverTrainDelegate != onJumpOverTrainDelegate2);
			}
		}

		public event OnTeleboardStartDelegate OnTeleboardStart
		{
			add
			{
				OnTeleboardStartDelegate onTeleboardStartDelegate = OnTeleboardStart__BackingField;
				OnTeleboardStartDelegate onTeleboardStartDelegate2;
				do
				{
					onTeleboardStartDelegate2 = onTeleboardStartDelegate;
					onTeleboardStartDelegate = Interlocked.CompareExchange(ref OnTeleboardStart__BackingField, (OnTeleboardStartDelegate)Delegate.Combine(onTeleboardStartDelegate2, value), onTeleboardStartDelegate);
				}
				while ((object)onTeleboardStartDelegate != onTeleboardStartDelegate2);
			}
			remove
			{
				OnTeleboardStartDelegate onTeleboardStartDelegate = OnTeleboardStart__BackingField;
				OnTeleboardStartDelegate onTeleboardStartDelegate2;
				do
				{
					onTeleboardStartDelegate2 = onTeleboardStartDelegate;
					onTeleboardStartDelegate = Interlocked.CompareExchange(ref OnTeleboardStart__BackingField, (OnTeleboardStartDelegate)Delegate.Remove(onTeleboardStartDelegate2, value), onTeleboardStartDelegate);
				}
				while ((object)onTeleboardStartDelegate != onTeleboardStartDelegate2);
			}
		}

		public event OnGliderEndDelegate OnVTolEnd
		{
			add
			{
				OnGliderEndDelegate onGliderEndDelegate = OnVTolEnd__BackingField;
				OnGliderEndDelegate onGliderEndDelegate2;
				do
				{
					onGliderEndDelegate2 = onGliderEndDelegate;
					onGliderEndDelegate = Interlocked.CompareExchange(ref OnVTolEnd__BackingField, (OnGliderEndDelegate)Delegate.Combine(onGliderEndDelegate2, value), onGliderEndDelegate);
				}
				while ((object)onGliderEndDelegate != onGliderEndDelegate2);
			}
			remove
			{
				OnGliderEndDelegate onGliderEndDelegate = OnVTolEnd__BackingField;
				OnGliderEndDelegate onGliderEndDelegate2;
				do
				{
					onGliderEndDelegate2 = onGliderEndDelegate;
					onGliderEndDelegate = Interlocked.CompareExchange(ref OnVTolEnd__BackingField, (OnGliderEndDelegate)Delegate.Remove(onGliderEndDelegate2, value), onGliderEndDelegate);
				}
				while ((object)onGliderEndDelegate != onGliderEndDelegate2);
			}
		}

		public event OnResetTeleportFXDelegate OnResetTeleportFX
		{
			add
			{
				OnResetTeleportFXDelegate onResetTeleportFXDelegate = OnResetTeleportFX__BackingField;
				OnResetTeleportFXDelegate onResetTeleportFXDelegate2;
				do
				{
					onResetTeleportFXDelegate2 = onResetTeleportFXDelegate;
					onResetTeleportFXDelegate = Interlocked.CompareExchange(ref OnResetTeleportFX__BackingField, (OnResetTeleportFXDelegate)Delegate.Combine(onResetTeleportFXDelegate2, value), onResetTeleportFXDelegate);
				}
				while ((object)onResetTeleportFXDelegate != onResetTeleportFXDelegate2);
			}
			remove
			{
				OnResetTeleportFXDelegate onResetTeleportFXDelegate = OnResetTeleportFX__BackingField;
				OnResetTeleportFXDelegate onResetTeleportFXDelegate2;
				do
				{
					onResetTeleportFXDelegate2 = onResetTeleportFXDelegate;
					onResetTeleportFXDelegate = Interlocked.CompareExchange(ref OnResetTeleportFX__BackingField, (OnResetTeleportFXDelegate)Delegate.Remove(onResetTeleportFXDelegate2, value), onResetTeleportFXDelegate);
				}
				while ((object)onResetTeleportFXDelegate != onResetTeleportFXDelegate2);
			}
		}

		public void Initialize()
		{
			_instance = this;
			_characterRendering = GetComponent<CharacterRendering>();
			_characterController = base.gameObject.GetComponent<CharacterController>();
			_layers = Layers.Instance;
			_game = Game.Instance;
			_characterController = Game.Charactercontroller;
			_hoverboard = Hoverboard.Instance;
			_longMagnet = _hoverboard.GetComponent<LongMagnet>();
			_routeManager = SingletonMonoBehaviour<RouteManager>.Instance;
			Renderer component = HoverboardCrashParticleSystem.GetComponent<Renderer>();
			component.material.renderQueue = RenderQueues.TransparentOverlay;
			_routeManager.OnRouteCreated += RouteManager_OnRouteCreated;
			Variable<bool> isInGame = _game.IsInGame;
			isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, (Variable<bool>.OnChangeDelegate)((bool flag) =>
			{
				if (!flag)
				{
					StopAllCoroutines();
					ImmuneToCriticalHit = false;
					_characterController.enabled = true;
					StopColliding = false;
				}
			}));
			VariableBool squeezeCollider = _squeezeCollider;
			squeezeCollider.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(squeezeCollider.OnChange, (VariableBool.OnChangeDelegate)((bool squeeze) =>
			{
				if (squeeze)
				{
					_characterController.height = 4f;
					_characterController.center = new Vector3(0f, 2f, _characterControllerCenter.z);
					CharacterTrigger.height = 4f;
					CharacterTrigger.center = new Vector3(0f, 4f, _characterColliderCenter.z);
				}
				else
				{
					_characterController.center = _characterControllerCenter;
					_characterController.height = _characterControllerHeight;
					CharacterTrigger.center = _characterColliderCenter;
					CharacterTrigger.height = _characterColliderHeight;
				}
			}));
			_characterRendering.Initialize();
			_characterModel = GetComponentInChildren<CharacterModel>();
			_characterModel.InitializeCharacter();
			_characterRoot = CharacterModel.transform;
			_superSneakers = this.FindObject<SuperSneakers>();
			_characterCamera = CharacterCamera.Instance;
			_followingGuard = FollowingGuard.Instance;
			CharacterPickupParticleSystem = GetComponentInChildren<CharacterPickupParticles>();
			CharacterTrigger = (CapsuleCollider)CharacterTriggerNotifier.Collider;
			CharacterTriggerNotifier.TriggerEnter += OnCharacterTriggerEnter;
			CharacterTriggerNotifier.TriggerExit += OnCharacterTriggerExit;
			_characterControllerCenter = CharacterController.center;
			_characterControllerHeight = CharacterController.height;
			_characterControllerSkinWidth = _characterController.skinWidth;
			_characterColliderCenter = CharacterTrigger.center;
			_characterColliderHeight = CharacterTrigger.height;
			OnLanding += MissionsManager.Instance.OnChangeIsCharacterOnGround;
			_originaljumpHeightSuperSneakers = jumpHeightSuperSneakers;
		}

		public void RecreateCharacterModel()
		{
			UnityEngine.Object.Destroy(CharacterModel.gameObject);
			_characterModel = _characterRendering.RecreateCharacterModel();
			_characterRoot = CharacterModel.transform;
		}

		protected void Awake()
		{
			_instance = this;
			GamePositionRoot.Instance.AddOffsetTransform(base.transform);
			_offsetFromFloorY = RouteChunk.GetBaseColliderOffsetY() + _characterController.skinWidth + _characterController.center.y - _characterController.height / 2f;
			Variable<bool> isGrounded = IsGrounded;
			isGrounded.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isGrounded.OnChange, new Variable<bool>.OnChangeDelegate(IsGrounded_OnChange));
		}

		protected void OnDestroy()
		{
			if (_routeManager != null)
			{
				_routeManager.OnRouteCreated -= RouteManager_OnRouteCreated;
			}
		}

		protected void FixedUpdate()
		{
			if (_movedSinceLastFixedUpdate)
			{
				_posFixedUpdatePrevious = _posFixedUpdateLast;
				_posFixedUpdateLast = base.transform.position;
				_movedSinceLastFixedUpdate = false;
			}
			PreviousFixedUpdateTransformPosition = base.transform.position;
			CharacterTriggerNotifier.ClearFrame();
			CoinMagnetCollider.ClearFrame();
			CoinMagnetLongCollider.ClearFrame();
		}

		protected void Update()
		{
			Vector3 transformPosition = TransformPosition;
			if (transformPosition.y < 0f)
			{
				transformPosition.y = 1f;
				base.transform.position = transformPosition;
				CachePositions();
			}
			if (_game.IsPaused || SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale == 0f)
			{
				return;
			}
			bool flag = DebugSettings.IsAllowed && DebugSettings.CharacterInvincible;
			Quaternion characterModelLocalRotation = GetCharacterModelLocalRotation();
			Quaternion to = Quaternion.Euler(0f, characterModelLocalRotation.eulerAngles.y, characterModelLocalRotation.eulerAngles.z);
			SetCharacterModelLocalRotation(Quaternion.RotateTowards(characterModelLocalRotation, to, 90f * Time.deltaTime));
			bool flag2 = _game.Modifiers.Hoverboard.Stop != CharacterModifier.StopSignal.DONT_STOP && _game.Modifiers.IsActive(_game.Modifiers.Hoverboard);
			if (!flag && _killSwitchEnabled)
			{
				if (GamePosition.z == _lastGamePositionZ && _game.IsInGame.Value && !_game.isInStumbleDeathSequence && !_game.isDead)
				{
					if (_killSwitchUpdates > 0)
					{
						_killSwitchUpdates--;
					}
					else if (!_game.IsPaused && !_game.isDead && !flag2 && GamePosition.z > 10f)
					{
						ExecuteKillSwitch();
					}
				}
				else if (_killSwitchUpdates != 10)
				{
					_killSwitchUpdates = 10;
				}
			}
			_lastGamePositionZ = GamePosition.z;
		}

		public void SetCharacterModelLocalRotation(Quaternion rotation)
		{
			_characterModel.transform.localRotation = rotation;
		}

		public Quaternion GetCharacterModelLocalRotation()
		{
			return _characterModel.transform.localRotation;
		}

		public void ResetCharacterModelLocalRotation()
		{
			_characterModel.transform.localRotation = Quaternion.identity;
		}

		public void Reset()
		{
			LastDeathCause = DeathCause.None;
			ClearLastObstacleType();
			IsVisibleShadow = true;
			ResetCharacterModelLocalRotation();
			CurrentStumbleState = StumbleState.Standard;
			TrackIndexCurrent = (_trackIndexTarget = 1);
			x = Route.GetTrackX(TrackIndexCurrent);
			_trackIndexPosition = TrackIndexCurrent;
			_trackMovement = 0;
			_trackMovementNext = 0;
			_ignoredMovement = 0;
			_squeezeCollider.Clear();
			SetGamePosition(Route.GetPosition(x, 0f) + Vector3.up * 5f);
			CharacterController.Move(-5f * Vector3.up);
			CachePositions();
			verticalSpeed = 0f;
			_superSneakersJump = null;
			JumpHeight = jumpHeightNormal;
			inAirJump = false;
			IsJumping = false;
			_isRolling = false;
			IsInLipFlight = false;
			_startedJumpFromGround = false;
			_jumpUpperCollision = false;
			_jumpUpperCollisionEvaded = true;
			LastGroundedY = 0f;
			__isInLipFlight = false;
			subwayColliders.Clear();
			_isInsideSubway = false;
		}

		public void InitForRun()
		{
			StartStumble();
		}

		public void Revive()
		{
			LastDeathCause = DeathCause.None;
			ClearLastObstacleType();
			_characterRendering.ResetTeleportFx();
			if (OnRevive__BackingField != null)
			{
				OnRevive__BackingField();
			}
			if (_isStumbling)
			{
				StopStumble();
			}
			IsJumping = true;
			_isFalling = false;
			verticalSpeed = CalculateJumpVerticalSpeed(15f);
			_superSneakersJump = null;
			if (_jumpUpperCollision)
			{
				float num = CharacterController.bounds.min.y - _jumpUpperCollisionColliderMinY;
				if (num < 0f)
				{
					CharacterController.Move(num * Vector3.up);
				}
			}
			CachePositions();
			_jumpUpperCollision = false;
			_jumpUpperCollisionEvaded = true;
		}

		private void SetLastObstacleType(ObstacleType ot)
		{
			_lastObstacleType = ot;
			_game.SpeedAtLastObstacleHit = _game.CurrentSpeed;
		}

		private void ClearLastObstacleType()
		{
			_lastObstacleType = ObstacleType.None;
			_game.SpeedAtLastObstacleHit = _game.speed.min;
		}

		public void SetGamePosition(Vector3 gamePosition)
		{
			base.transform.position = GamePositionRoot.Instance.GetWorldPosition(gamePosition);
			CachePositions();
		}

		public void UpdateGroundTag(Character character)
		{
			Ray ray = new Ray(character.CharacterRoot.position, -Vector3.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				switch (hitInfo.collider.tag)
				{
				case "Ground":
					CurrentRunPosition = RunPositions.ground;
					MissionsManager.Instance.RemoveProgressForThis(MissionTarget.LandOnTrainInRow);
					break;
				case "HitTrain":
					CurrentRunPosition = RunPositions.train;
					LandedOnTrain(hitInfo.collider);
					break;
				case "HitMovingTrain":
					CurrentRunPosition = RunPositions.movingTrain;
					LandedOnTrain(hitInfo.collider);
					break;
				case "Station":
					CurrentRunPosition = RunPositions.station;
					break;
				}
			}
		}

		public void UpdateInAirRunPosition()
		{
			if (!_characterController.isGrounded)
			{
				CurrentRunPosition = RunPositions.air;
			}
		}

		private void LandedOnTrain(Collider trainCollider)
		{
			if (Hoverboard.enabled && !GrindedTrains.Contains(trainCollider))
			{
				if (GrindedTrains.Count > GrindedTrainsBufferSize)
				{
					GrindedTrains.Dequeue();
				}
				GrindedTrains.Enqueue(trainCollider);
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.LandOnTrainInRow);
		}

		public bool IsDescending()
		{
			return CharacterController.velocity.y < 0f;
		}

		public void CreateExplosionAtCharactersFeet()
		{
			Vector3 positionVector = new Vector3(0f, 0f, -7f);
			RouteScoreChangeExplosion routeScoreChangeExplosion = RouteScoreChangeExplosion.CreateAtPosition(positionVector);
			HierarchyUtility.Reparent(routeScoreChangeExplosion.gameObject.transform, _characterModel.GetHoverboardGameObject().transform.parent);
		}

		public TrackLane GetLaneFromIndex(int laneIndex)
		{
			switch (laneIndex)
			{
			case 0:
				return TrackLane.Left;
			case 1:
				return TrackLane.Mid;
			case 2:
				return TrackLane.Right;
			default:
			{
				TrackLane trackLane = TrackLane.Mid;
				Debug.LogErrorFormat("Cannot convert lane index {0} to lane. Return {1}", laneIndex, trackLane);
				return trackLane;
			}
			}
		}

		public void ChangeTrackLeft1()
		{
			ChangeTrackBy(-1);
		}

		public void ChangeTrackLeft2()
		{
			ChangeTrackBy(-1);
		}

		public void ChangeTrackRight1()
		{
			ChangeTrackBy(1);
		}

		public void ChangeTrackRight2()
		{
			ChangeTrackBy(1);
		}

		public void ChangeTrackCenter()
		{
			if (TrackIndexCurrent == 2)
			{
				ChangeTrackLeft1();
			}
			else if (TrackIndexCurrent == 0)
			{
				ChangeTrackRight1();
			}
		}

		public void ChangeTrackBy(int trackDelta)
		{
			if (IsTeleboard)
			{
				ChangeTrackBy(trackDelta, 20f / Game.Instance.CurrentSpeed);
			}
			else
			{
				ChangeTrackBy(trackDelta, 30f / Game.Instance.CurrentSpeed);
			}
		}

		private void ChangeTrackTo(int newTrackIndex)
		{
			int trackDelta = newTrackIndex - TrackIndexCurrent;
			ChangeTrackBy(trackDelta);
		}

		public void ChangeTrackBy(int trackDelta, float duration)
		{
			if (_ignoredMovement == trackDelta)
			{
				return;
			}
			if (_trackMovement != trackDelta)
			{
				ForceChangeTrack(trackDelta, duration);
				if (IsTeleboard && !_game.IsInPogostickMode && !_game.IsInJetpackMode)
				{
					switch (trackDelta)
					{
					case 1:
						Teleboard(new Vector3(20f, 0f, 1f));
						break;
					case -1:
						Teleboard(new Vector3(-20f, 0f, 1f));
						break;
					}
				}
			}
			else
			{
				_trackMovementNext = trackDelta;
			}
		}

		public void ForceChangeTrack(int movement, float duration)
		{
			StopAllCoroutines();
			StartCoroutine(ChangeTrackCoroutine(movement, duration));
		}

		public void SetLastCheckpointGamePositionZ(float gamePositionZ)
		{
			_lastCheckpointGamePositionZ = gamePositionZ;
		}

		public void ForceLeaveSubway()
		{
			subwayColliders.Clear();
			_isInsideSubway = false;
		}

		public float GetTrackX()
		{
			return Route.GetTrackX(TrackIndexCurrent);
		}

		public void JumpAdvanced(float jumpHeight, bool willPlaySound = true, bool willPlayAnimation = true)
		{
			verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			if (_isRolling)
			{
				ExpandColliders();
			}
			IsJumping = true;
			_isFalling = false;
			IsGrounded.Value = false;
			if (willPlayAnimation)
			{
				NotifyOnJump(jumpHeight, willPlaySound);
			}
			StartCoroutine(ToggleDustWithDelay(0.5f));
		}

		private IEnumerator ToggleDustWithDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			if (OnToggleDust != null)
			{
				OnToggleDust(false);
			}
			_longMagnet.Deactivate();
			yield return null;
		}

		public void Jump()
		{
			bool flag = !IsJumping && verticalSpeed <= 0f && verticalSpeed > _verticalSpeed_jumpTolerance;
			bool flag2 = (flag || SetPieceForcedForgivingJump) && Time.timeSinceLevelLoad - SetPieceForgivingJumpTime < 0.15f && !IsGrounded.Value;
			SetPieceForcedForgivingJump = false;
			if (flag2 && OnSetPieceForgivingJump != null)
			{
				OnSetPieceForgivingJump();
			}
			OnSetPieceForgivingJump = null;
			if (_characterController.isGrounded || (flag && !TransitioningFromHeight) || flag2)
			{
				if (IsJumping && _jumpUpperCollisionEvaded)
				{
					_jumpUpperCollision = false;
				}
				if (_isRolling)
				{
					ExpandColliders();
				}
				_glideTimeLeft = 0f;
				_vTolEnded = false;
				_willSecondJump = true;
				IsJumping = true;
				_isFalling = false;
				IsGrounded.Value = false;
				if (IsJumpingHigher)
				{
					Vector3 gamePosition = GamePosition;
					jumpHeightSuperSneakers = _originaljumpHeightSuperSneakers;
					SuperSneakersJump value = default(SuperSneakersJump);
					value.StartGamePositionZ = gamePosition.z;
					value.Length = JumpLength(_game.CurrentSpeed, jumpHeightSuperSneakers) * superSneakersJumpApexRatio;
					value.EndGamePositionZ = value.StartGamePositionZ + value.Length;
					value.StartGamePositionY = gamePosition.y;
					_superSneakersJump = value;
					verticalSpeed = 0f;
				}
				else
				{
					verticalSpeed = CalculateJumpVerticalSpeed((!flag2) ? JumpHeight : SetPieceForgivingJumpHeight);
				}
				NotifyOnJump((!IsJumpingHigher) ? jumpHeightNormal : jumpHeightSuperSneakers, false);
				_isTucked = false;
				if (IsRunningOnGround())
				{
					_startedJumpFromGround = true;
					_trainJump = false;
					_trainJumpSampleGamePositionZ = GamePosition.z + _trainJumpSampleLength;
				}
			}
			else if (AllowDoubleJump())
			{
				if (IsJumping && _jumpUpperCollisionEvaded)
				{
					_jumpUpperCollision = false;
				}
				if (_isRolling)
				{
					ExpandColliders();
				}
				_glideTimeLeft = 0f;
				_vTolEnded = false;
				_willSecondJump = false;
				IsJumping = true;
				_isFalling = false;
				IsGrounded.Value = false;
				NotifyOnDoubleJump();
				_isTucked = false;
				if (IsJumpingHigher)
				{
					Vector3 gamePosition2 = GamePosition;
					jumpHeightSuperSneakers = 18f;
					SuperSneakersJump value2 = default(SuperSneakersJump);
					value2.StartGamePositionZ = gamePosition2.z;
					value2.Length = JumpLength(_game.CurrentSpeed, jumpHeightSuperSneakers) * superSneakersJumpApexRatio;
					value2.EndGamePositionZ = value2.StartGamePositionZ + value2.Length;
					value2.StartGamePositionY = gamePosition2.y;
					_superSneakersJump = value2;
					verticalSpeed = 0f;
				}
				else
				{
					verticalSpeed = CalculateJumpVerticalSpeed(JumpHeight);
				}
				if (IsRunningOnGround())
				{
					_startedJumpFromGround = true;
					_trainJump = false;
					_trainJumpSampleGamePositionZ = GamePosition.z + _trainJumpSampleLength;
				}
			}
			else if (verticalSpeed < 0f)
			{
				inAirJump = true;
			}
		}

		public void CheckInAirJump()
		{
			if (CharacterController.isGrounded && inAirJump)
			{
				Jump();
				inAirJump = false;
			}
		}

		private void Teleboard(Vector3 offset)
		{
			float duration = 17f / _game.CurrentSpeed;
			if (OnTeleboardStart__BackingField != null)
			{
				OnTeleboardStart__BackingField(duration, offset, _game.CurrentSpeed);
			}
		}

		public void Roll()
		{
			if (!_isRolling)
			{
				_isTucked = false;
				_glideTimeLeft = _glideTime;
				SuperSneakersJump? superSneakersJump = _superSneakersJump;
				if (superSneakersJump.HasValue)
				{
					_superSneakersJump = null;
				}
				_squeezeCollider.Add(this);
				if (!TransitioningFromHeight)
				{
					verticalSpeed = 0f - CalculateJumpVerticalSpeed(JumpHeight);
				}
				else
				{
					verticalSpeed = -250f;
				}
				_isRolling = true;
				inAirJump = false;
				NotifyOnRoll();
			}
		}

		public void EnterRoute()
		{
			RaiseOnEnterRoute();
		}

		public void EnterRouteEnd()
		{
			RaiseOnEnterRouteEnd();
		}

		public void ExitRoute()
		{
			RaiseExitRoute();
		}

		public void ApplyGravity()
		{
			if (verticalSpeed < 0f && CharacterController.isGrounded)
			{
				if (_startedJumpFromGround && _trainJump && IsRunningOnGround())
				{
					NotifyOnJumpOverTrain();
				}
				if (CurrentRunPosition != RunPositions.air)
				{
					_startedJumpFromGround = false;
				}
				IsGrounded.Value = true;
				if (IsJumping || _isFalling)
				{
					_jumpUpperCollision = false;
					_jumpUpperCollisionEvaded = true;
					IsJumping = false;
					_isFalling = false;
					_isTucked = false;
					NotifyOnLanding();
				}
				verticalSpeed = 0f;
			}
			else if (_startedJumpFromGround && _trainJumpSampleGamePositionZ < GamePosition.z)
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(new Ray(TransformPosition, -Vector3.up), out hitInfo) && (hitInfo.collider.CompareTag("HitMovingTrain") || hitInfo.collider.CompareTag("HitTrain")))
				{
					_trainJump = true;
				}
				_trainJumpSampleGamePositionZ += _trainJumpSampleLength;
			}
			if (IsFalling && IsGliderboard)
			{
				if (_glideTimeLeft < _glideTime)
				{
					verticalSpeed = gliderGravity * _glideVerticalSpeed.Evaluate(_glideTimeLeft / _glideTime);
					_glideTimeLeft += Time.deltaTime;
				}
				else
				{
					verticalSpeed -= gliderGravity * Time.deltaTime;
				}
			}
			else if (IsFalling && IsVTol)
			{
				if (_glideTimeLeft < _glideTime)
				{
					verticalSpeed = 0f;
					_glideTimeLeft += Time.deltaTime;
				}
				else
				{
					if (!_vTolEnded)
					{
						if (OnVTolEnd__BackingField != null)
						{
							OnVTolEnd__BackingField();
						}
						_vTolEnded = true;
					}
					verticalSpeed -= gravity * Time.deltaTime;
				}
			}
			else
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
			if (!CharacterController.isGrounded && !_isFalling && verticalSpeed < verticalFallSpeedLimit && !_isRolling)
			{
				_isFalling = true;
				_isTucked = false;
				NotifyOnHangtime();
				IsGrounded.Value = false;
			}
		}

		public void MoveWithGravity()
		{
			if (CharacterController.enabled)
			{
				verticalSpeed -= gravity * Time.deltaTime;
				if (verticalSpeed > 0f)
				{
					verticalSpeed = 0f;
				}
				Vector3 motion = verticalSpeed * Time.deltaTime * Vector3.up;
				CharacterController.Move(motion);
				CachePositions();
			}
		}

		public void MoveForward()
		{
			Vector3 gamePosition = GamePosition;
			float num = gamePosition.z + _game.CurrentSpeed * Time.deltaTime;
			Vector3 vector = Vector3.zero;
			bool flag = true;
			if (IsJumping && _jumpUpperCollision)
			{
				flag = false;
				if (!_jumpUpperCollisionEvaded && _jumpUpperCollisionColliderMinY < CharacterTrigger.bounds.max.y)
				{
					if (0f <= verticalSpeed)
					{
						vector = (0f - gravity) * Time.deltaTime * Time.deltaTime * Vector3.up;
					}
				}
				else
				{
					vector = Vector3.zero;
					_jumpUpperCollisionEvaded = true;
				}
			}
			if (verticalSpeed < 0f || flag)
			{
				vector = verticalSpeed * Time.deltaTime * Vector3.up;
			}
			if (IsJumping && _superSneakersJump.HasValue)
			{
				SuperSneakersJump value = _superSneakersJump.Value;
				if (GamePosition.z < value.EndGamePositionZ)
				{
					if (flag)
					{
						float num2 = superSneakersJumpCurve.Evaluate((num - value.StartGamePositionZ) / value.Length) * jumpHeightSuperSneakers + value.StartGamePositionY;
						float num3 = num2 - gamePosition.y;
						vector = Vector3.up * num3;
					}
					else
					{
						vector = Vector3.zero;
					}
				}
				else
				{
					_superSneakersJump = null;
					verticalSpeed = 0f;
					vector = Vector3.zero;
				}
			}
			Vector3 position = Route.GetPosition(x, num);
			Vector3 vector2 = new Vector3(gamePosition.x, 0f, gamePosition.z);
			Vector3 vector3 = position - vector2;
			if (CharacterController.enabled)
			{
				CharacterController.Move(vector + vector3);
			}
			else
			{
				base.transform.position = TransformPosition + vector3;
			}
			CachePositions();
			if (CharacterController.isGrounded)
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(TransformPosition, Vector3.down, out hitInfo, 2f))
				{
					LastGroundedY = hitInfo.point.y + _characterControllerSkinWidth;
				}
			}
			else
			{
				RaycastHit hitInfo2;
				if (Physics.Raycast(TransformPosition, Vector3.down, out hitInfo2, 50f, 1 << LayerMask.NameToLayer("Ramp"), QueryTriggerInteraction.Ignore) && hitInfo2.point.y > LastGroundedY)
				{
					LastGroundedY = hitInfo2.point.y;
				}
				if (Physics.Raycast(TransformPosition, Vector3.forward, out hitInfo2, 50f, 1 << LayerMask.NameToLayer("Ramp"), QueryTriggerInteraction.Ignore) && hitInfo2.point.y > LastGroundedY)
				{
					LastGroundedY = Mathf.Lerp(LastGroundedY, hitInfo2.point.y, (1f - hitInfo2.distance / 50f) * 0.25f);
				}
			}
			_movedSinceLastFixedUpdate = true;
		}

		public void CachePositions()
		{
			_transformPosition = base.transform.position;
			_gamePosition = GamePositionRoot.Instance.GetGamePosition(base.transform);
		}

		public void EndRoll()
		{
			ExpandColliders();
			_isRolling = false;
		}

		public float CalculateJumpVerticalSpeed(float jumpHeight)
		{
			if (jumpHeight < 0f)
			{
				SDebug.LogError("JumpHeight cannot be negative!", base.gameObject);
			}
			return Mathf.Sqrt(2f * jumpHeight * gravity);
		}

		public float CalculateJumpVerticalSpeed()
		{
			return CalculateJumpVerticalSpeed(JumpHeight);
		}

		public float JumpLength(float speed, float jumpHeight)
		{
			return speed * 2f * CalculateJumpVerticalSpeed(jumpHeight) / gravity;
		}

		public void StopStumble()
		{
			_followingGuard.ResetCatchUp();
			_isStumbling = false;
		}

		private IEnumerator ChangeTrackCoroutine(int move, float duration)
		{
			if (_ignoredMovement == move)
			{
				yield break;
			}
			_trackMovement = move;
			_trackMovementNext = 0;
			int newTrackIndex = _trackIndexTarget + move;
			float trackChangeIndexDistance = Mathf.Abs((float)newTrackIndex - _trackIndexPosition);
			float trackIndexPositionBegin = _trackIndexPosition;
			float startX = x;
			float endX = Route.GetTrackX(newTrackIndex);
			float dir = Mathf.Sign(newTrackIndex - _trackIndexTarget);
			float startRotation = _characterRotation;
			OnChangeTrackDirection trackChangeDir = ((move >= 0) ? OnChangeTrackDirection.Right : OnChangeTrackDirection.Left);
			if (newTrackIndex < 0)
			{
				SetLastObstacleType(ObstacleType.Sides);
				if (_stumbleState == StumbleState.Standard)
				{
					HandleStumble(StumbleType.Side, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, "side");
				}
				else if (_stumbleState == StumbleState.Safe)
				{
					RequestSafeStumble(StumbleType.Side, (move <= 0) ? StumbleHorizontalHit.Right : StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, "setpiece_side");
				}
				yield break;
			}
			if (newTrackIndex >= 3)
			{
				SetLastObstacleType(ObstacleType.Sides);
				if (_stumbleState == StumbleState.Standard)
				{
					HandleStumble(StumbleType.Side, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, "side");
				}
				else if (_stumbleState == StumbleState.Safe)
				{
					RequestSafeStumble(StumbleType.Side, (move <= 0) ? StumbleHorizontalHit.Right : StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, "setpiece_side");
				}
				yield break;
			}
			if (OnChangeTrack__BackingField != null)
			{
				OnChangeTrack__BackingField(trackChangeDir);
			}
			_trackIndexTarget = newTrackIndex;
			yield return StartCoroutine(pTween.To(trackChangeIndexDistance * duration, (float t) =>
			{
				_trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, newTrackIndex, t);
				x = Mathf.Lerp(startX, endX, t);
				_characterRotation = pMath.Bell(t) * dir * CharacterAngle + Mathf.Lerp(startRotation, 0f, t);
				CharacterRoot.localRotation = Quaternion.Euler(0f, _characterRotation, 0f);
			}));
			if (move != _ignoredMovement)
			{
				_ignoredMovement = 0;
			}
			TrackIndexCurrent = newTrackIndex;
			_trackMovement = 0;
			if (_trackMovementNext != 0)
			{
				StartCoroutine(ChangeTrackCoroutine(_trackMovementNext, duration));
			}
		}

		private ObstacleType GetObstacleTypeFromTag(Collider obstacleCollider)
		{
			switch (obstacleCollider.tag)
			{
			case "Stumble":
				return ObstacleType.MinorObstacle;
			case "HitTrain":
				return ObstacleType.Train;
			case "HitMovingTrain":
				return ObstacleType.MovingTrain;
			case "HitTrainRamp":
				return ObstacleType.TrainRamp;
			case "HitBarrierStandard":
				return ObstacleType.BarrierStandard;
			case "HitBarrierRoll":
				return ObstacleType.BarrierRoll;
			case "HitBarrierJump":
				return ObstacleType.BarrierJump;
			case "HitGates":
				return ObstacleType.Gates;
			case "HitPillar":
				return ObstacleType.Pillar;
			case "HitStationPlatform":
				return ObstacleType.StationPlatform;
			case "HitBlocker":
				return ObstacleType.Blocker;
			default:
				return ObstacleType.None;
			}
		}

		private ObstaclePassType GetPassTypeFromCollider(Collider obstacleCollider)
		{
			switch (obstacleCollider.tag)
			{
			case "RollBarrier":
				return ObstaclePassType.Roll;
			case "JumpBarrier":
				return ObstaclePassType.Jump;
			case "JumpHighBarrier":
				return ObstaclePassType.JumpHigh;
			default:
				return ObstaclePassType.None;
			}
		}

		private void OnCharacterTriggerExit(Collider collider)
		{
			if (collider.CompareTag("Subway"))
			{
				if (subwayColliders.Contains(collider))
				{
					subwayColliders.Remove(collider);
					_isInsideSubway = subwayColliders.Count > 0;
				}
				return;
			}
			ObstaclePassType passTypeFromCollider = GetPassTypeFromCollider(collider);
			if (passTypeFromCollider == _lastObstaclePassType && _lastObstaclePassTrackIndex == TrackIndexCurrent && OnPassedObstacle__BackingField != null)
			{
				OnPassedObstacle__BackingField(passTypeFromCollider);
			}
			if (passTypeFromCollider == ObstaclePassType.Roll && OnRollGuard__BackingField != null)
			{
				OnRollGuard__BackingField();
			}
		}

		private void OnCharacterTriggerEnter(Collider collider)
		{
			hitPoistionDistance = TransformPosition.z;
			if (!_game.IsInGame.Value)
			{
				return;
			}
			if (collider.CompareTag("Subway"))
			{
				subwayColliders.Add(collider);
				_isInsideSubway = subwayColliders.Count > 0;
				SubwayMaxY = collider.bounds.max.y - 3f;
			}
			else
			{
				if (StopColliding || collider.gameObject.layer == _layers.Unremovable || _game.isInStumbleDeathSequence)
				{
					return;
				}
				if (collider.gameObject.layer == Layers.Instance.Pickup)
				{
					PickupSpawnPoint component = collider.GetComponent<PickupSpawnPoint>();
					if (component != null && component.CanTriggerCollisionWithCharacter)
					{
						component.TriggerCollisionWithCharacter(CharacterPickupParticleSystem);
					}
				}
				Coin component2 = collider.GetComponent<Coin>();
				if (component2 != null)
				{
					component2.Pickup(CharacterPickupParticleSystem);
				}
				ObstacleType obstacleType = ObstacleType.None;
				if (collider.gameObject.layer == _layers.Hit)
				{
					obstacleType = GetObstacleTypeFromTag(collider);
					if (obstacleType != ObstacleType.None)
					{
						SetLastObstacleType(obstacleType);
					}
				}
				if (collider.gameObject.layer == _layers.Default || collider.gameObject.layer == _layers.Ramp)
				{
					if (collider.isTrigger && CharacterController.isGrounded)
					{
						IsGrounded.Value = true;
					}
					if (collider.isTrigger)
					{
						ObstaclePassType passTypeFromCollider = GetPassTypeFromCollider(collider);
						if (passTypeFromCollider != ObstaclePassType.None)
						{
							_lastObstaclePassType = passTypeFromCollider;
							_lastObstaclePassTrackIndex = TrackIndexCurrent;
						}
					}
				}
				else if (collider.isTrigger)
				{
					if (collider.gameObject.layer == _layers.Hit)
					{
						if (collider.CompareTag("Stumble"))
						{
							HandleStumble(StumbleType.Bush, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
						}
						else
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
						}
					}
				}
				else
				{
					if (DebugSettings.IsAllowed && DebugSettings.CharacterInvincible)
					{
						return;
					}
					_lastHitTag = collider.tag;
					ImpactX obstacleImpactX = GetObstacleImpactX(collider);
					ImpactY characterImpactY = GetCharacterImpactY(collider);
					ImpactZ obstacleImpactZ = GetObstacleImpactZ(collider);
					float num = (collider.bounds.min.x + collider.bounds.max.x) / 2f;
					float num2 = TransformPosition.x;
					int num3 = ((num2 < num) ? 1 : ((num2 > num) ? (-1) : 0));
					bool flag = num3 == 0 || _trackMovement == num3;
					bool flag2 = CharacterTrigger.bounds.center.z < collider.bounds.min.z;
					bool flag3 = obstacleImpactZ == ImpactZ.Before && !flag2 && flag;
					bool jumpUpperCollision = _jumpUpperCollision;
					if (IsJumping && characterImpactY == ImpactY.Upper && !CharacterController.isGrounded)
					{
						_jumpUpperCollision = true;
						_jumpUpperCollisionEvaded = false;
						verticalSpeed = 0f;
						if (!jumpUpperCollision)
						{
							_jumpUpperCollisionColliderMinY = collider.bounds.min.y;
						}
					}
					if (obstacleImpactZ == ImpactZ.Middle || flag3)
					{
						if (_trackMovement != 0)
						{
							float duration = 0.5f;
							if (_game.IsRunningOnTutorialTrack)
							{
								duration = 0.2f;
							}
							ChangeTrackBy(-_trackMovement, duration);
						}
						switch (obstacleImpactX)
						{
						case ImpactX.Left:
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, collider.name);
							break;
						case ImpactX.Right:
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, collider.name);
							break;
						}
						return;
					}
					if (obstacleImpactX == ImpactX.Middle || _trackMovement == 0)
					{
						if (obstacleImpactZ != ImpactZ.Before)
						{
							return;
						}
						if (characterImpactY == ImpactY.Lower)
						{
							if (!IsGliderboard)
							{
								verticalSpeed = CalculateJumpVerticalSpeed(8f);
							}
							else
							{
								verticalSpeed = CalculateJumpVerticalSpeed(1.5f);
							}
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
								return;
							}
							GameStats.Instance.StumbleCountIncrement();
							OnResetTeleportFX__BackingField();
							return;
						}
						if (collider.gameObject.CompareTag("HitMovingTrain"))
						{
							if (GjNoDeathActive)
							{
								GjSurviveFatalHit(collider);
								return;
							}
							HitByTrainSequence();
							NotifyCriticalHit();
							return;
						}
						if (characterImpactY == ImpactY.Middle)
						{
							if (GjNoDeathActive)
							{
								GjSurviveFatalHit(collider);
								return;
							}
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name, false, true);
							}
							else
							{
								OnResetTeleportFX__BackingField();
							}
							NotifyCriticalHit();
							return;
						}
						bool flag4 = jumpUpperCollision && _jumpUpperCollisionColliderMinY <= collider.bounds.min.y;
						bool flag5 = EnteredColliderFromBelow(collider, jumpUpperCollision);
						if (!IsJumping || ((!_jumpUpperCollision || !flag5) && !flag4))
						{
							if (GjNoDeathActive)
							{
								GjSurviveFatalHit(collider);
								return;
							}
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Upper, collider.name, false, true);
							}
							else
							{
								GameStats.Instance.StumbleCountIncrement();
								OnResetTeleportFX__BackingField();
							}
							NotifyCriticalHit();
						}
						return;
					}
					if (obstacleImpactZ == ImpactZ.Before && flag)
					{
						if (collider.gameObject.CompareTag("HitMovingTrain"))
						{
							if (GjNoDeathActive)
							{
								GjSurviveFatalHit(collider);
								return;
							}
							HitByTrainSequence();
							NotifyCriticalHit();
						}
						else if (collider.CompareTag("HitTrainRamp"))
						{
							if (!IsTeleboard)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
							}
						}
						else
						{
							ForceChangeTrack(-_trackMovement, 0.5f);
						}
					}
					else if (collider.CompareTag("HitTrainRamp"))
					{
						ForceChangeTrack(-_trackMovement, 0.5f);
					}
					switch (obstacleImpactX)
					{
					case ImpactX.Left:
						if (!IsTeleboard)
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.LeftCorner, StumbleVerticalHit.Middle, collider.name);
						}
						break;
					case ImpactX.Right:
						if (!IsTeleboard)
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.RightCorner, StumbleVerticalHit.Middle, collider.name);
						}
						break;
					}
				}
			}
		}

		private bool EnteredColliderFromBelow(Collider collider, bool jumpUpperCollisionRepeated)
		{
			Bounds bounds = CharacterTrigger.bounds;
			Vector3 vector = bounds.max - 0.5f * new Vector3(bounds.size.x, 0f, bounds.size.z);
			Vector3 normalized = (collider.bounds.min - vector).normalized;
			Vector3 lhs = Vector3.Cross(normalized, Vector3.left);
			Vector3 vector2 = _posFixedUpdateLast - _posFixedUpdatePrevious;
			float num = Vector3.Dot(lhs, -vector2);
			return num < 0f;
		}

		private void HitByTrainSequence()
		{
			if (Hoverboard.isActive)
			{
				NotifyOnJumpIfHitByTrain();
			}
			else
			{
				NotifyOnHitByTrain();
			}
		}

		private ImpactX GetObstacleImpactX(Collider collider)
		{
			Bounds bounds = CharacterTrigger.bounds;
			Bounds bounds2 = collider.bounds;
			float num = Mathf.Max(bounds.min.x, bounds2.min.x);
			float num2 = Mathf.Min(bounds.max.x, bounds2.max.x);
			float num3 = (num + num2) * 0.5f;
			float num4 = num3 - bounds2.min.x;
			if ((double)num4 > (double)bounds2.size.x - (double)ColliderTrackWidth * 0.33)
			{
				return ImpactX.Right;
			}
			if ((double)num4 < (double)ColliderTrackWidth * 0.33)
			{
				return ImpactX.Left;
			}
			return ImpactX.Middle;
		}

		private ImpactZ GetObstacleImpactZ(Collider collider)
		{
			Vector3 transformPosition = TransformPosition;
			Bounds bounds = collider.bounds;
			if (transformPosition.z > bounds.max.z - ((!(bounds.max.z - bounds.min.z > 30f)) ? ((bounds.max.z - bounds.min.z) * 0.5f) : stumbleCornerTolerance))
			{
				return ImpactZ.After;
			}
			if (transformPosition.z < bounds.min.z + stumbleCornerTolerance)
			{
				return ImpactZ.Before;
			}
			return ImpactZ.Middle;
		}

		private ImpactY GetCharacterImpactY(Collider collider)
		{
			Bounds bounds = CharacterTrigger.bounds;
			Bounds bounds2 = collider.bounds;
			float num = Mathf.Max(bounds.min.y, bounds2.min.y);
			float num2 = Mathf.Min(bounds.max.y, bounds2.max.y);
			float num3 = (num + num2) * 0.5f;
			float num4 = (num3 - bounds.min.y) / bounds.size.y;
			if (num4 < 0.33f)
			{
				return ImpactY.Lower;
			}
			if (num4 < 0.66f)
			{
				return ImpactY.Middle;
			}
			return ImpactY.Upper;
		}

		private void ExecuteKillSwitch()
		{
			// "Não Morrer": se travou num obstáculo, só deixa atravessar em vez de matar
			if (GjNoDeathActive)
			{
				GjPassThroughObstacles();
				return;
			}
			LastDeathCause = DeathCause.KillSwitch;
			_game.Die();
		}

		// ---- "Não Morrer" (Configurações) ----
		// A batida conta normal (tropeça e o guarda vem atrás), mas a que mataria não mata:
		// por 1 segundo os obstáculos deixam de segurar o personagem e ele atravessa. Nada some da pista e não tem pulo.
		private const float GjPassThroughSeconds = 1f;

		private float _gjPassThroughUntil;

		private bool GjNoDeathActive
		{
			get
			{
				return TrainingOptions.NoDeath && _game.IsInGame.Value && !_game.IsRunningOnTutorialTrack;
			}
		}

		private void GjSurviveFatalHit(Collider collider)
		{
			if (Time.time < _gjPassThroughUntil)
			{
				return;
			}
			if (!IsTeleboard && !_game.IsInJetpackMode && !_game.IsInPogostickMode)
			{
				GameStats.Instance.StumbleCountIncrement();
				NotifyOnStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name, true);
				StartStumble(true);
			}
			GjPassThroughObstacles();
		}

		private void GjPassThroughObstacles()
		{
			_killSwitchUpdates = 10;
			_gjPassThroughUntil = Time.time + GjPassThroughSeconds;
			Physics.IgnoreLayerCollision(_layers.Character, _layers.Hit, true);
			StopCoroutine("GjRestoreObstacleCollision");
			StartCoroutine("GjRestoreObstacleCollision");
		}

		private IEnumerator GjRestoreObstacleCollision()
		{
			yield return new WaitForSeconds(GjPassThroughSeconds);
			Physics.IgnoreLayerCollision(_layers.Character, _layers.Hit, false);
		}

		private bool AllowDoubleJump()
		{
			if (IsDoubleJumping && _willSecondJump)
			{
				return true;
			}
			return false;
		}

		private bool IsRunningFromTrain()
		{
			return CurrentRunPosition == RunPositions.train || CurrentRunPosition == RunPositions.movingTrain;
		}

		private bool IsRunningOnGround()
		{
			return CurrentRunPosition == RunPositions.ground;
		}

		private void ExpandColliders()
		{
			if (CharacterController.enabled)
			{
				CharacterController.Move(Vector3.up * 2f);
			}
			_squeezeCollider.Remove(this);
			if (CharacterController.enabled)
			{
				CharacterController.Move(Vector3.down * 2f);
			}
			CachePositions();
		}

		private void StartStumble(bool safeStumble = false)
		{
			_isStumbling = !safeStumble;
			if (!_game.IsRunningOnTutorialTrack)
			{
				_followingGuard.CatchUp();
			}
			_followingGuard.StartCoroutine(StumbleDecay());
		}

		private IEnumerator StumbleDecay()
		{
			yield return new WaitForSeconds(stumbleDecayTime);
			StopStumble();
		}

		public void RequestStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
		{
			HandleStumble(stumbleType, horizontalHit, verticalHit, colliderName);
		}

		public void RequestSafeStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
		{
			HandleStumble(stumbleType, horizontalHit, verticalHit, colliderName, true);
			_isStumbling = false;
		}

		private void HandleStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName, bool safeStumble = false, bool isCriticalHit = false)
		{
			if (!isCriticalHit)
			{
				GameStats.Instance.StumbleCountIncrement();
			}
			if (!_game.IsInJetpackMode && !_game.IsInPogostickMode)
			{
				NotifyOnStumble(stumbleType, horizontalHit, verticalHit, colliderName, safeStumble);
				StartStumble(safeStumble);
			}
		}

		private IEnumerator LaneSwitchJump(Vector3 gamePositionStart, Vector3 gamePositionEnd, float jumpHeight, float jumpsPerLaneSwitch)
		{
			bool hasLanded = false;
			bool jumpDone = false;
			NotifyOnJump(jumpHeight, false);
			while (!jumpDone)
			{
				Vector3 p = GamePosition;
				float z = p.z;
				float t = (z - gamePositionStart.z) / (gamePositionEnd.z - gamePositionStart.z);
				float yt = t / jumpsPerLaneSwitch;
				float y = gamePositionStart.y + 4f * yt * (1f - yt) * jumpHeight;
				p.y = y;
				SetGamePosition(p);
				if (t >= 1f)
				{
					jumpDone = true;
				}
				if (!hasLanded && t >= 0.8f)
				{
					NotifyOnLanding();
					hasLanded = true;
				}
				yield return null;
			}
		}

		private void NotifyCriticalHit()
		{
			LastDeathCause = DeathCause.CriticalHit;
			if (OnCriticalHit__BackingField != null)
			{
				CriticalHitType type;
				switch (_lastHitTag)
				{
				case "HitTrain":
					type = CriticalHitType.Train;
					break;
				case "HitBarrierStandard":
				case "HitBarrierJump":
				case "HitBarrierRoll":
					type = CriticalHitType.Barrier;
					break;
				case "HitMovingTrain":
					type = CriticalHitType.MovingTrain;
					break;
				default:
					type = CriticalHitType.None;
					break;
				}
				OnCriticalHit__BackingField(type);
			}
		}

		private void NotifyOnStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName, bool safeStumble)
		{
			if (OnStumble__BackingField != null)
			{
				OnStumble__BackingField(stumbleType, horizontalHit, verticalHit, colliderName, safeStumble);
			}
		}

		private void NotifyOnCriticalHit(CriticalHitType type)
		{
			if (OnCriticalHit__BackingField != null)
			{
				OnCriticalHit__BackingField(type);
			}
		}

		private void NotifyOnJump(float jumpHeight, bool willPlaySound)
		{
			if (OnJump__BackingField != null)
			{
				OnJump__BackingField(jumpHeight, willPlaySound);
			}
		}

		private void NotifyOnDoubleJump()
		{
			if (OnDoubleJump__BackingField != null)
			{
				OnDoubleJump__BackingField();
			}
		}

		private void NotifyOnJumpIfHitByTrain()
		{
			if (OnJumpIfHitByTrain__BackingField != null)
			{
				OnJumpIfHitByTrain__BackingField();
			}
		}

		public void NotifyOnLanding()
		{
			if (OnLanding__BackingField != null)
			{
				OnLanding__BackingField(this);
			}
		}

		private void NotifyOnHangtime()
		{
			if (OnHangtime__BackingField != null)
			{
				OnHangtime__BackingField();
			}
		}

		private void NotifyOnRoll()
		{
			if (OnRoll__BackingField != null)
			{
				OnRoll__BackingField();
			}
		}

		private void NotifyOnHitByTrain()
		{
			if (OnHitByTrain__BackingField != null)
			{
				OnHitByTrain__BackingField();
			}
		}

		private void NotifyOnChangeTrack(OnChangeTrackDirection direction)
		{
			if (OnChangeTrack__BackingField != null)
			{
				OnChangeTrack__BackingField(direction);
			}
		}

		public void NotifyOnTutorialMoveBackToCheckPoint(float duration)
		{
			if (OnTutorialMoveBackToCheckPoint__BackingField != null)
			{
				OnTutorialMoveBackToCheckPoint__BackingField(duration);
			}
		}

		public void NotifyOnTutorialStartFromCheckPoint()
		{
			if (OnTutorialStartFromCheckPoint__BackingField != null)
			{
				OnTutorialStartFromCheckPoint__BackingField();
			}
		}

		private void NotifyOnJumpOverTrain()
		{
			if (OnJumpOverTrain__BackingField != null)
			{
				OnJumpOverTrain__BackingField();
			}
		}

		public void ToggleDust(bool active)
		{
			RaiseOnToggleDust(active);
		}

		private void RaiseOnToggleDust(bool active)
		{
			if (OnToggleDust != null)
			{
				OnToggleDust(active);
			}
		}

		public void RaiseOnEnterRoute()
		{
			if (OnEnterRoute != null)
			{
				OnEnterRoute();
			}
		}

		public void RaiseOnEnterRouteEnd()
		{
			if (OnEnterRouteEnd != null)
			{
				OnEnterRouteEnd();
			}
		}

		private void RaiseExitRoute()
		{
			if (OnExitRoute != null)
			{
				OnExitRoute();
			}
		}

		private void IsGrounded_OnChange(bool value)
		{
			if (IsInLipFlight)
			{
				IsInLipFlight = false;
			}
		}

		private void RouteManager_OnRouteCreated(object sender, RouteManager.RouteCreatedEventArgs args)
		{
			AlternateRoute alternateRoute = args.Route as AlternateRoute;
			if (alternateRoute != null)
			{
				SubscribeToAlternateRoute(alternateRoute);
			}
		}

		private void SubscribeToAlternateRoute(AlternateRoute alternateRoute)
		{
			alternateRoute.OnCharacterEntryStarted += AlternateRoute_OnCharacterEntryStarted;
			alternateRoute.OnCharacterEntrySkipped += AlternateRoute_OnCharacterEntrySkipped;
			alternateRoute.OnCharacterEntered += AlternateRoute_OnCharacterEntered;
			alternateRoute.OnDestructing += AlternateRoute_OnDestructing;
		}

		private void AlternateRoute_OnCharacterEntryStarted(object sender, EventArgs args)
		{
		}

		private void AlternateRoute_OnDestructing(object sender, EventArgs args)
		{
			if (!UIScreenController.Instance.GetTopScreenName().Equals("IngameUI"))
			{
				ResetAfterRoute(true);
			}
			AlternateRoute alternateRoute = sender as AlternateRoute;
			if (alternateRoute != null)
			{
				alternateRoute.OnCharacterEntered -= AlternateRoute_OnCharacterEntered;
				alternateRoute.OnDestructing -= AlternateRoute_OnDestructing;
				alternateRoute.OnCharacterEntrySkipped -= AlternateRoute_OnCharacterEntrySkipped;
			}
		}

		private void AlternateRoute_OnCharacterEntered(object sender, EventArgs args)
		{
			_characterModel.IsVisibleShadow = false;
			CurrentStumbleState = StumbleState.Safe;
		}

		private void AlternateRoute_OnCharacterEntrySkipped(object sender, Route.CharacterEntrySkippedEventArgs args)
		{
			if (args.HasCharacterStartedEntryTransition)
			{
				StartCoroutine(ResetAfterRouteDelayed(false));
			}
		}

		private IEnumerator ResetAfterRouteDelayed(bool isRouteDestructing)
		{
			yield return null;
			ResetAfterRoute(isRouteDestructing);
		}

		public void ResetAfterRoute(bool isRouteDestructing)
		{
			if (OnRouteDone != null)
			{
				OnRouteDone(isRouteDestructing);
			}
			if (!_game.Modifiers.Hoverboard.isActive)
			{
				_characterModel.RemoveCurrentHoverboard();
			}
			if (!isRouteDestructing)
			{
				_game.Modifiers.Resume();
				_game.EnableEnemy();
			}
			else
			{
				_game.Modifiers.Reset();
			}
		}
	}
}
