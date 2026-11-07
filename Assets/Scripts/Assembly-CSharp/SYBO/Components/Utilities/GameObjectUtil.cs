using System;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	public static class GameObjectUtil
	{
		public static T GetOrAddComponent<T>(GameObject target) where T : Component
		{
			return GetOrAddComponent<T>(target, false);
		}

		public static T GetOrAddComponent<T>(GameObject target, bool willForceHideInInspector) where T : Component
		{
			return GetOrAddComponent(typeof(T), target, willForceHideInInspector) as T;
		}

		public static Component GetOrAddComponent(Type type, GameObject target, bool willForceHideInInspector)
		{
			if (type == null || target == null)
			{
				return null;
			}
			Component component = target.GetComponent(type);
			if (component == null)
			{
				component = target.AddComponent(type);
				if (component != null && willForceHideInInspector)
				{
					component.hideFlags = HideFlags.HideInInspector;
				}
			}
			return component;
		}
	}
}
