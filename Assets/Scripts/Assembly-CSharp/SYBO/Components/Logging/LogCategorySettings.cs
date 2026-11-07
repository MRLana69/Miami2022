using System;
using UnityEngine;

namespace SYBO.Components.Logging
{
	[Serializable]
	public class LogCategorySettings
	{
		[HideInInspector]
		public string categoryText;

		[HideInInspector]
		public string categoryClassName;

		[Tooltip("Pause the Unity Editor on errors in this category")]
		public bool pauseOnError;

		[Tooltip("Pause the Unity Editor on warnings in this category")]
		public bool pauseOnWarning;

		[Tooltip("The Logging level for this category")]
		public int logLevel;

		[Tooltip("The Logging level for this category")]
		public int hideStackLevel;

		[Tooltip("Prefix that's added to all Log Messages in this category")]
		public string messagePrefix;

		[HideInInspector]
		public bool hasInitializedMessagePrefix;
	}
}
