using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Authentication.Internal;
using Data.Save;
using Info;
using Kiloo.Common.Backend;
using Kiloo.Common.Backend.Authentication;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using Kiloo.GameCenter;
using Kiloo.Google;
using Kiloo.Integration.GameCenter;
using Kiloo.Integration.Google;
using UnityEngine;

namespace Authentication
{
	public class Auth
	{
		private class AuthResult
		{
			public bool success { get; set; }

			public Error error { get; set; }
		}

		private static Auth _instance;

		private const string DEFAULT_AUTH_BASE_URL = "https://auth.kiloo-games.com/";

		private const string ONLINESETTINGS_AUTH_URL_KEY = "kilooauth_url";

		private const bool ALLOW_ANONYMOUS_IN_EDITOR = true;

		private NativeLogin _native;

		private KilooAuth _kilooAuth;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action authenticatedEvent__BackingField;

		private const int LOGIN_VERSION = 1;

		private static Auth instance
		{
			get
			{
				return _instance ?? (_instance = new Auth());
			}
		}

		private bool allowAnonymous
		{
			get
			{
				return Application.isEditor ? true : false;
			}
		}

		public static string kilooTag
		{
			get
			{
				if (instance._kilooAuth != null && isAuthorized)
				{
					return instance._kilooAuth.kilooTag;
				}
				return string.Empty;
			}
		}

		public static bool isInitialized
		{
			get
			{
				return instance._kilooAuth != null && instance._native != null;
			}
		}

		public static bool isLoggedIn
		{
			get
			{
				return isInitialized && instance._native.isLoggedIn;
			}
		}

		public static bool isAuthorized
		{
			get
			{
				return isInitialized && instance._kilooAuth.state == KilooAuthState.Authorized;
			}
		}

		public static KilooAuth kilooAuth
		{
			get
			{
				return instance._kilooAuth;
			}
		}

		public static string displayName
		{
			get
			{
				return isInitialized ? instance._native.displayName : string.Empty;
			}
		}

		private event Action authenticatedEvent
		{
			add
			{
				Action action = authenticatedEvent__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref authenticatedEvent__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = authenticatedEvent__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref authenticatedEvent__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action authenticated
		{
			add
			{
				instance.authenticatedEvent += value;
			}
			remove
			{
				instance.authenticatedEvent -= value;
			}
		}

		private static bool IsSupported()
		{
			if (DeviceUtility.isKindleDevice)
			{
				return false;
			}
			return true;
		}

		public static Coroutine Init(bool userActivated)
		{
			if (!IsSupported())
			{
				return null;
			}
			bool flag = UserSave.nativeLoginCanceledVersion == 1;
			bool cloudSaveOptedOut = PreferencesSave.cloudSaveOptedOut;
			bool allowSilentSignIn = !userActivated;
			bool allowSignInPrompt = userActivated || (!flag && !cloudSaveOptedOut);
			return CoroutineUtil.StartCoroutine(instance.InitRoutine(allowSilentSignIn, allowSignInPrompt));
		}

		public static Coroutine SignOut()
		{
			return CoroutineUtil.StartCoroutine(instance.LogoutAsync());
		}

		public static void Reset()
		{
			if (_instance._kilooAuth != null && !instance.allowAnonymous)
			{
				instance._kilooAuth.Reset();
			}
			instance._kilooAuth = null;
		}

		public static void CreateKilooAuth()
		{
			if (IsSupported())
			{
				instance.InitKilooAuth();
			}
		}

		private IEnumerator InitRoutine(bool allowSilentSignIn, bool allowSignInPrompt)
		{
			yield return InitNativeAndLoginAsync(allowSilentSignIn, allowSignInPrompt);
			InitKilooAuth();
			if (_native.isLoggedIn)
			{
				AuthResult result = null;
				yield return _kilooAuth.AuthenticateAsync((bool success, Error error) =>
				{
					result = new AuthResult
					{
						success = success,
						error = error
					};
				});
				if (result.success)
				{
					authenticatedEvent__BackingField.SafeInvoke();
					yield break;
				}
				bool authMismatch = false;
				if (result.error.GetErrorCategory() == ErrorCategory.KilooAuth)
				{
					switch ((KilooAuthError)result.error.code)
					{
					case KilooAuthError.GameCenterAccountAlreadyUsed:
					case KilooAuthError.GameCenterAccountMismatch:
					case KilooAuthError.GoogleAccountAlreadyUsed:
					case KilooAuthError.GoogleAccountMismatch:
						authMismatch = true;
						break;
					}
				}
				if (authMismatch)
				{
					_kilooAuth.Reset();
					yield return _kilooAuth.AuthenticateAsync((bool success, Error error) =>
					{
						result = new AuthResult
						{
							success = success,
							error = error
						};
					});
					if (result.success)
					{
						authenticatedEvent__BackingField.SafeInvoke();
					}
				}
			}
			else
			{
				_kilooAuth.Reset();
			}
		}

		private IEnumerator InitNativeAndLoginAsync(bool allowSilentSignIn, bool allowSignInPrompt)
		{
			if (_native == null)
			{
				_native = new NativeLogin();
				_native.userCanceledLogin += OnUserCanceledLogin;
			}
			yield return _native.LoginAsync(allowSilentSignIn, allowSignInPrompt);
		}

		private IEnumerator LogoutAsync()
		{
			if (_native != null)
			{
				yield return _native.LogoutAsync();
			}
		}

		private void InitKilooAuth()
		{
			if (_kilooAuth == null)
			{
				SafeFileSystem directory = new SafeFileSystem(DeviceUtility.GetUserDataPath());
				IJsonSerializer jsonSerializer = new KilooSerializer();
				string value = OnlineSettings.instance.GetValue("kilooauth_url", "https://auth.kiloo-games.com/");
				_kilooAuth = new KilooAuth(new Uri(value), new JsonFileStorage(directory, jsonSerializer), allowAnonymous, jsonSerializer);
				if (GoogleSignInPlugin.isSupported)
				{
					_kilooAuth.AddPlatform(new GoogleKilooAuthPlatform());
				}
				if (GameCenterPlugin.isSupported)
				{
					_kilooAuth.AddPlatform(new GameCenterKilooAuthPlatform());
				}
			}
		}

		private void OnUserCanceledLogin()
		{
			UserSave.nativeLoginCanceledVersion = 1;
		}

		public static void DeleteKilooAuthDebug()
		{
			if (!Globals.DEBUG)
			{
				SDebug.LogError("DEBUG is not enabled - you cannot delete files with current project settings!");
				return;
			}
			string path = Path.Combine(DeviceUtility.GetUserDataPath(), "kilooauth");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		[Conditional("AUTH_LOGS")]
		private static void Log(string msg, UnityEngine.Object obj = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object obj = null)
		{
			SDebug.LogError("[Auth] " + msg, obj);
		}
	}
}
