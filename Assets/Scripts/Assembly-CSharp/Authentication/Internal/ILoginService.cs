using System;
using System.Collections;

namespace Authentication.Internal
{
	public interface ILoginService
	{
		bool isLoggedIn { get; }

		string displayName { get; }

		string alias { get; }

		event Action userCanceledLogin;

		IEnumerator Login(bool allowSilentSignIn, bool allowSignInPrompt);

		IEnumerator Logout();
	}
}
