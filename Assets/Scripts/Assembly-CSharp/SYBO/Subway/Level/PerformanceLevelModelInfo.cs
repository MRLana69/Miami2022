using System;
using SYBO.Common.Utilities;

namespace SYBO.Subway.Level
{
	[Serializable]
	public struct PerformanceLevelModelInfo
	{
		public ModelInfo LowPerformance;

		public ModelInfo MediumPerformance;

		public ModelInfo HighPerformance;
	}
}
