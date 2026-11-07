using System;

namespace SYBO.Subway.Level
{
	[Serializable]
	public struct EnvironmentSegmentSequence
	{
		public const string FieldPatternStart = "Start";

		public EnvironmentSegmentParts Start;

		public const string FieldPatternMid = "Mid";

		public EnvironmentSegmentParts Mid;

		public const string FieldPatternEnd = "End";

		public EnvironmentSegmentParts End;
	}
}
