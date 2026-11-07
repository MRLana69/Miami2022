using System;
using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIArea : IDisposable
	{
		public GUIArea(Rect area)
		{
			GUILayout.BeginArea(area);
		}

		public GUIArea(Rect area, string content)
		{
			GUILayout.BeginArea(area, content);
		}

		public GUIArea(Rect area, string content, string style)
		{
			GUILayout.BeginArea(area, content, style);
		}

		public void Dispose()
		{
			GUILayout.EndArea();
		}
	}
}
