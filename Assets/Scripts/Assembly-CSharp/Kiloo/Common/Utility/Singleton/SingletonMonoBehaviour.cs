using UnityEngine;

namespace Kiloo.Common.Utility.Singleton
{
	public class SingletonMonoBehaviour<TSubclass> : MonoBehaviour where TSubclass : SingletonMonoBehaviour<TSubclass>
	{
		private static TSubclass _instance;

		public static TSubclass instance
		{
			get
			{
				if (_instance == null)
				{
					UpdateInstance(false);
				}
				return _instance;
			}
		}

		public static TSubclass instanceOptional
		{
			get
			{
				if (_instance == null)
				{
					UpdateInstance(true);
				}
				return _instance;
			}
		}

		private static void UpdateInstance(bool optional)
		{
			TSubclass[] array = Object.FindObjectsOfType<TSubclass>();
			if (array.Length == 1)
			{
				_instance = array[0];
				return;
			}
			if (array.Length == 0)
			{
				if (!optional)
				{
					SDebug.LogError("Requested singleton of type " + typeof(TSubclass).Name + " not found.");
				}
				return;
			}
			SDebug.LogError("Requested singleton of type " + typeof(TSubclass).Name + " has " + array.Length + "instances.");
		}

		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = (TSubclass)null;
			}
		}
	}
}
