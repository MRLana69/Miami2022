using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public struct EnvironmentSequentialSegments
	{
		[SerializeField]
		public Transform Start;

		[SerializeField]
		public Transform Mid;

		[SerializeField]
		public Transform End;
	}
}
