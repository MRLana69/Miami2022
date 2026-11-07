using TopRun.Internal;

namespace TopRun
{
	public static class TopRunRoot
	{
		public static IManager manager { get; private set; }

		public static void Init()
		{
			if (manager == null)
			{
				manager = new Manager();
			}
		}

		public static void ClearSavedData()
		{
			Manager.ClearSavedData();
		}
	}
}
