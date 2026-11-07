using System.Collections.Generic;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace Onboarding
{
	public static class OnboardingTracking
	{
		public enum ScreenType
		{
			Gameover = 0,
			FrontScreen = 1
		}

		private const string PLAYER_PREF_ONBOARDING_GAMEOVER_LOG_KEY = "ShouldLogFrontScreenFlow";

		private const string PLAYER_PREF_ONBOARDING_FRONTSCREEN_LOG_KEY = "ShouldLogGameoverScreenFlow";

		private static List<string> listOfScreensAfterGameover = new List<string>();

		private static List<string> listOfScreensAfterFrontscreen = new List<string>();

		private const int LOG_LIMIT = 6;

		public static void LogScreenFlow(string screenName)
		{
			if (IsTrackingActive())
			{
				QueueOnboardingEvents(screenName);
			}
		}

		private static bool IsTrackingActive()
		{
			return IsTrackingActiveFor(ScreenType.FrontScreen) || IsTrackingActiveFor(ScreenType.Gameover);
		}

		private static bool IsTrackingActiveFor(ScreenType screenType)
		{
			string text = string.Empty;
			switch (screenType)
			{
			case ScreenType.FrontScreen:
				text = "ShouldLogGameoverScreenFlow";
				break;
			case ScreenType.Gameover:
				text = "ShouldLogFrontScreenFlow";
				break;
			}
			if (!text.IsNullOrEmpty())
			{
				return PlayerPrefs.GetInt(text, 0) != 0;
			}
			return false;
		}

		private static void DisableOnboardingTracking(ScreenType screenType)
		{
			string text = string.Empty;
			switch (screenType)
			{
			case ScreenType.FrontScreen:
				text = "ShouldLogGameoverScreenFlow";
				break;
			case ScreenType.Gameover:
				text = "ShouldLogFrontScreenFlow";
				break;
			}
			if (!text.IsNullOrEmpty())
			{
				PlayerPrefs.DeleteKey(text);
				PlayerPrefs.Save();
			}
		}

		public static void EnableOnboardingTracking(ScreenType screenType)
		{
			string text = string.Empty;
			switch (screenType)
			{
			case ScreenType.FrontScreen:
				text = "ShouldLogGameoverScreenFlow";
				break;
			case ScreenType.Gameover:
				text = "ShouldLogFrontScreenFlow";
				break;
			}
			if (!text.IsNullOrEmpty())
			{
				PlayerPrefs.SetInt(text, 1);
				PlayerPrefs.Save();
			}
		}

		private static void QueueOnboardingEvents(string screenName)
		{
			if (IsTrackingActiveFor(ScreenType.FrontScreen) && (listOfScreensAfterFrontscreen.Count > 0 || screenName == "FrontScreen"))
			{
				listOfScreensAfterFrontscreen.Add(screenName);
				DispatchQueuedEventsIfNeeded(ScreenType.FrontScreen);
			}
			if (IsTrackingActiveFor(ScreenType.Gameover) && (listOfScreensAfterGameover.Count > 0 || screenName == "GameOverScreen"))
			{
				listOfScreensAfterGameover.Add(screenName);
				DispatchQueuedEventsIfNeeded(ScreenType.Gameover);
			}
		}

		private static void DispatchQueuedEventsIfNeeded(ScreenType screenType)
		{
			if (LogLimitReached(screenType))
			{
				DispatchEvents(screenType);
			}
		}

		private static void DispatchEvents(ScreenType screenType)
		{
			List<string> list = ((screenType != ScreenType.Gameover) ? listOfScreensAfterFrontscreen : listOfScreensAfterGameover);
			string eventName = ((screenType != ScreenType.Gameover) ? "Front screen flow" : "Game over screen flow");
			if (list.Count > 1)
			{
				DisableOnboardingTracking(screenType);
				string argValue = string.Join(",", list.ToArray(), 1, list.Count - 1);
				Flurry.LogEventWithAParameter(eventName, "Screen order", argValue);
			}
		}

		private static bool LogLimitReached(ScreenType screenType)
		{
			List<string> list = ((screenType != ScreenType.Gameover) ? listOfScreensAfterFrontscreen : listOfScreensAfterGameover);
			return list.Count >= 6;
		}

		public static void OnApplicationQuit()
		{
			if (IsTrackingActive())
			{
				DispatchEvents(ScreenType.Gameover);
				DispatchEvents(ScreenType.FrontScreen);
			}
		}
	}
}
