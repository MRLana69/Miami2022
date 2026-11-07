using System;

namespace SYBO.Subway.Level
{
	[Flags]
	public enum BlockerObstacleType
	{
		BlockerStandard = 1,
		BlockerJump = 2,
		BlockerRoll = 4,
		All = int.MaxValue
	}
}
