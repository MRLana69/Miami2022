using UI;
using UnityEngine;

public class BoardsNotificationHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite notificationIcon;

	[SerializeField]
	private UILabel notificationLabel;

	[SerializeField]
	private UISprite exclamationMark;

	private bool _hasGoneToZero;

	private const float UPDATE_INTERVAL = 0.1f;

	private float _currentInterval;

	private void OnEnable()
	{
		RefreshNotification();
	}

	private void Update()
	{
		if (!_hasGoneToZero && !(UIScreenController.Instance.GetTopScreenName() != "BoardScreen"))
		{
			if (_currentInterval <= 0f)
			{
				_currentInterval += 0.1f;
				RefreshNotification();
			}
			else
			{
				_currentInterval -= Time.deltaTime;
			}
		}
	}

	private void RefreshNotification()
	{
		int numberOfNewHoverboardsAndThemes = HoverboardStickerManager.GetNumberOfNewHoverboardsAndThemes();
		if (numberOfNewHoverboardsAndThemes > 0)
		{
			notificationIcon.enabled = true;
			notificationLabel.enabled = true;
			notificationLabel.text = numberOfNewHoverboardsAndThemes.ToString();
			exclamationMark.enabled = numberOfNewHoverboardsAndThemes == 1;
			notificationLabel.enabled = numberOfNewHoverboardsAndThemes > 1;
		}
		else
		{
			notificationIcon.enabled = false;
			notificationLabel.enabled = false;
			exclamationMark.enabled = false;
			_hasGoneToZero = true;
		}
	}
}
