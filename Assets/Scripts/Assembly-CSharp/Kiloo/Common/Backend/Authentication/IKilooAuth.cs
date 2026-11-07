using UnityEngine;

namespace Kiloo.Common.Backend.Authentication
{
	public interface IKilooAuth
	{
		string kilooTag { get; }

		string accessToken { get; }

		KilooAuthState state { get; }

		Coroutine AuthenticateAsync(AuthenticateCompleteHandler completeHandler = null);

		void Invalidate();
	}
}
