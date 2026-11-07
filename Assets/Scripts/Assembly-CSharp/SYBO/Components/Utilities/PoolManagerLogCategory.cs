using SYBO.Components.Logging;

namespace SYBO.Components.Utilities
{
	public class PoolManagerLogCategory : LogCategory
	{
		public PoolManagerLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.PoolManager = this;
			CategoryText = "Pool";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[0];
		}
	}
}
