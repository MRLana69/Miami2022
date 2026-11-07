using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Utility;

namespace Authentication.Internal
{
	public class EditorLogin : ILoginService
	{
		private const float SIMULATED_LOGIN_TIME = 1f;

		private const float SIMULATED_LOGOUT_TIME = 0.5f;

		private const bool SIMULATE_LOGIN_FAIL = false;

		private const string SIMULATED_USER_NAME = "EditorUser";

		private const string SIMULATED_USER_ALIAS = "EditorAlias";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action userCanceledLogin__BackingField;

		public bool isLoggedIn { get; private set; }

		public string displayName
		{
			get
			{
				return (!isLoggedIn) ? string.Empty : "EditorUser";
			}
		}

		public string alias
		{
			get
			{
				return (!isLoggedIn) ? string.Empty : "EditorAlias";
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

		public IEnumerator Login(bool allowSilentSignIn, bool allowSignInPrompt)
		{
			yield return Wait.For(1f).UnscaledSeconds().StartCoroutine();
			isLoggedIn = true;
		}

		public IEnumerator Logout()
		{
			yield return Wait.For(0.5f).UnscaledSeconds().StartCoroutine();
			isLoggedIn = false;
		}
	}
}
