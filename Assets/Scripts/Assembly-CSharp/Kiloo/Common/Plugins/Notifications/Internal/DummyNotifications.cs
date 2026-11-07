using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Plugins.Notifications.Internal
{
	public class DummyNotifications : INotifications, IEnumerable<INotification>, IEnumerable
	{
		private bool _enabledLocalNotificationLogs;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action notificationReceived__BackingField;

		public int count
		{
			get
			{
				return 0;
			}
		}

		public INotification this[int index]
		{
			get
			{
				throw new IndexOutOfRangeException();
			}
		}

		public bool areNotificationsEnabled
		{
			get
			{
				return Globals.DEBUG_ARE_NOTIFICATIONS_ENABLED;
			}
		}

		public string registrationId
		{
			get
			{
				return string.Empty;
			}
		}

		public string oldRegistrationId
		{
			get
			{
				return string.Empty;
			}
		}

		public string errorMessage
		{
			get
			{
				return "Remote notifications not supported on this platform";
			}
		}

		public bool isErrorUserRecoverable
		{
			get
			{
				return false;
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

		public void ClearNotifications()
		{
		}

		public void ScheduleLocalNotification(DateTime date, string title, string message, string soundName)
		{
			if (_enabledLocalNotificationLogs)
			{
				Debug.Log(string.Concat("Local notification was scheduled for ", date, " with message: ", message));
			}
		}

		public void ClearAllLocalNotifications()
		{
			if (_enabledLocalNotificationLogs)
			{
				Debug.Log("Cancelled all local notifications");
			}
		}

		public void EnableLocalNotificationLogs()
		{
			_enabledLocalNotificationLogs = true;
		}

		public Coroutine RegisterForRemoteNotifications(RegisterForRemoteNotificationsCompletedHandler onComplete = null)
		{
			if (onComplete != null)
			{
				onComplete(null);
			}
			return null;
		}

		public Coroutine UnregisterForRemoteNotifications(UnregisterForRemoteNotificationsCompletedHandler onComplete = null)
		{
			if (onComplete != null)
			{
				onComplete(false);
			}
			return null;
		}

		public void HandleUserRecoverableError()
		{
		}

		public IEnumerator<INotification> GetEnumerator()
		{
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected virtual void OnNotificationReceived()
		{
			Action action = notificationReceived__BackingField;
			if (action != null)
			{
				action();
			}
		}
	}
}
