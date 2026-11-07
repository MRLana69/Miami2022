using System;
using System.Reflection;
using SYBO.Components.Settings;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	public abstract class SingletonMonoBehaviour : MonoBehaviour
	{
		public static string LogCategoryName = "Utilities";

		public int ID
		{
			get
			{
				return base.gameObject.transform.GetInstanceID();
			}
		}

		public bool IsDestroyed { get; internal set; }

		public virtual bool IsSavedInHierarchy
		{
			get
			{
				if (base.gameObject == null)
				{
					return false;
				}
				return (base.gameObject.hideFlags & HideFlags.HideAndDontSave) != HideFlags.HideAndDontSave;
			}
			set
			{
				if (value)
				{
					base.gameObject.hideFlags &= ~HideFlags.HideAndDontSave;
				}
				else
				{
					base.gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				LogFormat(1, "Just Set IsSavedInHierarchy={0} which is the same as flags={1}", value, base.gameObject.hideFlags & HideFlags.HideAndDontSave);
			}
		}

		public static bool IsShuttingDown
		{
			get
			{
				return false;
			}
		}

		public abstract SingletonMonoBehaviour GetInstance();

		public static GameObject InstantiateGameObjectFromPath(string path)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			if (path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
			{
				string text = path;
				path = path.Substring(0, path.Length - 7);
				LogWarningFormat("Singleon.InstantiateFromPath({1}) The path of '{1}' has been automatically corrected to path '{0}' for proper functionality. Please it yourself and recompile.", path, text, gameObject2);
			}
			gameObject = Resources.Load<GameObject>(path);
			if (gameObject != null)
			{
				gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			}
			else
			{
				LogErrorFormat("Singleon.InstantiateFromPath({0}) Path is not valid. Move prefab within any 'Resources' folder and provide relative path within which excludes 'Resources'.", path, gameObject2);
			}
			return gameObject2;
		}

		public static void LogFormat(int level, string format, params object[] args)
		{
			if (level <= StaticSettings.Log.GetLogLevel(LogCategoryName))
			{
				Debug.LogFormat(format, args);
			}
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			Debug.LogWarningFormat(format, args);
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			Debug.LogErrorFormat(format, args);
		}
	}
	public abstract class SingletonMonoBehaviour<T> : SingletonMonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		private bool _isSingletonInitialized;

		private bool _isSingletonInitializing;

		private static PrefabPathAttribute _attribute;

		private static T _instance;

		internal bool IsInitialized
		{
			get
			{
				return _isSingletonInitialized;
			}
			set
			{
				_isSingletonInitialized = value;
			}
		}

		public static T Instance
		{
			get
			{
				if (!IsInstantiated())
				{
					if (HasPrefabPathAttribute())
					{
						_attribute = GetPrefabPathAttribute();
						string path = _attribute.Path;
						GameObject gameObject = SingletonMonoBehaviour.InstantiateGameObjectFromPath(path);
						if (gameObject == null)
						{
							SingletonMonoBehaviour.LogErrorFormat("Singleton<{0}>.Instance - Prefab not found: '{0}' looking for '{1}'", typeof(T), path);
							return (T)null;
						}
						if (_instance == null)
						{
							SingletonMonoBehaviour.LogErrorFormat("Singleton<{0}>.Instance - Instantiated prefab '{1}' but didn't get any '{0}'", typeof(T), path);
							return (T)null;
						}
						SingletonMonoBehaviour.LogFormat(1, "Singleton<{0}>.Instance - Instantiating from prefab '{1}'", typeof(T), path);
						_instance.IsSavedInHierarchy = false;
						_instance.BaseInitialization();
						return _instance;
					}
					MethodInfo method = ReflectionWrapper.GetMethod(typeof(T), "Construct", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					Instance = method.Invoke(null, new object[1] { typeof(T) }) as T;
					if (_instance == null)
					{
						SingletonMonoBehaviour.LogErrorFormat("Singleton<{0}>.Instance - invoked Construct but didn't return a {0}", typeof(T));
						return (T)null;
					}
					SingletonMonoBehaviour.LogFormat(1, "Singleton<{0}>.Instance - invoked Construct to get an instance", typeof(T));
					_instance.IsSavedInHierarchy = false;
					_instance.BaseInitialization();
					return _instance;
				}
				SingletonMonoBehaviour.LogFormat(2, "Singleton<{0}>.Instance - Retrieved", typeof(T));
				_instance.BaseInitialization();
				return _instance;
			}
			protected set
			{
				SingletonMonoBehaviour.LogFormat(1, "Instance setting to {0} ", value);
				if (value == null)
				{
					_instance = (T)null;
				}
				else if (_instance == value)
				{
					if (!object.ReferenceEquals(_instance, value))
					{
						SingletonMonoBehaviour.LogFormat(2, "Singleton<{0}>.Instance - Updated assignment after reload", typeof(T));
						SingletonMonoBehaviourManager.Instance.UnregisterSingleton(_instance);
						_instance = value;
						SingletonMonoBehaviourManager.Instance.RegisterSingleton(_instance);
					}
					else
					{
						SingletonMonoBehaviour.LogFormat(2, "Singleton<{0}>.Instance - Identical assignment from code", typeof(T));
					}
				}
				else
				{
					if (_instance != null)
					{
						throw new Exception(string.Format("Attempting to set the {0}.Instance it has already been set.", typeof(T).Name));
					}
					SingletonMonoBehaviour.LogFormat(1, "Singleton<{0}>.Instance - Assigned from code", typeof(T));
					_instance = value;
					SingletonMonoBehaviourManager.Instance.RegisterSingleton(_instance);
				}
			}
		}

		public override SingletonMonoBehaviour GetInstance()
		{
			return Instance;
		}

		public static T Instantiate()
		{
			return Instance;
		}

		protected void Awake()
		{
			BaseInitialization();
		}

		protected T BaseInitialization()
		{
			if (!_isSingletonInitializing && !_isSingletonInitialized)
			{
				_isSingletonInitializing = true;
				Initialization();
				SubclassInitialize();
				_isSingletonInitializing = false;
			}
			return _instance;
		}

		private void Initialization()
		{
			SingletonMonoBehaviour.LogFormat(3, "Start of... Singleton<{0}>.Initialization()", this);
			if (!IsInitialized)
			{
				IsInitialized = true;
				SingletonMonoBehaviour.LogFormat(2, "Singleton<{0}>.Initialization - IsInitialized={1}", this, IsInitialized);
				if (Application.isPlaying)
				{
					UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				}
				SingletonMonoBehaviour.LogFormat(2, "CheckInstance() Before. _instance:{0} countOfAll:{1} ", _instance, SingletonMonoBehaviourManager.Instance.GetSingletonCount());
				if (Instance != this)
				{
					DestroyNewMeOrOldInstance(this, Instance);
				}
				SingletonMonoBehaviour.LogFormat(2, "CheckInstance() After. _instance:{0} countOfAll:{1} ", _instance, SingletonMonoBehaviourManager.Instance.GetSingletonCount());
			}
			else
			{
				SingletonMonoBehaviour.LogWarningFormat("Initialization() called 2+ times on the same gameObject. How is that possible?");
			}
			SingletonMonoBehaviour.LogFormat(3, "End of... Singleton<{0}>.Initialization()", this);
		}

		protected virtual void SubclassInitialize()
		{
		}

		public static T InstantiateFromPath(string path)
		{
			GameObject gameObject = SingletonMonoBehaviour.InstantiateGameObjectFromPath(path);
			if (gameObject != null)
			{
				T val = gameObject.GetComponent<T>();
				if (val == null)
				{
					val = gameObject.GetComponentInChildren<T>();
				}
				return val;
			}
			return (T)null;
		}

		public void DestroyNewMeOrOldInstance(SingletonMonoBehaviour<T> newSingleton, SingletonMonoBehaviour<T> oldSingleton)
		{
			bool isSavedInHierarchy = newSingleton.IsSavedInHierarchy;
			bool isSavedInHierarchy2 = oldSingleton.IsSavedInHierarchy;
			SingletonMonoBehaviour.LogFormat(1, "DestroyNewMeOrOldInstance() comparing... New:{0} and Old:{1}", newSingleton, oldSingleton);
			if (!isSavedInHierarchy2)
			{
				SingletonMonoBehaviourManager.Instance.UnregisterAndDestroySingleton(oldSingleton);
				Instance = (T)null;
				Instance = newSingleton as T;
			}
			else if (isSavedInHierarchy)
			{
				bool flag = isSavedInHierarchy && isSavedInHierarchy2;
				SingletonMonoBehaviour.LogFormat(1, "DestroyNewMeOrOldInstance() Both have IsSavedInHierarchy={0} but we choose to destroy the old={1}", flag, oldSingleton);
				SingletonMonoBehaviourManager.Instance.UnregisterAndDestroySingleton(oldSingleton);
				Instance = (T)null;
				Instance = newSingleton as T;
			}
			else
			{
				SingletonMonoBehaviourManager.Instance.UnregisterAndDestroySingleton(newSingleton);
			}
		}

		protected virtual void OnDestroy()
		{
			SingletonMonoBehaviour.LogFormat(1, "{0}.OnDestroy() IsDestroyed={1}", this, base.IsDestroyed);
			if (!base.IsDestroyed)
			{
				base.IsDestroyed = true;
			}
			if (SingletonMonoBehaviourManager.IsInstantiated())
			{
				SingletonMonoBehaviourManager.Instance.UnregisterSingleton(this);
			}
		}

		protected static object Construct(Type type)
		{
			GameObject gameObject = new GameObject(type.Name, type);
			return gameObject.GetComponent(type) as T;
		}

		public static bool IsInstantiated()
		{
			if (_instance != null)
			{
				return true;
			}
			if (SingletonMonoBehaviourManager.Instance.HasSingleton<T>())
			{
				Instance = SingletonMonoBehaviourManager.Instance.FindSingleton<T>();
				return true;
			}
			T[] array = UnityEngine.Object.FindObjectsOfType<T>();
			if (array.Length == 0)
			{
				return false;
			}
			if (array.Length == 1)
			{
				Instance = array[0];
				return true;
			}
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i].IsSavedInHierarchy)
				{
					Instance = array[i];
					return true;
				}
			}
			Instance = array[0];
			return true;
		}

		public static bool HasExecuteInEditModeAttribute()
		{
			return GetExecuteInEditModeAttributes() != null;
		}

		public static ExecuteInEditMode GetExecuteInEditModeAttributes()
		{
			return (ExecuteInEditMode)ReflectionWrapper.GetCustomAttribute(typeof(T), typeof(ExecuteInEditMode));
		}

		public static bool HasPrefabPathAttribute()
		{
			return GetPrefabPathAttribute() != null;
		}

		public static PrefabPathAttribute GetPrefabPathAttribute()
		{
			return (PrefabPathAttribute)ReflectionWrapper.GetCustomAttribute(typeof(T), typeof(PrefabPathAttribute));
		}

		public override string ToString()
		{
			if (this == null || base.gameObject == null || base.IsDestroyed)
			{
				return string.Format("[{0} (IsDestroyed={1})]", GetType().Name, base.IsDestroyed);
			}
			if (_attribute != null)
			{
				return string.Format("[{0} (ID={1}, IsSavedInHierarchy={2}, Path={3})]", GetType().Name, base.ID, IsSavedInHierarchy, _attribute.Path);
			}
			return string.Format("[{0} (ID={1}, IsSavedInHierarchy={2})]", GetType().Name, base.ID, IsSavedInHierarchy);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			T val = obj as T;
			if (val == null)
			{
				return false;
			}
			return val.ID == base.ID;
		}

		public override int GetHashCode()
		{
			return base.ID.GetHashCode();
		}
	}
}
