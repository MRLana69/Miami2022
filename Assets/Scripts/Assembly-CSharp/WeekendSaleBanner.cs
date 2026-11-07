using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo;
using UI;
using UI.Screens;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

public class WeekendSaleBanner : MonoBehaviour
{
	public enum WeekendSaleType
	{
		COINS = 0,
		KEYS = 1
	}

	[Header("Background")]
	[SerializeField]
	private UISprite _fill;

	[Header("Content")]
	[SerializeField]
	private UILabel _titleText;

	[SerializeField]
	private UILabel _subtitleText;

	[SerializeField]
	private GameObject _coinSprites;

	[SerializeField]
	private GameObject _keySprites;

	[SerializeField]
	private float _starAnimationPause;

	[SerializeField]
	private float _starCountDown;

	[SerializeField]
	private List<GameObject> _starPosition;

	[SerializeField]
	private GameObject _starPrefab;

	[Header("Timer")]
	[SerializeField]
	private UILabel _timerLabel;

	[SerializeField]
	private AlignTimer _alignTimer;

	private List<GameObject> _stars = new List<GameObject>();

	private IEnumerator _starAnimation;

	private DateTime _endDateTime;

	public void Init(DateTime endTime, WeekendSaleType type)
	{
		_endDateTime = endTime;
		_fill.gradientTop = NGUIMath.HexToColor(962521343u);
		_fill.gradientBottom = NGUIMath.HexToColor(2308243455u);
		_titleText.text = Strings.Get("WEEKEND_SALE_BANNER_TITLE");
		_titleText.color = ColorData.instance.sun;
		_titleText.effectColor = ColorData.instance.shadow;
		_subtitleText.text = Strings.Get("WEEKEND_SALE_BANNER_SUBTITLE");
		_subtitleText.color = Color.white;
		_subtitleText.effectColor = ColorData.instance.shadow;
		_timerLabel.text = Strings.Get("TIMER_ENDS_IN");
		_alignTimer.SetTimerColor(ColorData.instance.mahogany);
		_coinSprites.SetActive(type == WeekendSaleType.COINS);
		_keySprites.SetActive(type == WeekendSaleType.KEYS);
		if (_starPosition.Count <= 0 || _stars.Count > 0 || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (GameObject item in _starPosition)
		{
			_stars.Add(UnityEngine.Object.Instantiate(_starPrefab, item.transform.position, Quaternion.Euler(0f, 0f, 0f), item.transform));
		}
		_starAnimation = ActivateStars();
		StartCoroutine(_starAnimation);
	}

	private void Update()
	{
		if ((_endDateTime - CDateTime.UtcNow).TotalSeconds <= 0.0)
		{
			StoreScreen.RefreshOnNextShow();
		}
		_alignTimer.SetTimeRemaining(_endDateTime - CDateTime.UtcNow);
	}

	private void OnEnable()
	{
		if (_starAnimation != null)
		{
			StartCoroutine(_starAnimation);
		}
	}

	private void OnDisable()
	{
		if (_starAnimation != null)
		{
			StopCoroutine(_starAnimation);
		}
	}

	private IEnumerator ActivateStars()
	{
		while (true)
		{
			foreach (GameObject star in _stars)
			{
				int children = star.transform.childCount;
				for (int i = 0; i < children; i++)
				{
					star.transform.GetChild(i).GetComponent<Animation>().Play();
				}
				yield return new WaitForSeconds(_starCountDown);
			}
			yield return new WaitForSeconds(_starAnimationPause);
		}
	}
}
