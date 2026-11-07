using System;
using System.Diagnostics;
using Kiloo.Common.Plugins.DeviceUtility.Internal;
using UnityEngine;

namespace Kiloo.Common.Plugins.DeviceUtility
{
	public static class DeviceUtility
	{
		private static IDeviceUtilityPlatformBridge _platformBridge;

		public static bool hasPhysicalBackButton
		{
			get
			{
				if (!CheckInstance())
				{
					return false;
				}
				return _platformBridge.hasPhysicalBackButton;
			}
		}

		public static bool isKindleDevice
		{
			get
			{
				if (!CheckInstance())
				{
					return false;
				}
				return _platformBridge.isKindleDevice;
			}
		}

		private static void LogError(string msg)
		{
			SDebug.LogError("[DeviceUtility] " + msg);
		}

		private static void LogWarning(string msg)
		{
			if (!Globals.DEBUG_DEVICE_UTILITY_WARNINGS)
			{
			}
		}

		[Conditional("DEVICE_UTILITY_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}

		private static bool CheckInstance()
		{
			if (_platformBridge == null)
			{
				Init();
			}
			return _platformBridge != null;
		}

		public static void Init()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				_platformBridge = DeviceUtilityAndroid.CreatePlugin();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsEditor:
				_platformBridge = DeviceUtilitySimulated.CreatePlugin();
				break;
			default:
				LogError("There's no platform bridge for the current platform.");
				return;
			}
			_platformBridge.SetDeviceUtilityDebugEnabled(false);
		}

		public static bool IsSupportedOnPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
				return true;
			default:
				return false;
			}
		}

		public static void SetKeyboardReturnType(int keyboardReturnType)
		{
			if (CheckInstance())
			{
				_platformBridge.SetKeyboardReturnType(keyboardReturnType);
			}
		}

		public static string GetUserDataPath()
		{
			if (!CheckInstance())
			{
				throw new InvalidOperationException("Could not get user data path");
			}
			return _platformBridge.GetUserDataPath();
		}

		public static string GetCachePath()
		{
			if (!CheckInstance())
			{
				throw new InvalidOperationException("Could not get user data path");
			}
			return _platformBridge.GetCachePath();
		}

		public static string GetDeviceIdentifier()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetDeviceIdentifier();
		}

		public static string GetBundleVersion()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetBundleVersion();
		}

		public static string GetBundleIdentifier()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetBundleIdentifier();
		}

		public static string GetLocale()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetLocale();
		}

		public static string GetUnfilteredLocale()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetUnfilteredLocale();
		}

		public static string GetOSVersion()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetOSVersion();
		}

		public static string GetOSIdentifier()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetOSIdentifier();
		}

		public static string GetBuildIdentifier()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetBuildIdentifier();
		}

		public static void ShowNativePopup(string title, string message, string dismissButton, Action<string> onDismissed = null)
		{
			if (CheckInstance())
			{
				ShowNativePopup(title, message, dismissButton, null, null, onDismissed);
			}
		}

		public static void ShowNativePopup(string title, string message, string dismissButton, string optionalButton1, Action<string> onDismissed = null)
		{
			if (CheckInstance())
			{
				ShowNativePopup(title, message, dismissButton, optionalButton1, null, onDismissed);
			}
		}

		public static void ShowNativePopup(string title, string message, string dismissButton, string optionalButton1, string optionalButton2, Action<string> onDismissed = null)
		{
			if (CheckInstance())
			{
				_platformBridge.ShowNativePopup(title, message, dismissButton, optionalButton1, optionalButton2, onDismissed);
			}
		}

		public static void SetDeviceClipBoard(string text)
		{
			if (CheckInstance())
			{
				_platformBridge.SetDeviceClipBoard(text);
			}
		}

		public static int GetDeviceFreeMemory()
		{
			if (!CheckInstance())
			{
				return 0;
			}
			return _platformBridge.GetDeviceFreeMemory();
		}

		public static int GetCurrentMemoryUsage()
		{
			if (!CheckInstance())
			{
				return 0;
			}
			return _platformBridge.GetCurrentMemoryUsage();
		}

		public static int GetPeakMemoryUsage()
		{
			if (!CheckInstance())
			{
				return 0;
			}
			return _platformBridge.GetPeakMemoryUsage();
		}

		public static int GetMemoryUsageLimit()
		{
			if (!CheckInstance())
			{
				return 0;
			}
			return _platformBridge.GetMemoryUsageLimit();
		}

		public static int GetBatteryLevel()
		{
			if (!CheckInstance())
			{
				return -1;
			}
			return _platformBridge.GetBatteryLevel();
		}

		public static float GetBatteryTemperature()
		{
			if (!CheckInstance())
			{
				return -1f;
			}
			return _platformBridge.GetBatteryTemperature();
		}

		public static int GetFreeDiskSpace()
		{
			if (!CheckInstance())
			{
				return -1;
			}
			return _platformBridge.GetFreeDiskSpace();
		}

		public static int GetTotalDiskSpace()
		{
			if (!CheckInstance())
			{
				return -1;
			}
			return _platformBridge.GetTotalDiskSpace();
		}

		public static int GetFreeSDCardSpace()
		{
			if (!CheckInstance())
			{
				return -1;
			}
			return _platformBridge.GetFreeSDCardSpace();
		}

		public static int GetTotalSDCardSpace()
		{
			if (!CheckInstance())
			{
				return -1;
			}
			return _platformBridge.GetTotalSDCardSpace();
		}

		public static bool CanWeSendUserToAppSettings()
		{
			if (!CheckInstance())
			{
				return false;
			}
			return _platformBridge.CanWeSendUserToAppSettings();
		}

		public static void SendUserToAppSettings()
		{
			if (CheckInstance())
			{
				_platformBridge.SendUserToAppSettings();
			}
		}

		public static bool IsTablet()
		{
			if (!CheckInstance())
			{
				return false;
			}
			return _platformBridge.IsTablet();
		}

		public static string GetAndroidExternalSavePath()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetAndroidExternalSavePath();
		}

		public static int GetAndroidApiLevel()
		{
			if (!CheckInstance())
			{
				return -1;
			}
			return _platformBridge.GetAndroidApiLevel();
		}

		public static int GetAndroidVersionCode()
		{
			if (!CheckInstance())
			{
				return -1;
			}
			return _platformBridge.GetAndroidVersionCode();
		}

		public static int GetAndroidTopCutout()
		{
			if (!CheckInstance())
			{
				return 0;
			}
			return _platformBridge.GetAndroidTopCutout();
		}

		public static int GetAndroidBottomCutout()
		{
			if (!CheckInstance())
			{
				return 0;
			}
			return _platformBridge.GetAndroidBottomCutout();
		}

		public static bool GetIOSOtherAudioPlaying()
		{
			if (!CheckInstance())
			{
				return false;
			}
			return _platformBridge.GetIOSOtherAudioPlaying();
		}

		public static string GetUserLibraryPathiOS()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetUserLibraryPathiOS();
		}

		public static string GetUserDocumentsPathiOS()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetUserDocumentsPathiOS();
		}

		public static string GetUserTempPathiOS()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetUserTempPathiOS();
		}

		public static string GetiOSBuildNumber()
		{
			if (!CheckInstance())
			{
				return string.Empty;
			}
			return _platformBridge.GetiOSBuildNumber();
		}

		public static void MakeIOSNSLog(string message)
		{
			if (CheckInstance())
			{
				_platformBridge.MakeIOSNSLog(message);
			}
		}

		public static void DismissKeyboard()
		{
			if (CheckInstance())
			{
				_platformBridge.DismissKeyboard();
			}
		}
	}
}
