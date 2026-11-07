using SYBO.Components.Logging;

namespace SYBO.Components.Compression
{
	public class CompressionLogCategory : LogCategory
	{
		public CompressionLogCategory(int categoryId)
			: base(categoryId)
		{
			Log.Compression = this;
			CategoryText = "Compression";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[1] { "SYBO.Components.Compression." };
		}
	}
}
