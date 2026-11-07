using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class HideIfAttribute : DrawConditionAttribute
	{
		private string conditionName;

		public string ConditionName
		{
			get
			{
				return conditionName;
			}
		}

		public HideIfAttribute(string conditionName)
		{
			this.conditionName = conditionName;
		}
	}
}
