using System;
using System.Diagnostics;
using UnityEngine;

namespace SYBO.Components.Logging
{
	public abstract class LogCategory : IWriteLog
	{
		public readonly int CategoryId;

		public string CategoryText;

		public string MessagePrefix;

		public int Level;

		public int HideStackLevel;

		protected static readonly object[] _zeroArgs = new object[0];

		[ThreadStatic]
		protected static object[] _singleArgThreadStatic = null;

		[ThreadStatic]
		protected static object[] _twoArgsThreadStatic = null;

		[ThreadStatic]
		protected static object[] _threeArgsThreadStatic = null;

		public static ILogPlatform LogEngine { get; set; }

		protected static object[] _singleArg
		{
			get
			{
				if (_singleArgThreadStatic == null)
				{
					_singleArgThreadStatic = new object[1];
				}
				return _singleArgThreadStatic;
			}
		}

		protected static object[] _twoArgs
		{
			get
			{
				if (_twoArgsThreadStatic == null)
				{
					_twoArgsThreadStatic = new object[2];
				}
				return _twoArgsThreadStatic;
			}
		}

		protected static object[] _threeArgs
		{
			get
			{
				if (_threeArgsThreadStatic == null)
				{
					_threeArgsThreadStatic = new object[3];
				}
				return _threeArgsThreadStatic;
			}
		}

		public LogCategory(int categoryId)
		{
			CategoryId = categoryId;
		}

		public virtual string[] GetCallstackPrefixes()
		{
			return new string[0];
		}

