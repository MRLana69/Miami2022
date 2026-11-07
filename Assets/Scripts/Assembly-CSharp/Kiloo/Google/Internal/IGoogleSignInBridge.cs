using System;
using UnityEngine;

namespace Kiloo.Google.Internal
{
	internal interface IGoogleSignInBridge : IDisposable
	{
		bool IsConnected();

		Coroutine Connect(GoogleSignInBridgeRequest<IGoogleConnectResult> request);

		Coroutine SignIn(GoogleSignInBridgeRequest<IGoogleSignInResult> request, bool silent);

		Coroutine SignOut(GoogleSignInBridgeRequest<IGoogleSignOutResult> request);
	}
}
