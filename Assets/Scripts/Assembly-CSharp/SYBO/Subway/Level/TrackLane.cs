using System;

namespace SYBO.Subway.Level
{
	[Flags]
	public enum TrackLane
	{
		None = 0,
		Left = 1,
		Mid = 2,
		Right = 4,
		MidLeft = Left | Mid,
		MidRight = Mid | Right,
		Sides = Left | Right,
		All = int.MaxValue
	}
}
