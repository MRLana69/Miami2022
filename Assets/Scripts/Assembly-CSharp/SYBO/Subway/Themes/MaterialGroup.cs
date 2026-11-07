using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Themes
{
	[Serializable]
	public class MaterialGroup
	{
		[HideInInspector]
		public string Name;

		[FormerlySerializedAs("highPerformance")]
		public Material HighPerformance;

		[FormerlySerializedAs("mediumPerformance")]
		public Material MediumPerformance;

		[FormerlySerializedAs("lowPerformance")]
		public Material LowPerformance;
	}
}
