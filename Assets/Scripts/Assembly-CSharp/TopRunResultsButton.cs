using Data.Save;
using JetBrains.Annotations;
using TopRun;
using UnityEngine;

public class TopRunResultsButton : MonoBehaviour
{
	[SerializeField]
	private UILabel _weekLabel;

	[SerializeField]
	private UILabel _finishedLabel;

	[SerializeField]
	private UILabel _buttonLabel;

	[UsedImplicitly]
	private void OnEnable()
	{
		SetLabels();
	}

	private void SetLabels()
	{
		_weekLabel.text = string.Format(Strings.Get("TOPRUN_WEEK_TITLE"), TopRunSave.pendingTopRunResultsWeek);
		_finishedLabel.text = Strings.Get("TOPRUN_STATE_FINISHED");
		_buttonLabel.text = Strings.Get("TOPRUN_BUTTON_GET_RESULT");
	}

	[UsedImplicitly]
	private void OnClick()
	{
		IManager manager = TopRunRoot.manager;
		if (manager != null)
		{
			if (TopRunRewardHelper.ShouldShowResults())
			{
				SetBeatTopRunFriendsAwardProgression();
				TopRunRewardHelper.PrepareAndCallCelebrationScreen(manager);
			}
			else
			{
				SDebug.LogError("Torunament is null or doesn't have a result. Please make sure this button is never shown unless tournament results are available.");
			}
		}
		else
		{
			SDebug.LogError("TopRunManager is null. Please make sure this button is never shown unless maanger data is available.");
		}
	}

	private void SetBeatTopRunFriendsAwardProgression()
	{
		for (int i = 0; i < TopRunSave.pendingTopRunBeatenFriendsAward; i++)
		{
			Stat.TopRunFriendsBeaten.IncrementValue();
		}
	}
}
