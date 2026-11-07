using SYBO.Common.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	public abstract class AlternateRoute : Route
	{
		protected Game _game;

		public abstract CharacterRendering.ICustomAnimationClipList AlternateRouteAnimationClipList { get; }

		public override void Initialize(RouteManager routeManager, Vector3 originGamePosition, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, RouteType previousRouteType, SyboRandom randomGenerator)
		{
			base.Initialize(routeManager, originGamePosition, sectionSpawnMode, chunkBuildMode, previousRouteType, randomGenerator);
			_game = Game.Instance;
		}
	}
}
