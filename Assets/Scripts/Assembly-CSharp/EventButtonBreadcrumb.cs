using Data.Save;
using Kiloo;
using UnityEngine;

public class EventButtonBreadcrumb : MonoBehaviour
{
	[SerializeField]
	private GameObject _breadcrumb;

	private IEventManager _activeEventManager;

	private void OnEnable()
	{
		SSEventsManagerController.OnSSEventStartedHandler += CheckForUnseenEventPopup;
		SSEventsManagerController.OnSSEventChangedHandler += CheckForUnseenEventPopup;
		SSEventsManagerController.OnSSEventFinishedHandler += HideBreadcrumb;
		UIScreenController.OnPopupShown += PopupWasShown;
		CheckForUnseenEventPopup();
	}

	private void OnDisable()
	{
		SSEventsManagerController.OnSSEventStartedHandler -= CheckForUnseenEventPopup;
		SSEventsManagerController.OnSSEventChangedHandler -= CheckForUnseenEventPopup;
		SSEventsManagerController.OnSSEventFinishedHandler -= HideBreadcrumb;
		UIScreenController.OnPopupShown -= PopupWasShown;
	}

	private void PopupWasShown(string popup)
	{
		if (popup == "EventsPopup" || popup == "CharacterNameEventPopup")
		{
			HideBreadcrumb();
			UserSave.lastEventTypeShown = _activeEventManager.GetEventType();
			UserSave.lastEventShownTimestamp = CDateTime.UtcNow;
		}
	}

	private void CheckForUnseenEventPopup()
	{
		HideBreadcrumb();
		_activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
		if (_activeEventManager != null && _activeEventManager.IsEventActive())
		{
			if (_activeEventManager.GetEventType() != UserSave.lastEventTypeShown)
			{
				ShowBreadcrumb();
			}
			else if ((CDateTime.UtcNow - UserSave.lastEventShownTimestamp).Days >= 5)
			{
				ShowBreadcrumb();
			}
		}
	}

	private void ShowBreadcrumb()
	{
		_breadcrumb.SetActive(true);
	}

	private void HideBreadcrumb()
	{
		_breadcrumb.SetActive(false);
	}
}
