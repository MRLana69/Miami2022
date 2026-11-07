using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using WeeklyHunt;

namespace UI.Popups
{
	public class WeeklyHuntTesterPopup : UIPopup
	{
		private enum PeriodTiming
		{
			None = 0,
			Beginning = 1,
			FiveMinsToExpiry = 2,
			ThirtySecsToExpiry = 3
		}

		private enum ProgressStates
		{
			None = 0,
			Zero = 1,
			AlmostComplete = 2,
			Complete = 3
		}

		[SerializeField]
		private SelectPopupButton _weekSelectPopupButton;

		[SerializeField]
		private SelectPopupButton _timingSelectPopupButton;

		[SerializeField]
		private SelectPopupButton _progressStatesPopupButton;

		private WeeklyHuntData.PrizePeriodId _selectedPeriod;

		private PeriodTiming _periodTiming;

		private ProgressStates _progressState;

		private void Start()
		{
			List<string> list = Enum.GetNames(typeof(WeeklyHuntData.PrizePeriodId)).ToList();
			EventDelegate.Add(_weekSelectPopupButton.onChange, OnWeekSelected);
			_weekSelectPopupButton.SetItems(list.GetRange(0, list.Count - 1));
			List<string> items = Enum.GetNames(typeof(PeriodTiming)).ToList();
			EventDelegate.Add(_timingSelectPopupButton.onChange, OnTimeSelected);
			_timingSelectPopupButton.SetItems(items);
			List<string> items2 = Enum.GetNames(typeof(ProgressStates)).ToList();
			EventDelegate.Add(_progressStatesPopupButton.onChange, OnProgressSelected);
			_progressStatesPopupButton.SetItems(items2);
		}

		public void OnWeekSelected()
		{
			if (!string.IsNullOrEmpty(_weekSelectPopupButton.value))
			{
				_selectedPeriod = (WeeklyHuntData.PrizePeriodId)Enum.Parse(typeof(WeeklyHuntData.PrizePeriodId), _weekSelectPopupButton.value);
			}
		}

		public void OnTimeSelected()
		{
			if (!string.IsNullOrEmpty(_timingSelectPopupButton.value))
			{
				_periodTiming = (PeriodTiming)Enum.Parse(typeof(PeriodTiming), _timingSelectPopupButton.value);
			}
		}

		public void OnProgressSelected()
		{
			if (!string.IsNullOrEmpty(_progressStatesPopupButton.value))
			{
				_progressState = (ProgressStates)Enum.Parse(typeof(ProgressStates), _progressStatesPopupButton.value);
			}
		}

		[UsedImplicitly]
		public void SaveAndClose()
		{
			Dictionary<int, WeeklyHuntData.WeeklyHuntPeriodData> huntData = WeeklyHuntData.GetHuntData();
			if (_selectedPeriod != WeeklyHuntData.PrizePeriodId.None)
			{
				int additionalOffset = 0;
				if (_periodTiming == PeriodTiming.FiveMinsToExpiry)
				{
					additionalOffset = -300;
				}
				if (_periodTiming == PeriodTiming.ThirtySecsToExpiry)
				{
					additionalOffset = -30;
				}
				WeeklyHuntData.DebugJumpToWeek((int)_selectedPeriod, additionalOffset);
			}
			if (_progressState != ProgressStates.None)
			{
				int itemsToCollect = huntData[(int)_selectedPeriod].itemsToCollect;
				switch (_progressState)
				{
				case ProgressStates.Zero:
					WeeklyHuntData.DebugSetProgress((int)_selectedPeriod, 0);
					break;
				case ProgressStates.Complete:
					WeeklyHuntData.DebugSetProgress((int)_selectedPeriod, itemsToCollect);
					break;
				case ProgressStates.AlmostComplete:
					WeeklyHuntData.DebugSetProgress((int)_selectedPeriod, itemsToCollect - 1);
					break;
				}
			}
			ClosePopup();
		}

		[UsedImplicitly]
		public void ResetWeeklyHunt()
		{
			WeeklyHuntData.DebugResetWeeklyHuntData();
		}
	}
}
