using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Kiloo.Common.Plugins.Notifications
{
	public interface INotifications : IEnumerable<INotification>, IEnumerable
	{
		int count { get; }

		INotification this[int index] { get; }

		string registrationId { get; }

		string oldRegistrationId { get; }

		string errorMessage { get; }

		bool isErrorUserRecoverable { get; }

		bool areNotificationsEnabled { get; }

		event Action notificationReceived;

		Coroutine RegisterForRemoteNotifications([CanBeNull] RegisterForRemoteNotificationsCompletedHandler onComplete = null);

		Coroutine UnregisterForRemoteNotifications([CanBeNull] UnregisterForRemoteNotificationsCompletedHandler onComplete = null);

		void HandleUserRecoverableError();

		void ClearNotifications();

		void ScheduleLocalNotification(DateTime date, string title, string message, string soundName);

		void ClearAllLocalNotifications();

		void EnableLocalNotificationLogs();
	}
}
