using SYBO.Components.Logging;

namespace SYBO.Components.BuildSizeMonitor
{
	public class BuildSizeMonitorLogCategory : LogCategory
	{
		public BuildSizeMonitorLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.BuildSizeMonitor = this;
			CategoryText = "BuildSizeMonitor";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.BuildSizeMonitor." };
		}
	}
}
