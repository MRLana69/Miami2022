using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Subway.Level.Environments
{
	public class DistantObject : MonoBehaviour
	{
		private const int BaseRenderQueueOffset = 10;

		private Vector3 _trueSpawnPosition = Vector3.zero;

		private float _projectionFactor;

		public void Initialize(Vector3 trueSpawnGamePosition, Vector3 cameraGamePosition, float projectionFactor)
		{
			_trueSpawnPosition = trueSpawnGamePosition;
			_projectionFactor = 1f / projectionFactor;
			UpdateDistantObject(cameraGamePosition);
			base.transform.localScale = Vector3.one * _projectionFactor;
		}

		private void OnEnable()
		{
			if (!SingletonMonoBehaviour.IsShuttingDown)
			{
				SingletonMonoBehaviour<DistantEnvironmentController>.Instance.RegisterDistantObject(this);
			}
		}

		private void OnDisable()
		{
			if (!SingletonMonoBehaviour.IsShuttingDown)
			{
				SingletonMonoBehaviour<DistantEnvironmentController>.Instance.DeregisterDistantObject(this);
			}
		}

		public void UpdateDistantObject(Vector3 camemarGamePosition)
		{
			Vector3 vector = _trueSpawnPosition - camemarGamePosition;
			Vector3 gamePosition = _trueSpawnPosition;
			if (_projectionFactor > 0f)
			{
				gamePosition = camemarGamePosition + vector * _projectionFactor;
			}
			gamePosition.y = _trueSpawnPosition.y;
			base.transform.position = GamePositionRoot.Instance.GetWorldPosition(gamePosition);
		}
	}
}
