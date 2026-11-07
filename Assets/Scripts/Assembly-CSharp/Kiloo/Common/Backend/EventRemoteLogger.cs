using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.HTTP;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using SYBO.Components.Json;
using UnityEngine;

namespace Kiloo.Common.Backend
{
	public class EventRemoteLogger : IEventRemoteLogger, IEventLogger
	{
		private class Event
		{
			public string Name;

			public JsonDict Stats;

			public DateTime TimeStamp;
		}

		private class Session
		{
			public Guid SessionId;

			[UsedImplicitly]
			public string DeviceId;

			public DateTime StopTimeStamp;

			public List<Event> Events;

			public DateTime StartTimeStamp;

			public int Aborted;

			public Session()
			{
				Events = new List<Event>();
			}
		}

		private struct SessionFileInfo
		{
			public string File;

			public Guid SessionId;
		}

		[NotNull]
		public static readonly EventRemoteLogger instance = new EventRemoteLogger();

		public bool isPartOfSample;

		public bool allowSending;

		public bool allowSaving;

		private const string FOLDER = "event_logs";

		private const string API_POST_EVENTS_ROUTE = "api/v2/log";

		private const int MAX_UNSENT_FILES = 25;

		private readonly TimeSpan _dataExpireTime = TimeSpan.FromDays(28.0);

		private Session _session;

		private byte[] _lastSavedData;

		private string _lastSavedFilename;

		private bool _didAddApplicationStateHandlers;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onPauseOrQuit__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onResume__BackingField;

		private Uri _loggingServiceUri;

		private string _dataFolder;

		private int _maximumSessionPauseInSeconds = 600;

		private bool _sendOnPause;

		private float _sendOnPauseMaximumSendTime;

		private float _sendOnPauseRequestTimeWindow;

		public bool isSending { get; private set; }

