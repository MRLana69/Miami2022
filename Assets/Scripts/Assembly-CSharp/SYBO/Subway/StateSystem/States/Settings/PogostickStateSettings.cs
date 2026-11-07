using System;
using Info;
using SYBO.Subway.Pickups;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States.Settings
{
	public class PogostickStateSettings : ScriptableSingleton<PogostickStateSettings>
	{
		[Serializable]
		public class PickupWeight
		{
			public UpgradeType UpgradeType;

			public PickupType PickupType;
		}

		[SerializeField]
		private float _jumpHeight = 95f;

		[SerializeField]
		private float _jumpDistance = 800f;

		[SerializeField]
		private float _characterChangeTrackLength = 60f;

		[SerializeField]
		private float _finalJumpSpeed;

		[SerializeField]
		private int _rows = 5;

		[SerializeField]
		private int _startRowPosition = 1;

		[SerializeField]
		private int _endRowPosition = 1;

		[SerializeField]
		private PickupWeight[] _pickupList;

		[SerializeField]
		private AnimationCurve _jumpCurve;

		[SerializeField]
		private AnimationCurve _cameraCurve;

		[SerializeField]
		private AnimationCurve _cameraAimFollowCurve;

		[SerializeField]
		private float _fadeInPosition = 0.1f;

		[SerializeField]
		private float _hangtimePosition = 0.5f;

		[SerializeField]
		private float _smoothCameraXDuration = 0.05f;

		[SerializeField]
		private Vector3 _cameraAimOffset = Vector3.zero;

		public float JumpHeight
		{
			get
			{
				return _jumpHeight;
			}
		}

		public float JumpDistance
		{
			get
			{
				return _jumpDistance;
			}
		}

		public float CharacterChangeTrackLength
		{
			get
			{
				return _characterChangeTrackLength;
			}
		}

		public float FinalJumpSpeed
		{
			get
			{
				return _finalJumpSpeed;
			}
		}

		public int Rows
		{
			get
			{
				return _rows;
			}
		}

		public int StartRowPosition
		{
			get
			{
				return _startRowPosition;
			}
		}

		public int EndRowPosition
		{
			get
			{
				return _endRowPosition;
			}
		}

		public PickupWeight[] PickupList
		{
			get
			{
				return _pickupList;
			}
		}

		public AnimationCurve JumpCurve
		{
			get
			{
				return _jumpCurve;
			}
		}

		public AnimationCurve CameraCurve
		{
			get
			{
				return _cameraCurve;
			}
		}

		public AnimationCurve CameraAimFollowCurve
		{
			get
			{
				return _cameraAimFollowCurve;
			}
		}

		public float FadeInPosition
		{
			get
			{
				return _fadeInPosition;
			}
		}

		public float HangtimePosition
		{
			get
			{
				return _hangtimePosition;
			}
		}

		public float SmoothCameraXDuration
		{
			get
			{
				return _smoothCameraXDuration;
			}
		}

		public Vector3 CameraAimOffset
		{
			get
			{
				return _cameraAimOffset;
			}
		}
	}
}
