using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CharacterPassTrigger : TrackEventTrigger
{
	private Transform _transform;

	private BoxCollider _collider;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private EventHandler Triggered__BackingField;

	public float EndPosition
	{
		get
		{
			return _transform.position.z + _collider.center.z + 0.5f * _collider.size.z;
		}
	}

	public event EventHandler Triggered
	{
		add
		{
			EventHandler eventHandler = Triggered__BackingField;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref Triggered__BackingField, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
		remove
		{
			EventHandler eventHandler = Triggered__BackingField;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref Triggered__BackingField, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	private void Awake()
	{
		_transform = base.transform;
		_collider = GetComponent<Collider>() as BoxCollider;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && Triggered__BackingField != null)
		{
			Triggered__BackingField(this, EventArgs.Empty);
		}
	}

	protected void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = GetComponent<Collider>() as BoxCollider;
		Gizmos.DrawWireCube(base.transform.position + boxCollider.center, boxCollider.size);
	}
}
