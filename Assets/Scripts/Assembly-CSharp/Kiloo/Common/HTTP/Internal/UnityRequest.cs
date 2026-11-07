using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.HTTP.Internal
{
	internal class UnityRequest : Request
	{
		private AsyncOperation _sendAsyncOperation;

		private UnityWebRequest _unityWebRequest;

		public UnityRequest([NotNull] string method, [NotNull] Uri uri, [CanBeNull] byte[] bytes)
			: base(method, uri, bytes)
		{
		}

		public override void PrepareFetchResponse()
		{
			if (base.acceptGzip && !base.headers.Contains("Accept-Encoding"))
			{
				base.headers.Add("Accept-Encoding", "gzip");
			}
		}

		public override void FetchResponseAsync()
		{
			InternalPrepareFetchResponse();
			try
			{
				CheckBackOff();
				CoroutineUtil.StartCoroutine(SendCoroutine());
			}
			catch (Exception ex)
			{
				LogError(ex.ToString());
				base.exception = ex;
				base.response = null;
				if (!Request.IsFinalState(base.state))
				{
					base.state = RequestState.Error;
				}
				if (!(ex is HTTPBackOffException))
				{
					UpdateBackOffError();
				}
			}
		}

		public override void FetchResponse()
		{
			InternalPrepareFetchResponse();
			try
			{
				CheckBackOff();
				SendUnityWebRequest();
				while (!_sendAsyncOperation.isDone)
				{
					Thread.Sleep(0);
					AssertNotCanceled("sync sending/receiving");
				}
				IResponse response = CreateResponseFromUnityWebRequest();
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
			if (_unityWebRequest != null)
			{
				_unityWebRequest.Dispose();
				_unityWebRequest = null;
			}
			_sendAsyncOperation = null;
		}

		private IEnumerator SendCoroutine()
		{
			bool ok = true;
			try
			{
				SendUnityWebRequest();
			}
			catch (Exception ex)
			{
				LogError(ex.ToString());
				base.exception = ex;
				base.response = null;
				if (!Request.IsFinalState(base.state))
				{
					base.state = RequestState.Error;
				}
				UpdateBackOffError();
				ok = false;
			}
			while (ok && !_sendAsyncOperation.isDone)
			{
				yield return null;
				try
				{
					AssertNotCanceled("async sending/receiving");
				}
				catch (HTTPRequestCanceledException ex2)
				{
					_unityWebRequest.Abort();
					LogError(ex2.ToString());
					if (base.state == RequestState.TimedOut)
					{
						UpdateBackOffError();
					}
					ok = false;
				}
			}
			if (ok)
			{
				try
				{
					IResponse response = CreateResponseFromUnityWebRequest();
					AssertNotCanceled("response read");
					base.response = response;
					base.state = RequestState.Done;
				}
				catch (Exception ex3)
				{
					LogError(ex3.ToString());
					base.exception = ex3;
					base.response = null;
					if (!Request.IsFinalState(base.state))
					{
						base.state = RequestState.Error;
					}
					UpdateBackOffError();
				}
			}
			if (_unityWebRequest != null)
			{
				_unityWebRequest.Dispose();
				_unityWebRequest = null;
			}
			_sendAsyncOperation = null;
		}

		private void SendUnityWebRequest()
		{
			_unityWebRequest = new UnityWebRequest(base.uri.ToString(), base.method);
			_unityWebRequest.redirectLimit = (int)base.maxRedirectCount;
			foreach (KeyValuePair<string, List<string>> item in base.headers.GetInternalDictionary())
			{
				string key = item.Key;
				List<string> value = item.Value;
				string value2 = ((value.Count != 1) ? string.Join(", ", value.ToArray()) : value[0]);
				if (!string.IsNullOrEmpty(value2))
				{
					_unityWebRequest.SetRequestHeader(key, value2);
				}
			}
			if (base.bytes != null)
			{
				UploadHandlerRaw uploadHandler = new UploadHandlerRaw(base.bytes);
				_unityWebRequest.uploadHandler = uploadHandler;
				_unityWebRequest.disposeUploadHandlerOnDispose = true;
			}
			_unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			_unityWebRequest.disposeDownloadHandlerOnDispose = true;
			_unityWebRequest.chunkedTransfer = false;
			_sendAsyncOperation = _unityWebRequest.Send();
		}

		private IResponse CreateResponseFromUnityWebRequest()
		{
			long responseCode = _unityWebRequest.responseCode;
			if (responseCode < 100 || responseCode > 599)
			{
				string error = _unityWebRequest.error;
				throw new HTTPException((!string.IsNullOrEmpty(error)) ? error : "Unknown error");
			}
			byte[] array = _unityWebRequest.downloadHandler.data;
			int num = (int)_unityWebRequest.responseCode;
			string message = HTTPStatus.ToString(num);
			Headers headers = new Headers();
			Dictionary<string, string> responseHeaders = _unityWebRequest.GetResponseHeaders();
			if (responseHeaders != null)
			{
				headers.AddAll(responseHeaders);
			}
			if ("gzip".Equals(headers["Content-Encoding"], StringComparison.OrdinalIgnoreCase) && array[0] == 31 && array[1] == 139 && array.Length == int.Parse(headers["Content-Length"]))
			{
				array = Util.UnZip(array);
			}
			return new Response(num, message, headers, array);
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[UnityRequest] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[UnityRequest] " + msg);
		}
	}
}
