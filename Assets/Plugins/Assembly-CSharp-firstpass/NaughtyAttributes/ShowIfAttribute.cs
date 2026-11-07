using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ShowIfAttribute : DrawConditionAttribute
	{
		private string conditionName;

		public string ConditionName
		{
			get
			{
				return conditionName;
			}
		}

		public ShowIfAttribute(string conditionName)
		{
			this.conditionName = conditionName;
		}
	}
}
