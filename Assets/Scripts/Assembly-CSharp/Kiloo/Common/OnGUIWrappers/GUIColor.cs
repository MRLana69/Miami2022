using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIColor : InternalColor
	{
		public GUIColor(Color newColor)
		{
			base.previousColor = GUI.color;
			GUI.color = newColor;
		}

		public override void Dispose()
		{
			GUI.color = base.previousColor;
		}
	}
}
