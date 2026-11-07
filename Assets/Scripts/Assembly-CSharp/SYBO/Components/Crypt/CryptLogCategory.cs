using SYBO.Components.Logging;

namespace SYBO.Components.Crypt
{
	public class CryptLogCategory : LogCategory
	{
		public CryptLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.Crypt = this;
			CategoryText = "Crypt";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.Crypt." };
		}
	}
}
