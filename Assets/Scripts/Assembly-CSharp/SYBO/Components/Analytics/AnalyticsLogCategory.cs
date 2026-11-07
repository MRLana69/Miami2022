using SYBO.Components.Logging;

namespace SYBO.Components.Analytics
{
	public class AnalyticsLogCategory : LogCategory
	{
		public AnalyticsLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.Analytics = this;
			CategoryText = "Analytics";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.Analytics." };
		}
	}
}
