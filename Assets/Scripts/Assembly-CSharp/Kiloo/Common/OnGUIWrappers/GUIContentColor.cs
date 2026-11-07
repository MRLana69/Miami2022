using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIContentColor : InternalColor
	{
		public GUIContentColor(Color newColor)
		{
			base.previousColor = GUI.contentColor;
			GUI.contentColor = newColor;
		}

		public override void Dispose()
		{
			GUI.contentColor = base.previousColor;
		}
	}
}
