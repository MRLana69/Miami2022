using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kiloo.Common.Backend
{
	public interface IEventLogger
	{
		void LogEvent([NotNull] string eventName, [CanBeNull] Dictionary<string, object> parameters = null);
	}
}
