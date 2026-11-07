using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DropdownAttribute : DrawerAttribute
	{
		private string valuesFieldName;

		public string ValuesFieldName
		{
			get
			{
				return valuesFieldName;
			}
		}

		public DropdownAttribute(string valuesFieldName)
		{
			this.valuesFieldName = valuesFieldName;
		}
	}
}
