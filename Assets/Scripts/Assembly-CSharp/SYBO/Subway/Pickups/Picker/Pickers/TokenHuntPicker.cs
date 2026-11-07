using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class TokenHuntPicker : BasePicker, IPicker
	{
		void IPicker.Add(PickupManager pickupManager)
		{
			AddProtected(pickupManager, PickerType.TokenHunt);
		}

		bool IPicker.UpdatePicker()
		{
			UpdatePickerProtected();
			return true;
		}
	}
}
