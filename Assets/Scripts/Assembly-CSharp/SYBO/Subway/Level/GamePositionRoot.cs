using System.Collections.Generic;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class GamePositionRoot : SingletonMonoBehaviour<GamePositionRoot>
	{
		public const int ZDistanceThreshold = 50000;

		public const int ZDistanceTargetPosition = 1000;

		private int _currentGameOffsetZ;

		private Character _character;

		private HashSet<Transform> _offsetTransforms = new HashSet<Transform>();

		private HashSet<object> _locks = new HashSet<object>();

		private static GamePositionRoot _instance;

		private static bool _instantiated;

		public float CurrentGameOffsetZ
		{
			get
			{
				return _currentGameOffsetZ;
			}
		}

		public new static GamePositionRoot Instance
		{
			get
			{
				if (!_instantiated)
				{
					_instance = SingletonMonoBehaviour<GamePositionRoot>.Instance;
					_instantiated = true;
				}
				return _instance;
			}
		}

		private void Start()
		{
			_character = Character.Instance;
			Game.OnGameEnded += Game_OnGameEnded;
		}

		private int GetZDistanceTargetPosition()
		{
			return 1000;
		}

		private void LateUpdate()
		{
			float z = _character.TransformPosition.z;
			if (!(z >= 50000f) || _locks.Count != 0)
			{
				return;
			}
			int num = Mathf.FloorToInt(z) - GetZDistanceTargetPosition();
			_currentGameOffsetZ += num;
			Vector3 vector = Vector3.forward * num;
			foreach (Transform offsetTransform in _offsetTransforms)
			{
				offsetTransform.position -= vector;
			}
			_character.CachePositions();
		}

		public void AddOffsetTransform(Transform transform)
		{
			_offsetTransforms.Add(transform);
		}

		public void RemoveOffsetTransform(Transform transform)
		{
			_offsetTransforms.Remove(transform);
		}

		public Vector3 GetGamePosition(Transform transform)
		{
			return GetGamePosition(transform.position);
		}

		public Vector3 GetGamePosition(Vector3 worldPosition)
		{
			Vector3 result = worldPosition;
			result.z += _currentGameOffsetZ;
			return result;
		}

		public Vector3 GetWorldPosition(Vector3 gamePosition)
		{
			Vector3 result = gamePosition;
			result.z = GetWorldPositionZ(result.z);
			return result;
		}

		public float GetWorldPositionZ(float gamePositionZ)
		{
			return gamePositionZ - (float)_currentGameOffsetZ;
		}

		public void AddLock(object lockObject)
		{
			_locks.Add(lockObject);
		}

		public void RemoveLock(object lockObject)
		{
			_locks.Remove(lockObject);
		}

		private void Game_OnGameEnded()
		{
			ResetGamePosition();
		}

		private void ResetGamePosition()
		{
			_currentGameOffsetZ = 0;
			foreach (Transform offsetTransform in _offsetTransforms)
			{
				offsetTransform.position = Vector3.zero;
			}
			_locks.Clear();
		}
	}
}
