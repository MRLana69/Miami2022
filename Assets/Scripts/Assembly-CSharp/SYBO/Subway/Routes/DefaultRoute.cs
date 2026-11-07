using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	public class DefaultRoute : Route
	{
		[SerializeField]
		private DefaultRouteBuilder _defaultRouteBuilder;

		public override RouteBuilder Builder
		{
			get
			{
				return _defaultRouteBuilder;
			}
		}

		public override RouteType Type
		{
			get
			{
				return RouteType.Default;
			}
		}
	}
}
