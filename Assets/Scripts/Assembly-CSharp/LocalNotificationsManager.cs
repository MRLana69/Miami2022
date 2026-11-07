using System;

public class LocalNotificationsManager
{
	public const int TIME_OF_NOTIFICATION = 17;

	public const int TIME_OF_NOTIFICATION_DAILY_VIDEO = 19;

	public const string ONLINE_SETTING = "enable_local_notifications";

	public static DateTime GetNextNotificationTimeForVideos()
	{
		return DateTime.Today.AddHours(19.0);
	}
}
