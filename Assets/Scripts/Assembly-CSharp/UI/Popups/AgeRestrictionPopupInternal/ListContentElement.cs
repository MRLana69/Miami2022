namespace UI.Popups.AgeRestrictionPopupInternal
{
	public class ListContentElement
	{
		public string content { get; private set; }

		public int value { get; private set; }

		public ListElementHelper helper { get; set; }

		public ListContentElement(string content, int value)
		{
			this.content = content;
			this.value = value;
		}
	}
}
