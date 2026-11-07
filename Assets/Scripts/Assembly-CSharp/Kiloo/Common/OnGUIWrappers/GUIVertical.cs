using System;
using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIVertical : IDisposable
	{
		public GUIVertical()
		{
			GUILayout.BeginVertical();
		}

		public GUIVertical(params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginVertical(layoutOptions);
		}

		public GUIVertical(GUIStyle style, params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginVertical(style, layoutOptions);
		}

		public GUIVertical(string text, GUIStyle style, params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginVertical(text, style, layoutOptions);
		}

		public void Dispose()
		{
			GUILayout.EndVertical();
		}
	}
}
