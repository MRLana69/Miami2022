using System;
using UnityEngine;

namespace SYBO.Subway.Cameras
{
	[Serializable]
	public class CameraData
	{
		public static readonly CameraDataMode CAMERA_MODE_DEFAULT = CameraDataMode.CameraUpdateEnabled_IgnoreMe;

		public static readonly float FIELD_OF_VIEW_DEFAULT = 40f;

		public static readonly Vector3 CAMERA_TARGET_OFFSET_DEFAULT = new Vector3(0f, 20f, 0f);

		public static readonly Vector3 CAMERA_POSITION_OFFSET_DEFAULT = new Vector3(0f, 33f, -33f);

		public static readonly Vector3 CAMERA_TARGET_OFFSET_SMOOTH_TIME_DEFAULT = new Vector3(0.05f, 0.1f, 0f);

		public static readonly Vector3 CAMERA_POSITION_OFFSET_SMOOTH_TIME_DEFAULT = new Vector3(0f, 0f, 0f);

		public static readonly float TRANSFORM_TWEEN_DURATION = 0.5f;

		public static readonly float FIELD_OF_VIEW_SMOOTH_TIME_DEFAULT = 0.2f;

		[Tooltip("(Common)UseMe: Camera uses my CameraData.\n(Common)Ignore: Camera uses previous CameraData.\n(Rare)UpdateDisabled: State operates camera.")]
		[SerializeField]
		internal CameraDataMode _cameraDataMode;

		[SerializeField]
		internal bool _useLastGroundedY;

		[SerializeField]
		internal bool _dontMoveInY;

		[SerializeField]
		internal SmoothDampFloat _fieldOfView;

		[Tooltip("Camera will *LOOK AT* the character position plus this offset")]
		[SerializeField]
		internal Vector3 _cameraTargetOffset;

		[SerializeField]
		internal Vector3 _cameraTargetOffsetSmoothTime;

		[Tooltip("Camera position will *BE AT* the character position plus this offset")]
		[SerializeField]
		internal Vector3 _cameraPositionOffset;

		[SerializeField]
		internal Vector3 _cameraPositionOffsetSmoothTime;

		[SerializeField]
		private float _transformTweenDuration;

		public CameraDataMode CameraDataMode
		{
			get
			{
				return _cameraDataMode;
			}
			internal set
			{
				_cameraDataMode = value;
			}
		}

		public bool UseLastGroundedY
		{
			get
			{
				return _useLastGroundedY;
			}
		}

		public bool DontMoveInY
		{
			get
			{
				return _dontMoveInY;
			}
		}

		public SmoothDampFloat FieldOfView
		{
			get
			{
				return _fieldOfView;
			}
			set
			{
				_fieldOfView = value;
			}
		}

		public Vector3 CameraTargetOffset
		{
			get
			{
				return _cameraTargetOffset;
			}
		}

		public Vector3 CameraTargetOffsetSmoothTime
		{
			get
			{
				return _cameraTargetOffsetSmoothTime;
			}
		}

		public Vector3 CameraPositionOffset
		{
			get
			{
				return _cameraPositionOffset;
			}
			set
			{
				_cameraPositionOffset = value;
			}
		}

		public Vector3 CameraPositionOffsetSmoothTime
		{
			get
			{
				return _cameraPositionOffsetSmoothTime;
			}
		}

		public float TransformTweenDuration
		{
			get
			{
				return _transformTweenDuration;
			}
		}

		public CameraData()
		{
			CameraDataMode = CAMERA_MODE_DEFAULT;
			_fieldOfView = new SmoothDampFloat(FIELD_OF_VIEW_DEFAULT, FIELD_OF_VIEW_SMOOTH_TIME_DEFAULT);
			_cameraTargetOffset = CAMERA_TARGET_OFFSET_DEFAULT;
			_cameraTargetOffsetSmoothTime = CAMERA_TARGET_OFFSET_SMOOTH_TIME_DEFAULT;
			_cameraPositionOffset = CAMERA_POSITION_OFFSET_DEFAULT;
			_cameraPositionOffsetSmoothTime = CAMERA_POSITION_OFFSET_SMOOTH_TIME_DEFAULT;
			_transformTweenDuration = TRANSFORM_TWEEN_DURATION;
		}
	}
}
