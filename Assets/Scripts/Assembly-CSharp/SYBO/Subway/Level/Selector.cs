using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace SYBO.Subway.Level
{
	public class Selector : MonoBehaviour
	{
		private bool _initialized;

		protected Transform _transform;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<List<Selector>> PerformingSelection__BackingField;

		public bool Initialized
		{
			get
			{
				return _initialized;
			}
		}

		internal event Action<List<Selector>> PerformingSelection
		{
			add
			{
				Action<List<Selector>> action = PerformingSelection__BackingField;
				Action<List<Selector>> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref PerformingSelection__BackingField, (Action<List<Selector>>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<List<Selector>> action = PerformingSelection__BackingField;
				Action<List<Selector>> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref PerformingSelection__BackingField, (Action<List<Selector>>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected virtual void Awake()
		{
			if (!_initialized)
			{
				Initialize();
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < _transform.childCount; i++)
			{
				if (_transform.GetChild(i) != null)
				{
					_transform.GetChild(i).gameObject.SetActive(false);
				}
			}
		}

		protected virtual void Start()
		{
		}

		protected virtual void Initialize()
		{
			_transform = base.transform;
			for (int i = 0; i < _transform.childCount; i++)
			{
				if (_transform.GetChild(i) != null)
				{
					_transform.GetChild(i).gameObject.SetActive(false);
				}
			}
			_initialized = true;
		}

		public virtual void PerformSelection(List<GameObject> objectsToVisit)
		{
			InitializeCheck();
		}

		public void PerformSelection(List<GameObject> objectsToVisit, List<Selector> subSelectors)
		{
			PerformSelection(objectsToVisit);
			if (PerformingSelection__BackingField != null)
			{
				PerformingSelection__BackingField(subSelectors);
			}
		}

		protected void InitializeCheck()
		{
			if (!_initialized)
			{
				Initialize();
			}
		}
	}
}
