using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public struct EnvironmentSegmentParts
	{
		public const string FieldPatternLeft = "Left";

		public Transform Left;

		public const string FieldPatternRight = "Right";

		public Transform Right;

		public const string FieldPatternJoint = "Joint";

		public Transform Joint;
	}
}
