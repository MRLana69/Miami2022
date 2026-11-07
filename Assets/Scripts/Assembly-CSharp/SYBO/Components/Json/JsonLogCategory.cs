using SYBO.Components.Logging;

namespace SYBO.Components.Json
{
	public class JsonLogCategory : LogCategory
	{
		public JsonLogCategory(int categoryId)
			: base(categoryId)
		{
			CategoryText = "Json";
			Log.Json = this;
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.Json." };
		}
	}
}
