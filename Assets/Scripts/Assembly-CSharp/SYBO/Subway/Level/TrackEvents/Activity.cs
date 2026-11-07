using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	public abstract class Activity : MonoBehaviour
	{
		protected bool _initialized;

		protected Transform _transform;

		protected bool _running;

		public bool Running
		{
			get
			{
				return _running;
			}
		}

		private void Start()
		{
			CheckInitialize();
		}

		private void Update()
		{
			if (_running)
			{
				Execute();
			}
		}

		protected virtual void Initialize()
		{
			_transform = base.transform;
			_initialized = true;
		}

		protected void CheckInitialize()
		{
			if (!_initialized)
			{
				Initialize();
			}
		}

		public virtual void Prepare(IActivityInitializer initializer)
		{
			CheckInitialize();
			if (!_running)
			{
			}
		}

		public void Run()
		{
			CheckInitialize();
			_running = true;
		}

		public virtual void Stop()
		{
			_running = false;
		}

		protected abstract void Execute();
	}
}
