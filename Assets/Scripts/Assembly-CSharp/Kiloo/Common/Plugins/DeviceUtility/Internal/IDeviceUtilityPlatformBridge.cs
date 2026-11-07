using System;

namespace Kiloo.Common.Plugins.DeviceUtility.Internal
{
	internal interface IDeviceUtilityPlatformBridge
	{
		bool hasPhysicalBackButton { get; }

		bool isKindleDevice { get; }

		void SetDeviceUtilityDebugEnabled(bool val);

		string GetUserDataPath();

		string GetCachePath();

		string GetDeviceIdentifier();

		string GetBundleVersion();

		string GetBundleIdentifier();

		string GetLocale();

		string GetUnfilteredLocale();

		string GetOSVersion();

		string GetOSIdentifier();

		string GetBuildIdentifier();

		void SetKeyboardReturnType(int returnKeyType);

		void ShowNativePopup(string title, string message, string dismissButton, string optionalButton1, string optionalButton2, Action<string> onDismissed);

		void SetDeviceClipBoard(string text);

		int GetDeviceFreeMemory();

		int GetCurrentMemoryUsage();

		int GetPeakMemoryUsage();

		int GetMemoryUsageLimit();

		int GetBatteryLevel();

		float GetBatteryTemperature();

		int GetFreeDiskSpace();

		int GetTotalDiskSpace();

		int GetFreeSDCardSpace();

		int GetTotalSDCardSpace();

		bool CanWeSendUserToAppSettings();

		void SendUserToAppSettings();

		bool IsTablet();

		string GetAndroidExternalSavePath();

		int GetAndroidApiLevel();

		int GetAndroidVersionCode();

		int GetAndroidTopCutout();

		int GetAndroidBottomCutout();

		bool GetIOSOtherAudioPlaying();

		string GetUserLibraryPathiOS();

		string GetUserDocumentsPathiOS();

		string GetUserTempPathiOS();

		string GetiOSBuildNumber();

		void MakeIOSNSLog(string message);

		void DismissKeyboard();
	}
}
