using System;
using System.Collections.Generic;
using SYBO.Components.Settings;
using UnityEngine;
using UnityEngine.Events;

namespace SYBO.Components.Logging
{
	[Serializable]
	public class LogSettings
	{
		[Tooltip("Memory budget in MB")]
		public float MemoryBudget = 0.1f;

		[Tooltip("Log Categories")]
		public Dictionary<string, LogCategorySettings> Categories = new Dictionary<string, LogCategorySettings>();

		[Tooltip("The file path to the log destination")]
		[DebugSetting]
		public string LogsPathWin = "\\\\sybonas.sybogames.local\\Logs\\Brim\\UnityLogs\\{0}";

		[Tooltip("Ftp Settings")]
		[DebugSetting]
		public LogFTPSettings Ftp = new LogFTPSettings();

		[NonSerialized]
		public UnityEvent Changed;

		[Tooltip("Clear the Log when a new scene is loaded")]
		public bool ClearOnNewSceneLoaded;

		public int GetLogLevel(string categoryName)
		{
			if (!HasLogLevel(categoryName))
			{
				return 0;
			}
			LogCategorySettings value;
			if (Categories.TryGetValue(categoryName, out value))
			{
				return value.logLevel;
			}
			return 0;
		}

		public bool HasLogLevel(string categoryName)
		{
			LogCategorySettings value;
			if (Categories != null)
			{
				return Categories.TryGetValue(categoryName, out value);
			}
			return false;
		}
	}
}
