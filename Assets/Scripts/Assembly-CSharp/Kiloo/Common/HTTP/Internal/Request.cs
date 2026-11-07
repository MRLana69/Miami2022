using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP.Internal
{
	internal abstract class Request : IRequest
	{
		private DateTime _startTime;

		public string method { get; protected set; }

		public IResponse response { get; set; }

		public uint maxRedirectCount { get; set; }

		public Exception exception { get; set; }

		public RequestState state { get; set; }

		public bool ignoreCertificateErrors { get; set; }

		public bool ignoreContentMD5 { get; set; }

		public float timeout { get; set; }

		public float minBackOffTime { get; set; }

		public float maxBackOffTime { get; set; }

		public bool acceptGzip { get; set; }

		public Uri uri { get; set; }

		public byte[] bytes { get; set; }

		public float keepAlive { get; set; }

		public Headers headers { get; protected set; }

		public bool isFinished
		{
			get
			{
				return IsFinalState(state);
			}
		}

		public bool isSuccessful
		{
			get
			{
				return state == RequestState.Done && response.category == ResponseCategory.Successful;
			}
		}

		public string statusMessage
		{
			get
			{
				if (exception != null)
				{
					return string.Format("{0}, {1}: {2}", state, exception.GetType().Name, exception.Message);
				}
				if (response != null)
				{
					return string.Format("{0}: {1}", response.status, response.message);
				}
				return state.ToString();
			}
		}

		protected Request([NotNull] string method, [NotNull] Uri uri, [CanBeNull] byte[] bytes)
		{
			headers = new Headers();
			maxRedirectCount = 8u;
			acceptGzip = true;
			ignoreCertificateErrors = false;
			minBackOffTime = 1f;
			keepAlive = 5f;
			ignoreContentMD5 = true;
			this.method = method.ToUpperInvariant();
			this.uri = uri;
			this.bytes = bytes;
		}

		protected static bool IsFinalState(RequestState state)
		{
			return state == RequestState.Done || state == RequestState.Error || state == RequestState.Canceled || state == RequestState.TimedOut;
		}

		public void CheckBackOff()
		{
			double backOffSeconds = Hosts.GetBackOffSeconds(uri.Host);
			if (backOffSeconds > 0.0)
			{
				throw new HTTPBackOffException(uri.Host + " is busy, cannot make request at this time (waiting " + backOffSeconds + " more seconds)");
			}
		}

		public void UpdateBackOffError()
		{
			if (maxBackOffTime > 0f)
			{
				Hosts.BackOff(uri.Host, minBackOffTime, maxBackOffTime);
			}
		}

		public void UpdateBackOff(IResponse response)
		{
			string host = uri.Host;
			int status = response.status;
			if (status == 500 || status == 503)
			{
				bool flag = false;
				if (response.headers.Contains("Retry-After"))
				{
					DateTime dateTime = response.headers.Get("Retry-After", DateTime.MinValue, true);
					if (dateTime > DateTime.UtcNow)
					{
						Hosts.BackOff(host, dateTime);
						flag = true;
					}
				}
				if (!flag && maxBackOffTime > 0f)
				{
					Hosts.BackOff(uri.Host, minBackOffTime, maxBackOffTime);
				}
			}
			else
			{
				Hosts.DontBackOff(host);
			}
		}

		public abstract void PrepareFetchResponse();

		public abstract void FetchResponseAsync();

		public abstract void FetchResponse();

		protected void InternalPrepareFetchResponse()
		{
			if (state != RequestState.None && !isFinished)
			{
				throw new InvalidOperationException("Fetch already in process");
			}
			response = null;
			exception = null;
			state = RequestState.Waiting;
			_startTime = DateTime.UtcNow;
			PrepareFetchResponse();
		}

		protected void AssertNotCanceled(string step)
		{
			if (timeout > 0f && (DateTime.UtcNow - _startTime).TotalSeconds > (double)timeout)
			{
				state = RequestState.TimedOut;
			}
			if (isFinished)
			{
				if (state == RequestState.Canceled)
				{
					throw new HTTPRequestCanceledException("Request was canceled (" + step + ")");
				}
				if (state == RequestState.TimedOut)
				{
					throw new HTTPRequestCanceledException("Request was canceled because it failed to complete in time (" + step + ")");
				}
				throw new HTTPException("Unexpected request state (" + step + ")");
			}
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[Request] " + msg);
		}
	}
}
