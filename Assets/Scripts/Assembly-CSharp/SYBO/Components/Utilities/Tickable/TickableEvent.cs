using System;
using UnityEngine.Events;

namespace SYBO.Components.Utilities.Tickable
{
	[Serializable]
	public class TickableEvent : UnityEvent<ITickable>
	{
	}
}
