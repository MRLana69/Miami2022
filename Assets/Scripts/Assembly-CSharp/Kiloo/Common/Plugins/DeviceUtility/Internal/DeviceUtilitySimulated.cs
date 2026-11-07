using System;
using System.Diagnostics;
using System.Globalization;
using SYBO.Common.Utilities;
using UnityEngine;

namespace Kiloo.Common.Plugins.DeviceUtility.Internal
{
	internal class DeviceUtilitySimulated : MonoBehaviour, IDeviceUtilityPlatformBridge
	{
		private const string ASSETS_FOLDER = "Assets";

		private const string EDITOR_CACHE_FOLDER = "EditorCache";

		private static float MINIMUM_INCHES_REQUIRED_FOR_TABLET = 6.5f;

		private const int SIMULATED_ANDROID_BUNDLE_VERSION_CODE = 1;

		private const int SIMULATED_ANDROID_API_LEVEL = 8;

		private const bool SIMULATED_IOS_OTHER_AUDIO_PLAYING = false;

		private Action<string> _simulatedOnDismissed;

		private bool _showingPopup;

		private string _titleText;

		private string _messageText;

		private string _dismissText;

		private string _optionalButton1Text;

		private string _optionalButton2Text;

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
			SDebug.LogError("[DeviceUtilitySimulated] " + msg);
		}

		private static void LogWarning(string msg)
		{
		}

		[Conditional("DEVICE_UTILITY_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}

		public string GetBuildIdentifier()
		{
			return GetOSIdentifier();
		}

		public void SetKeyboardReturnType(int returnKeyType)
		{
		}

		public void SetDeviceUtilityDebugEnabled(bool val)
		{
		}

		public string GetUserDataPath()
		{
			return (Application.platform == RuntimePlatform.WebGLPlayer) ? Application.persistentDataPath : Application.dataPath.Replace("Assets", "EditorCache");
		}

		public string GetCachePath()
		{
			return Application.temporaryCachePath;
		}

		public string GetDeviceIdentifier()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public string GetBundleVersion()
		{
			return (Application.platform == RuntimePlatform.WebGLPlayer) ? Application.version : "1.0";
		}

		public string GetBundleIdentifier()
		{
			return "com.kiloo.subwaysimulated";
		}

		public string GetLocale()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			if (!CultureInfo.InvariantCulture.Equals(currentCulture))
			{
				string text = currentCulture.Name;
				if (text.Length != 5 || text[2] != '-')
				{
					text = currentCulture.TwoLetterISOLanguageName;
					if (text.Length != 2)
					{
						text = "en-US";
					}
				}
				return text;
			}
			return "en-US";
		}

		public string GetUnfilteredLocale()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			if (!CultureInfo.InvariantCulture.Equals(currentCulture))
			{
				return currentCulture.Name;
			}
			return "en-US";
		}

		public string GetOSVersion()
		{
			return Environment.OSVersion.Version.ToString(4);
		}

		public string GetOSIdentifier()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
				return "osx";
			case RuntimePlatform.WindowsEditor:
				return "windows";
			case RuntimePlatform.IPhonePlayer:
				return "ios";
			case RuntimePlatform.Android:
				return "android";
			default:
				return "other";
			}
		}

		public void ShowNativePopup(string title, string message, string dismissButton, string positiveButton, string negativeButton, Action<string> onDismissed)
		{
			if (!_showingPopup)
			{
				_simulatedOnDismissed = onDismissed;
				_titleText = title;
				_messageText = message;
				_dismissText = dismissButton;
				_optionalButton1Text = positiveButton;
				_optionalButton2Text = negativeButton;
				_showingPopup = true;
			}
			else
			{
				LogError("Already showing popup");
			}
		}

		public void SetDeviceClipBoard(string text)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = text;
			textEditor.SelectAll();
			textEditor.Copy();
		}

		public int GetDeviceFreeMemory()
		{
			return 0;
		}

		public int GetCurrentMemoryUsage()
		{
			return 0;
		}

		public int GetPeakMemoryUsage()
		{
			return 0;
		}

		public int GetMemoryUsageLimit()
		{
			return 0;
		}

		public int GetBatteryLevel()
		{
			return -1;
		}

		public float GetBatteryTemperature()
		{
			return -1f;
		}

		public int GetFreeDiskSpace()
		{
			return -1;
		}

		public int GetTotalDiskSpace()
		{
			return -1;
		}

		public int GetFreeSDCardSpace()
		{
			return -1;
		}

		public int GetTotalSDCardSpace()
		{
			return -1;
		}

		public bool IsTablet()
		{
			float f = (float)Screen.width / Screen.dpi;
			float f2 = (float)Screen.height / Screen.dpi;
			float num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
			return num >= MINIMUM_INCHES_REQUIRED_FOR_TABLET;
		}

		public string GetAndroidExternalSavePath()
		{
			return Application.persistentDataPath;
		}

		public int GetAndroidApiLevel()
		{
			return 8;
		}

		public int GetAndroidVersionCode()
		{
			return 1;
		}

		public string GetAndroidAdvertisingID()
		{
			return string.Empty;
		}

		public int GetAndroidTopCutout()
		{
			return 0;
		}

		public int GetAndroidBottomCutout()
		{
			return 0;
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

		public bool CanWeSendUserToAppSettings()
		{
			return false;
		}

		public void SendUserToAppSettings()
		{
		}

		private void OnGUI()
		{
			if (_showingPopup)
			{
				GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "Native Popup");
				GUILayout.Label(_titleText);
				GUILayout.Label(_messageText);
				if (GUILayout.Button(_dismissText, GUILayout.MinHeight(100f)))
				{
					TriggerNativePopupDismissEvent(_simulatedOnDismissed, _dismissText);
				}
				if (_optionalButton2Text != null && GUILayout.Button(_optionalButton2Text, GUILayout.MinHeight(100f)))
				{
					TriggerNativePopupDismissEvent(_simulatedOnDismissed, _optionalButton2Text);
				}
				if (_optionalButton1Text != null && GUILayout.Button(_optionalButton1Text, GUILayout.MinHeight(100f)))
				{
					TriggerNativePopupDismissEvent(_simulatedOnDismissed, _optionalButton1Text);
				}
			}
		}

		private void SimClosePopup()
		{
			_showingPopup = false;
			_titleText = null;
			_messageText = null;
			_dismissText = null;
			_optionalButton1Text = null;
			_optionalButton2Text = null;
			_simulatedOnDismissed = null;
		}

		private void TriggerNativePopupDismissEvent(Action<string> onDismissed, string button)
		{
			if (onDismissed != null)
			{
				onDismissed(button);
			}
			SimClosePopup();
		}

		public static DeviceUtilitySimulated CreatePlugin()
		{
			DeviceUtilitySimulated deviceUtilitySimulated = (DeviceUtilitySimulated)UnityEngine.Object.FindObjectOfType(typeof(DeviceUtilitySimulated));
			if (deviceUtilitySimulated == null)
			{
				GameObject gameObject = new GameObject("DeviceUtility Receiver");
				gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
				HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.DynamicManagers);
				HierarchyUtility.SetAsLastSibling(gameObject.transform);
				deviceUtilitySimulated = gameObject.AddComponent<DeviceUtilitySimulated>();
			}
			return deviceUtilitySimulated;
		}
	}
}
