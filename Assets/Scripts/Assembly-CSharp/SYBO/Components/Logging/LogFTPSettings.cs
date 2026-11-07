using System;
using SYBO.Components.Settings;
using UnityEngine;

namespace SYBO.Components.Logging
{
	[Serializable]
	public class LogFTPSettings
	{
		[Tooltip("Store the Log on in house ftp server (unless final build)")]
		public bool StoreLogsWithFtp = true;

		[Tooltip("The ftp URI pattern for log destination")]
		[DebugSetting]
		public string StoreLogsFtpUri = "ftp://ftp.sybogames.com/Logs/Brim/{0}/{1}";

		[Tooltip("How far ftp is allowed to fall behind, before discarding data")]
		public int FtpBuffers = 3;

		[Tooltip("Username")]
		[DebugSetting]
		public string Username = "brimlogsync";

		[Tooltip("Password")]
		[DebugSetting]
		public string Password = "brimlogsync";
	}
}
