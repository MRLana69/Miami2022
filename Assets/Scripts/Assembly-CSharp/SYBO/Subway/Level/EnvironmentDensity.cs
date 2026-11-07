using System;

namespace SYBO.Subway.Level
{
	[Flags]
	public enum EnvironmentDensity
	{
		Default = 0,
		Low = 1,
		Medium = 2,
		High = 4,
		All = int.MaxValue
	}
}
