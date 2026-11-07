using UnityEngine;

namespace SYBO.Subway.StateSystem.States.Settings
{
	public class FlyingStateSettings : ScriptableSingleton<FlyingStateSettings>
	{
		[SerializeField]
		private float _characterChangeTrackLength = 60f;

		[SerializeField]
		private float _flyHeight = 95f;

		[SerializeField]
		private float _flyAheadDuration = 1.5f;

		[SerializeField]
		private float _stopBeforeLandingChunkDistance = 50f;

		[SerializeField]
		private float _smoothCameraXDuration = 0.05f;

		[SerializeField]
		private float _smoothYDuration = 0.05f;

		[SerializeField]
		private float _cameraAimOffsetY = 15f;

		[SerializeField]
		private AnimationCurve _flyAheadCameraMovement;

		public float CharacterChangeTrackLength
		{
			get
			{
				return _characterChangeTrackLength;
			}
		}

		public float FlyHeight
		{
			get
			{
				return _flyHeight;
			}
		}

		public float FlyAheadDuration
		{
			get
			{
				return _flyAheadDuration;
			}
		}

		public float StopBeforeLandingChunkDistance
		{
			get
			{
				return _stopBeforeLandingChunkDistance;
			}
		}

		public float SmoothCameraXDuration
		{
			get
			{
				return _smoothCameraXDuration;
			}
		}

		public float SmoothYDuration
		{
			get
			{
				return _smoothYDuration;
			}
		}

		public float CameraAimOffsetY
		{
			get
			{
				return _cameraAimOffsetY;
			}
		}

		public AnimationCurve FlyAheadCameraMovement
		{
			get
			{
				return _flyAheadCameraMovement;
			}
		}
	}
}
