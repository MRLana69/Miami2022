using System;
using System.Diagnostics;
using System.Globalization;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace UI
{
	public class WelcomePackManager
	{
		private const int WELCOME_PACK_DAYS_ACTIVE = 3;

		private const int WELCOME_PACK_ACTIVE_FROM_VERSION = 1890;

		public static string welcomePackInAppID
		{
			get
			{
				return "com.kiloo.subwaysurfers.welcomepacktier02";
			}
		}

		public static TimeSpan welcomePackTimeLeft
		{
			get
			{
				return UserSave.welcomePackStartTime.AddDays(3.0) - CDateTime.UtcNow;
			}
		}

		public static bool IsWelcomeBoardActive(Boards.BoardId boardId)
		{
			return boardId == Boards.BoardId.starterPackBoard01 && IsWelcomePackActive();
		}

		public static bool IsWelcomePackActive()
		{
			UpdateSetWelcomePackStartTime();
			return (welcomePackTimeLeft.TotalSeconds > 0.0 && welcomePackTimeLeft.TotalDays < 3.0 && !BoardManager.IsBoardOwned(Boards.BoardId.starterPackBoard01) && !welcomePackInAppID.IsNullOrEmpty() && ParseFirstInstalledVersion() >= 1890) || Globals.DEBUG_ENABLE_WELCOME_PACK;
		}

		private static int ParseFirstInstalledVersion()
		{
			string s = GetFirstInstalledVersion().Replace(".", string.Empty);
			int result;
			int.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
			return result;
		}

		private static string GetFirstInstalledVersion()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return DeviceUtility.GetBundleVersion();
			}
			return UserSave.firstInstalledVersion;
		}

		private static void UpdateSetWelcomePackStartTime()
		{
			DateTime welcomePackStartTime = UserSave.welcomePackStartTime;
			if (welcomePackStartTime == DateTime.MinValue && !welcomePackInAppID.IsNullOrEmpty())
			{
				UserSave.welcomePackStartTime = CDateTime.UtcNow;
			}
		}

		public static void ResetWelcomePack()
		{
			UserSave.welcomePackStartTime = CDateTime.UtcNow;
			BoardManager.LockBoard(Boards.BoardId.starterPackBoard01);
		}

		[Conditional("ENABLE_WELCOME_PACK_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
