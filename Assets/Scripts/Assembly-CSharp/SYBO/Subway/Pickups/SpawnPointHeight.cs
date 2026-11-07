using System;

namespace SYBO.Subway.Pickups
{
	[Flags]
	public enum SpawnPointHeight
	{
		OnGround = 1,
		OnTrain = 2,
		InSky = 4
	}
}
