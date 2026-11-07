using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class JoinedABTestEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("test_tag")]
		[SerializedSizeLimit(16, false)]
		public readonly string Tag;

		[SerializeField]
		[SerializedAs("segment_index")]
		public readonly int SegmentIndex;

		public override string EventName
		{
			get
			{
				return "ab_test_segment_joined";
			}
		}

		public JoinedABTestEvent(string tag, int segmentIndex)
		{
			Tag = tag;
			SegmentIndex = segmentIndex;
		}
	}
}
