using System;
using UnityEngine;

namespace SYBO.Components.Logging
{
	public class LogPlatformDummy : ILogPlatform
	{
		void ILogPlatform.LogFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args)
		{
		}

		void ILogPlatform.LogWarningFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args)
		{
		}

		void ILogPlatform.LogErrorFormat(LogCategory logCategory, int logLevel, UnityEngine.Object context, string format, object[] args)
		{
		}

		void ILogPlatform.LogFormat(LogCategory logCategory, int logLevel, string format, object[] args)
		{
		}

		void ILogPlatform.LogWarningFormat(LogCategory logCategory, int logLevel, string format, object[] args)
		{
		}

		void ILogPlatform.LogErrorFormat(LogCategory logCategory, int logLevel, string format, object[] args)
		{
		}

		void ILogPlatform.LogLongString(LogCategory logCategory, int level, string data)
		{
		}

		void ILogPlatform.LogException(LogCategory logCategory, int logLevel, Exception exception)
		{
		}
	}
}
