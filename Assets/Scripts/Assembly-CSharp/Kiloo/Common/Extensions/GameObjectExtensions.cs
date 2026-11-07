using UnityEngine;

namespace Kiloo.Common.Extensions
{
	public static class GameObjectExtensions
	{
		public static GameObject Duplicate(this GameObject gameObject)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject);
			gameObject2.transform.position = gameObject.transform.position;
			gameObject2.transform.parent = gameObject.transform.parent;
			gameObject2.transform.localScale = gameObject.transform.localScale;
			gameObject2.transform.rotation = gameObject.transform.rotation;
			return gameObject2;
		}

		public static void SetLayerRecursively(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			foreach (Transform item in gameObject.transform)
			{
				item.gameObject.SetLayerRecursively(layer);
			}
		}

		public static void SetLayerRecursively(this GameObject gameObject, int layer, int layerToSkip)
		{
			if (gameObject.layer != layerToSkip)
			{
				gameObject.layer = layer;
			}
			foreach (Transform item in gameObject.transform)
			{
				item.gameObject.SetLayerRecursively(layer, layerToSkip);
			}
		}

		public static T GetOrAdd<T>(this GameObject go) where T : Component
		{
			T component = go.GetComponent<T>();
			return (!component) ? go.AddComponent<T>() : component;
		}

		public static T GetComponentInParents<T>(this GameObject gameObject) where T : Component
		{
			Transform transform = gameObject.transform;
			while (transform != null)
			{
				T component = transform.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				transform = transform.parent;
			}
			return (T)null;
		}

		public static void Blink(this GameObject go)
		{
			if (!Application.isPlaying)
			{
				go.SetActive(false);
				go.SetActive(true);
			}
		}
	}
}
