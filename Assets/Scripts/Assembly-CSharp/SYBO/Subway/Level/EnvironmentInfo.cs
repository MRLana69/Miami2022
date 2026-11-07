using System;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class EnvironmentInfo
	{
		public readonly EnvironmentKind Kind;

		public readonly bool HasStartEnd;

		public readonly bool HasSides;

		public readonly bool UseSameVariationPerSequence;

		public readonly bool UseSameVariationForSides;

		public readonly int MaxMidPieceCount;

		public readonly int MidPieceLengthInBlocks;

		public readonly int StartEndPieceLengthInBlocks;

		public static int DefaultMaxMidPieceCount = 100;

		public static int DefaultMidPieceLengthInBlocks = 2;

		public static int DefaultMaxMidLengthInBlocks = DefaultMidPieceLengthInBlocks * DefaultMaxMidPieceCount;

		public static readonly EnvironmentInfo None = new EnvironmentInfo(EnvironmentKind.None, false, false, false, false, 0, 0, 0);

		public static readonly EnvironmentInfo Empty = new EnvironmentInfo(EnvironmentKind.Empty, false, false, false, false, 0, 0, 0);

		public static readonly EnvironmentInfo FillersLow = new EnvironmentInfo(EnvironmentKind.FillersLow, false, true, false, false, DefaultMaxMidPieceCount, 2, 0);

		public static readonly EnvironmentInfo FillersMedium = new EnvironmentInfo(EnvironmentKind.FillersMedium, false, true, false, false, DefaultMaxMidPieceCount, 2, 0);

		public static readonly EnvironmentInfo FillersHigh = new EnvironmentInfo(EnvironmentKind.FillersHigh, false, true, false, false, DefaultMaxMidPieceCount, 2, 0);

		public static readonly EnvironmentInfo Gates = new EnvironmentInfo(EnvironmentKind.Gates, false, false, false, false, 1, 4, 0);

		public static readonly EnvironmentInfo Tube = new EnvironmentInfo(EnvironmentKind.Tube, false, false, true, false, DefaultMaxMidPieceCount, 2, 0);

		public static readonly EnvironmentInfo Pillars = new EnvironmentInfo(EnvironmentKind.Pillars, true, false, true, false, DefaultMaxMidPieceCount, 2, 1);

		public static readonly EnvironmentInfo Station = new EnvironmentInfo(EnvironmentKind.Station, true, false, true, false, DefaultMaxMidPieceCount, 2, 1);

		public static readonly EnvironmentInfo Epic = new EnvironmentInfo(EnvironmentKind.Epic, true, false, true, false, DefaultMaxMidPieceCount, 4, 4);

		public static readonly EnvironmentInfo[] All = new EnvironmentInfo[9] { FillersLow, FillersMedium, FillersHigh, Gates, Tube, Pillars, Station, Epic, Empty };

		public int MidPieceLength
		{
			get
			{
				return MidPieceLengthInBlocks * 90;
			}
		}

		public int StartEndPieceLength
		{
			get
			{
				return StartEndPieceLengthInBlocks * 90;
			}
		}

		public int MinLengthInBlocks
		{
			get
			{
				return (!HasStartEnd) ? MidPieceLengthInBlocks : (2 * StartEndPieceLengthInBlocks + MidPieceLengthInBlocks);
			}
		}

		public int LengthMin
		{
			get
			{
				return MinLengthInBlocks * 90;
			}
		}

		public int MidLengthMax
		{
			get
			{
				return MaxMidPieceCount * MidPieceLengthInBlocks;
			}
		}

		public int MaxLengthInBlocks
		{
			get
			{
				return (!HasStartEnd) ? MidLengthMax : (2 * StartEndPieceLengthInBlocks + MidLengthMax);
			}
		}

		public int MaxLength
		{
			get
			{
				return MaxLengthInBlocks * 90;
			}
		}

		public EnvironmentInfo(EnvironmentKind envKind, bool hasStartEnd, bool hasSides, bool useSameVariationPerSequence, bool useSameVariationForSides, int maxMidPieceCount, int lengthMidPieceInBlocks, int lengthStartEndPieceInBlocks)
		{
			Kind = envKind;
			HasStartEnd = hasStartEnd;
			HasSides = hasSides;
			UseSameVariationPerSequence = useSameVariationPerSequence;
			UseSameVariationForSides = useSameVariationForSides;
			if (DefaultMaxMidPieceCount < maxMidPieceCount)
			{
				SDebug.LogError(string.Format("[EnvironmentInfo] Kind {0} : Max mid piece count {1} larger than allowed {2}", envKind, maxMidPieceCount, DefaultMaxMidPieceCount));
			}
			MaxMidPieceCount = maxMidPieceCount;
			MidPieceLengthInBlocks = lengthMidPieceInBlocks;
			StartEndPieceLengthInBlocks = lengthStartEndPieceInBlocks;
		}

		public int GetSequenceLengthInBlocks(EnvironmentSequence sequence)
		{
			if (HasStartEnd && (sequence == EnvironmentSequence.Start || sequence == EnvironmentSequence.End))
			{
				return StartEndPieceLengthInBlocks;
			}
			return MidPieceLengthInBlocks;
		}

		public static EnvironmentInfo GetByKind(EnvironmentKind envKind)
		{
			EnvironmentInfo result = None;
			bool flag = false;
			EnvironmentInfo[] all = All;
			foreach (EnvironmentInfo environmentInfo in all)
			{
				if (environmentInfo.Kind == envKind)
				{
					result = environmentInfo;
					flag = true;
				}
			}
			if (!flag)
			{
				SDebug.LogError("Could not find " + typeof(EnvironmentInfo).Name + " for EnvironmentKind " + envKind);
			}
			return result;
		}
	}
}
