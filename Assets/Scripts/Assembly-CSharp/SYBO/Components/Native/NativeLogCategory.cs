using SYBO.Components.Logging;

namespace SYBO.Components.Native
{
	public class NativeLogCategory : LogCategory
	{
		public NativeLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.Native = this;
			CategoryText = "Native";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[0];
		}
	}
}
