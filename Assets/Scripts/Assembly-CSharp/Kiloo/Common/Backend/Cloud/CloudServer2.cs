using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using JetBrains.Annotations;
using Kiloo.Common.Backend.Authentication;
using Kiloo.Common.HTTP;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend.Cloud
{
	public sealed class CloudServer2 : ICloudServer
	{
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class Metadata
		{
			public string sourceDescription;

			public string sourceInstallationId;

			public int dataVersion;

			public bool zipped = true;
		}

		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class RequestItem
		{
			public string name;

			public DateTime? lastModified;

			public int? size;

			public Dictionary<string, string> metadata;

			public byte[] data;
		}

		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class RequestObject
		{
			public List<RequestItem> items;
		}

		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class ResponseObject
		{
			public CloudSaveStatus status;

			public List<RequestItem> items;
		}

		public static class ErrorCodes
		{
			public const string CATEGORY = "CloudSave";

			public const int INTERNAL_ERROR = 3000;

			public const int INVALID_REQUEST = 3001;

			public const int TIMEOUT = 3002;

			public const int QUOTA_EXCEEDED = 3003;
		}

		private class Item : ICloudItem
		{
			private readonly RequestItem _requestItem;

			private readonly CloudUserData _cloudUserData;

			private readonly string _revision;

			private readonly string _sourceDescription;

			private readonly string _sourceInstallationId;

			private readonly int _dataVersion;

			private readonly bool _zipped;

			public string name
			{
				get
				{
					return _requestItem.name;
				}
			}

			public string revision
			{
				get
				{
					return _revision;
				}
			}

			public DateTime? lastModified
			{
				get
				{
					return _requestItem.lastModified;
				}
			}

			public bool isModified
			{
				get
				{
					return false;
				}
			}

			public bool isOutOfDate
			{
				get
				{
					return false;
				}
			}

			public string sourceDescription
			{
				get
				{
					return _sourceDescription;
				}
			}

			public string sourceInstallationId
			{
				get
				{
					return _sourceInstallationId;
				}
			}

			public int dataVersion
			{
				get
				{
					return _dataVersion;
				}
			}

			public ICloudUserData userData
			{
				get
				{
					return _cloudUserData;
				}
			}

			public Item(RequestItem requestItem)
			{
				_requestItem = requestItem;
				_revision = ToRevision(requestItem.lastModified);
				_cloudUserData = new CloudUserData();
				Dictionary<string, string> metadata = _requestItem.metadata;
				if (metadata == null)
				{
					return;
				}
				string value;
				if (metadata.TryGetValue("zipped", out value))
				{
					_zipped = "true".Equals(value, StringComparison.OrdinalIgnoreCase);
				}
				if (metadata.TryGetValue("version", out value))
				{
					int.TryParse(value, out _dataVersion);
				}
				metadata.TryGetValue("source", out _sourceDescription);
				metadata.TryGetValue("installation", out _sourceInstallationId);
				foreach (KeyValuePair<string, string> item in metadata)
				{
					if (item.Key.StartsWith("_"))
					{
						_cloudUserData.Add(item.Key.Substring(1), item.Value);
					}
				}
			}

			public Stream GetDataStream()
			{
				if (_requestItem.data != null)
				{
					MemoryStream memoryStream = new MemoryStream(_requestItem.data, false);
					if (_zipped)
					{
						return new GZipStream(memoryStream, CompressionMode.Decompress);
					}
					return memoryStream;
				}
				return null;
			}
		}

		private const string API_ROUTE = "api/v2/cloudsave";

		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		[NotNull]
		public Uri baseUri { get; private set; }

		[NotNull]
		public IKilooAuth kilooAuth { get; private set; }

		[NotNull]
		public IBsonSerializer bsonSerializer { get; private set; }

		public float timeout { get; set; }

		public int minimumSizeForCompression { get; set; }

		public Ionic.Zlib.CompressionLevel compressionLevel { get; set; }

		public string kilooTag
		{
			get
			{
				return kilooAuth.kilooTag;
			}
		}

		public CloudServer2([NotNull] Uri baseUri, [NotNull] IKilooAuth kilooAuth, [NotNull] IBsonSerializer bsonSerializer)
		{
			if (baseUri == null)
			{
				throw new ArgumentNullException("baseUri");
			}
			if (kilooAuth == null)
			{
				throw new ArgumentNullException("kilooAuth");
			}
			if (bsonSerializer == null)
			{
				throw new ArgumentNullException("bsonSerializer");
			}
			this.baseUri = baseUri;
			this.kilooAuth = kilooAuth;
			this.bsonSerializer = bsonSerializer;
			timeout = 0f;
			minimumSizeForCompression = 256;
			compressionLevel = Ionic.Zlib.CompressionLevel.Default;
		}

		public Coroutine ListItemsAsync(string forKilooTag, CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			if (completeHandler == null)
			{
				throw new ArgumentNullException("completeHandler");
			}
			return CoroutineUtil.StartCoroutine(ListItemsCoroutine(forKilooTag, completeHandler));
		}

		public CloudServerResult ListItems(string forKilooTag)
		{
			CheckAuthentication(forKilooTag);
			IRequest request = PrepareListItemsRequest();
			request.SendAuthenticated(kilooAuth);
			return ProcessCloudItemsResponse(request);
		}

		public Coroutine UpdateItemsAsync(string forKilooTag, IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (completeHandler == null)
			{
				throw new ArgumentNullException("completeHandler");
			}
			return CoroutineUtil.StartCoroutine(UpdateItemsCoroutine(forKilooTag, items, completeHandler));
		}

		public CloudServerResult UpdateItems(string forKilooTag, IEnumerable<ICloudItem> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			CheckAuthentication(forKilooTag);
			IRequest request = PrepareUpdateItemsRequest(items);
			request.SendAuthenticated(kilooAuth);
			return ProcessCloudItemsResponse(request);
		}

		public Coroutine CommitItemsAsync(string forKilooTag, IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerCommitResult> completeHandler)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (completeHandler == null)
			{
				throw new ArgumentNullException("completeHandler");
			}
			return CoroutineUtil.StartCoroutine(CommitItemsCoroutine(forKilooTag, items, completeHandler));
		}

		public CloudServerCommitResult CommitItems(string forKilooTag, IEnumerable<ICloudItem> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			CheckAuthentication(forKilooTag);
			IDictionary<string, CloudServerCommitInfo> commitInfo;
			IRequest request = PrepareCommitItemsRequest(items, out commitInfo);
			try
			{
				request.SendAuthenticated(kilooAuth);
			}
			catch (Exception inner)
			{
				throw new CloudException("Commit request failed.", inner);
			}
			CloudServerResult cloudServerResult = ProcessCloudItemsResponse(request);
			CloudServerCommitResult cloudServerCommitResult = new CloudServerCommitResult();
			cloudServerCommitResult.status = cloudServerResult.status;
			cloudServerCommitResult.items = cloudServerResult.items;
			cloudServerCommitResult.errorMessage = cloudServerResult.errorMessage;
			cloudServerCommitResult.commitInfo = commitInfo;
			return cloudServerCommitResult;
		}

		[NotNull]
		private CloudServerResult ProcessCloudItemsResponse(IRequest request)
		{
			IResponse response = request.response;
			if (response == null || !response.IsBson())
			{
				if (request.isSuccessful)
				{
					throw new CloudException("Request response had unexpected content type.");
				}
				throw new CloudException("Request failed: " + request.statusMessage);
			}
			ServiceResponse<ResponseObject> serviceResponse;
			try
			{
				serviceResponse = response.ParseBson<ServiceResponse<ResponseObject>>(bsonSerializer);
			}
			catch (Exception inner)
			{
				throw new CloudException("Error parsing response", inner);
			}
			if (serviceResponse.result == null)
			{
				throw new CloudException("Request failed: " + serviceResponse.error);
			}
			List<RequestItem> items = serviceResponse.result.items;
			CloudServerResult cloudServerResult = new CloudServerResult();
			cloudServerResult.status = serviceResponse.result.status;
			cloudServerResult.items = ((items != null) ? ((IEnumerable<RequestItem>)items).Select((Func<RequestItem, ICloudItem>)((RequestItem responseItem) => new Item(responseItem))).ToArray() : null);
			return cloudServerResult;
		}

		private IRequest PrepareListItemsRequest()
		{
			try
			{
				return KilooWebClient.CreateGet(new Uri(baseUri, "api/v2/cloudsave")).WithQueryParameter("metaOnly", "true").WithHeader("Accept", "application/bson")
					.WithTimeout(timeout);
			}
			catch (Exception inner)
			{
				throw new CloudException("Failed preparing list items request.", inner);
			}
		}

		private static string ToRevision(DateTime? value)
		{
			if (value.HasValue)
			{
				return ((long)(value.Value.ToUniversalTime() - _epoch).TotalMilliseconds).ToString("X");
			}
			return null;
		}

		private static DateTime? FromRevision(string value)
		{
			long result;
			if (value != null && long.TryParse(value, NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out result))
			{
				return _epoch.AddMilliseconds(result);
			}
			return null;
		}

		private static string EscapeName(string name)
		{
			return name.Replace(",", "\\,");
		}

		private string BuildRefItemsQueryParameter(IEnumerable<ICloudItem> refItems)
		{
			if (refItems != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (ICloudItem refItem in refItems)
				{
					if (FromRevision(refItem.revision).HasValue)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.AppendFormat("{0},{1}", EscapeName(refItem.name), refItem.revision);
					}
				}
				if (stringBuilder.Length > 0)
				{
					return stringBuilder.ToString();
				}
			}
			return null;
		}

		private IRequest PrepareUpdateItemsRequest(IEnumerable<ICloudItem> refItems)
		{
			try
			{
				return KilooWebClient.CreateGet(new Uri(baseUri, "api/v2/cloudsave")).WithQueryParameter("refItems", BuildRefItemsQueryParameter(refItems)).WithQueryParameter("metaOnly", "false")
					.WithHeader("Accept", "application/bson")
					.WithAcceptGzip(false)
					.WithTimeout(timeout);
			}
			catch (Exception inner)
			{
				throw new CloudException("Failed preparing update request.", inner);
			}
		}

		private IRequest PrepareCommitItemsRequest(IEnumerable<ICloudItem> items, out IDictionary<string, CloudServerCommitInfo> commitInfo)
		{
			try
			{
				commitInfo = new Dictionary<string, CloudServerCommitInfo>();
				List<RequestItem> list = new List<RequestItem>();
				foreach (ICloudItem item in items)
				{
					CloudServerCommitInfo cloudServerCommitInfo = new CloudServerCommitInfo();
					byte[] array = null;
					bool flag = false;
					Stream dataStream = item.GetDataStream();
					if (dataStream != null)
					{
						flag = !dataStream.CanSeek || dataStream.Length >= minimumSizeForCompression;
						if (flag)
						{
							array = Zip(dataStream, out cloudServerCommitInfo.crc32, out cloudServerCommitInfo.size, out cloudServerCommitInfo.compressedSize);
							if (cloudServerCommitInfo.compressedSize > cloudServerCommitInfo.size)
							{
								flag = false;
								dataStream.Dispose();
								dataStream = item.GetDataStream();
							}
						}
						if (!flag)
						{
							array = dataStream.ReadAllBytes();
							cloudServerCommitInfo.crc32 = Checksum.ComputeCrc32(array);
							cloudServerCommitInfo.size = array.Length;
							cloudServerCommitInfo.compressedSize = array.Length;
						}
						dataStream.Dispose();
					}
					commitInfo.Add(item.name, cloudServerCommitInfo);
					list.Add(new RequestItem
					{
						name = item.name,
						lastModified = FromRevision(item.revision),
						metadata = MakeMetadataDictionary(item, flag),
						data = array
					});
				}
				RequestObject requestObject = new RequestObject();
				requestObject.items = list;
				RequestObject jsonData = requestObject;
				return KilooWebClient.CreatePost(new Uri(baseUri, "api/v2/cloudsave")).WithHeader("Accept", "application/bson").WithTimeout(timeout)
					.WithBsonContent(jsonData, bsonSerializer);
			}
			catch (Exception inner)
			{
				throw new CloudException("Failed preparing commit request.", inner);
			}
		}

		private Dictionary<string, string> MakeMetadataDictionary(ICloudItem item, bool zipped)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			dictionary.Add("version", string.Empty + item.dataVersion);
			dictionary.Add("installation", item.sourceInstallationId);
			dictionary.Add("source", item.sourceDescription);
			dictionary.Add("zipped", (!zipped) ? "false" : "true");
			if (item.userData != null)
			{
				foreach (KeyValuePair<string, string> userDatum in item.userData)
				{
					dictionary.Add("_" + userDatum.Key, userDatum.Value);
				}
			}
			return dictionary;
		}

		private void CheckAuthentication(string expectedKilooTag)
		{
			CheckAuthenticationResult(expectedKilooTag, kilooAuth.state == KilooAuthState.Authorized, null);
		}

		private Coroutine CheckAuthenticationAsync(string expectedKilooTag, Action<bool, string> completeHandler)
		{
			return CoroutineUtil.StartCoroutine(CheckAuthenticationCoroutine(expectedKilooTag, completeHandler));
		}

		private IEnumerator CheckAuthenticationCoroutine(string expectedKilooTag, Action<bool, string> completeHandler)
		{
			bool authSuccess = false;
			Error authError = null;
			yield return kilooAuth.AuthenticateAsync((bool success, Error error) =>
			{
				authSuccess = success;
				authError = error;
			});
			try
			{
				CheckAuthenticationResult(expectedKilooTag, authSuccess, authError);
				completeHandler(true, null);
			}
			catch (CloudException ex)
			{
				LogError(ex.Message);
				completeHandler(false, ex.Message);
			}
		}

		private void CheckAuthenticationResult(string expectedKilooTag, bool isAuthenticated, Error error)
		{
			if (isAuthenticated)
			{
				if (expectedKilooTag != null && expectedKilooTag != kilooAuth.kilooTag)
				{
					throw new CloudException("Wrong kiloo tag.");
				}
				return;
			}
			if (error == null)
			{
				throw new CloudException("Not authenticated.");
			}
			throw new CloudException("Authentication failed: " + error);
		}

		private void CompleteCloudServerRequest<T>(string method, CloudServerRequestCompleteHandler<T> completeHandler, T result) where T : CloudServerResult
		{
			completeHandler(result);
		}

		private void CompleteListItemsWithError(CloudServerRequestCompleteHandler<CloudServerResult> completeHandler, string errorMessage)
		{
			CompleteCloudServerRequest("ListItemsAsync", completeHandler, new CloudServerResult
			{
				status = CloudSaveStatus.Error,
				errorMessage = errorMessage
			});
		}

		private void CompleteUpdateItemsWithError(CloudServerRequestCompleteHandler<CloudServerResult> completeHandler, string errorMessage)
		{
			CompleteCloudServerRequest("UpdateItemsAsync", completeHandler, new CloudServerResult
			{
				status = CloudSaveStatus.Error,
				errorMessage = errorMessage
			});
		}

		private void CompleteCommitItemsWithError(CloudServerRequestCompleteHandler<CloudServerCommitResult> completeHandler, string errorMessage)
		{
			CompleteCloudServerRequest("CommitItemsAsync", completeHandler, new CloudServerCommitResult
			{
				status = CloudSaveStatus.Error,
				errorMessage = errorMessage
			});
		}

		private IEnumerator ListItemsCoroutine(string forKilooTag, CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			bool authenticationOk = false;
			string errorMessage = null;
			yield return CheckAuthenticationAsync(forKilooTag, (bool authOk, string message) =>
			{
				authenticationOk = authOk;
				errorMessage = message;
			});
			if (!authenticationOk)
			{
				CompleteListItemsWithError(completeHandler, errorMessage);
				yield break;
			}
			IRequest request;
			try
			{
				request = PrepareListItemsRequest();
			}
			catch (CloudException ex)
			{
				CompleteListItemsWithError(completeHandler, ex.Message);
				yield break;
			}
			yield return request.SendAuthenticatedAsync(kilooAuth);
			try
			{
				CloudServerResult result = ProcessCloudItemsResponse(request);
				CompleteCloudServerRequest("ListItemsAsync", completeHandler, result);
			}
			catch (CloudException ex2)
			{
				CompleteListItemsWithError(completeHandler, ex2.Message);
			}
		}

		private IEnumerator UpdateItemsCoroutine(string forKilooTag, IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			bool authenticationOk = false;
			string errorMessage = null;
			yield return CheckAuthenticationAsync(forKilooTag, (bool authOk, string message) =>
			{
				authenticationOk = authOk;
				errorMessage = message;
			});
			if (!authenticationOk)
			{
				CompleteUpdateItemsWithError(completeHandler, errorMessage);
				yield break;
			}
			IRequest request;
			try
			{
				request = PrepareUpdateItemsRequest(items);
			}
			catch (CloudException ex)
			{
				CompleteUpdateItemsWithError(completeHandler, ex.Message);
				yield break;
			}
			yield return request.SendAuthenticatedAsync(kilooAuth);
			try
			{
				CloudServerResult result = ProcessCloudItemsResponse(request);
				CompleteCloudServerRequest("UpdateItemsAsync", completeHandler, result);
			}
			catch (CloudException ex2)
			{
				CompleteUpdateItemsWithError(completeHandler, ex2.Message);
			}
		}

		private IEnumerator CommitItemsCoroutine(string forKilooTag, IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerCommitResult> completeHandler)
		{
			bool authenticationOk = false;
			string errorMessage = null;
			yield return CheckAuthenticationAsync(forKilooTag, (bool authOk, string message) =>
			{
				authenticationOk = authOk;
				errorMessage = message;
			});
			if (!authenticationOk)
			{
				CompleteCommitItemsWithError(completeHandler, errorMessage);
				yield break;
			}
			IRequest request;
			IDictionary<string, CloudServerCommitInfo> commitInfo;
			try
			{
				request = PrepareCommitItemsRequest(items, out commitInfo);
			}
			catch (CloudException ex)
			{
				CompleteCommitItemsWithError(completeHandler, ex.Message);
				yield break;
			}
			yield return request.SendAuthenticatedAsync(kilooAuth);
			try
			{
				CloudServerResult cloudServerResult = ProcessCloudItemsResponse(request);
				CompleteCloudServerRequest("CommitItemsAsync", completeHandler, new CloudServerCommitResult
				{
					status = cloudServerResult.status,
					items = cloudServerResult.items,
					commitInfo = commitInfo
				});
			}
			catch (CloudException ex2)
			{
				CompleteCommitItemsWithError(completeHandler, ex2.Message);
			}
		}

		private byte[] Zip(Stream data, out int inputCrc32, out int inputLength, out int outputLength)
		{
			if (data == null)
			{
				inputCrc32 = 0;
				inputLength = 0;
				outputLength = 0;
				return null;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, compressionLevel, true);
				inputLength = GjStreamUtil.CopyCount(data, gZipStream);
				gZipStream.Close();
				inputCrc32 = gZipStream.Crc32;
				byte[] array = memoryStream.ToArray();
				outputLength = array.Length;
				return array;
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[CloudServer2] " + msg);
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("CLOUD_DEBUG_LOGS")]
		private static void LogResult(string method, CloudServerResult result)
		{
			StringBuilder stringBuilder = new StringBuilder("[CloudServer2] ").Append(method).Append(" Status: ").Append(result.status)
				.Append(", Items: ");
			ICloudItem[] items = result.items;
			if (items == null)
			{
				stringBuilder.Append("null");
			}
			else
			{
				for (int i = 0; i < items.Length; i++)
				{
					ICloudItem cloudItem = items[i];
					byte[] bytes = cloudItem.GetBytes();
					int num = ((bytes != null) ? bytes.Length : 0);
					string text = ((cloudItem.sourceDescription != null) ? ("\"" + cloudItem.sourceDescription + "\"") : "null");
					string text2 = ((cloudItem.sourceInstallationId != null) ? ("\"" + cloudItem.sourceInstallationId + "\"") : "null");
					stringBuilder.AppendFormat("\n    [{0}]: \"{1}\",\n        revision: \"{2}\",\n        modified: {3},\n        outofdate: {4},\n        sourceDescription: {5},\n        sourceInstallationId: {6},\n        dataVersion: {7},\n        datasize: {8}", i, cloudItem.name, cloudItem.revision, cloudItem.isModified, cloudItem.isOutOfDate, text, text2, cloudItem.dataVersion, num);
				}
			}
			Debug.Log(stringBuilder.ToString());
		}
	}
}
