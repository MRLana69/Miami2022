using UnityEngine;

namespace SYBO.Components.Settings
{
	public class RuntimeSettings
	{
		[HideInInspector]
		[Tooltip("Is this a final build (release or release candidate)")]
		public bool IsFinalBuild;
	}
}
