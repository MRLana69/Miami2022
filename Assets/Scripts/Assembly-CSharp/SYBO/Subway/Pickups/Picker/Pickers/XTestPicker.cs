using System.Collections.Generic;
using SYBO.Subway.Pickups.Pickers;

namespace SYBO.Subway.Pickups.Picker.Pickers
{
	public class XTestPicker : BasePicker, IPicker
	{
		private int _temp_totalSpawnByThisPicker;

		void IPicker.Add(PickupManager pickupManager)
		{
			AddProtected(pickupManager, PickerType.XTest);
		}

		bool IPicker.UpdatePicker()
		{
			return MakeManyAndDontLetOtherPickersHaveATurn();
		}

		private bool MakeManyAndDontLetOtherPickersHaveATurn()
		{
			UpdatePickerProtected();
			return false;
		}

		private bool TestToJustMake5()
		{
			List<PickupSpawnPoint> list = new List<PickupSpawnPoint>();
			UpdatePickerProtected(list);
			foreach (PickupSpawnPoint item in list)
			{
				if (item.SpawnPointMode != SpawnPointMode.WillForcePickupType)
				{
					_temp_totalSpawnByThisPicker++;
				}
			}
			return false;
		}
	}
}
