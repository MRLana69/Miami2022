using UnityEngine;

namespace SYBO.Common.Utilities
{
	public struct GameObjectPositionWrapper
	{
		public GameObject GameObject;

		public Vector3 Position;

		public bool ParentIsSelector;

		public GameObjectPositionWrapper(GameObject gameObject, bool parentIsSelector = false)
		{
			GameObject = gameObject;
			Position = gameObject.transform.position;
			ParentIsSelector = parentIsSelector;
		}

		public GameObjectPositionWrapper(Component component, bool parentIsSelector = false)
		{
			GameObject = component.gameObject;
			Position = GameObject.transform.position;
			ParentIsSelector = parentIsSelector;
		}
	}
}
