using Data.Save;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Plugins.Notifications;
using UnityEngine;

public class RemoteNotificationControl
{
	public delegate void RemoteNotificationsEnabledHandler(string token, string oldToken);

	private const string CACHED_DEVICE_TOKEN_KEY = "remote_notifications_cached_device_token";

	private static bool _cachedDeviceTokenLoaded;

	private static string _cachedDeviceToken;

	public static void Disable()
	{
		PreferencesSave.remoteNotificationsEnabled = false;
	}

	public static void Enable(RemoteNotificationsEnabledHandler completionHandler, string soundToPlay)
	{
		if (Globals.DEBUG_DISABLE_NOTIFICATION_REQUEST)
		{
			return;
		}
		PreferencesSave.remoteNotificationsEnabled = true;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (completionHandler != null)
			{
				completionHandler(GetDeviceToken(), null);
			}
		}
		else
		{
			if (Application.platform != RuntimePlatform.Android || DeviceUtility.isKindleDevice)
			{
				return;
			}
			string registrationId = Notifications.instance.registrationId;
			if (string.IsNullOrEmpty(registrationId))
			{
				Notifications.instance.RegisterForRemoteNotifications((string deviceToken) =>
				{
					string oldToken2 = CacheTokenAndGetOldIfChanged(deviceToken);
					completionHandler(deviceToken, oldToken2);
				});
			}
			else if (completionHandler != null)
			{
				string oldToken = CacheTokenAndGetOldIfChanged(registrationId);
				completionHandler(registrationId, oldToken);
			}
		}
	}

	private static string CacheTokenAndGetOldIfChanged(string token)
	{
		if (!_cachedDeviceTokenLoaded)
		{
			_cachedDeviceToken = PlayerPrefs.GetString("remote_notifications_cached_device_token", null);
			_cachedDeviceTokenLoaded = true;
		}
		if (token != _cachedDeviceToken)
		{
			string cachedDeviceToken = _cachedDeviceToken;
			if (token == null)
			{
				PlayerPrefs.DeleteKey("remote_notifications_cached_device_token");
			}
			else
			{
				PlayerPrefs.SetString("remote_notifications_cached_device_token", token);
			}
			_cachedDeviceToken = token;
			return cachedDeviceToken;
		}
		return null;
	}

	public static string GetDeviceToken()
	{
		if (PreferencesSave.remoteNotificationsEnabled && Application.platform != RuntimePlatform.IPhonePlayer && Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice)
		{
			return Notifications.instance.registrationId;
		}
		return string.Empty;
	}

	private static string ByteArrayToHex(byte[] barray)
	{
		char[] array = new char[barray.Length * 2];
		for (int i = 0; i < barray.Length; i++)
		{
			byte b = (byte)(barray[i] >> 4);
			array[i * 2] = (char)((b <= 9) ? (b + 48) : (b + 55));
			b = (byte)(barray[i] & 0xF);
			array[i * 2 + 1] = (char)((b <= 9) ? (b + 48) : (b + 55));
		}
		return new string(array);
	}
}
