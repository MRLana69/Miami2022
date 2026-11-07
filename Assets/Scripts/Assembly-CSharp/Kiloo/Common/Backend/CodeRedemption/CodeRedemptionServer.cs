using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Kiloo.Common.HTTP;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend.CodeRedemption
{
	public sealed class CodeRedemptionServer<T> : ICodeRedemptionServer<T>
	{
		private readonly Uri _baseUri;

		private readonly IJsonSerializer _jsonSerializer;

		private const string API_GET_PAYLOAD = "api/v1/code/payload?code={0}";

		private const string API_CONFIRM_PAYOUT = "api/v1/code/confirm-payout";

		private const float PROMOCODE_TIMEOUT = 10f;

		public CodeRedemptionServer([NotNull] Uri baseUri, [NotNull] IJsonSerializer jsonSerializer)
		{
			if (baseUri == null)
			{
				throw new ArgumentNullException("baseUri");
			}
			if (jsonSerializer == null)
			{
				throw new ArgumentNullException("jsonSerializer");
			}
			_baseUri = baseUri;
			_jsonSerializer = jsonSerializer;
		}

		public Coroutine GetPayloadAsync([NotNull] string code, [NotNull] GetPayloadCompleteHandler<T> handler)
		{
			code = code.Trim();
			if (string.IsNullOrEmpty(code))
			{
				throw new ArgumentNullException("code");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			return KilooWebClient.CreateGet(new Uri(_baseUri, string.Format("api/v1/code/payload?code={0}", code))).WithKilooAntiCheat().WithHeader("Accept", "application/json")
				.WithTimeout(10f)
				.ThenMakeTheRequestAsync((IRequest request, bool fromCache) =>
				{
					IResponse response = request.response;
					T payload = default(T);
					string text = null;
					if (request.isSuccessful)
					{
						ServiceResponse<string> result;
						if (response.TryParseJson<ServiceResponse<string>>(out result, _jsonSerializer) && result != null)
						{
							try
							{
								payload = _jsonSerializer.FromJson<T>(result.result);
							}
							catch (Exception ex)
							{
								text = "The payload could not be parsed.";
								LogError("GetPayloadAsync: " + text + " - Catch Exception: " + ex);
							}
						}
						else
						{
							text = "The response could not be parsed.";
							LogError("GetPayloadAsync: " + text);
						}
					}
					else
					{
						Error serviceError = request.GetServiceError(_jsonSerializer);
						if (serviceError != null)
						{
							text = serviceError.message + ", Errorcode: " + serviceError.code;
							LogError("GetPayloadAsync: Request failed: " + serviceError.category + " error " + serviceError.code + " - " + serviceError.message);
						}
						else
						{
							text = "Request failed: " + request.statusMessage;
							LogError("GetPayloadAsync: Request failed: " + request.statusMessage + "\nException: " + request.exception);
						}
					}
					handler(payload, text);
				});
		}

		public Coroutine PostPayoutConfirmationAsync(string code, PostPayoutConfirmationCompleteHandler handler)
		{
			code = code.Trim();
			if (string.IsNullOrEmpty(code))
			{
				throw new ArgumentNullException("code");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			return KilooWebClient.CreatePost(new Uri(_baseUri, "api/v1/code/confirm-payout")).WithJsonContent(new { code }, _jsonSerializer).WithKilooAntiCheat()
				.WithTimeout(10f)
				.ThenMakeTheRequestAsync((IRequest request, bool fromCache) =>
				{
					IResponse response = request.response;
					string text = null;
					if (!request.isSuccessful)
					{
						Error serviceError = request.GetServiceError(_jsonSerializer);
						if (serviceError != null)
						{
							text = serviceError.message;
							LogError("PostPayoutConfirmationAsync: Request failed: " + serviceError.category + " error " + serviceError.code + " - " + serviceError.message);
						}
						else
						{
							text = "Request failed: " + request.statusMessage;
							LogError("PostPayoutConfirmationAsync: Request failed: " + request.statusMessage + "\nException: " + request.exception);
						}
					}
					if (string.IsNullOrEmpty(text) && !response.IsJson())
					{
						text = "Response is not JSON.";
						LogError("PostPayoutConfirmationAsync: " + text);
					}
					byte[] bytes = response.bytes;
					if (text != null && (bytes == null || bytes.Length == 0))
					{
						text = "Empty response.";
						LogError("PostPayoutConfirmationAsync: " + text);
					}
					handler(text == null, text);
				});
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[CodeRedemption] " + msg);
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("CODE_REDEMPTION_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[CodeRedemption] " + msg);
		}
	}
}
