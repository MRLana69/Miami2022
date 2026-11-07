public interface IEventManager
{
	SSEventType GetEventType();

	long GetEventID();

	bool IsEventActive();

	T GetEventValue<T>(out bool found);

	void GetPopupTitle(out string firstLine, out string secondLine);

	void GetPopupDescription(out string firstTitle, out string secondTitle);

	void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine);

	void SaveProgress();
}
