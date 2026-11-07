using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Ionic.Crc;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend.Cloud
{
	public sealed class CloudWorkingCopy
	{
		private class CommitContext
		{
			public string method;

			public ICloudItem[] modifiedLocalItems;

			public CloudServerCommitResult result;
		}

		private class Conflict : ICloudConflict
		{
			public string name
			{
				get
				{
					return mine.name;
				}
			}

			public ICloudItem mine { get; private set; }

			public ICloudItem theirs { get; private set; }

			public ICloudItem resolution { get; private set; }

			public CloudConflictResolution resolveMethod { get; private set; }

			public Conflict([NotNull] ICloudItem mine, [NotNull] ICloudItem theirs)
			{
				if (mine == null)
				{
					throw new ArgumentNullException("mine");
				}
				if (theirs == null)
				{
					throw new ArgumentNullException("theirs");
				}
				this.mine = mine;
				this.theirs = theirs;
				resolveMethod = CloudConflictResolution.NotResolved;
			}

			public void ResolveUsingMine()
			{
				resolution = new LocalItem(mine.name, theirs.revision, mine.lastModified, true, false, mine.sourceDescription, mine.sourceInstallationId, mine.dataVersion, mine.userData, true, (LocalItem s) => mine.GetDataStream());
				resolveMethod = CloudConflictResolution.UsingMine;
			}

			public void ResolveUsingTheirs()
			{
				resolution = theirs;
				resolveMethod = CloudConflictResolution.UsingTheirs;
			}

			public void ResolveWith(byte[] data, ICloudUserData userData)
			{
				byte[] copy = (byte[])data.Clone();
				resolution = new LocalItem(mine.name, theirs.revision, theirs.lastModified, true, false, mine.sourceDescription, mine.sourceInstallationId, mine.dataVersion, userData, false, (LocalItem s) => new MemoryStream(copy));
				resolveMethod = CloudConflictResolution.UsingCustom;
			}
		}

		public delegate void CompleteHandler(bool success, string errorMessage);

		public delegate void SynchronizeCompleteHandler(bool updateSuccess, bool commitSuccess, string errorMessage);

		private class ItemUpdate
		{
			public ICloudItem item;

			public IFile file;

			public IFile tempFile;

			public IFile bakFile;

			public bool fileRenamedToBak;

			public bool tempFileRenamed;
		}

		private class ItemMetadata
		{
			public string revision { get; private set; }

			public DateTime? lastModified { get; private set; }

			public bool isOutOfDate { get; private set; }

			public bool isModified { get; private set; }

			public int serverDataVersion { get; private set; }

			public int pristineCrc32 { get; private set; }

			public Dictionary<string, string> userData { get; private set; }

			public ItemMetadata(string revision = null, DateTime? lastModified = null, bool isOutOfDate = false, bool isModified = false, int serverDataVersion = 0, int pristineCrc32 = 0, Dictionary<string, string> userData = null)
			{
				this.revision = revision;
				this.lastModified = lastModified;
				this.isOutOfDate = isOutOfDate;
				this.isModified = isModified;
				this.serverDataVersion = serverDataVersion;
				this.pristineCrc32 = pristineCrc32;
				this.userData = ((userData == null) ? null : new Dictionary<string, string>(userData, StringComparer.OrdinalIgnoreCase));
			}

			public ItemMetadata(ICloudItem cloudItem, int pristineCrc32 = 0)
			{
				revision = cloudItem.revision;
				lastModified = cloudItem.lastModified;
				isOutOfDate = cloudItem.isOutOfDate;
				isModified = cloudItem.isModified;
				serverDataVersion = cloudItem.dataVersion;
				userData = ((cloudItem.userData == null) ? null : cloudItem.userData.ToDictionary());
				this.pristineCrc32 = pristineCrc32;
			}

			[Pure]
			public ItemMetadata WithRevision(string newRevision)
			{
				return new ItemMetadata(newRevision, lastModified, isOutOfDate, isModified, serverDataVersion, pristineCrc32, userData);
			}

			[Pure]
			public ItemMetadata WithIsOutOfDate(bool newIsOutOfDate)
			{
				return new ItemMetadata(revision, lastModified, newIsOutOfDate, isModified, serverDataVersion, pristineCrc32, userData);
			}

			[Pure]
			public ItemMetadata WithIsModified(bool newIsModified)
			{
				return new ItemMetadata(revision, lastModified, isOutOfDate, newIsModified, serverDataVersion, pristineCrc32, userData);
			}

			[Pure]
			public ItemMetadata WithServerDataVersion(int newServerDataVersion)
			{
				return new ItemMetadata(revision, lastModified, isOutOfDate, isModified, newServerDataVersion, pristineCrc32, userData);
			}

			[Pure]
			public ItemMetadata WithUserData(string key, string value)
			{
				ItemMetadata itemMetadata = new ItemMetadata(revision, lastModified, isOutOfDate, isModified, serverDataVersion, pristineCrc32, userData);
				if (value == null)
				{
					if (itemMetadata.userData != null)
					{
						itemMetadata.userData.Remove(key);
					}
				}
				else
				{
					if (itemMetadata.userData == null)
					{
						itemMetadata.userData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					}
					itemMetadata.userData[key] = value;
				}
				return itemMetadata;
			}

			[Pure]
			public ItemMetadata WithUserData(Dictionary<string, string> newUserData)
			{
				return new ItemMetadata(revision, lastModified, isOutOfDate, isModified, serverDataVersion, pristineCrc32, newUserData);
			}
		}

		private class LocalItem : ICloudItem
		{
			public delegate Stream GetDataStreamFunction(LocalItem localItem);

			private readonly GetDataStreamFunction _getDataStreamFunction;

			public string name { get; private set; }

			public string revision { get; private set; }

			public DateTime? lastModified { get; private set; }

			public bool isModified { get; private set; }

			public bool isOutOfDate { get; private set; }

			public string sourceDescription { get; private set; }

			public string sourceInstallationId { get; private set; }

			public int dataVersion { get; private set; }

			public ICloudUserData userData { get; private set; }

			public bool bytesAreSaved { get; private set; }

			public LocalItem(string name, string revision, DateTime? lastModified, bool isModified, bool isOutOfDate, string sourceDescription, string sourceInstallationId, int dataVersion, ICloudUserData userData, bool bytesAreSaved, GetDataStreamFunction getDataStreamFunction)
			{
				this.name = name;
				this.revision = revision;
				this.lastModified = lastModified;
				this.isModified = isModified;
				this.isOutOfDate = isOutOfDate;
				this.sourceDescription = sourceDescription;
				this.sourceInstallationId = sourceInstallationId;
				this.dataVersion = dataVersion;
				this.userData = userData;
				this.bytesAreSaved = bytesAreSaved;
				_getDataStreamFunction = getDataStreamFunction;
			}

			public Stream GetDataStream()
			{
				GetDataStreamFunction getDataStreamFunction = _getDataStreamFunction;
				if (getDataStreamFunction != null)
				{
					return getDataStreamFunction(this);
				}
				return null;
			}
		}

		private class Metadata
		{
			public string kilooTag;

			public Dictionary<string, ItemMetadata> items;
		}

		private class UpdateContext
		{
			public string method;

			public CloudConflictResolver conflictResolver;

			public ICloudItem[] localItemsWithRevisionIds;

			public CloudServerResult result;

			public List<ICloudConflict> conflicts;

			public List<ICloudItem> saveItems;

			public List<CloudItemUpdatedArgs> itemUpdatedEventArgs;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<CloudItemUpdatedArgs> itemUpdated__BackingField;

		private const string METADATA_FILE = ".cloudsave";

		private const string TEMP_SUFFIX = ".cloudtemp";

		private const string BAK_SUFFIX = ".cloudbak";

		private readonly ICloudServer _cloudServer;

		private readonly IDirectory _directory;

		private readonly ISerializer _serializer;

		private readonly string _installationId;

		private readonly int _dataVersion;

		private Metadata _meta;

		private bool _abortUpdate;

		private bool _abortSynchronize;

		private bool _isMetadataUpdating;

		private bool _isUpdating;

		private bool _isCommitting;

		private bool _isSynchronizing;

		public bool operationInProcess
		{
			get
			{
				return _isUpdating || _isCommitting || _isMetadataUpdating;
			}
		}

		public bool isModified
		{
			get
			{
				foreach (KeyValuePair<string, ItemMetadata> item in _meta.items)
				{
					if (item.Value.isModified)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isOutOfDate
		{
			get
			{
				foreach (KeyValuePair<string, ItemMetadata> item in _meta.items)
				{
					if (item.Value.isOutOfDate)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isIncompatible
		{
			get
			{
				foreach (KeyValuePair<string, ItemMetadata> item in _meta.items)
				{
					if (item.Value.serverDataVersion > _dataVersion)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isAccountMismatch
		{
			get
			{
				if (_meta.kilooTag != null && _cloudServer.kilooTag != null)
				{
					return _meta.kilooTag != _cloudServer.kilooTag;
				}
				return false;
			}
		}

		public string kilooTag
		{
			get
			{
				return _meta.kilooTag;
			}
		}

		public string sourceDescription { get; set; }

		public event EventHandler<CloudItemUpdatedArgs> itemUpdated
		{
			add
			{
				EventHandler<CloudItemUpdatedArgs> eventHandler = itemUpdated__BackingField;
				EventHandler<CloudItemUpdatedArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref itemUpdated__BackingField, (EventHandler<CloudItemUpdatedArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<CloudItemUpdatedArgs> eventHandler = itemUpdated__BackingField;
				EventHandler<CloudItemUpdatedArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref itemUpdated__BackingField, (EventHandler<CloudItemUpdatedArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public CloudWorkingCopy(int dataVersion, [NotNull] ICloudServer cloudServer, [NotNull] IDirectory directory, [NotNull] ISerializer serializer, string installationId = null)
		{
			if (cloudServer == null)
			{
				throw new ArgumentNullException("cloudServer");
			}
			if (directory == null)
			{
				throw new ArgumentNullException("directory");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			_dataVersion = dataVersion;
			_cloudServer = cloudServer;
			_directory = directory;
			_serializer = serializer;
			_installationId = installationId;
			sourceDescription = SystemInfo.deviceName;
			if (sourceDescription == "<unknown>")
			{
				sourceDescription = SystemInfo.deviceModel;
			}
			LoadWorkingCopy();
			ScanForLocalModifications();
		}

		public static void DeleteWorkingCopy([NotNull] IDirectory directory)
		{
			directory.File(".cloudsave").Delete();
		}

		public void Add(string file)
		{
			if (InternalAdd(file))
			{
				SaveWorkingCopy();
			}
		}

		private bool InternalAdd(string file)
		{
			string text = _directory.ToRelativePath(file);
			if (!_meta.items.ContainsKey(text))
			{
				Dictionary<string, ItemMetadata> items = _meta.items;
				bool exists = _directory.File(text).exists;
				items[text] = new ItemMetadata(null, null, false, exists);
				return true;
			}
			return false;
		}

		public void SetUserData(string file, string key, string value)
		{
			string key2 = _directory.ToRelativePath(file);
			ItemMetadata value2;
			if (!_meta.items.TryGetValue(key2, out value2))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(key, value);
				dictionary = dictionary;
				value2 = new ItemMetadata(null, null, false, true, 0, 0, dictionary);
			}
			else
			{
				value2 = value2.WithUserData(key, value).WithIsModified(true);
			}
			_meta.items[key2] = value2;
			SaveWorkingCopy();
		}

		public void SetUserData(string file, Dictionary<string, string> userData)
		{
			string key = _directory.ToRelativePath(file);
			ItemMetadata value;
			if (!_meta.items.TryGetValue(key, out value))
			{
				value = new ItemMetadata(null, null, false, true, 0, 0, userData);
			}
			else
			{
				value = value.WithUserData(userData).WithIsModified(true);
			}
			_meta.items[key] = value;
			SaveWorkingCopy();
		}

		public void SetUserData(string file, ICloudUserData userData)
		{
			SetUserData(file, (userData == null) ? null : userData.ToDictionary());
		}

		public bool IsFileAdded(string file)
		{
			string key = _directory.ToRelativePath(file);
			return _meta.items.ContainsKey(key);
		}

		public ICloudItem[] GetItems()
		{
			return (from entry in _meta.items
				let item = entry.Value
				select (ICloudItem)new LocalItem(entry.Key, item.revision, item.lastModified, item.isModified, item.isOutOfDate, sourceDescription, _installationId, _dataVersion, (item.userData == null) ? null : new CloudUserData(item.userData), true, OpenLocalItemFile)).ToArray();
		}

		public ICloudItem GetItem(string file)
		{
			string name = _directory.ToRelativePath(file);
			return GetLocalItem(name);
		}

		public string GetFile(ICloudItem item)
		{
			return item.name;
		}

		public string[] GetFiles()
		{
			return _meta.items.Select((KeyValuePair<string, ItemMetadata> item) => item.Key).ToArray();
		}

		public void Clear(bool keepFiles = false)
		{
			if (!keepFiles)
			{
				foreach (KeyValuePair<string, ItemMetadata> item in _meta.items)
				{
					_directory.File(item.Key).Delete();
				}
			}
			_meta = new Metadata
			{
				items = new Dictionary<string, ItemMetadata>()
			};
			SaveWorkingCopy();
		}

		public void ResetFile(string file)
		{
			string text = _directory.ToRelativePath(file);
			if (text != null)
			{
				_meta.items[text] = new ItemMetadata();
			}
			SaveWorkingCopy();
		}

		public void RefreshIsModified()
		{
			if (!isModified)
			{
				ScanForLocalModifications();
			}
		}

		public void ScanForLocalModifications()
		{
			Dictionary<string, ItemMetadata> dictionary = new Dictionary<string, ItemMetadata>();
			foreach (string key in _meta.items.Keys)
			{
				ItemMetadata itemMetadata = _meta.items[key];
				if (itemMetadata.isModified)
				{
					continue;
				}
				bool flag;
				try
				{
					IFile file = _directory.File(key);
					if (itemMetadata.pristineCrc32 == 0)
					{
						flag = file.exists;
					}
					else if (file.exists)
					{
						using (Stream stream = file.OpenRead())
						{
							int num = Checksum.ComputeCrc32(stream);
							flag = num != itemMetadata.pristineCrc32;
						}
					}
					else
					{
						flag = true;
					}
				}
				catch (Exception inner)
				{
					throw new CloudException("Error scanning for local modifications.", inner);
				}
				if (flag)
				{
					dictionary[key] = itemMetadata.WithIsModified(true);
				}
			}
			if (dictionary.Count <= 0)
			{
				return;
			}
			foreach (KeyValuePair<string, ItemMetadata> item in dictionary)
			{
				_meta.items[item.Key] = item.Value;
			}
			SaveWorkingCopy();
		}

		public Coroutine UpdateAsync(CloudConflictResolver conflictResolver, CompleteHandler completeHandler = null)
		{
			return CoroutineUtil.StartCoroutine(UpdateCoroutine(conflictResolver, completeHandler));
		}

		public void Update(CloudConflictResolver conflictResolver)
		{
			UpdateContext updateContext = PrepareUpdateContext("Update", conflictResolver);
			BeginUpdate();
			try
			{
				updateContext.result = _cloudServer.UpdateItems(_meta.kilooTag, updateContext.localItemsWithRevisionIds);
				if (!_abortUpdate)
				{
					ProcessUpdateResult(updateContext);
					ApplyUpdate(updateContext);
					RaiseItemUpdatedWithArgs(updateContext.itemUpdatedEventArgs);
				}
			}
			finally
			{
				EndUpdate();
			}
		}

		public Coroutine CommitAsync(CompleteHandler completeHandler = null)
		{
			return CoroutineUtil.StartCoroutine(CommitCoroutine(completeHandler));
		}

		public void Commit()
		{
			CommitContext commitContext = PrepareCommitContext("Commit");
			if (commitContext.modifiedLocalItems.Length == 0)
			{
				return;
			}
			BeginCommit();
			try
			{
				commitContext.result = _cloudServer.CommitItems(_meta.kilooTag, commitContext.modifiedLocalItems);
				ProcessCommitResult(commitContext);
			}
			finally
			{
				EndCommit();
			}
		}

		public Coroutine UpdateMetadataAsync(CompleteHandler completeHandler = null)
		{
			return CoroutineUtil.StartCoroutine(UpdateMetadataCoroutine(completeHandler));
		}

		public void UpdateMetadata()
		{
			CommitContext commitContext = PrepareUpdateMetadataContext("UpdateMetadata");
			BeginUpdateMetadata();
			try
			{
				CloudServerResult cloudServerResult = _cloudServer.ListItems(_meta.kilooTag);
				commitContext.result = new CloudServerCommitResult
				{
					status = cloudServerResult.status,
					items = cloudServerResult.items,
					commitInfo = new Dictionary<string, CloudServerCommitInfo>()
				};
				ProcessCommitResult(commitContext);
			}
			finally
			{
				EndUpdateMetadata();
			}
		}

		public Coroutine SynchronizeAsync(CloudConflictResolver conflictResolver, SynchronizeCompleteHandler completeHandler = null)
		{
			return CoroutineUtil.StartCoroutine(SynchronizeCoroutine(conflictResolver, completeHandler));
		}

		public void Synchronize(CloudConflictResolver conflictResolver)
		{
			if (operationInProcess || _isSynchronizing)
			{
				throw new CloudException("Synchronize: Cannot synchronize while another update or commit operation is running.");
			}
			BeginSynchronize();
			try
			{
				Update(conflictResolver);
				if (_abortSynchronize)
				{
					throw new CloudException("Synchronize was aborted.");
				}
				Commit();
			}
			finally
			{
				EndSynchronize();
			}
		}

		public void Abort()
		{
			if (_isUpdating)
			{
				_abortUpdate = true;
			}
			if (_isSynchronizing)
			{
				_abortSynchronize = true;
			}
		}

		private void LoadWorkingCopy()
		{
			try
			{
				IFile file = _directory.File(".cloudsave");
				if (file.exists)
				{
					_meta = file.LoadObject<Metadata>(_serializer);
					return;
				}
				_meta = new Metadata
				{
					items = new Dictionary<string, ItemMetadata>()
				};
			}
			catch (Exception inner)
			{
				throw new CloudException("Error loading working copy metadata", inner);
			}
		}

		private void SaveWorkingCopy()
		{
			try
			{
				IFile file = _directory.File(".cloudsave");
				file.SaveObject(_meta, _serializer);
			}
			catch (Exception inner)
			{
				throw new CloudException("Error saving working copy metadata.", inner);
			}
		}

		private Stream OpenLocalItemFile(LocalItem item)
		{
			IFile file = _directory.File(item.name);
			if (file.exists)
			{
				return file.OpenRead();
			}
			return null;
		}

		private LocalItem GetLocalItem(string name)
		{
			ItemMetadata value;
			_meta.items.TryGetValue(name, out value);
			if (value != null)
			{
				return new LocalItem(name, value.revision, value.lastModified, value.isModified, value.isOutOfDate, sourceDescription, _installationId, _dataVersion, (value.userData == null) ? null : new CloudUserData(value.userData), true, OpenLocalItemFile);
			}
			return null;
		}

		[CanBeNull]
		private ItemMetadata GetItemMetadata(string name)
		{
			ItemMetadata value;
			_meta.items.TryGetValue(name, out value);
			return value;
		}

		private void RaiseItemUpdatedWithArgs(List<CloudItemUpdatedArgs> args)
		{
			if (args == null || args.Count <= 0)
			{
				return;
			}
			EventHandler<CloudItemUpdatedArgs> eventHandler = itemUpdated__BackingField;
			if (eventHandler == null)
			{
				return;
			}
			foreach (CloudItemUpdatedArgs arg in args)
			{
				try
				{
					eventHandler(this, arg);
				}
				catch (Exception ex)
				{
					LogError("Exception in itemUpdated handler: " + ex);
				}
			}
		}

		[NotNull]
		private UpdateContext PrepareUpdateContext(string method, CloudConflictResolver conflictResolver)
		{
			if (operationInProcess)
			{
				throw new CloudException(method + ": Cannot update while another update or commit operation is running.");
			}
			if (isAccountMismatch)
			{
				throw new CloudException(method + ": Working copy belongs to another account.");
			}
			if (isIncompatible)
			{
				throw new CloudException(method + ": Cannot update because working copy is incompatible with server data version.");
			}
			UpdateContext updateContext = new UpdateContext();
			updateContext.method = method;
			updateContext.conflictResolver = conflictResolver;
			updateContext.localItemsWithRevisionIds = (from element in _meta.items
				let item = element.Value
				where !string.IsNullOrEmpty(item.revision)
				select (ICloudItem)GetLocalItem(element.Key)).ToArray();
			return updateContext;
		}

		private void ProcessUpdateResult(UpdateContext context)
		{
			if (context.result == null || context.result.status != CloudSaveStatus.OK)
			{
				throw new CloudException("Update request failed.");
			}
			bool flag = false;
			context.conflicts = new List<ICloudConflict>();
			context.saveItems = new List<ICloudItem>();
			bool flag2 = false;
			if (context.result.items != null && context.result.items.Length > 0)
			{
				ScanForLocalModifications();
				ICloudItem[] items = context.result.items;
				foreach (ICloudItem cloudItem in items)
				{
					InternalAdd(cloudItem.name);
					ItemMetadata itemMetadata = GetItemMetadata(cloudItem.name);
					ItemMetadata value;
					if (cloudItem.dataVersion > _dataVersion)
					{
						if (itemMetadata == null)
						{
							itemMetadata = new ItemMetadata();
						}
						value = itemMetadata.WithIsOutOfDate(true).WithServerDataVersion(cloudItem.dataVersion);
						flag2 = true;
					}
					else if (itemMetadata != null)
					{
						if (string.IsNullOrEmpty(cloudItem.revision))
						{
							value = itemMetadata.WithIsModified(true).WithIsOutOfDate(false).WithRevision(null);
						}
						else if (itemMetadata.isModified)
						{
							if (_installationId != null && cloudItem.sourceInstallationId == _installationId && itemMetadata.revision != null)
							{
								value = itemMetadata.WithRevision(cloudItem.revision).WithIsOutOfDate(false);
							}
							else
							{
								value = itemMetadata.WithIsOutOfDate(true);
								context.conflicts.Add(new Conflict(GetLocalItem(cloudItem.name), cloudItem));
							}
						}
						else
						{
							value = itemMetadata.WithIsOutOfDate(true);
							context.saveItems.Add(cloudItem);
						}
					}
					else
					{
						int dataVersion = cloudItem.dataVersion;
						value = new ItemMetadata(null, null, true, false, dataVersion);
						context.saveItems.Add(cloudItem);
					}
					_meta.items[cloudItem.name] = value;
				}
				flag = true;
			}
			if (_meta.kilooTag == null && _cloudServer.kilooTag != null)
			{
				_meta.kilooTag = _cloudServer.kilooTag;
				flag = true;
			}
			if (flag)
			{
				SaveWorkingCopy();
			}
			if (flag2)
			{
				throw new CloudException("Incompatible data version.");
			}
			if (context.conflicts.Count > 0 && context.conflictResolver != null)
			{
				try
				{
					context.conflictResolver(context.conflicts.ToArray());
				}
				catch (Exception inner)
				{
					throw new CloudException("Conflict resolver exception.", inner);
				}
			}
		}

		private void ApplyUpdate(UpdateContext context)
		{
			List<ICloudConflict> list = new List<ICloudConflict>(context.conflicts.Where((ICloudConflict conflict) => conflict.resolveMethod != CloudConflictResolution.NotResolved));
			foreach (ICloudConflict item in list)
			{
				context.saveItems.Add(item.resolution);
				context.conflicts.Remove(item);
			}
			if (context.conflicts.Count != 0)
			{
				throw new CloudException("Update has unresolved conflicts");
			}
			IFile file = _directory.File(".cloudsave");
			IFile file2 = _directory.File(".cloudsave.cloudtemp");
			IFile file3 = _directory.File(".cloudsave.cloudbak");
			bool flag = false;
			bool flag2 = false;
			List<ItemUpdate> list2 = new List<ItemUpdate>();
			Metadata metadata = new Metadata();
			metadata.kilooTag = _meta.kilooTag;
			metadata.items = _meta.items.ToDictionary((KeyValuePair<string, ItemMetadata> pair) => pair.Key, (KeyValuePair<string, ItemMetadata> pair) => pair.Value);
			Metadata metadata2 = metadata;
			try
			{
				foreach (ICloudItem saveItem in context.saveItems)
				{
					LocalItem localItem = saveItem as LocalItem;
					int pristineCrc = 0;
					if (localItem == null || !localItem.bytesAreSaved)
					{
						bool calclulateCrc = !saveItem.isModified;
						IFile file4 = _directory.File(saveItem.name + ".cloudtemp");
						pristineCrc = SaveItemData(saveItem, file4, calclulateCrc);
						list2.Add(new ItemUpdate
						{
							item = saveItem,
							file = _directory.File(saveItem.name),
							tempFile = file4
						});
					}
					metadata2.items[saveItem.name] = new ItemMetadata(saveItem, pristineCrc);
				}
				file2.SaveObject(metadata2, _serializer);
				flag2 = true;
				List<CloudItemUpdatedArgs> list3 = new List<CloudItemUpdatedArgs>();
				foreach (ItemUpdate item2 in list2)
				{
					item2.bakFile = _directory.File(item2.item.name + ".cloudbak");
					if (item2.file.exists)
					{
						item2.file.MoveTo(item2.bakFile);
						item2.fileRenamedToBak = true;
					}
					if (item2.tempFile.exists)
					{
						item2.tempFile.MoveTo(item2.file);
						item2.tempFileRenamed = true;
					}
					list3.Add(new CloudItemUpdatedArgs(item2.item, item2.item.name));
				}
				file.MoveTo(file3);
				flag = true;
				file2.MoveTo(file);
				_meta = metadata2;
				context.itemUpdatedEventArgs = list3;
			}
			catch (Exception inner)
			{
				foreach (ItemUpdate item3 in list2)
				{
					if (item3.tempFileRenamed)
					{
						item3.file.Delete();
					}
					else
					{
						item3.tempFile.Delete();
					}
					if (item3.fileRenamedToBak)
					{
						item3.bakFile.MoveTo(item3.file);
					}
				}
				if (flag2)
				{
					file2.Delete();
					if (flag)
					{
						file3.MoveTo(file);
					}
				}
				throw new CloudException("Failed to apply update.", inner);
			}
			finally
			{
				try
				{
					foreach (ItemUpdate item4 in list2)
					{
						item4.bakFile.Delete();
					}
					file3.Delete();
				}
				catch (Exception ex)
				{
					LogError("Error deleting bak files: " + ex);
				}
			}
		}

		private void BeginUpdate()
		{
			_abortUpdate = false;
			_isUpdating = true;
		}

		private void EndUpdate()
		{
			_abortUpdate = false;
			_isUpdating = false;
		}

		private void EndUpdateWithError(string message, CompleteHandler completeHandler)
		{
			EndUpdate();
			if (completeHandler != null)
			{
				completeHandler(false, message);
			}
			LogError("Update failed: " + message);
		}

		private void EndUpdateWithSuccess(CompleteHandler completeHandler)
		{
			EndUpdate();
			if (completeHandler != null)
			{
				completeHandler(true, null);
			}
		}

		private IEnumerator UpdateCoroutine(CloudConflictResolver conflictResolver, CompleteHandler completeHandler)
		{
			UpdateContext context;
			try
			{
				context = PrepareUpdateContext("UpdateAsync", conflictResolver);
			}
			catch (CloudException ex)
			{
				EndUpdateWithError(ex.Message, completeHandler);
				yield break;
			}
			BeginUpdate();
			yield return _cloudServer.UpdateItemsAsync(_meta.kilooTag, context.localItemsWithRevisionIds, (CloudServerResult result) =>
			{
				context.result = result;
			});
			if (!_abortUpdate)
			{
				try
				{
					ProcessUpdateResult(context);
				}
				catch (CloudException ex2)
				{
					EndUpdateWithError(ex2.Message, completeHandler);
					yield break;
				}
				foreach (ICloudConflict conflict in context.conflicts)
				{
					while (conflict.resolution == null && !_abortUpdate)
					{
						yield return null;
					}
				}
				if (!_abortUpdate)
				{
					try
					{
						ApplyUpdate(context);
					}
					catch (CloudException ex3)
					{
						EndUpdateWithError(ex3.Message, completeHandler);
						yield break;
					}
				}
			}
			if (_abortUpdate)
			{
				EndUpdateWithError("Update was aborted.", completeHandler);
				yield break;
			}
			RaiseItemUpdatedWithArgs(context.itemUpdatedEventArgs);
			EndUpdateWithSuccess(completeHandler);
		}

		[NotNull]
		private CommitContext PrepareCommitContext(string method)
		{
			if (operationInProcess)
			{
				throw new CloudException(method + ": Cannot commit while another update or commit operation is running.");
			}
			if (isAccountMismatch)
			{
				throw new CloudException(method + ": Working copy belongs to another account.");
			}
			if (isOutOfDate)
			{
				throw new CloudException(method + ": Cannot commit when working copy is out of date. Update first.");
			}
			ScanForLocalModifications();
			CommitContext commitContext = new CommitContext();
			commitContext.method = method;
			commitContext.modifiedLocalItems = _meta.items.Where((KeyValuePair<string, ItemMetadata> entry) => entry.Value.isModified).Select((Func<KeyValuePair<string, ItemMetadata>, ICloudItem>)((KeyValuePair<string, ItemMetadata> entry) => GetLocalItem(entry.Key))).ToArray();
			return commitContext;
		}

		private void ProcessCommitResult(CommitContext context)
		{
			ICloudItem[] array = ((context.result.status != CloudSaveStatus.OK) ? new ICloudItem[0] : context.modifiedLocalItems);
			ICloudItem[] items = context.result.items;
			if (items != null)
			{
				List<string> list = new List<string>(_meta.items.Keys);
				foreach (string localItemName in list)
				{
					ICloudItem cloudItem = Array.Find(items, (ICloudItem item) => item.name == localItemName);
					if (cloudItem == null || cloudItem.revision == null)
					{
						_meta.items[localItemName] = _meta.items[localItemName].WithRevision(null).WithIsModified(true).WithIsOutOfDate(false);
					}
				}
				ICloudItem[] array2 = items;
				foreach (ICloudItem theirItem in array2)
				{
					ItemMetadata itemMetadata = GetItemMetadata(theirItem.name);
					if (itemMetadata == null || theirItem.revision != itemMetadata.revision)
					{
						if (itemMetadata != null && Array.Find(array, (ICloudItem item) => item.name == theirItem.name) != null)
						{
							_meta.items[theirItem.name] = new ItemMetadata(theirItem, context.result.commitInfo[theirItem.name].crc32);
						}
						else if (itemMetadata == null)
						{
							Dictionary<string, ItemMetadata> items2 = _meta.items;
							string name = theirItem.name;
							int dataVersion = theirItem.dataVersion;
							items2[name] = new ItemMetadata(null, null, true, false, dataVersion);
						}
						else
						{
							_meta.items[theirItem.name] = itemMetadata.WithIsOutOfDate(true).WithServerDataVersion(theirItem.dataVersion);
						}
					}
				}
				if (_meta.kilooTag == null && _cloudServer.kilooTag != null)
				{
					_meta.kilooTag = _cloudServer.kilooTag;
				}
				SaveWorkingCopy();
			}
			if (context.result.status == CloudSaveStatus.OutOfDate)
			{
				throw new CloudException(context.method + " failed because local data is out of date.");
			}
			if (context.result.status != CloudSaveStatus.OK)
			{
				if (context.result.errorMessage != null)
				{
					throw new CloudException(context.method + " failed with error: " + context.result.errorMessage);
				}
				throw new CloudException(context.method + " failed with status: " + context.result.status);
			}
		}

		private void BeginCommit()
		{
			_isCommitting = true;
		}

		private void EndCommit()
		{
			_isCommitting = false;
		}

		private void EndCommitWithError(string message, CompleteHandler completeHandler)
		{
			EndCommit();
			if (completeHandler != null)
			{
				completeHandler(false, message);
			}
			LogError("Commit failed: " + message);
		}

		private void EndCommitWithSuccess(CompleteHandler completeHandler)
		{
			EndCommit();
			if (completeHandler != null)
			{
				completeHandler(true, null);
			}
		}

		private IEnumerator CommitCoroutine(CompleteHandler completeHandler)
		{
			CommitContext context;
			try
			{
				context = PrepareCommitContext("CommitAsync");
			}
			catch (CloudException ex)
			{
				EndCommitWithError(ex.Message, completeHandler);
				yield break;
			}
			if (context.modifiedLocalItems.Length > 0)
			{
				BeginCommit();
				yield return _cloudServer.CommitItemsAsync(_meta.kilooTag, context.modifiedLocalItems, (CloudServerCommitResult result) =>
				{
					context.result = result;
				});
				try
				{
					ProcessCommitResult(context);
				}
				catch (CloudException ex2)
				{
					EndCommitWithError(ex2.Message, completeHandler);
					yield break;
				}
			}
			EndCommitWithSuccess(completeHandler);
		}

		[NotNull]
		private CommitContext PrepareUpdateMetadataContext(string method)
		{
			if (operationInProcess)
			{
				throw new CloudException(method + ": Cannot update metadata while another update or commit operation is running.");
			}
			if (isAccountMismatch)
			{
				throw new CloudException(method + ": Working copy belongs to another account.");
			}
			CommitContext commitContext = new CommitContext();
			commitContext.method = method;
			commitContext.modifiedLocalItems = new ICloudItem[0];
			return commitContext;
		}

		private void BeginUpdateMetadata()
		{
			_isMetadataUpdating = true;
		}

		private void EndUpdateMetadata()
		{
			_isMetadataUpdating = false;
		}

		private void EndUpdateMetadataWithError(string message, CompleteHandler completeHandler)
		{
			EndUpdateMetadata();
			if (completeHandler != null)
			{
				completeHandler(false, message);
			}
			LogError("Update metadata failed: " + message);
		}

		private void EndUpdateMetadataWithSuccess(CompleteHandler completeHandler)
		{
			EndUpdateMetadata();
			if (completeHandler != null)
			{
				completeHandler(true, null);
			}
		}

		private IEnumerator UpdateMetadataCoroutine(CompleteHandler completeHandler)
		{
			CommitContext context;
			try
			{
				context = PrepareUpdateMetadataContext("UpdateMetadataAsync");
			}
			catch (CloudException ex)
			{
				EndUpdateMetadataWithError(ex.Message, completeHandler);
				yield break;
			}
			BeginUpdateMetadata();
			yield return _cloudServer.ListItemsAsync(_meta.kilooTag, (CloudServerResult result) =>
			{
				context.result = new CloudServerCommitResult
				{
					status = result.status,
					items = result.items,
					commitInfo = new Dictionary<string, CloudServerCommitInfo>()
				};
			});
			try
			{
				ProcessCommitResult(context);
			}
			catch (CloudException ex2)
			{
				EndUpdateMetadataWithError(ex2.Message, completeHandler);
				yield break;
			}
			EndUpdateMetadataWithSuccess(completeHandler);
		}

		private void BeginSynchronize()
		{
			_abortSynchronize = false;
			_isSynchronizing = true;
		}

		private void EndSynchronize()
		{
			_isSynchronizing = false;
			_abortSynchronize = false;
		}

		private void EndSynchronizeWithError(bool updateSuccess, string message, SynchronizeCompleteHandler completeHandler)
		{
			EndSynchronize();
			if (completeHandler != null)
			{
				completeHandler(updateSuccess, false, message);
			}
			LogError("Synchronize failed: " + message);
		}

		private void EndSynchronizeWithSuccess(SynchronizeCompleteHandler completeHandler)
		{
			EndSynchronize();
			if (completeHandler != null)
			{
				completeHandler(true, true, null);
			}
		}

		private IEnumerator SynchronizeCoroutine(CloudConflictResolver conflictResolver, SynchronizeCompleteHandler completeHandler)
		{
			if (_isSynchronizing || operationInProcess)
			{
				EndSynchronizeWithError(false, "Cannot synchronize while another update or commit operation is running.", completeHandler);
				yield break;
			}
			BeginSynchronize();
			bool updateSuccess = false;
			string updateMessage = null;
			yield return CoroutineUtil.StartCoroutine(UpdateCoroutine(conflictResolver, (bool success, string error) =>
			{
				updateSuccess = success;
				updateMessage = error;
			}));
			if (!updateSuccess)
			{
				EndSynchronizeWithError(false, updateMessage, completeHandler);
				yield break;
			}
			if (_abortSynchronize)
			{
				EndSynchronizeWithError(true, "Synchronize was aborted.", completeHandler);
				yield break;
			}
			bool commitSuccess = false;
			string commitMessage = null;
			yield return CoroutineUtil.StartCoroutine(CommitCoroutine((bool success, string error) =>
			{
				commitSuccess = success;
				commitMessage = error;
			}));
			if (!commitSuccess)
			{
				EndSynchronizeWithError(true, commitMessage, completeHandler);
			}
			else
			{
				EndSynchronizeWithSuccess(completeHandler);
			}
		}

		private int SaveItemData(ICloudItem item, IFile file, bool calclulateCrc32)
		{
			int result = 0;
			using (Stream stream = item.GetDataStream())
			{
				if (stream != null)
				{
					using (Stream stream2 = file.OpenWrite(true))
					{
						if (calclulateCrc32)
						{
							CRC32 cRC = new CRC32();
							result = cRC.GetCrc32AndCopy(stream, stream2);
						}
						else
						{
							stream.CopyTo(stream2);
						}
					}
				}
				else
				{
					file.Delete();
				}
			}
			return result;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[CloudWorkingCopy] " + msg);
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("CLOUD_DEBUG_LOGS")]
		private static void LogEach<T>(IEnumerable<T> items, Action<T> logMethod)
		{
			foreach (T item in items)
			{
				logMethod(item);
			}
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("CLOUD_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[CloudWorkingCopy] " + msg);
		}
	}
}
