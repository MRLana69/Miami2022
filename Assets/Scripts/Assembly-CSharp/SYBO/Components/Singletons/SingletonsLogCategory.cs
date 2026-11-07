using SYBO.Components.Logging;

namespace SYBO.Components.Singletons
{
	public class SingletonsLogCategory : LogCategory
	{
		public SingletonsLogCategory(int categoryId)
			: base(categoryId)
		{
			CategoryText = "Singletons";
			Log.Singletons = this;
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[0];
		}
	}
}