		public bool ApplySetting(LogCategorySettings setting)
		{
			bool result = false;
			string name = GetType().Name;
			if (setting.categoryClassName != name)
			{
				setting.categoryClassName = name;
				result = true;
			}
			if (setting.categoryText != CategoryText)
			{
				setting.categoryText = CategoryText;
				result = true;
			}
			if (!setting.hasInitializedMessagePrefix)
			{
				setting.hasInitializedMessagePrefix = true;
				setting.messagePrefix = CategoryText + " - ";
				result = true;
			}
			Level = setting.logLevel;
			HideStackLevel = setting.hideStackLevel;
			MessagePrefix = setting.messagePrefix;
			return result;
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(string format)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(string format, params object[] args)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(int level, string format)
		{
			LogEngine.LogFormat(this, level, format, _zeroArgs);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(int level, string format, object arg1)
		{
			_singleArg[0] = arg1;
			LogEngine.LogFormat(this, level, format, _singleArg);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(int level, string format, object arg1, object arg2)
		{
			_twoArgs[0] = arg1;
			_twoArgs[1] = arg2;
			LogEngine.LogFormat(this, level, format, _twoArgs);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(int level, string format, object arg1, object arg2, object arg3)
		{
			_threeArgs[0] = arg1;
			_threeArgs[1] = arg2;
			_threeArgs[2] = arg3;
			LogEngine.LogFormat(this, level, format, _threeArgs);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(int level, string format, params object[] args)
		{
			LogEngine.LogFormat(this, level, format, args);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(Enum level, string format)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(Enum level, string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(Enum level, string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(Enum level, string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(Enum level, string format, params object[] args)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogLongString(int level, string data)
		{
			LogEngine.LogLongString(this, level, data);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogLongString(Enum level, string data)
		{
			LogEngine.LogLongString(this, Convert.ToInt32(level), data);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(string format)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(string format, params object[] args)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(int level, string format)
		{
			LogEngine.LogWarningFormat(this, level, format, _zeroArgs);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(int level, string format, object arg1)
		{
			_singleArg[0] = arg1;
			LogEngine.LogWarningFormat(this, level, format, _singleArg);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(int level, string format, object arg1, object arg2)
		{
			_twoArgs[0] = arg1;
			_twoArgs[1] = arg2;
			LogEngine.LogWarningFormat(this, level, format, _twoArgs);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(int level, string format, object arg1, object arg2, object arg3)
		{
			_threeArgs[0] = arg1;
			_threeArgs[1] = arg2;
			_threeArgs[2] = arg3;
			LogEngine.LogWarningFormat(this, level, format, _threeArgs);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(int level, string format, params object[] args)
		{
			LogEngine.LogWarningFormat(this, level, format, args);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(Enum level, string format)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(Enum level, string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(Enum level, string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(Enum level, string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(Enum level, string format, params object[] args)
		{
		}

		public void LogErrorFormat(string format)
		{
			LogErrorFormat(0, format);
		}

		public void LogErrorFormat(string format, object arg1)
		{
			LogErrorFormat(0, format, arg1);
		}

		public void LogErrorFormat(string format, object arg1, object arg2)
		{
			LogErrorFormat(0, format, arg1, arg2);
		}

		public void LogErrorFormat(string format, object arg1, object arg2, object arg3)
		{
			LogErrorFormat(0, format, arg1, arg2, arg3);
		}

		public void LogErrorFormat(string format, params object[] args)
		{
			LogErrorFormat(0, format, args);
		}

		public void LogErrorFormat(int level, string format)
		{
			LogEngine.LogErrorFormat(this, level, format, _zeroArgs);
		}

		public void LogErrorFormat(int level, string format, object arg1)
		{
			_singleArg[0] = arg1;
			LogEngine.LogErrorFormat(this, level, format, _singleArg);
		}

		public void LogErrorFormat(int level, string format, object arg1, object arg2)
		{
			_twoArgs[0] = arg1;
			_twoArgs[1] = arg2;
			LogEngine.LogErrorFormat(this, level, format, _twoArgs);
		}

		public void LogErrorFormat(int level, string format, object arg1, object arg2, object arg3)
		{
			_threeArgs[0] = arg1;
			_threeArgs[1] = arg2;
			_threeArgs[2] = arg3;
			LogEngine.LogErrorFormat(this, level, format, _threeArgs);
		}

		public void LogErrorFormat(int level, string format, params object[] args)
		{
			LogEngine.LogErrorFormat(this, level, format, args);
		}

		public void LogErrorFormat(Enum level, string format)
		{
			LogErrorFormat(Convert.ToInt32(level), format);
		}

		public void LogErrorFormat(Enum level, string format, object arg1)
		{
			LogErrorFormat(Convert.ToInt32(level), format, arg1);
		}

		public void LogErrorFormat(Enum level, string format, object arg1, object arg2)
		{
			LogErrorFormat(Convert.ToInt32(level), format, arg1, arg2);
		}

		public void LogErrorFormat(Enum level, string format, object arg1, object arg2, object arg3)
		{
			LogErrorFormat(Convert.ToInt32(level), format, arg1, arg2, arg3);
		}

		public void LogErrorFormat(Enum level, string format, params object[] args)
		{
			LogErrorFormat(Convert.ToInt32(level), format, args);
		}

		public void LogException(Exception exception)
		{
			LogEngine.LogException(this, 0, exception);
		}

		public void LogException(int level, Exception exception)
		{
			LogEngine.LogException(this, level, exception);
		}

		public void LogException(Enum level, Exception exception)
		{
			LogEngine.LogException(this, Convert.ToInt32(level), exception);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(UnityEngine.Object context, string format)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(UnityEngine.Object context, string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(UnityEngine.Object context, string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(UnityEngine.Object context, string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		[DebuggerHidden]
		public void LogFormat(UnityEngine.Object context, string format, params object[] args)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, int level, string format)
		{
			LogEngine.LogFormat(this, level, context, format, _zeroArgs);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, int level, string format, object arg1)
		{
			_singleArg[0] = arg1;
			LogEngine.LogFormat(this, level, context, format, _singleArg);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2)
		{
			_twoArgs[0] = arg1;
			_twoArgs[1] = arg2;
			LogEngine.LogFormat(this, level, context, format, _twoArgs);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3)
		{
			_threeArgs[0] = arg1;
			_threeArgs[1] = arg2;
			_threeArgs[2] = arg3;
			LogEngine.LogFormat(this, level, context, format, _threeArgs);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, int level, string format, params object[] args)
		{
			LogEngine.LogFormat(this, level, context, format, args);
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, Enum level, string format)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, Enum level, string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, Enum level, string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, Enum level, string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_LOG")]
		public void LogFormat(UnityEngine.Object context, Enum level, string format, params object[] args)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, string format)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, int level, string format)
		{
			LogEngine.LogWarningFormat(this, level, context, format, _zeroArgs);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1)
		{
			_singleArg[0] = arg1;
			LogEngine.LogWarningFormat(this, level, context, format, _singleArg);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2)
		{
			_twoArgs[0] = arg1;
			_twoArgs[1] = arg2;
			LogEngine.LogWarningFormat(this, level, context, format, _twoArgs);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3)
		{
			_threeArgs[0] = arg1;
			_threeArgs[1] = arg2;
			_threeArgs[2] = arg3;
			LogEngine.LogWarningFormat(this, level, context, format, _threeArgs);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, int level, string format, params object[] args)
		{
			LogEngine.LogWarningFormat(this, level, context, format, args);
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, Enum level, string format)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, Enum level, string format, object arg1)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, Enum level, string format, object arg1, object arg2)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, Enum level, string format, object arg1, object arg2, object arg3)
		{
		}

		[Conditional("ENABLE_LOGGING_WARNING")]
		public void LogWarningFormat(UnityEngine.Object context, Enum level, string format, params object[] args)
		{
		}

		public void LogErrorFormat(UnityEngine.Object context, string format)
		{
			LogErrorFormat(context, 0, format);
		}

		public void LogErrorFormat(UnityEngine.Object context, string format, object arg1)
		{
			LogErrorFormat(context, 0, format, arg1);
		}

		public void LogErrorFormat(UnityEngine.Object context, string format, object arg1, object arg2)
		{
			LogErrorFormat(context, 0, format, arg1, arg2);
		}

		public void LogErrorFormat(UnityEngine.Object context, string format, object arg1, object arg2, object arg3)
		{
			LogErrorFormat(context, 0, format, arg1, arg2, arg3);
		}

		public void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
		{
			LogErrorFormat(context, 0, format, args);
		}

		public void LogErrorFormat(UnityEngine.Object context, int level, string format)
		{
			LogEngine.LogErrorFormat(this, level, context, format, _zeroArgs);
		}

		public void LogErrorFormat(UnityEngine.Object context, int level, string format, object arg1)
		{
			_singleArg[0] = arg1;
			LogEngine.LogErrorFormat(this, level, context, format, _singleArg);
		}

		public void LogErrorFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2)
		{
			_twoArgs[0] = arg1;
			_twoArgs[1] = arg2;
			LogEngine.LogErrorFormat(this, level, context, format, _twoArgs);
		}

		public void LogErrorFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3)
		{
			_threeArgs[0] = arg1;
			_threeArgs[1] = arg2;
			_threeArgs[2] = arg3;
			LogEngine.LogErrorFormat(this, level, context, format, _threeArgs);
		}

		public void LogErrorFormat(UnityEngine.Object context, int level, string format, params object[] args)
		{
			LogEngine.LogErrorFormat(this, level, context, format, args);
		}

		public void LogErrorFormat(UnityEngine.Object context, Enum level, string format)
		{
			LogErrorFormat(context, Convert.ToInt32(level), format);
		}

		public void LogErrorFormat(UnityEngine.Object context, Enum level, string format, object arg1)
		{
			LogErrorFormat(context, Convert.ToInt32(level), format, arg1);
		}

		public void LogErrorFormat(UnityEngine.Object context, Enum level, string format, object arg1, object arg2)
		{
			LogErrorFormat(context, Convert.ToInt32(level), format, arg1, arg2);
		}

		public void LogErrorFormat(UnityEngine.Object context, Enum level, string format, object arg1, object arg2, object arg3)
		{
			LogErrorFormat(context, Convert.ToInt32(level), format, arg1, arg2, arg3);
		}

		public void LogErrorFormat(UnityEngine.Object context, Enum level, string format, params object[] args)
		{
			LogErrorFormat(context, Convert.ToInt32(level), format, args);
		}

		void IWriteLog.LogFormat(int level, string format)
		{
		}

		void IWriteLog.LogFormat(int level, string format, object arg1)
		{
		}

		void IWriteLog.LogFormat(int level, string format, object arg1, object arg2)
		{
		}

		void IWriteLog.LogFormat(int level, string format, object arg1, object arg2, object arg3)
		{
		}

		void IWriteLog.LogFormat(int level, string format, params object[] args)
		{
		}

		void IWriteLog.LogLongString(int level, string data)
		{
		}

		void IWriteLog.LogWarningFormat(int level, string format)
		{
		}

		void IWriteLog.LogWarningFormat(int level, string format, object arg1)
		{
		}

		void IWriteLog.LogWarningFormat(int level, string format, object arg1, object arg2)
		{
		}

		void IWriteLog.LogWarningFormat(int level, string format, object arg1, object arg2, object arg3)
		{
		}

		void IWriteLog.LogWarningFormat(int level, string format, params object[] args)
		{
		}

		void IWriteLog.LogFormat(UnityEngine.Object context, int level, string format)
		{
		}

		void IWriteLog.LogFormat(UnityEngine.Object context, int level, string format, object arg1)
		{
		}

		void IWriteLog.LogFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2)
		{
		}

		void IWriteLog.LogFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3)
		{
		}

		void IWriteLog.LogFormat(UnityEngine.Object context, int level, string format, params object[] args)
		{
		}

		void IWriteLog.LogWarningFormat(UnityEngine.Object context, int level, string format)
		{
		}

		void IWriteLog.LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1)
		{
		}

		void IWriteLog.LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2)
		{
		}

		void IWriteLog.LogWarningFormat(UnityEngine.Object context, int level, string format, object arg1, object arg2, object arg3)
		{
		}

		void IWriteLog.LogWarningFormat(UnityEngine.Object context, int level, string format, params object[] args)
		{
		}
	}
}
