using SYBO.Common.Automation.Samplers;

namespace SYBO.Subway.Automation.Samplers
{
	public class InGameIntervalSampler : IntervalSampler
	{
		public InGameIntervalSampler(float sampleInterval)
			: base(sampleInterval)
		{
		}

		public override bool Update(float unscaledDeltaTime)
		{
			if (!Game.IsInstantiated || !Game.Instance.IsInGame.Value)
			{
				_timeSinceLastSample = 0f;
				return false;
			}
			return base.Update(unscaledDeltaTime);
		}
	}
}
