using System;
using JetBrains.Annotations;
using Kiloo.Plugins;
using UnityEngine;

namespace Kiloo.Google.Internal
{
	internal sealed class AndroidGoogleSignInBridge : GoogleSignInBridge
	{
		private AndroidJavaObject _plugin;

		[UsedImplicitly]
		private void Awake()
		{
			_plugin = new AndroidJavaObject("com.kiloo.google.GoogleSignInPlugin");
		}

		private bool PluginInit()
		{
			return _plugin.Call<bool>("init", new object[1] { base.name });
		}

		protected override bool PluginIsConnected()
		{
			return _plugin.Call<bool>("isConnected", new object[0]);
		}

		protected override void PluginConnect()
		{
			_plugin.Call("connect");
		}

		protected override bool PluginSignIn(bool silent)
		{
			return _plugin.Call<bool>((!silent) ? "signIn" : "silentSignIn", new object[0]);
		}

		protected override bool PluginSignOut()
		{
			return _plugin.Call<bool>("signOut", new object[0]);
		}

		public static bool IsSupported()
		{
			return Application.platform == RuntimePlatform.Android;
		}

		public static GoogleSignInBridge Create()
		{
			AndroidGoogleSignInBridge androidGoogleSignInBridge = PluginUtil.CreateBridgeObject<AndroidGoogleSignInBridge>();
			if (androidGoogleSignInBridge.PluginInit())
			{
				return androidGoogleSignInBridge;
			}
			androidGoogleSignInBridge.Dispose();
			throw new NotSupportedException("Failed to initialize plugin. Check log and make sure Server_Client_Id is configured.");
		}
	}
}
