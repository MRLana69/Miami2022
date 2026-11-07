namespace NaughtyAttributes
{
	public abstract class MetaAttribute : NaughtyAttribute
	{
		private int order;

		public int Order
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
			}
		}
	}
}
