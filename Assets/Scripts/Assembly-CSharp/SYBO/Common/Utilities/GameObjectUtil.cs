using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Common.Utilities
{
	public static class GameObjectUtil
	{
		private struct HierarchyDepthContainer
		{
			public Component Component;

			public int HierarchyCount;

			public int Index;

			public HierarchyDepthContainer(Component component, int index)
			{
				Component = component;
				HierarchyCount = 0;
				Index = index;
			}
		}

		public static string GetLongName(this Component comp)
		{
			return Utils.GetLongName(comp.transform);
		}

		public static string GetLongName(this GameObject obj)
		{
			return Utils.GetLongName(obj.transform);
		}

		public static GameObject Instantiate(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			return UnityEngine.Object.Instantiate(go);
		}

		public static GameObject Instantiate(Transform transform)
		{
			if (transform == null)
			{
				return null;
			}
			return UnityEngine.Object.Instantiate(transform.gameObject);
		}

		public static Transform InstantiateTransform(GameObject go)
		{
			GameObject gameObject = Instantiate(go);
			return (!(gameObject == null)) ? gameObject.transform : null;
		}

		public static Transform InstantiateTransform(Transform transform)
		{
			GameObject gameObject = Instantiate(transform.gameObject);
			return (!(gameObject == null)) ? gameObject.transform : null;
		}

		public static T AddComponentIfMissing<T>(this Component comp) where T : Component
		{
			return comp.gameObject.AddComponentIfMissing<T>();
		}

		public static T AddComponentIfMissing<T>(this GameObject go) where T : Component
		{
			T val = go.GetComponent<T>();
			if (val == null)
			{
				val = go.AddComponent<T>();
			}
			return val;
		}

		public static Component AddComponentIfMissing(this Component comp, Type type)
		{
			return comp.gameObject.AddComponentIfMissing(type);
		}

		public static Component AddComponentIfMissing(this GameObject go, Type type)
		{
			Component component = go.GetComponent(type);
			if (component == null)
			{
				component = go.AddComponent(type);
			}
			return component;
		}

		public static void ResetLocal(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		public static void ResetLocalPosition(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
		}

		public static void ResetLocalRotation(this Transform transform)
		{
			transform.localRotation = Quaternion.identity;
		}

		public static void ResetLocalScale(this Transform transform)
		{
			transform.localScale = Vector3.one;
		}

		public static void UnparentAndDestroy(this Transform transform)
		{
			transform.parent = null;
			UnityEngine.Object.Destroy(transform.gameObject);
		}

		public static void UnparentAndDestroy(this GameObject go)
		{
			go.transform.parent = null;
			UnityEngine.Object.Destroy(go);
		}

		public static bool IsDestroyed(this GameObject gameObject)
		{
			return gameObject == null && !object.ReferenceEquals(gameObject, null);
		}

		public static int ComparePositionZ(Vector3 lhs, Vector3 rhs)
		{
			if (lhs.z < rhs.z)
			{
				return -1;
			}
			if (lhs.z > rhs.z)
			{
				return 1;
			}
			return 0;
		}

		public static void SortTransformChildren(this Transform parentTransform, Comparison<Transform> cmp)
		{
			Transform[] array = new Transform[parentTransform.childCount];
			for (int i = 0; i < parentTransform.childCount; i++)
			{
				array[i] = parentTransform.GetChild(i);
			}
			Array.Sort(array, cmp);
			for (int j = 0; j < parentTransform.childCount; j++)
			{
				Transform transform = array[j];
				transform.SetSiblingIndex(j);
				transform.SortTransformChildren(cmp);
			}
		}

		public static bool IsPrefab(this GameObject go)
		{
			return false;
		}

		public static void GetComponentsInChildrenBFS<T>(this Component c, bool includeInactive, List<T> result) where T : Component
		{
			c.gameObject.GetComponentsInChildrenBFS(includeInactive, result, true);
		}

		public static void GetComponentsInChildrenBFS<T>(this Component c, bool includeInactive, List<T> result, bool includeSelf) where T : Component
		{
			c.gameObject.GetComponentsInChildrenBFS(includeInactive, result, includeSelf, int.MaxValue);
		}

		public static void GetComponentsInChildrenBFS<T>(this Component c, bool includeInactive, List<T> result, bool includeSelf, int searchDepth) where T : Component
		{
			c.gameObject.GetComponentsInChildrenBFS(includeInactive, result, includeSelf);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this GameObject go) where T : Component
		{
			return go.GetComponentsInChildrenBFS<T>(false, true);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this GameObject go, bool includeInactive) where T : Component
		{
			return go.GetComponentsInChildrenBFS<T>(includeInactive, true);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this Component c) where T : Component
		{
			return c.gameObject.GetComponentsInChildrenBFS<T>(false, true);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this Component c, bool includeInactive) where T : Component
		{
			return c.gameObject.GetComponentsInChildrenBFS<T>(includeInactive, true);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this Component c, bool includeInactive, bool includeSelf) where T : Component
		{
			return c.GetComponentsInChildrenBFS<T>(includeInactive, includeSelf, int.MaxValue);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this Component c, bool includeInactive, bool includeSelf, int searchDepth) where T : Component
		{
			return c.gameObject.GetComponentsInChildrenBFS<T>(includeInactive, includeSelf, searchDepth);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this GameObject go, bool includeInactive, bool includeSelf) where T : Component
		{
			return go.GetComponentsInChildrenBFS<T>(includeInactive, includeSelf, int.MaxValue);
		}

		public static T[] GetComponentsInChildrenBFS<T>(this GameObject go, bool includeInactive, bool includeSelf, int searchDepth) where T : Component
		{
			List<T> list = new List<T>();
			go.GetComponentsInChildrenBFS(includeInactive, list, includeSelf, searchDepth);
			return list.ToArray();
		}

		public static void GetComponentsInChildrenBFS<T>(this GameObject go, bool includeInactive, List<T> result) where T : Component
		{
			go.GetComponentsInChildrenBFS(includeInactive, result, true);
		}

		public static void GetComponentsInChildrenBFS<T>(this GameObject go, bool includeInactive, List<T> result, bool includeSelf) where T : Component
		{
			go.GetComponentsInChildrenBFS(includeInactive, result, includeSelf, int.MaxValue);
		}

		public static void GetComponentsInChildrenBFS<T>(this GameObject go, bool includeInactive, List<T> result, bool includeSelf, int searchDepth) where T : Component
		{
			if (result == null)
			{
				return;
			}
			List<Component> list = new List<Component>();
			go.GetComponentsInChildrenBFS(typeof(T), includeInactive, list, includeSelf, searchDepth);
			foreach (Component item in list)
			{
				result.Add((T)item);
			}
		}

		public static Component[] GetComponentsInChildrenBFS(this Component c, Type type)
		{
			return c.GetComponentsInChildrenBFS(type, false, true);
		}

		public static Component[] GetComponentsInChildrenBFS(this Component c, Type type, bool includeInactive)
		{
			return c.GetComponentsInChildrenBFS(type, includeInactive, true);
		}

		public static Component[] GetComponentsInChildrenBFS(this Component c, Type type, bool includeInactive, bool includeSelf)
		{
			return c.GetComponentsInChildrenBFS(type, includeInactive, includeSelf, int.MaxValue);
		}

		public static Component[] GetComponentsInChildrenBFS(this Component c, Type type, bool includeInactive, bool includeSelf, int searchDepth)
		{
			return c.gameObject.GetComponentsInChildrenBFS(type, includeInactive, includeSelf, searchDepth);
		}

		public static Component[] GetComponentsInChildrenBFS(this GameObject go, Type type)
		{
			return go.GetComponentsInChildrenBFS(type, false, true);
		}

		public static Component[] GetComponentsInChildrenBFS(this GameObject go, Type type, bool includeInactive)
		{
			return go.GetComponentsInChildrenBFS(type, includeInactive, true);
		}

		public static Component[] GetComponentsInChildrenBFS(this GameObject go, Type type, bool includeInactive, bool includeSelf)
		{
			return go.GetComponentsInChildrenBFS(type, includeInactive, includeSelf, int.MaxValue);
		}

		public static Component[] GetComponentsInChildrenBFS(this GameObject go, Type type, bool includeInactive, bool includeSelf, int searchDepth)
		{
			List<Component> list = new List<Component>();
			go.GetComponentsInChildrenBFS(type, includeInactive, list, includeSelf, searchDepth);
			return list.ToArray();
		}

		public static void GetComponentsInChildrenBFS(this GameObject go, Type type, bool includeInactive, List<Component> result, bool includeSelf, int searchDepth)
		{
			if (result == null)
			{
				return;
			}
			Component[] componentsInChildren = go.GetComponentsInChildren(type, includeInactive);
			List<HierarchyDepthContainer> list = new List<HierarchyDepthContainer>(componentsInChildren.Length);
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				if (component.gameObject == go)
				{
					if (includeSelf)
					{
						list.Add(new HierarchyDepthContainer(component, list.Count));
					}
					continue;
				}
				HierarchyDepthContainer item = new HierarchyDepthContainer(component, list.Count);
				Transform transform = component.transform;
				do
				{
					item.HierarchyCount++;
					transform = transform.parent;
				}
				while (transform != go.transform);
				if (item.HierarchyCount <= searchDepth)
				{
					list.Add(item);
				}
			}
			list.Sort((HierarchyDepthContainer x, HierarchyDepthContainer y) => (x.HierarchyCount == y.HierarchyCount) ? x.Index.CompareTo(y.Index) : x.HierarchyCount.CompareTo(y.HierarchyCount));
			for (int num = 0; num < list.Count; num++)
			{
				result.Add(list[num].Component);
			}
		}
	}
}
