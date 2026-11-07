namespace Missions
{
	public class AwardTier
	{
		public AwardTierType tierType { get; set; }

		public int reward { get; set; }

		public AwardGoal[] listGoals { get; set; }
	}
}
