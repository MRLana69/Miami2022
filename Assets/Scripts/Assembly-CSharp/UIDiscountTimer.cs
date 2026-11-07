using System;
using InApp;
using Kiloo;
using UnityEngine;

public class UIDiscountTimer : MonoBehaviour
{
	private UILabel _timerLabel;

	private double endTimeDouble;

	private DateTime endTime;

	private TimeSpan timeLeft;

	private void Awake()
	{
		_timerLabel = GetComponent<UILabel>();
	}

	private void OnEnable()
	{
		endTimeDouble = InAppManager.GetDiscountEndTime();
		endTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(endTimeDouble);
	}

	private void Update()
	{
		timeLeft = endTime - CDateTime.UtcNow;
		if (0 < timeLeft.Ticks)
		{
			if (timeLeft.Days > 0)
			{
				string key = ((timeLeft.Days != 1) ? "TIME_LEFT_DAYS_PLURAL" : "TIME_LEFT_DAYS_SINGULAR");
				_timerLabel.text = string.Format(Strings.Get(key), timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
			}
			else
			{
				_timerLabel.text = string.Format(Strings.Get("TIME_LEFT_HOURS"), timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
			}
		}
		else
		{
			SendMessageUpwards("ForceEndDiscount", SendMessageOptions.DontRequireReceiver);
		}
	}
}
