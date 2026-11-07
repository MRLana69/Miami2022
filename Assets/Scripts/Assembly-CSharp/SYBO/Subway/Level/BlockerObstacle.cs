using SYBO.Common.Utilities.Flags;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class BlockerObstacle : Obstacle
	{
		[EnumFlagProperty(typeof(BlockerObstacleType))]
		[SerializeField]
		private BlockerObstacleType _blockerTypes = BlockerObstacleType.All;

		[SerializeField]
		private bool _withCoins;

		[SerializeField]
		private bool _noLaneRailsUntilNextBlocker;
	}
}
