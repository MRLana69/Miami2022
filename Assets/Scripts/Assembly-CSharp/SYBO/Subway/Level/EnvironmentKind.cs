using System;
using SYBO.Common.Exceptions;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class EnvironmentKind
	{
		public const string FieldPatternType = "_type";

		[SerializeField]
		private EnvironmentType _type;

		[SerializeField]
		private EnvironmentDensity _density;

		public static readonly EnvironmentKind Empty = new EnvironmentKind(EnvironmentType.Empty, EnvironmentDensity.Default);

		public static readonly EnvironmentKind FillersLow = new EnvironmentKind(EnvironmentType.Fillers, EnvironmentDensity.Low);

		public static readonly EnvironmentKind FillersMedium = new EnvironmentKind(EnvironmentType.Fillers, EnvironmentDensity.Medium);

		public static readonly EnvironmentKind FillersHigh = new EnvironmentKind(EnvironmentType.Fillers, EnvironmentDensity.High);

		public static readonly EnvironmentKind Gates = new EnvironmentKind(EnvironmentType.Gates, EnvironmentDensity.Default);

		public static readonly EnvironmentKind Tube = new EnvironmentKind(EnvironmentType.Tube, EnvironmentDensity.Default);

		public static readonly EnvironmentKind Pillars = new EnvironmentKind(EnvironmentType.Pillars, EnvironmentDensity.Default);

		public static readonly EnvironmentKind Station = new EnvironmentKind(EnvironmentType.Station, EnvironmentDensity.Default);

		public static readonly EnvironmentKind Epic = new EnvironmentKind(EnvironmentType.Epic, EnvironmentDensity.Default);

		public static readonly EnvironmentKind[] All = new EnvironmentKind[9] { FillersLow, FillersMedium, FillersHigh, Gates, Tube, Pillars, Station, Epic, Empty };

		public static readonly EnvironmentKind[] AllUnconstrained = new EnvironmentKind[4] { FillersLow, FillersMedium, FillersHigh, Tube };

		public static readonly EnvironmentKind[] AllWithSplitPieces = new EnvironmentKind[4] { Tube, Pillars, Station, Epic };

		public EnvironmentType Type
		{
			get
			{
				return _type;
			}
		}

		public EnvironmentDensity Density
		{
			get
			{
				return _density;
			}
		}

		public bool IsRepeatable
		{
			get
			{
				return _type == EnvironmentType.Fillers || _type == EnvironmentType.Tube;
			}
		}

		public static EnvironmentKind None
		{
			get
			{
				return new EnvironmentKind(EnvironmentType.None, EnvironmentDensity.Default);
			}
		}

		public EnvironmentKind(EnvironmentType type, EnvironmentDensity density)
		{
			_type = type;
			_density = density;
		}

		public static bool operator ==(EnvironmentKind c1, EnvironmentKind c2)
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(EnvironmentKind c1, EnvironmentKind c2)
		{
			return !c1.Equals(c2);
		}

		public override bool Equals(object other)
		{
			if (!(other is EnvironmentKind))
			{
				return false;
			}
			return Equals((EnvironmentKind)other);
		}

		public bool Equals(EnvironmentKind other)
		{
			return _type == other._type && _density == other._density;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 29 + _type.GetHashCode();
			return num * 29 + _density.GetHashCode();
		}

		public override string ToString()
		{
			return string.Concat(_type, " ", _density);
		}

		public EnvironmentKind GetValidated()
		{
			if (IsValidDensity(this))
			{
				return this;
			}
			return new EnvironmentKind(_type, GetDefaultDensity(_type));
		}

		public static bool IsValidDensity(EnvironmentKind envKind)
		{
			switch (envKind.Type)
			{
			case EnvironmentType.None:
			case EnvironmentType.Tube:
			case EnvironmentType.Pillars:
			case EnvironmentType.Gates:
			case EnvironmentType.Station:
			case EnvironmentType.Epic:
				return envKind.Density == EnvironmentDensity.Default;
			case EnvironmentType.Fillers:
				return envKind.Density == EnvironmentDensity.Low || envKind.Density == EnvironmentDensity.Medium || envKind.Density == EnvironmentDensity.High;
			default:
				throw new SwitchDefaultException("Missing switch case for " + envKind);
			}
		}

		public static EnvironmentDensity GetDefaultDensity(EnvironmentType envType)
		{
			switch (envType)
			{
			case EnvironmentType.None:
			case EnvironmentType.Tube:
			case EnvironmentType.Pillars:
			case EnvironmentType.Gates:
			case EnvironmentType.Station:
			case EnvironmentType.Epic:
				return EnvironmentDensity.Default;
			case EnvironmentType.Fillers:
				return EnvironmentDensity.Low;
			default:
				throw new SwitchDefaultException("Missing switch case for " + envType);
			}
		}
	}
}
