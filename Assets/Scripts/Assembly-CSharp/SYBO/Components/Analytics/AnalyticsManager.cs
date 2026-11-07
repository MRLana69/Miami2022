using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using SYBO.Components.Json;
using SYBO.Components.Logging;

namespace SYBO.Components.Analytics
{
	public class AnalyticsManager
	{
		public static readonly string DefaultUserID = "00000000-0000-f000-0000-000000000000";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> OnEventPosted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action OnSendEventsChunkToServerStarted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<ProcessAnalyticsEventsResponse> OnSendEventsChunkToServerFinished__BackingField;

		private bool _validUser;

		private bool _inFlush;

		private ProcessAnalyticsEventsResponse _lastFlushServerResponse;

		private bool _flushDelayed;

		private bool _retryOnTimeoutOr5xxError;

		private DateTime _deviceStartupTime;

		private string _userID = string.Empty;

		private string _gameVersion = string.Empty;

		private PersistentAnalyticsEventLog _eventLog;

		private ICoroutineHandler _coroutineHost;

		private static long _tickOffset;

		private static long _lastTick = -1L;

		public IPostAnalytics Endpoint { get; set; }

		public bool InFlush
		{
			get
			{
				return _inFlush;
			}
		}

		public ProcessAnalyticsEventsResponse LastFlushServerResponse
		{
			get
			{
				return _lastFlushServerResponse;
			}
		}

		public bool HasValidUserID
		{
			get
			{
				return _validUser;
			}
		}

		public string GameVersion
		{
			get
			{
				return _gameVersion;
			}
			set
			{
				_gameVersion = value;
			}
		}

		public event Action<string> OnEventPosted
		{
			add
			{
				Action<string> action = OnEventPosted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnEventPosted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = OnEventPosted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnEventPosted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action OnSendEventsChunkToServerStarted
		{
			add
			{
				Action action = OnSendEventsChunkToServerStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnSendEventsChunkToServerStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnSendEventsChunkToServerStarted__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnSendEventsChunkToServerStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<ProcessAnalyticsEventsResponse> OnSendEventsChunkToServerFinished
		{
			add
			{
				Action<ProcessAnalyticsEventsResponse> action = OnSendEventsChunkToServerFinished__BackingField;
				Action<ProcessAnalyticsEventsResponse> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnSendEventsChunkToServerFinished__BackingField, (Action<ProcessAnalyticsEventsResponse>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<ProcessAnalyticsEventsResponse> action = OnSendEventsChunkToServerFinished__BackingField;
				Action<ProcessAnalyticsEventsResponse> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnSendEventsChunkToServerFinished__BackingField, (Action<ProcessAnalyticsEventsResponse>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public AnalyticsManager(IPostAnalytics endpoint, PersistentAnalyticsEventLog eventLog, ICoroutineHandler coroutineHost)
		{
			Endpoint = endpoint;
			_eventLog = eventLog;
			_deviceStartupTime = DateTime.UtcNow.AddMilliseconds(-GetMsTicksSinceDeviceStartup());
			_userID = DefaultUserID;
			_validUser = false;
			_coroutineHost = coroutineHost;
			AnalyticsEvent.GetDeviceStartupTime = GetDeviceStartupTime;
			AnalyticsEvent.GetUserID = GetUserID;
			AnalyticsEvent.GetGameVersion = GetGameVersion;
		}

		private string GetGameVersion()
		{
			return _gameVersion;
		}

		public void Shutdown()
		{
			_eventLog.Shutdown();
		}

		public void PostEvent<T>(T eventData) where T : AnalyticsEvent
		{
			if (eventData != null)
			{
				TransferEvent<T> obj = new TransferEvent<T>(eventData, eventData.EventName);
				string eventJson = JsonApi.Serialize(obj);
				PostEvent(eventJson);
			}
		}

		public void SetUserID(string userID)
		{
			if (!(_userID == userID))
			{
				_userID = userID;
				_validUser = true;
				if (_flushDelayed)
				{
					_flushDelayed = false;
					Flush();
				}
			}
		}

		public void ChangeUserID(string userID)
		{
			if (!(_userID == userID))
			{
				_eventLog.Replace(_userID, userID);
				_userID = userID;
				_validUser = true;
				if (_flushDelayed)
				{
					_flushDelayed = false;
					Flush();
				}
			}
		}

		public void Flush()
		{
			if (!_validUser)
			{
				_flushDelayed = true;
			}
			else if (!_inFlush)
			{
				_retryOnTimeoutOr5xxError = true;
				FlushInternal();
			}
		}

		private DateTime GetDeviceStartupTime()
		{
			return _deviceStartupTime;
		}

		public string GetUserID()
		{
			return _userID;
		}

		private void PostEvent(string eventJson)
		{
			_eventLog.Append(eventJson);
			if (OnEventPosted__BackingField != null)
			{
				OnEventPosted__BackingField(eventJson);
			}
		}

		private void FlushInternal()
		{
			if (Endpoint == null)
			{
				Log.Analytics.LogErrorFormat("AnalyticsManager: Tried to Flush, but Endpoint == null");
				return;
			}
			_inFlush = true;
			AsyncRes nextChunkForServerAsync = _eventLog.GetNextChunkForServerAsync();
			_coroutineHost.StartCoroutine(WaitForResult(nextChunkForServerAsync, HandleJsonChunk));
		}

		private IEnumerator WaitForResult(AsyncRes res, Action<byte[], int, string> receiver)
		{
			while (!res.IsReady)
			{
				yield return null;
			}
			receiver((byte[])res.Result, res.ChunkLength, res.UncompressedMD5Hash);
		}

		private void HandleJsonChunk(byte[] eventsJson, int uncompressedChunkLength, string uncompressedMD5Hash)
		{
			if (eventsJson == null || eventsJson.Length <= 2)
			{
				_inFlush = false;
				return;
			}
			if (OnSendEventsChunkToServerStarted__BackingField != null)
			{
				OnSendEventsChunkToServerStarted__BackingField();
			}
			Endpoint.PostAnalyticsEvents(eventsJson, uncompressedChunkLength, uncompressedMD5Hash, HandlePostAnalyticsReponse);
		}

		private void HandlePostAnalyticsReponse(ProcessAnalyticsEventsResponse response)
		{
			_lastFlushServerResponse = response;
			if (response == ProcessAnalyticsEventsResponse.AcceptedByServer || response == ProcessAnalyticsEventsResponse.RejectedByServer)
			{
				_eventLog.ConsumeChunk();
			}
			if (OnSendEventsChunkToServerFinished__BackingField != null)
			{
				OnSendEventsChunkToServerFinished__BackingField(response);
			}
			bool flag = false;
			switch (response)
			{
			case ProcessAnalyticsEventsResponse.BackendUnavailable:
				flag = _retryOnTimeoutOr5xxError;
				_retryOnTimeoutOr5xxError = false;
				break;
			default:
				flag = _eventLog.HasData;
				_retryOnTimeoutOr5xxError = true;
				break;
			case ProcessAnalyticsEventsResponse.None:
			case ProcessAnalyticsEventsResponse.RequestError:
				break;
			}
			if (flag)
			{
				FlushInternal();
			}
			else
			{
				_inFlush = false;
			}
		}

		public static long GetMsTicksSinceDeviceStartup()
		{
			long num = (uint)Environment.TickCount;
			if (num < _lastTick)
			{
				_tickOffset += 4294967295L;
			}
			_lastTick = num;
			return _tickOffset + num;
		}
	}
}
