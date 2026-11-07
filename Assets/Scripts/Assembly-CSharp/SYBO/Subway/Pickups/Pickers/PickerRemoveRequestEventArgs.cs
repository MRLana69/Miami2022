namespace SYBO.Subway.Pickups.Pickers
{
	public class PickerRemoveRequestEventArgs
	{
		public IPicker IPicker { get; private set; }

		public PickerRemoveRequestEventArgs(IPicker iPicker)
		{
			IPicker = iPicker;
		}
	}
}
