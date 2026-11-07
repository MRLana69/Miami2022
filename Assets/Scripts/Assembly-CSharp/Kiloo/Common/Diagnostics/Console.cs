using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common.Diagnostics
{
	public class Console : MonoBehaviour
	{
		private struct Log
		{
			public string message;

			public string stackTrace;

			public LogType type;
		}

		public KeyCode toggleKey = KeyCode.BackQuote;

		private List<Log> logs = new List<Log>();

		private Vector2 scrollPosition;

		public bool show;

		private bool collapse;

		private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
		{
			{
				LogType.Assert,
				Color.white
			},
			{
				LogType.Error,
				Color.red
			},
			{
				LogType.Exception,
				Color.red
			},
			{
				LogType.Log,
				Color.white
			},
			{
				LogType.Warning,
				Color.yellow
			}
		};

		private const int margin = 20;

		private Rect windowRect = new Rect(20f, 20f, Screen.width - 40, Screen.height - 40);

		private Rect titleBarRect = new Rect(0f, 0f, 10000f, 20f);

		private GUIContent hideLabel = new GUIContent("Hide", "Hide the console window");

		private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");

		private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

		[UsedImplicitly]
		private void OnEnable()
		{
			Application.logMessageReceived += HandleLog;
		}

		[UsedImplicitly]
		private void OnDisable()
		{
			Application.logMessageReceived -= HandleLog;
		}

		[UsedImplicitly]
		private void Update()
		{
		}

		[UsedImplicitly]
		private void OnGUI()
		{
			if (show)
			{
				windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
			}
		}

		public void ConsoleWindow(int windowID)
		{
			ConsoleView(true);
			GUI.DragWindow(titleBarRect);
		}

		public void ConsoleView(bool showHideButton)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for (int i = 0; i < logs.Count; i++)
			{
				Log log = logs[i];
				if (!collapse || i <= 0 || !(log.message == logs[i - 1].message))
				{
					GUI.contentColor = logTypeColors[log.type];
					GUILayout.Label(log.message);
				}
			}
			GUILayout.EndScrollView();
			GUI.contentColor = Color.white;
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(clearLabel))
			{
				logs.Clear();
			}
			if (showHideButton && GUILayout.Button(hideLabel))
			{
				show = false;
			}
			collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
		}

		private void HandleLog(string message, string stackTrace, LogType type)
		{
			logs.Add(new Log
			{
				message = message,
				stackTrace = stackTrace,
				type = type
			});
		}
	}
}
