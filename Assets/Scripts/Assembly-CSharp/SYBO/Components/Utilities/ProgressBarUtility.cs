namespace SYBO.Components.Utilities
{
	public static class ProgressBarUtility
	{
		public static bool IsAvailable()
		{
			return false;
		}

		public static bool HasProgressBar()
		{
			if (IsAvailable())
			{
			}
			return false;
		}

		public static void DisplayCancelableProgressBar(string title, string info, float progress)
		{
			if (!IsAvailable())
			{
			}
		}

		public static void DisplayProgressBar(string title, string info, float progress)
		{
			if (!IsAvailable())
			{
			}
		}

		public static void ClearProgressBar()
		{
			if (!IsAvailable())
			{
			}
		}

		public static void ClearProgressBarAfterDelay(float delayInSeconds)
		{
			if (!IsAvailable())
			{
			}
		}
	}
}
