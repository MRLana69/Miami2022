using System;

namespace SYBO.Subway.Level
{
	[Flags]
	public enum BasicObstacleType
	{
		LightSignal = 0,
		Bush = 1,
		BushMix = 2,
		BushAndPowerbox = Bush | BushMix,
		Dumpster = 4
	}
}
