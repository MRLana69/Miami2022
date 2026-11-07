using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Authentication.Internal
{
	public class NativeLogin
	{
		private const bool SIMULATE_IN_EDITOR = true;

		private readonly ILoginService _service;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action userCanceledLogin__BackingField;

		private bool isInitialized
		{
			get
			{
				return _service != null;
			}
		}

		public bool isLoggedIn
		{
			get
			{
				return isInitialized && _service.isLoggedIn;
			}
		}

		public string displayName
		{
			get
			{
				if (isLoggedIn)
				{
					if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						return _service.alias;
					}
					return _service.displayName;
				}
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

		public NativeLogin()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				_service = new GoogleLogin();
				break;
			case RuntimePlatform.IPhonePlayer:
				_service = new GameCenterLogin();
				break;
			default:
				if (Application.isEditor)
				{
					_service = new EditorLogin();
				}
				break;
			}
			if (_service != null)
			{
				_service.userCanceledLogin += () =>
				{
					userCanceledLogin__BackingField.SafeInvoke();
				};
			}
		}

		public Coroutine LoginAsync(bool allowSilentSignIn, bool allowSignInPrompt)
		{
			return CoroutineUtil.StartCoroutine(LoginRoutine(allowSilentSignIn, allowSignInPrompt));
		}

		public Coroutine LogoutAsync()
		{
			return CoroutineUtil.StartCoroutine(LogoutRoutine());
		}

		private IEnumerator LoginRoutine(bool allowSilentSignIn, bool allowSignInPrompt)
		{
			yield return (!isInitialized) ? null : CoroutineUtil.StartCoroutine(_service.Login(allowSilentSignIn, allowSignInPrompt));
		}

		private IEnumerator LogoutRoutine()
		{
			yield return (!isInitialized) ? null : CoroutineUtil.StartCoroutine(_service.Logout());
		}

		[Conditional("NATIVELOGIN_LOGS")]
		private static void Log(string msg, UnityEngine.Object obj = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object obj = null)
		{
			SDebug.LogError("[NativeLogin] " + msg, obj);
		}
	}
}
