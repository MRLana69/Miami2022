using UnityEngine;

namespace SYBO.Subway.Pickups
{
	public class PickupRotate : MonoBehaviour
	{
		[SerializeField]
		private Transform _target;

		[SerializeField]
		private float _speed = 180f;

		[SerializeField]
		private float _rotatePhase = 0.9f;

		private float _worldPosZ;

		private Vector3 _localEuler = Vector3.up;

		private bool _willRotate = true;

		public bool WillRotate
		{
			get
			{
				return _willRotate;
			}
		}

		public void Awake()
		{
			TrackObject component = base.gameObject.GetComponent<TrackObject>();
			if (component != null)
			{
				component.OnActivate += TrackObject_OnActivate;
				component.OnDeactivate += TrackObject_OnDeactivate;
			}
			_willRotate = false;
		}

		protected void OnDestroy()
		{
			StopRotating();
		}

		public void PhasedRotate(float time)
		{
			_localEuler.y = time * _speed + _worldPosZ * _rotatePhase;
			_target.localEulerAngles = _localEuler;
		}

		private void Activate()
		{
			_worldPosZ = base.transform.position.z;
			_willRotate = true;
		}

		private void StopRotating()
		{
			_willRotate = false;
		}

		private void PickupController_OnActivate()
		{
			Activate();
		}

		private void TrackObject_OnActivate()
		{
			Activate();
		}

		private void TrackObject_OnDeactivate()
		{
			StopRotating();
		}
	}
}
