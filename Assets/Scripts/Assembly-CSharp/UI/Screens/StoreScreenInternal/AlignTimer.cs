using System;
using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	public class AlignTimer : MonoBehaviour
	{
		[Header("Timer")]
		[SerializeField]
		private Layout _timeLayout;

		[SerializeField]
		private UILabel _timeLabel;

		[SerializeField]
		private UISprite _animatedClock;

		[SerializeField]
		private UISprite _clockFrame;

		private int _savedTime = -1;

		private bool _shouldAlignTime;

		public void SetTimerColor(Color color)
		{
			_timeLabel.color = color;
			_animatedClock.color = color;
			_clockFrame.color = color;
		}

		public void SetTimeRemaining(TimeSpan ts)
		{
			if (ts.Days > 0)
			{
				if (ts.Hours != _savedTime)
				{
					_savedTime = ts.Hours;
					_shouldAlignTime = true;
				}
			}
			else if (ts.Hours > 0)
			{
				if (ts.Minutes != _savedTime)
				{
					_savedTime = ts.Minutes;
					_shouldAlignTime = true;
				}
			}
			else if (ts.Seconds != _savedTime)
			{
				_savedTime = ts.Seconds;
				_shouldAlignTime = true;
			}
			if (_shouldAlignTime)
			{
				_timeLabel.text = ts.NicifyTimeString();
				_timeLayout.padding = 2;
				_timeLayout.Align(true);
				_shouldAlignTime = false;
			}
		}
	}
}
