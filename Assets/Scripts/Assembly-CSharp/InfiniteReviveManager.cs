using ABTest;
using Info;
using Kiloo.Common.Extensions;

public static class InfiniteReviveManager
{
	private const int ONLINESETTINGS_KEYCAP_DEFAULT_VALUE = 1024;

	private const string ONLINESETTINGS_KEYCAP = "revive_key_cap";

	public static int keyCap
	{
		get
		{
			return OnlineSettings.instance.GetValue("revive_key_cap", 1024);
		}
	}

	public static bool CanBeOverriddenByOnlineSettings
	{
		get
		{
			return GetTestGroup() == "D";
		}
	}

	private static string GetTestGroup(string defaultValue = "D")
	{
		string payloadString = ABTestManager.GetPayloadString("infiniterevive");
		string[] array = payloadString.Split('_');
		if (!array[0].IsNullOrEmpty())
		{
			return array[0];
		}
		return defaultValue;
	}

	public static int GetMaxNumberOfRevives(int defaultValue)
	{
		string payloadString = ABTestManager.GetPayloadString("infiniterevive");
		string[] array = payloadString.Split('_');
		int result;
		if (int.TryParse(array[1], out result))
		{
			return result;
		}
		return defaultValue;
	}

	public static bool NeedsKeyIncreaseWhenUsingVideos()
	{
		if (GetTestGroup() == "B" && SaveMeManager.GetNumberOfKeysToSaveMe() < keyCap)
		{
			return true;
		}
		return false;
	}

	public static bool NeedsKeyIncreaseWhenUsingKeys()
	{
		if (GetTestGroup() == "B")
		{
			if (SaveMeManager.GetNumberOfKeysToSaveMe() < keyCap)
			{
				return true;
			}
			return false;
		}
		return true;
	}
}
