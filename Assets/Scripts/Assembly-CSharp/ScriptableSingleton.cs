using UnityEngine;

public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
{
	private static T _cachedInstance;

	private static string _fileName
	{
		get
		{
			return typeof(T).Name;
		}
	}

	private static string _resourcePath
	{
		get
		{
			return _fileName;
		}
	}

	public static T instance
	{
		get
		{
			if (_cachedInstance == null)
			{
				_cachedInstance = Resources.Load(_resourcePath) as T;
			}
			if (_cachedInstance == null)
			{
				_cachedInstance = ScriptableObject.CreateInstance<T>();
				_cachedInstance.OnCreate();
			}
			return _cachedInstance;
		}
	}

	protected virtual void OnCreate()
	{
	}
}
