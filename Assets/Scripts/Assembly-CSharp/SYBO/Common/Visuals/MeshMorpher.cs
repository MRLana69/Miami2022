using UnityEngine;

namespace SYBO.Common.Visuals
{
	public class MeshMorpher : MonoBehaviour
	{
		private enum AnimationType
		{
			Auto = 0,
			Manual = 1
		}

		[SerializeField]
		private MeshFilter _targetMeshFilter;

		[SerializeField]
		private Mesh[] _morphTargets;

		[SerializeField]
		private float _loopDuration = 1f;

		[SerializeField]
		private WrapMode _wrapMode;

		[SerializeField]
		private bool _animate;

		[SerializeField]
		private AnimationType _animationType;

		[SerializeField]
		private float _animTime;

		[SerializeField]
		private DeviceInfo.PerformanceLevel _minPerformanceLevel;

		private Mesh _mesh;

		private Vector3[] _vertices;

		private Vector3[][] _morphTargetsVertices;

		public bool Animate
		{
			get
			{
				return _animate;
			}
			set
			{
				_animate = value;
			}
		}

		public void Reset()
		{
			if (Application.isPlaying)
			{
				_animTime = 0f;
				SetMorph(0f);
			}
		}

		private void BuildMesh(int idx1, int idx2, float t)
		{
			if (_morphTargetsVertices == null)
			{
				Initialize();
			}
			Vector3[] array = _morphTargetsVertices[idx1];
			Vector3[] array2 = _morphTargetsVertices[idx2];
			for (int i = 0; i < _vertices.Length; i++)
			{
				_vertices[i] = Vector3.Lerp(array[i], array2[i], t);
			}
			_mesh.vertices = _vertices;
		}

		private void SetMorph(float t)
		{
			int num = Mathf.Clamp(Mathf.FloorToInt(t), 0, _morphTargets.Length - 2);
			float t2 = Mathf.Clamp(t - (float)num, 0f, 1f);
			BuildMesh(num, num + 1, t2);
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			if (_morphTargets != null)
			{
				_morphTargetsVertices = new Vector3[_morphTargets.Length][];
				for (int i = 0; i < _morphTargets.Length; i++)
				{
					_morphTargetsVertices[i] = _morphTargets[i].vertices;
				}
				_vertices = new Vector3[_morphTargetsVertices[0].Length];
				for (int j = 0; j < _vertices.Length; j++)
				{
					_vertices[j] = _morphTargetsVertices[0][j];
				}
				_mesh = new Mesh();
				_mesh.MarkDynamic();
				_mesh.vertices = _vertices;
				_mesh.uv = _morphTargets[0].uv.Clone() as Vector2[];
				_mesh.triangles = _morphTargets[0].triangles.Clone() as int[];
			}
		}

		private void OnDestroy()
		{
			if (Application.isPlaying)
			{
				Object.Destroy(_mesh);
			}
			else
			{
				Object.DestroyImmediate(_mesh);
			}
		}

		private void OnEnable()
		{
			if (!(_targetMeshFilter == null))
			{
				_targetMeshFilter.sharedMesh = _mesh;
				if (_wrapMode == WrapMode.Once)
				{
					_animTime = 0f;
				}
			}
		}

		private void OnDisable()
		{
			if (_targetMeshFilter != null)
			{
				_targetMeshFilter.sharedMesh = null;
			}
		}

		private void Update()
		{
			if (_morphTargets != null && DeviceInfo.Instance.performanceLevel >= _minPerformanceLevel && _animate)
			{
				if (_animationType == AnimationType.Auto)
				{
					float num = Time.deltaTime * (float)(_morphTargets.Length - 1) / _loopDuration;
					_animTime += num;
					float morph = ((_wrapMode == WrapMode.Once) ? Mathf.Clamp(_animTime, 0f, _morphTargets.Length - 1) : ((_wrapMode == WrapMode.PingPong) ? Mathf.PingPong(_animTime, _morphTargets.Length - 1) : ((_wrapMode != WrapMode.Once) ? Mathf.Repeat(_animTime, _morphTargets.Length - 1) : Mathf.Min(_animTime, _morphTargets.Length - 1))));
					SetMorph(morph);
				}
				else
				{
					SetMorph(_animTime);
				}
			}
		}
	}
}
