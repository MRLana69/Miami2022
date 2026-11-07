using System;

namespace SYBO.Components.Settings
{
	public interface ISettingsParser
	{
		object ReflectionReadSettings(Type settingsType);
	}
}
