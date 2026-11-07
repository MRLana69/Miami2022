using System;
using System.Reflection;
using SYBO.Components.Utilities;

namespace SYBO.Components.Singletons
{
	public abstract class Singleton<T> where T : Singleton<T>
	{
		private static object _lockObject = new object();

		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					Type typeFromHandle = typeof(T);
					ConstructorInfo[] constructors = ReflectionWrapper.GetConstructors(typeFromHandle, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (constructors.Length != 1)
					{
						throw new SingletonException(string.Format("Singleton {0} has an invalid number of constructors!\nSingletons must have exactly one constructor, it must be private and without parameters.", typeFromHandle.Name));
					}
					if (constructors[0].IsPublic)
					{
						throw new SingletonException(string.Format("Singleton {0} has a public constructor!\nSingletons must have exactly one constructor, it must be private and without parameters.", typeFromHandle.Name));
					}
					if (constructors[0].GetParameters().Length != 0)
					{
						throw new SingletonException(string.Format("Singleton {0} constructor has parameters!\nSingletons must have exactly one constructor, it must be private and without parameters.", typeFromHandle.Name));
					}
					lock (_lockObject)
					{
						if (_instance == null)
						{
							_instance = constructors[0].Invoke(null) as T;
							_instance.Initialize();
						}
					}
				}
				return _instance;
			}
		}

		public Singleton()
		{
		}

		public static T GetInstance()
		{
			return Instance;
		}

		public virtual void Initialize()
		{
		}
	}
}
