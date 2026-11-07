using Missions;
using UnityEngine;

public class AwardsNotificationHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite notificationIcon;

	[SerializeField]
	private UILabel notificationLabel;

	[SerializeField]
	private UISprite exclamationMark;

	private bool _hasGoneToZero;

	private void OnEnable()
	{
		RefreshNotification();
	}

	private void Update()
	{
		if (!_hasGoneToZero && UIScreenController.Instance.GetTopScreenName() == "AwardsScreen")
		{
			RefreshNotification();
		}
	}

	private void RefreshNotification()
	{
		int numberOfNewVisibleAwards = AwardsManager.instance.GetNumberOfNewVisibleAwards();
		if (numberOfNewVisibleAwards > 0)
		{
			notificationIcon.enabled = true;
			notificationLabel.enabled = true;
			notificationLabel.text = numberOfNewVisibleAwards.ToString();
			exclamationMark.enabled = numberOfNewVisibleAwards == 1;
			notificationLabel.enabled = numberOfNewVisibleAwards > 1;
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
