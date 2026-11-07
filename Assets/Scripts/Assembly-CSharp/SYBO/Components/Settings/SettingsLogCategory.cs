using SYBO.Components.Logging;

namespace SYBO.Components.Settings
{
	public class SettingsLogCategory : LogCategory
	{
		public SettingsLogCategory(int categoryId)
			: base(categoryId)
		{
			CategoryText = "Settings";
			Log.Settings = this;
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.Settings." };
		}
	}
}
