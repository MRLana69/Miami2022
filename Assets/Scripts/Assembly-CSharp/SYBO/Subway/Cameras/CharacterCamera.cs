using System.Collections;
using SYBO.Common.Data;
using SYBO.Common.Exceptions;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Cameras
{
	[RequireComponent(typeof(MainCamera))]
	public class CharacterCamera : MonoBehaviour
	{
		public enum CameraState
		{
			Menu = 0,
			DefaultRunning = 1,
			Custom = 2
		}

		public const float DefaultCharacterLookAtAngle = 21.5f;

		public const float DefaultAspectRatio = 0.5625f;

		private const float _HIGH_SPEED_1_FOV = 5f;

		private const float _HIGH_SPEED_1_FOV_SMOOTH_DURATION = 1f;

		private const float _HIGH_SPEED_2_FOV = 10f;

		private const float _HIGH_SPEED_2_FOV_SMOOTH_DURATION = 1f;

		private const float _SMOOTH_DAMP_THRESHHOLD = 0.5f;

		private const float MenuFieldOfView = 71f;

		public const float DefaultFieldOfView = 68f;

		private const float LOW_ASPECT_RATIO = 1.5f;

		private const float HIGH_ASPECT_RATIO = 2.2f;

		private const float FRONTSCREEN_FOV_LOW_ASPECT_RATIO = 71f;

		private const float FRONTSCREEN_FOV_HIGH_ASPECT_RATIO = 80f;

		[SerializeField]
		private float tunnelDelta = -20f;

		[SerializeField]
		private MainCamera _mainCamera;

		private Vector3 _position;

		private Vector3 _target;

		public float _speed;

		private float _previousCameraEndYPosition;

		private float _previousCameraEndXRotation;

		private CameraData _currentCameraData;

		[SerializeField]
		private CameraData _defaultCameraData;

		private CameraSpeedType _cameraSpeedType;

		private float _customhighSpeedFOV;

		private CameraState _currentCameraState;

		private float _lastZ;

		private Vector3 _shake = Vector3.zero;

		private Vector3 _diff = Vector3.zero;

		private bool _fovGipping;

		private Camera _camera;

		private SmoothDampVector3B _cameraTarget;

		private Vector3 _lastCameraTransformPosition;

		private Vector3 _lastCameraTarget;

		private SmoothDampVector3B _cameraPosition;

		private Character _character;

		private SmoothDampFloat _fieldOfViewSmoothDampFloat;

		private bool _hasSuperSneakers;

		private float _cameraModifierColliderStartGamePositionZ;

		private Curve offsetDeltaCurve;

		private float _cameraModifierColliderEndGamePositionZ;

		private float _cameraModifierY = 0.5f;

		private float _cameraModifierX = 0.75f;

		private Vector3 _previousCameraEndPosition;

		private Vector3 _previousTargetEndPosition;

		private float _tweenTimeTransform;

		private bool _initialized;

		private static CharacterCamera instance;

		private float _lastDeltaYFactor;

		public Vector3 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public Vector3 Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}

		public float Speed
		{
			get
			{
				return _speed;
			}
		}

		public CameraData CameraData
		{
			get
			{
				return _currentCameraData;
			}
			private set
			{
				_currentCameraData = value;
				_cameraTarget = new SmoothDampVector3B(_target, _currentCameraData.CameraTargetOffsetSmoothTime);
				_cameraPosition = new SmoothDampVector3B(_position, _currentCameraData.CameraPositionOffsetSmoothTime);
				_fieldOfViewSmoothDampFloat.Target = _currentCameraData.FieldOfView.Target;
				_fieldOfViewSmoothDampFloat.SmoothTime = _currentCameraData.FieldOfView.SmoothTime;
				_previousCameraEndYPosition = base.transform.position.y;
				_previousCameraEndXRotation = base.transform.rotation.eulerAngles.x;
				_tweenTimeTransform = 0f;
			}
		}

		public CameraSpeedType CameraSpeedType
		{
			get
			{
				return _cameraSpeedType;
			}
			private set
			{
				_cameraSpeedType = value;
				switch (_cameraSpeedType)
				{
				case CameraSpeedType.Normal:
					if (_currentCameraData != null)
					{
						_fieldOfViewSmoothDampFloat = _currentCameraData.FieldOfView;
					}
					else
					{
						SDebug.LogError("We are missing a _cameraData when _cameraSpeedType: " + _cameraSpeedType);
					}
					break;
				case CameraSpeedType.HighSpeed1:
					_fieldOfViewSmoothDampFloat.Target = 5f;
					_fieldOfViewSmoothDampFloat.SmoothTime = 1f;
					break;
				case CameraSpeedType.HighSpeed2:
					_fieldOfViewSmoothDampFloat.Target = 10f;
					_fieldOfViewSmoothDampFloat.SmoothTime = 1f;
					break;
				case CameraSpeedType.CustomSpeed:
					_fieldOfViewSmoothDampFloat.Target = _customhighSpeedFOV;
					_fieldOfViewSmoothDampFloat.SmoothTime = 1f;
					break;
				default:
					SwitchDefaultException.Throw(_cameraSpeedType.ToString());
					break;
				}
			}
		}

		public Camera Camera
		{
			get
			{
				return _camera;
			}
		}

		public static CharacterCamera Instance
		{
			get
			{
				return instance ?? (instance = Object.FindObjectOfType(typeof(CharacterCamera)) as CharacterCamera).Initialize();
			}
		}

		public Vector3 CameraPositionValue
		{
			get
			{
				return _cameraPosition.Value;
			}
		}

		public MainCamera MainCamera
		{
			get
			{
				return _mainCamera;
			}
		}

		public void SetCameraDataForcefully(CameraData cameraData)
		{
			SetCameraDataForcefully(cameraData, _camera.transform.localPosition, _camera.transform.localRotation);
		}

		public void SetCameraDataForcefully(CameraData cameraData, Vector3 cameraLocalPosition, Quaternion cameraLocalRotation)
		{
			CameraData = cameraData;
			_camera.transform.localPosition = cameraLocalPosition;
			_camera.transform.localRotation = cameraLocalRotation;
			_camera.fieldOfView = cameraData.FieldOfView.Target;
			if (_character == null)
			{
				_character = Character.Instance;
			}
			_cameraTarget = new SmoothDampVector3B(_character.TransformPosition + _currentCameraData.CameraTargetOffset, _currentCameraData.CameraTargetOffsetSmoothTime);
			_cameraPosition = new SmoothDampVector3B(_character.TransformPosition + _currentCameraData.CameraPositionOffset, _currentCameraData.CameraPositionOffsetSmoothTime);
		}

		public void SetCameraState(CameraState state)
		{
			if (state == CameraState.Menu)
			{
				SetCameraDataForcefully(GetDefaultCameraDataForMenu());
			}
			else
			{
				CameraData = _defaultCameraData;
				if (_currentCameraState == CameraState.Menu)
				{
					_cameraTarget = new SmoothDampVector3B(_currentCameraData.CameraTargetOffset + _character.TransformPosition, _currentCameraData.CameraTargetOffsetSmoothTime);
					_cameraPosition = new SmoothDampVector3B(_currentCameraData.CameraPositionOffset + _character.TransformPosition, _currentCameraData.CameraPositionOffsetSmoothTime);
				}
				else
				{
					_cameraTarget.SmoothTime = _currentCameraData.CameraTargetOffsetSmoothTime;
					_cameraPosition.SmoothTime = _currentCameraData.CameraPositionOffsetSmoothTime;
				}
			}
			_currentCameraState = state;
		}

		protected void Awake()
		{
			Initialize();
		}

		private CharacterCamera Initialize()
		{
			if (_initialized)
			{
				return this;
			}
			Game.OnCharacterModifierCollectionChanged += CharacterModifierCollection_OnChanged;
			_lastZ = base.transform.position.z;
			_camera = GetComponent<Camera>();
			_mainCamera = GetComponent<MainCamera>();
			if (instance == null)
			{
				instance = this;
			}
			_fieldOfViewSmoothDampFloat = new SmoothDampFloat(CameraData.FIELD_OF_VIEW_DEFAULT, CameraData.FIELD_OF_VIEW_SMOOTH_TIME_DEFAULT);
			_cameraTarget = new SmoothDampVector3B(CameraData.CAMERA_POSITION_OFFSET_DEFAULT, CameraData.CAMERA_POSITION_OFFSET_SMOOTH_TIME_DEFAULT);
			_cameraPosition = new SmoothDampVector3B(CameraData.CAMERA_TARGET_OFFSET_DEFAULT, CameraData.CAMERA_TARGET_OFFSET_SMOOTH_TIME_DEFAULT);
			_initialized = true;
			return this;
		}

		protected void Start()
		{
			_character = Game.Instance.Character;
			_character.OnStumble += Character_OnStumble;
		}

		private void Character_OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName, bool safeStumble)
		{
			Shake();
		}

		protected void LateUpdate()
		{
			if (_currentCameraState == CameraState.Custom)
			{
				base.transform.position = _position;
				base.transform.LookAt(_target);
				_cameraTarget.Value = _target;
				_cameraTarget.Update();
				_cameraPosition.Target = _position;
				_cameraPosition.Update();
				_speed = (_position.z - _lastZ) / Time.deltaTime;
				if (float.IsNaN(Speed))
				{
					_speed = 0f;
				}
				_lastZ = _position.z;
			}
			else
			{
				if (_currentCameraData == null || _currentCameraData.CameraDataMode == CameraDataMode.CameraUpdateDisabled)
				{
					return;
				}
				if (_currentCameraData.CameraDataMode == CameraDataMode.CameraUpdateEnabled_UseMyCameraData)
				{
					Vector3 zero = Vector3.zero;
					zero = ((offsetDeltaCurve == null || !(_character.GamePosition.z < _cameraModifierColliderEndGamePositionZ)) ? Vector3.zero : offsetDeltaCurve.Evaluate(_character.GamePosition.z - _cameraModifierColliderStartGamePositionZ));
					Vector3 transformPosition = _character.TransformPosition;
					Vector3 target = default(Vector3);
					target.x = (transformPosition.x + _currentCameraData.CameraTargetOffset.x) * _cameraModifierX;
					if (_currentCameraData.DontMoveInY)
					{
						target.y = _cameraTarget.TargetY;
					}
					else
					{
						float num = 0f;
						num = ((!_hasSuperSneakers && !_character.IsInLipFlight && (!_character.Hoverboard.isActive || !_character.Hoverboard.IsJumper) && !_character.IsDoubleJumping) ? 0f : ((!_character.IsDoubleJumping || !_character.IsJumpingHigher) ? 0.5f : 0.8f));
						if (!_character.IsGrounded.Value)
						{
							num = Mathf.Lerp(_lastDeltaYFactor, num, Time.deltaTime * 2f);
						}
						if (transformPosition.y < _character.LastGroundedY)
						{
							target.y = transformPosition.y;
						}
						else
						{
							target.y = _character.LastGroundedY;
						}
						float num2 = Mathf.Max(transformPosition.y - _character.LastGroundedY, 0f);
						target.y = target.y + num * num2 + _currentCameraData.CameraTargetOffset.y;
						_lastDeltaYFactor = num;
					}
					target.z = transformPosition.z + _currentCameraData.CameraTargetOffset.z;
					_cameraTarget.Target = target;
					_cameraTarget.Update();
					target = _cameraTarget.Value;
					target.y = Mathf.Clamp(target.y, transformPosition.y - 5f, transformPosition.y + 25f);
					_cameraTarget.Value = target;
					_target = target + Vector3.up * zero.y * _cameraModifierY;
					Vector3 target2 = default(Vector3);
					target2.x = ((_currentCameraData.CameraPositionOffset.x != 0f) ? (transformPosition.x + _currentCameraData.CameraPositionOffset.x) : _target.x);
					target2.y = _target.y + _currentCameraData.CameraPositionOffset.y - _currentCameraData.CameraTargetOffset.y;
					target2.z = transformPosition.z + _currentCameraData.CameraPositionOffset.z;
					_cameraPosition.Target = target2;
					_cameraPosition.Update();
					_position = new Vector3(_cameraPosition.Value.x, _cameraPosition.Value.y + zero.y, _cameraPosition.Value.z);
					if (_character.IsInsideSubway)
					{
						float subwayMaxY = _character.SubwayMaxY;
						_position.y = Mathf.Min(subwayMaxY, _position.y);
						_target = _position - _currentCameraData.CameraPositionOffset + _currentCameraData.CameraTargetOffset;
					}
					base.transform.position = _position;
					base.transform.LookAt(_target);
					if (_tweenTimeTransform / _currentCameraData.TransformTweenDuration < 1f)
					{
						Vector3 eulerAngles = base.transform.rotation.eulerAngles;
						Vector3 position = base.transform.position;
						position.y = Mathf.SmoothStep(_previousCameraEndYPosition, _position.y, _tweenTimeTransform / _currentCameraData.TransformTweenDuration);
						eulerAngles.x = Mathf.LerpAngle(_previousCameraEndXRotation, base.transform.rotation.eulerAngles.x, _tweenTimeTransform / _currentCameraData.TransformTweenDuration);
						base.transform.position = position;
						base.transform.rotation = Quaternion.Euler(eulerAngles);
						_tweenTimeTransform += Time.deltaTime;
					}
					base.transform.position = base.transform.position + _shake;
					if (Mathf.Abs(_camera.fieldOfView - _fieldOfViewSmoothDampFloat.Target) > 0.5f)
					{
						_fieldOfViewSmoothDampFloat.Update();
					}
					_speed = (_position.z - _lastZ) / Time.deltaTime;
					if (float.IsNaN(Speed))
					{
						_speed = 0f;
					}
					_lastZ = _position.z;
				}
				else if (CameraData.CameraDataMode == CameraDataMode.CameraUpdateEnabled_DoNotUseMyCameraData)
				{
					base.transform.position = _position;
					base.transform.LookAt(_target);
					_cameraTarget.Value = _target;
					_cameraTarget.Update();
					_cameraPosition.Target = _position;
					_cameraPosition.Update();
					_speed = (_position.z - _lastZ) / Time.deltaTime;
					if (float.IsNaN(Speed))
					{
						_speed = 0f;
					}
					_lastZ = _position.z;
				}
			}
		}

		private void DebugLogCameraDetails()
		{
			if (base.transform.position != _lastCameraTransformPosition || _target != _lastCameraTarget)
			{
				_lastCameraTransformPosition = base.transform.position;
				_lastCameraTarget = _target;
			}
		}

		protected void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(_position, _target);
		}

		public void Shake()
		{
			_diff = Vector3.zero;
			float amplitude = 100f;
			StartCoroutine(pTween.To(0.3f, (float t) =>
			{
				_diff += Random.insideUnitSphere;
				_shake = (1f - t) * _diff * amplitude * Time.deltaTime;
			}));
		}

		public void ResetShake()
		{
			_shake = Vector3.zero;
		}

		public void Shake(float intensity)
		{
			_shake = Random.insideUnitSphere * intensity * Time.deltaTime;
		}

		public void FOVGip(float time, float multiplier)
		{
			if (_fovGipping)
			{
				return;
			}
			_fovGipping = true;
			float startFOV = _camera.fieldOfView;
			StartCoroutine(pTween.To(time, (float t) =>
			{
				if ((double)t < 0.5)
				{
					_camera.fieldOfView = Mathf.Lerp(startFOV, startFOV * multiplier, t * 2f);
				}
				else
				{
					_camera.fieldOfView = Mathf.Lerp(startFOV * multiplier, startFOV, t * 2f - 1f);
				}
				if (t == 1f)
				{
					_fovGipping = false;
				}
			}));
		}

		public void SetFOV(float addedFOV)
		{
			_customhighSpeedFOV = addedFOV;
			StopCoroutine("FOVTarget_Coroutine");
			StartCoroutine("FOVTarget_Coroutine");
		}

		private IEnumerator FOVTarget_Coroutine()
		{
			while (true)
			{
				yield return null;
			}
		}

		public void RevertFOV()
		{
			StopCoroutine("FOVTarget_Coroutine");
		}

		public void EnterCameraModifierCollider(float colliderLength)
		{
			_cameraModifierColliderStartGamePositionZ = _character.GamePosition.z;
			_cameraModifierColliderEndGamePositionZ = _cameraModifierColliderStartGamePositionZ + colliderLength;
			offsetDeltaCurve = new Curve();
			offsetDeltaCurve.AddKey(0f, Vector3.zero, -Vector3.up, -Vector3.up);
			offsetDeltaCurve.AddKey(colliderLength / 2f, Vector3.up * tunnelDelta);
			offsetDeltaCurve.AddKey(colliderLength, Vector3.zero, Vector3.up * 0.001f, Vector3.up * 0.001f);
		}

		public void ExitCameraModifierCollider()
		{
			offsetDeltaCurve = null;
		}

		public void CharacterModifierCollection_OnChanged(object sender, CharacterModifierCollectionChangedEventArgs characterModifierCollectionChangedEventArgs)
		{
			CharacterModifierCollectionChangeType characterModifierCollectionChangeType = characterModifierCollectionChangedEventArgs.CharacterModifierCollectionChangeType;
			CharacterModifier characterModifier = characterModifierCollectionChangedEventArgs.CharacterModifier;
			switch (characterModifierCollectionChangeType)
			{
			case CharacterModifierCollectionChangeType.Added:
				if (characterModifier.GetType() == typeof(SuperSneakers))
				{
					_hasSuperSneakers = true;
					_previousCameraEndYPosition = base.transform.position.y;
					_previousCameraEndXRotation = base.transform.rotation.eulerAngles.x;
					_tweenTimeTransform = 0f;
				}
				break;
			case CharacterModifierCollectionChangeType.Removed:
				if (characterModifier.GetType() == typeof(SuperSneakers))
				{
					_hasSuperSneakers = false;
					_previousCameraEndYPosition = base.transform.position.y;
					_previousCameraEndXRotation = base.transform.rotation.eulerAngles.x;
					_tweenTimeTransform = 0f;
				}
				break;
			}
		}

		public CameraData GetDefaultCameraDataForMenu()
		{
			CameraData defaultCameraData = GetDefaultCameraData();
			defaultCameraData.CameraDataMode = CameraDataMode.CameraUpdateDisabled;
			defaultCameraData.FieldOfView.Target = GetFOVForAspectRatio(Utils.GetAspectRatio());
			defaultCameraData.FieldOfView.SmoothTime = 1f;
			return defaultCameraData;
		}

		private float GetFOVForAspectRatio(float aspectRatio)
		{
			float t = Mathf.Abs(aspectRatio - 1.5f) / 0.70000005f;
			return Mathf.Lerp(71f, 80f, t);
		}

		public CameraData GetDefaultCameraData()
		{
			return new CameraData();
		}
	}
}
