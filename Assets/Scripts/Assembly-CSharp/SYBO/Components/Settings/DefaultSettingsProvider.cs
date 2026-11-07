using System;

namespace SYBO.Components.Settings
{
	public class DefaultSettingsProvider : ISettingsProvider
	{
		private ISettingsParser _reader;

		public DefaultSettingsProvider(ISettingsParser reader)
		{
			_reader = reader;
		}

		object ISettingsProvider.ReflectionGetSettings(Type settingsType)
		{
			return _reader.ReflectionReadSettings(settingsType);
		}
	}
}
