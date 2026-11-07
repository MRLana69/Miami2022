using System;

namespace SYBO.Subway.Level
{
	[Serializable]
	public struct EnvironmentConstraint
	{
		public EnvironmentType Type;

		public EnvironmentDensity SideDensity;

		public EnvironmentSegmentLength SegmentLength;
	}
}
