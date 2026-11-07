using System;

namespace SYBO.Common.Exceptions
{
	public class SwitchDefaultException : Exception
	{
		public SwitchDefaultException(string missingSwitchValue)
			: base("SwitchDefaultException: Add case for '" + missingSwitchValue + "'.")
		{
		}

		public static void Throw(string missingSwitchValue)
		{
			throw new SwitchDefaultException(missingSwitchValue);
		}
	}
}
