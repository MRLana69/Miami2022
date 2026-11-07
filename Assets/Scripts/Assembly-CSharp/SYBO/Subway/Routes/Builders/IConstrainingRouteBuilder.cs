using System.Collections.Generic;

namespace SYBO.Subway.Routes.Builders
{
	public interface IConstrainingRouteBuilder
	{
		int ConstraintStartZ { get; }

		int ConstraintEndZ { get; }

		ICollection<RouteType> ConstrainedRouteTypes { get; }

		ICollection<RouteBuilder> ConstrainedBuilders { get; }

		RouteConstraint GetConstraint(RouteType routeType);

		bool IsConstrainingStartAt(int positionZ);

		bool IsConstrainingEndAt(int positionZ);

		bool IsConstrainingRange(int startZ, int endZ);
	}
}
