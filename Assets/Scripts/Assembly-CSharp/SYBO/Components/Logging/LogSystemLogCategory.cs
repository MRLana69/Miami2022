namespace SYBO.Components.Logging
{
	public class LogSystemLogCategory : LogCategory
	{
		public LogSystemLogCategory(int categoryId)
			: base(categoryId)
		{
			CategoryText = "Log";
			Log.LogSystem = this;
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "LogManager:" };
		}
	}
}
