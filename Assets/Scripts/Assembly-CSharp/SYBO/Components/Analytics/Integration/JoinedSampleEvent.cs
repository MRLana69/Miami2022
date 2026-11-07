using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class JoinedSampleEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("source")]
		public readonly int Source;

		[SerializeField]
		[SerializedAs("source_id")]
		public readonly long SourceId;

		[SerializeField]
		[SerializedSizeLimit(8, false)]
		[SerializedAs("aggregated_data_completeness")]
		public readonly string AggregatedDataCompleteness;

		[SerializeField]
		[SerializedAs("days_played")]
		public readonly int DaysPlayed;

		[SerializeField]
		[SerializedAs("session_count")]
		public readonly int SessionCount;

		[SerializeField]
		[SerializedAs("keys_balance")]
		public readonly int KeysBalance;

		[SerializeField]
		[SerializedAs("keys_received")]
		public readonly int KeysReceived;

		[SerializeField]
		[SerializedAs("coins_balance")]
		public readonly int CoinsBalance;

		[SerializeField]
		[SerializedAs("coins_received")]
		public readonly int CoinsReceived;

		[SerializeField]
		[SerializedAs("missing_keys_count")]
		public readonly int MissingKeysCount;

		[SerializeField]
		[SerializedAs("missing_coins_count")]
		public readonly int MissingCoinsCount;

		[SerializeField]
		[SerializedAs("highscore")]
		public readonly int HighScore;

		public override string EventName
		{
			get
			{
				return "joined_sample";
			}
		}

		public JoinedSampleEvent(JoinedSampleSource source, long sourceId, AggregatedDataTracking.DataCompleteness aggregatedDataCompleteness, int daysPlayed, int sessionCount, int keysBalance, int keysReceived, int coinsBalance, int coinsReceived, int missingKeysCount, int missingCoinsCount, int highscore)
		{
			Source = (int)source;
			SourceId = sourceId;
			AggregatedDataCompleteness = aggregatedDataCompleteness.ToString().ToLowerInvariant();
			DaysPlayed = daysPlayed;
			SessionCount = sessionCount;
			KeysBalance = keysBalance;
			KeysReceived = keysReceived;
			CoinsBalance = coinsBalance;
			CoinsReceived = coinsReceived;
			MissingKeysCount = missingKeysCount;
			MissingCoinsCount = missingCoinsCount;
			HighScore = highscore;
		}
	}
}
