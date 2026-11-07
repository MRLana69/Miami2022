using System;
using SYBO.Subway.Routes.Builders;

namespace SYBO.Subway.Routes
{
	[Serializable]
	public class DeterministicRouteBuilder : DefaultRouteBuilder
	{
		private int _midSectionIndex;

		protected override RouteSection GetNextSectionPrefab()
		{
			RouteSection prefab = _midSectionPrefabs[_midSectionIndex++].Prefab;
			if (_midSectionIndex >= _midSectionPrefabs.Count)
			{
				_midSectionIndex = 0;
			}
			return prefab;
		}

		protected override bool CanSpawnSection(RouteSection section)
		{
			return true;
		}
	}
}
