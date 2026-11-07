using System;

namespace SYBO.Subway.Level
{
	[Flags]
	public enum EnvironmentType
	{
		None = 0,
		Fillers = 1,
		Tube = 2,
		Pillars = 4,
		Gates = 8,
		Station = 0x10,
		Epic = 0x20,
		Empty = 0x40,
		All = int.MaxValue
	}
}
