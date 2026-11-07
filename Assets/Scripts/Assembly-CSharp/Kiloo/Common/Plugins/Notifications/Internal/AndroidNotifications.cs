using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Utility;
using Kiloo.Plugins;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Plugins.Notifications.Internal
{
	internal class AndroidNotifications : INotifications, IDisposable, IEnumerable<INotification>, IEnumerable
	{
		private List<AndroidNotification> _notifications;

		private RegisterForRemoteNotificationsCompletedHandler _onRegisterForRemoteNotificationsCompleted;

		private UnregisterForRemoteNotificationsCompletedHandler _onUnregisterForRemoteNotificationsCompleted;

		private NotificationsMessageHelper _messageHelper;

		private AndroidJavaObject _notificationsJavaObject;

		private AndroidJavaObject _localNotificationManager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action notificationReceived__BackingField;

		public int count
		{
			get
			{
				return (_notifications != null) ? _notifications.Count : 0;
			}
		}

		public INotification this[int index]
		{
			get
			{
				if (_notifications != null)
				{
					return _notifications[index];
				}
				throw new IndexOutOfRangeException();
			}
		}

		public bool areNotificationsEnabled
		{
			get
			{
				return _notificationsJavaObject.Call<bool>("AreNotificationsEnabled", new object[0]);
			}
		}

		public string registrationId
		{
			get
			{
				return _notificationsJavaObject.Call<string>("getRegistrationId", new object[0]);
			}
		}

		public string oldRegistrationId
		{
			get
			{
				return _notificationsJavaObject.Call<string>("getOldRegistrationId", new object[0]);
			}
		}

		public string errorMessage
		{
			get
			{
				return _notificationsJavaObject.Call<string>("getErrorMessage", new object[0]);
			}
		}

		public bool isErrorUserRecoverable
		{
			get
			{
				return _notificationsJavaObject.Call<bool>("isErrorUserRecoverable", new object[0]);
			}
		}

		public event Action notificationReceived
		{
			add
			{
				Action action = notificationReceived__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref notificationReceived__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = notificationReceived__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref notificationReceived__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private AndroidNotifications()
		{
		}

		public static INotifications Create(bool receiveNotifications)
		{
			AndroidNotifications androidNotifications = new AndroidNotifications();
			androidNotifications.Init(receiveNotifications);
			return androidNotifications;
		}

		private void Init(bool receiveNotifications)
		{
			if (_notificationsJavaObject != null)
			{
				return;
			}
			if (_messageHelper == null)
			{
				_messageHelper = PluginUtil.CreateBridgeObject<NotificationsMessageHelper>();
				_messageHelper.registerForRemoteNotificationsCompleted += OnRegisterForRemoteNotificationsCompleted;
				_messageHelper.unregisterForRemoteNotificationsCompleted += OnUnregisterForRemoteNotificationsCompleted;
				if (receiveNotifications)
				{
					_messageHelper.notificationReceived += OnNotificationReceived;
				}
			}
			_notificationsJavaObject = new AndroidJavaObject("com.kiloo.unityplugins.notifications.NotificationsUnityPlugin");
			if (!_notificationsJavaObject.Call<bool>("init", new object[2] { _messageHelper.name, receiveNotifications }))
			{
				throw new PluginException("Current activity is not a UnityPluginActivity (java)");
			}
		}

		public void ClearNotifications()
		{
			_notifications = null;
		}

		public void ScheduleLocalNotification(DateTime date, string title, string message, string soundName)
		{
			LazyInitLocalNotifications();
			DateTime dateTime = new DateTime(1970, 1, 1);
			TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
			dateTime = dateTime.AddHours(currentTimeZone.GetUtcOffset(DateTime.Now).TotalHours);
			long num = (date.Ticks - dateTime.Ticks) / 10000;
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.Date", num);
			_localNotificationManager.Call("ScheduleLocalNotification", androidJavaObject, title, message, soundName);
		}

		private void LazyInitLocalNotifications()
		{
			if (_localNotificationManager == null)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kiloo.unityplugins.notifications.LocalNotificationsManager"))
				{
					_localNotificationManager = androidJavaClass.CallStatic<AndroidJavaObject>("GetInstance", new object[0]);
					_localNotificationManager.Call("InitNotificationChannel", "cpush.wav", Strings.Get("NOTIFICATION_ANDROID_CHANNEL_NAME"), Strings.Get("NOTIFICATION_ANDROID_CHANNEL_DESCRIPTION"));
				}
			}
		}

		public void ClearAllLocalNotifications()
		{
			LazyInitLocalNotifications();
			_localNotificationManager.Call("RemoveAllActiveNotifications");
		}

		public void EnableLocalNotificationLogs()
		{
			LazyInitLocalNotifications();
			_localNotificationManager.Call("ShowLog", true);
		}

		public Coroutine RegisterForRemoteNotifications(RegisterForRemoteNotificationsCompletedHandler onComplete = null)
		{
			return CoroutineUtil.StartCoroutine(RegisterForRemoteNotificationsCoroutine(onComplete));
		}

		private IEnumerator RegisterForRemoteNotificationsCoroutine(RegisterForRemoteNotificationsCompletedHandler onComplete)
		{
			if (_onRegisterForRemoteNotificationsCompleted != null || _onUnregisterForRemoteNotificationsCompleted != null)
			{
				throw new InvalidOperationException("Registration process already running.");
			}
			bool working = true;
			string regid = null;
			_onRegisterForRemoteNotificationsCompleted = (string id) =>
			{
				regid = id;
				working = false;
			};
			if (!_notificationsJavaObject.Call<bool>("registerForRemoteNotifications", new object[0]))
			{
				working = false;
			}
			while (working)
			{
				yield return null;
			}
			_onRegisterForRemoteNotificationsCompleted = null;
			if (onComplete != null)
			{
				onComplete(regid);
			}
		}

		public Coroutine UnregisterForRemoteNotifications(UnregisterForRemoteNotificationsCompletedHandler onComplete = null)
		{
			return CoroutineUtil.StartCoroutine(UnregisterForRemoteNotificationsCoroutine(onComplete));
		}

		private IEnumerator UnregisterForRemoteNotificationsCoroutine(UnregisterForRemoteNotificationsCompletedHandler onComplete)
		{
			if (_onRegisterForRemoteNotificationsCompleted != null || _onUnregisterForRemoteNotificationsCompleted != null)
			{
				throw new InvalidOperationException("Registration process already running.");
			}
			bool working = true;
			bool succeeded = false;
			_onUnregisterForRemoteNotificationsCompleted = (bool success) =>
			{
				succeeded = success;
				working = false;
			};
			if (!_notificationsJavaObject.Call<bool>("unregisterForRemoteNotifications", new object[0]))
			{
				working = false;
			}
			while (working)
			{
				yield return null;
			}
			_onUnregisterForRemoteNotificationsCompleted = null;
			if (onComplete != null)
			{
				onComplete(succeeded);
			}
		}

		public void HandleUserRecoverableError()
		{
			_notificationsJavaObject.Call("handleUserRecoverableError");
		}

		public void Dispose()
		{
			if (_messageHelper != null)
			{
				UnityEngine.Object.Destroy(_messageHelper.gameObject);
				_messageHelper = null;
			}
			if (_notificationsJavaObject != null)
			{
				_notificationsJavaObject.Call("dispose");
				_notificationsJavaObject.Dispose();
				_notificationsJavaObject = null;
			}
		}

		public IEnumerator<INotification> GetEnumerator()
		{
			foreach (AndroidNotification notification in _notifications)
			{
				yield return notification;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _notifications.GetEnumerator();
		}

		private void OnRegisterForRemoteNotificationsCompleted(string regid)
		{
			RegisterForRemoteNotificationsCompletedHandler onRegisterForRemoteNotificationsCompleted = _onRegisterForRemoteNotificationsCompleted;
			if (onRegisterForRemoteNotificationsCompleted != null)
			{
				_onRegisterForRemoteNotificationsCompleted = null;
				onRegisterForRemoteNotificationsCompleted(regid);
			}
		}

		private void OnUnregisterForRemoteNotificationsCompleted(string success)
		{
			UnregisterForRemoteNotificationsCompletedHandler onUnregisterForRemoteNotificationsCompleted = _onUnregisterForRemoteNotificationsCompleted;
			if (onUnregisterForRemoteNotificationsCompleted != null)
			{
				_onUnregisterForRemoteNotificationsCompleted = null;
				onUnregisterForRemoteNotificationsCompleted(success != "0");
			}
		}

		private void OnNotificationReceived(string payload)
		{
			if (_notifications == null)
			{
				_notifications = new List<AndroidNotification>(1);
			}
			_notifications.Add(new AndroidNotification(payload));
			Action action = notificationReceived__BackingField;
			if (action != null)
			{
				action();
			}
		}

		[Conditional("NOTIFICATIONS_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[AndroidNotifications] " + msg);
		}
	}
}
