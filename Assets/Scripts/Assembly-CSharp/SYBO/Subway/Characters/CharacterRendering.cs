using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Managers;
using SYBO.Common;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Level;
using SYBO.Subway.Routes;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using UnityEngine;

namespace SYBO.Subway.Characters
{
	public class CharacterRendering : MonoBehaviour
	{
		public class Animations
		{
			public List<string> TOP_MENU = new List<string> { "topMenu" };

			public List<string> INTRO_RUN = new List<string> { "introRun" };

			public List<string> RUN = new List<string> { "run" };

			public List<string> LAND = new List<string> { "land" };

			public List<string> BIG_LAND = new List<string> { "land" };

			public List<string> SMALL_LAND = new List<string> { "land" };

			public List<string> JUMP = new List<string> { "jump" };

			public List<string> SMALL_JUMP = new List<string> { "jump" };

			public List<string> TUCK = new List<string> { "tuck" };

			public List<string> HANGTIME = new List<string> { "hangtime" };

			public List<string> DOUBLEJUMP = new List<string> { "jump" };

			public List<string> DOUBLEHANGTIME = new List<string> { "hangtime" };

			public List<string> ROLL = new List<string> { "roll" };

			public List<string> DODGE_LEFT = new List<string> { "dodgeLeft" };

			public List<string> DODGE_RIGHT = new List<string> { "dodgeRight" };

			public List<string> TURBO_HEADSTART1 = new List<string> { "dodgeRight" };

			public List<string> TURBO_HEADSTART2 = new List<string> { "dodgeRight" };

			public List<string> STARTGRIND = new List<string> { "grind" };

			public List<string> GRIND = new List<string> { "grind" };

			public List<string> GET_ON_BOARD = new List<string> { "getOnBoard" };

			public List<string> HIT_MID = new List<string> { "hitMid" };

			public List<string> HIT_UPPER = new List<string> { "hitUpper" };

			public List<string> HIT_LOWER = new List<string> { "hitLower" };

			public List<string> HIT_MOVING = new List<string> { "hitMoving" };

			public List<string> STUMBLE = new List<string> { "stumble_low" };

			public List<string> STUMBLEDEATH = new List<string> { "stumbleFall" };

			public List<string> STUMBLE_MIX = new List<string> { "stumble" };

			public List<string> STUMBLE_LEFT_SIDE = new List<string> { "stumbleLeftSide" };

			public List<string> STUMBLE_RIGHT_SIDE = new List<string> { "stumbleRightSide" };

			public List<string> STUMBLE_LEFT_CORNER = new List<string> { "stumbleLeftCorner" };

			public List<string> STUMBLE_RIGHT_CORNER = new List<string> { "stumbleRightCorner" };

			public List<string> HOLDMAGNET = new List<string> { "hold_magnet" };

			public List<string> WALLRIDELEFT = new List<string> { "grind" };

			public List<string> WALLRIDELEFTEXIT = new List<string> { "grind" };

			public List<string> WALLRIDERIGHT = new List<string> { "grind" };

			public List<string> WALLRIDERIGHTEXIT = new List<string> { "grind" };

			public List<string> WALLGRINDLEFT = new List<string> { "grind" };

			public List<string> WALLGRINDLEFTEXIT = new List<string> { "grind" };

			public List<string> WALLGRINDRIGHT = new List<string> { "grind" };

			public List<string> WALLGRINDRIGHTEXIT = new List<string> { "grind" };

			public List<string> ROUTE_CELEBRATE = new List<string> { "routeCelebrate" };

			public List<string> BIG_AIR_TRICK = new List<string> { "bigAirTrick" };

			public List<string> ROUTE_ENTER = new List<string> { "routeEnter" };

			public List<string> ROUTE_EXIT = new List<string> { "routeExit" };

			public string DEFAULT_HOVERBOARD_ANIMATION;

			private List<string> _animationNamesFiltered = new List<string>();

			private List<string> _animationNamesExcluded = new List<string>();

			public string TopMenu
			{
				get
				{
					return GetRandomAnimationName(TOP_MENU);
				}
			}

			public string IntroRun
			{
				get
				{
					return GetRandomAnimationName(INTRO_RUN);
				}
			}

			public string Run
			{
				get
				{
					return GetRandomAnimationName(RUN);
				}
			}

			public string Land
			{
				get
				{
					return GetRandomAnimationName(LAND);
				}
			}

			public string BigLand
			{
				get
				{
					return GetRandomAnimationName(BIG_LAND);
				}
			}

			public string SmallLand
			{
				get
				{
					return GetRandomAnimationName(SMALL_LAND);
				}
			}

			public string Jump
			{
				get
				{
					return GetRandomAnimationName(JUMP);
				}
			}

			public string SmallJump
			{
				get
				{
					return GetRandomAnimationName(SMALL_JUMP);
				}
			}

			public string Tuck
			{
				get
				{
					return GetRandomAnimationName(TUCK);
				}
			}

			public string Hangtime
			{
				get
				{
					return GetRandomAnimationName(HANGTIME);
				}
			}

			public string DoubleJump
			{
				get
				{
					return GetRandomAnimationName(DOUBLEJUMP);
				}
			}

			public string DoubleHangtime
			{
				get
				{
					return GetRandomAnimationName(DOUBLEHANGTIME);
				}
			}

			public string TurboHeadstart1
			{
				get
				{
					return GetRandomAnimationName(TURBO_HEADSTART1);
				}
			}

			public string TurboHeadstart2
			{
				get
				{
					return GetRandomAnimationName(TURBO_HEADSTART2);
				}
			}

			public List<string> HoverboardJump
			{
				get
				{
					return GetRandomHoverJumps(JUMP, HANGTIME);
				}
			}

			public string Roll
			{
				get
				{
					return GetRandomAnimationName(ROLL);
				}
			}

			public string DodgeLeft
			{
				get
				{
					return GetRandomAnimationName(DODGE_LEFT);
				}
			}

			public string DodgeRight
			{
				get
				{
					return GetRandomAnimationName(DODGE_RIGHT);
				}
			}

			public string StartGrind
			{
				get
				{
					return GetRandomAnimationName(STARTGRIND);
				}
			}

			public string GetOnBoard
			{
				get
				{
					return GetRandomAnimationName(GET_ON_BOARD);
				}
			}

			public string HitMid
			{
				get
				{
					return GetRandomAnimationName(HIT_MID);
				}
			}

			public string HitUpper
			{
				get
				{
					return GetRandomAnimationName(HIT_UPPER);
				}
			}

			public string HitLower
			{
				get
				{
					return GetRandomAnimationName(HIT_LOWER);
				}
			}

			public string HitMoving
			{
				get
				{
					return GetRandomAnimationName(HIT_MOVING);
				}
			}

			public string Stumble
			{
				get
				{
					return GetRandomAnimationName(STUMBLE);
				}
			}

			public string StumbleDeath
			{
				get
				{
					return GetRandomAnimationName(STUMBLEDEATH);
				}
			}

