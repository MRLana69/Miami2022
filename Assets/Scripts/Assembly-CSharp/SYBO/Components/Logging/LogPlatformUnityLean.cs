using System;
using UnityEngine;

namespace SYBO.Components.Logging
{
	public class LogPlatformUnityLean : ILogPlatform
	{
		void ILogPlatform.LogFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args)
		{
			if (logCategory.Level >= logLevel)
			{
				if (logCategory.Level - logCategory.HideStackLevel < logLevel)
				{
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
					Debug.LogFormat(context, logCategory.MessagePrefix + format, args);
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
				}
				else
				{
					Debug.LogFormat(context, logCategory.MessagePrefix + format, args);
				}
			}
		}

		void ILogPlatform.LogWarningFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args)
		{
			if (logCategory.Level >= logLevel)
			{
				if (logCategory.Level - logCategory.HideStackLevel < logLevel)
				{
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
					Debug.LogWarningFormat(context, logCategory.MessagePrefix + format, args);
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
				}
				else
				{
					Debug.LogWarningFormat(context, logCategory.MessagePrefix + format, args);
				}
			}
		}

		void ILogPlatform.LogErrorFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args)
		{
			if (logCategory.Level >= logLevel)
			{
				if (logCategory.Level - logCategory.HideStackLevel < logLevel)
				{
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
					Debug.LogErrorFormat(context, logCategory.MessagePrefix + format, args);
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
				}
				else
				{
					Debug.LogErrorFormat(context, logCategory.MessagePrefix + format, args);
				}
			}
		}

		void ILogPlatform.LogFormat(LogCategory logCategory, int logLevel, string format, object[] args)
		{
			if (logCategory.Level >= logLevel)
			{
				if (logCategory.Level - logCategory.HideStackLevel < logLevel)
				{
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
					Debug.LogFormat(logCategory.MessagePrefix + format, args);
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
				}
				else
				{
					Debug.LogFormat(logCategory.MessagePrefix + format, args);
				}
			}
		}

		void ILogPlatform.LogWarningFormat(LogCategory logCategory, int logLevel, string format, object[] args)
		{
			if (logCategory.Level >= logLevel)
			{
				if (logCategory.Level - logCategory.HideStackLevel < logLevel)
				{
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
					Debug.LogWarningFormat(logCategory.MessagePrefix + format, args);
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
				}
				else
				{
					Debug.LogWarningFormat(logCategory.MessagePrefix + format, args);
				}
			}
		}

		void ILogPlatform.LogErrorFormat(LogCategory logCategory, int logLevel, string format, object[] args)
		{
			if (logCategory.Level >= logLevel)
			{
				if (logCategory.Level - logCategory.HideStackLevel < logLevel)
				{
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
					Debug.LogErrorFormat(logCategory.MessagePrefix + format, args);
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
				}
				else
				{
					Debug.LogErrorFormat(logCategory.MessagePrefix + format, args);
				}
			}
		}

		void ILogPlatform.LogLongString(LogCategory logCategory, int level, string data)
		{
			if (logCategory.Level < level)
			{
				return;
			}
			if (logCategory.Level - logCategory.HideStackLevel < level)
			{
				Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			}
			if (data.Length > 768)
			{
				int num = 0;
				int num2 = num + 768;
				while (num2 < data.Length)
				{
					int num3 = data.LastIndexOf('\n', num2);
					if (num3 >= 0)
					{
						Debug.Log(data.Substring(num, num3 - num));
						num = num3 + 1;
						num2 = num + 768;
						continue;
					}
					break;
				}
				Debug.Log(data.Substring(num));
			}
			else
			{
				Debug.Log(data);
			}
			if (logCategory.Level - logCategory.HideStackLevel < level)
			{
				Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
			}
		}

		void ILogPlatform.LogException(LogCategory logCategory, int logLevel, Exception exception)
		{
			if (logCategory.Level >= logLevel)
			{
				if (logCategory.Level - logCategory.HideStackLevel < logLevel)
				{
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
					Debug.LogException(exception);
					Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
				}
				else
				{
					Debug.LogException(exception);
				}
			}
		}
	}
}
