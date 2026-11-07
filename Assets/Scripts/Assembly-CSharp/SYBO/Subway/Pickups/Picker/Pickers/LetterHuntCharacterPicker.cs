using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class LetterHuntCharacterPicker : BasePicker, IPicker
	{
		void IPicker.Add(PickupManager pickupManager)
		{
			AddProtected(pickupManager, PickerType.LetterHuntCharacter);
		}

		bool IPicker.UpdatePicker()
		{
			PickupData pickupDataForPickupType = _pickupManager.GetPickupDataForPickupType(PickupType.LetterHuntCharacter);
			if (pickupDataForPickupType.PickupEnabledState == PickupEnabledState.Enabled)
			{
				UpdatePickerProtected();
			}
			return true;
		}
	}
}
