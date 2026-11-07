using System;
using UnityEngine;

namespace Kiloo.Common.OnGUIWrappers
{
	public class GUIScrollView : IDisposable
	{
		public Vector2 Scroll { get; set; }

		public GUIScrollView(Vector2 scrollPosition, params GUILayoutOption[] options)
		{
			Scroll = GUILayout.BeginScrollView(scrollPosition, options);
		}

		public GUIScrollView(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
		{
			Scroll = GUILayout.BeginScrollView(scrollPosition, style, options);
		}

		public GUIScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollBar, GUIStyle verticalScrollBar, params GUILayoutOption[] options)
		{
			Scroll = GUILayout.BeginScrollView(scrollPosition, horizontalScrollBar, verticalScrollBar, options);
		}

		public void Dispose()
		{
			GUILayout.EndScrollView();
		}
	}
}
