using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Subway.Managers
{
	public class DemoVideoModeManager
	{
		public const string DEMO_VIDEO_MODE_IS_ENABLED = "DEMO_VIDEO_MODE_IS_ENABLED";

		public static bool IsEnabled
		{
			get
			{
				int num = PlayerPrefs.GetInt("DEMO_VIDEO_MODE_IS_ENABLED", 0);
				return num == 1;
			}
			set
			{
				if (Application.isPlaying)
				{
					SDebug.LogError("Demo Video Mode can be toggled in edit-mode only.");
					return;
				}
				if (value)
				{
					PlayerPrefs.SetInt("DEMO_VIDEO_MODE_IS_ENABLED", 1);
				}
				else
				{
					PlayerPrefs.SetInt("DEMO_VIDEO_MODE_IS_ENABLED", 0);
				}
				if (value)
				{
					SingletonMonoBehaviour<QualitySettingsManager>.Instance.SetQualityType(QualityType.DemoVideoMode);
				}
				else
				{
					SingletonMonoBehaviour<QualitySettingsManager>.Instance.SetQualityType(QualityType.Default);
				}
			}
		}

		public static void RefreshIsEnabled()
		{
			IsEnabled = IsEnabled;
		}
	}
}
