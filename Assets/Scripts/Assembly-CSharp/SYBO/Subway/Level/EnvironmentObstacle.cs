using SYBO.Common.Utilities.Flags;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class EnvironmentObstacle : Obstacle
	{
		[SerializeField]
		private EnvironmentObstacleType _obstacleType = EnvironmentObstacleType.Pillar;

		[Tooltip("Indicates which lanes should be blocked. 'None' indicates no specific lane, just an obstacles at given position, e.g. pillar")]
		[EnumFlagProperty(typeof(TrackLane))]
		[SerializeField]
		private TrackLane _blockedLanes;

		[Tooltip("Indicates obstacle and requried environment segment length - standard (180) or extended (360). Resets to 'standard' if not available for given obstacle type")]
		[SerializeField]
		private EnvironmentSegmentLength _obstacleLength = EnvironmentSegmentLength.Standard;

		[HideInInspector]
		[SerializeField]
		private EnvironmentConstraint _environmentConstraint;

		public EnvironmentObstacleType ObstacleType
		{
			get
			{
				return _obstacleType;
			}
		}

		public TrackLane BlockedLanes
		{
			get
			{
				return _blockedLanes;
			}
		}

		public EnvironmentSegmentLength ObstacleLength
		{
			get
			{
				return _obstacleLength;
			}
		}

		public EnvironmentConstraint EnvironmentConstraint
		{
			get
			{
				return _environmentConstraint;
			}
		}

		private void OnValidate()
		{
			switch (_obstacleType)
			{
			case EnvironmentObstacleType.Pillar:
				_blockedLanes = TrackLane.None;
				_obstacleLength = EnvironmentSegmentLength.Standard;
				_environmentConstraint.Type = EnvironmentType.Pillars;
				_environmentConstraint.SideDensity = EnvironmentDensity.Default;
				_environmentConstraint.SegmentLength = EnvironmentSegmentLength.Standard;
				break;
			case EnvironmentObstacleType.Station:
				_blockedLanes = TrackLane.Sides;
				_environmentConstraint.Type = EnvironmentType.Station;
				_environmentConstraint.SideDensity = EnvironmentDensity.Default;
				_environmentConstraint.SegmentLength = _obstacleLength;
				break;
			case EnvironmentObstacleType.Gates:
				_obstacleLength = EnvironmentSegmentLength.Extended;
				_environmentConstraint.Type = EnvironmentType.Gates;
				_environmentConstraint.SideDensity = EnvironmentDensity.Default;
				_environmentConstraint.SegmentLength = _obstacleLength;
				break;
			}
		}
	}
}
