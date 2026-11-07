using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend.Cloud
{
	public sealed class SimulatedCloudServer : ICloudServer
	{
		private class Metadata
		{
			public string revision;

			public DateTime? lastModified;

			public string sourceDescription;

			public string sourceInstallationId;

			public int dataVersion;

			public Dictionary<string, string> userdata;
		}

		private class Item : ICloudItem
		{
			public delegate byte[] GetBytesFunction(string name);

			private readonly GetBytesFunction _getBytesFunction;

			public string name { get; private set; }

			public string revision { get; private set; }

			public DateTime? lastModified { get; private set; }

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

			public string sourceDescription { get; private set; }

			public string sourceInstallationId { get; private set; }

			public int dataVersion { get; private set; }

			public ICloudUserData userData { get; private set; }

			public Item(string name, string revision, DateTime? lastModified, string sourceDescription, string sourceInstallationId, int dataVersion, ICloudUserData userData, GetBytesFunction getBytesFunction)
			{
				this.name = name;
				this.revision = revision;
				this.lastModified = lastModified;
				this.sourceDescription = sourceDescription ?? SystemInfo.deviceName;
				this.sourceInstallationId = sourceInstallationId;
				this.dataVersion = dataVersion;
				this.userData = userData;
				_getBytesFunction = getBytesFunction;
			}

			public Stream GetDataStream()
			{
				GetBytesFunction getBytesFunction = _getBytesFunction;
				if (getBytesFunction != null && !string.IsNullOrEmpty(name))
				{
					return new MemoryStream(getBytesFunction(name));
				}
				return null;
			}
		}

		private static SimulatedCloudServer _instance = null;

		public float simulatedLatency;

		public bool simulateError;

		private const string REPOSITORY_DIR = ".simulatedcloudserver";

		private const string METADATA_DIR = ".metadata";

		private readonly IDirectory _localDir;

		private readonly IDirectory _metadataDir;

		private Dictionary<string, Item> _items;

		private readonly IJsonSerializer _jsonSerializer;

		private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static SimulatedCloudServer instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SimulatedCloudServer();
				}
				return _instance;
			}
		}

		public string kilooTag
		{
			get
			{
				return null;
			}
		}

		private SimulatedCloudServer()
		{
			_jsonSerializer = new KilooSerializer();
			_localDir = new SafeFileSystem(Path.Combine(DeviceUtility.GetUserDataPath(), ".simulatedcloudserver"));
			_metadataDir = _localDir.Subdirectory(".metadata");
			LoadRepository();
		}

		public void Reload()
		{
			LoadRepository();
		}

		public ICloudItem GetItem(string name)
		{
			Item value;
			if (_items.TryGetValue(name, out value))
			{
				return value;
			}
			return null;
		}

		public List<ICloudItem> GetItems()
		{
			return ((IEnumerable<KeyValuePair<string, Item>>)_items).Select((Func<KeyValuePair<string, Item>, ICloudItem>)((KeyValuePair<string, Item> item) => item.Value)).ToList();
		}

		public void SetItemLastModified(string name, DateTime lastModified)
		{
			Item value;
			if (_items.TryGetValue(name, out value))
			{
				value = new Item(name, value.revision, lastModified.ToUniversalTime().TrimToWholeMilliSeconds(), value.sourceDescription, value.sourceInstallationId, value.dataVersion, value.userData, LoadItem);
				SaveMetadata(value);
				_items[name] = value;
			}
			else
			{
				LogError("SetItemTimestamp: Item \"" + name + "\" not found");
			}
		}

		public void SetItemRevision(string name, string revision)
		{
			Item value;
			if (_items.TryGetValue(name, out value))
			{
				value = new Item(name, revision, value.lastModified, value.sourceDescription, value.sourceInstallationId, value.dataVersion, value.userData, LoadItem);
				SaveMetadata(value);
				_items[name] = value;
			}
			else
			{
				LogError("SetItemRevision: Item \"" + name + "\" not found");
			}
		}

		public void SetItemSourceDescription(string name, string sourceDescription)
		{
			Item value;
			if (_items.TryGetValue(name, out value))
			{
				value = new Item(name, value.revision, value.lastModified, sourceDescription, value.sourceInstallationId, value.dataVersion, value.userData, LoadItem);
				SaveMetadata(value);
				_items[name] = value;
			}
			else
			{
				LogError("SetItemSourceDescription: Item \"" + name + "\" not found");
			}
		}

		public void SetItemSourceInstallationId(string name, string sourceInstallationId)
		{
			Item value;
			if (_items.TryGetValue(name, out value))
			{
				value = new Item(name, value.revision, value.lastModified, value.sourceDescription, sourceInstallationId, value.dataVersion, value.userData, LoadItem);
				SaveMetadata(value);
				_items[name] = value;
			}
			else
			{
				LogError("SetItemSourceInstallationId: Item \"" + name + "\" not found");
			}
		}

		public void SetItemDataVersion(string name, int dataVersion)
		{
			Item value;
			if (_items.TryGetValue(name, out value))
			{
				value = new Item(name, value.revision, value.lastModified, value.sourceDescription, value.sourceInstallationId, dataVersion, value.userData, LoadItem);
				SaveMetadata(value);
				_items[name] = value;
			}
			else
			{
				LogError("SetItemDataVersion: Item \"" + name + "\" not found");
			}
		}

		public void SimulateNewRevision(string name)
		{
			Item value;
			if (_items.TryGetValue(name, out value))
			{
				DateTime utcNow = DateTime.UtcNow;
				value = new Item(name, utcNow.ToJSON(), utcNow, value.sourceDescription, value.sourceInstallationId, value.dataVersion, value.userData, LoadItem);
				SaveMetadata(value);
				_items[name] = value;
			}
			else
			{
				LogError("SimulateCommitExisting: Item \"" + name + "\" not found");
			}
		}

		public void SimulateNewRevision(string name, byte[] bytes, string sourceDescription, string sourceInstallationId, int dataVersion, ICloudUserData userData)
		{
			_localDir.File(name).SaveRawData(bytes);
			DateTime utcNow = DateTime.UtcNow;
			Item item = new Item(name, utcNow.ToJSON(), utcNow, sourceDescription, sourceInstallationId, dataVersion, userData, LoadItem);
			SaveMetadata(item);
			_items[name] = item;
		}

		public Coroutine ListItemsAsync(string forKilooTag, CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			return CoroutineUtil.StartCoroutine(ListItemsCoroutine(completeHandler));
		}

		public CloudServerResult ListItems(string forKilooTag)
		{
			if (simulatedLatency > 0f)
			{
				Thread.Sleep(TimeSpan.FromSeconds(simulatedLatency));
			}
			return InternalListItems();
		}

		public Coroutine UpdateItemsAsync(string forKilooTag, IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			return CoroutineUtil.StartCoroutine(UpdateItemsCoroutine(items, completeHandler));
		}

		public CloudServerResult UpdateItems(string forKilooTag, IEnumerable<ICloudItem> items)
		{
			if (simulatedLatency > 0f)
			{
				Thread.Sleep(TimeSpan.FromSeconds(simulatedLatency));
			}
			return InternalUpdateItems(items);
		}

		public Coroutine CommitItemsAsync(string forKilooTag, IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerCommitResult> completeHandler)
		{
			return CoroutineUtil.StartCoroutine(CommitItemsCoroutine(items, completeHandler));
		}

		public CloudServerCommitResult CommitItems(string forKilooTag, IEnumerable<ICloudItem> items)
		{
			if (simulatedLatency > 0f)
			{
				Thread.Sleep(TimeSpan.FromSeconds(simulatedLatency));
			}
			return InternalCommitItems(items);
		}

		private CloudServerResult InternalListItems()
		{
			CloudServerResult cloudServerResult;
			if (simulateError)
			{
				cloudServerResult = new CloudServerResult();
				cloudServerResult.status = CloudSaveStatus.Error;
				cloudServerResult.items = null;
				return cloudServerResult;
			}
			cloudServerResult = new CloudServerResult();
			cloudServerResult.status = CloudSaveStatus.OK;
			cloudServerResult.items = GetStatusList();
			return cloudServerResult;
		}

		private IEnumerator ListItemsCoroutine(CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			if (simulatedLatency > 0f)
			{
				yield return Wait.For(simulatedLatency).UnscaledSeconds().StartCoroutine();
			}
			CloudServerResult result = InternalListItems();
			completeHandler(result);
		}

		private CloudServerResult InternalUpdateItems(IEnumerable<ICloudItem> items)
		{
			ICloudItem[] array = items.ToArray();
			CloudServerResult cloudServerResult;
			if (simulateError)
			{
				cloudServerResult = new CloudServerResult();
				cloudServerResult.status = CloudSaveStatus.Error;
				cloudServerResult.items = null;
				return cloudServerResult;
			}
			List<ICloudItem> list = new List<ICloudItem>();
			ICloudItem[] array2 = array;
			foreach (ICloudItem cloudItem in array2)
			{
				if (!_items.ContainsKey(cloudItem.name))
				{
					list.Add(new Item(cloudItem.name, null, null, null, null, 0, null, null));
				}
			}
			foreach (KeyValuePair<string, Item> item in _items)
			{
				Item repositoryItem = item.Value;
				ICloudItem cloudItem2 = Array.Find(array, (ICloudItem item) => item.name == repositoryItem.name && item.revision == repositoryItem.revision);
				if (cloudItem2 == null)
				{
					list.Add(repositoryItem);
				}
			}
			list.Sort((ICloudItem a, ICloudItem b) => string.CompareOrdinal(a.name, b.name));
			cloudServerResult = new CloudServerResult();
			cloudServerResult.status = CloudSaveStatus.OK;
			cloudServerResult.items = list.ToArray();
			return cloudServerResult;
		}

		public static void DeleteServerDebug()
		{
			if (instance._localDir != null && instance._localDir.exists)
			{
				instance._localDir.Delete(true);
			}
		}

		private IEnumerator UpdateItemsCoroutine(IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerResult> completeHandler)
		{
			if (simulatedLatency > 0f)
			{
				yield return Wait.For(simulatedLatency).UnscaledSeconds().StartCoroutine();
			}
			CloudServerResult result = InternalUpdateItems(items);
			completeHandler(result);
		}

		private static string ToRevision(DateTime? value)
		{
			if (value.HasValue)
			{
				return ((long)(value.Value.ToUniversalTime() - _epoch).TotalMilliseconds).ToString("X");
			}
			return null;
		}

		private CloudServerCommitResult InternalCommitItems(IEnumerable<ICloudItem> items)
		{
			ICloudItem[] array = items.ToArray();
			CloudServerCommitResult cloudServerCommitResult;
			if (simulateError)
			{
				cloudServerCommitResult = new CloudServerCommitResult();
				cloudServerCommitResult.status = CloudSaveStatus.Error;
				return cloudServerCommitResult;
			}
			Dictionary<string, CloudServerCommitInfo> dictionary = new Dictionary<string, CloudServerCommitInfo>();
			ICloudItem[] array2 = array;
			foreach (ICloudItem cloudItem in array2)
			{
				byte[] bytes = cloudItem.GetBytes();
				dictionary[cloudItem.name] = ((bytes != null) ? new CloudServerCommitInfo
				{
					crc32 = Checksum.ComputeCrc32(bytes),
					size = bytes.Length,
					compressedSize = bytes.Length
				} : new CloudServerCommitInfo());
			}
			CloudSaveStatus cloudSaveStatus = CloudSaveStatus.OK;
			ICloudItem[] array3 = array;
			foreach (ICloudItem cloudItem2 in array3)
			{
				Item value;
				if (_items.TryGetValue(cloudItem2.name, out value))
				{
					if (value.revision != cloudItem2.revision)
					{
						cloudSaveStatus = CloudSaveStatus.OutOfDate;
						break;
					}
				}
				else if (!string.IsNullOrEmpty(cloudItem2.revision))
				{
					cloudSaveStatus = CloudSaveStatus.OutOfDate;
					break;
				}
			}
			if (cloudSaveStatus == CloudSaveStatus.OK)
			{
				DateTime value2 = DateTime.UtcNow.TrimToWholeMilliSeconds();
				string revision = ToRevision(value2);
				ICloudItem[] array4 = array;
				foreach (ICloudItem cloudItem3 in array4)
				{
					Item item = new Item(cloudItem3.name, revision, value2, cloudItem3.sourceDescription, cloudItem3.sourceInstallationId, cloudItem3.dataVersion, cloudItem3.userData, LoadItem);
					_items[cloudItem3.name] = item;
					SaveMetadata(item);
					_localDir.File(cloudItem3.name).SaveRawData(cloudItem3.GetBytes());
				}
			}
			cloudServerCommitResult = new CloudServerCommitResult();
			cloudServerCommitResult.status = cloudSaveStatus;
			cloudServerCommitResult.items = GetStatusList();
			cloudServerCommitResult.commitInfo = dictionary;
			return cloudServerCommitResult;
		}

		private IEnumerator CommitItemsCoroutine(IEnumerable<ICloudItem> items, CloudServerRequestCompleteHandler<CloudServerCommitResult> completeHandler)
		{
			if (simulatedLatency > 0f)
			{
				yield return Wait.For(simulatedLatency).UnscaledSeconds().StartCoroutine();
			}
			CloudServerCommitResult result = InternalCommitItems(items);
			completeHandler(result);
		}

		private void LoadRepository()
		{
			_items = new Dictionary<string, Item>();
			IEnumerable<IFile> files = _metadataDir.GetFiles();
			foreach (IFile item2 in files)
			{
				Item item = LoadMetadata(item2);
				if (item != null)
				{
					_items[item.name] = item;
				}
			}
		}

		private ICloudItem[] GetStatusList()
		{
			List<ICloudItem> list = new List<ICloudItem>();
			foreach (KeyValuePair<string, Item> item in _items)
			{
				Item value = item.Value;
				list.Add(new Item(value.name, value.revision, value.lastModified, value.sourceDescription, value.sourceInstallationId, value.dataVersion, value.userData, (string name) => (byte[])null));
			}
			return list.ToArray();
		}

		private void SaveMetadata(Item item)
		{
			Metadata metadata = new Metadata();
			metadata.revision = item.revision;
			metadata.lastModified = item.lastModified;
			metadata.sourceDescription = item.sourceDescription;
			metadata.sourceInstallationId = item.sourceInstallationId;
			metadata.dataVersion = item.dataVersion;
			metadata.userdata = ((item.userData == null) ? null : item.userData.ToDictionary());
			Metadata data = metadata;
			IFile file = _metadataDir.File(item.name);
			file.SaveJsonValue(data, _jsonSerializer);
		}

		private Item LoadMetadata(IFile file)
		{
			try
			{
				Metadata metadata = file.LoadJsonValue<Metadata>(_jsonSerializer);
				return new Item(file.name, metadata.revision, metadata.lastModified, metadata.sourceDescription, metadata.sourceInstallationId, metadata.dataVersion, (metadata.userdata == null) ? null : new CloudUserData(metadata.userdata), LoadItem);
			}
			catch (Exception ex)
			{
				LogError("LoadMetadata: Error loading \"" + file.name + "\", " + ex);
			}
			return null;
		}

		private byte[] LoadItem(string name)
		{
			try
			{
				return _localDir.File(name).LoadRawData();
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[SimulatedCloudServer] " + msg);
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("CLOUD_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[SimulatedCloudServer] " + msg);
		}
	}
}
