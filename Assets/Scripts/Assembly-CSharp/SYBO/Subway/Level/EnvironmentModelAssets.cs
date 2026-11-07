using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class EnvironmentModelAssets
	{
		[HideInInspector]
		public string Name;

		public const string FieldPatternEnvironment = "Environment";

		public EnvironmentKind Environment;

		public const string FieldPatternSequenceVariations = "SequenceVariations";

		public EnvironmentSegmentSequence[] SequenceVariations;
	}
}
