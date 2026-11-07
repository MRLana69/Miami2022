using System;

namespace SYBO.Subway.Routes
{
	[Serializable]
	public class WeightedRouteChunk : WeightedPrefab<RouteChunk>
	{
		public WeightedRouteChunk(RouteChunk prefab, int weight)
			: base(prefab, weight)
		{
		}
	}
}
