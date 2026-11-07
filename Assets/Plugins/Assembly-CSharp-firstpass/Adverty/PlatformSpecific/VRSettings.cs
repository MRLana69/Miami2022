using Adverty.Native;
using UnityEngine.XR;

namespace Adverty.PlatformSpecific
{
	public class VRSettings : IPlatformVRSettings, INativeClass
	{
		public bool IsEnabled()
		{
			return XRSettings.enabled;
		}

		public string GetModel()
		{
			return XRSettings.loadedDeviceName;
		}
	}
}
