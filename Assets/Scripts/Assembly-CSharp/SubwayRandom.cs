using Kiloo;
using UnityEngine;

public static class SubwayRandom
{
	public static long DateTimeTicksRandomOffset()
	{
		return CDateTime.Now.Ticks + Random.Range(0, int.MaxValue);
	}
}
