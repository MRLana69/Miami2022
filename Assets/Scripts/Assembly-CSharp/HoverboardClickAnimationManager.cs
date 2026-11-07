using Data.Save;
using Kiloo.Common.Timer;
using SYBO.Subway;
using UI.Helpers;

public static class HoverboardClickAnimationManager
{
	private const float HOVERBOARD_CLICK_ANIMATION_DELAY = 2.8f;

	private const int IN_TUTORIAL_RUN_STEP = 0;

	private const int SECOND_STEP = 1;

	private const int LAST_STEP = 2;

	private const int TUTORIAL_OVER = 3;

	public static HoverboardClickingAnimationHelper hoverboardClickingAnimationHelper { get; set; }

	public static bool HasPossibilityOfShowingClickAnimation()
	{
		if (Globals.DEBUG_SHOW_HOVERBOARD_CLICK_ANIMATION_EACH_RUN)
		{
			return true;
		}
		return OnboardingSave.numberOfHoverboardsUsed <= 0 && OnboardingSave.currentHoverboardTutorialStep < 3;
	}

	public static void CheckIfShouldShowClickAnimation()
	{
		if (!CanShowClickingAnimationInCurrentSession())
		{
			return;
		}
		if (OnboardingSave.isTutorialCompleted)
		{
			if (OnboardingSave.currentHoverboardTutorialStep == 0)
			{
				OnboardingSave.currentHoverboardTutorialStep++;
			}
			ShowClickingAnimationWithDelay(2.8f);
		}
		else
		{
			ShowClickingAnimation();
		}
	}

	private static bool CanShowClickingAnimationInCurrentSession()
	{
		if (Globals.DEBUG_SHOW_HOVERBOARD_CLICK_ANIMATION_EACH_RUN && OnboardingSave.isTutorialCompleted)
		{
			return true;
		}
		return Game.Instance.IsFirstRun && CanShowClickingAnimation() && AreTutorialStepRequirementsReached(OnboardingSave.currentHoverboardTutorialStep);
	}

	private static bool AreTutorialStepRequirementsReached(int tutorialStep)
	{
		switch (tutorialStep)
		{
		case 0:
			return true;
		case 1:
			return OnboardingSave.firstNonTutorialRunSession == OnboardingSave.currentSessionCount;
		case 2:
			return OnboardingSave.firstNonTutorialRunSession != OnboardingSave.currentSessionCount;
		default:
			return false;
		}
	}

	private static void ShowClickingAnimationWithDelay(float delay)
	{
		Timer.ExecuteWithDelay(delay, ShowClickingAnimation);
	}

	private static void ShowClickingAnimation()
	{
		if (CanShowClickingAnimation())
		{
			hoverboardClickingAnimationHelper.PlayAnimationAndDeactivate();
			if (OnboardingSave.isTutorialCompleted)
			{
				OnboardingSave.currentHoverboardTutorialStep++;
			}
		}
	}

	private static bool CanShowClickingAnimation()
	{
		if (Globals.DEBUG_SHOW_HOVERBOARD_CLICK_ANIMATION_EACH_RUN)
		{
			return true;
		}
		return OnboardingSave.numberOfHoverboardsUsed <= 0 && !Game.Instance.IsPaused && !Game.Instance.isDead;
	}
}
