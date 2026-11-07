using UnityEngine;

namespace SYBO.Common.Utilities
{
	public struct ComponentPositionWrapper<T> where T : Component
	{
		public T Component;

		public Vector3 Position;

		public ComponentPositionWrapper(T component)
		{
			Component = component;
			Position = component.gameObject.transform.position;
		}
	}
}
