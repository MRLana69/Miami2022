using Adverty.Native;
using UnityEngine;
using UnityEngine.Networking;

namespace Adverty.PlatformSpecific
{
	public class WebRequest : IPlatformWebRequest, INativeClass
	{
		public AsyncOperation Send(UnityWebRequest request)
		{
			return request.SendWebRequest();
		}

		public bool IsNetworkError(UnityWebRequest request)
		{
			return request.isNetworkError;
		}

		public bool IsHttpError(UnityWebRequest request)
		{
			return request.isHttpError;
		}

		public UnityWebRequest GetTexture(string uri, bool nonReadable)
		{
			return UnityWebRequestTexture.GetTexture(uri, nonReadable);
		}
	}
}
