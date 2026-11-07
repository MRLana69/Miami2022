using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.GameCenter;
using UnityEngine;

namespace Authentication.Internal
{
	public class GameCenterLogin : ILoginService
	{
		private const float LOGIN_TIMEOUT = 5f;

		private GameCenterPlugin.LocalPlayer _user;

		private bool _isConsideredLoggedIn;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action userCanceledLogin__BackingField;

		private bool _waitForAuthEvent;

		public bool isLoggedIn
		{
			get
			{
				return _isConsideredLoggedIn && _user.authenticated;
			}
		}

		public string displayName
		{
			get
			{
				return (!isLoggedIn) ? string.Empty : _user.displayName;
			}
		}

		public string alias
		{
			get
			{
				return (!isLoggedIn) ? string.Empty : _user.alias;
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

		public GameCenterLogin()
		{
			GameCenterPlugin.authenticationEvent += OnAuthenticateHandler;
			GameCenterPlugin.Init();
		}

		public IEnumerator Login(bool allowSilentSignIn, bool allowSignInPrompt)
		{
			float timeout = 0f;
			while (!GameCenterPlugin.initialAuthenticationComplete && timeout < 5f)
			{
				yield return null;
				timeout += Time.deltaTime;
			}
			if (GameCenterPlugin.hasLoginView && allowSignInPrompt)
			{
				_waitForAuthEvent = true;
				GameCenterPlugin.ShowLoginView();
				Flurry.LogEventWithAParameter("Native Login", "CompletedLogin", (!_user.authenticated) ? "no" : "yes");
				while (_waitForAuthEvent)
				{
					yield return null;
				}
			}
			_isConsideredLoggedIn = _user.authenticated;
		}

		public IEnumerator Logout()
		{
			_isConsideredLoggedIn = false;
			yield return null;
		}

		private void OnAuthenticateHandler(bool view, int errorCode, string error)
		{
			if (_waitForAuthEvent)
			{
				if (errorCode == 2)
				{
					userCanceledLogin__BackingField.SafeInvoke();
				}
				_waitForAuthEvent = false;
			}
			GameCenterPlugin.GetLocalPlayer(ref _user);
		}
	}
}
