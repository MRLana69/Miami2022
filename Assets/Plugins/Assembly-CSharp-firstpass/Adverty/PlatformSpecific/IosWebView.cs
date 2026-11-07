using System;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
	public class IosWebView : IIosWebView, INativeClass
	{
		public IntPtr CWebViewPlugin_Open(int id, string url, string closeButtonPath, string backButtonPath, string forwardButtonPath)
		{
			throw new NotImplementedException();
		}

		public void CWebViewPlugin_Close(IntPtr instance)
		{
			throw new NotImplementedException();
		}

		public void CWebViewPlugin_SetVisibility(IntPtr instance, bool visibility)
		{
			throw new NotImplementedException();
		}

		public void CWebViewPlugin_LoadURL(IntPtr instance, string url)
		{
			throw new NotImplementedException();
		}

		public void CWebViewPlugin_Mute(IntPtr instance)
		{
			throw new NotImplementedException();
		}

		public void CWebViewPlugin_SetScrollEnabled(IntPtr instance, bool isScrollEnabled)
		{
			throw new NotImplementedException();
		}

		public void CWebViewPlugin_SetZoomEnabled(IntPtr instance, bool isZoomEnabled)
		{
			throw new NotImplementedException();
		}

		public void CWebViewPlugin_SetPageNavigationEnabled(IntPtr instance, bool isNavigationEnabled)
		{
			throw new NotImplementedException();
		}
	}
}
