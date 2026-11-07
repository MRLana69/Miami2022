namespace Missions
{
	public class AwardStatGoal : AwardGoal
	{
		public Stat stat { get; set; }

		public AwardStatGoal()
		{
			base.hasIcon = false;
		}
	}
}
