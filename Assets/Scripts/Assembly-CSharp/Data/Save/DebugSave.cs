namespace Data.Save
{
	public class DebugSave : DataManager<DebugSave>
	{
		public static SaveDataStrings debug_saveData
		{
			get
			{
				return DataManager<DebugSave>.instance.save.debug_dataSerialized;
			}
			set
			{
				DataManager<DebugSave>.instance.save.debug_dataSerialized = value;
			}
		}

		public static int overridePerformanceLevel
		{
			get
			{
				return DataManager<DebugSave>.localDebug.overridePerformanceLevel;
			}
			set
			{
				DataManager<DebugSave>.localDebug.overridePerformanceLevel = value;
			}
		}

		public static bool disableAdSystem
		{
			get
			{
				return DataManager<DebugSave>.localDebug.disableAdSystem;
			}
			set
			{
				DataManager<DebugSave>.localDebug.disableAdSystem = value;
			}
		}

		public static bool useLocalTimeInsteadOfServerTime
		{
			get
			{
				return DataManager<DebugSave>.localDebug.useLocalTimeInsteadOfServerTime;
			}
			set
			{
				DataManager<DebugSave>.localDebug.useLocalTimeInsteadOfServerTime = value;
			}
		}
	}
}
