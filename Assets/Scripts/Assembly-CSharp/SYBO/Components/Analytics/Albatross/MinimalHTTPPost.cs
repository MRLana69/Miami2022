using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Components.Logging;
using UnityEngine;

namespace SYBO.Components.Analytics.Albatross
{
	public class MinimalHTTPPost : PostAnalyticsProvider.IHTTPPostLayer
	{
		private MonoBehaviour _coroutineHost;

		private float _timeout;

		public MinimalHTTPPost(MonoBehaviour coroutineHost, float timeout = 60f)
		{
			_coroutineHost = coroutineHost;
			_timeout = timeout;
		}

		public void POST(string url, Dictionary<string, string> headers, byte[] bytes, Action<int, string, string> onFinish)
		{
			_coroutineHost.StartCoroutine(WaitForWWWRequest(url, headers, bytes, onFinish));
		}

		private IEnumerator WaitForWWWRequest(string url, Dictionary<string, string> headers, byte[] bytes, Action<int, string, string> onFinish)
		{
			using (WWW www = new WWW(url, bytes, headers))
			{
				while (!www.isDone)
				{
					yield return null;
				}
				int responseStatusCode = GetResponseCode(www);
				bool hasStatusInHeader = www.isDone && www.responseHeaders != null && www.responseHeaders.ContainsKey("STATUS");
				string responseBody = string.Empty;
				if (responseStatusCode != 0)
				{
					responseBody = www.text;
				}
				string responseError = string.Empty;
				if (www.isDone && !string.IsNullOrEmpty(www.error))
				{
					responseError = www.error;
				}
				onFinish(responseStatusCode, responseBody, responseError);
			}
		}

		public static int GetResponseCode(WWW request)
		{
			if (!request.isDone)
			{
				return 0;
			}
			if (request.responseHeaders == null)
			{
				return 0;
			}
			if (!request.responseHeaders.ContainsKey("STATUS"))
			{
				return 0;
			}
			return ParseResponseCode(request.responseHeaders["STATUS"]);
		}

		public static int ParseResponseCode(string statusLine)
		{
			int result = 0;
			string[] array = statusLine.Split(' ');
			if (array.Length < 3)
			{
				Log.Analytics.LogErrorFormat("invalid response status:{0}", statusLine);
			}
			else
			{
				int.TryParse(array[1], out result);
			}
			return result;
		}
	}
}
