using UnityEngine;

namespace SYBO.Components.Analytics
{
	public class UnityRandomGen : IRandomGen
	{
		public uint NextUint()
		{
			return (uint)Random.Range(int.MinValue, int.MaxValue);
		}

		public float NextFloat()
		{
			return Random.value;
		}
	}
}
