using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics
{
	public class ClientUpdatedEvent : BaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("device_id")]
		[SerializedSizeLimit(44, false)]
		protected string _deviceID;

		[SerializeField]
		[SerializedAs("sampling_key")]
		protected float _samplingKey;

		public string DeviceID
		{
			get
			{
				return _deviceID;
			}
		}

		public float SamplingKey
		{
			get
			{
				return _samplingKey;
			}
		}

		public override string EventName
		{
			get
			{
				return "client_updated";
			}
		}

		public ClientUpdatedEvent()
		{
			_deviceID = BaseSessionEvent.GetDeviceID();
			_samplingKey = AnalyticsEvent.GetSamplingKey();
			_gameVersion = AnalyticsEvent.GetGameVersion();
		}
	}
}
