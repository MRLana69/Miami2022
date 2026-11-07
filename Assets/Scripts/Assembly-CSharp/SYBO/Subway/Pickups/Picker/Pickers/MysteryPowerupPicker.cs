using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class MysteryPowerupPicker : BasePicker, IPicker
	{
		void IPicker.Add(PickupManager pickupManager)
		{
			AddProtected(pickupManager, PickerType.MysteryPowerUp);
		}

		bool IPicker.UpdatePicker()
		{
			UpdatePickerProtected();
			return true;
		}
	}
}
