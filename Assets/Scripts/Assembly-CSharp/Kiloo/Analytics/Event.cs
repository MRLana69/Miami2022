using System.Collections.Generic;

namespace Kiloo.Analytics
{
	internal class Event
	{
		public readonly string name;

		public readonly Dictionary<string, object> parameters;

		public Event(string name, Dictionary<string, object> parameters = null)
		{
			this.name = name;
			this.parameters = parameters;
		}
	}
}
