using System.Collections.Generic;
using SYBO.Common.Utilities;
using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes
{
	public class RouteCollection
	{
		private List<Route> _routes = new List<Route>();

		private bool _hasAddedRouteSinceUpdate;

		public List<Route> Routes
		{
			get
			{
				return _routes;
			}
		}

		public RouteCollection(RouteManager routeManager)
		{
		}

		public void Add(Route route)
		{
			_routes.Add(route);
			_hasAddedRouteSinceUpdate = true;
		}

		private int SortRoutes(Route aRoute, Route bRoute)
		{
			int num = -1;
			int num2 = 0;
			RouteBuilder builder = aRoute.Builder;
			RouteBuilder builder2 = bRoute.Builder;
			if (builder.IsDone)
			{
				num2 = -num;
			}
			else if (builder2.IsDone)
			{
				num2 = num;
			}
			if (num2 != 0)
			{
				return num2;
			}
			num2 = num * (builder.SpawnedLeadingEdgeGamePositionZ - builder2.SpawnedLeadingEdgeGamePositionZ);
			IConstrainingRouteBuilder constrainingRouteBuilder = builder as IConstrainingRouteBuilder;
			IConstrainingRouteBuilder constrainingRouteBuilder2 = builder2 as IConstrainingRouteBuilder;
			bool flag = constrainingRouteBuilder != null && builder2.ConstrainingBuilders.Contains(constrainingRouteBuilder);
			if (flag)
			{
				num2 = -num;
			}
			bool flag2 = constrainingRouteBuilder2 != null && builder.ConstrainingBuilders.Contains(constrainingRouteBuilder2);
			if (flag2)
			{
				num2 = num;
			}
			if (flag && flag2)
			{
				num2 = 0;
				SDebug.LogError(string.Concat("Got builders constraining each other: ", builder, " AND ", builder2));
			}
			return num2;
		}

		public void Remove(Route route)
		{
			if (_routes.Contains(route))
			{
				_routes.Remove(route);
				route.Builder.Destruct();
				Object.Destroy(route.gameObject);
			}
		}

		public void RemoveAll()
		{
			int num = _routes.Count - 1;
			while (0 <= num)
			{
				Remove(_routes[num]);
				num--;
			}
		}

		public Route GetRouteContainingPoint(Vector3 gamePosition)
		{
			Route result = null;
			for (int i = 0; i < _routes.Count; i++)
			{
				Route route = _routes[i];
				if (route.Builder.Bounds.Contains(gamePosition))
				{
					result = route;
					break;
				}
			}
			return result;
		}

		public Route GetRouteContainingBounds(Bounds bounds)
		{
			Route result = null;
			foreach (Route route in _routes)
			{
				if (route.Builder.Bounds.Intersects(bounds))
				{
					result = route;
					break;
				}
			}
			return result;
		}

		public void Update(float characterGamePositionZ, SyboRandom random)
		{
			if (_hasAddedRouteSinceUpdate)
			{
				_routes.Sort(SortRoutes);
				_hasAddedRouteSinceUpdate = false;
			}
			int num = _routes.Count - 1;
			while (0 <= num)
			{
				_routes[num].UpdateRoute(characterGamePositionZ, random);
				num--;
			}
			int num2 = _routes.Count - 1;
			while (0 <= num2)
			{
				RouteBuilder builder = _routes[num2].Builder;
				if (builder.IsDone && builder.IsCleanedUp)
				{
					Remove(_routes[num2]);
				}
				num2--;
			}
		}

		private void DrawBounds(Bounds bounds)
		{
			DrawBoundsInternal(bounds, null, Color.red, 0.25f);
		}

		private void DrawBounds(Bounds bounds, Color color)
		{
			DrawBoundsInternal(bounds, null, color, 0.25f);
		}

		private void DrawBounds(Bounds bounds, Color color, float duration)
		{
			DrawBoundsInternal(bounds, null, color, duration);
		}

		private void DrawBounds(Bounds bounds, Transform theTransform, Color color, float duration = 0f)
		{
			DrawBoundsInternal(bounds, theTransform, color, duration);
		}

		private void DrawBoundsInternal(Bounds bounds, Transform theTransform, Color color, float duration = 0f)
		{
			Vector3 extents = bounds.extents;
			Vector3 center = bounds.center;
			Vector3 vector = new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z);
			Vector3 vector2 = new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z);
			Vector3 vector3 = new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z);
			Vector3 vector4 = new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z);
			Vector3 vector5 = new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z);
			Vector3 vector6 = new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z);
			Vector3 vector7 = new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z);
			Vector3 vector8 = new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z);
			if (theTransform != null)
			{
				vector = theTransform.TransformPoint(vector);
				vector2 = theTransform.TransformPoint(vector2);
				vector3 = theTransform.TransformPoint(vector3);
				vector4 = theTransform.TransformPoint(vector4);
				vector5 = theTransform.TransformPoint(vector5);
				vector6 = theTransform.TransformPoint(vector6);
				vector7 = theTransform.TransformPoint(vector7);
				vector8 = theTransform.TransformPoint(vector8);
			}
			Debug.DrawLine(vector, vector2, color, duration);
			Debug.DrawLine(vector2, vector4, color, duration);
			Debug.DrawLine(vector4, vector3, color, duration);
			Debug.DrawLine(vector3, vector, color, duration);
			Debug.DrawLine(vector5, vector6, color, duration);
			Debug.DrawLine(vector6, vector8, color, duration);
			Debug.DrawLine(vector8, vector7, color, duration);
			Debug.DrawLine(vector7, vector5, color, duration);
			Debug.DrawLine(vector, vector5, color, duration);
			Debug.DrawLine(vector2, vector6, color, duration);
			Debug.DrawLine(vector4, vector8, color, duration);
			Debug.DrawLine(vector3, vector7, color, duration);
		}
	}
}
