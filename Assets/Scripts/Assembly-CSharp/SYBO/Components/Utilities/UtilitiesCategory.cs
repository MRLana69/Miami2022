using SYBO.Components.Logging;

namespace SYBO.Components.Utilities
{
	public class UtilitiesCategory : LogCategory
	{
		public UtilitiesCategory(int categoryId)
			: base(categoryId)
		{
			CategoryText = "Utilities";
			Log.Utilities = this;
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[2] { "SyboRandom:", "SYBO.Components.Utilities." };
		}
	}
}
