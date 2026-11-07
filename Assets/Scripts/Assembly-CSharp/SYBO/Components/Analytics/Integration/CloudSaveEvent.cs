using System;
using Kiloo.Common.Extensions;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class CloudSaveEvent : DeviceInfoEvent
	{
		[SerializeField]
		[SerializedAs("action_type")]
		public readonly int Action;

		[SerializeField]
		[SerializedSizeLimit(36, true)]
		[SerializedAs("target_uid")]
		public readonly string TargetUid;

		public new static Func<string> GetUserID;

		public override string EventName
		{
			get
			{
				return "cloud_save";
			}
		}

		public CloudSaveEvent(int action, string targetUid = null)
		{
			Action = action;
			TargetUid = targetUid;
			_userID = GetUserID.SafeInvoke();
		}

		public override string ToString()
		{
			return string.Format("\n uid: {0} \n action: {1}\n target_uid: {2} \n client_version: {3}", base.UserId, Action, TargetUid, base.GameVersion);
		}
	}
}
