using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	[Serializable]
	public class SingletonMonoBehaviourManager : ScriptableObject
	{
		private static SingletonMonoBehaviourManager _instance;

		private List<SingletonMonoBehaviour> Singletons = new List<SingletonMonoBehaviour>();

		public static SingletonMonoBehaviourManager Instance
		{
			get
			{
				if (!IsInstantiated())
				{
					_instance = UnityEngine.Object.FindObjectOfType<SingletonMonoBehaviourManager>();
					if (!IsInstantiated())
					{
						_instance = ScriptableObject.CreateInstance<SingletonMonoBehaviourManager>();
					}
					if (!IsInstantiated())
					{
						throw new Exception(string.Format("Instance.get({0}) This cannot be null.", _instance));
					}
				}
				return _instance;
			}
		}

		public static bool IsInstantiated()
		{
			return _instance != null;
		}

		public int GetSingletonCount<T>() where T : SingletonMonoBehaviour
		{
			return Singletons.OfType<T>().Count();
		}

		public int GetSingletonCount()
		{
			return Singletons.Count;
		}

		private void OnEnable()
		{
			CheckInstance();
		}

		protected void OnDisable()
		{
			CheckInstance();
		}

		private static void CheckInstance()
		{
			if (!IsInstantiated())
			{
				_instance = Instance;
			}
			_instance.RemoveAnyNullOrMissing();
		}

		private void RemoveAnyNullOrMissing()
		{
			for (int num = Singletons.Count - 1; num >= 0; num--)
			{
				SingletonMonoBehaviour singletonMonoBehaviour = Singletons[num];
				if (singletonMonoBehaviour == null || singletonMonoBehaviour.gameObject == null)
				{
					SingletonMonoBehaviour.LogWarningFormat("Singletons.Remove at {0} because its null", num);
					Singletons.RemoveAt(num);
				}
			}
		}

		internal SingletonMonoBehaviour FindSingleton(GameObject go)
		{
			RemoveAnyNullOrMissing();
			SingletonMonoBehaviour result = null;
			for (int num = Singletons.Count - 1; num >= 0; num--)
			{
				SingletonMonoBehaviour singletonMonoBehaviour = Singletons[num];
				if (singletonMonoBehaviour.gameObject == go)
				{
					result = singletonMonoBehaviour;
					break;
				}
			}
			return result;
		}

		public void LogSingletons()
		{
			SingletonMonoBehaviour.LogFormat(0, "LogSingletons() Count:{0}", GetSingletonCount());
			foreach (SingletonMonoBehaviour singleton in Singletons)
			{
				SingletonMonoBehaviour.LogFormat(0, "\t{0}", singleton);
			}
		}

		internal bool HasSingleton(GameObject go)
		{
			return FindSingleton(go) != null;
		}

		internal T FindSingleton<T>() where T : SingletonMonoBehaviour
		{
			RemoveAnyNullOrMissing();
			T result = (T)null;
			Type typeFromHandle = typeof(T);
			for (int num = Singletons.Count - 1; num >= 0; num--)
			{
				SingletonMonoBehaviour singletonMonoBehaviour = Singletons[num];
				if (singletonMonoBehaviour.GetType() == typeFromHandle)
				{
					result = (T)singletonMonoBehaviour;
					break;
				}
			}
			return result;
		}

		internal bool HasSingleton<T>() where T : SingletonMonoBehaviour
		{
			return FindSingleton<T>();
		}

		private bool IsSingletonUniqueByType(SingletonMonoBehaviour singleton)
		{
			bool result = true;
			for (int num = Singletons.Count - 1; num >= 0; num--)
			{
				SingletonMonoBehaviour singletonMonoBehaviour = Singletons[num];
				if (singletonMonoBehaviour != singleton && singletonMonoBehaviour.GetType() == singleton.GetType())
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public void RegisterSingleton(SingletonMonoBehaviour singleton)
		{
			if (singleton != null)
			{
				RemoveAnyNullOrMissing();
				Singletons.Add(singleton);
				return;
			}
			throw new Exception(string.Format("RegisterSingleton({0}) This cannot be null.", _instance));
		}

		public void UnregisterSingleton(SingletonMonoBehaviour singleton)
		{
			Singletons.Remove(singleton);
		}

		public void UnregisterAndDestroySingleton(SingletonMonoBehaviour singleton)
		{
			SingletonMonoBehaviour.LogFormat(1, "UnregisterAndDestroySingleton() Destroy {0}", singleton);
			UnregisterSingleton(singleton);
			if (!(singleton == null))
			{
				DestroySingleton(singleton);
				if (IsInstantiated() && Instance.Singletons.Contains(singleton))
				{
					SingletonMonoBehaviour.LogWarningFormat("\tUnregisterAndDestroySingleton() Destroyed, yet now isFound={0}. Why?", singleton.IsSavedInHierarchy);
				}
			}
		}

		internal void DestroySingleton(SingletonMonoBehaviour singletonToDestroy)
		{
			if (Application.isPlaying)
			{
				SingletonMonoBehaviour.LogFormat(1, "About To... Destroy({0}.gameObject);", singletonToDestroy);
				singletonToDestroy.IsDestroyed = true;
				UnityEngine.Object.Destroy(singletonToDestroy.gameObject);
			}
		}
	}
}
