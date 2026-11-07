#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Plugins.Notifications;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NotificationsObserver : MonoBehaviour
{
	public delegate void NotificationHandler(Notification notification);

	private static readonly IJsonSerializer _jsonConverter = new KilooSerializer();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private NotificationHandler onNotificationReceived__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<Notification> onNotificationPressed__BackingField;

	private const string TITLE_DICT_KEY = "title";

	private const string MESSAGE_DICT_KEY = "message";

	private const string TYPE_DICT_KEY = "type";

	private const string SOUNDNAME_DICT_KEY = "soundName";

	private const string USERINFO_DICT_KEY = "userInfo";

	private static NotificationsObserver _instance = null;

	public static NotificationsObserver instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("NotificationsObserver");
				gameObject.AddComponent<NotificationsObserver>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}
	}

	public event NotificationHandler onNotificationReceived
	{
		add
		{
			NotificationHandler notificationHandler = onNotificationReceived__BackingField;
			NotificationHandler notificationHandler2;
			do
			{
				notificationHandler2 = notificationHandler;
				notificationHandler = Interlocked.CompareExchange(ref onNotificationReceived__BackingField, (NotificationHandler)Delegate.Combine(notificationHandler2, value), notificationHandler);
			}
			while ((object)notificationHandler != notificationHandler2);
		}
		remove
		{
			NotificationHandler notificationHandler = onNotificationReceived__BackingField;
			NotificationHandler notificationHandler2;
			do
			{
				notificationHandler2 = notificationHandler;
				notificationHandler = Interlocked.CompareExchange(ref onNotificationReceived__BackingField, (NotificationHandler)Delegate.Remove(notificationHandler2, value), notificationHandler);
			}
			while ((object)notificationHandler != notificationHandler2);
		}
	}

	public event Action<Notification> onNotificationPressed
	{
		add
		{
			Action<Notification> action = onNotificationPressed__BackingField;
			Action<Notification> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onNotificationPressed__BackingField, (Action<Notification>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<Notification> action = onNotificationPressed__BackingField;
			Action<Notification> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onNotificationPressed__BackingField, (Action<Notification>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			return;
		}
		LogError("There is more than one NotificationsObserver in the scene!");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	[UsedImplicitly]
	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			return;
		}
		Notification notification = null;
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			if (DidAppStartFromNotificationAndroid(out notification))
			{
				onNotificationPressed__BackingField.SafeInvoke(notification);
			}
			break;
		case RuntimePlatform.IPhonePlayer:
			if (DidAppStartFromNotificationIOS(out notification))
			{
				onNotificationPressed__BackingField.SafeInvoke(notification);
			}
			break;
		}
	}

	private bool DidAppStartFromNotificationAndroid(out Notification notification)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.kiloo.unityutilities.UnityPluginActivity");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getCurrent", new object[0]);
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getIntent", new object[0]);
		if (androidJavaObject2.Call<bool>("hasExtra", new object[1] { "message" }))
		{
			AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getExtras", new object[0]);
			string message = androidJavaObject3.Call<string>("getString", new object[1] { "message" });
			string json = androidJavaObject3.Call<string>("getString", new object[1] { "userInfo" });
			Dictionary<string, object> dictionary = _jsonConverter.FromJson<Dictionary<string, object>>(json);
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> item in dictionary)
				{
				}
			}
			notification = new Notification
			{
				message = message,
				userInfo = dictionary
			};
			return true;
		}
		notification = null;
		return false;
	}

	private bool DidAppStartFromNotificationIOS(out Notification notification)
	{
		notification = null;
		return false;
	}

	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice)
		{
			Notifications.instance.notificationReceived += onAndroidNotificationReceived;
		}
	}

	private void onAndroidNotificationReceived()
	{
		Debug.Log("Notification received");
		foreach (INotification item in Notifications.instance)
		{
			if (item != null)
			{
				string title = item["title"] as string;
				string message = item["message"] as string;
				string type = item["type"] as string;
				Dictionary<string, object> userInfo = item["userInfo"] as Dictionary<string, object>;
				alertListeners(title, message, type, userInfo, string.Empty);
			}
		}
	}

	private void alertListeners(string title, string message, string type, Dictionary<string, object> userInfo, string soundName = "")
	{
		Notification notification = new Notification();
		notification.type = type;
		notification.message = message;
		if (userInfo != null)
		{
			notification.userInfo = userInfo;
		}
		NotificationHandler notificationHandler = onNotificationReceived__BackingField;
		if (notificationHandler != null)
		{
			notificationHandler(notification);
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_NOTIFICATIONSOBSERVER_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
