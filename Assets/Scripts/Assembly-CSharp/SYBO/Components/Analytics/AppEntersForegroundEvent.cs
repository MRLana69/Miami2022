namespace SYBO.Components.Analytics
{
	public class AppEntersForegroundEvent : BaseSessionEvent
	{
		public override string EventName
		{
			get
			{
				return "app_enters_foreground";
			}
		}
	}
}
