namespace SYBO.Subway.Pickups.Pickers
{
	public class PickerRemovedEventArgs : PickerRemoveRequestEventArgs
	{
		public PickerRemovedEventArgs(IPicker iPicker)
			: base(iPicker)
		{
		}
	}
}
