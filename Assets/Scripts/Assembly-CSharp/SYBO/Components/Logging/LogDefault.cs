namespace SYBO.Components.Logging
{
	public class LogDefault : LogCategory
	{
		public LogDefault(int categoryId)
			: base(categoryId)
		{
			Log.Default = this;
			CategoryText = "Default";
		}

		public override string[] GetCallstackPrefixes()
		{
			return new string[0];
		}
	}
}
