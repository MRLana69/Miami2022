using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class DefaultPicker : BasePicker, IPicker
	{
		void IPicker.Add(PickupManager pickupManager)
		{
			AddProtected(pickupManager, PickerType.Default);
		}

		bool IPicker.UpdatePicker()
		{
			UpdatePickerProtected();
			return true;
		}
	}
}
