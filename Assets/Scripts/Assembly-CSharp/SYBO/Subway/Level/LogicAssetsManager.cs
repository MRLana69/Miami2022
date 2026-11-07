using System;
using System.Collections.Generic;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[PrefabPath("Prefabs/Managers/LogicAssetsManager")]
	public class LogicAssetsManager : SingletonMonoBehaviour<LogicAssetsManager>
	{
		[Serializable]
		public class BasicObstaclePrefab
		{
			[SerializeField]
			public BasicObstacleType Type;

			[SerializeField]
			public GameObject Prefab;

			internal BasicObstaclePrefab(BasicObstacleType type)
			{
				Type = type;
				Prefab = null;
			}
		}

		[Serializable]
		public class BlockerObstaclePrefab
		{
			[SerializeField]
			public BlockerObstacleType Type;

			[SerializeField]
			public GameObject Prefab;

			internal BlockerObstaclePrefab(BlockerObstacleType type)
			{
				Type = type;
				Prefab = null;
			}
		}

		[Serializable]
		public class EnvironmentObstaclePrefab
		{
			[SerializeField]
			public EnvironmentObstacleType Type;

			[SerializeField]
			public GameObject Prefab;

			internal EnvironmentObstaclePrefab(EnvironmentObstacleType type)
			{
				Type = type;
				Prefab = null;
			}
		}

		[Serializable]
		public class ObstacleAssets
		{
			[SerializeField]
			public List<BasicObstaclePrefab> basicObstacles;

			[SerializeField]
			public List<BasicObstaclePrefab> basicObstaclesWithCoins;

			[SerializeField]
			public List<BlockerObstaclePrefab> blockerObstacles;

			[SerializeField]
			public List<BlockerObstaclePrefab> blockerObstaclesWithCoins;

			[SerializeField]
			public List<EnvironmentObstaclePrefab> environmentObstacles;

			[SerializeField]
			public List<EnvironmentObstaclePrefab> environmentObstaclesExtended;

			internal ObstacleAssets()
			{
				basicObstacles = new List<BasicObstaclePrefab>
				{
					new BasicObstaclePrefab(BasicObstacleType.LightSignal),
					new BasicObstaclePrefab(BasicObstacleType.Bush),
					new BasicObstaclePrefab(BasicObstacleType.BushMix),
					new BasicObstaclePrefab(BasicObstacleType.BushAndPowerbox),
					new BasicObstaclePrefab(BasicObstacleType.Dumpster)
				};
				basicObstaclesWithCoins = new List<BasicObstaclePrefab>
				{
					new BasicObstaclePrefab(BasicObstacleType.LightSignal),
					new BasicObstaclePrefab(BasicObstacleType.Bush),
					new BasicObstaclePrefab(BasicObstacleType.BushMix),
					new BasicObstaclePrefab(BasicObstacleType.BushAndPowerbox),
					new BasicObstaclePrefab(BasicObstacleType.Dumpster)
				};
				blockerObstacles = new List<BlockerObstaclePrefab>
				{
					new BlockerObstaclePrefab(BlockerObstacleType.BlockerStandard),
					new BlockerObstaclePrefab(BlockerObstacleType.BlockerRoll),
					new BlockerObstaclePrefab(BlockerObstacleType.BlockerJump)
				};
				blockerObstaclesWithCoins = new List<BlockerObstaclePrefab>
				{
					new BlockerObstaclePrefab(BlockerObstacleType.BlockerStandard),
					new BlockerObstaclePrefab(BlockerObstacleType.BlockerRoll),
					new BlockerObstaclePrefab(BlockerObstacleType.BlockerJump)
				};
				environmentObstacles = new List<EnvironmentObstaclePrefab>
				{
					new EnvironmentObstaclePrefab(EnvironmentObstacleType.Pillar),
					new EnvironmentObstaclePrefab(EnvironmentObstacleType.Station),
					new EnvironmentObstaclePrefab(EnvironmentObstacleType.Gates)
				};
				environmentObstaclesExtended = new List<EnvironmentObstaclePrefab>
				{
					new EnvironmentObstaclePrefab(EnvironmentObstacleType.Station)
				};
			}
		}

		[SerializeField]
		public ObstacleAssets obstacles = new ObstacleAssets();

		[SerializeField]
		public EnvironmentAssets environment = new EnvironmentAssets();

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
		}

		public bool HasEnvironmentSegmentLogic(EnvironmentType envType)
		{
			switch (envType)
			{
			case EnvironmentType.Tube:
			case EnvironmentType.Pillars:
			case EnvironmentType.Station:
				return true;
			default:
				return false;
			}
		}

		public Transform SpawnEnvironmentSegmentLogic(EnvironmentType envType, EnvironmentSequence sequence)
		{
			EnvironmentSequentialSegments environmentSequentialSegments;
			switch (envType)
			{
			case EnvironmentType.Pillars:
				environmentSequentialSegments = environment.pillars;
				break;
			case EnvironmentType.Station:
				environmentSequentialSegments = environment.station;
				break;
			case EnvironmentType.Tube:
				environmentSequentialSegments = environment.tube;
				break;
			default:
				SDebug.LogError("No logic defined for environment type " + envType);
				return null;
			}
			Transform transform = null;
			switch (sequence)
			{
			case EnvironmentSequence.Start:
				transform = environmentSequentialSegments.Start;
				break;
			case EnvironmentSequence.Mid:
				transform = environmentSequentialSegments.Mid;
				break;
			case EnvironmentSequence.End:
				transform = environmentSequentialSegments.End;
				break;
			}
			if (transform == null)
			{
				return null;
			}
			return PoolManager.Spawn(transform);
		}

		public void DespawnEnvironmentSegmentLogic(Transform segmentTransform)
		{
			PoolManager.Despawn(segmentTransform);
		}
	}
}
