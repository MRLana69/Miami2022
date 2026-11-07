using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class SDebug
{
	private const int UNITY_LOG_MAXIMUM_CHAR_LIMIT = 15500;

	[Conditional("ENABLE_LOGGING_LOG")]
	public static void Log(object message, UnityEngine.Object context = null)
	{
		string fullLog = message.ToString();
		if (MessageExceedsLimit(fullLog))
		{
			LogExtendedMessage(fullLog, context);
		}
		else
		{
			Debug.Log(message, context);
		}
	}

	private static bool MessageExceedsLimit(string fullLog)
	{
		return fullLog.Length > 15500;
	}

	private static void LogExtendedMessage(string fullLog, UnityEngine.Object context = null)
	{
		int num = 0;
		int num2 = 0;
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		while (!flag)
		{
			int num3 = 15500;
			if (num + num3 > fullLog.Length)
			{
				num3 = fullLog.Length - num;
				flag = true;
			}
			else
			{
				string text = fullLog.Substring(num, 15500);
				if (text.LastIndexOf(Environment.NewLine) != -1 && text.LastIndexOf(Environment.NewLine) > 5000)
				{
					num3 = text.LastIndexOf(Environment.NewLine) + 1;
				}
				else if (text.LastIndexOf(' ') != -1 && text.LastIndexOf(Environment.NewLine) > 5000)
				{
					num3 = text.LastIndexOf(' ') + 1;
				}
			}
			stringBuilder.Length = 0;
			stringBuilder.Append("<size=9><color=grey><b>(Extended log)</b> Part </color><color=silver><b>");
			stringBuilder.Append(num2 + 1);
			stringBuilder.Append("</b></color><color=grey> from index </color><color=olive><b>");
			stringBuilder.Append(string.Format("{0:n0}", num));
			stringBuilder.Append("</b></color><color=grey>/</color><color=silver>");
			stringBuilder.Append(string.Format("{0:n0}", fullLog.Length));
			stringBuilder.Append("</color><color=grey> with a length of </color><color=silver><b>");
			stringBuilder.Append(string.Format("{0:n0}", num3));
			stringBuilder.Append("</b></color><color=grey> characters:</color></size>");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(fullLog.Substring(num, num3));
			StackTraceLogType stackTraceLogType = Application.GetStackTraceLogType(LogType.Log);
			if (!flag)
			{
				Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			}
			Debug.Log(stringBuilder, context);
			Application.SetStackTraceLogType(LogType.Log, stackTraceLogType);
			num += num3 + 1;
			num2++;
		}
	}

	[Conditional("ENABLE_LOGGING_WARNING")]
	public static void LogWarning(object message, UnityEngine.Object context = null)
	{
		Debug.LogWarning(message, context);
	}

	public static void LogError(object message, UnityEngine.Object context = null)
	{
		Debug.LogError(message, context);
	}

	[StringFormatMethod("msg")]
	[Conditional("ENABLE_LOGGING_LOG")]
	public static void LogFormat(UnityEngine.Object context, string msg, params object[] args)
	{
		Debug.LogFormat(context, msg, args);
	}

	[StringFormatMethod("msg")]
	[Conditional("ENABLE_LOGGING_LOG")]
	public static void LogFormat(string msg, params object[] args)
	{
		Debug.LogFormat(msg, args);
	}

	[StringFormatMethod("msg")]
	[Conditional("ENABLE_LOGGING_WARNING")]
	public static void LogWarningFormat(UnityEngine.Object context, string msg, params object[] args)
	{
		Debug.LogWarningFormat(context, msg, args);
	}

	[StringFormatMethod("msg")]
	[Conditional("ENABLE_LOGGING_WARNING")]
	public static void LogWarningFormat(string msg, params object[] args)
	{
		Debug.LogWarningFormat(msg, args);
	}

	[StringFormatMethod("msg")]
	public static void LogErrorFormat(UnityEngine.Object context, string msg, params object[] args)
	{
		Debug.LogErrorFormat(context, msg, args);
	}

	[StringFormatMethod("msg")]
	public static void LogErrorFormat(string msg, params object[] args)
	{
		Debug.LogErrorFormat(msg, args);
	}

	public static void LogAssertion(object message)
	{
	}

	public static void LogAssertion(object message, UnityEngine.Object context)
	{
	}

	[StringFormatMethod("format")]
	public static void LogAssertionFormat(string format, params object[] args)
	{
	}

	[StringFormatMethod("format")]
	public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
	{
	}

	public static void LogException(Exception exception)
	{
		Debug.LogException(exception);
	}

	public static void LogException(Exception exception, UnityEngine.Object context)
	{
		Debug.LogException(exception, context);
	}
}
