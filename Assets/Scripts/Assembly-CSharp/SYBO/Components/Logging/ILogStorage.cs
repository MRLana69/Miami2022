using System;
using System.Collections.Generic;

namespace SYBO.Components.Logging
{
	public interface ILogStorage
	{
		int NumberOfCollapsedLogs { get; }

		int NumberOfLogs { get; }

		int NumberOfCollapsedLogsWarning { get; }

		int NumberOfLogsWarning { get; }

		int NumberOfCollapsedLogsError { get; }

		int NumberOfLogsError { get; }

		float GcTotalMemoryMB { get; }

		string FpsText { get; }

		float LogsMemUsageMB { get; }

		string LogDate { get; }

		event Action<LogEntry> OnLogAdded;

		event Action OnLogCleared;

		void AddLogEntry(LogEntry entry);

		bool Clear();

		void Tick();

		bool StoreLogs();

		IList<LogEntry> GetLogs();

		IList<LogEntry> GetCollapsedLogs();

		LogEntry GetCollapsedEntry(LogEntry selectedLog);

		Sample GetSample(int sampleId);

		int GetSampleCount();
	}
}
