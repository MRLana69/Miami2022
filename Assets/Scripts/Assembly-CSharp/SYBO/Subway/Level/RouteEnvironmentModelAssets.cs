using System;
using SYBO.Subway.Routes;

namespace SYBO.Subway.Level
{
	[Serializable]
	public class RouteEnvironmentModelAssets
	{
		public RouteCategory RouteCategory;

		public EnvironmentModelAssets[] EnvironmentAssets;
	}
}
