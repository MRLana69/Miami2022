using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	public class DeterministicRoute : Route
	{
		[SerializeField]
		private DeterministicRouteBuilder _deterministicRouteBuilder;

		public override RouteType Type
		{
			get
			{
				return RouteType.Default;
			}
		}

		public override RouteBuilder Builder
		{
			get
			{
				return _deterministicRouteBuilder;
			}
		}
	}
}
