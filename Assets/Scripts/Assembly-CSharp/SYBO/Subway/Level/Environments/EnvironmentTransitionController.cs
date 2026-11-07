using System;
using System.Collections.Generic;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Routes;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Level.Environments
{
	public class EnvironmentTransitionController : SingletonMonoBehaviour<EnvironmentTransitionController>
	{
		private EnvironmentKind _lastEnvironmentKind = EnvironmentKind.None;

		private Queue<Transform> _spawnedTransitions = new Queue<Transform>();

		protected override void SubclassInitialize()
		{
			base.SubclassInitialize();
			RouteChunk.OnEnvironmentSegmentSpawned = (Action<EnvironmentKind, int, Route>)Delegate.Combine(RouteChunk.OnEnvironmentSegmentSpawned, new Action<EnvironmentKind, int, Route>(RouteChunk_OnEnvironmentSegmentSpawned));
			Game.OnGameEnded += Game_OnGameEnded;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			RouteChunk.OnEnvironmentSegmentSpawned = (Action<EnvironmentKind, int, Route>)Delegate.Remove(RouteChunk.OnEnvironmentSegmentSpawned, new Action<EnvironmentKind, int, Route>(RouteChunk_OnEnvironmentSegmentSpawned));
			Game.OnGameEnded -= Game_OnGameEnded;
		}

		private void RouteChunk_OnEnvironmentSegmentSpawned(EnvironmentKind environmentKind, int gamePositionZ, Route route)
		{
			if (route.Type != RouteType.Default || environmentKind == _lastEnvironmentKind)
			{
				return;
			}
			TransitionConfig[] environmentTransitionConfigs = ThemeManager.Instance.Theme.EnvironmentTransitionConfigs;
			foreach (TransitionConfig transitionConfig in environmentTransitionConfigs)
			{
				Transform prefab;
				if (transitionConfig.EnvironmentKind == _lastEnvironmentKind)
				{
					if (transitionConfig.TryGetEndPrefab(environmentKind, Game.Instance.RandomGenerator, out prefab))
					{
						Spawn(prefab, gamePositionZ);
					}
				}
				else if (transitionConfig.EnvironmentKind == environmentKind && transitionConfig.TryGetStartPrefab(_lastEnvironmentKind, Game.Instance.RandomGenerator, out prefab))
				{
					Spawn(prefab, gamePositionZ);
				}
			}
			_lastEnvironmentKind = environmentKind;
		}

		private void Spawn(Transform prefab, int gamePositionZ)
		{
			if (!(prefab == null))
			{
				Transform transform = PoolManager.Spawn(prefab);
				transform.position = Vector3.forward * GamePositionRoot.Instance.GetWorldPositionZ(gamePositionZ);
				transform.parent = base.transform;
				GamePositionRoot.Instance.AddOffsetTransform(transform);
				_spawnedTransitions.Enqueue(transform);
			}
		}

		private void Game_OnGameEnded()
		{
			_lastEnvironmentKind = EnvironmentKind.None;
			while (_spawnedTransitions.Count > 0)
			{
				DespawnFirstInstance();
			}
		}

		private void LateUpdate()
		{
			if (_spawnedTransitions.Count > 0 && Character.Instance.TransformPosition.z - 170f > _spawnedTransitions.Peek().position.z)
			{
				DespawnFirstInstance();
			}
		}

		private void DespawnFirstInstance()
		{
			Transform instance = _spawnedTransitions.Dequeue();
			GamePositionRoot.Instance.RemoveOffsetTransform(instance);
			PoolManager.Despawn(instance);
		}
	}
}