		public event Action onPauseOrQuit
		{
			add
			{
				Action action = onPauseOrQuit__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onPauseOrQuit__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onPauseOrQuit__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onPauseOrQuit__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onResume
		{
			add
			{
				Action action = onResume__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onResume__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onResume__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onResume__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private EventRemoteLogger()
		{
			string userDataPath = DeviceUtility.GetUserDataPath();
			if (!string.IsNullOrEmpty(userDataPath))
			{
				_dataFolder = Path.Combine(userDataPath, "event_logs");
				if (_dataFolder != null && !Directory.Exists(_dataFolder))
				{
					Directory.CreateDirectory(_dataFolder);
				}
			}
		}

		public void SetServiceInfo(Uri baseUri, int maximumSessionPauseInSeconds = 600)
		{
			_loggingServiceUri = new Uri(baseUri, "api/v2/log");
			_maximumSessionPauseInSeconds = maximumSessionPauseInSeconds;
		}

		public void CheckSessionRemoval()
		{
			if (isPartOfSample)
			{
				StartSession();
			}
			else
			{
				Save();
			}
		}

		private string GetSessionJSON()
		{
			if (_session == null)
			{
				throw new InvalidOperationException("EventLogger.GetSessionJSON: Session not started.");
			}
			JsonContext jsonContext = new JsonContext();
			jsonContext.RemoveWhitespace = false;
			JsonContext context = jsonContext;
			return JsonApi.Serialize(_session, context);
		}

		public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("name parameter cannot be null.");
			}
			if (_session == null)
			{
				throw new InvalidOperationException("EventLogger.LogEvent: Session not started.");
			}
			JsonDict stats = ConvertFromDictionary(parameters);
			Event obj = new Event();
			obj.TimeStamp = DateTime.UtcNow;
			obj.Name = eventName;
			obj.Stats = stats;
			Event item = obj;
			_session.Events.Add(item);
		}

		private JsonDict ConvertFromDictionary(Dictionary<string, object> dict)
		{
			JsonDict jsonDict = new JsonDict();
			if (dict != null)
			{
				foreach (KeyValuePair<string, object> item in dict)
				{
					jsonDict[item.Key] = item.Value;
				}
			}
			return jsonDict;
		}

		public void Send(float maximumSendTime, float requestTimeWindow)
		{
			if (!allowSending)
			{
				return;
			}
			if (_loggingServiceUri == null)
			{
				throw new InvalidOperationException("EventLogger.Send: ServiceUri not initialized.");
			}
			if (_dataFolder == null || isSending)
			{
				return;
			}
			isSending = true;
			DateTime sendStartTime = DateTime.UtcNow;
			byte[] lastSavedData = _lastSavedData;
			string lastSavedFilename = _lastSavedFilename;
			string[] files = Directory.GetFiles(_dataFolder, "*");
			Array.Sort(files);
			string bundleVersion = DeviceUtility.GetBundleVersion();
			if (string.IsNullOrEmpty(bundleVersion))
			{
				bundleVersion = "0.0";
			}
			if (Globals.DEBUG)
			{
				bundleVersion += ".0";
			}
			bool flag = false;
			try
			{
				flag = ThreadPool.QueueUserWorkItem(delegate
				{
					string[] array = files;
					foreach (string path in array)
					{
						bool flag2 = false;
						string fileName = Path.GetFileName(path);
						DateTime result;
						if (TimeUtility.TryParseSortableNumber(fileName, out result))
						{
							if (maximumSendTime > 0f)
							{
								double num2 = (double)maximumSendTime - (DateTime.UtcNow - sendStartTime).TotalSeconds;
								if (num2 < (double)requestTimeWindow)
								{
									break;
								}
							}
							if (!isSending)
							{
								break;
							}
							try
							{
								byte[] array2 = ((!(fileName == lastSavedFilename)) ? FileUtil.Load(path) : lastSavedData);
								if (array2 == null)
								{
									throw new Exception("Request content should not be null");
								}
								IRequest request = KilooWebClient.CreatePost(_loggingServiceUri).WithContent(array2, "application/json").WithHeader("Kiloo-App-Version", bundleVersion)
									.ThenMakeTheRequest();
								if (request.isSuccessful)
								{
									flag2 = true;
								}
								else
								{
									IResponse response = request.response;
									if (response != null)
									{
										string value = response.headers.Get("Retry-After");
										if (string.IsNullOrEmpty(value))
										{
											switch (request.response.status)
											{
											case 400:
											case 401:
											case 402:
											case 403:
											case 404:
											case 405:
											case 406:
											case 407:
											case 409:
											case 410:
											case 412:
											case 413:
											case 414:
											case 415:
											case 501:
											case 505:
												flag2 = true;
												break;
											default:
												isSending = false;
												break;
											}
										}
									}
									else
									{
										isSending = false;
									}
								}
							}
							catch (Exception)
							{
								flag2 = true;
							}
							if (flag2)
							{
								File.Delete(path);
							}
						}
					}
					isSending = false;
				});
			}
			catch (Exception)
			{
			}
			if (!flag)
			{
				isSending = false;
			}
			if (!(maximumSendTime > 0f))
			{
				return;
			}
			while (isSending)
			{
				Utils.ThreadSleep(TimeSpan.FromMilliseconds(5.0));
				double num = (double)maximumSendTime - (DateTime.UtcNow - sendStartTime).TotalSeconds;
				if (num <= 0.0)
				{
					isSending = false;
					break;
				}
			}
		}

		public Coroutine SendAsync(Action onComplete = null)
		{
			return CoroutineUtil.StartCoroutine(SendCoroutine(onComplete));
		}

		public void SendOnPause(bool enable, float maximumSendTime = 3f, float requestTimeWindow = 2f)
		{
			_sendOnPause = enable;
			_sendOnPauseMaximumSendTime = maximumSendTime;
			_sendOnPauseRequestTimeWindow = requestTimeWindow;
		}

		private void Save()
		{
			if (_session != null && _dataFolder != null)
			{
				DateTime utcNow = DateTime.UtcNow;
				_session.StopTimeStamp = utcNow;
				_session.Aborted = Convert.ToInt32(!isPartOfSample);
				if (allowSaving)
				{
					string sessionJSON = GetSessionJSON();
					byte[] bytes = Encoding.UTF8.GetBytes(sessionJSON);
					string text = utcNow.ToSortableNumber();
					string path = Path.Combine(_dataFolder, text);
					FileUtil.Save(path, bytes);
					_lastSavedData = bytes;
					_lastSavedFilename = text;
				}
				_session.Events.Clear();
				if (!isPartOfSample)
				{
					_session = null;
				}
			}
		}

		private void StartSession()
		{
			bool flag = true;
			if (_session != null)
			{
				if (_session.StopTimeStamp != DateTime.MinValue)
				{
					DateTime stopTimeStamp = _session.StopTimeStamp;
					_session.StopTimeStamp = DateTime.MinValue;
					if ((DateTime.UtcNow - stopTimeStamp).TotalSeconds < (double)_maximumSessionPauseInSeconds)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				_session = new Session();
			}
			if (flag)
			{
				_session.SessionId = Guid.NewGuid();
				_session.DeviceId = AnalyticsSave.flurryUserId;
				_session.StartTimeStamp = DateTime.UtcNow;
			}
			if (!_didAddApplicationStateHandlers)
			{
				ApplicationState.quit += OnApplicationQuit;
				ApplicationState.paused += OnApplicationPause;
				ApplicationState.resumed += OnApplicationResume;
				_didAddApplicationStateHandlers = true;
			}
		}

		private void CleanAndSend()
		{
			CleanupOldUnsentFiles();
			SendAsync();
		}

		private void OnApplicationQuit()
		{
			ApplicationState.quit -= OnApplicationQuit;
			ApplicationState.paused -= OnApplicationPause;
			ApplicationState.resumed -= OnApplicationResume;
			_didAddApplicationStateHandlers = false;
			if (isPartOfSample)
			{
				onPauseOrQuit__BackingField.SafeInvoke();
				Save();
			}
		}

		private void OnApplicationResume()
		{
			if (isPartOfSample)
			{
				CleanAndSend();
				StartSession();
				onResume__BackingField.SafeInvoke();
			}
		}

		private void OnApplicationPause()
		{
			if (isPartOfSample)
			{
				onPauseOrQuit__BackingField.SafeInvoke();
				Save();
			}
			if (_sendOnPause)
			{
				Send(_sendOnPauseMaximumSendTime, _sendOnPauseRequestTimeWindow);
			}
		}

		private IEnumerator SendCoroutine(Action onComplete)
		{
			if (!isSending)
			{
				Send(0f, 0f);
			}
			while (isSending)
			{
				yield return null;
			}
			if (onComplete != null)
			{
				onComplete();
			}
		}

		private void CleanupOldUnsentFiles()
		{
			if (_dataFolder == null || isSending)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			string[] files = Directory.GetFiles(_dataFolder, "*");
			Array.Sort(files);
			List<SessionFileInfo> list = new List<SessionFileInfo>();
			Guid guid = Guid.Empty;
			string[] array = files;
			foreach (string text in array)
			{
				string fileName = Path.GetFileName(text);
				DateTime result;
				if (!TimeUtility.TryParseSortableNumber(fileName, out result))
				{
					continue;
				}
				try
				{
					Guid sessionId;
					try
					{
						byte[] array2 = FileUtil.Load(text);
						string json = Encoding.UTF8.GetString(array2, 0, array2.Length);
						Session session = JsonApi.Deserialize<Session>(json);
						sessionId = session.SessionId;
					}
					catch (Exception)
					{
						File.Delete(text);
						goto end_IL_006f;
					}
					if (guid == sessionId)
					{
						File.Delete(text);
					}
					else if (utcNow - result > _dataExpireTime)
					{
						File.Delete(text);
						guid = sessionId;
					}
					else
					{
						list.Add(new SessionFileInfo
						{
							File = text,
							SessionId = sessionId
						});
					}
					end_IL_006f:;
				}
				catch (Exception)
				{
				}
			}
			if (list.Count <= 25)
			{
				return;
			}
			while (list.Count > 25)
			{
				Guid sessionId2 = list[0].SessionId;
				foreach (SessionFileInfo item in list)
				{
					if (sessionId2 == item.SessionId)
					{
						try
						{
							File.Delete(item.File);
						}
						catch (Exception)
						{
						}
					}
				}
				int num = list.RemoveAll((SessionFileInfo fileInfo) => fileInfo.SessionId == sessionId2);
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		[Conditional("STATISTICS_DEBUG_LOGS")]
		private void LogError(string msg)
		{
			SDebug.LogError("[EventLogger] " + msg);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		[Conditional("STATISTICS_DEBUG_LOGS")]
		private void LogWarning(string msg)
		{
		}

		[Conditional("BACKEND_DEBUG_LOGS")]
		[Conditional("STATISTICS_DEBUG_LOGS")]
		private void Log(string msg)
		{
		}
	}
}
