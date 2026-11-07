using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class RequiredAttribute : ValidatorAttribute
	{
		private string message;

		public string Message
		{
			get
			{
				return message;
			}
		}

		public RequiredAttribute(string message = null)
		{
			this.message = message;
		}
	}
}
