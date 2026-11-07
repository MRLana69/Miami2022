using System;
using System.Runtime.InteropServices;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
	public class AndroidRenderingPlugin : IAndroidRenderingPlugin, INativeClass
	{
		[DllImport("glbridge")]
		private static extern void SetGlIssuePluginEventMethod(IntPtr glIssuePluginMethod);

		[DllImport("glbridge")]
		private static extern void SetUnityRenderEventHandlerMethod(IntPtr unityRenderEventHandlerMethod);

		[DllImport("glbridge")]
		private static extern void SetNativeEventHandlerMethod(IntPtr nativeEventsRecieverMethod);

		[DllImport("glbridge")]
		private static extern void IssuePluginEvent(int eventId);

		public void NativeSetGlIssuePluginEventMethod(IntPtr glIssuePluginMethod)
		{
			SetGlIssuePluginEventMethod(glIssuePluginMethod);
		}

		public void NativeSetUnityRenderEventHandlerMethod(IntPtr unityRenderEventHandlerMethod)
		{
			SetUnityRenderEventHandlerMethod(unityRenderEventHandlerMethod);
		}

		public void NativeSetNativeEventHandlerMethod(IntPtr nativeEventsRecieverMethod)
		{
			SetNativeEventHandlerMethod(nativeEventsRecieverMethod);
		}

		public void NativeIssuePluginEvent(int eventId)
		{
			IssuePluginEvent(eventId);
		}
	}
}
