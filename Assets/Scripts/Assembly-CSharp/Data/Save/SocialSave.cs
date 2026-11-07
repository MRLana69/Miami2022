using System.Collections.Generic;

namespace Data.Save
{
	public class SocialSave : DataManager<SocialSave>
	{
		public static Dictionary<string, Friend.Status> friendStatus
		{
			get
			{
				return DataManager<SocialSave>.cloud.friendStatus;
			}
			set
			{
				DataManager<SocialSave>.cloud.friendStatus = value;
			}
		}
	}
}
