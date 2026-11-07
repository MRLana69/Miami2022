using SYBO.Common.Utilities;
using SYBO.Common.Visuals;
using UnityEngine;

namespace SYBO.Subway.Visuals
{
	[RequireComponent(typeof(TrackObject))]
	public class OnActivationMeshCombineLauncher : MonoBehaviour
	{
		private TrackObject _trackObject;

		private MeshCombiner _meshCombiner;

		private void Awake()
		{
			_trackObject = GetComponent<TrackObject>();
			if (_trackObject != null)
			{
				_trackObject.OnActivate += TrackObject_OnActivate;
				_trackObject.OnDeactivate += TrackObject_OnDeactivate;
			}
			_meshCombiner = this.AddComponentIfMissing(typeof(MeshCombiner)) as MeshCombiner;
		}

		private void TrackObject_OnActivate()
		{
			_meshCombiner.CombineInactive();
		}

		private void TrackObject_OnDeactivate()
		{
			_meshCombiner.DestroyCombined();
		}

		private void OnDestroy()
		{
			if (_trackObject != null)
			{
				_trackObject.OnActivate -= TrackObject_OnActivate;
				_trackObject.OnDeactivate -= TrackObject_OnDeactivate;
			}
		}
	}
}
