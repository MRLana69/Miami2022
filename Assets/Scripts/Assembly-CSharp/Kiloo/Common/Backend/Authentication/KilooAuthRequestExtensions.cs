using System;
using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using Kiloo.Common.HTTP;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend.Authentication
{
	public static class KilooAuthRequestExtensions
	{
		private const string AUTHORIZATION_HEADER = "Authorization";

		private const string AUTHORIZATION_FORMAT = "KilooAuth AccessToken=\"{0}\"";

		public static IRequest WithKilooAuth([NotNull] this IRequest request, [NotNull] IKilooAuth kilooAuth)
		{
			if (kilooAuth.state == KilooAuthState.Authorized)
			{
				if (request.uri.Scheme != "https")
				{
					throw new InvalidOperationException("KilooAuth only allowed when using https.");
				}
				request.headers["Authorization"] = string.Format("KilooAuth AccessToken=\"{0}\"", kilooAuth.accessToken);
				return request;
			}
			throw new InvalidOperationException(string.Concat("KilooAuth state must Authorized (current state: ", kilooAuth.state, ")"));
		}

		public static void SendAuthenticated([NotNull] this IRequest request, IKilooAuth kilooAuth, WebClient.SendOptions options = default(WebClient.SendOptions))
		{
			try
			{
				request.WithKilooAuth(kilooAuth);
				request.Send(options);
				if (!request.isSuccessful && request.IsAuthenticationErrorResponse())
				{
					kilooAuth.Invalidate();
				}
			}
			catch (Exception exception)
			{
				request.exception = exception;
				request.state = RequestState.Error;
				throw;
			}
		}

		public static Coroutine SendAuthenticatedAsync([NotNull] this IRequest request, [NotNull] IKilooAuth kilooAuth, WebClient.RequestCompleteHandler onComplete = null, WebClient.SendOptions options = default(WebClient.SendOptions))
		{
			return CoroutineUtil.StartCoroutine(SendAuthenticatedAsyncCoroutine(request, kilooAuth, onComplete, options));
		}

		public static bool IsAuthenticationErrorResponse(this IRequest request)
		{
			return request.response != null && request.response.status == 401;
		}

		private static IEnumerator SendAuthenticatedAsyncCoroutine(IRequest request, IKilooAuth kilooAuth, WebClient.RequestCompleteHandler onComplete, WebClient.SendOptions options)
		{
			bool isFromCache = false;
			int tries = 0;
			bool retry;
			do
			{
				retry = false;
				tries++;
				yield return kilooAuth.AuthenticateAsync((bool success, Error error) =>
				{
					if (!success)
					{
						try
						{
							throw (error == null) ? new KilooBackendException("Authentication failed") : new KilooBackendException(error);
						}
						catch (Exception exception)
						{
							request.exception = exception;
							request.state = RequestState.Error;
						}
					}
				});
				if (kilooAuth.state != KilooAuthState.Authorized)
				{
					break;
				}
				request.WithKilooAuth(kilooAuth);
				yield return request.SendAsync((IRequest rqst, bool fromCache) =>
				{
					isFromCache = fromCache;
				}, options);
				if (!request.isSuccessful && request.IsAuthenticationErrorResponse())
				{
					kilooAuth.Invalidate();
					retry = true;
					if (tries != 1)
					{
					}
				}
			}
			while (retry && tries <= 1);
			if (onComplete != null)
			{
				onComplete(request, isFromCache);
			}
		}

		[Conditional("KILOO_AUTH_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[KilooAuthRequestExtensions] " + msg);
		}
	}
}
