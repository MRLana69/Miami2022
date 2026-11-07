using System;
using Kiloo;
using TopRun;
using UI.Generic.Layout;
using UI.Screens;
using UnityEngine;

public class UITopRunTimer : MonoBehaviour
{
	[SerializeField]
	private UILabel _currentTopRunWeek;

	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private PauseScreen _pauseScreen;

	[SerializeField]
	private Layout _timerLayout;

	private bool _lessThanAdayLeft;

	private int _totalSeconds = -1;

	private IManager _tournamentManager;

	private ITournament _currentTournament;

	public void DoTimer()
	{
		_tournamentManager = TopRunRoot.manager;
		if (_tournamentManager != null && _tournamentManager.tournament != null)
		{
			SetTextTopRunTimerLabels();
		}
	}

	private void SetTextTopRunTimerLabels()
	{
		if (_tournamentManager != null && _tournamentManager.tournament != null)
		{
			_currentTournament = _tournamentManager.tournament;
			TimeSpan timeSpan = _currentTournament.officialEndTimeUTC - CDateTime.UtcNow;
			if (CDateTime.UtcNow > _currentTournament.officialEndTimeUTC)
			{
				_lessThanAdayLeft = false;
				_timerLabel.text = Strings.Get("TOPRUN_STATE_NEXT_SOON");
				_timerLabel.fontSize = 15;
			}
			else if (timeSpan.Days > 1)
			{
				_lessThanAdayLeft = false;
				_timerLabel.text = string.Format(Strings.Get("TOPRUN_STATE_DAYS_LEFT_FORMAT"), timeSpan.Days);
				_timerLabel.fontSize = 24;
			}
			else if (timeSpan.Days == 1)
			{
				_lessThanAdayLeft = false;
				_timerLabel.text = string.Format(Strings.Get("TOPRUN_STATE_DAYS_LEFT_SINGLE_FORMAT"), timeSpan.Days);
				_timerLabel.fontSize = 24;
			}
			else
			{
				_timerLabel.fontSize = 24;
				_lessThanAdayLeft = true;
				Update();
			}
			_currentTopRunWeek.text = string.Format(Strings.Get("TOPRUN_WEEK_TITLE"), _tournamentManager.tournament.sequenceId);
			_timerLayout.Align();
		}
	}

	private void Update()
	{
		if (!_lessThanAdayLeft)
		{
			return;
		}
		TimeSpan timeSpan = _currentTournament.officialEndTimeUTC - CDateTime.UtcNow;
		int num = (int)timeSpan.TotalSeconds;
		if (_totalSeconds != num)
		{
			_totalSeconds = num;
			if (timeSpan.Ticks > 0)
			{
				_timerLabel.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				return;
			}
			_lessThanAdayLeft = false;
			_timerLabel.text = Strings.Get("TOPRUN_STATE_NEXT_SOON");
			_timerLabel.fontSize = 15;
			_timerLayout.Align();
		}
	}
}
