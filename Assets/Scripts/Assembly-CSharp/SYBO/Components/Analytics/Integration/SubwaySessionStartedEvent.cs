using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class SubwaySessionStartedEvent : SessionStartedEvent, ISubwayBaseEvent
	{
		[SerializeField]
		[SerializedAs("highscore")]
		public long HighScore;

		[SerializeField]
		[SerializedAs("coin_balance")]
		public long CoinBalance;

		[SerializeField]
		[SerializedAs("key_balance")]
		public long KeyBalance;

		public int SessionEventId
		{
			get
			{
				return -1;
			}
		}
	}
}
