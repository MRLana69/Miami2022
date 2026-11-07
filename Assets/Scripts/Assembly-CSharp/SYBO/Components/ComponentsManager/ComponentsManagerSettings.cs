using SYBO.Components.Settings;

namespace SYBO.Components.ComponentsManager
{
	public class ComponentsManagerSettings
	{
		[DebugSetting]
		public string[] _componentZipPath = new string[1] { "/Unity/Assets/SYBO/Components/{0}/" };
	}
}
