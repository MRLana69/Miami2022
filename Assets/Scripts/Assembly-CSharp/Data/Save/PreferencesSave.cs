namespace Data.Save
{
	public class PreferencesSave : DataManager<PreferencesSave>
	{
		public static bool neverAskForRating
		{
			get
			{
				return DataManager<PreferencesSave>.cloud.neverAskForRating;
			}
			set
			{
				DataManager<PreferencesSave>.cloud.neverAskForRating = value;
			}
		}

		public static string language
		{
			get
			{
				return DataManager<PreferencesSave>.cloud.language;
			}
			set
			{
				DataManager<PreferencesSave>.cloud.language = value;
			}
		}

		public static bool soundEffectsEnabled
		{
			get
			{
				return DataManager<PreferencesSave>.cloud.soundEffectsEnabled;
			}
			set
			{
				DataManager<PreferencesSave>.cloud.soundEffectsEnabled = value;
			}
		}

		public static bool musicEnabled
		{
			get
			{
				return DataManager<PreferencesSave>.cloud.musicEnabled;
			}
			set
			{
				DataManager<PreferencesSave>.cloud.musicEnabled = value;
			}
		}

		public static bool topRunChallengersEnabled
		{
			get
			{
				return DataManager<PreferencesSave>.cloud.topRunChallengersEnabled;
			}
			set
			{
				DataManager<PreferencesSave>.cloud.topRunChallengersEnabled = value;
			}
		}

		public static bool remoteNotificationsEnabled
		{
			get
			{
				return DataManager<PreferencesSave>.cloud.remoteNotificationsEnabled;
			}
			set
			{
				DataManager<PreferencesSave>.cloud.remoteNotificationsEnabled = value;
			}
		}

		public static int localNotificationsEnabled
		{
			get
			{
				return DataManager<PreferencesSave>.cloud.localNotificationsEnabled;
			}
			set
			{
				DataManager<PreferencesSave>.cloud.localNotificationsEnabled = value;
			}
		}

		public static bool cloudSaveOptedOut
		{
			get
			{
				return DataManager<PreferencesSave>.local.cloudSaveOptedOut;
			}
			set
			{
				DataManager<PreferencesSave>.local.cloudSaveOptedOut = value;
			}
		}
	}
}
