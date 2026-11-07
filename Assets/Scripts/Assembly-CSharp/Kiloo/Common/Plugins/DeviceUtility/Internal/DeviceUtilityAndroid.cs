using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.Plugins.DeviceUtility.Internal
{
	internal class DeviceUtilityAndroid : MonoBehaviour, IDeviceUtilityPlatformBridge
	{
		private const char PARAMETER_SEPARATOR = ';';

		private const string OS_IDENTIFIER = "android";

		private AndroidJavaObject _androidBridge;

		private const int ANDROID_BUTTON_POSITIVE = -1;

		private const int ANDROID_BUTTON_NEGATIVE = -2;

		private const int ANDROID_BUTTON_NEUTRAL = -3;

		private const string CALLBACK_RECEIVER_NAME = "DeviceUtility Receiver";

		public bool hasPhysicalBackButton
		{
			get
			{
				return true;
			}
		}

		public bool isKindleDevice
		{
			get
			{
				return false;
			}
		}

		private static void LogError(string msg)
		{
			SDebug.LogError("[DeviceUtilityAndroid] " + msg);
		}

		private static void LogWarning(string msg)
		{
		}

		[Conditional("DEVICE_UTILITY_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}

		[UsedImplicitly]
		private void Awake()
		{
			_androidBridge = new AndroidJavaObject("com.kiloo.unityplugin.deviceutility.DeviceUtilityBridgeAndroid");
		}

		[UsedImplicitly]
		private void _callback_alertViewDidDismissWithButtonIndex(string message)
		{
			try
			{
				string[] array = message.Split(';');
				if (array.Length == 2)
				{
					string text = array[0].Trim();
					string text2 = array[1].Trim(' ', '-');
					char[] array2 = text.ToCharArray();
					char[] array3 = text2.ToCharArray();
					if (array2.Length == 1 && array3.Length == 1)
					{
						char c = array2[0];
						char c2 = array3[0];
						double numericValue = char.GetNumericValue(c);
						double numericValue2 = char.GetNumericValue(c2);
						int ticket = (int)numericValue;
						int num = (int)numericValue2;
						Action<int> action = Depository.CheckOut(ticket) as Action<int>;
						if (action != null)
						{
							action(num * -1);
						}
					}
					return;
				}
			}
			catch (Exception)
			{
			}
			LogError("Malformed dialog callback message");
		}

		public bool CanWeSendUserToAppSettings()
		{
			return true;
		}

		public void SendUserToAppSettings()
		{
			_androidBridge.Call("sendUserToAppSettings");
		}

		public bool IsTablet()
		{
			return _androidBridge.Call<bool>("isTablet", new object[0]);
		}

		public string GetBuildIdentifier()
		{
			return (!isKindleDevice) ? GetOSIdentifier() : "kindle";
		}

		public void SetKeyboardReturnType(int returnKeyType)
		{
		}

		public void SetDeviceUtilityDebugEnabled(bool val)
		{
			_androidBridge.Set("debugLogEnabled", val);
		}

		public string GetUserDataPath()
		{
			string text = null;
			text = _androidBridge.Call<string>("getUserDataPath", new object[0]);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("Could not get user data path");
			}
			return text;
		}

		public string GetCachePath()
		{
			string text = null;
			text = _androidBridge.Call<string>("getCachePath", new object[0]);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("Could not get cache path");
			}
			return text;
		}

		public string GetDeviceIdentifier()
		{
			return _androidBridge.Call<string>("getDeviceIdentifier", new object[0]);
		}

		public string GetBundleVersion()
		{
			return _androidBridge.Call<string>("getBundleVersion", new object[0]);
		}

		public string GetBundleIdentifier()
		{
			return _androidBridge.Call<string>("getBundleIdentifier", new object[0]);
		}

		public string GetLocale()
		{
			return _androidBridge.Call<string>("getLocale", new object[0]);
		}

		public string GetUnfilteredLocale()
		{
			return _androidBridge.Call<string>("getUnfilteredLocale", new object[0]);
		}

		public string GetOSVersion()
		{
			return _androidBridge.Call<string>("getOSVersion", new object[0]);
		}

		public string GetOSIdentifier()
		{
			return "android";
		}

		public void ShowNativePopup(string title, string message, string cancelButton, string optionalButton1, string optionalButton2, Action<string> onDismissed)
		{
			int num = 0;
			if (onDismissed != null)
			{
				Action<int> deposit = (int buttonPressedIndex) =>
				{
					switch (buttonPressedIndex)
					{
					case -1:
						onDismissed(optionalButton1);
						break;
					case -3:
						onDismissed(optionalButton2);
						break;
					default:
						onDismissed(cancelButton);
						break;
					}
				};
				num = Depository.CheckIn(deposit, this);
			}
			_androidBridge.Call("showNativePopup", num, title, message, cancelButton, optionalButton1, optionalButton2);
		}

		public void SetDeviceClipBoard(string text)
		{
			_androidBridge.Call("setDeviceClipBoard", text);
		}

		public int GetDeviceFreeMemory()
		{
			return _androidBridge.Call<int>("getDeviceFreeMemory", new object[0]);
		}

		public int GetCurrentMemoryUsage()
		{
			return _androidBridge.Call<int>("getDeviceCurrentMemoryUsage", new object[0]);
		}

		public int GetPeakMemoryUsage()
		{
			return 0;
		}

		public int GetMemoryUsageLimit()
		{
			return _androidBridge.Call<int>("getDeviceMemoryLimit", new object[0]);
		}

		public int GetBatteryLevel()
		{
			return _androidBridge.Call<int>("getBatteryLevel", new object[0]);
		}

		public float GetBatteryTemperature()
		{
			return _androidBridge.Call<float>("getBatteryTemperature", new object[0]);
		}

		public int GetFreeDiskSpace()
		{
			return _androidBridge.Call<int>("getFreeDiskSpace", new object[0]);
		}

		public int GetTotalDiskSpace()
		{
			return _androidBridge.Call<int>("getTotalDiskSpace", new object[0]);
		}

		public int GetFreeSDCardSpace()
		{
			return _androidBridge.Call<int>("getFreeSDCardSpace", new object[0]);
		}

		public int GetTotalSDCardSpace()
		{
			return _androidBridge.Call<int>("getTotalSDCardSpace", new object[0]);
		}

		public string GetAndroidExternalSavePath()
		{
			return _androidBridge.Call<string>("getAndroidExternalSavePath", new object[0]);
		}

		public int GetAndroidApiLevel()
		{
			return _androidBridge.Call<int>("getAndroidApiLevel", new object[0]);
		}

		public int GetAndroidVersionCode()
		{
			return _androidBridge.Call<int>("getAndroidVersionCode", new object[0]);
		}

		public int GetAndroidTopCutout()
		{
			return _androidBridge.Call<int>("getTopInset", new object[0]);
		}

		public int GetAndroidBottomCutout()
		{
			return _androidBridge.Call<int>("getBottomInset", new object[0]);
		}

		public bool GetIOSOtherAudioPlaying()
		{
			return false;
		}

		public string GetUserLibraryPathiOS()
		{
			return string.Empty;
		}

		public string GetUserDocumentsPathiOS()
		{
			return string.Empty;
		}

		public string GetUserTempPathiOS()
		{
			return string.Empty;
		}

		public string GetiOSBuildNumber()
		{
			return string.Empty;
		}

		public void MakeIOSNSLog(string message)
		{
		}

		public void DismissKeyboard()
		{
		}

		public static DeviceUtilityAndroid CreatePlugin()
		{
			GameObject gameObject = new GameObject("DeviceUtility Receiver");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<DeviceUtilityAndroid>();
		}
	}
}
