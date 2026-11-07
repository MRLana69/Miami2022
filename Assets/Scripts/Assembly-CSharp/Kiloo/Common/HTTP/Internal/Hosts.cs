using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP.Internal
{
	internal static class Hosts
	{
		private class HostData
		{
			public DateTime backOffUntil = DateTime.MinValue;

			public double lastBackOffDuration;
		}

		private static readonly Dictionary<string, HostData> _hostData = new Dictionary<string, HostData>();

		public static void BackOff(string host, double minTime, double maxTime)
		{
			lock (_hostData)
			{
				HostData value;
				if (!_hostData.TryGetValue(host, out value))
				{
					value = new HostData();
					_hostData[host] = value;
				}
				value.lastBackOffDuration *= 2.0;
				if (value.lastBackOffDuration < minTime)
				{
					value.lastBackOffDuration = minTime;
				}
				if (value.lastBackOffDuration > maxTime)
				{
					value.lastBackOffDuration = maxTime;
				}
				value.backOffUntil = DateTime.UtcNow.AddSeconds(value.lastBackOffDuration);
			}
		}

		public static void BackOff(string host, double seconds)
		{
			BackOff(host, DateTime.UtcNow.AddSeconds(seconds));
		}

		public static void BackOff(string host, DateTime until)
		{
			lock (_hostData)
			{
				HostData value;
				if (!_hostData.TryGetValue(host, out value))
				{
					value = new HostData();
					_hostData[host] = value;
				}
				value.backOffUntil = until.ToUniversalTime();
				value.lastBackOffDuration = (until - DateTime.UtcNow).TotalSeconds;
				if (value.lastBackOffDuration < 0.0)
				{
					value.lastBackOffDuration = 0.0;
				}
			}
		}

		public static void DontBackOff(string host)
		{
			lock (_hostData)
			{
				HostData value;
				if (_hostData.TryGetValue(host, out value))
				{
					value.lastBackOffDuration = 0.0;
					value.backOffUntil = DateTime.MinValue;
				}
			}
		}

		public static double GetBackOffSeconds(string host)
		{
			lock (_hostData)
			{
				HostData value;
				if (_hostData.TryGetValue(host, out value))
				{
					DateTime utcNow = DateTime.UtcNow;
					if (utcNow < value.backOffUntil)
					{
						return (value.backOffUntil - utcNow).TotalSeconds;
					}
				}
				return 0.0;
			}
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[Hosts] " + msg);
		}
	}
}
