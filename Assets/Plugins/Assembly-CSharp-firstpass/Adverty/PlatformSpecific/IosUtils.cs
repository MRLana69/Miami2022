using System;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
	public class IosUtils : IIosNativeUtils, INativeClass
	{
		public string GetSystemLocale()
		{
			throw new NotImplementedException();
		}

		public void SetUnityReceiverGameObjectName(string name)
		{
			throw new NotImplementedException();
		}
	}
}
