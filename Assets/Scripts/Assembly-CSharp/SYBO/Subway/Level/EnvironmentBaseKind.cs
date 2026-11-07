namespace SYBO.Subway.Level
{
	public struct EnvironmentBaseKind
	{
		public readonly EnvironmentBaseType Type;

		public readonly EnvironmentBaseShadowSequence ShadowSequence;

		public readonly int NumTracks;

		public static readonly EnvironmentBaseKind None = new EnvironmentBaseKind(EnvironmentBaseType.None, EnvironmentBaseShadowSequence.None, 0);

		public static readonly EnvironmentBaseKind Empty = new EnvironmentBaseKind(EnvironmentBaseType.Empty, EnvironmentBaseShadowSequence.None, 0);

		public static readonly EnvironmentBaseKind Tracks0 = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.None, 0);

		public static readonly EnvironmentBaseKind Tracks1 = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.None, 1);

		public static readonly EnvironmentBaseKind Tracks1ShadowStart = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.Start, 1);

		public static readonly EnvironmentBaseKind Tracks1ShadowMid = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.Mid, 1);

		public static readonly EnvironmentBaseKind Tracks1ShadowEnd = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.End, 1);

		public static readonly EnvironmentBaseKind Tracks2 = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.None, 2);

		public static readonly EnvironmentBaseKind Tracks2ShadowStart = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.Start, 2);

		public static readonly EnvironmentBaseKind Tracks2ShadowMid = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.Mid, 2);

		public static readonly EnvironmentBaseKind Tracks2ShadowEnd = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.End, 2);

		public static readonly EnvironmentBaseKind Tracks3 = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.None, 3);

		public static readonly EnvironmentBaseKind Tracks3ShadowStart = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.Start, 3);

		public static readonly EnvironmentBaseKind Tracks3ShadowMid = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.Mid, 3);

		public static readonly EnvironmentBaseKind Tracks3ShadowEnd = new EnvironmentBaseKind(EnvironmentBaseType.GroundAndTracks, EnvironmentBaseShadowSequence.End, 3);

		public static readonly EnvironmentBaseKind Tracks3Gates = new EnvironmentBaseKind(EnvironmentBaseType.Gates, EnvironmentBaseShadowSequence.None, 3);

		public static readonly EnvironmentBaseKind Tracks3GatesShadow = new EnvironmentBaseKind(EnvironmentBaseType.Gates, EnvironmentBaseShadowSequence.StartEnd, 3);

		public static readonly EnvironmentBaseKind Tracks3Special = new EnvironmentBaseKind(EnvironmentBaseType.Special, EnvironmentBaseShadowSequence.None, 3);

		private EnvironmentBaseKind(EnvironmentBaseType envBaseType, EnvironmentBaseShadowSequence shadowSequence, int numTracks)
		{
			Type = envBaseType;
			ShadowSequence = shadowSequence;
			NumTracks = numTracks;
		}

		public static bool operator ==(EnvironmentBaseKind c1, EnvironmentBaseKind c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(EnvironmentBaseKind c1, EnvironmentBaseKind c2)
		{
			return !c1.Equals(c2);
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 29 + Type.GetHashCode();
			num = num * 29 + ShadowSequence.GetHashCode();
			return num * 29 + NumTracks.GetHashCode();
		}

		public override bool Equals(object other)
		{
			if (!(other is EnvironmentBaseKind))
			{
				return false;
			}
			return Equals((EnvironmentBaseKind)other);
		}

		public bool Equals(EnvironmentBaseKind other)
		{
			return Type == other.Type && ShadowSequence == other.ShadowSequence && NumTracks == other.NumTracks;
		}

		public override string ToString()
		{
			return string.Format("EnvironmentBaseKind: type={0} shadow={1} numTrack={2}", Type, ShadowSequence, NumTracks);
		}
	}
}
