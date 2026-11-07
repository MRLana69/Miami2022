using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class EnvironmentAssets
	{
		[SerializeField]
		public EnvironmentSequentialSegments station;

		[SerializeField]
		public EnvironmentSequentialSegments tube;

		[SerializeField]
		public EnvironmentSequentialSegments pillars;
	}
}
