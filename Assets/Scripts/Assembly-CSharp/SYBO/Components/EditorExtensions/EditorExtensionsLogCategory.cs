using SYBO.Components.Logging;

namespace SYBO.Components.EditorExtensions
{
	public class EditorExtensionsLogCategory : LogCategory
	{
		public EditorExtensionsLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.EditorExtensions = this;
			CategoryText = "EditorExtensions";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.EditorExtensions." };
		}
	}
}
