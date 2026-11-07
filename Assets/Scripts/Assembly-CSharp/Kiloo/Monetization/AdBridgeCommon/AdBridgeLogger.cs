using UnityEngine;
using Utilities;

namespace Kiloo.Monetization.AdBridgeCommon
{
	public class AdBridgeLogger
	{
		private string _platform;

		private string _providerId;

		private AdBridgeMethodInvoker _defaultAdBridgeMethodInvoker;

		private bool _isLoggingEnabled;

		public AdBridgeLogger(string providerId, bool isLoggingEnabled, AdBridgeMethodInvoker defaultAdBridgeMethodInvoker)
		{
			_isLoggingEnabled = isLoggingEnabled;
			_platform = Application.platform.ToFriendlyString();
			_providerId = providerId;
			_defaultAdBridgeMethodInvoker = defaultAdBridgeMethodInvoker;
		}

		public void UpdateDebugLogsState()
		{
			if (_isLoggingEnabled)
			{
				_defaultAdBridgeMethodInvoker.EnableDebugLogs();
			}
			else
			{
				_defaultAdBridgeMethodInvoker.DisableDebugLogs();
			}
		}

		public void LogError(string msg, Object context = null)
		{
			SDebug.LogError(string.Format("{0}{1}: {2}", _providerId, _platform, msg), context);
		}

		public void LogWarning(string msg, Object context = null)
		{
		}

		public void Log(string msg, Object context = null)
		{
			if (!_isLoggingEnabled)
			{
			}
		}
	}
}
