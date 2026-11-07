using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class SuperMysteryBoxPicker : BasePicker, IPicker
	{
		void IPicker.Add(PickupManager pickupManager)
		{
			AddProtected(pickupManager, PickerType.SuperMysteryBox);
		}

		bool IPicker.UpdatePicker()
		{
			UpdatePickerProtected();
			return true;
		}
	}
}
