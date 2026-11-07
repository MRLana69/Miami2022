using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityUtilities
{
	public class PersistentSingletonMonoBehaviour<TSubclass> : MonoBehaviour where TSubclass : PersistentSingletonMonoBehaviour<TSubclass>
	{
		private static TSubclass _instance;

		private bool _skipOnLevelWasLoaded;

		public static TSubclass instance
		{
			get
			{
				return _instance;
			}
		}

		protected bool instanceExists
		{
			get
			{
				return _instance != null;
			}
		}

		[UsedImplicitly]
		private void Awake()
		{
			if (instanceExists)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			_instance = (TSubclass)this;
			Object.DontDestroyOnLoad(base.gameObject);
			OnPersistentSingletonAwake();
			OnAwakeOrSwitch();
			_skipOnLevelWasLoaded = true;
			SceneManager.sceneLoaded += (Scene scene, LoadSceneMode loadSceneMode) =>
			{
				if (!_skipOnLevelWasLoaded && instance == this)
				{
					OnSceneSwitched();
					OnAwakeOrSwitch();
				}
			};
		}

		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = (TSubclass)null;
				OnPersistentSingletonDestroyed();
			}
		}

		protected virtual void Start()
		{
			_skipOnLevelWasLoaded = false;
		}

		protected virtual void OnPersistentSingletonAwake()
		{
		}

		protected virtual void OnPersistentSingletonDestroyed()
		{
		}

		protected virtual void OnSceneSwitched()
		{
		}

		protected virtual void OnAwakeOrSwitch()
		{
		}
	}
}
