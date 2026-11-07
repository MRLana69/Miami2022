using System;
using UnityEngine;

namespace SYBO.Components.Logging
{
	public interface ILogPlatform
	{
		void LogFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args);

		void LogWarningFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args);

		void LogErrorFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args);

		void LogFormat(LogCategory logCategory, int logLevel, string format, object[] args);

		void LogWarningFormat(LogCategory logCategory, int logLevel, string format, object[] args);

		void LogErrorFormat(LogCategory logCategory, int logLevel, string format, object[] args);

		void LogException(LogCategory logCategory, int logLevel, Exception exception);

		void LogLongString(LogCategory logCategory, int level, string data);
	}
}
