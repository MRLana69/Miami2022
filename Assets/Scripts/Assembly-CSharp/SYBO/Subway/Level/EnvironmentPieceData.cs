using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public struct EnvironmentPieceData
	{
		[SerializeField]
		public Transform Left;

		[SerializeField]
		public Transform Right;

		[SerializeField]
		public Transform Joint;
	}
}
