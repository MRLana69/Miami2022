using System;

namespace SYBO.Components.Settings
{
	public interface ISettingsProvider
	{
		object ReflectionGetSettings(Type settingsType);
	}
}
