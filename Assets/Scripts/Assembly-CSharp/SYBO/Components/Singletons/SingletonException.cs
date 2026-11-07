using System;

namespace SYBO.Components.Singletons
{
	public class SingletonException : Exception
	{
		public SingletonException(string message)
			: base(message)
		{
		}
	}
}
