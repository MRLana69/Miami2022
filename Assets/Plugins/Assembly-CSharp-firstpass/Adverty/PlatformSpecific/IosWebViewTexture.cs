using System;
using Adverty.Native;

namespace Adverty.PlatformSpecific
{
	public class IosWebViewTexture : IIosWebViewTexture, INativeClass
	{
		public IntPtr CWebViewTexturePlugin_Create(int width, int height, int eventListenerId)
		{
			throw new NotImplementedException();
		}

		public IntPtr CWebViewTexturePlugin_GetTexturePointer(IntPtr instance)
		{
			throw new NotImplementedException();
		}

		public void CWebViewTexturePlugin_UpdateTexture(IntPtr instance, float delay)
		{
			throw new NotImplementedException();
		}

		public void CWebViewTexturePlugin_LoadURL(IntPtr instance, string html)
		{
			throw new NotImplementedException();
		}

		public void CWebViewTexturePlugin_RunJavaScript(IntPtr instance, string javaScriptString)
		{
			throw new NotImplementedException();
		}

		public void CWebViewTexturePlugin_Destroy(IntPtr instance)
		{
			throw new NotImplementedException();
		}
	}
}
