using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	public class RouteSectionGroup : MonoBehaviour
	{
		[SerializeField]
		private List<WeightedRouteSection> _routeSections = new List<WeightedRouteSection>();

		public List<WeightedRouteSection> RouteSections
		{
			get
			{
				return _routeSections;
			}
		}
	}
}
