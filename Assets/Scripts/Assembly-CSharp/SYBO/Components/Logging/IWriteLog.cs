using System;
using UnityEngine;

namespace SYBO.Components.Logging
{
	public interface IWriteLog
	{
		void LogFormat(int level, string format);

		void LogFormat(int level, string format, object arg1);

		void LogFormat(int level, string format, object arg1, object arg2);

		void LogFormat(int level, string format, object arg1, object arg2, object arg3);

		void LogFormat(int level, string format, params object[] args);

		void LogLongString(int level, string data);

		void LogWarningFormat(int level, string format);

		void LogWarningFormat(int level, string format, object arg1);

		void LogWarningFormat(int level, string format, object arg1, object arg2);

		void LogWarningFormat(int level, string format, object arg1, object arg2, object arg3);

		void LogWarningFormat(int level, string format, params object[] args);

		void LogErrorFormat(int level, string format);

		void LogErrorFormat(int level, string format, object arg1);

		void LogErrorFormat(int level, string format, object arg1, object arg2);

		void LogErrorFormat(int level, string format, object arg1, object arg2, object arg3);

		void LogErrorFormat(int level, string format, params object[] args);

		void LogException(Exception exception);

		void LogException(int level, Exception exception);

		void LogFormat(UnityEngine.Object context, int level, string format);

		void LogFormat(UnityEngine.Object context, int level, string format, object arg1);

		void LogFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2);

		void LogFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3);

		void LogFormat(UnityEngine.Object context, int level, string format, params object[] args);

		void LogWarningFormat(UnityEngine.Object context, int level, string format);

		void LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1);

		void LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2);

		void LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3);

		void LogWarningFormat(UnityEngine.Object context, int level, string format, params object[] args);

		void LogErrorFormat(UnityEngine.Object context, int level, string format);

		void LogErrorFormat(UnityEngine.Object context, int level, string format, object arg1);

		void LogErrorFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2);

		void LogErrorFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3);

		void LogErrorFormat(UnityEngine.Object context, int level, string format, params object[] args);
	}
}
