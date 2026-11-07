using System;
using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIHorizontal : IDisposable
	{
		public GUIHorizontal()
		{
			GUILayout.BeginHorizontal();
		}

		public GUIHorizontal(GUIStyle style, params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginHorizontal(style, layoutOptions);
		}

		public GUIHorizontal(params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginHorizontal(layoutOptions);
		}

		public GUIHorizontal(string text, GUIStyle style, params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginHorizontal(text, style, layoutOptions);
		}

		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}
