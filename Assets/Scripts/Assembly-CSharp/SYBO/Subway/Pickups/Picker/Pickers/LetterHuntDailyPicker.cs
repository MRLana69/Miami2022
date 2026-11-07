using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class LetterHuntDailyPicker : BasePicker, IPicker
	{
		void IPicker.Add(PickupManager pickupManager)
		{
			AddProtected(pickupManager, PickerType.LetterHuntDaily);
		}

		bool IPicker.UpdatePicker()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntDaily);
			if (pickupDataForPickupType.PickupEnabledState == PickupEnabledState.Enabled)
			{
				UpdatePickerProtected();
				return false;
			}
			return true;
		}
	}
}
