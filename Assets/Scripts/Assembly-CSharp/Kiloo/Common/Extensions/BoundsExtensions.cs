using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class BoundsExtensions
	{
		public static void SetWidth(this Bounds bound, float width)
		{
			Vector3 extents = bound.extents;
			extents.x = width / 2f;
			bound.extents = extents;
		}

		public static void SetHeight(this Bounds bound, float height)
		{
			Vector3 extents = bound.extents;
			extents.y = height / 2f;
			bound.extents = extents;
		}

		public static int GetWidth(this Bounds bound)
		{
			return (int)bound.extents.x * 2;
		}

		public static int GetHeight(this Bounds bound)
		{
			return (int)bound.extents.y * 2;
		}
	}
}
