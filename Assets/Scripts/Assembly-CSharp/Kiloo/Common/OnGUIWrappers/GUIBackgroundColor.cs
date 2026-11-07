using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIBackgroundColor : InternalColor
	{
		public GUIBackgroundColor(Color newColor)
		{
			base.previousColor = GUI.backgroundColor;
			GUI.backgroundColor = newColor;
		}

		public override void Dispose()
		{
			GUI.backgroundColor = base.previousColor;
		}
	}
}
