using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class DevSessionStartedEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("online_settings")]
		public readonly string OnlineSettings;

		[SerializeField]
		[SerializedSizeLimit(256, true)]
		[SerializedAs("svn_branch")]
		public readonly string Branch;

		[SerializeField]
		[SerializedAs("svn_revision")]
		public readonly int Revision;

		[SerializeField]
		[SerializedAs("local_modifications")]
		public readonly int Modifications;

		public override string EventName
		{
			get
			{
				return "dev_session_started";
			}
		}

		public DevSessionStartedEvent(string onlineSettings, string branch, int revision, bool modifications)
		{
			OnlineSettings = onlineSettings;
			Branch = branch;
			Revision = revision;
			Modifications = (modifications ? 1 : 0);
		}
	}
}
