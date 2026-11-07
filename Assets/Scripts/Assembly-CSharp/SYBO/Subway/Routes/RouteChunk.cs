using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Managers;
using SYBO.Subway.Routes.Builders;
using SYBO.Subway.Themes;
using SYBO.Subway.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Routes
{
	public class RouteChunk : MonoBehaviour
	{
		public class ChunkBlockConstraint
		{
			public readonly EnvironmentKind EnvironmentKind;

			public readonly EnvironmentBaseKind EnvironmentBaseKind;

			public static readonly ChunkBlockConstraint None = new ChunkBlockConstraint(EnvironmentKind.None, EnvironmentBaseKind.Tracks3);

			public ChunkBlockConstraint(EnvironmentKind envKind, EnvironmentBaseKind envBaseKind)
			{
				EnvironmentKind = envKind;
				EnvironmentBaseKind = envBaseKind;
			}

			public static bool operator ==(ChunkBlockConstraint c1, ChunkBlockConstraint c2)
			{
				return c1.Equals(c2);
			}

			public static bool operator !=(ChunkBlockConstraint c1, ChunkBlockConstraint c2)
			{
				return !c1.Equals(c2);
			}

			public override bool Equals(object other)
			{
				if (!(other is ChunkBlockConstraint))
				{
					return false;
				}
				ChunkBlockConstraint chunkBlockConstraint = (ChunkBlockConstraint)other;
				return EnvironmentKind == chunkBlockConstraint.EnvironmentKind && EnvironmentBaseKind == chunkBlockConstraint.EnvironmentBaseKind;
			}

			public override int GetHashCode()
			{
				int num = 17;
				num = num * 29 + EnvironmentKind.GetHashCode();
				return num * 29 + EnvironmentBaseKind.GetHashCode();
			}
		}

		public const int TrackLaneWidth = 20;

		public const int TrackWidth = 60;

		public const int TrackHeight = 90;

		public const int BlockLength = 90;

		public const int BlockCountMin = 2;

		public const int MinChunkLength = 180;

		private const float BaseColliderCenterY = -9.4f;

		private const float BaseColliderSizeY = 20f;

		private readonly Vector3 LogicHideOffset = Vector3.up * -1000f;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnDeactivate__BackingField;

		public static Action<EnvironmentKind, int, Route> OnEnvironmentSegmentSpawned;

		[Header("Spawn Conditions")]
		[Tooltip("Indicates if chunk can be spawned more than once within a section. Doesn't disallow from appearing in another section.")]
		[SerializeField]
		private bool _isRepeatableInSections = true;

		[Header("Appearence")]
		[FormerlySerializedAs("_numBlocks")]
		[Tooltip("Determines length of the chunk. Each block is 90 units")]
		[SerializeField]
		[Range(2f, 48f)]
		private int _blockCount = 2;

		[Tooltip("Indicates if surrounding environment should be generated")]
		[SerializeField]
		private bool _generateEnvironment;

		[Tooltip("Indicates if environment base should be generated (rails or something else specific to current environment)")]
		[SerializeField]
		private bool _generateEnvironmentBase = true;

		[Tooltip("If none are specifically set as allowed, then all will be possible")]
		[SerializeField]
		private EnvironmentKind[] _limitedAllowedEnvironmentKinds;

		[SerializeField]
		[HideInInspector]
		private List<TrackObject> _trackObjects = new List<TrackObject>();

		private List<TrackObject> _environmentTrackObjects = new List<TrackObject>();

		[SerializeField]
		[HideInInspector]
		private EnvironmentBase[] _forcedEnvironmentBases;

		[SerializeField]
		[HideInInspector]
		private SYBO.Subway.Level.Environment[] _forcedEnvironments;

		[Header("Collected From Children")]
		[SerializeField]
		[HideInInspector]
		private List<Selector> _selectors = new List<Selector>();

		private List<Selector> _environmentSelectors = new List<Selector>();

		[SerializeField]
		private RouteTransitionTrigger _routeTransitionTrigger;

		[Header("Runtime Fields (unset)")]
		public RouteChunk Previous;

		public RouteChunk Next;

		[Header("Reporting")]
		[SerializeField]
		private string _reportedName;

		[SerializeField]
		[HideInInspector]
		public int PreviewEnvironmentIndex;

		private Transform _transform;

		private Transform _environmentTransform;

		private List<ChunkBlockConstraint> _blockConstraints;

		private LogicAssetsManager _logicAssetManager;

		private ThemeManager _themeManager;

		private bool _built;

		private Dictionary<Transform, Vector3> _hiddenObstacles = new Dictionary<Transform, Vector3>();

		private RouteBuilder _builder;

		internal RouteSection _section;

		private RouteChunk _sourcePrefab;

		public bool GenerateEnvironmentBase
		{
			get
			{
				return _generateEnvironmentBase;
			}
		}

		public List<Selector> Selectors
		{
			get
			{
				return _selectors;
			}
		}

		public List<Selector> EnvironmentSelectors
		{
			get
			{
				return _environmentSelectors;
			}
		}

		public Transform EnvironmentTransform
		{
			get
			{
				return _environmentTransform;
			}
		}

		public bool IsRepeatableInSections
		{
			get
			{
				return _isRepeatableInSections;
			}
		}

		public bool IsBuilt
		{
			get
			{
				return _built;
			}
		}

		public int Length
		{
			get
			{
				return 90 * _blockCount;
			}
		}

		public int BlockCount
		{
			get
			{
				return _blockCount;
			}
		}

		public bool GenerateEnvironment
		{
			get
			{
				return _generateEnvironment;
			}
		}

		public IList<EnvironmentKind> LimitedAllowedEnvironmentKinds
		{
			get
			{
				return _limitedAllowedEnvironmentKinds;
			}
		}

		public Bounds TrackBounds
		{
			get
			{
				Vector3 size = new Vector3(60f, 90f, Length);
				Vector3 vector = new Vector3(0f, size.y, size.z) * 0.5f;
				return new Bounds(GamePositionStart + vector, size);
			}
		}

		public float GamePositionStartZ
		{
			get
			{
				return GamePositionStart.z;
			}
		}

		public Vector3 GamePositionStart
		{
			get
			{
				return GamePositionRoot.Instance.GetGamePosition(_transform);
			}
		}

		public Vector3 GamePositionEnd
		{
			get
			{
				return GamePositionStart + Vector3.forward * Length;
			}
		}

		public RouteBuilder RouteBuilder
		{
			get
			{
				return _builder;
			}
		}

		public RouteSection Section
		{
			get
			{
				return _section;
			}
		}

		public string ReportedName
		{
			get
			{
				return _reportedName;
			}
		}

		public RouteTransitionTrigger RouteTransitionTrigger
		{
			get
			{
				return _routeTransitionTrigger;
			}
		}

		public event Action OnDeactivate
		{
			add
			{
				Action action = OnDeactivate__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDeactivate__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnDeactivate__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDeactivate__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void Initialize(RouteBuilder builder, RouteSection section, RouteChunk sourcePrefab)
		{
			_builder = builder;
			_section = section;
			_sourcePrefab = sourcePrefab;
		}

		protected void Awake()
		{
			_transform = base.transform;
			_logicAssetManager = SingletonMonoBehaviour<LogicAssetsManager>.Instance;
			_themeManager = ThemeManager.Instance;
			_environmentTransform = new GameObject("GeneratedEnvironment").transform;
			_environmentTransform.parent = _transform;
			_environmentTransform.ResetLocalPosition();
			_trackObjects.Clear();
			this.GetComponentsInChildrenBFS(true, _trackObjects);
			Rigidbody rigidbody = _environmentTransform.AddComponentIfMissing<Rigidbody>();
			rigidbody.isKinematic = true;
			BoxCollider boxCollider = _environmentTransform.AddComponentIfMissing<BoxCollider>();
			boxCollider.center = new Vector3(0f, -9.4f, 0.5f * (float)Length);
			boxCollider.size = new Vector3(70f, 20f, Length);
			boxCollider.tag = "Ground";
			_forcedEnvironmentBases = GetComponentsInChildren<EnvironmentBase>();
			_forcedEnvironments = GetComponentsInChildren<SYBO.Subway.Level.Environment>();
			Array.Sort(_forcedEnvironmentBases, (EnvironmentBase l, EnvironmentBase r) => SYBO.Common.Utilities.GameObjectUtil.ComparePositionZ(l.transform.position, r.transform.position));
			Array.Sort(_forcedEnvironments, (SYBO.Subway.Level.Environment l, SYBO.Subway.Level.Environment r) => SYBO.Common.Utilities.GameObjectUtil.ComparePositionZ(l.transform.position, r.transform.position));
			EvaluateEnvironmentConstraints();
		}

		public IEnumerator Destruct()
		{
			if (Next != null)
			{
				Next.Previous = null;
			}
			Next = null;
			if (Previous != null)
			{
				Previous.Next = null;
			}
			Previous = null;
			while (_environmentTransform.childCount > 0)
			{
				PoolManager.Despawn(_environmentTransform.GetChild(0));
				yield return null;
			}
			_built = false;
		}

		private void OnDrawGizmos()
		{
			int num = ((_limitedAllowedEnvironmentKinds == null || !_limitedAllowedEnvironmentKinds.Contains(EnvironmentKind.Epic)) ? 180 : 360);
			float zStep = 90f;
			DrawSegments(base.transform.position, Length, 0f, zStep, Color.white, Color.white, Color.white);
			DrawSegments(base.transform.position, Length, 90f, num, Color.green, Color.white, Color.red);
		}

		public void DrawSegments(Vector3 from, float length, float ySize, float zStep, Color startColor, Color midColor, Color endColor)
		{
			Gizmos.color = startColor;
			float num = 0f;
			Vector3 size = new Vector3(60f, ySize, zStep);
			while (num < length)
			{
				Vector3 center = from + new Vector3(0f, 0.5f * ySize, num + 0.5f * zStep);
				Gizmos.DrawWireCube(center, size);
				num += zStep;
				if (num < length - zStep)
				{
					Gizmos.color = midColor;
				}
				else
				{
					Gizmos.color = endColor;
				}
			}
		}

		public bool SupportsAnyEnvironmentOf(IList<EnvironmentKind> environmentKinds)
		{
			if (_limitedAllowedEnvironmentKinds.Length == 0)
			{
				return true;
			}
			foreach (EnvironmentKind environmentKind in environmentKinds)
			{
				if (_limitedAllowedEnvironmentKinds.Contains(environmentKind))
				{
					return true;
				}
			}
			return false;
		}

		public bool SupportsEnvironment(EnvironmentKind environmentKind)
		{
			if (environmentKind == EnvironmentKind.None)
			{
			}
			if (_limitedAllowedEnvironmentKinds.Length == 0)
			{
				return true;
			}
			return _limitedAllowedEnvironmentKinds.Contains(environmentKind);
		}

		public EnvironmentKind GetRandomAllowedEnvironmentKind(SyboRandom random)
		{
			int num = random.Range(0, _limitedAllowedEnvironmentKinds.Length);
			return _limitedAllowedEnvironmentKinds[num];
		}

		public override string ToString()
		{
			RouteType routeType = RouteType.None;
			if (_builder != null)
			{
				routeType = _builder.Route.Type;
			}
			return string.Format("[RouteChunk: name={0} routeType={1}, length={2}, z={3}]", base.name, routeType, Length, base.transform.position.z);
		}

		public IEnumerator ActivateTrackObjectsASync()
		{
			for (int i = 0; i < _trackObjects.Count; i++)
			{
				TrackObject to = _trackObjects[i];
				if (to == null)
				{
					SDebug.LogErrorFormat(this, "{0} inst {1} ActivateTrackObjects found destroyed track object at index {2}", base.name, GetInstanceID(), i);
				}
				else if (to.gameObject.activeInHierarchy)
				{
					to.Activate();
					yield return null;
				}
			}
			for (int j = 0; j < _environmentTrackObjects.Count; j++)
			{
				TrackObject to2 = _environmentTrackObjects[j];
				if (to2 == null)
				{
					SDebug.LogErrorFormat(this, "{0} inst {1} ActivateTrackObjects found destroyed track object at index {2}", base.name, GetInstanceID(), j);
				}
				else if (to2.gameObject.activeInHierarchy)
				{
					to2.Activate();
					yield return null;
				}
			}
		}

		public IEnumerator DeactivateTrackObjects()
		{
			if (OnDeactivate__BackingField != null)
			{
				OnDeactivate__BackingField();
			}
			for (int i = 0; i < _trackObjects.Count; i++)
			{
				TrackObject to = _trackObjects[i];
				if (to == null)
				{
					SDebug.LogErrorFormat(this, "{0} inst {1} DeactivateTrackObjects found destroyed track object at index {2}", base.name, GetInstanceID(), i);
				}
				else if (to.gameObject.activeInHierarchy && to.Active)
				{
					to.Deactivate();
					yield return null;
				}
			}
			for (int j = 0; j < _environmentTrackObjects.Count; j++)
			{
				TrackObject to2 = _environmentTrackObjects[j];
				if (to2 == null)
				{
					SDebug.LogErrorFormat(this, "{0} inst {1} DeactivateTrackObjects found destroyed track object at index {2}", base.name, GetInstanceID(), j);
				}
				else if (to2.gameObject.activeInHierarchy && to2.Active)
				{
					to2.Deactivate();
					yield return null;
				}
			}
			_environmentTrackObjects.Clear();
		}

		public void Build(EnvironmentKind routeDesiredEnvironmentKind, EnvironmentKind previousDesiredEnvironmentKind)
		{
			RestoreHiddenLogic();
			_environmentSelectors.Clear();
			if (!_generateEnvironment && !_generateEnvironmentBase)
			{
				_built = true;
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			EnvironmentKind environmentKind = EnvironmentKind.None;
			EnvironmentKind environmentKind2 = previousDesiredEnvironmentKind;
			EnvironmentKind environmentKind3 = routeDesiredEnvironmentKind;
			int prevSegmentVariation = 0;
			EnvironmentBaseKind environmentBaseKind = EnvironmentBaseKind.Tracks3;
			if (1 < _blockCount)
			{
				environmentBaseKind = GetEnvironmentConstraintFromIndex(1).EnvironmentBaseKind;
			}
			int num4 = -1;
			EnvironmentSequence environmentSequence = EnvironmentSequence.None;
			while (num3 < _blockCount)
			{
				ChunkBlockConstraint environmentConstraintFromIndex = GetEnvironmentConstraintFromIndex(num3);
				if (num == num3)
				{
					environmentKind3 = routeDesiredEnvironmentKind;
					EnvironmentKind environmentKind4 = environmentConstraintFromIndex.EnvironmentKind;
					if (environmentKind4 != EnvironmentKind.None)
					{
						environmentKind3 = environmentKind4;
					}
					num4 = GetCurrentEnvironmentEndBlock(num3);
					if (num4 == -1)
					{
						num4 = _blockCount - 1;
					}
					bool flag = environmentKind3 == routeDesiredEnvironmentKind;
					bool flag2 = environmentKind3 != environmentKind;
					bool shouldEndCurrentEnvironment = false;
					if (num3 > 0 && !flag && flag2)
					{
						shouldEndCurrentEnvironment = true;
					}
					bool shouldStartCurrentEnvironment = false;
					if (environmentKind2 != environmentKind3)
					{
						shouldStartCurrentEnvironment = true;
					}
					if (environmentKind2 != environmentKind3)
					{
						prevSegmentVariation = 0;
					}
					if (environmentKind3 != EnvironmentKind.None && environmentKind3 != EnvironmentKind.Empty)
					{
						EnvironmentInfo byKind = EnvironmentInfo.GetByKind(environmentKind3);
						environmentSequence = EvalNextEnvironmentSequence(num, num4, byKind, shouldStartCurrentEnvironment, shouldEndCurrentEnvironment);
						num = CreateEnvironmentSegment(num3, byKind, environmentSequence, ref prevSegmentVariation);
					}
					else
					{
						num++;
					}
					if (OnEnvironmentSegmentSpawned != null)
					{
						int arg = Mathf.FloorToInt(GamePositionStartZ) + num3 * 90;
						OnEnvironmentSegmentSpawned(environmentKind3, arg, RouteBuilder.Route);
					}
				}
				if (num2 == num3)
				{
					EnvironmentBaseKind environmentBaseKind2 = GetEnvironmentConstraintFromIndex(num3).EnvironmentBaseKind;
					if (_generateEnvironmentBase)
					{
						environmentBaseKind2 = GetEnvironmentBaseKindToPlace(environmentKind3, environmentKind2, routeDesiredEnvironmentKind, environmentBaseKind2, environmentBaseKind, num3, num4);
					}
					EnvironmentBaseKind baseKindNext = environmentBaseKind2;
					if (environmentBaseKind != EnvironmentBaseKind.None)
					{
						baseKindNext = environmentBaseKind;
					}
					num2 = ((!(environmentBaseKind2 != EnvironmentBaseKind.None)) ? (num2 + 1) : (num2 + CreateEnvironmentBase(num3, environmentBaseKind2, baseKindNext, environmentKind3)));
				}
				environmentKind2 = environmentKind3;
				num3++;
				environmentKind = ((num >= _blockCount) ? ((!(Next != null)) ? EnvironmentKind.None : Next.GetEnvironmentConstraintFromIndex(0).EnvironmentKind) : GetEnvironmentConstraintFromIndex(num).EnvironmentKind);
				environmentBaseKind = ((num3 >= _blockCount - 1) ? ((!(Next != null)) ? EnvironmentBaseKind.None : Next.GetEnvironmentConstraintFromIndex(0).EnvironmentBaseKind) : GetEnvironmentConstraintFromIndex(num3).EnvironmentBaseKind);
			}
			_built = true;
		}

		public void HideLogic(float untilGamePositionZ)
		{
			foreach (Transform item in base.transform)
			{
				if (!(item == _environmentTransform))
				{
					HideLogicRecursive(item, untilGamePositionZ);
				}
			}
		}

		private void HideLogicRecursive(Transform target, float maxGamePositionZ)
		{
			Collider component = target.GetComponent<Collider>();
			Vector3 worldPosition = ((!(component != null)) ? target.position : component.bounds.min);
			Vector3 gamePosition = GamePositionRoot.Instance.GetGamePosition(worldPosition);
			if (!(target.GetComponent<SnapObject>() != null))
			{
				foreach (Transform item in target)
				{
					HideLogicRecursive(item, maxGamePositionZ);
				}
				return;
			}
			if (gamePosition.z < maxGamePositionZ && target.gameObject.layer != Layers.Instance.Unremovable)
			{
				Component[] componentsInChildrenBFS = target.GetComponentsInChildrenBFS(typeof(IHideable), false, true, 1);
				Component[] array = componentsInChildrenBFS;
				foreach (Component component2 in array)
				{
					IHideable hideable = component2 as IHideable;
					hideable.Hide();
				}
				Vector3 value;
				if (!_hiddenObstacles.TryGetValue(target, out value))
				{
					_hiddenObstacles[target] = target.localPosition;
					target.localPosition += LogicHideOffset;
				}
			}
		}

		public void RestoreHiddenLogic()
		{
			foreach (KeyValuePair<Transform, Vector3> hiddenObstacle in _hiddenObstacles)
			{
				if (hiddenObstacle.Key != null)
				{
					hiddenObstacle.Key.localPosition = hiddenObstacle.Value;
				}
				Component[] componentsInChildrenBFS = hiddenObstacle.Key.GetComponentsInChildrenBFS(typeof(IHideable), false, true, 1);
				Component[] array = componentsInChildrenBFS;
				foreach (Component component in array)
				{
					IHideable hideable = component as IHideable;
					hideable.Restore();
				}
			}
			_hiddenObstacles.Clear();
		}

		private EnvironmentBaseKind GetEnvironmentBaseKindToPlace(EnvironmentKind envKind, EnvironmentKind previousEnvKind, EnvironmentKind routeDesiredEnvironment, EnvironmentBaseKind envBaseKindConstraint, EnvironmentBaseKind nextConstraint, int blockIndex, int envKindEndBlock)
		{
			if (envKind == EnvironmentKind.Epic)
			{
				return EnvironmentBaseKind.None;
			}
			if (envKind == EnvironmentKind.Gates)
			{
				return EnvironmentBaseKind.Tracks3GatesShadow;
			}
			bool flag = envKind != previousEnvKind;
			if (envKind == EnvironmentKind.Tube || envKind == EnvironmentKind.Pillars || envKind == EnvironmentKind.Station)
			{
				bool flag2 = blockIndex >= envKindEndBlock - 1;
				if (flag2 && routeDesiredEnvironment == envKind && !_builder.WillChangeEnvironment(Length) && nextConstraint == EnvironmentBaseKind.None)
				{
					flag2 = false;
				}
				if (flag)
				{
					return GetStartEnvironmentBase(envBaseKindConstraint.NumTracks);
				}
				if (flag2)
				{
					return GetEndEnvironmentBase(envBaseKindConstraint.NumTracks);
				}
				switch (envBaseKindConstraint.NumTracks)
				{
				case 1:
					return EnvironmentBaseKind.Tracks1ShadowMid;
				case 2:
					return EnvironmentBaseKind.Tracks2ShadowMid;
				default:
					return EnvironmentBaseKind.Tracks3ShadowMid;
				}
			}
			if (envBaseKindConstraint != EnvironmentBaseKind.None)
			{
				return envBaseKindConstraint;
			}
			return GetDefaultEnvironmentBaseKind(_builder.Route.Type);
		}

		private EnvironmentBaseKind GetStartEnvironmentBase(int numTracks)
		{
			switch (numTracks)
			{
			case 1:
				return EnvironmentBaseKind.Tracks1ShadowStart;
			case 2:
				return EnvironmentBaseKind.Tracks2ShadowStart;
			default:
				return EnvironmentBaseKind.Tracks3ShadowStart;
			}
		}

		private EnvironmentBaseKind GetEndEnvironmentBase(int numTracks)
		{
			switch (numTracks)
			{
			case 1:
				return EnvironmentBaseKind.Tracks1ShadowEnd;
			case 2:
				return EnvironmentBaseKind.Tracks2ShadowEnd;
			default:
				return EnvironmentBaseKind.Tracks3ShadowEnd;
			}
		}

		private EnvironmentBaseKind GetDefaultEnvironmentBaseKind(RouteType routeType)
		{
			if (routeType == RouteType.Default)
			{
				return EnvironmentBaseKind.Tracks3;
			}
			return EnvironmentBaseKind.None;
		}

		private int GetCurrentEnvironmentEndBlock(int currentBlockIndex)
		{
			if (_blockConstraints.Count <= currentBlockIndex)
			{
				SDebug.LogError("Block index " + currentBlockIndex + " out of bounds, block constraint count " + _blockConstraints.Count);
				return -1;
			}
			ChunkBlockConstraint environmentConstraintFromIndex = GetEnvironmentConstraintFromIndex(currentBlockIndex);
			for (int i = currentBlockIndex + 1; i < _blockConstraints.Count; i++)
			{
				ChunkBlockConstraint environmentConstraintFromIndex2 = GetEnvironmentConstraintFromIndex(i);
				if (environmentConstraintFromIndex.EnvironmentKind != environmentConstraintFromIndex2.EnvironmentKind)
				{
					return i - 1;
				}
			}
			return -1;
		}

		private ChunkBlockConstraint GetEnvironmentConstraintFromIndex(int blockIndex)
		{
			if (_blockConstraints.Count <= blockIndex)
			{
				SDebug.LogError("Block index " + blockIndex + " out of bounds, block constraint count " + _blockConstraints.Count);
				return ChunkBlockConstraint.None;
			}
			return _blockConstraints[blockIndex];
		}

		private ChunkBlockConstraint GetNextEnvironmentConstraint(int currentBlockIndex)
		{
			if (_blockConstraints.Count <= currentBlockIndex)
			{
				SDebug.LogError("Block index " + currentBlockIndex + " out of bounds, block constraint count " + _blockConstraints.Count);
				return ChunkBlockConstraint.None;
			}
			int i = currentBlockIndex;
			ChunkBlockConstraint environmentConstraintFromIndex = GetEnvironmentConstraintFromIndex(i);
			ChunkBlockConstraint chunkBlockConstraint = GetEnvironmentConstraintFromIndex(i + 1);
			for (; i < _blockConstraints.Count; i++)
			{
				if (!(environmentConstraintFromIndex.EnvironmentKind != chunkBlockConstraint.EnvironmentKind))
				{
					break;
				}
				chunkBlockConstraint = GetEnvironmentConstraintFromIndex(i + 1);
			}
			if (environmentConstraintFromIndex == chunkBlockConstraint)
			{
				chunkBlockConstraint = ChunkBlockConstraint.None;
			}
			return chunkBlockConstraint;
		}

		private int CreateEnvironmentBase(int blockIndex, EnvironmentBaseKind baseKind, EnvironmentBaseKind baseKindNext, EnvironmentKind envKind)
		{
			int num = 2;
			if (baseKind == EnvironmentBaseKind.Tracks3GatesShadow)
			{
				num = 4;
			}
			else if ((baseKind.ShadowSequence == EnvironmentBaseShadowSequence.Start || baseKind.ShadowSequence == EnvironmentBaseShadowSequence.End) && (envKind == EnvironmentKind.Pillars || envKind == EnvironmentKind.Station))
			{
				num = 1;
			}
			if (baseKind.Type == EnvironmentBaseType.Empty)
			{
				return num;
			}
			if (_themeManager.Theme.ShouldIgnoreBase(envKind))
			{
				return num;
			}
			EnvironmentBaseInfo environmentBaseInfo = new EnvironmentBaseInfo(baseKind, num);
			Transform environmentBaseModel = SingletonMonoBehaviour<AssetManager>.Instance.GetEnvironmentBaseModel(environmentBaseInfo);
			if (environmentBaseModel != null)
			{
				Transform transform = PoolManager.Spawn(environmentBaseModel);
				int num2 = blockIndex * 90;
				transform.parent = _environmentTransform;
				transform.localPosition = num2 * Vector3.forward;
			}
			else
			{
				SDebug.LogError("Env Base at block " + blockIndex + ", no model for ground " + environmentBaseInfo);
			}
			return num;
		}

		private EnvironmentSequence EvalNextEnvironmentSequence(int nextEnvBlockIndex, int envEndBlockIndex, EnvironmentInfo environmentInfo, bool shouldStartCurrentEnvironment, bool shouldEndCurrentEnvironment)
		{
			if (shouldEndCurrentEnvironment)
			{
				return EnvironmentSequence.End;
			}
			if (shouldStartCurrentEnvironment)
			{
				return EnvironmentSequence.Start;
			}
			if (_blockCount <= nextEnvBlockIndex)
			{
				SDebug.LogError("RouteChunk: next environment blockIndex=" + nextEnvBlockIndex + " out of bounds, numBlocks= " + _blockCount);
				return EnvironmentSequence.End;
			}
			int num = envEndBlockIndex - nextEnvBlockIndex + 1;
			int sequenceLengthInBlocks = environmentInfo.GetSequenceLengthInBlocks(EnvironmentSequence.End);
			if (num < sequenceLengthInBlocks)
			{
				SDebug.LogError(string.Concat("RouteChunk: not enough blocks left (", num, ") for end piece of ", environmentInfo.Kind, " with block num ", sequenceLengthInBlocks));
				return EnvironmentSequence.End;
			}
			if (num == sequenceLengthInBlocks)
			{
				return EnvironmentSequence.End;
			}
			return EnvironmentSequence.Mid;
		}

		private int CreateEnvironmentSegment(int blockIndex, EnvironmentInfo environmentInfo, EnvironmentSequence sequence, ref int prevSegmentVariation)
		{
			int num = blockIndex * 90;
			if (!environmentInfo.HasStartEnd && (sequence == EnvironmentSequence.Start || sequence == EnvironmentSequence.End))
			{
				sequence = EnvironmentSequence.Mid;
			}
			if (_logicAssetManager.HasEnvironmentSegmentLogic(environmentInfo.Kind.Type))
			{
				CreateEnvironmentLogic(environmentInfo.Kind.Type, sequence, num);
			}
			prevSegmentVariation = CreateEnvironmentModel(environmentInfo, sequence, prevSegmentVariation, num);
			return blockIndex + environmentInfo.GetSequenceLengthInBlocks(sequence);
		}

		private void CreateEnvironmentLogic(EnvironmentType envType, EnvironmentSequence sequence, float positionZ)
		{
			Transform transform = _logicAssetManager.SpawnEnvironmentSegmentLogic(envType, sequence);
			if (!(transform == null))
			{
				PlaceholderPrefab component = transform.GetComponent<PlaceholderPrefab>();
				if (component != null)
				{
					_environmentSelectors.AddRange(component.Selectors);
					_environmentTrackObjects.AddRange(component.TrackObjects);
				}
				else
				{
					_environmentSelectors.AddRange(transform.GetComponentsInChildren<Selector>(true));
					_environmentTrackObjects.AddRange(transform.GetComponentsInChildren<TrackObject>(true));
				}
				transform.parent = _environmentTransform;
				transform.localPosition = Vector3.forward * positionZ;
			}
		}

		private int CreateEnvironmentModel(EnvironmentInfo environmentInfo, EnvironmentSequence sequence, int prevVariation, float positionZ)
		{
			int num = prevVariation;
			int num2 = 1;
			bool flag = prevVariation == 0 || !environmentInfo.UseSameVariationPerSequence;
			if (flag || !environmentInfo.UseSameVariationForSides)
			{
				num2 = _themeManager.GetEnvironmentModelVariationCount(RouteCategory.Default, environmentInfo.Kind);
			}
			if (environmentInfo.HasSides)
			{
				int variation = num;
				int variation2 = num;
				num = 0;
				if (!environmentInfo.UseSameVariationForSides)
				{
					variation = UnityEngine.Random.Range(1, num2 + 1);
					variation2 = UnityEngine.Random.Range(1, num2 + 1);
				}
				PlaceEnvironmentModel(environmentInfo.Kind, sequence, EnvironmentSide.Left, variation, positionZ);
				PlaceEnvironmentModel(environmentInfo.Kind, sequence, EnvironmentSide.Right, variation2, positionZ);
			}
			else
			{
				if (flag)
				{
					num = UnityEngine.Random.Range(1, num2 + 1);
				}
				PlaceEnvironmentModel(environmentInfo.Kind, sequence, EnvironmentSide.None, num, positionZ);
			}
			return num;
		}

		private void PlaceEnvironmentModel(EnvironmentKind envTypeInfo, EnvironmentSequence sequence, EnvironmentSide side, int variation, float positionZ)
		{
			Transform environmentSegmentModel = _themeManager.GetEnvironmentSegmentModel(RouteCategory.Default, envTypeInfo, sequence, side, variation);
			if (!(environmentSegmentModel == null))
			{
				Transform transform = PoolManager.Spawn(environmentSegmentModel);
				PlaceholderPrefab component = transform.GetComponent<PlaceholderPrefab>();
				if (component != null)
				{
					_environmentSelectors.AddRange(component.Selectors);
					_environmentTrackObjects.AddRange(component.TrackObjects);
				}
				else
				{
					_environmentSelectors.AddRange(transform.GetComponentsInChildren<Selector>(true));
					_environmentTrackObjects.AddRange(transform.GetComponentsInChildren<TrackObject>(true));
				}
				transform.parent = _environmentTransform;
				transform.localPosition = Vector3.forward * positionZ;
			}
		}

		private void EvaluateEnvironmentConstraints()
		{
			if (_blockConstraints == null)
			{
				_blockConstraints = new List<ChunkBlockConstraint>(_blockCount);
			}
			_blockConstraints.Clear();
			for (int i = 0; i < _blockCount; i++)
			{
				EnvironmentKind envKind = EnvironmentKind.None;
				EnvironmentBaseKind envBaseKind = EnvironmentBaseKind.None;
				SYBO.Subway.Level.Environment environmentConstraint = GetEnvironmentConstraint(i);
				EnvironmentBase environmentBaseConstraint = GetEnvironmentBaseConstraint(i);
				if (environmentConstraint != null)
				{
					envKind = environmentConstraint.EnvironmentKind;
				}
				if (environmentBaseConstraint != null)
				{
					envBaseKind = EnvironentBaseToBaseKind(environmentBaseConstraint);
				}
				_blockConstraints.Add(new ChunkBlockConstraint(envKind, envBaseKind));
			}
		}

		private SYBO.Subway.Level.Environment GetEnvironmentConstraint(int blockIndex)
		{
			if (_forcedEnvironments.Length == 0)
			{
				return null;
			}
			SYBO.Subway.Level.Environment[] forcedEnvironments = _forcedEnvironments;
			foreach (SYBO.Subway.Level.Environment environment in forcedEnvironments)
			{
				Vector3 localPos = environment.transform.position - _transform.position;
				int blockIndexFromLocalPosition = GetBlockIndexFromLocalPosition(localPos);
				int num = blockIndexFromLocalPosition + environment.BlockCount - 1;
				if (blockIndexFromLocalPosition <= blockIndex && blockIndex <= num)
				{
					return environment;
				}
			}
			return null;
		}

		private EnvironmentBase GetEnvironmentBaseConstraint(int blockIndex)
		{
			if (_forcedEnvironmentBases.Length == 0)
			{
				return null;
			}
			EnvironmentBase[] forcedEnvironmentBases = _forcedEnvironmentBases;
			foreach (EnvironmentBase environmentBase in forcedEnvironmentBases)
			{
				Vector3 localPos = environmentBase.transform.position - _transform.position;
				int blockIndexFromLocalPosition = GetBlockIndexFromLocalPosition(localPos);
				int num = blockIndexFromLocalPosition + environmentBase.BlockCount - 1;
				if (blockIndexFromLocalPosition <= blockIndex && blockIndex <= num)
				{
					return environmentBase;
				}
			}
			return null;
		}

		public static EnvironmentBaseKind EnvironentBaseToBaseKind(EnvironmentBase envBase)
		{
			if (envBase.Type == EnvironmentBase.EnvironmentBaseType.Tracks)
			{
				switch (envBase.NumTracks)
				{
				case 0:
					return EnvironmentBaseKind.Tracks0;
				case 1:
					return EnvironmentBaseKind.Tracks1;
				case 2:
					return EnvironmentBaseKind.Tracks2;
				case 3:
					return EnvironmentBaseKind.Tracks3;
				default:
					return EnvironmentBaseKind.None;
				}
			}
			return EnvironmentBaseKind.Empty;
		}

		public static int GetBlockIndexFromLocalPosition(Vector3 localPos)
		{
			int num = Mathf.Clamp((int)localPos.z, 0, int.MaxValue);
			return num / 90;
		}

		public static float GetBaseColliderOffsetY()
		{
			return 0.6000004f;
		}

		public static bool HasParentRouteChunk(Transform childTransform)
		{
			return GetParentRouteChunk(childTransform) != null;
		}

		public static RouteChunk GetParentRouteChunk(Transform childTransform)
		{
			return childTransform.GetComponentInParent<RouteChunk>();
		}
	}
}
