using System;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public struct GroundAssetGroup
	{
		public Transform Ground;

		public Transform GroundShadowMid;

		public Transform GroundShortShadowStart;

		public Transform GroundShortShadowEnd;

		public Transform Track;

		public Transform TrackShadowStart;

		public Transform TrackShadowMid;

		public Transform TrackShadowEnd;

		public Transform TrackShortShadowStart;

		public Transform TrackShortShadowEnd;

		[Header("Complete segments")]
		public Transform TracksGates;

		public Transform TracksGatesShadow;

		public Transform TracksSpecial;
	}
}
