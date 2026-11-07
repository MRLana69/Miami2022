using System;

namespace SYBO.Components.Utilities.Exceptions
{
	public class SwitchDefaultException : Exception
	{
		public SwitchDefaultException(string missingValue)
			: base(string.Format("Add case for '{0}'.", missingValue))
		{
		}

		public static void Throw(object missingValue)
		{
			throw new SwitchDefaultException(missingValue.ToString());
		}
	}
}
