using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common.Plugins.Notifications.Internal
{
	internal class NotificationsMessageHelper : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> notificationReceived__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> registerForRemoteNotificationsCompleted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<string> unregisterForRemoteNotificationsCompleted__BackingField;

		public event Action<string> notificationReceived
		{
			add
			{
				Action<string> action = notificationReceived__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref notificationReceived__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = notificationReceived__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref notificationReceived__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> registerForRemoteNotificationsCompleted
		{
			add
			{
				Action<string> action = registerForRemoteNotificationsCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref registerForRemoteNotificationsCompleted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = registerForRemoteNotificationsCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref registerForRemoteNotificationsCompleted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<string> unregisterForRemoteNotificationsCompleted
		{
			add
			{
				Action<string> action = unregisterForRemoteNotificationsCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref unregisterForRemoteNotificationsCompleted__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string> action = unregisterForRemoteNotificationsCompleted__BackingField;
				Action<string> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref unregisterForRemoteNotificationsCompleted__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		[UsedImplicitly]
		private void OnNotificationReceived(string payload)
		{
			Action<string> action = notificationReceived__BackingField;
			if (action != null)
			{
				action(payload);
			}
		}

		[UsedImplicitly]
		private void OnRegisterForRemoteNotificationsCompleted(string regid)
		{
			Action<string> action = registerForRemoteNotificationsCompleted__BackingField;
			if (action != null)
			{
				action(regid);
			}
		}

		[UsedImplicitly]
		private void OnUnregisterForRemoteNotificationsCompleted(string success)
		{
			Action<string> action = unregisterForRemoteNotificationsCompleted__BackingField;
			if (action != null)
			{
				action(success);
			}
		}
	}
}
