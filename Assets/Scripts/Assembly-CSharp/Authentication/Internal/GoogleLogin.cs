using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.Google;

namespace Authentication.Internal
{
	public class GoogleLogin : ILoginService
	{
		private GoogleSignInPlugin _google;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action userCanceledLogin__BackingField;

		public bool isLoggedIn
		{
			get
			{
				return _google.profileIsUpdated && _google.isSignedIn;
			}
		}

		public string displayName
		{
			get
			{
				return (!isLoggedIn) ? string.Empty : _google.profile.displayName;
			}
		}

		public string alias
		{
			get
			{
				return string.Empty;
			}
		}

		public event Action userCanceledLogin
		{
			add
			{
				Action action = userCanceledLogin__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref userCanceledLogin__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = userCanceledLogin__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref userCanceledLogin__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public GoogleLogin()
		{
			GoogleSignInPlugin.Init();
			_google = GoogleSignInPlugin.instance;
		}

		public IEnumerator Login(bool allowSilentSignIn, bool allowSignInPrompt)
		{
			GoogleSignInPluginRequest requestResult = null;
			if (allowSilentSignIn)
			{
				GoogleSignInPluginRequest request = new GoogleSignInPluginRequest
				{
					silent = true
				};
				yield return _google.SignIn(request, (GoogleSignInPluginRequest result) =>
				{
					requestResult = result;
				});
			}
			if ((!allowSilentSignIn || requestResult.errorCode != 0) && allowSignInPrompt)
			{
				GoogleSignInPluginRequest request2 = new GoogleSignInPluginRequest
				{
					silent = false
				};
				requestResult = null;
				yield return _google.SignIn(request2, (GoogleSignInPluginRequest result) =>
				{
					requestResult = result;
				});
				Flurry.LogEventWithAParameter("Native Login", "CompletedLogin", (requestResult.errorCode != 0) ? "no" : "yes");
				if (requestResult.errorCode == 12501)
				{
					userCanceledLogin__BackingField.SafeInvoke();
				}
			}
		}

		public IEnumerator Logout()
		{
			GoogleSignInPluginRequest request = new GoogleSignInPluginRequest();
			yield return _google.SignOut(request, (GoogleSignInPluginRequest result) =>
			{
			});
		}
	}
}
