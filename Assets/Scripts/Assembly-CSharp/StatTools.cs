using Data.Save;

public static class StatTools
{
	public static void SetValue(this Stat stat, int value)
	{
		PlayerSave.SetStatValue(stat, value);
	}

	public static int GetValue(this Stat stat)
	{
		return PlayerSave.GetStatValue(stat);
	}

	public static void IncrementValue(this Stat stat)
	{
		stat.AddValue(1);
	}

	public static void AddValue(this Stat stat, int value)
	{
		PlayerSave.SetStatValue(stat, PlayerSave.GetStatValue(stat) + value);
	}
}
