using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class PlayerDiedEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("game_no")]
		public int GameID;

		[SerializeField]
		[SerializedAs("run_time")]
		public int RunTime;

		[SerializeField]
		[SerializedAs("death_cause")]
		[SerializedSizeLimit(36, false)]
		public string DeathCause;

		[SerializeField]
		[SerializedAs("last_obstacle")]
		[SerializedSizeLimit(36, false)]
		public string LastObstacle;

		[SerializeField]
		[SerializedAs("speed")]
		public float Speed;

		[SerializeField]
		[SerializedAs("chunk")]
		[SerializedSizeLimit(72, false)]
		public string Chunk;

		[SerializeField]
		[SerializedAs("chunk_prev")]
		[SerializedSizeLimit(72, false)]
		public string ChunkPrev;

		[SerializeField]
		[SerializedAs("chunk_next")]
		[SerializedSizeLimit(72, false)]
		public string ChunkNext;

		[SerializeField]
		[SerializedAs("position_in_chunk")]
		public float PositionInChunk;

		[SerializeField]
		[SerializedAs("lane")]
		[SerializedSizeLimit(6, false)]
		public string Lane;

		[SerializeField]
		[SerializedAs("max_frame_time_2s")]
		public int MaxFrameTime2s;

		public override string EventName
		{
			get
			{
				return "player_died";
			}
		}
	}
}
