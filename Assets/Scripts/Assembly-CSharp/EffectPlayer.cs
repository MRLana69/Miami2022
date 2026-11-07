using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _effectList = new List<GameObject>();

	[SerializeField]
	private float _delay;

	[SerializeField]
	private float _duration;

	[SerializeField]
	private float _maxDuration;

	[SerializeField]
	private float _minimumRotation;

	[SerializeField]
	private float _maximumRotation;

	[SerializeField]
	private bool _doLoop;

	private int _loopFrame = -1;

	[SerializeField]
	private bool _doRandomizeStartingIndex;

	private float _currentDelay;

	private int _currentIndex = -1;

	private float _currentWait;

	private int _startingIndex;

	private float _selectedDuration;

	public int LoopFrame
	{
		set
		{
			_loopFrame = value;
		}
	}

	public float Duration
	{
		get
		{
			return _selectedDuration;
		}
	}

	private void Awake()
	{
		foreach (GameObject effect in _effectList)
		{
			effect.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (_doRandomizeStartingIndex)
		{
			_startingIndex = Random.Range(0, _effectList.Count);
		}
		else
		{
			_startingIndex = 0;
		}
		if (_maxDuration > _duration)
		{
			_selectedDuration = Random.Range(_duration, _maxDuration);
		}
		else
		{
			_selectedDuration = _duration;
		}
		Reset();
	}

	private void Reset()
	{
		_currentDelay = _delay;
		SetActiveIndex(_startingIndex);
		_currentWait = _selectedDuration / (float)_effectList.Count;
	}

	private void Toggle(GameObject gameObject, bool toggle)
	{
		gameObject.SetActive(toggle);
	}

	private void SetActiveIndex(int index)
	{
		if (_currentIndex >= 0)
		{
			Toggle(_effectList[_currentIndex], false);
		}
		_currentIndex = index;
		Toggle(_effectList[_currentIndex], true);
	}

	private void Update()
	{
		if (_currentDelay > 0f)
		{
			_currentDelay -= Time.deltaTime;
			if (_currentDelay > 0f)
			{
				return;
			}
		}
		if (_loopFrame >= 0)
		{
			if (_currentIndex != _loopFrame)
			{
				_currentWait = 0f;
				SetActiveIndex(_loopFrame);
			}
			return;
		}
		if (_currentWait > 0f)
		{
			_currentWait -= Time.deltaTime;
			return;
		}
		int num = _currentIndex + 1;
		if (num >= _effectList.Count)
		{
			if (!_doLoop)
			{
				Toggle(_effectList[num - 1], false);
				_currentIndex = 0;
				base.gameObject.SetActive(false);
				return;
			}
			num = 0;
		}
		SetActiveIndex(num);
		_currentWait = _selectedDuration / (float)_effectList.Count;
	}
}
