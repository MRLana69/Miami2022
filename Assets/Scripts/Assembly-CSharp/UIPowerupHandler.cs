using System.Collections.Generic;
using Info;
using SYBO.Subway;
using UnityEngine;
using Utilities;

public class UIPowerupHandler : MonoBehaviour
{
	public GameObject PowerupPrefab;

	private int _screenOffset = 26;

	private int _bottomOffset = 5;

	private List<UIPowerupHelper> _powerupSlots = new List<UIPowerupHelper>();

	private bool _isPowerupButtonsShowing;

	private const int MAX_SHOWABLE_PROGRESS_BARS = 8;

	private int previousSlotAmountLastFrame;

	private void Start()
	{
		InstantiatePowerupSlots(8);
		SlideinPowerupHelper.OnButtonsShow += HandleSlideInPowerUpShow;
		SlideinPowerupHelper.OnButtonsHide += HandleSlideInPowerUpHide;
	}

	private void OnDestroy()
	{
		SlideinPowerupHelper.OnButtonsShow -= HandleSlideInPowerUpShow;
		SlideinPowerupHelper.OnButtonsHide -= HandleSlideInPowerUpHide;
	}

	private void HandleSlideInPowerUpShow()
	{
		if (!Game.Instance.isDead)
		{
			_isPowerupButtonsShowing = true;
			ReanchorPowerupSlots();
			for (int i = 0; i < _powerupSlots.Count; i++)
			{
				SetPowerupSlotPosition(i);
			}
		}
	}

	private void HandleSlideInPowerUpHide()
	{
		if (!Game.Instance.isDead)
		{
			_isPowerupButtonsShowing = false;
			ReanchorPowerupSlots();
			for (int i = 0; i < _powerupSlots.Count; i++)
			{
				SetPowerupSlotPosition(i);
			}
		}
	}

	private void Update()
	{
		List<ActivePowerup> activePowerups = GameStats.Instance.GetActivePowerups();
		int num = activePowerups.Count;
		if (num > 8)
		{
			num = 8;
		}
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			SetPowerupSlotContent(activePowerups[i], i);
			num2++;
		}
		if (previousSlotAmountLastFrame != num2)
		{
			previousSlotAmountLastFrame = num2;
			for (int j = num2; j < _powerupSlots.Count; j++)
			{
				DisablePowerupSlot(_powerupSlots[j]);
			}
		}
	}

	private void DisablePowerupSlot(UIPowerupHelper slot)
	{
		if (slot.gameObject.activeInHierarchy)
		{
			slot.HidePowerupSlot();
			slot.gameObject.SetActive(false);
		}
	}

	private void SetPowerupSlotContent(ActivePowerup powerup, int i)
	{
		if (powerup.type != UpgradeType.hoverboard || powerup.type != UpgradeType.mysteryPowerup)
		{
			_powerupSlots[i].SetPowerupSlot(powerup);
		}
		if (!_powerupSlots[i].gameObject.activeInHierarchy)
		{
			_powerupSlots[i].gameObject.SetActive(true);
		}
	}

	private void InstantiatePowerupSlots(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			GameObject gameObject = base.gameObject.AddChild(PowerupPrefab);
			_powerupSlots.Add(gameObject.GetComponent<UIPowerupHelper>());
			if (i % 2 == 0)
			{
				_powerupSlots[i].GetComponent<UIAnchor>().side = UIAnchor.Side.BottomLeft;
			}
			else
			{
				_powerupSlots[i].GetComponent<UIAnchor>().side = UIAnchor.Side.BottomRight;
			}
			DisablePowerupSlot(_powerupSlots[i]);
			SetPowerupSlotPosition(i);
		}
	}

	private void SetPowerupSlotPosition(int i)
	{
		float x = _screenOffset;
		if (_powerupSlots[i].GetComponent<UIAnchor>().side == UIAnchor.Side.BottomRight)
		{
			x = _powerupSlots[i].getSliderWidth() * -1 + _screenOffset / 2;
		}
		float num = 0f;
		num = (_isPowerupButtonsShowing ? ((float)(_screenOffset / 4 * (i + 1)) + (float)i * _powerupSlots[i].getSliderHeight() + (float)_bottomOffset) : ((float)getNumberOfSpaces(i) + getSliderHights(i, i) + (float)_bottomOffset));
		if (SafeArea.IsRestricted)
		{
			num += (float)SafeArea.OFFSET_BOTTOM;
		}
		_powerupSlots[i].setContainerPosition(x, num, 0f);
	}

	private void ReanchorPowerupSlots()
	{
		for (int i = 0; i < _powerupSlots.Count; i++)
		{
			if (i % 2 == 0)
			{
				_powerupSlots[i].GetComponent<UIAnchor>().side = UIAnchor.Side.BottomLeft;
			}
			else
			{
				_powerupSlots[i].GetComponent<UIAnchor>().side = UIAnchor.Side.BottomRight;
			}
			if (_isPowerupButtonsShowing)
			{
				_powerupSlots[i].GetComponent<UIAnchor>().side = UIAnchor.Side.BottomRight;
			}
		}
	}

	public float getSliderHights(int i, int slotPosition)
	{
		return (float)Mathf.FloorToInt(i / 2) * _powerupSlots[slotPosition].getSliderHeight();
	}

	public int getNumberOfSpaces(int i)
	{
		if (i % 2 == 0)
		{
			return _screenOffset / 4 * (Mathf.CeilToInt((float)i / 2f) + 1);
		}
		return _screenOffset / 4 * Mathf.CeilToInt((float)i / 2f);
	}
}
