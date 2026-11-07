using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class MissionCompletedEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedSizeLimit(64, false)]
		[SerializedAs("mission_set")]
		public readonly string MissionSet;

		[SerializeField]
		[SerializedAs("mission_index")]
		public readonly short MissionIndex;

		[SerializeField]
		[SerializedAs("source_id")]
		public readonly long SourceId;

		public override string EventName
		{
			get
			{
				return "mission_completed";
			}
		}

		public MissionCompletedEvent(string missionSet, short missionIndex, long sourceId)
		{
			MissionSet = missionSet;
			MissionIndex = missionIndex;
			SourceId = sourceId;
		}
	}
}
