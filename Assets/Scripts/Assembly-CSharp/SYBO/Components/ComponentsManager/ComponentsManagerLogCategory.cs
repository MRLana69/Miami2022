using SYBO.Components.Logging;

namespace SYBO.Components.ComponentsManager
{
	public class ComponentsManagerLogCategory : LogCategory
	{
		public ComponentsManagerLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.ComponentsManager = this;
			CategoryText = "ComponentsManager";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.ComponentsManager." };
		}
	}
}
