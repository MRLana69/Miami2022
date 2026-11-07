using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Kiloo.Common.HTTP;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend.Authentication
{
	public sealed class KilooAuth : IKilooAuth
	{
		public delegate void DetachPlatformsCompleteHandler(bool success, Error error);

		public class TempCode
		{
			public string code;

			public DateTimeOffset expires;
		}

		public delegate void GenerateTemporaryCodeCompleteHandler(TempCode tempCode, Error error);

		private struct PersistentAuthData
		{
			public string loginToken;

			public KilooAuthState state;

			public DateTime expires;

			public string kilooId;

			public string kilooTag;

			public Dictionary<string, string> signatures;
		}

		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class AccessTokenResult
		{
			public string accessToken;

			public string kilooId;

			public string kilooTag;

			public DateTime expires = default(DateTime);
		}

		public const string STORAGE_KEY = "kilooauth";

		public const string KILOO_ID_KEY = "kilooId";

		public const string TEMPORARY_CODE_KEY = "temporaryCode";

		private const float TIME_TOLERANCE = 60f;

		private const string API_CREATE = "api/v3/user/create";

		private const string API_LOGIN = "api/v3/user/login";

		private const string API_DETACH = "api/v3/user/detach";

		private const string API_GENERATE_TEMPORARY_CODE = "api/v3/user/generatetemporarycode";

		private readonly Uri _baseUri;

		private readonly IStorage _storage;

		private readonly IJsonSerializer _jsonSerializer;

		private readonly Dictionary<string, IKilooAuthPlatform> _platforms;

		private PersistentAuthData _data;

		public string temporaryCode { get; set; }

		public string kilooTag
		{
			get
			{
				return _data.kilooTag;
			}
		}

		public string accessToken
		{
			get
			{
				return _data.loginToken;
			}
		}

		public KilooAuthState state
		{
			get
			{
				if (_data.state == KilooAuthState.Authorized && _data.expires < DateTimeOffset.UtcNow)
				{
					_data.state = KilooAuthState.Expired;
				}
				return _data.state;
			}
			private set
			{
				_data.state = value;
			}
		}

		public bool allowAnonymous { get; set; }

		public IEnumerable<IKilooAuthPlatform> platforms
		{
			get
			{
				foreach (IKilooAuthPlatform value in _platforms.Values)
				{
					yield return value;
				}
			}
		}

		public KilooAuth([NotNull] Uri baseUri, [NotNull] IStorage storage, bool allowAnonymous, IJsonSerializer jsonSerializer, params IKilooAuthPlatform[] platforms)
			: this(baseUri, storage, allowAnonymous, jsonSerializer, (IEnumerable<IKilooAuthPlatform>)platforms)
		{
		}

		public KilooAuth([NotNull] Uri baseUri, [NotNull] IStorage storage, bool allowAnonymous, [NotNull] IJsonSerializer jsonSerializer, IEnumerable<IKilooAuthPlatform> platforms)
		{
			if (baseUri == null)
			{
				throw new ArgumentNullException("baseUri");
			}
			if (storage == null)
			{
				throw new ArgumentNullException("storage");
			}
			if (jsonSerializer == null)
			{
				throw new ArgumentNullException("jsonSerializer");
			}
			if (baseUri.Scheme != "https")
			{
				throw new InvalidOperationException("KilooAuth only allowed when using https.");
			}
			_baseUri = baseUri;
			_storage = storage;
			_jsonSerializer = jsonSerializer;
			this.allowAnonymous = allowAnonymous;
			_platforms = new Dictionary<string, IKilooAuthPlatform>();
			if (platforms != null)
			{
				foreach (IKilooAuthPlatform platform in platforms)
				{
					_platforms[platform.name] = platform;
				}
			}
			Load();
		}

		public Coroutine AuthenticateAsync(AuthenticateCompleteHandler completeHandler = null)
		{
			return CoroutineUtil.StartCoroutine(AuthenticateCoroutine(completeHandler));
		}

		public void Invalidate()
		{
			if (state == KilooAuthState.Authorized)
			{
				state = KilooAuthState.Invalid;
				Save();
			}
			else
			{
				LogWarning("Invalidate called when not Authorized. This has no effect.");
			}
		}

		public void AddPlatform(IKilooAuthPlatform platform)
		{
			_platforms[platform.name] = platform;
		}

		public void RemovePlatform(string platformKey)
		{
			_platforms.Remove(platformKey);
		}

		public bool HasPlatform(string platformKey)
		{
			return _platforms.ContainsKey(platformKey);
		}

		public Coroutine DetachPlatformsAsync(string[] platformsToDetach, DetachPlatformsCompleteHandler completeHandler = null)
		{
			return CoroutineUtil.StartCoroutine(DetachPlatformsCoroutine(platformsToDetach, completeHandler));
		}

		public Coroutine GenerateTemporaryCodeAsync(GenerateTemporaryCodeCompleteHandler completeHandler = null)
		{
			return CoroutineUtil.StartCoroutine(GenerateTemporaryCodeCoroutine(completeHandler));
		}

		public void Reset()
		{
			temporaryCode = null;
			_data = default(PersistentAuthData);
			Save();
		}

		private bool CanAuthenticate()
		{
			return allowAnonymous || _platforms.Any((KeyValuePair<string, IKilooAuthPlatform> pair) => pair.Value.value != null);
		}

		private bool IsCachedTokenValid()
		{
			return state == KilooAuthState.Authorized && !string.IsNullOrEmpty(accessToken) && _platforms.All((KeyValuePair<string, IKilooAuthPlatform> pair) => pair.Value.signature == GetAuthenticatedSignature(pair.Value.name));
		}

		private Dictionary<string, object> BuildAuthenticationRequestContent(out Dictionary<string, string> signatures)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (_data.kilooId != null)
			{
				dictionary["kilooId"] = _data.kilooId;
			}
			else if (temporaryCode != null)
			{
				dictionary["temporaryCode"] = temporaryCode;
			}
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (IKilooAuthPlatform value2 in _platforms.Values)
			{
				IDictionary<string, string> value = value2.value;
				if (value != null)
				{
					string signature = value2.signature;
					string name = value2.name;
					dictionary2[name] = signature;
					if (_data.kilooId == null || signature != GetAuthenticatedSignature(name))
					{
						dictionary[name] = value;
					}
				}
			}
			signatures = dictionary2;
			return dictionary;
		}

		private Error HandleAuthenticationResult(IRequest request, Dictionary<string, string> signatures)
		{
			Error result = null;
			IResponse response = request.response;
			ServiceResponse<AccessTokenResult> result2 = null;
			if (response != null)
			{
				response.TryParseJson<ServiceResponse<AccessTokenResult>>(out result2, _jsonSerializer);
			}
			if (request.isSuccessful && result2 != null && result2.result != null)
			{
				AccessTokenResult result3 = result2.result;
				_data.loginToken = result3.accessToken;
				_data.kilooId = result3.kilooId;
				_data.kilooTag = result3.kilooTag;
				state = KilooAuthState.Authorized;
				_data.expires = result3.expires.AddSeconds(-60.0);
				DateTime utcNow = DateTime.UtcNow;
				DateTime? dateTime = response.headers.Get("Date", null, false);
				if (dateTime.HasValue)
				{
					TimeSpan timeSpan = dateTime.Value - utcNow;
					try
					{
						_data.expires -= timeSpan;
					}
					catch (Exception)
					{
					}
				}
				if (_data.expires < utcNow)
				{
					LogError("Access token expire time returned by server is in the past. This is an error.");
					_data.expires = utcNow.AddYears(1);
				}
				_data.signatures = signatures;
			}
			else
			{
				if (result2 != null && result2.error != null)
				{
					result = result2.error;
				}
				else
				{
					Error error = new Error();
					error.category = ErrorCategory.HTTP.ToString();
					error.code = ((request.response != null) ? request.response.status : 0);
					error.message = "Authenticate request failed: " + request.statusMessage;
					result = error;
				}
				state = KilooAuthState.Error;
			}
			Save();
			return result;
		}

		private IEnumerator AuthenticateCoroutine(AuthenticateCompleteHandler completeHandler)
		{
			Error error = null;
			if (state == KilooAuthState.Authenticating)
			{
			}
			while (state == KilooAuthState.Authenticating)
			{
				yield return null;
			}
			List<YieldInstruction> updates = new List<YieldInstruction>();
			foreach (IKilooAuthPlatform value in _platforms.Values)
			{
				updates.Add(value.UpdateAsync());
			}
			yield return CoroutineUtil.WaitForAll(updates);
			if (CanAuthenticate())
			{
				if (!IsCachedTokenValid())
				{
					state = KilooAuthState.Authenticating;
					Dictionary<string, string> signatures;
					Dictionary<string, object> authRequestData = BuildAuthenticationRequestContent(out signatures);
					IRequest request = ((authRequestData.Count != 0) ? KilooWebClient.CreatePost(new Uri(_baseUri, "api/v3/user/login")).WithJsonContent(authRequestData, _jsonSerializer) : KilooWebClient.CreatePost(new Uri(_baseUri, "api/v3/user/create")));
					yield return request.SendAsync(null, new WebClient.SendOptions
					{
						debugOutputLimit = int.MaxValue
					});
					error = HandleAuthenticationResult(request, signatures);
				}
			}
			else
			{
				error = new Error
				{
					category = ErrorCategory.KilooAuth.ToString(),
					code = KilooAuthError.NoPlatformsLoggedIn.GetErrorCode(),
					message = "Could not authenticate, because user is not logged in to any of the supported platforms"
				};
				Save();
			}
			if (completeHandler != null)
			{
				completeHandler(state == KilooAuthState.Authorized, error);
			}
		}

		private IEnumerator DetachPlatformsCoroutine(string[] platformsToDetach, DetachPlatformsCompleteHandler completeHandler)
		{
			if (platformsToDetach == null || platformsToDetach.Length == 0)
			{
				yield break;
			}
			if (state != KilooAuthState.Authorized)
			{
				LogError("DetachPlatformsAsync: Not authorized.");
				yield break;
			}
			IRequest request = KilooWebClient.CreatePost(new Uri(_baseUri, "api/v3/user/detach")).WithKilooAuth(this).WithJsonContent(new
			{
				detach = platformsToDetach
			}, _jsonSerializer);
			yield return request.SendAsync();
			IResponse response = request.response;
			Error error = null;
			ServiceResponse<AccessTokenResult> responseData = null;
			if (response != null)
			{
				response.TryParseJson<ServiceResponse<AccessTokenResult>>(out responseData, _jsonSerializer);
			}
			if (request.isSuccessful && responseData != null && responseData.result != null)
			{
				_data.loginToken = responseData.result.accessToken;
				foreach (string key in platformsToDetach)
				{
					_data.signatures.Remove(key);
				}
				Save();
			}
			else
			{
				error = ((responseData == null || responseData.error == null) ? new Error
				{
					category = ErrorCategory.HTTP.ToString(),
					code = ((request.response != null) ? request.response.status : 0),
					message = "Detach request failed: " + request.statusMessage
				} : responseData.error);
				LogError("DetachPlatformsAsync: " + error.message);
			}
			if (completeHandler != null)
			{
				completeHandler(error == null, error);
			}
		}

		private IEnumerator GenerateTemporaryCodeCoroutine(GenerateTemporaryCodeCompleteHandler completeHandler)
		{
			if (state != KilooAuthState.Authorized)
			{
				LogError("GenerateTemporaryCodeAsync: Not authorized.");
				yield break;
			}
			IRequest request = KilooWebClient.CreatePost(new Uri(_baseUri, "api/v3/user/generatetemporarycode")).WithKilooAuth(this);
			yield return request.SendAsync();
			IResponse response = request.response;
			Error error = null;
			ServiceResponse<TempCode> responseData = null;
			if (response != null)
			{
				response.TryParseJson<ServiceResponse<TempCode>>(out responseData, _jsonSerializer);
			}
			TempCode result = null;
			if (request.isSuccessful && responseData != null && responseData.result != null)
			{
				result = responseData.result;
				temporaryCode = result.code;
			}
			else
			{
				error = ((responseData == null || responseData.error == null) ? new Error
				{
					category = ErrorCategory.HTTP.ToString(),
					code = ((request.response != null) ? request.response.status : 0),
					message = "Generate temporary code request failed: " + request.statusMessage
				} : responseData.error);
				LogError("GenerateTemporaryCodeAsync: " + error.message);
			}
			if (completeHandler != null)
			{
				completeHandler(result, error);
			}
		}

		private void Load()
		{
			try
			{
				if (_storage.ItemExists("kilooauth"))
				{
					_data = _storage.Load<PersistentAuthData>("kilooauth");
					return;
				}
			}
			catch (Exception ex)
			{
				LogError("Load error: " + ex);
			}
			_data = default(PersistentAuthData);
		}

		private void Save()
		{
			try
			{
				_storage.Save("kilooauth", _data);
			}
			catch (Exception ex)
			{
				LogError("Save error: " + ex);
			}
		}

		private string GetAuthenticatedSignature(string platformName)
		{
			string value = null;
			if (_data.signatures != null)
			{
				_data.signatures.TryGetValue(platformName, out value);
			}
			return value;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[KilooAuth] " + msg);
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[KilooAuth] " + msg);
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("KILOO_AUTH_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[KilooAuth] " + msg);
		}
	}
}
