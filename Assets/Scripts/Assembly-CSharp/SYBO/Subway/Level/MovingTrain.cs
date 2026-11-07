using System.Collections.Generic;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(TrackObject))]
	public class MovingTrain : MonoBehaviour
	{
		private Transform _characterControllerTransform;

		[SerializeField]
		private float speed = 1f;

		[SerializeField]
		private float trainCount = 3f;

		[SerializeField]
		private static float autoPilotActivationDistance = 200f;

		[SerializeField]
		private AudioClip trianPassClip;

		private static MovingTrainUpdater _updater = null;

		private static HashSet<MovingTrain> _activeTrains = new HashSet<MovingTrain>();

		private Transform _transform;

		private Transform _train;

		private BoxCollider _trainCollider;

		private Vector3 _trainColliderInitialSize;

		private TrackObject _trackObject;

		private bool _active;

		private bool _autoPilot;

		private bool _initalized;

		private float _distanceToCameraZ;

		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				if (speed != value)
				{
					speed = value;
					if (Application.isPlaying)
					{
						CheckInitialize();
						UpdateTrainCollider();
					}
				}
			}
		}

		public bool Active
		{
			get
			{
				return _active;
			}
		}

		public float DistanceToCameraZ
		{
			get
			{
				return _distanceToCameraZ;
			}
		}

		public static int GetActiveTrainCount()
		{
			return _activeTrains.Count;
		}

		private void Awake()
		{
			CheckInitialize();
		}

		private void CheckInitialize()
		{
			if (!_initalized && Game.IsInstantiated)
			{
				if (_updater == null)
				{
					GameObject gameObject = new GameObject("MovingTrainUpdater");
					_updater = gameObject.AddComponent<MovingTrainUpdater>();
					_updater.SetAudioClip(trianPassClip);
				}
				_transform = base.transform;
				if (_transform.childCount == 0)
				{
				}
				_train = _transform.GetChild(0);
				_train.localPosition = -Vector3.up * 200f;
				_trainCollider = GetComponent<BoxCollider>();
				_trainColliderInitialSize = _trainCollider.size;
				_active = false;
				_trackObject = GetComponent<TrackObject>();
				_trackObject.OnActivate += OnActivate;
				_trackObject.OnDeactivate += OnDeactivate;
				UpdateTrainCollider();
				_initalized = true;
			}
		}

		private void OnDestroy()
		{
			if (_trackObject != null)
			{
				_trackObject.OnActivate -= OnActivate;
				_trackObject.OnDeactivate -= OnDeactivate;
			}
		}

		private void UpdateTrainCollider()
		{
			_trainCollider.size = new Vector3(_trainColliderInitialSize.x, _trainColliderInitialSize.y, _trainColliderInitialSize.z / (1f + speed));
			_trainCollider.center = new Vector3(0f, 15.2f, (30f * trainCount + 1f) / (1f + speed));
		}

		private void OnActivate()
		{
			_trainCollider.enabled = true;
			_activeTrains.Add(this);
			_updater.AddMovingTrain(this);
			_active = true;
			_autoPilot = false;
			_train.localPosition = new Vector3(0f, 0f, (_transform.position.z - Character.Instance.CharacterController.transform.position.z) * speed);
		}

		public void UpdateMovingTrain(float gameSpeed, float characterPositionZ, float cameraPositionZ)
		{
			_distanceToCameraZ = Mathf.Abs(_transform.position.z - cameraPositionZ);
			if (_autoPilot)
			{
				_train.position -= Vector3.forward * Time.deltaTime * gameSpeed * speed;
				return;
			}
			float num = _transform.position.z - characterPositionZ;
			_train.localPosition = new Vector3(0f, 0f, num * speed);
		}

		public Vector3 GetTrainModelPosition()
		{
			return _train.position;
		}

		private void OnDeactivate()
		{
			_activeTrains.Remove(this);
			_updater.RemoveMovingTrain(this);
			_active = false;
			_train.localPosition = -100f * Vector3.up;
		}

		public override int GetHashCode()
		{
			return GetInstanceID() * 29;
		}

		public override bool Equals(object o)
		{
			return object.ReferenceEquals(this, o);
		}

		public static void ActivateAutoPilot()
		{
			foreach (MovingTrain activeTrain in _activeTrains)
			{
				if (activeTrain == null)
				{
					SDebug.LogErrorFormat("Found NULL train in active trains list");
					continue;
				}
				CharacterController characterController = Character.Instance.CharacterController;
				if (characterController == null)
				{
					SDebug.LogErrorFormat("Reference to character controller NULL");
				}
				else if (activeTrain._trainCollider.bounds.min.z - characterController.transform.position.z < autoPilotActivationDistance)
				{
					activeTrain._trainCollider.enabled = false;
					activeTrain._autoPilot = true;
				}
			}
		}
	}
}
