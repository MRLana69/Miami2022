using System;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
	public class OsxUtils : IOsxNativeUtils, INativeClass
	{
		public string GetSystemLocale()
		{
			throw new NotImplementedException();
		}
	}
}
