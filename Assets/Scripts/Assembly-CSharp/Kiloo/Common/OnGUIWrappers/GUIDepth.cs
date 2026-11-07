using System;
using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIDepth : IDisposable
	{
		private int _previousDepth;

		public GUIDepth(int depth)
		{
			_previousDepth = GUI.depth;
			GUI.depth = depth;
		}

		public void Dispose()
		{
			GUI.depth = _previousDepth;
		}
	}
}
