using System;
using System.Collections;
using System.Text;
using Kiloo.Plugins;
using UnityEngine;

namespace Kiloo.Google.Internal
{
	internal sealed class SimulatedGoogleSignInBridge : GoogleSignInBridge
	{
		[Serializable]
		public class Profile
		{
			public string displayName;

			public string userId;

			public string photoUrl;

			public string idToken;
		}

		[Serializable]
		public class SignInResult
		{
			public Profile profile;

			public int errorCode;

			public string errorMessage;
		}

		[Tooltip("User id of user when signed in.")]
		public string userId = "10101010";

		[Tooltip("Display name of user when signed in.")]
		public string displayName = "John Doe";

		[Tooltip("Set to non-zero error code to simulate sign-in error.")]
		public int signInError;

		[Tooltip("Message used when signInError is non-zero.")]
		public string signInErrorMessage;

		public float latency = 0.1f;

		private const string PHOTO_URL_FORMAT = "http://imaginary-photo-cdn.com/{0}/picture";

		private bool _connected;

		private bool _signedIn;

		protected override bool PluginIsConnected()
		{
			return _connected;
		}

		protected override void PluginConnect()
		{
			StartCoroutine(WaitAndDo(0.1f, () =>
			{
				SendMessage("OnGoogleConnected", string.Empty);
			}));
		}

		protected override bool PluginSignIn(bool silent)
		{
			string payload;
			if (signInError != 0)
			{
				payload = JsonUtility.ToJson(new SignInResult
				{
					errorCode = signInError,
					errorMessage = signInErrorMessage
				});
			}
			else if (silent && !_signedIn)
			{
				payload = JsonUtility.ToJson(new SignInResult
				{
					errorCode = 4,
					errorMessage = "Sign in required"
				});
			}
			else
			{
				_signedIn = true;
				payload = JsonUtility.ToJson(new SignInResult
				{
					profile = new Profile
					{
						userId = userId,
						displayName = displayName,
						photoUrl = string.Format("http://imaginary-photo-cdn.com/{0}/picture", userId),
						idToken = MakeIdToken()
					}
				});
			}
			StartCoroutine(WaitAndDo(latency, () =>
			{
				SendMessage("OnGoogleSignInResultReceived", payload);
			}));
			return true;
		}

		protected override bool PluginSignOut()
		{
			StartCoroutine(WaitAndDo(latency, () =>
			{
				_signedIn = false;
				SendMessage("OnGoogleSignedOut", string.Empty);
			}));
			return true;
		}

		public static bool IsSupported()
		{
			return true;
		}

		public static GoogleSignInBridge Create()
		{
			return PluginUtil.CreateBridgeObject<SimulatedGoogleSignInBridge>();
		}

		private string MakeIdToken()
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(userId + displayName + string.Format("http://imaginary-photo-cdn.com/{0}/picture", userId) + DateTimeOffset.Now));
		}

		private IEnumerator WaitAndDo(float wait, Action action)
		{
			yield return new WaitForSecondsRealtime(wait);
			action();
		}
	}
}
