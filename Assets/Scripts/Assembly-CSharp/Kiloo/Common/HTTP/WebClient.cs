using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.HTTP.Internal;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP
{
	public static class WebClient
	{
		public struct SendOptions
		{
			public bool disableCache;

			public bool useCacheOnError;

			public float simulatedRequestLatency;

			public IList<SimulatedRequestResult> simulatedRequestResults;

			public int debugOutputLimit;
		}

		public delegate void RequestCompleteHandler(IRequest request, bool isFromCache);

		private class CacheInfo
		{
			public bool expired = true;

			public CacheMetaData metadata;

			public string filename;

			public string metaFilename;
		}

		[Flags]
		public enum ClearCacheFlags
		{
			All = 1,
			Expired = 2,
			Old = 4
		}

		private class CacheMetaData
		{
			public DateTime timestamp;

			public Headers requestHeaders;

			public Headers responseHeaders;
		}

		public const string META_DATA_SUFFIX = ".metadata";

		public const string KILOO_DEVICE_TIME_HEADER = "Kiloo-Device-Time";

		private static string _cachePath = null;

		private static readonly string[] _cachedHeadersToUpdate = new string[6] { "Date", "ETag", "Content-Location", "Expires", "Cache-Control", "Vary" };

		public static float defaultSimulatedRequestLatency = 0f;

		public static IList<SimulatedRequestResult> defaultSimulatedRequestResults = null;

		private static System.Random _random = null;

		public static float timeout { get; set; }

		public static string cachePath
		{
			get
			{
				if (_cachePath == null)
				{
					string text = DeviceUtility.GetCachePath();
					if (string.IsNullOrEmpty(text))
					{
						throw new Exception("Unable to get cache path");
					}
					_cachePath = Path.Combine(text, "webclientcache");
					if (!Directory.Exists(_cachePath))
					{
						Directory.CreateDirectory(_cachePath);
					}
				}
				return _cachePath;
			}
		}

		[Pure]
		public static Uri AddQueryParametersToUri(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters)
		{
			if (queryParameters != null)
			{
				StringBuilder stringBuilder = null;
				foreach (KeyValuePair<string, string> item in queryParameters.OrderBy((KeyValuePair<string, string> pair) => pair.Key))
				{
					if (stringBuilder == null)
					{
						string query = uri.Query;
						if (query.Length > 1)
						{
							stringBuilder = new StringBuilder(query, 1, query.Length - 1, 256);
							stringBuilder.Append('&');
						}
						else
						{
							stringBuilder = new StringBuilder(256);
						}
					}
					else
					{
						stringBuilder.Append('&');
					}
					stringBuilder.Append(Uri.EscapeDataString(item.Key));
					stringBuilder.Append('=');
					stringBuilder.Append(Uri.EscapeDataString(item.Value));
				}
				if (stringBuilder != null)
				{
					UriBuilder uriBuilder = new UriBuilder(uri);
					uriBuilder.Query = stringBuilder.ToString();
					uri = uriBuilder.Uri;
				}
			}
			return uri;
		}

		public static Uri AddQueryParametersToUri(string uri, IEnumerable<KeyValuePair<string, string>> queryParameters)
		{
			return AddQueryParametersToUri(new Uri(uri), queryParameters);
		}

		public static IRequest CreatePost(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreatePost(AddQueryParametersToUri(uri, queryParameters));
		}

		public static IRequest CreateGet(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreateGet(AddQueryParametersToUri(uri, queryParameters));
		}

		public static IRequest CreatePut(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreatePut(AddQueryParametersToUri(uri, queryParameters));
		}

		public static IRequest CreateDelete(Uri uri, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
		{
			return HTTPFactory.CreateDelete(AddQueryParametersToUri(uri, queryParameters));
		}

		[Obsolete("Use SendRequestAsync instead")]
		public static Coroutine RequestAsync(IRequest request, RequestCompleteHandler onComplete = null, SendOptions options = default(SendOptions))
		{
			return SendRequestAsync(request, onComplete, options);
		}

		public static Coroutine SendRequestAsync(IRequest request, RequestCompleteHandler onComplete = null, SendOptions options = default(SendOptions))
		{
			return CoroutineUtil.StartCoroutine(request.SendRequestCoroutine(onComplete, options));
		}

		[Obsolete("Use SendRequest instead")]
		public static IRequest Request(IRequest request, SendOptions options)
		{
			return SendRequest(request, options);
		}

		public static IRequest SendRequest(IRequest request, SendOptions options = default(SendOptions))
		{
			request.PrepareFetchResponse();
			PrepareRequest(request);
			CacheInfo cacheInfo = ((!options.disableCache) ? HandleCacheBeforeRequest(request) : null);
			if (cacheInfo == null || cacheInfo.expired)
			{
				Action action = null;
				float num = 0f;
				if (num > 0f)
				{
					Thread.Sleep(Util.ConvertTimeoutToMilliseconds(num));
				}
				if (action != null)
				{
					action();
				}
				else
				{
					if (request.timeout <= 0f)
					{
						request.timeout = timeout;
					}
					request.FetchResponse();
				}
				if (cacheInfo != null)
				{
					HandleCacheAfterRequest(request, cacheInfo, options.useCacheOnError);
				}
			}
			if (cacheInfo != null && cacheInfo.metadata != null)
			{
				byte[] bytes = LoadCacheContents(cacheInfo.filename);
				request.response = HTTPFactory.CreateResponse(cacheInfo.metadata.responseHeaders, bytes);
				request.state = RequestState.Done;
			}
			if (!request.isSuccessful)
			{
				if (request.exception != null)
				{
					LogError("SendRequest Failed: \"" + request.uri.AbsoluteUri + "\" exception: " + request.exception);
				}
				else
				{
					LogError("SendRequest Failed: \"" + request.uri.AbsoluteUri + "\" status: " + request.statusMessage);
				}
			}
			return request;
		}

		private static void PrepareRequest(IRequest request)
		{
			request.PrepareFetchResponse();
			if (request.headers.Contains("Kiloo-Device-Time"))
			{
				request.headers["Kiloo-Device-Time"] = '"' + DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz", DateTimeFormatInfo.InvariantInfo) + '"';
			}
		}

		private static IEnumerator SendRequestCoroutine(this IRequest request, RequestCompleteHandler onComplete, SendOptions options)
		{
			PrepareRequest(request);
			CacheInfo cache = ((!options.disableCache) ? HandleCacheBeforeRequest(request) : null);
			if (cache == null || cache.expired)
			{
				Action applySimulationToRequest = null;
				float waitBeforeApply = 0f;
				if (waitBeforeApply > 0f)
				{
					yield return new WaitForSecondsRealtime(waitBeforeApply);
				}
				if (applySimulationToRequest != null)
				{
					applySimulationToRequest();
				}
				else
				{
					if (request.timeout <= 0f)
					{
						request.timeout = timeout;
					}
					float requestTimeout = request.timeout;
					float startTime = Time.realtimeSinceStartup;
					request.FetchResponseAsync();
					while (!request.isFinished)
					{
						if (requestTimeout > 0f && Time.realtimeSinceStartup - startTime > requestTimeout)
						{
							request.state = RequestState.TimedOut;
							break;
						}
						yield return null;
					}
				}
				if (cache != null)
				{
					HandleCacheAfterRequest(request, cache, options.useCacheOnError);
				}
			}
			bool useCachedResponse = cache != null && cache.metadata != null;
			if (useCachedResponse)
			{
				byte[] bytes = LoadCacheContents(cache.filename);
				request.response = HTTPFactory.CreateResponse(cache.metadata.responseHeaders, bytes);
				request.state = RequestState.Done;
			}
			if (!request.isSuccessful)
			{
				if (request.exception != null)
				{
					LogError("SendRequestAsync Failed: \"" + request.uri.AbsoluteUri + "\" exception: " + request.exception);
				}
				else
				{
					LogError("SendRequestAsync Failed: \"" + request.uri.AbsoluteUri + "\" status: " + request.statusMessage);
				}
			}
			if (onComplete != null)
			{
				onComplete(request, useCachedResponse);
			}
		}

		private static CacheInfo HandleCacheBeforeRequest(IRequest request)
		{
			if (!"GET".Equals(request.method, StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}
			CacheInfo cacheInfo = new CacheInfo();
			try
			{
				string text = cachePath;
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string path = StringUtility.ToHexString(Checksum.ComputeSHA1(request.uri.ToString()));
				cacheInfo.filename = Path.Combine(text, path);
				cacheInfo.metaFilename = cacheInfo.filename + ".metadata";
				if (File.Exists(cacheInfo.filename))
				{
					try
					{
						cacheInfo.metadata = LoadCacheMetaData(cacheInfo.metaFilename);
						string text2 = cacheInfo.metadata.responseHeaders["Vary"];
						if (!string.IsNullOrEmpty(text2))
						{
							if (text2 == "*")
							{
								cacheInfo.metadata = null;
							}
							else
							{
								string[] array = text2.Split(',');
								for (int i = 0; i < array.Length; i++)
								{
									string name = array[i].Trim();
									if (request.headers[name] != cacheInfo.metadata.requestHeaders[name])
									{
										cacheInfo.metadata = null;
										break;
									}
								}
							}
						}
						if (cacheInfo.metadata != null)
						{
							DateTime dateTime = cacheInfo.metadata.responseHeaders.Get("Expires", DateTime.MinValue);
							if (DateTime.UtcNow < dateTime)
							{
								cacheInfo.expired = false;
							}
							if (cacheInfo.expired)
							{
								string value = cacheInfo.metadata.responseHeaders["ETag"];
								if (!string.IsNullOrEmpty(value))
								{
									request.headers["If-None-Match"] = value;
								}
								string value2 = cacheInfo.metadata.responseHeaders["Last-Modified"];
								if (!string.IsNullOrEmpty(value2))
								{
									request.headers["If-Modified-Since"] = value2;
								}
							}
						}
					}
					catch (Exception ex)
					{
						LogError("Could not load headers for cache: " + ex);
						cacheInfo.metadata = null;
					}
				}
			}
			catch (Exception ex2)
			{
				LogError("Error handling cache, cache disabled: " + ex2);
				return null;
			}
			return cacheInfo;
		}

		private static void HandleCacheAfterRequest(IRequest request, CacheInfo cacheInfo, bool useCacheOnError)
		{
			if (request.response != null)
			{
				IResponse response = request.response;
				int status = response.status;
				Headers headers = response.headers;
				DateTime dateTime = headers.Get("Expires", DateTime.MinValue);
				bool flag = dateTime > DateTime.UtcNow;
				string value = headers["ETag"];
				string value2 = headers["Last-Modified"];
				if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2))
				{
					flag = true;
				}
				switch (status)
				{
				case 200:
					cacheInfo.metadata = null;
					if (flag)
					{
						SaveCacheMetaData(cacheInfo.metaFilename, new CacheMetaData
						{
							responseHeaders = headers,
							requestHeaders = request.headers
						});
						SaveCacheContents(cacheInfo.filename, response.bytes);
					}
					break;
				case 304:
					if (cacheInfo.metadata != null)
					{
						cacheInfo.metadata.requestHeaders = request.headers;
						UpdateCachedHeaders(cacheInfo.metadata.responseHeaders, headers);
						SaveCacheMetaData(cacheInfo.metaFilename, cacheInfo.metadata);
					}
					break;
				default:
					if (!useCacheOnError)
					{
						cacheInfo.metadata = null;
					}
					break;
				}
			}
			else if (useCacheOnError)
			{
				LogWarning("No response - using cached version");
			}
			else
			{
				cacheInfo.metadata = null;
			}
		}

		private static void UpdateCachedHeaders(Headers cached, Headers updated)
		{
			string[] cachedHeadersToUpdate = _cachedHeadersToUpdate;
			foreach (string name in cachedHeadersToUpdate)
			{
				if (updated.Contains(name))
				{
					cached.Remove(name);
					IEnumerable<string> all = updated.GetAll(name);
					cached.SetAll(name, all);
				}
			}
		}

		public static void ClearCache(ClearCacheFlags flags, float olderThanSeconds)
		{
			try
			{
				string path = cachePath;
				if (!Directory.Exists(path))
				{
					return;
				}
				if ((flags & ClearCacheFlags.All) != 0)
				{
					string[] files = Directory.GetFiles(path);
					string[] array = files;
					foreach (string path2 in array)
					{
						File.Delete(path2);
					}
					return;
				}
				DateTime utcNow = DateTime.UtcNow;
				string[] files2 = Directory.GetFiles(path);
				string[] array2 = files2;
				foreach (string text in array2)
				{
					if (!text.EndsWith(".metadata"))
					{
						continue;
					}
					bool flag = false;
					try
					{
						CacheMetaData cacheMetaData = LoadCacheMetaData(text);
						if ((flags & ClearCacheFlags.Old) != 0)
						{
							double totalSeconds = (utcNow - cacheMetaData.timestamp).TotalSeconds;
							if (totalSeconds > (double)olderThanSeconds)
							{
								flag = true;
							}
						}
						if (!flag && (flags & ClearCacheFlags.Expired) != 0)
						{
							DateTime dateTime = cacheMetaData.responseHeaders.Get("Expires", DateTime.MaxValue);
							if (dateTime <= utcNow)
							{
								flag = true;
							}
						}
					}
					catch (Exception ex)
					{
						LogError("Error while clearing cache: " + ex);
						flag = true;
					}
					if (!flag)
					{
						continue;
					}
					try
					{
						File.Delete(text);
						string text2 = text.Remove(text.Length - ".metadata".Length);
						if (text2.Length > 0)
						{
							File.Delete(text2);
						}
					}
					catch (Exception ex2)
					{
						LogError("Error while clearing cache: " + ex2);
					}
				}
			}
			catch (Exception ex3)
			{
				LogError("Error while clearing cache: " + ex3);
			}
		}

		private static void SaveCacheMetaData(string file, CacheMetaData metadata)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(file, FileMode.Create, FileAccess.Write)))
			{
				metadata.timestamp = DateTime.UtcNow;
				binaryWriter.Write(metadata.timestamp.ToUnixTime());
				metadata.responseHeaders.WriteTo(binaryWriter);
				metadata.requestHeaders.WriteTo(binaryWriter);
			}
		}

		private static CacheMetaData LoadCacheMetaData(string file)
		{
			CacheMetaData cacheMetaData = new CacheMetaData();
			using (BinaryReader binaryReader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read)))
			{
				cacheMetaData.timestamp = TimeUtility.FromUnixTime(binaryReader.ReadInt64());
				cacheMetaData.responseHeaders = new Headers();
				cacheMetaData.responseHeaders.ReadFrom(binaryReader);
				cacheMetaData.requestHeaders = new Headers();
				cacheMetaData.requestHeaders.ReadFrom(binaryReader);
				return cacheMetaData;
			}
		}

		private static void SaveCacheContents(string file, byte[] bytes)
		{
			byte[] bytes2 = bytes ?? new byte[0];
			FileUtil.WriteAllBytes(file, bytes2);
		}

		private static byte[] LoadCacheContents(string file)
		{
			byte[] array = FileUtil.ReadAllBytes(file);
			if (array.Length == 0)
			{
				array = null;
			}
			return array;
		}

		[Obsolete("Not used in Unity versions 5.4 or newer.")]
		public static void CloseExpiredIdleConnections()
		{
			Connection.CloseExpiredIdleConnections();
		}

		[Obsolete("Not used in Unity versions 5.4 or newer.")]
		public static void CloseIdleConnectionsToHost(string host)
		{
			Connection.CloseIdleConnectionsToHost(host);
		}

		[Obsolete("Not used in Unity versions 5.4 or newer.")]
		public static void CloseIdleConnections()
		{
			Connection.CloseIdleConnections();
		}

		[Conditional("HTTP_SIMULATION_ENABLED")]
		private static void ApplyDefaultSimulationParameters(ref SendOptions options)
		{
			if (options.simulatedRequestLatency <= 0f)
			{
				options.simulatedRequestLatency = defaultSimulatedRequestLatency;
			}
			if (options.simulatedRequestResults == null)
			{
				options.simulatedRequestResults = defaultSimulatedRequestResults;
			}
		}

		private static float GetSimulatedRequestResponseChanceTotal(IList<SimulatedRequestResult> simulatedRequestResults)
		{
			if (simulatedRequestResults != null)
			{
				float num = 0f;
				{
					foreach (SimulatedRequestResult simulatedRequestResult in simulatedRequestResults)
					{
						if (simulatedRequestResult.chance > 0f)
						{
							num += simulatedRequestResult.chance;
						}
					}
					return num;
				}
			}
			return 0f;
		}

		private static SimulatedRequestResult GetRandomSimulatedRequestResult(IList<SimulatedRequestResult> simulatedRequestResults)
		{
			float simulatedRequestResponseChanceTotal = GetSimulatedRequestResponseChanceTotal(simulatedRequestResults);
			if (simulatedRequestResponseChanceTotal > 0f)
			{
				if (_random == null)
				{
					_random = new System.Random();
				}
				double num = _random.NextDouble() * (double)simulatedRequestResponseChanceTotal;
				if (num < (double)simulatedRequestResponseChanceTotal)
				{
					foreach (SimulatedRequestResult simulatedRequestResult in simulatedRequestResults)
					{
						if (simulatedRequestResult.chance >= 0f)
						{
							num -= (double)simulatedRequestResult.chance;
							if (num <= 0.0)
							{
								return simulatedRequestResult;
							}
						}
					}
				}
			}
			return null;
		}

		[Conditional("HTTP_SIMULATION_ENABLED")]
		private static void DoSimulation(IRequest request, ref Action applySimulationToRequest, ref float waitTimeBeforeApply, float simulatedRequestLatency, IList<SimulatedRequestResult> simulatedRequestResults)
		{
			waitTimeBeforeApply = simulatedRequestLatency;
			applySimulationToRequest = null;
			SimulatedRequestResult simulatedResult = GetRandomSimulatedRequestResult(simulatedRequestResults);
			if (simulatedResult == null)
			{
				return;
			}
			if (simulatedResult.exception != null)
			{
				waitTimeBeforeApply = simulatedResult.latency;
			}
			else
			{
				waitTimeBeforeApply += simulatedResult.latency;
			}
			if (simulatedResult.exception != null || simulatedResult.response != null)
			{
				applySimulationToRequest = () =>
				{
					request.state = simulatedResult.state;
					request.exception = simulatedResult.exception;
					request.response = simulatedResult.response;
				};
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[WebClient] " + msg);
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[WebClient] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[WebClient] " + msg);
		}
	}
}
