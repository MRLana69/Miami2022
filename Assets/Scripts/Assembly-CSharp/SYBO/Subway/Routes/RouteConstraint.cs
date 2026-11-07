using SYBO.Subway.Level;

namespace SYBO.Subway.Routes
{
	public struct RouteConstraint
	{
		public readonly EnvironmentKind[] AllowedEnvironmentKinds;

		public static RouteConstraint None = new RouteConstraint(new EnvironmentKind[0]);

		public RouteConstraint(EnvironmentKind[] allowedEnvironmentKinds)
		{
			AllowedEnvironmentKinds = allowedEnvironmentKinds;
		}
	}
}
