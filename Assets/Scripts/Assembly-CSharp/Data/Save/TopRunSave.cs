using TopRun.Internal;

namespace Data.Save
{
	public class TopRunSave : DataManager<TopRunSave>
	{
		public static ManagerData managerData
		{
			get
			{
				return DataManager<TopRunSave>.cloud.managerData;
			}
			set
			{
				DataManager<TopRunSave>.cloud.managerData = value;
			}
		}

		public static RanksData ranksData
		{
			get
			{
				return DataManager<TopRunSave>.local.ranksData;
			}
			set
			{
				DataManager<TopRunSave>.local.ranksData = value;
			}
		}

		public static PlayerIdsData playerIdsData
		{
			get
			{
				return DataManager<TopRunSave>.local.playerIdsData;
			}
			set
			{
				DataManager<TopRunSave>.local.playerIdsData = value;
			}
		}

		public static string lastTopRunResultAwardedID
		{
			get
			{
				return DataManager<TopRunSave>.cloud.lastTopRunResultAwardedID;
			}
			set
			{
				DataManager<TopRunSave>.cloud.lastTopRunResultAwardedID = value;
			}
		}

		public static string pendingTopRunResultID
		{
			get
			{
				return DataManager<TopRunSave>.cloud.pendingTopRunResultID;
			}
			set
			{
				DataManager<TopRunSave>.cloud.pendingTopRunResultID = value;
			}
		}

		public static int pendingTopRunResultsScore
		{
			get
			{
				return DataManager<TopRunSave>.cloud.pendingTopRunResultsScore;
			}
			set
			{
				DataManager<TopRunSave>.cloud.pendingTopRunResultsScore = value;
			}
		}

		public static int pendingTopRunResultsRank
		{
			get
			{
				return DataManager<TopRunSave>.cloud.pendingTopRunResultsRank;
			}
			set
			{
				DataManager<TopRunSave>.cloud.pendingTopRunResultsRank = value;
			}
		}

		public static int pendingTopRunResultsWeek
		{
			get
			{
				return DataManager<TopRunSave>.cloud.pendingTopRunResultsWeek;
			}
			set
			{
				DataManager<TopRunSave>.cloud.pendingTopRunResultsWeek = value;
			}
		}

		public static int lastWeekMedalWasShown
		{
			get
			{
				return DataManager<TopRunSave>.local.lastWeekMedalWasShown;
			}
			set
			{
				DataManager<TopRunSave>.local.lastWeekMedalWasShown = value;
			}
		}

		public static int pendingTopRunBeatenFriendsAward
		{
			get
			{
				return DataManager<TopRunSave>.cloud.pendingTopRunBeatenFriendsAward;
			}
			set
			{
				DataManager<TopRunSave>.cloud.pendingTopRunBeatenFriendsAward = value;
			}
		}
	}
}
