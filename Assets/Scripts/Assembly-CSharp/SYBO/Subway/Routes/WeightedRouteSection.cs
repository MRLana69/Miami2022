using System;

namespace SYBO.Subway.Routes
{
	[Serializable]
	public class WeightedRouteSection : WeightedPrefab<RouteSection>
	{
		public WeightedRouteSection(RouteSection prefab, int weight)
			: base(prefab, weight)
		{
		}
	}
}
