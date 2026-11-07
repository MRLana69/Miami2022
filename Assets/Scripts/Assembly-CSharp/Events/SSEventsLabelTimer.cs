using System;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using UnityEngine;

namespace Events
{
	[RequireComponent(typeof(UILabel))]
	public class SSEventsLabelTimer : MonoBehaviour
	{
		private UILabel _thisLabel;

		private Timer _timer;

		private void Awake()
		{
			StartTimer();
		}

		private void OnDisable()
		{
			if (_timer != null)
			{
				_timer.Stop();
			}
		}

		private void OnEnable()
		{
			StartTimer();
		}

		private void StartTimer()
		{
			if (_timer == null)
			{
				_timer = new Timer(1f);
				_timer.intervalElapsed += RefreshTimeLabel;
			}
			if (!_timer.isRunning)
			{
				_timer.Start();
			}
			RefreshTimeLabel();
		}

		private void RefreshTimeLabel()
		{
			if (_thisLabel == null)
			{
				_thisLabel = base.gameObject.GetComponent<UILabel>();
			}
			TimeSpan ts = TimeSpan.FromSeconds(SSEventsManagerController.instance.TimeLeftInSeconds());
			_thisLabel.text = ts.NicifyTimeString();
		}
	}
}
