using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	public interface IMoveObjectActivityInitializer : IActivityInitializer
	{
		Transform Transform { get; }

		float Speed { get; }

		RouteChunk RouteChunk { get; }
	}
}
