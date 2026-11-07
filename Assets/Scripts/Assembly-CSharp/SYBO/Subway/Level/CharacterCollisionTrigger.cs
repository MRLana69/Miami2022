using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[RequireComponent(typeof(Collider))]
	public class CharacterCollisionTrigger : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler OnCharacterEnter__BackingField;

		public event EventHandler OnCharacterEnter
		{
			add
			{
				EventHandler eventHandler = OnCharacterEnter__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEnter__BackingField, (EventHandler)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler eventHandler = OnCharacterEnter__BackingField;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnCharacterEnter__BackingField, (EventHandler)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (OnCharacterEnter__BackingField != null && other.CompareTag("Player"))
			{
				OnCharacterEnter__BackingField(this, EventArgs.Empty);
			}
		}
	}
}
