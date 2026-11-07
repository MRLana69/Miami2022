using System;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Subway.Managers
{
	[PrefabPath("Prefabs/Managers/QualitySettingsManager")]
	public class QualitySettingsManager : SingletonMonoBehaviour<QualitySettingsManager>
	{
		public class QualityTypeChangedEventArgs : EventArgs
		{
			public QualityType QualityType { get; internal set; }

			public QualityTypeChangedEventArgs(QualityType qualityType)
			{
				QualityType = qualityType;
			}
		}

		private const string _QUALITY_LEVEL_NAME_GOOD = "Good";

		private const string _QUALITY_LEVEL_NAME_DEMO_VIDEO_MODE = "Demo Video Mode";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<QualityTypeChangedEventArgs> OnQualityTypeChanged__BackingField;

		public event EventHandler<QualityTypeChangedEventArgs> OnQualityTypeChanged
		{
			add
			{
				EventHandler<QualityTypeChangedEventArgs> eventHandler = OnQualityTypeChanged__BackingField;
				EventHandler<QualityTypeChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnQualityTypeChanged__BackingField, (EventHandler<QualityTypeChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<QualityTypeChangedEventArgs> eventHandler = OnQualityTypeChanged__BackingField;
				EventHandler<QualityTypeChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnQualityTypeChanged__BackingField, (EventHandler<QualityTypeChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			if (!DemoVideoModeManager.IsEnabled)
			{
			}
		}

		public void SetQualityType(QualityType qualityType)
		{
			switch (qualityType)
			{
			case QualityType.Default:
				SetQualityLevel("Good");
				break;
			case QualityType.DemoVideoMode:
				SetQualityLevel("Demo Video Mode");
				break;
			}
			RaiseOnQualityLevelChanged(new QualityTypeChangedEventArgs(qualityType));
		}

		private int GetQualityLevelIntFromName(string qualityLevelName)
		{
			string[] names = QualitySettings.names;
			for (int i = 0; i < names.Length; i++)
			{
				string text = names[i];
				if (text == qualityLevelName)
				{
					return i;
				}
			}
			return -1;
		}

		private bool HasQualityLevelIntFromName(string qualityLevelName)
		{
			return GetQualityLevelIntFromName(qualityLevelName) != -1;
		}

		private void SetQualityLevel(int qualityLevel)
		{
			QualitySettings.SetQualityLevel(qualityLevel, true);
			DebugLogQualitySettings("SetQualityLevel");
		}

		private void SetQualityLevel(string qualityLevelName)
		{
			if (HasQualityLevelIntFromName(qualityLevelName))
			{
				int qualityLevelIntFromName = GetQualityLevelIntFromName(qualityLevelName);
				SetQualityLevel(qualityLevelIntFromName);
			}
			else
			{
				SDebug.LogError("No qualityLevelInt available for qualityLevelName: " + qualityLevelName);
			}
		}

		private void DebugLogQualitySettings(string callingMethodName)
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
		}

		private void RaiseOnQualityLevelChanged(QualityTypeChangedEventArgs e)
		{
			if (OnQualityTypeChanged__BackingField != null)
			{
				OnQualityTypeChanged__BackingField(this, e);
			}
		}
	}
}
