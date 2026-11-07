using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace Utilities
{
	public static class SafeArea
	{
		private static int _offsetTop;

		private static int _offsetBottom;

		private static bool _isRestricted;

		private static bool _hasCheckedOnce;

		public static bool PortraitUpsideDownIsAllowed = true;

		public static int OFFSET_TOP
		{
			get
			{
				VerifyHasAssignedOffsets();
				return _offsetTop;
			}
		}

		public static int OFFSET_BOTTOM
		{
			get
			{
				VerifyHasAssignedOffsets();
				return _offsetBottom;
			}
		}

		public static bool IsRestricted
		{
			get
			{
				return CheckForSafeAreaRestrictions();
			}
		}

		public static bool deviceUsesStatusBar
		{
			get
			{
				if (IsRestricted && OFFSET_TOP > 0)
				{
					return true;
				}
				return false;
			}
		}

		private static bool CheckForSafeAreaRestrictions()
		{
			if (!_hasCheckedOnce)
			{
				_isRestricted = false;
				if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
				{
					if (Screen.height == 2436 && Screen.width == 1125)
					{
						SetOffsets(out _isRestricted);
					}
					else if (Screen.height == 2688 && Screen.width == 1242)
					{
						SetOffsets(out _isRestricted);
					}
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					SetOffsets(out _isRestricted);
				}
				else if (Application.platform == RuntimePlatform.Android && !DeviceUtility.isKindleDevice)
				{
					SetOffsets(out _isRestricted, true);
				}
				_hasCheckedOnce = true;
			}
			return _isRestricted;
		}

		private static void SetOffsets(out bool restrictionsWereDetected, bool isAndroid = false)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (!isAndroid)
			{
				num = IOSViewController.GetTopInsetInPoints();
				num2 = IOSViewController.GetBottomInsetInPoints();
			}
			else
			{
				num = DeviceUtility.GetAndroidTopCutout();
				num2 = DeviceUtility.GetAndroidBottomCutout();
			}
			if (num > 0f || num2 > 0f)
			{
				restrictionsWereDetected = true;
				num3 = (isAndroid ? ((float)Screen.height) : ((float)IOSViewController.GetDeviceHeightInPoints()));
				float num4 = num / num3;
				float num5 = num2 / num3;
				float num6 = 0f;
				if (UIScreenController.IsInstanced && UIScreenController.Instance != null && UIScreenController.Instance.root != null)
				{
					num6 = UIScreenController.Instance.root.manualHeight;
				}
				else
				{
					SDebug.LogError("UIScreenController was not ready  for SafeArea! Are you attempting to use SafeArea way too early in startup?");
				}
				_offsetTop = (num6 * num4).RoundToInt();
				_offsetBottom = (num6 * num5).RoundToInt();
			}
			else
			{
				restrictionsWereDetected = false;
			}
		}

		private static void VerifyHasAssignedOffsets()
		{
			if (!_hasCheckedOnce)
			{
				CheckForSafeAreaRestrictions();
			}
		}
	}
}
