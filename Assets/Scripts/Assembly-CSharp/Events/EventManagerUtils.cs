using System;
using System.Collections.Generic;

namespace Events
{
	public static class EventManagerUtils
	{
		private static Dictionary<Type, Dictionary<string, object>> _cachedValues = new Dictionary<Type, Dictionary<string, object>>();

		public static Dictionary<string, object> GetCachedEventValueDict(Type type)
		{
			Dictionary<string, object> value = null;
			try
			{
				if (!_cachedValues.TryGetValue(type, out value))
				{
					value = new Dictionary<string, object>();
					Array values = Enum.GetValues(type);
					foreach (object item in values)
					{
						string name = Enum.GetName(type, item);
						if (!string.IsNullOrEmpty(name))
						{
							value.Add(name, item);
						}
					}
					_cachedValues.Add(type, value);
				}
			}
			catch
			{
				SDebug.LogErrorFormat("Error while caching Enum values of the type '{0}'" + type);
			}
			return value;
		}
	}
}
