using System;
using UnityEngine;

namespace Kiloo.Google.Internal
{
	internal class GoogleSignInBridgeFactory
	{
		public static bool isPlatformSupported
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform == RuntimePlatform.Android)
				{
					return AndroidGoogleSignInBridge.IsSupported();
				}
				return false;
			}
		}

		public static IGoogleSignInBridge Create()
		{
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.Android)
			{
				return AndroidGoogleSignInBridge.Create();
			}
			throw new NotSupportedException();
		}
	}
}
