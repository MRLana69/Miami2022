using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class GameStartedEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("game_no")]
		public int GameID;

		[SerializeField]
		[SerializedAs("character")]
		[SerializedSizeLimit(72, false)]
		public string Character;

		[SerializeField]
		[SerializedAs("character_outfit")]
		[SerializedSizeLimit(72, false)]
		public string CharacterOutfit;

		[SerializeField]
		[SerializedAs("board")]
		[SerializedSizeLimit(72, false)]
		public string Board;

		[SerializeField]
		[SerializedAs("board_ability_types_added")]
		[SerializedSizeLimit(72, false)]
		public string BoardAbilityTypesAdded;

		[SerializeField]
		[SerializedAs("board_ability_desc_added")]
		[SerializedSizeLimit(72, false)]
		public string BoardAbilityDescriptionsAdded;

		[SerializeField]
		[SerializedAs("player_level")]
		public short PlayerLevel;

		[SerializeField]
		[SerializedAs("highscore")]
		public long HighScore;

		[SerializeField]
		[SerializedAs("coin_balance")]
		public long CoinBalance;

		[SerializeField]
		[SerializedAs("key_balance")]
		public long KeyBalance;

		[SerializeField]
		[SerializedAs("hoverboard_balance")]
		public long HoverboardBalance;

		[SerializeField]
		[SerializedAs("headstart_balance")]
		public long HeadStartBalance;

		[SerializeField]
		[SerializedAs("score_booster_balance")]
		public long ScoreBoosterBalance;

		public override string EventName
		{
			get
			{
				return "game_started";
			}
		}
	}
}