			public string StumbleMix
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_MIX);
				}
			}

			public string StumbleLeftSide
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_LEFT_SIDE);
				}
			}

			public string StumbleRightSide
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_RIGHT_SIDE);
				}
			}

			public string StumbleLeftCorner
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_LEFT_CORNER);
				}
			}

			public string StumbleRightCorner
			{
				get
				{
					return GetRandomAnimationName(STUMBLE_RIGHT_CORNER);
				}
			}

			public string WallRideLeft
			{
				get
				{
					return GetRandomAnimationName(WALLRIDELEFT);
				}
			}

			public string WallRideLeftExit
			{
				get
				{
					return GetRandomAnimationName(WALLRIDELEFTEXIT);
				}
			}

			public string WallRideRight
			{
				get
				{
					return GetRandomAnimationName(WALLRIDERIGHT);
				}
			}

			public string WallRideRightExit
			{
				get
				{
					return GetRandomAnimationName(WALLRIDERIGHTEXIT);
				}
			}

			public string WallGrindLeft
			{
				get
				{
					return GetRandomAnimationName(WALLGRINDLEFT);
				}
			}

			public string WallGrindLeftExit
			{
				get
				{
					return GetRandomAnimationName(WALLGRINDLEFTEXIT);
				}
			}

			public string WallGrindRight
			{
				get
				{
					return GetRandomAnimationName(WALLGRINDRIGHT);
				}
			}

			public string WallGrindRightExit
			{
				get
				{
					return GetRandomAnimationName(WALLGRINDRIGHTEXIT);
				}
			}

			public string RouteCelebrate
			{
				get
				{
					return GetRandomAnimationName(ROUTE_CELEBRATE);
				}
			}

			public string BigAirTrick
			{
				get
				{
					return GetRandomAnimationName(BIG_AIR_TRICK);
				}
			}

			public string RouteEnter
			{
				get
				{
					return GetRandomAnimationName(ROUTE_ENTER);
				}
			}

			public string RouteExit
			{
				get
				{
					return GetRandomAnimationName(ROUTE_EXIT);
				}
			}

			public string Grind(string lastAnimationName)
			{
				return GetRandomAnimationNameExcluding(GRIND, lastAnimationName);
			}

			private string GetRandomAnimationName(List<string> animationsNames)
			{
				int index = UnityEngine.Random.Range(0, animationsNames.Count);
				return animationsNames[index];
			}

			private string GetRandomAnimationNameExcluding(List<string> animationNames, string excludeAnimationName)
			{
				_animationNamesExcluded.Clear();
				_animationNamesExcluded.Add(excludeAnimationName);
				return GetRandomAnimationNameExcluding(animationNames, _animationNamesExcluded);
			}

			private string GetRandomAnimationNameExcluding(List<string> animationNames, IEnumerable<string> excludeAnimationNames)
			{
				if (animationNames.Count == 1)
				{
					return animationNames[0];
				}
				_animationNamesFiltered.Clear();
				foreach (string animationName in animationNames)
				{
					if (!excludeAnimationNames.Contains(animationName))
					{
						_animationNamesFiltered.Add(animationName);
					}
				}
				return GetRandomAnimationName(_animationNamesFiltered);
			}

			private List<string> GetRandomHoverJumps(List<string> hoverboardJump, List<string> hoverboardHangtime)
			{
				if (hoverboardJump.Count != hoverboardHangtime.Count)
				{
				}
				int index = UnityEngine.Random.Range(0, Mathf.Min(hoverboardJump.Count, hoverboardHangtime.Count));
				List<string> list = new List<string>();
				list.Add(hoverboardJump[index]);
				list.Add(hoverboardHangtime[index]);
				return list;
			}
		}

		public interface ICustomAnimationClipList
		{
			void Initialize(Animations animations, CharacterRendering characterRendering);
		}

		[Serializable]
		public class AnimationClipLists
		{
			public AnimationClip[] topMenu;

			public AnimationClip[] introRun;

			public AnimationClip[] run;

			public AnimationClip[] jump;

			public AnimationClip[] hangtime;

			public AnimationClip[] doubleJump;

			public AnimationClip[] doubleHangtime;

			public AnimationClip[] landing;

			public AnimationClip[] dodgeLeft;

			public AnimationClip[] dodgeRight;

			public AnimationClip[] roll;

			public AnimationClip[] hitMid;

			public AnimationClip[] hitUpper;

			public AnimationClip[] hitLower;

			public AnimationClip[] hitMoving;

			public AnimationClip[] stumble;

			public AnimationClip[] stumbleMix;

			public AnimationClip[] stumbleDeath;

			public AnimationClip[] stumbleLeftSide;

			public AnimationClip[] stumbleRightSide;

			public AnimationClip[] stumbleLeftCorner;

			public AnimationClip[] stumbleRightCorner;

			public AnimationClip[] holdMagnet;
		}

		[Serializable]
		public class JetpackClips
		{
			public AnimationClip[] run;

			public AnimationClip[] dodgeLeft;

			public AnimationClip[] dodgeRight;

			public AnimationClip[] turboHeadstart1;

			public AnimationClip[] turboHeadstart2;
		}

		[Serializable]
		public class PogostickClips
		{
			public AnimationClip[] run;

			public AnimationClip[] hangtime;

			public AnimationClip[] dodgeLeft;

			public AnimationClip[] dodgeRight;
		}

		[Serializable]
		public class SuperSneaksClips
		{
			public AnimationClip[] run;
		}

		private const int GrindIndexCornerGrindLeft = 0;

		private const int GrindIndexOnRail = 1;

		private const int GrindIndexCornerGrindRight = 2;

		private const float FaceLengthJumpAnimation = 0.15f;

		private const float FadeLengthJumpAnimationAfterTuck = 1f;

		private const float CameraShakeAmountForTurboHeadstart = 20f;

		[SerializeField]
		private AnimationClipLists defaultAnimations;

		[SerializeField]
		private JetpackClips jetpackAnimations;

		[SerializeField]
		private SuperSneaksClips SuperSneaksAnimations;

		[SerializeField]
		private PogostickClips pogostickClips;

		[SerializeField]
		private GameObject teleboardWarpPrefab;

		[SerializeField]
		private Material teleboardFXMaterial;

		[SerializeField]
		private AnimationCurve jetpackParticleOffsetCurve;

		[SerializeField]
		private GameObject characterModelPrefab;

		[SerializeField]
		private GameObject characterRenderingEffectsPrefab;

		public Animations animations = new Animations();

		private CharacterModel _characterModel;

		private static CharacterRendering _instance;

		private Game _game;

		private Character _character;

		private Hoverboard _hoverboard;

		private SuperSneakers _superSneakers;

		private FollowingGuard _followingGuard;

		private CharacterController _characterController;

		private MeshRenderer _shadowMeshRenderer;

		private CharacterRenderingEffects _characterRenderingEffects;

		private Revive _revive;

		private SafeAnimation _characterSafeAnimation;

		private SafeAnimation _hoverboardSafeAnimation;

		private bool _initialized;

		private HoverboardRendering _defaultHoverboardRendering;

		private HoverboardRendering.AbilityAnimationList _defaultHoverboardAnimationList;

		private GameObject _teleboardWarpGameObject;

		private Renderer _telewarpRenderer;

		private AnimationState _teleboardAnimation;

		private Vector3 _teleboardWarpPosition;

		private CharacterCamera _characterCamera;

		private HoverboardRendering _hoverboardRendering;

		private List<AnimationClip> _addedAnimClipsNames = new List<AnimationClip>();

		private HoverboardBundle _cachedHoverboardBundle;

		private Hoverboard.BoardAbility[] _currentBoardAbilities;

		private Vector3 _initRot = Vector3.zero;

		private Vector3 _initScale = Vector3.one;

		private ParticleSystem[] _killParticleSystem;

		private AnimationState _caughtAnimationState;

		private Material _cachedModelMaterial;

		private Material[] _cachedHeadMaterials;

		private Material _cachedCustomizationMaterial;

		private Material _cachedHoverboardMaterial;

		private Material _cachedSupersneakersMaterial;

		private Material _cachedDoubleJumperMaterial;

		private Material[] _cachedCustomHoverboardModelMaterials;

		private Transform _extraTeleportFX;

		private bool _isTeleportFxActive;

		private bool _isCachedCustomizationState = true;

		private bool _isDoublejump;

		private bool _isRolling;

		private bool _isDeath;

		private static float _DEFAULT_SPEED_RUN_ANIMATION = 1f;

		private RouteManager _routeManager;

		private string _lastGrindAnimation = string.Empty;

		public Action OnExtraTeleboardEffectEnd;

		public Action OnResetCharacterTeleportFX;

		private string jumpAnimation;

		private string hangtimeAnimation;

		private bool _queuedHangtime;

		public CharacterModel CharacterModel
		{
			get
			{
				return _characterModel;
			}
		}

		public Animation CharacterAnimation
		{
			get
			{
				return _characterSafeAnimation.Animation;
			}
		}

		public static CharacterRendering Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Utils.FindObject<CharacterRendering>();
				}
				return _instance;
			}
		}

		public SafeAnimation CharacterSafeAnimation
		{
			get
			{
				return _characterSafeAnimation;
			}
		}

		public SafeAnimation HoverboardSafeAnimation
		{
			get
			{
				return _hoverboardSafeAnimation;
			}
		}

		public void Initialize()
		{
			_instance = this;
			_game = Game.Instance;
			_character = Character.Instance;
			_characterController = Game.Charactercontroller;
			_hoverboard = Hoverboard.Instance;
			_superSneakers = this.FindObject<SuperSneakers>();
			_followingGuard = FollowingGuard.Instance;
			_revive = Revive.Instance;
			_characterCamera = CharacterCamera.Instance;
			_routeManager = SingletonMonoBehaviour<RouteManager>.Instance;
			_routeManager.OnRouteCreated += RouteManager_OnRouteCreated;
			InitializeCharacterModel();
			InitializeAnimations();
			InitializeCharacterRenderingEffects();
			Variable<bool> isInGame = _game.IsInGame;
			isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, new Variable<bool>.OnChangeDelegate(IsInGame_OnChange));
			_character.OnChangeTrack += OnChangeTrack;
			_character.OnStumble += OnStumble;
			_character.OnRevive += OnRevive;
			_character.OnResetTeleportFX += ResetCharacterTeleportFX;
			_character.OnTutorialMoveBackToCheckPoint += OnTutorialMoveBackToCheckPoint;
			_character.OnTutorialStartFromCheckPoint += OnTutorialStartFromCheckPoint;
			_character.OnHitByTrain += OnHitByTrain;
			_character.OnJump += OnJump;
			_character.OnDoubleJump += OnDoubleJump;
			_character.OnJumpIfHitByTrain += OnJump;
			_character.OnRoll += OnRoll;
			_character.OnLanding += OnLanding;
			_character.OnHangtime += Character_OnHangtime;
			Variable<bool> isGrounded = _character.IsGrounded;
			isGrounded.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isGrounded.OnChange, new Variable<bool>.OnChangeDelegate(OnChangeIsGrounded));
			_character.OnTeleboardStart += HandleOnTeleboardStart;
			Character character = _character;
			character.OnRouteDone = (Action<bool>)Delegate.Combine(character.OnRouteDone, new Action<bool>(OnRouteDone));
			Character character2 = _character;
			character2.OnEnterRoute = (Action)Delegate.Combine(character2.OnEnterRoute, new Action(Character_OnEnterRoute));
			Character character3 = _character;
			character3.OnEnterRouteEnd = (Action)Delegate.Combine(character3.OnEnterRouteEnd, new Action(Character_OnEnterRouteEnd));
			Character character4 = _character;
			character4.OnExitRoute = (Action)Delegate.Combine(character4.OnExitRoute, new Action(Character_OnExitRoute));
			Character character5 = _character;
			character5.OnToggleDust = (Action<bool>)Delegate.Combine(character5.OnToggleDust, new Action<bool>(ToggleDust));
			_game.OnStageMenuSequence += OnStageMenuSequence;
			_game.OnIntroRun += OnIntroRun;
			_game.OnTurboHeadstartInput += HandleOnTurboHeadstart;
			_game.OnEndStumbleDeathSequence += Game_OnEndStumbleDeathSequence;
			_hoverboard.OnSwitchToHoverboard += OnSwitchToHoverboard;
			_hoverboard.OnEndHoverboard += OnSwitchToRunning;
			_hoverboard.OnJump += OnJump;
			_hoverboard.OnRun += OnRun;
			JetpackState state = GameStateMachine.Instance.GetState<JetpackState>(4);
			state.OnStart = (Action)Delegate.Combine(state.OnStart, new Action(OnSwitchToJetpack));
			state.OnStop = (Action)Delegate.Combine(state.OnStop, new Action(JetpackOnStop));
			state.OnFlyAheadStart = (Action)Delegate.Combine(state.OnFlyAheadStart, new Action(JetpackOnFlyAheadStart));
			state.OnFlyAheadUpdate = (Action<float>)Delegate.Combine(state.OnFlyAheadUpdate, new Action<float>(JetpackOnFlyAheadUpdate));
			HeadstartState state2 = GameStateMachine.Instance.GetState<HeadstartState>(3);
			state2.OnStart = (Action)Delegate.Combine(state2.OnStart, new Action(OnSwitchToJetpack));
			state2.OnStop = (Action)Delegate.Combine(state2.OnStop, new Action(JetpackOnStop));
			state2.OnFlyAheadStart = (Action)Delegate.Combine(state2.OnFlyAheadStart, new Action(JetpackOnFlyAheadStart));
			state2.OnFlyAheadUpdate = (Action<float>)Delegate.Combine(state2.OnFlyAheadUpdate, new Action<float>(JetpackOnFlyAheadUpdate));
			SlideinPowerupHelper.OnScoreBoostActivated += HandleOnScorebooster;
			_superSneakers.OnSwitchToSuperSneakers += OnSwitchToSuperSneakers;
			_superSneakers.SuperSneakerOnStop += SuperSneakersOnStop;
			PogostickState state3 = GameStateMachine.Instance.GetState<PogostickState>(5);
			state3.OnStart = (Action)Delegate.Combine(state3.OnStart, new Action(PogostickOnStart));
			state3.OnHangtime = (Action)Delegate.Combine(state3.OnHangtime, new Action(PogostickOnHangtime));
			state3.OnStop = (Action)Delegate.Combine(state3.OnStop, new Action(PogostickOnStop));
			FollowingGuard followingGuard = _followingGuard;
			followingGuard.OnCatchPlayer = (FollowingGuard.OnCatchPlayerDelegate)Delegate.Combine(followingGuard.OnCatchPlayer, new FollowingGuard.OnCatchPlayerDelegate(OnCatchPlayer));
			_revive.OnRevive += OnRevive;
			_revive.OnSwitchToRunning += OnSwitchToRunning;
			HoverboardBundle hoverboardBundle = SingletonMonoBehaviour<HoverboardModelFactory>.Instance.RequestHoverboardInstance("normal");
			hoverboardBundle.AddModel(false);
			_defaultHoverboardRendering = hoverboardBundle.GetComponentInChildren<HoverboardRendering>();
			_defaultHoverboardAnimationList = new HoverboardRendering.AbilityAnimationList();
			_defaultHoverboardAnimationList.defaultHoverboardAnimation = _defaultHoverboardRendering.defaultHoverboardAnimation;
			_defaultHoverboardAnimationList.runAnimations = _defaultHoverboardRendering.runAnimations;
			_defaultHoverboardAnimationList.landAnimations = _defaultHoverboardRendering.landAnimations;
			_defaultHoverboardAnimationList.jumpAnimations = _defaultHoverboardRendering.jumpAnimations;
			_defaultHoverboardAnimationList.hangtimeAnimations = _defaultHoverboardRendering.hangtimeAnimations;
			_defaultHoverboardAnimationList.rollAnimations = _defaultHoverboardRendering.rollAnimations;
			_defaultHoverboardAnimationList.dodgeLeftAnimations = _defaultHoverboardRendering.dodgeLeftAnimations;
			_defaultHoverboardAnimationList.dodgeRightAnimations = _defaultHoverboardRendering.dodgeRightAnimations;
			_defaultHoverboardAnimationList.grindAnimations = _defaultHoverboardRendering.grindAnimations;
			_defaultHoverboardAnimationList.grindLandAnimations = _defaultHoverboardRendering.grindLandAnimations;
			_defaultHoverboardAnimationList.getOnBoardAnimations = _defaultHoverboardRendering.getOnBoardAnimations;
			UnityEngine.Object.Destroy(hoverboardBundle.gameObject);
		}

		protected void Awake()
		{
			_instance = this;
		}

		protected void OnDestroy()
		{
			if (_routeManager != null)
			{
				_routeManager.OnRouteCreated -= RouteManager_OnRouteCreated;
			}
		}

		public CharacterModel RecreateCharacterModel()
		{
			InitializeCharacterModel();
			InitializeAnimations();
			_characterModel.RecreateCharacter();
			_characterRenderingEffects.RecreateModel(_characterModel);
			OnStageMenuSequence();
			return _characterModel;
		}

		private void DebugLog(string message)
		{
			if (0 == 0)
			{
			}
		}

		private void InitializeCharacterModel()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPrefab);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			_killParticleSystem = gameObject.GetComponentsInChildren<ParticleSystem>();
			_characterModel = gameObject.GetComponent<CharacterModel>();
			_shadowMeshRenderer = _characterModel.MeshBlobShadow;
			_characterSafeAnimation = new SafeAnimation(_characterModel.CharacterAnimation, false);
			_hoverboardSafeAnimation = new SafeAnimation(null, false);
		}

		private void InitializeCharacterRenderingEffects()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(characterRenderingEffectsPrefab);
			HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.Dynamic);
			_characterRenderingEffects = gameObject.GetComponent<CharacterRenderingEffects>();
			_characterRenderingEffects.Initialize(_characterModel);
			_teleboardWarpGameObject = UnityEngine.Object.Instantiate(teleboardWarpPrefab);
			_teleboardWarpGameObject.transform.parent = _character.transform;
			_teleboardAnimation = _teleboardWarpGameObject.GetComponent<Animation>()["teleboardWarp"];
			_teleboardAnimation.normalizedTime = 0f;
			_teleboardAnimation.weight = 1f;
			_teleboardAnimation.enabled = true;
			_characterSafeAnimation.SetSpeed(_teleboardAnimation, 0f);
			_teleboardWarpGameObject.SetActive(false);
		}

		private void InitializeAnimations()
		{
			_addedAnimClipsNames.Clear();
			animations.RUN = InitializeClips(SuperSneaksAnimations.run);
			animations.RUN = InitializeClips(defaultAnimations.run);
			animations.INTRO_RUN = InitializeClips(defaultAnimations.introRun);
			animations.LAND = InitializeClips(defaultAnimations.landing);
			animations.BIG_LAND = InitializeClips(defaultAnimations.landing);
			animations.SMALL_LAND = InitializeClips(defaultAnimations.landing);
			animations.JUMP = InitializeClips(defaultAnimations.jump);
			animations.SMALL_JUMP = InitializeClips(defaultAnimations.jump);
			animations.HANGTIME = InitializeClips(defaultAnimations.hangtime);
			animations.DOUBLEJUMP = InitializeClips(defaultAnimations.doubleJump);
			animations.DOUBLEHANGTIME = InitializeClips(defaultAnimations.doubleHangtime);
			animations.ROLL = InitializeClips(defaultAnimations.roll);
			animations.DODGE_LEFT = InitializeClips(defaultAnimations.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(defaultAnimations.dodgeRight);
			animations.TOP_MENU = InitializeClips(defaultAnimations.topMenu);
			animations.HIT_MID = InitializeClips(defaultAnimations.hitMid);
			animations.HIT_UPPER = InitializeClips(defaultAnimations.hitUpper);
			animations.HIT_LOWER = InitializeClips(defaultAnimations.hitLower);
			animations.HIT_MOVING = InitializeClips(defaultAnimations.hitMoving);
			animations.STUMBLE = InitializeClips(defaultAnimations.stumble);
			animations.STUMBLEDEATH = InitializeClips(defaultAnimations.stumbleDeath);
			animations.STUMBLE_MIX = InitializeClips(defaultAnimations.stumbleMix);
			animations.STUMBLE_LEFT_SIDE = InitializeClips(defaultAnimations.stumbleLeftSide);
			animations.STUMBLE_RIGHT_SIDE = InitializeClips(defaultAnimations.stumbleRightSide);
			animations.STUMBLE_LEFT_CORNER = InitializeClips(defaultAnimations.stumbleLeftCorner);
			animations.STUMBLE_RIGHT_CORNER = InitializeClips(defaultAnimations.stumbleRightCorner);
			animations.HOLDMAGNET = InitializeClips(defaultAnimations.holdMagnet);
			_characterSafeAnimation.GetAnimationState("stumble").AddMixingTransform(_characterModel.SpineTransform);
			_characterSafeAnimation.GetAnimationState("stumble").layer = 2;
			_characterSafeAnimation.GetAnimationState("stumble").weight = 1f;
			_characterSafeAnimation.GetAnimationState("stumbleCornerLeft").AddMixingTransform(_characterModel.SpineTransform);
			_characterSafeAnimation.GetAnimationState("stumbleCornerLeft").layer = 2;
			_characterSafeAnimation.GetAnimationState("stumbleCornerLeft").weight = 1f;
			_characterSafeAnimation.GetAnimationState("stumbleCornerRight").AddMixingTransform(_characterModel.SpineTransform);
			_characterSafeAnimation.GetAnimationState("stumbleCornerRight").layer = 2;
			_characterSafeAnimation.GetAnimationState("stumbleCornerRight").weight = 1f;
		}

		public List<string> InitializeClips(AnimationClip[] clips, IEnumerable<string> excludedAnimationNames = null)
		{
			List<string> list = new List<string>();
			foreach (AnimationClip animationClip in clips)
			{
				if (excludedAnimationNames == null || !excludedAnimationNames.Contains(animationClip.name))
				{
					if (!_addedAnimClipsNames.Contains(animationClip))
					{
						_addedAnimClipsNames.Add(animationClip);
						_characterSafeAnimation.AddClip(animationClip, animationClip.name);
					}
					list.Add(animationClip.name);
				}
			}
			return list;
		}

		private string GetHoverboardAnimationName(string animationName)
		{
			AnimationClip animationClip = _hoverboardSafeAnimation.GetAnimationClip(animationName);
			if (animationClip != null)
			{
				return animationName;
			}
			return animations.DEFAULT_HOVERBOARD_ANIMATION;
		}

		private bool HasHoverboardAnimation(string animationToCheck)
		{
			return animationToCheck != null && _hoverboardSafeAnimation.Animation.GetClip(animationToCheck) != null;
		}

		private void OnIntroRun()
		{
			ParticleSystem[] killParticleSystem = _killParticleSystem;
			foreach (ParticleSystem particleSystem in killParticleSystem)
			{
				particleSystem.Stop();
			}
			AnimationEvent animationEvent = new AnimationEvent();
			animationEvent.functionName = "SetAnimationSpeedEvent";
			animationEvent.time = 0.1f;
			animationEvent.messageOptions = SendMessageOptions.RequireReceiver;
			_characterSafeAnimation.TryAddAnimationEvent(CharacterSafeAnimation.Animation, "run", animationEvent);
			_characterSafeAnimation.TryAddAnimationEvent(CharacterSafeAnimation.Animation, "run2", animationEvent);
			_characterSafeAnimation.TryAddAnimationEvent(CharacterSafeAnimation.Animation, "run3", animationEvent);
			_characterSafeAnimation.TryAddAnimationEvent(CharacterSafeAnimation.Animation, "run4_long", animationEvent);
			OnSwitchToRunning();
			string run = animations.Run;
			_characterSafeAnimation.CrossFadeQueued(run, 0.2f);
			_characterModel.SprayCanModel.SetActive(false);
			_characterModel.StopEyeAnimations();
			_isDeath = false;
		}

		public void OnRun()
		{
			if (_character.IsFalling || _character.IsJumping || _character.IsRolling || _isDeath)
			{
				return;
			}
			string run = animations.Run;
			_characterSafeAnimation.SetSpeed(run, _DEFAULT_SPEED_RUN_ANIMATION);
			if (_characterController.isGrounded)
			{
				if (_superSneakers.isActive)
				{
					_characterSafeAnimation.SetSpeed(run, _DEFAULT_SPEED_RUN_ANIMATION);
				}
				_characterSafeAnimation.CrossFade(run);
				if (_hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(run));
				}
			}
		}

		private void OnChangeTrack(Character.OnChangeTrackDirection direction)
		{
			if ((_character.IsGrounded.Value || _game.IsInJetpackMode) && !_game.IsInPogostickMode && !_isDeath)
			{
				string text = ((direction != Character.OnChangeTrackDirection.Left) ? animations.DodgeRight : animations.DodgeLeft);
				if (!_hoverboard.isActive && !_game.IsInJetpackMode)
				{
					_characterSafeAnimation.SetSpeed(text, _game.CurrentSpeedNormalized);
				}
				else
				{
					_characterSafeAnimation.SetSpeed(text, _DEFAULT_SPEED_RUN_ANIMATION);
				}
				if (!_characterSafeAnimation.IsPlaying(jumpAnimation))
				{
					_characterSafeAnimation.CrossFade(text, 0.2f);
				}
				if (_hoverboard.isActive && _hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(text), 0.2f);
				}
			}
			if (!_character.IsJumping)
			{
				string run = animations.Run;
				if (!_hoverboard.isActive && !_game.IsInJetpackMode)
				{
					_characterSafeAnimation.SetSpeed(run, _game.CurrentSpeedNormalized);
				}
				else
				{
					_characterSafeAnimation.SetSpeed(run, _DEFAULT_SPEED_RUN_ANIMATION);
				}
				_characterSafeAnimation.CrossFadeQueued(run, (!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
				if (_hoverboard.isActive && _hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), 0.02f);
				}
			}
		}

		private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName, bool safeStumble)
		{
			switch (stumbleType)
			{
			case Character.StumbleType.Bush:
				_characterSafeAnimation.CrossFade(animations.StumbleMix, 0.05f);
				_characterSafeAnimation.CrossFadeQueued(animations.Run, 0.5f);
				return;
			case Character.StumbleType.Side:
				if (!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard) && !_game.IsInJetpackMode && !_game.IsInPogostickMode)
				{
					if (horizontalHit == Character.StumbleHorizontalHit.LeftCorner || horizontalHit == Character.StumbleHorizontalHit.Left)
					{
						_characterSafeAnimation.CrossFade(animations.StumbleLeftSide, 0.2f);
					}
					if (horizontalHit == Character.StumbleHorizontalHit.RightCorner || horizontalHit == Character.StumbleHorizontalHit.Right)
					{
						_characterSafeAnimation.CrossFade(animations.StumbleRightSide, 0.2f);
					}
				}
				if (!_character.IsJumping)
				{
					_characterSafeAnimation.CrossFadeQueued(animations.Run, (!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
				}
				return;
			}
			switch (horizontalHit)
			{
			case Character.StumbleHorizontalHit.Center:
				switch (verticalHit)
				{
				case Character.StumbleVerticalHit.Lower:
					if (_game.Modifiers.IsActive(_game.Modifiers.Hoverboard))
					{
						_characterSafeAnimation.CrossFade(animations.StumbleMix, 0.05f);
					}
					else
					{
						_characterSafeAnimation.CrossFade(animations.Stumble, 0.05f);
					}
					_characterSafeAnimation.CrossFadeQueued(animations.Run, 0.5f);
					break;
				case Character.StumbleVerticalHit.Middle:
					_characterSafeAnimation.CrossFade(animations.HitMid, 0.07f);
					if (safeStumble)
					{
						_characterSafeAnimation.CrossFadeQueued(animations.Run, 0.3f);
					}
					break;
				case Character.StumbleVerticalHit.Upper:
					_characterSafeAnimation.CrossFade(animations.HitUpper, 0.07f);
					break;
				}
				return;
			case Character.StumbleHorizontalHit.Left:
				_characterSafeAnimation.Play((!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? animations.StumbleLeftSide : animations.StumbleLeftCorner);
				break;
			case Character.StumbleHorizontalHit.LeftCorner:
				_characterSafeAnimation.Play(animations.StumbleLeftCorner);
				break;
			case Character.StumbleHorizontalHit.Right:
				_characterSafeAnimation.Play((!_game.Modifiers.IsActive(_game.Modifiers.Hoverboard)) ? animations.StumbleRightSide : animations.StumbleRightCorner);
				break;
			case Character.StumbleHorizontalHit.RightCorner:
				_characterSafeAnimation.Play(animations.StumbleRightCorner);
				break;
			}
			_characterSafeAnimation.PlayQueued(animations.Run);
		}

		private void Game_OnEndStumbleDeathSequence()
		{
			_isDeath = true;
			string stumbleDeath = animations.StumbleDeath;
			_characterSafeAnimation.CrossFade(stumbleDeath, 0.2f);
		}

		private void OnChangeIsGrounded(bool isGrounded)
		{
			if (_shadowMeshRenderer != null)
			{
				_shadowMeshRenderer.enabled = isGrounded;
			}
		}

		private void OnRoll()
		{
			StartCoroutine(OnRollPlayAnimation());
		}

		private IEnumerator OnRollPlayAnimation()
		{
			_isRolling = true;
			string rollAnimation = animations.Roll;
			string runAnimation = animations.Run;
			if (_superSneakers.isActive)
			{
				_characterSafeAnimation.SetSpeed(runAnimation, _DEFAULT_SPEED_RUN_ANIMATION);
			}
			else
			{
				_characterSafeAnimation.SetSpeed(runAnimation, _game.CurrentSpeedNormalized);
			}
			_characterSafeAnimation.CrossFade(rollAnimation, 0.1f);
			if (_hoverboard.isActive)
			{
				_characterSafeAnimation.CrossFadeQueued(runAnimation, 0.2f);
				if (_hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(rollAnimation), 0.1f);
					_hoverboardSafeAnimation.CrossFadeQueued(GetHoverboardAnimationName(runAnimation), (!_hoverboard.isActive) ? 0f : 0.2f);
				}
			}
			else
			{
				_characterSafeAnimation.CrossFadeQueued(runAnimation, 0.1f);
			}
			float endTime = Time.time + _characterSafeAnimation.GetAnimationState(rollAnimation).length;
			while (Time.time < endTime && _characterSafeAnimation.GetAnimationState(rollAnimation).enabled && _isRolling)
			{
				yield return null;
			}
			EndRollAnimation();
		}

		private void ToggleDust(bool active)
		{
			_characterRenderingEffects.ToggleDust(active);
		}

		private void EndRollAnimation()
		{
			_isRolling = false;
			_character.EndRoll();
		}

		private void OverrideRollTransition()
		{
			string run = animations.Run;
			if (_hoverboard.isActive)
			{
				_characterSafeAnimation.CrossFadeQueued(run, 0.2f);
				if (_hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), (!_hoverboard.isActive) ? 0f : 0.2f);
				}
			}
			else
			{
				_characterSafeAnimation.CrossFadeQueued(run, 0.1f);
			}
		}

		private void OnHitByTrain()
		{
			_characterSafeAnimation.Play(animations.HitMoving);
			Vector3 currentPos = _character.GamePosition;
			Vector3 camPos = GamePositionRoot.Instance.GetGamePosition(_character.CharacterCamera.transform);
			StartCoroutine(pTween.To(0.5f, (float t) =>
			{
				_character.SetGamePosition(Vector3.Lerp(currentPos, new Vector3(camPos.x, camPos.y - 33f, currentPos.z), t));
			}));
		}

		private void OnJump()
		{
			OnJump(_character.jumpHeightNormal, false);
		}

		private void OnJump(float jumpHeight, bool willPlaySound)
		{
			if (_isRolling)
			{
				EndRollAnimation();
			}
			if (_hoverboard.isActive)
			{
				List<string> hoverboardJump = animations.HoverboardJump;
				jumpAnimation = hoverboardJump[0];
				hangtimeAnimation = hoverboardJump[1];
			}
			else if (jumpHeight == 0f || jumpHeight > _character.jumpHeightNormal * 0.5f)
			{
				jumpAnimation = animations.Jump;
			}
			else
			{
				jumpAnimation = animations.SmallJump;
			}
			if (_character.IsTucked)
			{
				_characterSafeAnimation.CrossFade(jumpAnimation, 1f);
			}
			else
			{
				_characterSafeAnimation.CrossFade(jumpAnimation, 0.15f);
			}
			if (_hoverboard.isActive && _hoverboardSafeAnimation.HasAnimation())
			{
				_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(jumpAnimation), 0.15f);
			}
		}

		private void OnJump(int intensity)
		{
			jumpAnimation = animations.Jump;
			if (_character.IsTucked)
			{
				_characterSafeAnimation.CrossFade(jumpAnimation, 1f);
			}
			else
			{
				_characterSafeAnimation.CrossFade(jumpAnimation, 0.15f);
			}
		}

		private void OnDoubleJump()
		{
			_isDoublejump = true;
			_characterRenderingEffects.PlayDoubleJumpThruster();
			if (_hoverboard.isActive)
			{
				List<string> hoverboardJump = animations.HoverboardJump;
				jumpAnimation = hoverboardJump[0];
				hangtimeAnimation = hoverboardJump[1];
			}
			else
			{
				jumpAnimation = animations.DoubleJump;
			}
			_characterSafeAnimation.Play(jumpAnimation);
			if (_hoverboard.isActive && _hoverboardSafeAnimation.HasAnimation())
			{
				_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(jumpAnimation), 0.15f);
			}
		}

		private void OnRevive()
		{
			StopAllCoroutines();
			_isDeath = false;
			jumpAnimation = animations.Jump;
			_characterSafeAnimation.Stop();
			_characterSafeAnimation.Play(jumpAnimation);
			if (CoinMagnet.Instance.IsActive)
			{
				_characterSafeAnimation.SetEnabled("hold_magnet", true);
				_characterSafeAnimation.Play("hold_magnet");
			}
		}

		public void Character_OnHangtime()
		{
			if (_character.IsRolling)
			{
				return;
			}
			if (_isDoublejump)
			{
				OnDoubleHangtime();
				return;
			}
			if (!_hoverboard.isActive || hangtimeAnimation == null)
			{
				hangtimeAnimation = animations.Hangtime;
			}
			if (!_queuedHangtime || !_characterSafeAnimation.IsPlaying(jumpAnimation))
			{
				_characterSafeAnimation.CrossFade(hangtimeAnimation, 0.3f);
			}
			else
			{
				_characterSafeAnimation.CrossFadeQueued(hangtimeAnimation);
			}
			if (_hoverboardSafeAnimation != null && _hoverboardSafeAnimation.HasAnimation())
			{
				if (!_queuedHangtime)
				{
					_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(hangtimeAnimation), 0.3f);
				}
				else
				{
					_hoverboardSafeAnimation.CrossFadeQueued(GetHoverboardAnimationName(hangtimeAnimation));
				}
			}
		}

		private void Character_OnHangtime_AfterAlternateRoute()
		{
			_queuedHangtime = false;
			_character.OnHangtime -= Character_OnHangtime_AfterAlternateRoute;
		}

		private void OnDoubleHangtime()
		{
			_isDoublejump = false;
			if (!_hoverboard.isActive || hangtimeAnimation == null)
			{
				hangtimeAnimation = animations.DoubleHangtime;
			}
			_characterSafeAnimation.CrossFade(hangtimeAnimation, 0.3f);
			if (_hoverboardSafeAnimation.HasAnimation())
			{
				_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(hangtimeAnimation), 0.3f);
			}
		}

		private void OnLanding(Character character)
		{
			character.UpdateGroundTag(_character);
			string text = animations.Run;
			if (_superSneakers.isActive)
			{
				_characterSafeAnimation.SetSpeed(text, _DEFAULT_SPEED_RUN_ANIMATION);
			}
			else
			{
				_characterSafeAnimation.SetSpeed(text, _game.CurrentSpeedNormalized);
			}
			if (_character.IsRolling)
			{
				return;
			}
			if (_hoverboard.isActive)
			{
				string text2;
				if (_character.CurrentRunPosition == Character.RunPositions.train || _character.CurrentRunPosition == Character.RunPositions.movingTrain)
				{
					text = animations.Grind(string.Empty);
					text2 = text + "_land";
				}
				else
				{
					text2 = animations.Land;
				}
				if (_characterSafeAnimation.HasAnimationClip(text2))
				{
					_characterSafeAnimation.CrossFade(text2, 0.1f);
				}
				_characterSafeAnimation.CrossFadeQueued(text, 0.2f);
				if (_hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(text2), 0.1f);
					_hoverboardSafeAnimation.CrossFadeQueued(GetHoverboardAnimationName(text), 0.2f);
				}
			}
			else if (_character.verticalSpeed < -20f)
			{
				string animationName = ((_character.verticalSpeed < -150f) ? animations.BigLand : ((!(_character.verticalSpeed < -40f)) ? animations.SmallLand : animations.Land));
				_characterSafeAnimation.CrossFade(animationName, 0.05f);
				_characterSafeAnimation.CrossFadeQueued(text, 0.1f);
			}
			else
			{
				_characterSafeAnimation.CrossFade(text, 0.1f);
			}
		}

		private void StartGrind(int side)
		{
			string text = string.Empty;
			switch (side)
			{
			case 0:
				text = animations.WallGrindLeft;
				break;
			case 1:
				text = animations.Grind(_lastGrindAnimation);
				break;
			case 2:
				text = animations.WallGrindRight;
				break;
			}
			if (_characterSafeAnimation.HasAnimationClip(text))
			{
				if (_character.verticalSpeed > -10f)
				{
					_characterSafeAnimation.CrossFade(animations.StartGrind, 0.1f);
					_characterSafeAnimation.CrossFadeQueued(text, 0.1f);
				}
				else
				{
					_characterSafeAnimation.CrossFade(text, 0.1f);
				}
				_lastGrindAnimation = text;
			}
		}

		public void EndGrind()
		{
			_lastGrindAnimation = string.Empty;
		}

		private void OnGrindExit(int side)
		{
			string animationName = string.Empty;
			switch (side)
			{
			case 0:
				animationName = animations.WallGrindLeftExit;
				break;
			case 2:
				animationName = animations.WallGrindRightExit;
				break;
			}
			_characterSafeAnimation.CrossFade(animationName, 0.1f);
		}

		private void StartWallRide(int side)
		{
			string animationName = string.Empty;
			switch (side)
			{
			case 0:
				animationName = animations.WallRideLeft;
				break;
			case 1:
				animationName = animations.WallRideRight;
				break;
			}
			_characterSafeAnimation.CrossFade(animationName, 0.2f);
		}

		private void StopWallRide(int side)
		{
			string animationName = string.Empty;
			switch (side)
			{
			case 0:
				animationName = animations.WallRideLeftExit;
				break;
			case 1:
				animationName = animations.WallRideRightExit;
				break;
			}
			_characterSafeAnimation.CrossFade(animationName, 0.1f);
		}

		private void OnTutorialMoveBackToCheckPoint(float duration)
		{
			_characterSafeAnimation.CrossFade(animations.Run, duration);
		}

		private void OnTutorialStartFromCheckPoint()
		{
			_characterSafeAnimation.Play(animations.Run);
		}

		private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTimeBeforeScreen)
		{
			_caughtAnimationState = _characterSafeAnimation.GetAnimationState(currentCharacterCaught);
			StartCoroutine(CatchPlayerAnimStarter(_caughtAnimationState, catchUpTime));
		}

		private IEnumerator CatchPlayerAnimStarter(AnimationState caught, float delay)
		{
			caught.weight = 0f;
			caught.normalizedTime = 0f;
			caught.enabled = true;
			yield return new WaitForSeconds(delay);
			StartCoroutine(pTween.To(0.2f, (float t) =>
			{
				caught.weight = Mathf.Lerp(0f, 1f, t);
			}));
		}

		private void OnStageMenuSequence()
		{
			StopAllCoroutines();
			ResetTeleportFx();
			_isCachedCustomizationState = _characterModel.OutfitModel.enabled;
			_characterModel.transform.localRotation = Quaternion.identity;
			if (_characterSafeAnimation.Animation != null)
			{
				if (_caughtAnimationState != null)
				{
					_caughtAnimationState.enabled = false;
				}
				_characterSafeAnimation.SetLocalRotation(Quaternion.identity);
				_characterSafeAnimation.Play(animations.TopMenu);
			}
			_characterModel.SprayCanModel.SetActive(true);
			_characterModel.StartEyeAnimations(UIModelController.ModelScreen.Character);
			ResetJetpackTrusters();
		}

		internal void OnSwitchToRunningAfterAlternateRoute()
		{
			OnSwitchToRunning();
			if (!_hoverboard.isActive)
			{
				if (!_isRolling)
				{
					OnRun();
				}
				else
				{
					OverrideRollTransition();
				}
			}
		}

		public void OnSwitchToRunning()
		{
			if (_character.SuperSneakers.isActive || _game.HasSuperSneakers)
			{
				animations.RUN = InitializeClips(SuperSneaksAnimations.run);
			}
			else
			{
				animations.RUN = InitializeClips(defaultAnimations.run);
			}
			animations.LAND = InitializeClips(defaultAnimations.landing);
			animations.BIG_LAND = InitializeClips(defaultAnimations.landing);
			animations.SMALL_LAND = InitializeClips(defaultAnimations.landing);
			animations.SMALL_JUMP = InitializeClips(defaultAnimations.jump);
			animations.HIT_MID = InitializeClips(defaultAnimations.hitMid);
			animations.JUMP = InitializeClips(defaultAnimations.jump);
			animations.HANGTIME = InitializeClips(defaultAnimations.hangtime);
			animations.DOUBLEJUMP = InitializeClips(defaultAnimations.doubleJump);
			animations.DOUBLEHANGTIME = InitializeClips(defaultAnimations.doubleHangtime);
			animations.ROLL = InitializeClips(defaultAnimations.roll);
			animations.DODGE_LEFT = InitializeClips(defaultAnimations.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(defaultAnimations.dodgeRight);
			if (_cachedHoverboardBundle != null)
			{
				ToggleCustomHoverboard(null, null);
			}
		}

		private void OnSwitchToHoverboard(HoverboardBundle hoverBoardBundle, Hoverboard.BoardAbility[] abilities)
		{
			ToggleCustomHoverboard(hoverBoardBundle, abilities);
			string getOnBoard = animations.GetOnBoard;
			hangtimeAnimation = animations.Hangtime;
			_characterSafeAnimation.CrossFade(getOnBoard, 0.1f);
			if (_hoverboardSafeAnimation.HasAnimation())
			{
				_hoverboardSafeAnimation.CrossFade(GetHoverboardAnimationName(getOnBoard), 0.1f);
			}
			if (!_character.IsFalling && !_character.IsJumping)
			{
				string run = animations.Run;
				_characterSafeAnimation.CrossFadeQueued(run, 0.2f);
				if (_hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFadeQueued(GetHoverboardAnimationName(run), 0.2f);
				}
			}
			else
			{
				_characterSafeAnimation.CrossFade(hangtimeAnimation, 0.2f);
				if (_hoverboardSafeAnimation.HasAnimation())
				{
					_hoverboardSafeAnimation.CrossFadeQueued(GetHoverboardAnimationName(hangtimeAnimation), 0.2f);
				}
			}
		}

		private void ToggleCustomHoverboard(HoverboardBundle newHoverboardBundle, Hoverboard.BoardAbility[] abilities)
		{
			if (_cachedHoverboardBundle != null && _cachedHoverboardBundle != newHoverboardBundle)
			{
				UnityEngine.Object.Destroy(_cachedHoverboardBundle);
			}
			if (_extraTeleportFX != null)
			{
				ATeleboardEffect component = _extraTeleportFX.GetComponent<ATeleboardEffect>();
				component.OnTeleboardEffectEnd = (Action)Delegate.Remove(component.OnTeleboardEffectEnd, new Action(HandleOnExtraTeleboardEffectEnd));
				UnityEngine.Object.Destroy(_extraTeleportFX.gameObject);
			}
			_cachedHoverboardBundle = newHoverboardBundle;
			_currentBoardAbilities = abilities;
			_cachedCustomHoverboardModelMaterials = new Material[_characterModel.CurrentHoverboardModelsRenderes.Count];
			if (newHoverboardBundle != null)
			{
				_teleboardWarpGameObject.SetActive(true);
				if (_telewarpRenderer == null)
				{
					_telewarpRenderer = _teleboardWarpGameObject.GetComponentInChildren<Renderer>();
				}
				_telewarpRenderer.enabled = false;
				_teleboardAnimation.normalizedTime = 0f;
				_teleboardAnimation.weight = 0f;
				_teleboardAnimation.enabled = false;
				for (int i = 1; i < _cachedHoverboardBundle.CustomSets.Length; i++)
				{
					if (BoardManager.IsCustomSetEquippedForBoard(_cachedHoverboardBundle.boardId, i - 1) && _cachedHoverboardBundle.CustomSets[i].ability == Hoverboard.BoardAbility.Teleport)
					{
						if (_cachedHoverboardBundle.CustomSets[i].ExtraFXPrefab != null)
						{
							_extraTeleportFX = UnityEngine.Object.Instantiate(_cachedHoverboardBundle.CustomSets[i].ExtraFXPrefab).transform;
							_extraTeleportFX.parent = _character.transform;
							_extraTeleportFX.localPosition = Vector3.zero;
							_extraTeleportFX.localRotation = Quaternion.identity;
							_extraTeleportFX.gameObject.SetActive(false);
							ATeleboardEffect component2 = _extraTeleportFX.GetComponent<ATeleboardEffect>();
							component2.OnTeleboardEffectEnd = (Action)Delegate.Combine(component2.OnTeleboardEffectEnd, new Action(HandleOnExtraTeleboardEffectEnd));
						}
						break;
					}
				}
				_hoverboardSafeAnimation.Animation = newHoverboardBundle.gameObject.GetComponent<Animation>();
				_hoverboardRendering = newHoverboardBundle.gameObject.GetComponentInChildren<HoverboardRendering>();
				if (_hoverboardRendering != null)
				{
					_hoverboardRendering.Initialize(_characterSafeAnimation.Animation, _hoverboardSafeAnimation.Animation, _defaultHoverboardAnimationList, _addedAnimClipsNames, abilities);
					return;
				}
				_hoverboardRendering = _defaultHoverboardRendering;
				_hoverboardRendering.Initialize(_characterSafeAnimation.Animation, _hoverboardSafeAnimation.Animation, _defaultHoverboardAnimationList, _addedAnimClipsNames, abilities);
			}
			else
			{
				_hoverboardSafeAnimation.Animation = null;
				_hoverboardRendering = null;
				_teleboardWarpGameObject.SetActive(false);
			}
		}

		private void OnSwitchToJetpack()
		{
			animations.RUN = InitializeClips(jetpackAnimations.run);
			animations.DODGE_LEFT = InitializeClips(jetpackAnimations.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(jetpackAnimations.dodgeRight);
			animations.TURBO_HEADSTART1 = InitializeClips(jetpackAnimations.turboHeadstart1);
			animations.TURBO_HEADSTART2 = InitializeClips(jetpackAnimations.turboHeadstart2);
			_characterModel.MeshJetpack.enabled = true;
			_characterRenderingEffects.SetRightAndLeftParticlesActive(true);
			string run = animations.Run;
			_characterSafeAnimation.CrossFade(run);
		}

		private void JetpackOnFlyAheadStart()
		{
			_initRot = _characterRenderingEffects.JetpackParticles.transform.rotation.eulerAngles;
			_initScale = _characterRenderingEffects.JetpackParticles.transform.localScale;
		}

		private void JetpackOnFlyAheadUpdate(float ratio)
		{
			float num = Mathf.Lerp(0f, 1f, jetpackParticleOffsetCurve.Evaluate(ratio));
			_characterRenderingEffects.JetpackParticles.transform.rotation = Quaternion.Euler(_initRot - new Vector3(num, 0f, 0f));
			_characterRenderingEffects.JetpackParticles.transform.localScale = _initScale + new Vector3(0f, 0f, num * 2f);
		}

		private void JetpackOnStop()
		{
			_characterRenderingEffects.SetRightAndLeftParticlesActive(false);
			_characterRenderingEffects.JetpackParticles.transform.localScale = _initScale;
			_characterModel.MeshJetpack.enabled = false;
			if (_hoverboard.isActive)
			{
				OnSwitchToHoverboard(_cachedHoverboardBundle, _currentBoardAbilities);
			}
			else if (_superSneakers.isActive)
			{
				OnSwitchToSuperSneakers();
				Character_OnHangtime();
			}
			else
			{
				OnSwitchToRunning();
				Character_OnHangtime();
			}
		}

		private void HandleOnTurboHeadstart(int version)
		{
			if (_isRolling)
			{
				EndRollAnimation();
			}
			string text = string.Empty;
			switch (version)
			{
			case 1:
				text = animations.TurboHeadstart1;
				_characterSafeAnimation.CrossFade(text, 0.1f);
				_characterSafeAnimation.CrossFadeQueued(animations.Run, 0.2f);
				break;
			case 2:
				_character.TurboHeadstartParticleSystem.Play();
				text = animations.TurboHeadstart2;
				_characterSafeAnimation.CrossFade(text, 0.1f);
				_characterSafeAnimation.CrossFadeQueued(animations.Run, 0.2f);
				break;
			}
			if (!(text != string.Empty))
			{
				return;
			}
			Transform[] jetpackThrusters = _characterModel.JetpackThrusters;
			foreach (Transform transform in jetpackThrusters)
			{
				transform.GetComponent<Renderer>().enabled = true;
			}
			StopAllCoroutines();
			StartCoroutine(pTween.To(_characterSafeAnimation.GetAnimationState(text).length, (float t) =>
			{
				JetpackOnFlyAheadUpdate(t);
				_characterCamera.Shake(20f);
				float num = 0f;
				num = ((t < 0.2f) ? (t * 5f) : ((!(t > 0.8f)) ? 1f : ((1f - t) * 5f)));
				num += Mathf.Sin(t * 2f) * 0.2f;
				if (num < 0f)
				{
					num *= -1f;
				}
				Transform[] jetpackThrusters2 = _characterModel.JetpackThrusters;
				foreach (Transform transform2 in jetpackThrusters2)
				{
					if (GameStateMachine.Instance.GetState<HeadstartState>(3).TurboHeadstart < 2)
					{
						transform2.localScale = Vector3.one * num;
					}
					else
					{
						transform2.localScale = new Vector3(num * 2.5f, num, num);
					}
				}
				if (t == 1f)
				{
					Transform[] jetpackThrusters3 = _characterModel.JetpackThrusters;
					foreach (Transform transform3 in jetpackThrusters3)
					{
						transform3.GetComponent<Renderer>().enabled = false;
					}
				}
			}));
		}

		private void ResetJetpackTrusters()
		{
			if (_characterModel.JetpackThrusters != null)
			{
				Transform[] jetpackThrusters = _characterModel.JetpackThrusters;
				foreach (Transform transform in jetpackThrusters)
				{
					transform.GetComponent<Renderer>().enabled = false;
				}
			}
		}

		private void HandleOnScorebooster(int tier)
		{
			_characterRenderingEffects.PlayScoreboostFX(tier);
		}

		private void HandleOnExtraTeleboardEffectEnd()
		{
			ResetTeleportFx();
			if (OnExtraTeleboardEffectEnd != null)
			{
				OnExtraTeleboardEffectEnd();
			}
		}

		private void PogostickOnStart()
		{
			animations.RUN = InitializeClips(pogostickClips.run);
			animations.DODGE_LEFT = InitializeClips(pogostickClips.dodgeLeft);
			animations.DODGE_RIGHT = InitializeClips(pogostickClips.dodgeRight);
			animations.HANGTIME = InitializeClips(pogostickClips.hangtime, CharacterModel.CharacterRig.CharacterCustomization.OutfitInfo.ExcludedAnimationsNames);
			_characterModel.MeshPogostick.enabled = true;
			_characterModel.MeshBlobShadow.enabled = false;
			_characterRenderingEffects.SetPogostickParticlesActive(true);
			ResetTeleportFx();
			string run = animations.Run;
			_characterSafeAnimation.CrossFade(run);
		}

		private void PogostickOnHangtime()
		{
			string hangtime = animations.Hangtime;
			_characterSafeAnimation.CrossFade(hangtime, 0.2f);
			_characterRenderingEffects.SetPogostickParticlesActive(false);
		}

		private void PogostickOnStop()
		{
			_characterModel.MeshPogostick.enabled = false;
			_characterRenderingEffects.SetPogostickParticlesActive(false);
			if (_hoverboard.isActive)
			{
				OnSwitchToHoverboard(_cachedHoverboardBundle, _currentBoardAbilities);
			}
			else if (_superSneakers.isActive)
			{
				OnSwitchToSuperSneakers();
				Character_OnHangtime();
			}
			else if (!_game.IsInJetpackMode)
			{
				OnSwitchToRunning();
				Character_OnHangtime();
			}
		}

		private void IsInGame_OnChange(bool isInGame)
		{
			if (!isInGame)
			{
				JetpackOnStop();
				PogostickOnStop();
			}
		}

		private void OnSwitchToSuperSneakers()
		{
			if (!_hoverboard.isActive)
			{
				OnSwitchToRunning();
				if (!_isRolling)
				{
					OnRun();
				}
				else
				{
					OverrideRollTransition();
				}
			}
		}

		private void SuperSneakersOnStop()
		{
			if (!_hoverboard.isActive)
			{
				OnSwitchToRunning();
				OnRun();
			}
		}

		private void SwitchToAlternateRoute(AlternateRoute route)
		{
			ICustomAnimationClipList alternateRouteAnimationClipList = route.AlternateRouteAnimationClipList;
			if (alternateRouteAnimationClipList != null)
			{
				alternateRouteAnimationClipList.Initialize(animations, this);
			}
			jumpAnimation = animations.RouteEnter;
			_characterSafeAnimation.CrossFade(jumpAnimation, 0.1f);
			_characterRenderingEffects.SetPogostickParticlesActive(true);
			_queuedHangtime = true;
			ResetTeleportFx();
		}

		private void SwitchFromAlternateRoute(bool isExitingFromAir)
		{
			if (isExitingFromAir)
			{
				if (!_character.IsRolling)
				{
					jumpAnimation = animations.BigAirTrick;
					if (_characterSafeAnimation.HasAnimationClip(jumpAnimation))
					{
						_characterSafeAnimation.CrossFade(jumpAnimation, 0.3f);
					}
				}
				_character.OnHangtime += Character_OnHangtime_AfterAlternateRoute;
			}
			if (!isExitingFromAir)
			{
				OnSwitchToRunningAfterAlternateRoute();
			}
		}

		private IEnumerator RouteCelebrate_Coroutine()
		{
			string routeCelebration = animations.RouteCelebrate;
			if (_characterSafeAnimation.HasAnimationClip(routeCelebration))
			{
				_characterSafeAnimation.CrossFade(routeCelebration, 0.1f);
			}
			yield return null;
		}

		private void SetPieceTransitionTriggerState_OnHangtime()
		{
			string hangtime = animations.Hangtime;
			_characterSafeAnimation.CrossFade(hangtime, 0.2f);
			SetPieceTransitionTriggerState_OnEnd();
		}

		private void SetPieceTransitionTriggerState_OnEnd()
		{
			_characterRenderingEffects.SetPogostickParticlesActive(false);
		}

		private void HandleOnTeleboardStart(float duration, Vector3 offset, float gameSpeed)
		{
			if (_isTeleportFxActive)
			{
				StopAllCoroutines();
				ResetCharacterTeleportFX();
			}
			if (_extraTeleportFX != null)
			{
				ATeleboardEffect component = _extraTeleportFX.GetComponent<ATeleboardEffect>();
				if (component != null)
				{
					component.StartTeleBoardEffect();
				}
				return;
			}
			_telewarpRenderer.enabled = true;
			_teleboardAnimation.normalizedTime = 0f;
			_teleboardAnimation.weight = 0f;
			_teleboardAnimation.enabled = false;
			_isTeleportFxActive = true;
			Material sharedMaterial = _characterModel.Model.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial != teleboardFXMaterial)
			{
				_cachedModelMaterial = sharedMaterial;
			}
			_characterModel.Model.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material[] sharedMaterials = _characterModel.HeadModel.sharedMaterials;
			_cachedHeadMaterials = new Material[sharedMaterials.Length];
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				_cachedHeadMaterials[i] = sharedMaterials[i];
				sharedMaterials[i] = teleboardFXMaterial;
			}
			_characterModel.HeadModel.materials = sharedMaterials;
			Material sharedMaterial2 = _characterModel.OutfitModel.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial2 != teleboardFXMaterial)
			{
				_cachedCustomizationMaterial = sharedMaterial2;
			}
			_characterModel.OutfitModel.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material sharedMaterial3 = _characterModel.MeshSuperSneaker.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial3 != teleboardFXMaterial)
			{
				_cachedSupersneakersMaterial = sharedMaterial3;
			}
			_characterModel.MeshSuperSneaker.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material sharedMaterial4 = _characterModel.DoubleJumpers.GetComponent<Renderer>().sharedMaterial;
			if (sharedMaterial4 != teleboardFXMaterial)
			{
				_cachedDoubleJumperMaterial = sharedMaterial4;
			}
			_characterModel.DoubleJumpers.GetComponent<Renderer>().material = teleboardFXMaterial;
			Material sharedMaterial5 = _characterModel.CurrentHoverboadBasemodel.sharedMaterial;
			if (sharedMaterial5 != teleboardFXMaterial)
			{
				_cachedHoverboardMaterial = sharedMaterial5;
			}
			_characterModel.CurrentHoverboadBasemodel.material = teleboardFXMaterial;
			if (_characterModel.CurrentHoverboardModelsRenderes.Count > 0)
			{
				for (int j = 0; j < _characterModel.CurrentHoverboardModelsRenderes.Count; j++)
				{
					if (_characterModel.CurrentHoverboardModelsRenderes[j] != null)
					{
						Material sharedMaterial6 = _characterModel.CurrentHoverboardModelsRenderes[j].sharedMaterial;
						if (sharedMaterial6 != teleboardFXMaterial && sharedMaterial6 != null)
						{
							_cachedCustomHoverboardModelMaterials[j] = sharedMaterial6;
						}
					}
				}
				for (int k = 0; k < _characterModel.CurrentHoverboardModelsRenderes.Count; k++)
				{
					if (_characterModel.CurrentHoverboardModelsRenderes[k] != null)
					{
						_characterModel.CurrentHoverboardModelsRenderes[k].material = teleboardFXMaterial;
					}
				}
			}
			_isCachedCustomizationState = _characterModel.OutfitModel.enabled;
			StartCoroutine(TeleboardStartScale(duration, gameSpeed));
		}

		private IEnumerator TeleboardStartScale(float duration, float gameSpeed)
		{
			float time = 0f;
			float t = 0f;
			float z = 0f;
			Vector3 model1position = _character.TransformPosition;
			HierarchyUtility.ReparentByHierarchyPathType(_characterModel.transform, HierarchyPathType.Null);
			_characterSafeAnimation.Stop();
			while (time < duration)
			{
				time += Time.deltaTime;
				t = Mathf.Clamp01(time / duration);
				if (!_isTeleportFxActive)
				{
					ResetTeleportFx();
					break;
				}
				z = _character.TransformPosition.z;
				model1position.z = z;
				_characterModel.transform.position = model1position;
				_characterModel.transform.localScale = new Vector3(1f - 0.5f * t, 1f + 0.5f * t, 1f - t);
				teleboardFXMaterial.SetFloat("Offset", t * 0.2f);
				if (t == 1f)
				{
					_characterModel.transform.parent = base.transform;
					_characterModel.transform.localPosition = Vector3.zero;
					_characterModel.transform.localScale = Vector3.zero;
					if (_character.IsGrounded.Value)
					{
						OnRun();
					}
					HandleOnTeleboardEnd(duration, gameSpeed);
				}
				yield return null;
			}
		}

		public void HandleOnTeleboardEnd(float duration, float gameSpeed)
		{
			if (_character.IsGrounded.Value)
			{
				OnRun();
			}
			else
			{
				Character_OnHangtime();
			}
			_teleboardWarpGameObject.transform.parent = _character.transform;
			_teleboardWarpGameObject.transform.localPosition = Vector3.zero;
			_teleboardAnimation.normalizedTime = 1f;
			_teleboardAnimation.weight = 1f;
			_teleboardAnimation.speed = 0f - 0.01f * gameSpeed;
			_teleboardAnimation.enabled = true;
			StartCoroutine(TeleboardEndScale(duration));
		}

		private IEnumerator TeleboardEndScale(float duration)
		{
			float time = 0f;
			float t = 0f;
			while (time < duration)
			{
				time += Time.deltaTime;
				t = Mathf.Clamp01(time / duration);
				teleboardFXMaterial.SetFloat("Offset", t * 0.2f);
				if (t == 1f)
				{
					ResetCharacterTeleportFX();
				}
				yield return null;
			}
		}

		private void ResetCharacterTeleportFX()
		{
			_characterModel.transform.parent = base.transform;
			_characterModel.transform.localPosition = Vector3.zero;
			_characterModel.transform.localScale = Vector3.one;
			_characterModel.Model.enabled = true;
			_characterModel.HeadModel.enabled = true;
			_characterModel.OutfitModel.enabled = _isCachedCustomizationState;
			if (Hoverboard.Instance.isActive)
			{
				if (_characterModel.CurrentHoverboard != null && !_game.IsInPogostickMode)
				{
					_characterModel.CurrentHoverboard.SetActive(true);
				}
				if (_cachedHoverboardMaterial != null)
				{
					_characterModel.CurrentHoverboadBasemodel.material = _cachedHoverboardMaterial;
				}
				if (_cachedCustomHoverboardModelMaterials != null && _cachedCustomHoverboardModelMaterials.Length > 0)
				{
					for (int i = 0; i < _characterModel.CurrentHoverboardModelsRenderes.Count; i++)
					{
						if (_characterModel.CurrentHoverboardModelsRenderes[i] != null)
						{
							Renderer renderer = _characterModel.CurrentHoverboardModelsRenderes[i];
							if (renderer != null)
							{
								renderer.material = _cachedCustomHoverboardModelMaterials[i];
							}
						}
					}
				}
			}
			if (_cachedModelMaterial != null)
			{
				_characterModel.Model.GetComponent<Renderer>().material = _cachedModelMaterial;
				_cachedModelMaterial = null;
			}
			if (_cachedHeadMaterials != null)
			{
				_characterModel.HeadModel.materials = _cachedHeadMaterials;
				_cachedHeadMaterials = null;
			}
			if (_cachedCustomizationMaterial != null)
			{
				_characterModel.OutfitModel.GetComponent<Renderer>().material = _cachedCustomizationMaterial;
				_cachedCustomizationMaterial = null;
			}
			if (_cachedSupersneakersMaterial != null)
			{
				_characterModel.MeshSuperSneaker.GetComponent<Renderer>().material = _cachedSupersneakersMaterial;
				_cachedSupersneakersMaterial = null;
			}
			if (_cachedDoubleJumperMaterial != null)
			{
				_characterModel.DoubleJumpers.GetComponent<Renderer>().material = _cachedDoubleJumperMaterial;
				_cachedDoubleJumperMaterial = null;
			}
			_characterSafeAnimation.Play();
			if (_character.IsGrounded.Value)
			{
				OnRun();
			}
			_isTeleportFxActive = false;
			if (OnResetCharacterTeleportFX != null)
			{
				OnResetCharacterTeleportFX();
			}
		}

		private void OnApplicationPause(bool isApplicationPaused)
		{
			if (isApplicationPaused)
			{
				ResetTeleportFx();
			}
		}

		public void ResetTeleportFx()
		{
			if (_isTeleportFxActive)
			{
				StopAllCoroutines();
				ResetCharacterTeleportFX();
			}
		}

		private void Character_OnEnterRoute()
		{
		}

		private void Character_OnEnterRouteEnd()
		{
			_characterRenderingEffects.SetPogostickParticlesActive(false);
		}

		private void Character_OnExitRoute()
		{
		}

		private void RouteManager_OnRouteCreated(object sender, RouteManager.RouteCreatedEventArgs args)
		{
			AlternateRoute alternateRoute = args.Route as AlternateRoute;
			if (alternateRoute != null)
			{
				alternateRoute.OnCharacterExited += AlternateRoute_OnCharacterExited;
				alternateRoute.OnDestructing += AlternateRoute_OnDestructing;
				alternateRoute.OnCharacterEntryStarted += AlternateRoute_OnCharacterEntryStarted;
				alternateRoute.OnCharacterEntrySkipped += AlternateRoute_OnCharacterEntrySkipped;
			}
		}

		private void AlternateRoute_OnCharacterEntryStarted(object sender, Route.CharacterEntryStartedEventArgs args)
		{
			SwitchToAlternateRoute(args.ToRoute as AlternateRoute);
		}

		private void AlternateRoute_OnCharacterEntrySkipped(object sender, Route.CharacterEntrySkippedEventArgs args)
		{
			if (args.HasCharacterStartedEntryTransition)
			{
				OnRouteDone(false);
			}
		}

		private void AlternateRoute_OnCharacterExited(object sender, Route.CharacterExitedEventArgs args)
		{
			AlternateRoute alternateRoute = sender as AlternateRoute;
			alternateRoute.OnCharacterExited -= AlternateRoute_OnCharacterExited;
			alternateRoute.OnCharacterEntrySkipped -= AlternateRoute_OnCharacterEntrySkipped;
			SwitchFromAlternateRoute(true);
		}

		private void OnRouteDone(bool isRouteDestructing)
		{
			if (!isRouteDestructing)
			{
				SwitchFromAlternateRoute(false);
			}
		}

		private void AlternateRoute_OnDestructing(object sender, EventArgs args)
		{
			Route route = (Route)sender;
			route.OnCharacterExited -= AlternateRoute_OnCharacterExited;
			route.OnDestructing -= AlternateRoute_OnDestructing;
		}
	}
}
