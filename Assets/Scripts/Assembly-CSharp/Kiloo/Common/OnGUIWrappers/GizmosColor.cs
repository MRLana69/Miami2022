using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GizmosColor : InternalColor
	{
		public GizmosColor(Color newColor)
		{
			base.previousColor = GUI.color;
			Gizmos.color = newColor;
		}

		public override void Dispose()
		{
			Gizmos.color = base.previousColor;
		}
	}
}
