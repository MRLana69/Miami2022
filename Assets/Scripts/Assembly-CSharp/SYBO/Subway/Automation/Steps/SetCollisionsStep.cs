using SYBO.Common.Automation.Steps;
using UnityEngine;

namespace SYBO.Subway.Automation.Steps
{
	public class SetCollisionsStep : AutomationStep
	{
		private bool _enable;

		public SetCollisionsStep(bool enable)
		{
			_enable = enable;
		}

		public override void Update(float unscaledDeltaTime)
		{
			Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, !_enable);
			Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Pickup, !_enable);
			base.IsDone = true;
		}
	}
}
