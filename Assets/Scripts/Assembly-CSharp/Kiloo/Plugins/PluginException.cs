using System;

namespace Kiloo.Plugins
{
	public class PluginException : Exception
	{
		public PluginException()
		{
		}

		public PluginException(string msg)
			: base(msg)
		{
		}
	}
}
