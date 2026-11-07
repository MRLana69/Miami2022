public struct Mission
{
	public MissionType type;

	public int goal;

	public Mission(MissionType type, int goal)
	{
		this.type = type;
		this.goal = goal;
	}
}
