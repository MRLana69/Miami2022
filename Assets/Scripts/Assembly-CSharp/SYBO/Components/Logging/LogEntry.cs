using System;
using UnityEngine;

namespace SYBO.Components.Logging
{
	public class LogEntry
	{
		public int Count = 1;

		public LogType EntryType;

		public string Condition;

		public string Stacktrace;

		public int SampleId;

		public int CategoryId;

		public bool WasDuplicate;

		public float time;

		public byte loadedScene;

		public float memory;

		public float fps;

		public string fpsText;

		public LogEntry CreateCopy()
		{
			return (LogEntry)MemberwiseClone();
		}

		public float GetMemoryUsage()
		{
			int num = 8 + IntPtr.Size + IntPtr.Size + 4 + 4 + 1 + 4 + 1 + 4 + 4 + IntPtr.Size;
			if (!WasDuplicate)
			{
				num += Condition.Length * 2 + Stacktrace.Length * 2;
			}
			return num;
		}
	}
}
