using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace Utilities
{
	public static class RuntimePlatformExtensions
	{
		public static string ToFriendlyString(this RuntimePlatform platform)
		{
			switch (platform)
			{
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice)
				{
					return "Kindle";
				}
				return RuntimePlatform.Android.ToString();
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			default:
				return platform.ToString();
			}
		}
	}
}
