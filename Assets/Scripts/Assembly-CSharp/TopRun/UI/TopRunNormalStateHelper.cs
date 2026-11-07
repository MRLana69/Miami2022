using System;
using Kiloo;
using Kiloo.Common.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace TopRun.UI
{
	public class TopRunNormalStateHelper : MonoBehaviour
	{
		private enum NormalStateType
		{
			Ongoing = 0,
			WaitingForResults = 1,
			Over = 2
		}

		[SerializeField]
		private UISprite _background;

		[FormerlySerializedAs("_weekLabel")]
		[SerializeField]
		private UILabel _leftLabel;

		[FormerlySerializedAs("_statusLabel")]
		[SerializeField]
		private UILabel _rightLabel;

		private ITournament _currentTournament;

		private const string ZERO_CLOCK = "00:00:00";

		private const int HOURS_THRESHOLD = 100;

		private const string DAYS_LEFT_FORMAT = "[B6CCE0]{0}[-] {1}";

		private NormalStateType currentState { get; set; }

		public void InitNormalState()
		{
			IManager manager = TopRunRoot.manager;
			if (manager != null && manager.tournament != null)
			{
				ITournament tournament = (_currentTournament = manager.tournament);
				if (CDateTime.UtcNow > tournament.officialEndTimeUTC)
				{
					if (tournament.hasResult)
					{
						SetOverState(tournament);
					}
					else if (manager.score <= 0)
					{
						SetOverState(tournament);
					}
					else
					{
						SetWaitingForResultsState(tournament);
					}
				}
				else if (tournament.isFinished)
				{
					SetOverState(tournament);
				}
				else
				{
					SetOngoingState(tournament);
				}
			}
			else
			{
				SDebug.LogError("Normal State triggered while no Tournament Data is available.");
			}
		}

		private void Update()
		{
			if (_currentTournament == null)
			{
				return;
			}
			switch (currentState)
			{
			case NormalStateType.Ongoing:
			{
				SetOngoingState(_currentTournament);
				DateTime officialEndTimeUTC = _currentTournament.officialEndTimeUTC;
				if (CDateTime.UtcNow > officialEndTimeUTC)
				{
					if (TopRunRoot.manager.score > 0)
					{
						SetWaitingForResultsState(_currentTournament);
					}
					else
					{
						SetOverState(_currentTournament);
					}
				}
				break;
			}
			case NormalStateType.WaitingForResults:
				if (_currentTournament.hasResult)
				{
					TopRunRewardHelper.RefreshPendingResults();
				}
				break;
			case NormalStateType.Over:
				SetOverState(_currentTournament);
				break;
			default:
				SDebug.LogError("No avilable handler for case " + currentState);
				break;
			}
		}

		private void SetOngoingState(ITournament tournament)
		{
			currentState = NormalStateType.Ongoing;
			_leftLabel.text = string.Format(Strings.Get("TOPRUN_WEEK_TITLE"), tournament.sequenceId);
			DateTime officialEndTimeUTC = _currentTournament.officialEndTimeUTC;
			_rightLabel.text = "[B6CCE0]{0}[-] {1}".Populate(GetDaysLeft(officialEndTimeUTC), GetDaysPostfix(officialEndTimeUTC));
		}

		private void SetWaitingForResultsState(ITournament tournament)
		{
			currentState = NormalStateType.WaitingForResults;
			_leftLabel.text = string.Format(Strings.Get("TOPRUN_WEEK_TITLE"), tournament.sequenceId);
			_rightLabel.text = Strings.Get("TOPRUN_STATE_WAIT_FOR_RESULTS");
		}

		private void SetOverState(ITournament tournament)
		{
			currentState = NormalStateType.Over;
			_leftLabel.text = string.Format(Strings.Get("TOPRUN_WEEK_TITLE"), tournament.sequenceId);
			DateTime nextTournamentFetchTime = TopRunRoot.manager.nextTournamentFetchTime;
			TimeSpan timeSpan = nextTournamentFetchTime - CDateTime.UtcNow;
			if (HasNextTournamentStarted(nextTournamentFetchTime) || timeSpan.TotalHours > 100.0)
			{
				_rightLabel.text = Strings.Get("TOPRUN_STATE_NEXT_SOON");
				return;
			}
			_rightLabel.text = "{0} [B6CCE0]{1}[-]".Populate(Strings.Get("TOPRUN_STATE_NEXT_IN"), GetTimeToNextTournament(TopRunRoot.manager.nextTournamentFetchTime));
		}

		private string GetDaysLeft(DateTime tournamentEnd)
		{
			TimeSpan timeSpan = tournamentEnd - CDateTime.UtcNow;
			if (CDateTime.UtcNow > tournamentEnd)
			{
				return "00:00:00";
			}
			if (timeSpan.Days > 0)
			{
				return timeSpan.Days.ToString();
			}
			return GetTimeLeftFormatted(timeSpan);
		}

		private static string GetDaysPostfix(DateTime tournamentEnd)
		{
			TimeSpan timeSpan = tournamentEnd - CDateTime.UtcNow;
			if (CDateTime.UtcNow > tournamentEnd)
			{
				return "00:00:00";
			}
			if (timeSpan.Days > 1)
			{
				return Strings.Get("TOPRUN_STATE_DAYS_LEFT");
			}
			if (timeSpan.Days == 1)
			{
				return Strings.Get("TOPRUN_STATE_DAYS_LEFT_SINGLE");
			}
			return Strings.Get("TOPRUN_STATE_TIME_LEFT");
		}

		private bool HasNextTournamentStarted(DateTime nextTournamentStartTime)
		{
			if ((nextTournamentStartTime - CDateTime.UtcNow).Days > 99)
			{
				return true;
			}
			if (CDateTime.UtcNow > nextTournamentStartTime)
			{
				return true;
			}
			return false;
		}

		private string GetTimeToNextTournament(DateTime nextTournamentStartTime)
		{
			TimeSpan timeSpan = nextTournamentStartTime - CDateTime.UtcNow;
			if (timeSpan.Days > 99)
			{
				return string.Empty;
			}
			if (CDateTime.UtcNow > nextTournamentStartTime)
			{
				return "00:00:00";
			}
			return GetTimeLeftFormatted(timeSpan);
		}

		private string GetTimeLeftFormatted(TimeSpan timeSpan)
		{
			return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes, timeSpan.Seconds);
		}
	}
}
