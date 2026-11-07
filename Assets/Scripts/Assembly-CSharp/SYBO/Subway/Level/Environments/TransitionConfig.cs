using System;
using SYBO.Common.Utilities;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Level.Environments
{
	[Serializable]
	public class TransitionConfig
	{
		[Serializable]
		private class WeightRandomizedTransitionPrefabContainer : WeightRandomizedPrefabContainter<WeightedTransitionPrefab, Transform>
		{
			[Tooltip("Environments that do not trigger spawning transitions from this.")]
			public EnvironmentKind[] ExcludeEnvironments;

			[Range(0f, 1f)]
			public float SpawnProbability = 1f;
		}

		[Serializable]
		private class WeightedTransitionPrefab : WeightedPrefab<Transform>
		{
		}

		[SerializeField]
		[Tooltip("Environment to evaluate.")]
		private EnvironmentKind _environmentKind = EnvironmentKind.None;

		[SerializeField]
		[Tooltip("Transition configs used when transitioning TO this environment.")]
		private WeightRandomizedTransitionPrefabContainer _transitionStart = new WeightRandomizedTransitionPrefabContainer();

		[SerializeField]
		[Tooltip("Transition configs used when transitioning FROM this environment.")]
		private WeightRandomizedTransitionPrefabContainer _transitionEnd = new WeightRandomizedTransitionPrefabContainer();

		public EnvironmentKind EnvironmentKind
		{
			get
			{
				return _environmentKind;
			}
		}

		public void Initialize()
		{
			_transitionStart.RemoveEmptyEntries();
			_transitionEnd.RemoveEmptyEntries();
		}

		public bool TryGetStartPrefab(EnvironmentKind fromEnvironment, SyboRandom random, out Transform prefab)
		{
			prefab = null;
			EnvironmentKind[] excludeEnvironments = _transitionStart.ExcludeEnvironments;
			foreach (EnvironmentKind environmentKind in excludeEnvironments)
			{
				if (environmentKind == fromEnvironment)
				{
					return false;
				}
			}
			if (random.Value() > _transitionStart.SpawnProbability)
			{
				return false;
			}
			prefab = _transitionStart.PickSemiRandomlyOrderedPrefab(random);
			return true;
		}

		public bool TryGetEndPrefab(EnvironmentKind toEnvironment, SyboRandom random, out Transform prefab)
		{
			prefab = null;
			EnvironmentKind[] excludeEnvironments = _transitionEnd.ExcludeEnvironments;
			foreach (EnvironmentKind environmentKind in excludeEnvironments)
			{
				if (environmentKind == toEnvironment)
				{
					return false;
				}
			}
			if (random.Value() > _transitionEnd.SpawnProbability)
			{
				return false;
			}
			prefab = _transitionEnd.PickSemiRandomlyOrderedPrefab(random);
			return true;
		}
	}
}
