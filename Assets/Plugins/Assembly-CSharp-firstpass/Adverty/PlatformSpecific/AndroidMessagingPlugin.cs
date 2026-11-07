using System;
using System.Runtime.InteropServices;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
	public class AndroidMessagingPlugin : IAndroidMessagingPlugin, INativeClass
	{
		[DllImport("glbridge")]
		private static extern void SetNativeEventHandlerMethod(IntPtr nativeEventsRecieverMethod);

		[DllImport("glbridge")]
		private static extern void SetNativeCreateUnityObjectMethod(IntPtr nativeCreateUnityObjectMethod);

		[DllImport("glbridge")]
		private static extern void SetNativeForgetUnityObjectMethod(IntPtr nativeForgetUnityObjectMethod);

		[DllImport("glbridge")]
		private static extern void SetNativeSetIntMethod(IntPtr nativeSetIntMethod);

		[DllImport("glbridge")]
		private static extern void SetNativeSetFloatMethod(IntPtr nativeSetFloatMethod);

		[DllImport("glbridge")]
		private static extern void SetNativeSetBoolMethod(IntPtr nativeSetBoolMethod);

		[DllImport("glbridge")]
		private static extern void SetNativeSetStringMethod(IntPtr nativeSetStringMethod);

		public void NativeSetNativeEventHandlerMethod(IntPtr nativeEventsRecieverMethod)
		{
			SetNativeEventHandlerMethod(nativeEventsRecieverMethod);
		}

		public void NativeSetNativeCreateUnityObjectMethod(IntPtr nativeCreateUnityObjectMethod)
		{
			SetNativeCreateUnityObjectMethod(nativeCreateUnityObjectMethod);
		}

		public void NativeSetNativeForgetUnityObjectMethod(IntPtr nativeForgetUnityObjectMethod)
		{
			SetNativeForgetUnityObjectMethod(nativeForgetUnityObjectMethod);
		}

		public void NativeSetNativeSetIntMethod(IntPtr nativeSetIntMethod)
		{
			SetNativeSetIntMethod(nativeSetIntMethod);
		}

		public void NativeSetNativeSetFloatMethod(IntPtr nativeSetFloatMethod)
		{
			SetNativeSetFloatMethod(nativeSetFloatMethod);
		}

		public void NativeSetNativeSetBoolMethod(IntPtr nativeSetBoolMethod)
		{
			SetNativeSetBoolMethod(nativeSetBoolMethod);
		}

		public void NativeSetNativeSetStringMethod(IntPtr nativeSetStringMethod)
		{
			SetNativeSetStringMethod(nativeSetStringMethod);
		}
	}
}
