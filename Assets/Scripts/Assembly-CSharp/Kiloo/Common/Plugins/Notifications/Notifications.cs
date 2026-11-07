using System;
using Kiloo.Common.Plugins.Notifications.Internal;

namespace Kiloo.Common.Plugins.Notifications
{
	public static class Notifications
	{
		public static INotifications instance { get; private set; }

		public static INotifications Init(bool receiveNotifications)
		{
			if (instance == null)
			{
				instance = (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WebGLPlayer) ? (INotifications)new DummyNotifications() : AndroidNotifications.Create(receiveNotifications);
				return instance;
			}
			throw new InvalidOperationException();
		}
	}
}
