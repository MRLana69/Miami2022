using System;

namespace SYBO.Subway.Level
{
	[Flags]
	public enum EnvironmentObstacleType
	{
		None = 0,
		Pillar = 1,
		Station = 2,
		Gates = 4
	}
}
