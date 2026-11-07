using System;
using System.Collections.Generic;

namespace SYBO.Subway.Level
{
	public class EnvironmentBaseInfo : IEquatable<EnvironmentBaseInfo>
	{
		public readonly EnvironmentBaseKind EnvironmentBaseKind;

		public readonly int LengthInBlocks;

		public static readonly EnvironmentBaseInfo Tracks0 = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks0, 2);

		public static readonly EnvironmentBaseInfo Tracks1 = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks1, 2);

		public static readonly EnvironmentBaseInfo Tracks1ShadowMid = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks1ShadowMid, 2);

		public static readonly EnvironmentBaseInfo Tracks1ShortShadowStart = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks1ShadowStart, 1);

		public static readonly EnvironmentBaseInfo Tracks1ShortShadowEnd = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks1ShadowEnd, 1);

		public static readonly EnvironmentBaseInfo Tracks2 = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks2, 2);

		public static readonly EnvironmentBaseInfo Tracks2ShadowMid = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks2ShadowMid, 2);

		public static readonly EnvironmentBaseInfo Tracks2ShortShadowStart = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks2ShadowStart, 1);

		public static readonly EnvironmentBaseInfo Tracks2ShortShadowEnd = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks2ShadowEnd, 1);

		public static readonly EnvironmentBaseInfo Tracks3 = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3, 2);

		public static readonly EnvironmentBaseInfo Tracks3ShadowStart = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3ShadowStart, 2);

		public static readonly EnvironmentBaseInfo Tracks3ShadowMid = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3ShadowMid, 2);

		public static readonly EnvironmentBaseInfo Tracks3ShadowEnd = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3ShadowEnd, 2);

		public static readonly EnvironmentBaseInfo Tracks3ShortShadowStart = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3ShadowStart, 1);

		public static readonly EnvironmentBaseInfo Tracks3ShortShadowEnd = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3ShadowEnd, 1);

		public static readonly EnvironmentBaseInfo Tracks3Gates = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3Gates, 2);

		public static readonly EnvironmentBaseInfo Tracks3GatesShadow = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3GatesShadow, 4);

		public static readonly EnvironmentBaseInfo Tracks3Special = new EnvironmentBaseInfo(EnvironmentBaseKind.Tracks3Special, 1);

		private static readonly Dictionary<EnvironmentBaseInfo, string> AllGroundKinds = new Dictionary<EnvironmentBaseInfo, string>
		{
			{ Tracks0, "tracks_0" },
			{ Tracks1, "tracks_1" },
			{ Tracks1ShadowMid, "tracks_1_shadow_mid" },
			{ Tracks1ShortShadowStart, "tracks_1_short_shadow_start" },
			{ Tracks1ShortShadowEnd, "tracks_1_short_shadow_end" },
			{ Tracks2, "tracks_2" },
			{ Tracks2ShadowMid, "tracks_2_shadow_mid" },
			{ Tracks2ShortShadowStart, "tracks_2_short_shadow_start" },
			{ Tracks2ShortShadowEnd, "tracks_2_short_shadow_end" },
			{ Tracks3, "tracks_3" },
			{ Tracks3ShadowStart, "tracks_3_shadow_start" },
			{ Tracks3ShadowMid, "tracks_3_shadow_mid" },
			{ Tracks3ShadowEnd, "tracks_3_shadow_end" },
			{ Tracks3ShortShadowStart, "tracks_3_short_shadow_start" },
			{ Tracks3ShortShadowEnd, "tracks_3_short_shadow_end" },
			{ Tracks3Gates, "tracks_3_gates" },
			{ Tracks3GatesShadow, "tracks_3_gates_shadow" },
			{ Tracks3Special, "tracks_3_special" }
		};

		public EnvironmentBaseInfo(EnvironmentBaseKind environmentBaseType, int lengthInBlocks)
		{
			EnvironmentBaseKind = environmentBaseType;
			LengthInBlocks = lengthInBlocks;
		}

		public static bool operator ==(EnvironmentBaseInfo c1, EnvironmentBaseInfo c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(EnvironmentBaseInfo c1, EnvironmentBaseInfo c2)
		{
			return !c1.Equals(c2);
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 29 + EnvironmentBaseKind.GetHashCode();
			return num * 29 + LengthInBlocks.GetHashCode();
		}

		public override bool Equals(object other)
		{
			if (!(other is EnvironmentBaseInfo))
			{
				return false;
			}
			return Equals((EnvironmentBaseInfo)other);
		}

		public bool Equals(EnvironmentBaseInfo other)
		{
			return EnvironmentBaseKind == other.EnvironmentBaseKind && LengthInBlocks == other.LengthInBlocks;
		}

		public override string ToString()
		{
			return string.Format("EnvironmentBaseInfo: kind=[{0}] lengthInBlocks={1}", EnvironmentBaseKind, LengthInBlocks);
		}

		public static string GetName(EnvironmentBaseInfo groundInfo)
		{
			string value;
			if (!AllGroundKinds.TryGetValue(groundInfo, out value))
			{
				return "UNDEFINED";
			}
			return value;
		}
	}
}
