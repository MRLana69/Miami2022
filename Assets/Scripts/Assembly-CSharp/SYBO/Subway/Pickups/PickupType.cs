using System;

namespace SYBO.Subway.Pickups
{
	[Flags]
	public enum PickupType
	{
		Key = 1,
		CoinMagnet = 2,
		SuperSneakers = 4,
		JetPack = 8,
		MysteryBox = 0x10,
		MultiplierScore2x = 0x20,
		MysteryPowerup = 0x40,
		LetterHuntCharacter = 0x80,
		SuperMysteryBox = 0x100,
		TokenHunt = 0x200,
		LetterHuntDaily = 0x400,
		PogoStick = 0x800
	}
}
