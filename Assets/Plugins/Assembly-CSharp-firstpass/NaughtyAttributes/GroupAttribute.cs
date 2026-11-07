namespace NaughtyAttributes
{
	public abstract class GroupAttribute : NaughtyAttribute
	{
		private string name;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public GroupAttribute(string name)
		{
			this.name = name;
		}
	}
}
