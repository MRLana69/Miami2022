using System;
using Data.Save;
using Kiloo.Common.Extensions;
using UnityEngine;

public class UIDailyTimer : MonoBehaviour
{
	[SerializeField]
	private UILabel _timerLabel;

	private bool _refreshPage;

	private int _totalSeconds = -1;

	private void Update()
	{
		TimeSpan ts = EventsSave.wordHuntTimeLeft;
		if (ts.Ticks <= 0)
		{
			ts = EventsSave.dailyWordDeviceTimeLeft;
		}
		int num = (int)ts.TotalSeconds;
		if (_totalSeconds != num)
		{
			_totalSeconds = num;
			_timerLabel.text = ts.NicifyTimeString();
			if (_refreshPage)
			{
				SendMessageUpwards("RefreshDailyChallenge", SendMessageOptions.DontRequireReceiver);
				_refreshPage = false;
			}
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			_refreshPage = true;
		}
	}
}
