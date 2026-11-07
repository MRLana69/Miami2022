using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Google.Internal
{
	internal abstract class GoogleSignInBridge : MonoBehaviour, IGoogleSignInBridge, IDisposable
	{
		private class ConnectResult : IGoogleConnectResult
		{
			public string errorMessage { get; set; }

			public int errorCode { get; set; }
		}

		[Serializable]
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class SignInResultReceivedPayload
		{
			public ProfilePayload profile;

			public string errorMessage;

			public int errorCode;
		}

		[Serializable]
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class ProfilePayload
		{
			public string displayName;

			public string photoUrl;

			public string serverAuthCode;

			public string userId;
		}

		[Serializable]
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class ConnectionSuspendedPayload
		{
			public int cause;
		}

		[Serializable]
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class ConnectionFailedPayload
		{
			public string errorMessage;

			public int errorCode;
		}

		private class GoogleSignInResult : IGoogleSignInResult
		{
			public IGoogleProfile profile { get; set; }

			public string errorMessage { get; set; }

			public int errorCode { get; set; }
		}

		private class GoogleProfile : IGoogleProfile
		{
			public string displayName { get; set; }

			public string photoUrl { get; set; }

			public string serverAuthCode { get; set; }

			public string userId { get; set; }
		}

		private bool _connectionIsSuspended;

		private GoogleSignInBridgeRequest<IGoogleConnectResult> _pendingConnectRequest;

		private GoogleSignInBridgeRequest<IGoogleSignInResult> _pendingSignInRequest;

		private GoogleSignInBridgeRequest<IGoogleSignOutResult> _pendingSignOutRequest;

		public void Dispose()
		{
			if ((bool)base.gameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public bool IsConnected()
		{
			return PluginIsConnected();
		}

		public Coroutine Connect(GoogleSignInBridgeRequest<IGoogleConnectResult> request)
		{
			return StartCoroutine(ConnectCoroutine(request));
		}

		public Coroutine SignIn(GoogleSignInBridgeRequest<IGoogleSignInResult> request, bool silent)
		{
			return StartCoroutine(SignInCoroutine(request, silent));
		}

		public Coroutine SignOut(GoogleSignInBridgeRequest<IGoogleSignOutResult> request)
		{
			return StartCoroutine(SignOutCoroutine(request));
		}

		protected abstract bool PluginIsConnected();

		protected abstract void PluginConnect();

		protected abstract bool PluginSignIn(bool silent);

		protected abstract bool PluginSignOut();

		private IEnumerator ConnectCoroutine([NotNull] GoogleSignInBridgeRequest<IGoogleConnectResult> request)
		{
			request.done = false;
			request.canceled = false;
			request.result = null;
			while (_pendingConnectRequest != null)
			{
				yield return null;
			}
			if (_connectionIsSuspended || PluginIsConnected())
			{
				request.canceled = true;
				request.done = true;
				yield break;
			}
			_pendingConnectRequest = request;
			PluginConnect();
			while (!request.done)
			{
				yield return null;
				if (request.canceled)
				{
					request.done = true;
				}
			}
			_pendingConnectRequest = null;
		}

		private IEnumerator SignInCoroutine(GoogleSignInBridgeRequest<IGoogleSignInResult> request, bool silent)
		{
			request.done = false;
			request.canceled = false;
			request.result = null;
			while (_pendingSignInRequest != null)
			{
				yield return null;
			}
			_pendingSignInRequest = request;
			if (PluginSignIn(silent))
			{
				while (!request.done)
				{
					yield return null;
					if (request.canceled)
					{
						request.done = true;
					}
				}
			}
			else
			{
				request.canceled = true;
				request.done = true;
			}
			_pendingSignInRequest = null;
		}

		private IEnumerator SignOutCoroutine(GoogleSignInBridgeRequest<IGoogleSignOutResult> request)
		{
			request.done = false;
			request.canceled = false;
			request.result = null;
			while (_pendingSignOutRequest != null)
			{
				yield return null;
			}
			_pendingSignOutRequest = request;
			if (PluginSignOut())
			{
				while (!request.done)
				{
					yield return null;
					if (request.canceled)
					{
						request.done = true;
					}
				}
			}
			else
			{
				request.canceled = true;
				request.done = true;
			}
			_pendingSignOutRequest = null;
		}

		private IGoogleSignInResult ToGoogleSignInResult(SignInResultReceivedPayload signInResultReceivedPayload)
		{
			if (signInResultReceivedPayload == null)
			{
				return null;
			}
			GoogleSignInResult googleSignInResult = new GoogleSignInResult();
			googleSignInResult.profile = ToGoogleProfile(signInResultReceivedPayload.profile);
			googleSignInResult.errorCode = signInResultReceivedPayload.errorCode;
			googleSignInResult.errorMessage = signInResultReceivedPayload.errorMessage;
			return googleSignInResult;
		}

		private IGoogleProfile ToGoogleProfile(ProfilePayload profile)
		{
			if (profile == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(profile.displayName) && string.IsNullOrEmpty(profile.photoUrl) && string.IsNullOrEmpty(profile.serverAuthCode) && string.IsNullOrEmpty(profile.userId))
			{
				return null;
			}
			GoogleProfile googleProfile = new GoogleProfile();
			googleProfile.displayName = profile.displayName;
			googleProfile.photoUrl = profile.photoUrl;
			googleProfile.serverAuthCode = profile.serverAuthCode;
			googleProfile.userId = profile.userId;
			return googleProfile;
		}

		[UsedImplicitly]
		private void OnGoogleConnected()
		{
			if (_pendingConnectRequest != null)
			{
				_pendingConnectRequest.result = new ConnectResult();
				_pendingConnectRequest.done = true;
			}
			_connectionIsSuspended = false;
		}

		[UsedImplicitly]
		private void OnGoogleConnectionSuspended(string payload)
		{
			_connectionIsSuspended = true;
		}

		[UsedImplicitly]
		private void OnGoogleConnectionFailed(string payload)
		{
			ConnectionFailedPayload connectionFailedPayload = JsonUtility.FromJson<ConnectionFailedPayload>(payload);
			if (_pendingConnectRequest != null)
			{
				_pendingConnectRequest.result = new ConnectResult
				{
					errorMessage = connectionFailedPayload.errorMessage,
					errorCode = connectionFailedPayload.errorCode
				};
				_pendingConnectRequest.done = true;
			}
		}

		[UsedImplicitly]
		private void OnGoogleSignInResultReceived(string payload)
		{
			SignInResultReceivedPayload signInResultReceivedPayload = JsonUtility.FromJson<SignInResultReceivedPayload>(payload);
			if (_pendingSignInRequest != null)
			{
				_pendingSignInRequest.result = ToGoogleSignInResult(signInResultReceivedPayload);
				_pendingSignInRequest.done = true;
			}
		}

		[UsedImplicitly]
		private void OnGoogleSignedOut()
		{
			if (_pendingSignOutRequest != null)
			{
				_pendingSignOutRequest.done = true;
			}
		}
	}
}
