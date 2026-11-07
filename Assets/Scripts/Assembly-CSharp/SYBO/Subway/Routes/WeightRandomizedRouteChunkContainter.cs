using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	[Serializable]
	public class WeightRandomizedRouteChunkContainter : WeightRandomizedPrefabContainter<WeightedRouteChunk, RouteChunk>
	{
		public int GetMaxLength()
		{
			int num = 0;
			using (IEnumerator<WeightedRouteChunk> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					WeightedRouteChunk current = enumerator.Current;
					if (current.Prefab != null)
					{
						num = Mathf.Max(num, current.Prefab.Length);
					}
				}
				return num;
			}
		}

		public int GetMinLength()
		{
			int num = int.MaxValue;
			using (IEnumerator<WeightedRouteChunk> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					WeightedRouteChunk current = enumerator.Current;
					if (current.Prefab != null)
					{
						num = ((num != 0) ? Mathf.Min(num, current.Prefab.Length) : current.Prefab.Length);
					}
				}
			}
			return (num != int.MaxValue) ? num : 0;
		}
	}
}
