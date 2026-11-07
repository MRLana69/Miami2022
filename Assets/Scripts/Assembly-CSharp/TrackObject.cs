using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TrackObject : MonoBehaviour
{
	[SerializeField]
	private bool _debugActivateOnStart;

	private bool _initialized;

	private Transform _transform;

	private bool _active;

	private TrackObject[] _childTrackObjects;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnActivate__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action OnDeactivate__BackingField;

	private bool _propagateActivationStateToChildren;

	public Transform Transform
	{
		get
		{
			InitializeCheck();
			return _transform;
		}
	}

	public bool Active
	{
		get
		{
			return _active;
		}
	}

	public bool PropagateActivationStateToChildren
	{
		get
		{
			return _propagateActivationStateToChildren;
		}
		set
		{
			if (_propagateActivationStateToChildren != value)
			{
				_propagateActivationStateToChildren = value;
				if (_active && _propagateActivationStateToChildren)
				{
					ActivateChildren();
				}
			}
		}
	}

	public event Action OnActivate
	{
		add
		{
			Action action = OnActivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnActivate__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnActivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnActivate__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action OnDeactivate
	{
		add
		{
			Action action = OnDeactivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnDeactivate__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnDeactivate__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnDeactivate__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public void Hack_PropagateActivationStateToChildrenImmediately()
	{
		PropagateActivationStateToChildren = false;
		PropagateActivationStateToChildren = true;
	}

	private void Awake()
	{
		InitializeCheck();
	}

	private void Start()
	{
		if ((Debug.isDebugBuild || Application.isEditor) && _debugActivateOnStart)
		{
			_propagateActivationStateToChildren = true;
			if (base.gameObject.activeInHierarchy && !_active)
			{
				Activate();
			}
		}
	}

	private void InitializeCheck()
	{
		if (!_initialized)
		{
			_transform = base.transform;
			_initialized = true;
		}
	}

	private void ActivateChildren()
	{
		_childTrackObjects = GetComponentsInChildren<TrackObject>(true);
		for (int i = 0; i < _childTrackObjects.Length; i++)
		{
			if (_childTrackObjects[i] != this)
			{
				_childTrackObjects[i].Activate();
			}
		}
	}

	private void DeactivateChildren()
	{
		_childTrackObjects = GetComponentsInChildren<TrackObject>(true);
		for (int i = 0; i < _childTrackObjects.Length; i++)
		{
			if (_childTrackObjects[i] != this)
			{
				_childTrackObjects[i].Deactivate();
			}
		}
	}

	public void Activate()
	{
		if (!_active)
		{
			if (OnActivate__BackingField != null)
			{
				OnActivate__BackingField();
			}
			if (_propagateActivationStateToChildren)
			{
				ActivateChildren();
			}
			_active = true;
		}
	}

	public void Deactivate()
	{
		if (_active)
		{
			_active = false;
			if (OnDeactivate__BackingField != null)
			{
				OnDeactivate__BackingField();
			}
			if (_propagateActivationStateToChildren)
			{
				DeactivateChildren();
			}
		}
	}
}
