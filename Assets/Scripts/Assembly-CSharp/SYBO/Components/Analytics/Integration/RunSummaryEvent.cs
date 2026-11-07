using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class RunSummaryEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("game_no")]
		public int GameID;

		[SerializeField]
		[SerializedAs("score")]
		public long Score;

		[SerializeField]
		[SerializedAs("headstarts_used")]
		public short HeadStartsUsed;

		[SerializeField]
		[SerializedAs("hoverboards_used")]
		public int HoverboardsUsed;

		[SerializeField]
		[SerializedAs("score_multipliers_collected")]
		public int ScoreMultiplierCollected;

		[SerializeField]
		[SerializedAs("score_multipliers_spawned")]
		public int ScoreMultiplierSpawned;

		[SerializeField]
		[SerializedAs("score_boosters_used")]
		public short ScoreBoosterUsed;

		[SerializeField]
		[SerializedAs("coins_modified")]
		public long CoinsCollected;

		[SerializeField]
		[SerializedAs("coins_collected")]
		public long CoinsCollectedRaw;

		[SerializeField]
		[SerializedAs("coins_spawned")]
		public long CoinsSpawned;

		[SerializeField]
		[SerializedAs("coin_magnets_collected")]
		public int CoinMagnetsCollected;

		[SerializeField]
		[SerializedAs("coin_magnets_spawned")]
		public int CoinMagnetsSpawned;

		[SerializeField]
		[SerializedAs("jetpacks_collected")]
		public int JetpacksCollected;

		[SerializeField]
		[SerializedAs("jetpacks_spawned")]
		public int JetpacksSpawned;

		[SerializeField]
		[SerializedAs("super_sneakers_collected")]
		public int SuperSneakersCollected;

		[SerializeField]
		[SerializedAs("super_sneakers_spawned")]
		public int SuperSneakersSpawned;

		[SerializeField]
		[SerializedAs("mystery_boxes_collected")]
		public int MysteryBoxesCollected;

		[SerializeField]
		[SerializedAs("mystery_boxes_spawned")]
		public int MysteryBoxesSpawned;

		[SerializeField]
		[SerializedAs("super_mystery_boxes_collected")]
		public int SuperMysteryBoxesCollected;

		[SerializeField]
		[SerializedAs("super_mystery_boxes_spawned")]
		public int SuperMysteryBoxesSpawned;

		[SerializeField]
		[SerializedAs("keys_collected")]
		public int KeysCollected;

		[SerializeField]
		[SerializedAs("keys_spawned")]
		public int KeysSpawned;

		[SerializeField]
		[SerializedAs("daily_letters_collected")]
		public int DailyLettersCollected;

		[SerializeField]
		[SerializedAs("daily_letters_spawned")]
		public int DailyLettersSpawned;

		[SerializeField]
		[SerializedAs("char_letters_collected")]
		public int CharacterLettersCollected;

		[SerializeField]
		[SerializedAs("char_letters_spawned")]
		public int CharacterLettersSpawned;

		[SerializeField]
		[SerializedAs("tokens_collected")]
		public int TokensCollected;

		[SerializeField]
		[SerializedAs("tokens_spawned")]
		public int TokensSpawned;

		[SerializeField]
		[SerializedAs("pogo_spawned")]
		public int PogoSticksSpawned;

		[SerializeField]
		[SerializedAs("pogo_collected")]
		public int PogoSticksCollected;

		[SerializeField]
		[SerializedAs("mystery_powerups_spawned")]
		public int MysteryPowerupsSpawned;

		[SerializeField]
		[SerializedAs("mystery_powerup_hoverboards_collected")]
		public int MysteryPowerupHoverboardsCollected;

		[SerializeField]
		[SerializedAs("mystery_powerup_doublecoins_collected")]
		public int MysteryPowerupDoubleCoinsCollected;

		[SerializeField]
		[SerializedAs("mystery_powerup_doublejumps_collected")]
		public int MysteryPowerupDoubleJumpsCollected;

		[SerializeField]
		[SerializedAs("mystery_powerup_plusmultipliers_collected")]
		public int MysteryPowerupPlusMultipliersCollected;

		[SerializeField]
		[SerializedAs("stumbles")]
		public int NumberOfStumbles;

		[SerializeField]
		[SerializedAs("fps")]
		public int Framerate;

		public override string EventName
		{
			get
			{
				return "run_summary";
			}
		}
	}
}
