using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP.Internal
{
	[Obsolete("Not used in Unity versions 5.4 or newer.")]
	internal class SocketsRequest : Request
	{
		public const string PROTOCOL = "HTTP/1.1";

		private const string EOL = "\r\n";

		private bool useSsl
		{
			get
			{
				return base.uri.Scheme.ToLower() == "https";
			}
		}

		public SocketsRequest([NotNull] string method, [NotNull] Uri uri, [CanBeNull] byte[] bytes)
			: base(method, uri, bytes)
		{
		}

		public override void PrepareFetchResponse()
		{
			if (!base.headers.Contains("Host"))
			{
				base.headers.Set("Host", base.uri.Host);
				if (base.acceptGzip)
				{
					base.headers.Add("Accept-Encoding", "gzip");
				}
				if (base.bytes != null && base.bytes.Length > 0)
				{
					base.headers.Set("Date", DateTime.UtcNow);
				}
				if (base.keepAlive <= 0.001f)
				{
					base.headers["Connection"] = "close";
				}
			}
		}

		public override void FetchResponseAsync()
		{
			if (base.state != RequestState.None && !base.isFinished)
			{
				throw new InvalidOperationException("Request.FetchResponseAsync called while already in process");
			}
			base.state = RequestState.Waiting;
			try
			{
				if (!ThreadPool.QueueUserWorkItem(delegate
				{
					base.state = RequestState.None;
					FetchResponse();
				}))
				{
					throw new HTTPException("Could not queue asynchronous request worker thread");
				}
			}
			catch (Exception ex)
			{
				base.exception = ex;
				base.state = RequestState.Error;
			}
		}

		public override void FetchResponse()
		{
			InternalPrepareFetchResponse();
			try
			{
				IResponse response = InternalFetchResponse();
				AssertNotCanceled("response read");
				base.response = response;
				base.state = RequestState.Done;
			}
			catch (HTTPRequestCanceledException ex)
			{
				LogError(ex.ToString());
				if (base.state == RequestState.TimedOut)
				{
					UpdateBackOffError();
				}
			}
			catch (Exception ex2)
			{
				LogError(ex2.ToString());
				base.exception = ex2;
				base.response = null;
				if (!Request.IsFinalState(base.state))
				{
					base.state = RequestState.Error;
				}
				if (!(ex2 is HTTPBackOffException))
				{
					UpdateBackOffError();
				}
			}
		}

		private bool RedirectCheck(IResponse response)
		{
			int status = response.status;
			if (status == 307 || status == 302 || status == 301)
			{
				Uri uri = new Uri(response.headers.Get("Location"));
				if (useSsl && uri.Scheme.ToLower() != "https")
				{
					throw new HTTPSecurityException("Redirects from secure to insecure sites are not allowed");
				}
				base.uri = uri;
				return true;
			}
			return false;
		}

		private IResponse InternalFetchResponse()
		{
			IResponse response = null;
			bool flag = base.headers["Connection"] != "close";
			int num = Util.ConvertTimeoutToMilliseconds(base.timeout);
			int num2 = 0;
			bool flag2;
			do
			{
				flag2 = false;
				CheckBackOff();
				Connection connection = null;
				try
				{
					connection = Connection.Open(base.uri.Host, base.uri.Port, useSsl, num, num, base.ignoreCertificateErrors);
					Stream stream = connection.GetStream();
					AssertNotCanceled("connection made");
					WriteToStream(stream);
					stream.Flush();
					AssertNotCanceled("request sent");
					SocketsResponse socketsResponse = new SocketsResponse();
					base.state = RequestState.Reading;
					socketsResponse.ReadFromStream(stream, base.ignoreContentMD5);
					response = socketsResponse;
					UpdateBackOff(socketsResponse);
					if (flag && response.headers["Connection"] != "close")
					{
						connection.KeepAlive(Util.ConvertTimeoutToMilliseconds(base.keepAlive));
					}
					else
					{
						connection.Close();
					}
					connection = null;
					flag2 = RedirectCheck(response);
					if (flag2 && ++num2 > base.maxRedirectCount)
					{
						flag2 = false;
						throw new HTTPException("Too many redirects");
					}
				}
				catch (Exception)
				{
					if (connection != null)
					{
						if (connection.reused)
						{
							flag2 = true;
						}
						connection.Close();
					}
					if (!flag2)
					{
						throw;
					}
				}
			}
			while (flag2);
			return response;
		}

		private void WriteToStream(Stream outputStream)
		{
			StreamWriter streamWriter = new StreamWriter(outputStream, ASCIIEncoding.ASCII);
			streamWriter.Write(base.method);
			streamWriter.Write(' ');
			streamWriter.Write(base.uri.PathAndQuery);
			streamWriter.Write(' ');
			streamWriter.Write("HTTP/1.1");
			streamWriter.Write("\r\n");
			foreach (KeyValuePair<string, string> header in base.headers)
			{
				streamWriter.Write(header.Key.Trim());
				streamWriter.Write(':');
				streamWriter.Write(header.Value.Trim());
				streamWriter.Write("\r\n");
			}
			int num = ((base.bytes != null) ? base.bytes.Length : 0);
			if ((num > 0 || base.method == "POST" || base.method == "PUT") && base.headers.Get("Content-Length", string.Empty) == string.Empty)
			{
				streamWriter.Write("Content-Length: ");
				streamWriter.Write(num);
				streamWriter.Write("\r\n");
			}
			streamWriter.Write("\r\n");
			streamWriter.Flush();
			if (base.bytes != null)
			{
				outputStream.Write(base.bytes, 0, base.bytes.Length);
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[SocketsRequest] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[SocketsRequest] " + msg);
		}
	}
}
