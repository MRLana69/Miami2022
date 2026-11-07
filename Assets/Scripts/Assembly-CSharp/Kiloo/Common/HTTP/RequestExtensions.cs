using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common.HTTP.Internal;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.HTTP
{
	public static class RequestExtensions
	{
		public const int DEFAULT_DUMP_MAX_BODY_CHARS = 1000;

		public const int DEFAULT_DUMP_MAX_HEX_LINES = 50;

		public static IRequest WithContent(this IRequest request, byte[] content, string contentType)
		{
			if (content != null)
			{
				request.bytes = content;
				if (!string.IsNullOrEmpty(contentType))
				{
					request.headers["Content-Type"] = contentType;
				}
			}
			return request;
		}

		public static IRequest WithContent(this IRequest request, string content, string contentType = "text/plain; charset=utf-8", Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			return request.WithContent(encoding.GetBytes(content), contentType);
		}

		public static IRequest WithJsonContent(this IRequest request, object jsonData, IJsonSerializer jsonSerializer)
		{
			string s = jsonSerializer.ToJson(jsonData);
			return request.WithContent(Encoding.UTF8.GetBytes(s), "application/json");
		}

		public static IRequest WithBsonContent(this IRequest request, object jsonData, IBsonSerializer bsonSerializer)
		{
			byte[] content = bsonSerializer.ToBson(jsonData);
			return request.WithContent(content, "application/bson");
		}

		public static IRequest WithHeaders(this IRequest request, IDictionary<string, string> headers)
		{
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					request.headers[header.Key] = header.Value;
				}
			}
			return request;
		}

		public static IRequest WithHeader(this IRequest request, string key, string value)
		{
			request.headers[key] = value;
			return request;
		}

		public static IRequest WithTimeout(this IRequest request, float timeout)
		{
			request.timeout = timeout;
			return request;
		}

		public static IRequest WithKeepAlive(this IRequest request, float keepAlive)
		{
			request.keepAlive = keepAlive;
			return request;
		}

		public static IRequest WithMinBackOffTime(this IRequest request, float minBackOffTime)
		{
			request.minBackOffTime = minBackOffTime;
			return request;
		}

		public static IRequest WithMaxBackOffTime(this IRequest request, float maxBackOffTime)
		{
			request.maxBackOffTime = maxBackOffTime;
			return request;
		}

		public static IRequest WithIgnoreCertificateErrors(this IRequest request, bool ignoreCertificateErrors)
		{
			request.ignoreCertificateErrors = ignoreCertificateErrors;
			return request;
		}

		public static IRequest WithIgnoreContentMD5(this IRequest request, bool ignoreContentMD5)
		{
			request.ignoreContentMD5 = ignoreContentMD5;
			return request;
		}

		public static IRequest WithAcceptGzip(this IRequest request, bool acceptGzip)
		{
			request.acceptGzip = acceptGzip;
			return request;
		}

		public static IRequest WithQueryParameter(this IRequest request, [NotNull] string name, string value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Value cannot be null or empty.", "name");
			}
			if (value != null)
			{
				Uri uri = request.uri;
				string text = Uri.EscapeDataString(name);
				string text2 = Uri.EscapeDataString(value);
				string query = uri.Query;
				int capacity = query.Length + text.Length + 1 + text2.Length;
				StringBuilder stringBuilder;
				if (query.Length > 1)
				{
					stringBuilder = new StringBuilder(query, 1, query.Length - 1, capacity);
					stringBuilder.Append('&');
				}
				else
				{
					stringBuilder = new StringBuilder(capacity);
				}
				stringBuilder.Append(text);
				stringBuilder.Append('=');
				stringBuilder.Append(text2);
				UriBuilder uriBuilder = new UriBuilder(uri);
				uriBuilder.Query = stringBuilder.ToString();
				request.uri = uriBuilder.Uri;
			}
			return request;
		}

		public static IRequest ThenMakeTheRequest(this IRequest request, WebClient.SendOptions options = default(WebClient.SendOptions))
		{
			return WebClient.SendRequest(request, options);
		}

		public static IRequest Send(this IRequest request, WebClient.SendOptions options = default(WebClient.SendOptions))
		{
			return WebClient.SendRequest(request, options);
		}

		public static Coroutine ThenMakeTheRequestAsync(this IRequest request, WebClient.RequestCompleteHandler onComplete = null, WebClient.SendOptions options = default(WebClient.SendOptions))
		{
			return WebClient.SendRequestAsync(request, onComplete, options);
		}

		public static Coroutine SendAsync(this IRequest request, WebClient.RequestCompleteHandler onComplete = null, WebClient.SendOptions options = default(WebClient.SendOptions))
		{
			return WebClient.SendRequestAsync(request, onComplete, options);
		}

		public static string CreateDump(this IRequest request, RequestDumpOption options = RequestDumpOption.All, int maxBodyChars = 0, int maxHexLines = 0)
		{
			if (maxBodyChars == 0)
			{
				maxBodyChars = 1000;
			}
			if (maxHexLines == 0)
			{
				maxHexLines = 50;
			}
			StringBuilder stringBuilder = new StringBuilder(request.method);
			stringBuilder.Append(' ');
			stringBuilder.Append(request.uri.AbsoluteUri);
			if ((options & RequestDumpOption.Status) != 0)
			{
				stringBuilder.Append("\n\nStatus: ").Append(request.statusMessage);
			}
			if ((options & RequestDumpOption.RequestHeaders) != 0)
			{
				stringBuilder.Append("\n\nRequest headers:\n").Append(request.headers);
			}
			if ((options & RequestDumpOption.RequestBody) != 0)
			{
				byte[] bytes = request.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					string value = Util.PrintableContent(bytes, request.headers, maxBodyChars, maxHexLines);
					stringBuilder.Append("\n\nRequest body:\n").Append(value);
				}
				else
				{
					stringBuilder.Append("\n\nRequest has no body");
				}
			}
			IResponse response = request.response;
			if (response != null)
			{
				if ((options & RequestDumpOption.ResponseHeaders) != 0)
				{
					stringBuilder.Append("\n\nResponse headers:\n").Append(response.headers);
				}
				if ((options & RequestDumpOption.ResponseBody) != 0)
				{
					byte[] bytes2 = response.bytes;
					if (bytes2 != null && bytes2.Length > 0)
					{
						string value2 = Util.PrintableContent(bytes2, response.headers, maxBodyChars, maxHexLines);
						stringBuilder.Append("\n\nResponse body:\n").Append(value2);
					}
					else
					{
						stringBuilder.Append("\n\nResponse has no body");
					}
				}
			}
			else if ((options & RequestDumpOption.AllResponse) != 0)
			{
				stringBuilder.Append("\n\nNo response (NULL)");
			}
			return stringBuilder.ToString();
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[RequestExtensions] " + msg);
		}
	}
}
