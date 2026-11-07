using System;

namespace SYBO.Components.Singletons
{
	[Serializable]
	public class SampleSingleton1Runtime : Singleton<SampleSingleton1Runtime>
	{
		private SampleSingleton1Runtime()
		{
		}
	}
}
