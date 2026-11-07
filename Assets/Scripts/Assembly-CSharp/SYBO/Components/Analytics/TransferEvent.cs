using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics
{
	public class TransferEvent<T> where T : AnalyticsEvent
	{
		[SerializeField]
		private string name;

		[SerializeField]
		[SerializedAs("event")]
		private T eventData;

		public TransferEvent(T eventData, string name)
		{
			this.name = name;
			this.eventData = eventData;
		}

		public T GetEventData()
		{
			return eventData;
		}
	}
}
