using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using Kiloo.Google.Internal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Google
{
	public sealed class GoogleSignInPlugin
	{
		public delegate void ProfileUpdatedHandler(IGoogleProfile oldProfile, IGoogleProfile newProfile);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ProfileUpdatedHandler profileUpdated__BackingField;

		private readonly IGoogleSignInBridge _bridge;

		public static bool isSupported
		{
			get
			{
				return GoogleSignInBridgeFactory.isPlatformSupported;
			}
		}

		public static GoogleSignInPlugin instance { get; private set; }

		public bool profileIsUpdated { get; private set; }

		public IGoogleProfile profile { get; private set; }

		public bool isSignedIn
		{
			get
			{
				return profile != null;
			}
		}

		public static bool isInitialized
		{
			get
			{
				return instance != null;
			}
		}

		public event ProfileUpdatedHandler profileUpdated
		{
			add
			{
				ProfileUpdatedHandler profileUpdatedHandler = profileUpdated__BackingField;
				ProfileUpdatedHandler profileUpdatedHandler2;
				do
				{
					profileUpdatedHandler2 = profileUpdatedHandler;
					profileUpdatedHandler = Interlocked.CompareExchange(ref profileUpdated__BackingField, (ProfileUpdatedHandler)Delegate.Combine(profileUpdatedHandler2, value), profileUpdatedHandler);
				}
				while ((object)profileUpdatedHandler != profileUpdatedHandler2);
			}
			remove
			{
				ProfileUpdatedHandler profileUpdatedHandler = profileUpdated__BackingField;
				ProfileUpdatedHandler profileUpdatedHandler2;
				do
				{
					profileUpdatedHandler2 = profileUpdatedHandler;
					profileUpdatedHandler = Interlocked.CompareExchange(ref profileUpdated__BackingField, (ProfileUpdatedHandler)Delegate.Remove(profileUpdatedHandler2, value), profileUpdatedHandler);
				}
				while ((object)profileUpdatedHandler != profileUpdatedHandler2);
			}
		}

		private GoogleSignInPlugin()
		{
			_bridge = GoogleSignInBridgeFactory.Create();
		}

		public static void Init()
		{
			if (instance == null)
			{
				instance = new GoogleSignInPlugin();
			}
		}

		public Coroutine SignIn(GoogleSignInPluginRequest request = null, Action<GoogleSignInPluginRequest> onComplete = null)
		{
			return CoroutineUtil.StartCoroutine(SignInCoroutine(request, onComplete));
		}

		public Coroutine SignOut(GoogleSignInPluginRequest request = null, Action<GoogleSignInPluginRequest> onComplete = null)
		{
			return CoroutineUtil.StartCoroutine(SignOutCoroutine(request, onComplete));
		}

		private IEnumerator SignInCoroutine(GoogleSignInPluginRequest request, Action<GoogleSignInPluginRequest> onComplete)
		{
			if (!_bridge.IsConnected())
			{
				GoogleSignInBridgeRequest<IGoogleConnectResult> connectRequest = new GoogleSignInBridgeRequest<IGoogleConnectResult>();
				yield return _bridge.Connect(connectRequest);
				if (connectRequest.result.errorCode != 0)
				{
					LogError("Error connecting");
					CompleteRequest(request, onComplete, "SignIn", null, connectRequest.result.errorCode, connectRequest.result.errorMessage);
					yield break;
				}
			}
			GoogleSignInBridgeRequest<IGoogleSignInResult> signInRequest = new GoogleSignInBridgeRequest<IGoogleSignInResult>();
			bool silent = request != null && request.silent;
			yield return _bridge.SignIn(signInRequest, silent);
			if (signInRequest.result.errorCode != 0)
			{
				LogError("Error signing in: " + signInRequest.result.errorCode + ": " + signInRequest.result.errorMessage);
			}
			UpdateProfile(signInRequest.result.profile);
			CompleteRequest(request, onComplete, "SignIn", signInRequest.result.profile, signInRequest.result.errorCode, signInRequest.result.errorMessage);
		}

		private IEnumerator SignOutCoroutine(GoogleSignInPluginRequest request, Action<GoogleSignInPluginRequest> onComplete)
		{
			if (!_bridge.IsConnected())
			{
				GoogleSignInBridgeRequest<IGoogleConnectResult> connectRequest = new GoogleSignInBridgeRequest<IGoogleConnectResult>();
				yield return _bridge.Connect(connectRequest);
				if (connectRequest.result.errorCode != 0)
				{
					LogError("Error connecting");
					CompleteRequest(request, onComplete, "SignOut", null, connectRequest.result.errorCode, connectRequest.result.errorMessage);
					yield break;
				}
			}
			GoogleSignInBridgeRequest<IGoogleSignOutResult> signOutRequest = new GoogleSignInBridgeRequest<IGoogleSignOutResult>();
			yield return _bridge.SignOut(signOutRequest);
			UpdateProfile(null);
			CompleteRequest(request, onComplete, "SignOut");
		}

		private void UpdateProfile(IGoogleProfile newProfile)
		{
			profileIsUpdated = true;
			if (!AreEqual(profile, newProfile))
			{
				IGoogleProfile oldProfile = profile;
				profile = newProfile;
				ProfileUpdatedHandler profileUpdatedHandler = profileUpdated__BackingField;
				if (profileUpdatedHandler != null)
				{
					profileUpdatedHandler(oldProfile, newProfile);
				}
			}
		}

		private void CompleteRequest(GoogleSignInPluginRequest request, Action<GoogleSignInPluginRequest> onComplete, string context, IGoogleProfile requestProfile = null, int errorCode = 0, string errorMessage = null)
		{
			if (request == null && onComplete != null)
			{
				request = new GoogleSignInPluginRequest();
			}
			if (request != null)
			{
				request.profile = requestProfile;
				request.errorCode = errorCode;
				request.errorMessage = errorMessage;
			}
			if (onComplete != null)
			{
				onComplete(request);
			}
		}

		private static bool AreEqual(IGoogleProfile a, IGoogleProfile b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			return a.userId == b.userId && a.displayName == b.displayName && a.serverAuthCode == b.serverAuthCode && a.photoUrl == b.photoUrl;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[GoogleSignIn] " + msg);
		}

		[Conditional("GOOGLE_SIGN_IN_DEBUG_LOGS")]
		[StringFormatMethod("msg")]
		private static void Log(string msg, params object[] args)
		{
			Debug.Log("[GoogleSignIn] " + string.Format(msg, args));
		}
	}
}
