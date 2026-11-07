using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class ComponentExtensions
	{
		public static T AddComponent<T>(this Component component) where T : Component
		{
			return component.gameObject.AddComponent<T>();
		}

		public static T GetOrAdd<T>(this Component c) where T : Component
		{
			return c.gameObject.GetOrAdd<T>();
		}
	}
}
