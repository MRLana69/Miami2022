using System;
using System.Collections.Generic;
using System.Linq;

namespace Kiloo.Common.Backend.Cloud
{
	public static class CloudUserDataExtensions
	{
		public static Dictionary<string, string> ToDictionary(this ICloudUserData userData)
		{
			return userData.ToDictionary((KeyValuePair<string, string> pair) => pair.Key, (KeyValuePair<string, string> pair) => pair.Value, StringComparer.OrdinalIgnoreCase);
		}

		public static ICloudUserData ToUserData(this IDictionary<string, string> dict)
		{
			return new CloudUserData(dict);
		}
	}
}
