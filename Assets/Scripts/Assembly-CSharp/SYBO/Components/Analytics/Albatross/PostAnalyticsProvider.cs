using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SYBO.Components.Logging;

namespace SYBO.Components.Analytics.Albatross
{
	public class PostAnalyticsProvider : IPostAnalytics
	{
		public interface IHTTPPostLayer
		{
			void POST(string url, Dictionary<string, string> headers, byte[] bytes, Action<int, string, string> onFinish);
		}

		private const string AUTH_FORMAT = "{0} {1}\n{2}";

		private const bool CompressContent = true;

		private string _baseURL;

		private string _applicationId;

		private string _userAgentString;

		private string _apiVersion;

		private HMACSHA1 _hmac;

		private IHTTPPostLayer _httpLayer;

		string IPostAnalytics.BaseURL
		{
			get
			{
				return _baseURL;
			}
			set
			{
				_baseURL = value;
			}
		}

		public PostAnalyticsProvider(string baseUrl, string applicationId, string apiVersion, string userAgentString, byte[] key, IHTTPPostLayer httpLayer)
		{
			_baseURL = baseUrl;
			_applicationId = applicationId;
			_apiVersion = apiVersion;
			_userAgentString = userAgentString;
			_hmac = new HMACSHA1(key);
			_httpLayer = httpLayer;
		}

		public void PostAnalyticsEvents(byte[] analyticsJsonBytes, int uncompressedLength, string uncompressedMD5Hash, Action<ProcessAnalyticsEventsResponse> onFinished)
		{
			string method = "POST";
			string text = string.Format("/{0}/analytics", _applicationId);
			string url = _baseURL + text;
			Dictionary<string, string> headers = GenerateHeaders(method, text, uncompressedMD5Hash, true);
			byte[] postData = analyticsJsonBytes;
			_httpLayer.POST(url, headers, postData, (int responseCode, string httpResponse, string errorMessage) =>
			{
				switch (responseCode)
				{
				case 0:
				{
					ProcessAnalyticsEventsResponse obj = ProcessAnalyticsEventsResponse.BackendUnavailable;
					if (!(errorMessage == string.Empty) && !errorMessage.Contains("SSL"))
					{
						obj = ProcessAnalyticsEventsResponse.RequestError;
						Log.Analytics.LogErrorFormat("Analytics: Request error uploading {0} bytes ({1} bytes uncompressed) to\n{2}\nError: {3}", postData.Length, uncompressedLength, url, errorMessage);
					}
					onFinished(obj);
					break;
				}
				case 200:
					onFinished(ProcessAnalyticsEventsResponse.AcceptedByServer);
					break;
				case 400:
				case 401:
				case 402:
				case 403:
				case 404:
				case 405:
				case 406:
				case 407:
				case 408:
				case 409:
				case 410:
				case 411:
				case 412:
				case 413:
				case 414:
				case 415:
				case 416:
				case 417:
				case 418:
				case 419:
				case 420:
				case 421:
				case 422:
				case 423:
				case 424:
				case 425:
				case 426:
				case 427:
				case 428:
				case 429:
				case 430:
				case 431:
				case 432:
				case 433:
				case 434:
				case 435:
				case 436:
				case 437:
				case 438:
				case 439:
				case 440:
				case 441:
				case 442:
				case 443:
				case 444:
				case 445:
				case 446:
				case 447:
				case 448:
				case 449:
				case 450:
				case 451:
				case 452:
				case 453:
				case 454:
				case 455:
				case 456:
				case 457:
				case 458:
				case 459:
				case 460:
				case 461:
				case 462:
				case 463:
				case 464:
				case 465:
				case 466:
				case 467:
				case 468:
				case 469:
				case 470:
				case 471:
				case 472:
				case 473:
				case 474:
				case 475:
				case 476:
				case 477:
				case 478:
				case 479:
				case 480:
				case 481:
				case 482:
				case 483:
				case 484:
				case 485:
				case 486:
				case 487:
				case 488:
				case 489:
				case 490:
				case 491:
				case 492:
				case 493:
				case 494:
				case 495:
				case 496:
				case 497:
				case 498:
				case 499:
					Log.Analytics.LogErrorFormat("Analytics: Rejected (HTTP {2}) {0} bytes ({1} bytes uncompressed) to\n{3}\nResponse:\n{4}\nError: {5}", postData.Length, uncompressedLength, responseCode, url, httpResponse, errorMessage);
					onFinished(ProcessAnalyticsEventsResponse.RejectedByServer);
					break;
				default:
					if (responseCode >= 500 && responseCode < 600)
					{
						onFinished(ProcessAnalyticsEventsResponse.BackendUnavailable);
					}
					else
					{
						Log.Analytics.LogErrorFormat("Analytics: Rejected (HTTP {2}) {0} bytes ({1} bytes uncompressed) to\n{3}\nResponse:\n{4}\nError: {5}", postData.Length, uncompressedLength, responseCode, url, httpResponse, errorMessage);
						onFinished(ProcessAnalyticsEventsResponse.RejectedByServer);
					}
					break;
				}
			});
		}

		private Dictionary<string, string> GenerateHeaders(string method, string uri, string uncompressedMD5Hash, bool gzipData)
		{
			string text = string.Empty;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("User-Agent", _userAgentString);
			dictionary.Add("X-Api-Version", _apiVersion);
			if (!string.IsNullOrEmpty(uncompressedMD5Hash))
			{
				dictionary.Add("Content-Type", "application/json");
				if (gzipData)
				{
					dictionary.Add("Content-Encoding", "gzip");
				}
				text = uncompressedMD5Hash;
				dictionary.Add("Content-Md5", text);
			}
			string s = string.Format("{0} {1}\n{2}", method, uri, text);
			string value = Convert.ToBase64String(_hmac.ComputeHash(Encoding.ASCII.GetBytes(s)));
			dictionary.Add("Authorization", value);
			return dictionary;
		}
	}
}
