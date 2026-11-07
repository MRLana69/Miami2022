using System;
using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIEnable : IDisposable
	{
		private bool PreviousState { get; set; }

		public GUIEnable(bool newState)
		{
			PreviousState = GUI.enabled;
			GUI.enabled = newState;
		}

		public void Dispose()
		{
			GUI.enabled = PreviousState;
		}
	}
}
